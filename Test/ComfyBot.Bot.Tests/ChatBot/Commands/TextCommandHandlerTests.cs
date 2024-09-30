using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using ComfyBot.Settings;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Tests.ChatBot.Commands;

[TestFixture]
public class TextCommandHandlerTests
{
    private Mock<IRepository<TextCommandOld>> repository;
    private Mock<ITextCommandReplyLoader> replyLoader;

    private Mock<ITwitchClient> twitchClient;
    private Mock<IChatCommand> chatCommand;

    private TextCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        this.repository = new Mock<IRepository<TextCommandOld>>();
        this.replyLoader = new Mock<ITextCommandReplyLoader>();

        this.twitchClient = new Mock<ITwitchClient>();
        this.chatCommand = new Mock<IChatCommand>();

        this.handler = new TextCommandHandler(this.repository.Object, this.replyLoader.Object);
    }

    [TestCase("channel1", "reply1")]
    [TestCase("channel2", "reply2")]
    public void HandleShouldSendLoadedReply(string channel, string reply)
    {
        ApplicationSettings.Default.Channel = channel;
        TextCommandOld command1 = new();
        TextCommandOld command2 = new();
        this.repository.Setup(r => r.GetAll()).Returns(new[] { command1, command2 });
        this.replyLoader.Setup(l => l.TryGetReply(command1, this.chatCommand.Object, out reply)).Returns(false);
        this.replyLoader.Setup(l => l.TryGetReply(command1, this.chatCommand.Object, out reply)).Returns(true);

        this.handler.Handle(this.twitchClient.Object, this.chatCommand.Object);

        this.twitchClient.Verify(c => c.SendMessage(channel, reply, false), Times.Once);
    }

    [Test]
    public void HandleShouldSendNothingIfNoReplyFound()
    {
        TextCommandOld commandOld = new();
        this.repository.Setup(r => r.GetAll()).Returns(new[] { commandOld });
        string reply;
        this.replyLoader.Setup(l => l.TryGetReply(commandOld, this.chatCommand.Object, out reply)).Returns(false);

        this.handler.Handle(this.twitchClient.Object, this.chatCommand.Object);

        this.twitchClient.Verify(c => c.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Never);
    }
}