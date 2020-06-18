namespace ComfyBot.Bot.Tests.ChatBot.Timezones
{
    using ComfyBot.Bot.ChatBot.Timezones;
    using ComfyBot.Common.Http;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class TimezoneLoaderTests
    {
        private Mock<IHttpService> httpService;

        private TimezoneLoader timezoneLoader;

        [SetUp]
        public void Setup()
        {
            this.httpService = new Mock<IHttpService>();
            this.timezoneLoader = new TimezoneLoader();

            HttpService.OverrideInstance(this.httpService.Object);
        }

        [Test]
        public void TryLoadShouldReturnFalseWhenNoMatchingTimezoneFound()
        {
            string[] foundZones = new string[0];
            this.httpService.Setup(s => s.GetAsync<string[]>("http://worldtimeapi.org/api/timezone")).ReturnsAsync(foundZones);

            bool result = this.timezoneLoader.TryLoad("test", out Timezone zone);

            Assert.False(result);
            Assert.IsNull(zone);
        }

        [TestCase("a/b/test", "test")]
        [TestCase("a/test", "test")]
        [TestCase("test", "test")]
        public void TryLoadShouldReturnTrueWhenMatchingTimezoneFound(string foundZone, string searchText)
        {
            string[] foundZones = { foundZone };
            this.httpService.Setup(s => s.GetAsync<string[]>("http://worldtimeapi.org/api/timezone")).ReturnsAsync(foundZones);

            bool result = this.timezoneLoader.TryLoad(searchText, out Timezone zone);

            Assert.True(result);
            Assert.IsNotNull(zone);
        }

        [Test]
        public void TryLoadShouldCacheTimezones()
        {
            string[] foundZones = { "test" };
            this.httpService.Setup(s => s.GetAsync<string[]>("http://worldtimeapi.org/api/timezone")).ReturnsAsync(foundZones);

            this.timezoneLoader.TryLoad("test", out Timezone zone1);
            this.timezoneLoader.TryLoad("test", out Timezone zone2);

            this.httpService.Verify(s => s.GetAsync<string[]>("http://worldtimeapi.org/api/timezone"), Times.Once());
        }

        [TearDown]
        public void TearDown()
        {
            HttpService.OverrideInstance(null);
        }
    }
}