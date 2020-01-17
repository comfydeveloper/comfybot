namespace ComfyBot.Application.TextCommands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using ComfyBot.Application.Shared;
    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Application.Shared.Extensions;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    public class TextCommandsTabViewModel : InitializableTab
    {
        private readonly IRepository<TextCommand> repository;
        private readonly IMapper<TextCommand, TextCommandModel> mapper;

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
            IEnumerable<TextCommand> textCommands = this.repository.GetAll();

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

            this.Commands.Remove(model);
            this.repository.Remove(model.Id);
        }

        private void OnResponseUpdate(object sender, PropertyChangedEventArgs e)
        {
            TextCommandModel model = (TextCommandModel)sender;
            TextCommand entity = new TextCommand();
            this.mapper.MapToEntity(model, entity);

            this.repository.AddOrUpdate(entity);
        }
    }
}