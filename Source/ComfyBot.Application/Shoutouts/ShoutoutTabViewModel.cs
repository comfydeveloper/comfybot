namespace ComfyBot.Application.Shoutouts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using ComfyBot.Application.Shared;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    public class ShoutoutTabViewModel : InitializableTab
    {
        private readonly IRepository<Shoutout> repository;

        public ShoutoutTabViewModel(IRepository<Shoutout> repository)
        {
            this.repository = repository;

            this.AddShoutoutCommand = new DelegateCommand(this.AddShoutout);
            this.RemoveShoutoutCommand = new ParameterCommand(this.RemoveShoutout);
        }

        public DelegateCommand AddShoutoutCommand { get; }

        public ParameterCommand RemoveShoutoutCommand { get; }

        public ObservableCollection<ShoutoutModel> Shoutouts { get; set; } = new ObservableCollection<ShoutoutModel>();

        protected override void Initialize()
        {
            this.Shoutouts.Clear();
            IEnumerable<Shoutout> allShoutouts = this.repository.GetAll();

            foreach (Shoutout shoutout in allShoutouts)
            {
                this.Shoutouts.Add(new ShoutoutModel { Id = shoutout.Id, Command = shoutout.Command, Message = shoutout.Message });
            }

            this.Shoutouts.CollectionChanged += this.OnCollectionChanged;

            foreach (ShoutoutModel shoutout in this.Shoutouts)
            {
                shoutout.PropertyChanged += this.OnShoutoutChanged;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ShoutoutModel newItem in e.NewItems)
                {
                    newItem.PropertyChanged += this.OnShoutoutChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ShoutoutModel oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= this.OnShoutoutChanged;
                }
            }
        }

        private void OnShoutoutChanged(object sender, PropertyChangedEventArgs e)
        {
            ShoutoutModel model = (ShoutoutModel)sender;

            if (string.IsNullOrEmpty(model.Command) || string.IsNullOrEmpty(model.Message))
            {
                return;
            }

            Shoutout shoutout = new Shoutout
                                {
                                    Id = model.Id,
                                    Command = model.Command,
                                    Message = model.Message
                                };

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