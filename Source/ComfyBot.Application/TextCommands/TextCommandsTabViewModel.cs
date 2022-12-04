namespace ComfyBot.Application.TextCommands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using ComfyBot.Application.Shared;
    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Application.Shared.Extensions;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    public class TextCommandsTabViewModel : InitializableTab
    {
        private readonly IRepository<TextCommand> repository;
        private readonly IMapper<TextCommand, TextCommandModel> mapper;
        private string searchText;

        public TextCommandsTabViewModel(IRepository<TextCommand> repository,
                                        IMapper<TextCommand, TextCommandModel> mapper)
        {
            this.repository = repository;
            this.mapper = mapper;

            this.AddTextCommandCommand = new DelegateCommand(this.AddTextCommand);
            this.RemoveTextCommandCommand = new ParameterCommand(this.RemoveTextCommand);
        }

        public DelegateCommand AddTextCommandCommand { get; }

        public ParameterCommand RemoveTextCommandCommand { get; set; }

        public ObservableCollection<TextCommandModel> Commands { get; set; } = new ObservableCollection<TextCommandModel>();

        protected override void Initialize()
        {
            IEnumerable<TextCommand> textCommands = this.repository.GetAll().OrderBy(c => c.Commands.OrderBy(text => text).FirstOrDefault());

            foreach (TextCommand entity in textCommands)
            {
                TextCommandModel model = new TextCommandModel();
                this.mapper.MapToModel(entity, model);
                this.Commands.Add(model);
            }

            this.Commands.RegisterCollectionItemChanged(this.OnResponseUpdate);
        }

        private void AddTextCommand()
        {
            this.Commands.Add(new TextCommandModel { Id = Guid.NewGuid().ToString() });
        }

        private void RemoveTextCommand(object parameter)
        {
            TextCommandModel model = (TextCommandModel)parameter;

            if (MessageBox.Show(GetDeletionMessage(model),
                               "Delete command",
                               MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Commands.Remove(model);
                this.repository.Remove(model.Id);
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
            this.mapper.MapToEntity(model, entity);

            this.repository.AddOrUpdate(entity);
        }

        [ExcludeFromCodeCoverage]
        public string SearchText
        {
            get => this.searchText;
            set { this.searchText = value; this.UpdateSearch(); }
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
}