namespace ComfyBot.Bot.Tests.ChatBot.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

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
            this.chatMessage = new Mock<IChatMessage>();
            this.chatCommand = new Mock<IChatCommand>();
            this.twitchClient = new Mock<ITwitchClient>();
            this.timezoneLoader = new Mock<ITimezoneLoader>();
            this.timeLoader = new Mock<ITimeLoader>();

            this.chatCommand.Setup(c => c.ChatMessage).Returns(this.chatMessage.Object);

            this.commandHandler = new TimezoneCommandHandler(this.timezoneLoader.Object, this.timeLoader.Object);
        }

        [Test]
        public void HandleShouldLoadTimeForTimezone()
        {
            Settings.ApplicationSettings.Default.Channel = "channel";
            this.chatCommand.Setup(c => c.CommandText).Returns("timezone");
            this.chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string> { "zone" });
            this.chatCommand.Setup(c => c.ArgumentsAsString).Returns("zone");
            this.chatMessage.Setup(m => m.UserName).Returns("user");
            Timezone zone = new Timezone();
            this.timezoneLoader.Setup(l => l.TryLoad("zone", out zone)).Returns(true);
            TimezoneInfo timezoneInfo = new TimezoneInfo
                                        {
                                            Timezone = "area/location/region",
                                            DateTime = new DateTime(2020, 01, 01)
                                        };
            this.timeLoader.Setup(l => l.GetTime(zone)).Returns(timezoneInfo);

            this.commandHandler.Handle(this.twitchClient.Object, this.chatCommand.Object);

            this.twitchClient.Verify(c => c.SendMessage("channel", $@"user: area/location/region {timezoneInfo.DateTime:G}", false));
        }

        [Test]
        public void HandleShouldNotifyUserWhenTimezoneWasNotFound()
        {
            Settings.ApplicationSettings.Default.Channel = "channel";
            this.chatCommand.Setup(c => c.CommandText).Returns("timezone");
            this.chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string> { "zone" });
            this.chatCommand.Setup(c => c.ArgumentsAsString).Returns("zone");
            this.chatMessage.Setup(m => m.UserName).Returns("user");
            Timezone zone = new Timezone();
            this.timezoneLoader.Setup(l => l.TryLoad("zone", out zone)).Returns(false);

            this.commandHandler.Handle(this.twitchClient.Object, this.chatCommand.Object);

            this.twitchClient.Verify(c => c.SendMessage("channel", @"Sorry user, can't find timezone info for 'zone'.", false));
        }
    }
}