namespace ComfyBot.Application.Shoutouts
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

    public class ShoutoutTabViewModel : InitializableTab
    {
        private readonly IRepository<Shoutout> repository;
        private readonly IMapper<Shoutout, ShoutoutModel> mapper;

        public ShoutoutTabViewModel(IRepository<Shoutout> repository,
                                    IMapper<Shoutout, ShoutoutModel> mapper)
        {
            this.repository = repository;
            this.mapper = mapper;

            this.AddShoutoutCommand = new DelegateCommand(this.AddShoutout);
            this.RemoveShoutoutCommand = new ParameterCommand(this.RemoveShoutout);
        }

        public DelegateCommand AddShoutoutCommand { get; }

        public ParameterCommand RemoveShoutoutCommand { get; }

        public ObservableCollection<ShoutoutModel> Shoutouts { get; set; } = new ObservableCollection<ShoutoutModel>();

        protected override void Initialize()
        {
            IEnumerable<Shoutout> allShoutouts = this.repository.GetAll();

            foreach (Shoutout shoutout in allShoutouts)
            {
                ShoutoutModel model = new ShoutoutModel();
                this.mapper.MapToModel(shoutout, model);
                this.Shoutouts.Add(model);
            }

            this.Shoutouts.RegisterCollectionItemChanged(this.OnShoutoutChanged);
        }

        private void OnShoutoutChanged(object sender, PropertyChangedEventArgs e)
        {
            ShoutoutModel model = (ShoutoutModel)sender;

            if (string.IsNullOrEmpty(model.Command) || string.IsNullOrEmpty(model.Message))
            {
                return;
            }

            Shoutout shoutout = new Shoutout();
            this.mapper.MapToEntity(model, shoutout);

            this.repository.AddOrUpdate(shoutout);
        }

        private void AddShoutout()
        {
            ShoutoutModel shoutoutModel = new ShoutoutModel { Id = Guid.NewGuid().ToString() };
            this.Shoutouts.Add(shoutoutModel);
        }

        private void RemoveShoutout(object shoutout)
        {
            ShoutoutModel model = (ShoutoutModel)shoutout;
            this.Shoutouts.Remove(model);
            this.repository.Remove(model.Id);
        }
    }
}