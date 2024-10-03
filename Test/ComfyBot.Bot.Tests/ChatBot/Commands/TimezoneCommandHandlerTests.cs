using System;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Bot.ChatBot.Wrappers;
using NSubstitute;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Commands;

[TestFixture]
public class TimezoneCommandHandlerTests
{
    private IChatMessage chatMessage;
    private IChatCommand chatCommand;
    private ITimezoneLoader timezoneLoader;
    private ITimeLoader timeLoader;
    private IMessageSender messageSender;

    private TimezoneCommandHandler commandHandler;

    [SetUp]
    public void Setup()
    {
        this.chatMessage = Substitute.For<IChatMessage>();
        this.chatCommand = Substitute.For<IChatCommand>();
        this.timezoneLoader = Substitute.For<ITimezoneLoader>();
        this.timeLoader = Substitute.For<ITimeLoader>();
        this.messageSender = Substitute.For<IMessageSender>();

        this.chatCommand.ChatMessage.Returns(this.chatMessage);

        this.commandHandler = new TimezoneCommandHandler(this.timezoneLoader, this.timeLoader, this.messageSender);
    }

    [Test]
    public void HandleShouldLoadTimeForTimezone()
    {
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

        this.commandHandler.Handle(this.chatCommand);

        this.messageSender.Received(1).Send($"user: area/location/region {timezoneInfo.DateTime:G}");
    }

    [Test]
    public void HandleShouldNotifyUserWhenTimezoneWasNotFound()
    {
        this.chatCommand.CommandText.Returns("timezone");
        this.chatCommand.ArgumentsAsList.Returns(["zone"]);
        this.chatCommand.ArgumentsAsString.Returns("zone");
        this.chatMessage.UserName.Returns("user");
        this.timezoneLoader.TryLoad("zone", out Arg.Any<Timezone>()).Returns(false);

        this.commandHandler.Handle(this.chatCommand);

        this.messageSender.Received(1).Send("Sorry user, can't find timezone info for 'zone'.");
    }
}