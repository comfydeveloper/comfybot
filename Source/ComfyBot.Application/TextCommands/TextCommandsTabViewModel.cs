using ComfyBot.Application.Shared.Wrappers;

namespace ComfyBot.Application.TextCommands;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Shared;
using Shared.Contracts;
using Shared.Extensions;
using Data.Models;
using Data.Repositories;

public class TextCommandsTabViewModel : InitializableTab
{
    private readonly IRepository<TextCommand> repository;
    private readonly IMapper<TextCommand, TextCommandModel> mapper;
    private readonly IMessageBox messageBox;
    private string searchText;

    public TextCommandsTabViewModel(IRepository<TextCommand> repository,
        IMapper<TextCommand, TextCommandModel> mapper,
        IMessageBox messageBox)
    {
        this.repository = repository;
        this.mapper = mapper;
        this.messageBox = messageBox;

        AddTextCommandCommand = new DelegateCommand(AddTextCommand);
        RemoveTextCommandCommand = new ParameterCommand(RemoveTextCommand);
    }

    public DelegateCommand AddTextCommandCommand { get; }

    public ParameterCommand RemoveTextCommandCommand { get; set; }

    public ObservableCollection<TextCommandModel> Commands { get; set; } = new();

    protected override void Initialize()
    {
        IEnumerable<TextCommand> textCommands = repository.GetAll().OrderBy(c => c.Commands.OrderBy(text => text).FirstOrDefault());

        foreach (TextCommand entity in textCommands)
        {
            TextCommandModel model = new TextCommandModel();
            mapper.MapToModel(entity, model);
            Commands.Add(model);
        }

        Commands.RegisterCollectionItemChanged(OnResponseUpdate);
    }

    private void AddTextCommand()
    {
        Commands.Add(new TextCommandModel { Id = Guid.NewGuid().ToString() });
    }

    private void RemoveTextCommand(object parameter)
    {
        TextCommandModel model = (TextCommandModel)parameter;

        if (messageBox.Show(GetDeletionMessage(model),
                "Delete command",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            Commands.Remove(model);
            repository.Remove(model.Id);
        }
    }

    private static string GetDeletionMessage(TextCommandModel model)
    {
        if (model.Commands.Any())
        {
            return $"Do you want to delete the command \"{string.Join(", ", model.Commands.Select(c => c.Text))}\"?";
        }
        return "Do you want to delete the command?";
    }

    private void OnResponseUpdate(object sender, PropertyChangedEventArgs e)
    {
        TextCommandModel model = (TextCommandModel)sender;
        TextCommand entity = new TextCommand();
        mapper.MapToEntity(model, entity);

        repository.Write(entity);
    }

    [ExcludeFromCodeCoverage]
    public string SearchText
    {
        get => searchText;
        set { searchText = value; UpdateSearch(); }
    }

    [ExcludeFromCodeCoverage]
    private void UpdateSearch()
    {
        ICollectionView collectionView = CollectionViewSource.GetDefaultView(Commands);

        if (string.IsNullOrEmpty(SearchText))
        {
            collectionView.Filter = o => true;
        }
        else
        {
            collectionView.Filter = o =>
            {
                TextCommandModel response = (TextCommandModel)o;

                return response.Commands.Any(k => k.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                       || response.Replies.Any(k => k.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            };
        }

        collectionView.Refresh();
    }
}