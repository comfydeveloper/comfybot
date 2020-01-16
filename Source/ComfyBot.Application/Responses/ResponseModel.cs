namespace ComfyBot.Application.Responses
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using ComfyBot.Application.Shared;
    using ComfyBot.Application.Shared.Extensions;

    public class ResponseModel : NotifyingModel
    {
        private int timeout = 60;

        public ResponseModel()
        {
            this.AddUserCommand = new DelegateCommand(this.AddUser);
            this.AddLooseKeywordCommand = new DelegateCommand(this.AddLooseKeyword);
            this.RemoveUserCommand = new ParameterCommand(this.RemoveUser);
            this.RemoveLooseKeywordCommand = new ParameterCommand(this.RemoveLooseKeyword);

            this.Users.RegisterCollectionItemChanged(this.OnTextModelUpdate);
            this.LooseKeywords.RegisterCollectionItemChanged(this.OnTextModelUpdate);
        }

        public DelegateCommand AddUserCommand { get; }

        public DelegateCommand AddLooseKeywordCommand { get; }

        public ParameterCommand RemoveUserCommand { get; }

        public ParameterCommand RemoveLooseKeywordCommand { get; }

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

        private void RemoveUser(object parameter)
        {
            this.Users.Remove((TextModel)parameter);
            this.OnPropertyChanged();
        }

        private void RemoveLooseKeyword(object parameter)
        {
            this.LooseKeywords.Remove((TextModel)parameter);
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