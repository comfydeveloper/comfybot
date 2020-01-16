namespace ComfyBot.Application.Responses
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using ComfyBot.Application.Shared;
    using ComfyBot.Application.Shared.Extensions;

    public class MessageResponseModel : NotifyingModel
    {
        private int timeout = 60;

        public MessageResponseModel()
        {
            this.AddUserCommand = new DelegateCommand(this.AddUser);
            this.AddLooseKeywordCommand = new DelegateCommand(this.AddLooseKeyword);
            this.AddAllKeywordCommand = new DelegateCommand(this.AddAllKeyword);
            this.AddExactKeywordCommand = new DelegateCommand(this.AddExactKeyword);
            this.AddReplyCommand = new DelegateCommand(this.AddReply);
            this.RemoveUserCommand = new ParameterCommand(this.RemoveUser);
            this.RemoveLooseKeywordCommand = new ParameterCommand(this.RemoveLooseKeyword);
            this.RemoveAllKeywordCommand = new ParameterCommand(this.RemoveAllKeyword);
            this.RemoveExactKeywordCommand = new ParameterCommand(this.RemoveExactKeyword);
            this.RemoveReplyCommand = new ParameterCommand(this.RemoveReply);

            this.Users.RegisterCollectionItemChanged(this.OnTextModelUpdate);
            this.LooseKeywords.RegisterCollectionItemChanged(this.OnTextModelUpdate);
            this.AllKeywords.RegisterCollectionItemChanged(this.OnTextModelUpdate);
            this.ExactKeywords.RegisterCollectionItemChanged(this.OnTextModelUpdate);
            this.Replies.RegisterCollectionItemChanged(this.OnTextModelUpdate);
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

        public ObservableCollection<TextModel> Users { get; set; } = new ObservableCollection<TextModel>();

        public ObservableCollection<TextModel> ExactKeywords { get; set; } = new ObservableCollection<TextModel>();

        public ObservableCollection<TextModel> LooseKeywords { get; set; } = new ObservableCollection<TextModel>();

        public ObservableCollection<TextModel> AllKeywords { get; set; } = new ObservableCollection<TextModel>();

        public ObservableCollection<TextModel> Replies { get; set; } = new ObservableCollection<TextModel>();

        public int Timeout
        {
            get => this.timeout;
            set { this.timeout = value; this.OnPropertyChanged(); }
        }

        private void AddUser()
        {
            this.Users.Add(new TextModel());
        }

        private void AddLooseKeyword()
        {
            this.LooseKeywords.Add(new TextModel());
        }

        private void AddAllKeyword()
        {
            this.AllKeywords.Add(new TextModel());
        }

        private void AddExactKeyword()
        {
            this.ExactKeywords.Add(new TextModel());
        }

        private void AddReply()
        {
            this.Replies.Add(new TextModel());
        }

        private void RemoveUser(object parameter)
        {
            this.Users.Remove((TextModel)parameter);
            this.OnPropertyChanged();
        }

        private void RemoveLooseKeyword(object parameter)
        {
            this.LooseKeywords.Remove((TextModel)parameter);
            this.OnPropertyChanged();
        }

        private void RemoveAllKeyword(object parameter)
        {
            this.AllKeywords.Remove((TextModel)parameter);
            this.OnPropertyChanged();
        }

        private void RemoveExactKeyword(object parameter)
        {
            this.ExactKeywords.Remove((TextModel)parameter);
            this.OnPropertyChanged();
        }

        private void RemoveReply(object parameter)
        {
            this.Replies.Remove((TextModel)parameter);
            this.OnPropertyChanged();
        }

        private void OnTextModelUpdate(object sender, PropertyChangedEventArgs e)
        {
            TextModel model = (TextModel) sender;

            if (string.IsNullOrEmpty(model.Text))
            {
                return;
            }
            this.OnPropertyChanged();
        }
    }
}