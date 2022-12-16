namespace ComfyBot.Application.Shared
{
    public class TextModel : NotifyingModel
    {
        private string text;

        public string Text
        {
            get => text;
            set { text = value; OnPropertyChanged(); }
        }
    }
}