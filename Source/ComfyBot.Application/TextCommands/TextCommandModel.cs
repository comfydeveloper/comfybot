namespace ComfyBot.Application.TextCommands
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using ComfyBot.Application.Shared;
    using ComfyBot.Application.Shared.Extensions;

    public class TextCommandModel : NotifyingModel
    {
        private int timeout = 30;

        public TextCommandModel()
        {
            this.AddTextCommand = new DelegateCommand(this.AddText);
            this.AddReplyCommand = new DelegateCommand(this.AddReply);
            this.RemoveTextCommand = new ParameterCommand(this.RemoveText);
            this.RemoveReplyCommand = new ParameterCommand(this.RemoveReply);

            this.Replies.RegisterCollectionItemChanged(this.OnReplyUpdate);
            this.Commands.RegisterCollectionItemChanged(this.OnReplyUpdate);
        }

        public string Id { get; set; }

        public ObservableCollection<TextModel> Replies { get; set; } = new();

        public ObservableCollection<TextModel> Commands { get; set; } = new();

        public int Timeout
        {
            get => this.timeout;
            set { this.timeout = value; this.OnPropertyChanged(); }
        }

        public DelegateCommand AddReplyCommand { get; }

        public ParameterCommand RemoveReplyCommand { get; }

        public DelegateCommand AddTextCommand { get; }

        public ParameterCommand RemoveTextCommand { get; }

        private void AddText()
        {
            this.Commands.Add(new TextModel());
        }

        private void RemoveText(object parameter)
        {
            this.Commands.Remove((TextModel)parameter);
            this.OnPropertyChanged();
        }

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