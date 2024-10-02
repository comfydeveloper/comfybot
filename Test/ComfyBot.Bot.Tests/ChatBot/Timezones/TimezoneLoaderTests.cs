using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Common.Http;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Timezones;

[TestFixture]
public class TimezoneLoaderTests
{
    private IHttpService httpService;

    private TimezoneLoader timezoneLoader;

    [SetUp]
    public void Setup()
    {
        this.httpService = Substitute.For<IHttpService>();
        this.timezoneLoader = new TimezoneLoader();

        HttpService.OverrideInstance(this.httpService);
    }

    [Test]
    public void TryLoadShouldReturnFalseWhenNoMatchingTimezoneFound()
    {
        string[] foundZones = [];
        this.httpService.GetAsync<string[]>("http://worldtimeapi.org/api/timezone").Returns(foundZones);

        bool result = this.timezoneLoader.TryLoad("test", out Timezone zone);

        result.Should().BeFalse();
        zone.Should().BeNull();
        Assert.False(result);
        Assert.IsNull(zone);
    }

    [TestCase("a/b/test", "test")]
    [TestCase("a/test", "test")]
    [TestCase("test", "test")]
    public void TryLoadShouldReturnTrueWhenMatchingTimezoneFound(string foundZone, string searchText)
    {
        string[] foundZones = [foundZone];
        this.httpService.GetAsync<string[]>("http://worldtimeapi.org/api/timezone").Returns(foundZones);

        bool result = this.timezoneLoader.TryLoad(searchText, out Timezone zone);

        result.Should().BeTrue();
        zone.Should().NotBeNull();
    }

    [Test]
    public void TryLoadShouldCacheTimezones()
    {
        string[] foundZones = ["test"];
        this.httpService.GetAsync<string[]>("http://worldtimeapi.org/api/timezone").Returns(foundZones);

        this.timezoneLoader.TryLoad("test", out Timezone _);
        this.timezoneLoader.TryLoad("test", out Timezone _);

        this.httpService.Received(1).GetAsync<string[]>("http://worldtimeapi.org/api/timezone");
    }

    [TearDown]
    public void TearDown()
    {
        HttpService.OverrideInstance(null);
    }
}