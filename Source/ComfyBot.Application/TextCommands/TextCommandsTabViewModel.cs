using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Features.TextCommands;
using ComfyBot.Application.Shared;
using ComfyBot.Application.Shared.Extensions;
using ComfyBot.Application.Shared.Wrappers;

namespace ComfyBot.Application.TextCommands;

public class TextCommandsTabViewModel : InitializableTab
{
    private readonly IQueryHandler<GetCommands.Query, GetCommands.Result> getHandler;
    private readonly ICommandHandler<AddCommand.Command> addHandler;
    private readonly ICommandHandler<UpdateCommand.Command> updateHandler;
    private readonly ICommandHandler<RemoveCommand.Command> removeHandler;
    private readonly IMessageBox messageBox;
    private string searchText;

    public TextCommandsTabViewModel(
        IQueryHandler<GetCommands.Query, GetCommands.Result> getHandler,
        ICommandHandler<AddCommand.Command> addHandler,
        ICommandHandler<UpdateCommand.Command> updateHandler,
        ICommandHandler<RemoveCommand.Command> removeHandler,
        IMessageBox messageBox)
    {
        this.getHandler = getHandler;
        this.addHandler = addHandler;
        this.updateHandler = updateHandler;
        this.removeHandler = removeHandler;
        this.messageBox = messageBox;

        this.AddTextCommandCommand = new DelegateCommand(this.AddTextCommand);
        this.RemoveTextCommandCommand = new ParameterCommand(this.RemoveTextCommand);
    }

    public DelegateCommand AddTextCommandCommand { get; }

    public ParameterCommand RemoveTextCommandCommand { get; set; }

    public ObservableCollection<TextCommandModel> Commands { get; set; } = [];

    protected override void Initialize()
    {
        GetCommands.Result result = this.getHandler.Handle(new GetCommands.Query()).Result;

        foreach (GetCommands.TextCommandEntry entry in result.Entries)
        {
            TextCommandModel model = new()
            {
                Id = entry.Id.ToString(),
                Timeout = entry.TimeoutInSeconds,
            };

            model.Commands.AddRange(entry.Commands.ToTextModels().OrderBy(m => m.Text));
            model.Replies.AddRange(entry.Replies.ToTextModels().OrderBy(m => m.Text));

            this.Commands.Add(model);
        }

        this.Commands.RegisterCollectionItemChanged(this.OnResponseUpdate);
    }

    private void AddTextCommand()
    {
        Guid id = Guid.NewGuid();
        this.Commands.Add(new TextCommandModel { Id = id.ToString() });

        this.addHandler.Handle(new AddCommand.Command(id));
    }

    private void OnResponseUpdate(object sender, PropertyChangedEventArgs e)
    {
        TextCommandModel model = (TextCommandModel)sender;

        UpdateCommand.Command command = new(
            Guid.Parse(model.Id),
            model.Timeout,
            model.Commands.ToStrings(),
            model.Replies.ToStrings());

        this.updateHandler.Handle(command);
    }

    private void RemoveTextCommand(object parameter)
    {
        TextCommandModel model = (TextCommandModel)parameter;

        if (this.messageBox.Show(GetDeletionMessage(model), "Delete command", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            this.Commands.Remove(model);
            this.removeHandler.Handle(new RemoveCommand.Command(Guid.Parse(model.Id)));
        }
    }

    private static string GetDeletionMessage(TextCommandModel model)
    {
        if (model.Commands.Any())
        {
            return $"Do you want to delete the command [{string.Join(", ", model.Commands.Select(c => c.Text))}]?";
        }
        return "Do you want to delete the command?";
    }

    [ExcludeFromCodeCoverage]
    public string SearchText
    {
        get => this.searchText;
        set {
            this.searchText = value;
            this.UpdateSearch(); }
    }

    [ExcludeFromCodeCoverage]
    private void UpdateSearch()
    {
        ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Commands);

        if (string.IsNullOrEmpty(this.SearchText))
        {
            collectionView.Filter = o => true;
        }
        else
        {
            collectionView.Filter = o =>
            {
                TextCommandModel response = (TextCommandModel)o;

                return response.Commands.Any(k => k.Text.Contains(this.searchText, StringComparison.OrdinalIgnoreCase))
                       || response.Replies.Any(k => k.Text.Contains(this.searchText, StringComparison.OrdinalIgnoreCase));
            };
        }

        collectionView.Refresh();
    }
}