using System;
using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Common.Http;
using Moq;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Timezones;

[TestFixture]
public class TimezoneLoaderTests
{
    private Mock<IHttpService> httpService;

    private TimezoneLoader timezoneLoader;

    [SetUp]
    public void Setup()
    {
        httpService = new Mock<IHttpService>();
        timezoneLoader = new TimezoneLoader();

        HttpService.OverrideInstance(httpService.Object);
    }

    [Test]
    public void TryLoadShouldReturnFalseWhenNoMatchingTimezoneFound()
    {
        string[] foundZones = Array.Empty<string>();
        httpService.Setup(s => s.GetAsync<string[]>("http://worldtimeapi.org/api/timezone")).ReturnsAsync(foundZones);

        bool result = timezoneLoader.TryLoad("test", out Timezone zone);

        Assert.False(result);
        Assert.IsNull(zone);
    }

    [TestCase("a/b/test", "test")]
    [TestCase("a/test", "test")]
    [TestCase("test", "test")]
    public void TryLoadShouldReturnTrueWhenMatchingTimezoneFound(string foundZone, string searchText)
    {
        string[] foundZones = { foundZone };
        httpService.Setup(s => s.GetAsync<string[]>("http://worldtimeapi.org/api/timezone")).ReturnsAsync(foundZones);

        bool result = timezoneLoader.TryLoad(searchText, out Timezone zone);

        Assert.True(result);
        Assert.IsNotNull(zone);
    }

    [Test]
    public void TryLoadShouldCacheTimezones()
    {
        string[] foundZones = { "test" };
        httpService.Setup(s => s.GetAsync<string[]>("http://worldtimeapi.org/api/timezone")).ReturnsAsync(foundZones);

        timezoneLoader.TryLoad("test", out Timezone zone1);
        timezoneLoader.TryLoad("test", out Timezone zone2);

        httpService.Verify(s => s.GetAsync<string[]>("http://worldtimeapi.org/api/timezone"), Times.Once());
    }

    [TearDown]
    public void TearDown()
    {
        HttpService.OverrideInstance(null);
    }
}