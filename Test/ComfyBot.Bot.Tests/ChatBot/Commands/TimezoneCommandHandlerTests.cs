using System;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Tests.ChatBot.Commands;

[TestFixture]
public class TimezoneCommandHandlerTests
{
    private IChatMessage chatMessage;
    private IChatCommand chatCommand;
    private ITwitchClient twitchClient;
    private ITimezoneLoader timezoneLoader;
    private ITimeLoader timeLoader;
    private BotSettings settings;

    private TimezoneCommandHandler commandHandler;

    [SetUp]
    public void Setup()
    {
        this.chatMessage = Substitute.For<IChatMessage>();
        this.chatCommand = Substitute.For<IChatCommand>();
        this.twitchClient = Substitute.For<ITwitchClient>();
        this.timezoneLoader = Substitute.For<ITimezoneLoader>();
        this.timeLoader = Substitute.For<ITimeLoader>();
        this.settings = new BotSettings();
        IOptions<BotSettings> options = Substitute.For<IOptions<BotSettings>>();
        options.Value.Returns(this.settings);

        this.chatCommand.ChatMessage.Returns(this.chatMessage);

        this.commandHandler = new TimezoneCommandHandler(this.timezoneLoader, this.timeLoader, options);
    }

    [Test]
    public void HandleShouldLoadTimeForTimezone()
    {
        this.settings.Channel = "channel";
        this.chatCommand.CommandText.Returns("timezone");
        this.chatCommand.ArgumentsAsList.Returns(["zone"]);
        this.chatCommand.ArgumentsAsString.Returns("zone");
        this.chatMessage.UserName.Returns("user");
        Timezone zone = new();
        this.timezoneLoader.TryLoad("zone", out Arg.Any<Timezone>()).Returns(
            x =>
            {
                x[1] = zone;
                return true;
            });
        TimezoneInfo timezoneInfo = new()
        {
            Timezone = "area/location/region",
            DateTime = new DateTime(2020, 01, 01)
        };
        this.timeLoader.GetTime(zone).Returns(timezoneInfo);

        this.commandHandler.Handle(this.twitchClient, this.chatCommand);

        this.twitchClient.Received(1).SendMessage("channel", $"user: area/location/region {timezoneInfo.DateTime:G}", false);
    }

    [Test]
    public void HandleShouldNotifyUserWhenTimezoneWasNotFound()
    {
        this.settings.Channel = "channel";
        this.chatCommand.CommandText.Returns("timezone");
        this.chatCommand.ArgumentsAsList.Returns(["zone"]);
        this.chatCommand.ArgumentsAsString.Returns("zone");
        this.chatMessage.UserName.Returns("user");
        this.timezoneLoader.TryLoad("zone", out Arg.Any<Timezone>()).Returns(false);

        this.commandHandler.Handle(this.twitchClient, this.chatCommand);

        this.twitchClient.Received(1).SendMessage("channel", "Sorry user, can't find timezone info for 'zone'.", false);
    }
}