using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Common.Http;
using Moq;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Timezones;

[TestFixture]
public class TimeLoaderTests
{
    private Mock<IHttpService> httpService;

    private TimeLoader timeLoader;

    [SetUp]
    public void Setup()
    {
        httpService = new Mock<IHttpService>();
        timeLoader = new TimeLoader();

        HttpService.OverrideInstance(httpService.Object);
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

        TimezoneInfo timezoneInfo = timeLoader.GetTime(timezone);

        httpService.Verify(s => s.GetAsync<TimezoneInfo>($"http://worldtimeapi.org/api/timezone/{timezone}"));
    }

    [TearDown]
    public void TearDown()
    {
        HttpService.OverrideInstance(null);
    }
}