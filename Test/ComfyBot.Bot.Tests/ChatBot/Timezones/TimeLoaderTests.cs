using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Common.Http;
using NSubstitute;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Timezones;

[TestFixture]
public class TimeLoaderTests
{
    private IHttpService httpService;

    private TimeLoader timeLoader;

    [SetUp]
    public void Setup()
    {
        this.httpService = Substitute.For<IHttpService>();
        this.timeLoader = new TimeLoader();

        HttpService.OverrideInstance(this.httpService);
    }

    [Test]
    public void GetTimeShouldMapTimezone()
    {
        Timezone timezone = new()
        {
            Area = "area",
            Location = "location",
            Region = "region"
        };

        this.timeLoader.GetTime(timezone);

        this.httpService.Received(1).GetAsync<TimezoneInfo>($"http://worldtimeapi.org/api/timezone/{timezone}");
    }

    [TearDown]
    public void TearDown()
    {
        HttpService.OverrideInstance(null);
    }
}
