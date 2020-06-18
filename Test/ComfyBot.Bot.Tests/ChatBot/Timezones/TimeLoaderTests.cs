namespace ComfyBot.Bot.Tests.ChatBot.Timezones
{
    using ComfyBot.Bot.ChatBot.Timezones;
    using ComfyBot.Common.Http;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class TimeLoaderTests
    {
        private Mock<IHttpService> httpService;

        private TimeLoader timeLoader;

        [SetUp]
        public void Setup()
        {
            this.httpService = new Mock<IHttpService>();
            this.timeLoader = new TimeLoader();

            HttpService.OverrideInstance(this.httpService.Object);
        }

        [Test]
        public void GetTimeShouldMapTimezone()
        {
            Timezone timezone = new Timezone
                                {
                                    Area = "area",
                                    Location = "location",
                                    Region = "region"
                                };

            TimezoneInfo timezoneInfo = this.timeLoader.GetTime(timezone);

            this.httpService.Verify(s => s.GetAsync<TimezoneInfo>($"http://worldtimeapi.org/api/timezone/{timezone}"));
        }

        [TearDown]
        public void TearDown()
        {
            HttpService.OverrideInstance(null);
        }
    }
}