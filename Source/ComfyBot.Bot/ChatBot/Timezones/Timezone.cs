namespace ComfyBot.Bot.ChatBot.Timezones
{
    public class Timezone
    {
        public string Area { get; set; }

        public string Location { get; set; }

        public string Region { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Region))
            {
                return $"{Area}/{Location}/{this.Region}";
            }
            if (!string.IsNullOrEmpty(this.Location))
            {
                return $"{Area}/{Location}";
            }
            return this.Area;
        }
    }
}