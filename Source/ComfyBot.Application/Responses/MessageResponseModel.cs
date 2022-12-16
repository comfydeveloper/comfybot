namespace ComfyBot.Application.Responses
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Shared;
    using Shared.Extensions;

    public class MessageResponseModel : NotifyingModel
    {
        private int timeout = 60;
        private int priority;
        private bool replyAlways;

        public MessageResponseModel()
        {
            AddUserCommand = new DelegateCommand(AddUser);
            AddLooseKeywordCommand = new DelegateCommand(AddLooseKeyword);
            AddAllKeywordCommand = new DelegateCommand(AddAllKeyword);
            AddExactKeywordCommand = new DelegateCommand(AddExactKeyword);
            AddReplyCommand = new DelegateCommand(AddReply);
            RemoveUserCommand = new ParameterCommand(RemoveUser);
            RemoveLooseKeywordCommand = new ParameterCommand(RemoveLooseKeyword);
            RemoveAllKeywordCommand = new ParameterCommand(RemoveAllKeyword);
            RemoveExactKeywordCommand = new ParameterCommand(RemoveExactKeyword);
            RemoveReplyCommand = new ParameterCommand(RemoveReply);

            Users.RegisterCollectionItemChanged(OnTextModelUpdate);
            LooseKeywords.RegisterCollectionItemChanged(OnTextModelUpdate);
            AllKeywords.RegisterCollectionItemChanged(OnTextModelUpdate);
            ExactKeywords.RegisterCollectionItemChanged(OnTextModelUpdate);
            Replies.RegisterCollectionItemChanged(OnTextModelUpdate);
        }

        public DelegateCommand AddUserCommand { get; }

        public DelegateCommand AddLooseKeywordCommand { get; }

        public DelegateCommand AddAllKeywordCommand { get; }

        public DelegateCommand AddExactKeywordCommand { get; }

        public DelegateCommand AddReplyCommand { get; }

        public ParameterCommand RemoveUserCommand { get; }

        public ParameterCommand RemoveLooseKeywordCommand { get; }

        public ParameterCommand RemoveAllKeywordCommand { get; }

        public ParameterCommand RemoveExactKeywordCommand { get; }

        public ParameterCommand RemoveReplyCommand { get; }

        public string Id { get; set; }

        public ObservableCollection<TextModel> Users { get; set; } = new();

        public ObservableCollection<TextModel> ExactKeywords { get; set; } = new();

        public ObservableCollection<TextModel> LooseKeywords { get; set; } = new();

        public ObservableCollection<TextModel> AllKeywords { get; set; } = new();

        public ObservableCollection<TextModel> Replies { get; set; } = new();

        public int Timeout
        {
            get => timeout;
            set { timeout = value; OnPropertyChanged(); }
        }

        public bool ReplyAlways 
        { 
            get => replyAlways; 
            set { replyAlways = value; OnPropertyChanged(); } 
        }

        public int Priority
        {
            get => priority;
            set { priority = value; OnPropertyChanged(); }
        }

        private void AddUser()
        {
            Users.Add(new TextModel());
        }

        private void AddLooseKeyword()
        {
            LooseKeywords.Add(new TextModel());
        }

        private void AddAllKeyword()
        {
            AllKeywords.Add(new TextModel());
        }

        private void AddExactKeyword()
        {
            ExactKeywords.Add(new TextModel());
        }

        private void AddReply()
        {
            Replies.Add(new TextModel());
        }

        private void RemoveUser(object parameter)
        {
            Users.Remove((TextModel)parameter);
            OnPropertyChanged();
        }

        private void RemoveLooseKeyword(object parameter)
        {
            LooseKeywords.Remove((TextModel)parameter);
            OnPropertyChanged();
        }

        private void RemoveAllKeyword(object parameter)
        {
            AllKeywords.Remove((TextModel)parameter);
            OnPropertyChanged();
        }

        private void RemoveExactKeyword(object parameter)
        {
            ExactKeywords.Remove((TextModel)parameter);
            OnPropertyChanged();
        }

        private void RemoveReply(object parameter)
        {
            Replies.Remove((TextModel)parameter);
            OnPropertyChanged();
        }

        private void OnTextModelUpdate(object sender, PropertyChangedEventArgs e)
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