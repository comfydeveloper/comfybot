namespace ComfyBot.Application.Shoutouts
{
    using ComfyBot.Application.Shared;

    public class ShoutoutModel : NotifyingModel
    {
        private string command;
        private string message;

        public string Id { get; set; }

        public string Command
        {
            get => this.command;
            set { this.command = value; this.OnPropertyChanged(); }
        }

        public string Message
        {
            get => this.message;
            set { this.message = value; this.OnPropertyChanged(); }
        }
    }
}