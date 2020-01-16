namespace ComfyBot.Application.Responses
{
    using ComfyBot.Application.Shared;

    public class TextModel : NotifyingModel
    {
        private string text;

        public string Text
        {
            get => this.text;
            set { this.text = value; this.OnPropertyChanged(); }
        }
    }
}