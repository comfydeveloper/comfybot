using ComfyBot.Bot.ChatBot.Timezones;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Timezones;

[TestFixture]
public class TimezoneTests
{
    [TestCase("area", "location", "region", "area/location/region")]
    [TestCase("area", "location", "", "area/location")]
    [TestCase("area", "", "", "area")]
    public void ToStringShouldReturnCombinedProperties(string area, string location, string region, string expected)
    {
        Timezone zone = new Timezone
        {
            Area = area,
            Location = location,
            Region = region
        };

        Assert.AreEqual(expected, zone.ToString());
    }
}