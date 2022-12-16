namespace ComfyBot.Bot.ChatBot.Timezones
{
    public class Timezone
    {
        public string Area { get; set; }

        public string Location { get; set; }

        public string Region { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Region))
            {
                return $"{Area}/{Location}/{Region}";
            }
            if (!string.IsNullOrEmpty(Location))
            {
                return $"{Area}/{Location}";
            }
            return Area;
        }
    }
}