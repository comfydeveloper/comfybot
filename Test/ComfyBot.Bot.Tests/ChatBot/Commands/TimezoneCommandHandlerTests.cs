namespace ComfyBot.Bot.Tests.ChatBot.Commands
{
    using System;
    using System.Collections.Generic;
    using ComfyBot.Bot.ChatBot.Commands;
    using ComfyBot.Bot.ChatBot.Timezones;
    using ComfyBot.Bot.ChatBot.Wrappers;

    using Moq;

    using NUnit.Framework;

    using TwitchLib.Client.Interfaces;

    [TestFixture]
    public class TimezoneCommandHandlerTests
    {
        private Mock<IChatMessage> chatMessage;
        private Mock<IChatCommand> chatCommand;
        private Mock<ITwitchClient> twitchClient;
        private Mock<ITimezoneLoader> timezoneLoader;
        private Mock<ITimeLoader> timeLoader;

        private TimezoneCommandHandler commandHandler;

        [SetUp]
        public void Setup()
        {
            chatMessage = new Mock<IChatMessage>();
            chatCommand = new Mock<IChatCommand>();
            twitchClient = new Mock<ITwitchClient>();
            timezoneLoader = new Mock<ITimezoneLoader>();
            timeLoader = new Mock<ITimeLoader>();

            chatCommand.Setup(c => c.ChatMessage).Returns(chatMessage.Object);

            commandHandler = new TimezoneCommandHandler(timezoneLoader.Object, timeLoader.Object);
        }

        [Test]
        public void HandleShouldLoadTimeForTimezone()
        {
            Settings.ApplicationSettings.Default.Channel = "channel";
            chatCommand.Setup(c => c.CommandText).Returns("timezone");
            chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string> { "zone" });
            chatCommand.Setup(c => c.ArgumentsAsString).Returns("zone");
            chatMessage.Setup(m => m.UserName).Returns("user");
            Timezone zone = new Timezone();
            timezoneLoader.Setup(l => l.TryLoad("zone", out zone)).Returns(true);
            TimezoneInfo timezoneInfo = new TimezoneInfo
                                        {
                                            Timezone = "area/location/region",
                                            DateTime = new DateTime(2020, 01, 01)
                                        };
            timeLoader.Setup(l => l.GetTime(zone)).Returns(timezoneInfo);

            commandHandler.Handle(twitchClient.Object, chatCommand.Object);

            twitchClient.Verify(c => c.SendMessage("channel", $@"user: area/location/region {timezoneInfo.DateTime:G}", false));
        }

        [Test]
        public void HandleShouldNotifyUserWhenTimezoneWasNotFound()
        {
            Settings.ApplicationSettings.Default.Channel = "channel";
            chatCommand.Setup(c => c.CommandText).Returns("timezone");
            chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string> { "zone" });
            chatCommand.Setup(c => c.ArgumentsAsString).Returns("zone");
            chatMessage.Setup(m => m.UserName).Returns("user");
            Timezone zone = new Timezone();
            timezoneLoader.Setup(l => l.TryLoad("zone", out zone)).Returns(false);

            commandHandler.Handle(twitchClient.Object, chatCommand.Object);

            twitchClient.Verify(c => c.SendMessage("channel", @"Sorry user, can't find timezone info for 'zone'.", false));
        }
    }
}