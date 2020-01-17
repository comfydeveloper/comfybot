namespace ComfyBot.Application.TextCommands
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using ComfyBot.Application.Shared;
    using ComfyBot.Application.Shared.Extensions;

    public class TextCommandModel : NotifyingModel
    {
        private string command;
        private int timeout = 30;

        public TextCommandModel()
        {
            this.AddReplyCommand = new DelegateCommand(this.AddReply);
            this.RemoveReplyCommand = new ParameterCommand(this.RemoveReply);

            this.Replies.RegisterCollectionItemChanged(this.OnReplyUpdate);
        }

        public string Id { get; set; }

        public string Command
        {
            get => this.command;
            set { this.command = value; this.OnPropertyChanged(); }
        }

        public ObservableCollection<TextModel> Replies { get; set; } = new ObservableCollection<TextModel>();

        public int Timeout
        {
            get => this.timeout;
            set { this.timeout = value; this.OnPropertyChanged(); }
        }

        public DelegateCommand AddReplyCommand { get; }

        public ParameterCommand RemoveReplyCommand { get; }

        private void AddReply()
        {
            this.Replies.Add(new TextModel());
        }

        private void RemoveReply(object parameter)
        {
            this.Replies.Remove((TextModel)parameter);
            this.OnPropertyChanged();
        }

        private void OnReplyUpdate(object sender, PropertyChangedEventArgs e)
        {
            TextModel model = (TextModel)sender;

            if (string.IsNullOrEmpty(model.Text))
            {
                return;
            }
            this.OnPropertyChanged();
        }
    }
}