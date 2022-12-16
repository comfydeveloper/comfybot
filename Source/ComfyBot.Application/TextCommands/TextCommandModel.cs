namespace ComfyBot.Application.TextCommands
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Shared;
    using Shared.Extensions;

    public class TextCommandModel : NotifyingModel
    {
        private int timeout = 30;

        public TextCommandModel()
        {
            AddTextCommand = new DelegateCommand(AddText);
            AddReplyCommand = new DelegateCommand(AddReply);
            RemoveTextCommand = new ParameterCommand(RemoveText);
            RemoveReplyCommand = new ParameterCommand(RemoveReply);

            Replies.RegisterCollectionItemChanged(OnReplyUpdate);
            Commands.RegisterCollectionItemChanged(OnReplyUpdate);
        }

        public string Id { get; set; }

        public ObservableCollection<TextModel> Replies { get; set; } = new();

        public ObservableCollection<TextModel> Commands { get; set; } = new();

        public int Timeout
        {
            get => timeout;
            set { timeout = value; OnPropertyChanged(); }
        }

        public DelegateCommand AddReplyCommand { get; }

        public ParameterCommand RemoveReplyCommand { get; }

        public DelegateCommand AddTextCommand { get; }

        public ParameterCommand RemoveTextCommand { get; }

        private void AddText()
        {
            Commands.Add(new TextModel());
        }

        private void RemoveText(object parameter)
        {
            Commands.Remove((TextModel)parameter);
            OnPropertyChanged();
        }

        private void AddReply()
        {
            Replies.Add(new TextModel());
        }

        private void RemoveReply(object parameter)
        {
            Replies.Remove((TextModel)parameter);
            OnPropertyChanged();
        }

        private void OnReplyUpdate(object sender, PropertyChangedEventArgs e)
        {
            TextModel model = (TextModel)sender;

            if (string.IsNullOrEmpty(model.Text))
            {
                return;
            }
            OnPropertyChanged();
        }
    }
}