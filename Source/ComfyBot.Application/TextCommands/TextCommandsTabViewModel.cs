using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Features.TextCommands;
using ComfyBot.Application.Shared;
using ComfyBot.Application.Shared.Extensions;
using ComfyBot.Application.Shared.Services;
using ComfyBot.Application.Shared.Wrappers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ComfyBot.Application.TextCommands;

public class TextCommandsTabViewModel : InitializableTab
{
    private readonly IScopedServiceProvider provider;
    private readonly IMessageBox messageBox;
    private string searchText;

    public TextCommandsTabViewModel(IScopedServiceProvider provider, IMessageBox messageBox)
    {
        this.provider = provider;
        this.messageBox = messageBox;

        this.AddTextCommandCommand = new DelegateCommand(this.AddTextCommand);
        this.RemoveTextCommandCommand = new ParameterCommand(this.RemoveTextCommand);
    }

    public DelegateCommand AddTextCommandCommand { get; }

    public ParameterCommand RemoveTextCommandCommand { get; set; }

    public ObservableCollection<TextCommandModel> Commands { get; set; } = [];

    protected override void Initialize()
    {
        using var getHandler = this.provider.Create<IQueryHandler<GetCommands.Query, GetCommands.Result>>();
        var outcome = getHandler.Service.Handle(new GetCommands.Query()).Result;

        if (!outcome.IsSuccess)
        {
            return;
        }

        foreach (GetCommands.TextCommandEntry entry in outcome.Payload.Entries)
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

        this.Commands.RegisterCollectionItemChanged(this.OnCommandUpdate);
    }

    private void AddTextCommand()
    {
        Guid id = Guid.NewGuid();
        this.Commands.Add(new TextCommandModel { Id = id.ToString() });

        using var addHandler = this.provider.Create<ICommandHandler<AddCommand.Command>>();
        addHandler.Service.Handle(new AddCommand.Command(id)).Wait();
    }

    private void OnCommandUpdate(object sender, PropertyChangedEventArgs e)
    {
        TextCommandModel model = (TextCommandModel)sender;

        UpdateCommand.Command command = new(
            Guid.Parse(model.Id),
            model.Timeout,
            model.Commands.ToStrings(),
            model.Replies.ToStrings());

        using var updateHandler = this.provider.Create<ICommandHandler<UpdateCommand.Command>>();
        updateHandler.Service.Handle(command).Wait();
    }

    private void RemoveTextCommand(object parameter)
    {
        TextCommandModel model = (TextCommandModel)parameter;

        if (this.messageBox.Show(GetDeletionMessage(model), "Delete command", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            this.Commands.Remove(model);
            using var removeHandler = this.provider.Create<ICommandHandler<RemoveCommand.Command>>();
            removeHandler.Service.Handle(new RemoveCommand.Command(Guid.Parse(model.Id))).Wait();
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