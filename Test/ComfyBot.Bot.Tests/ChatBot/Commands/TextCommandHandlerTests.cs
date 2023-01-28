namespace ComfyBot.Bot.Tests.ChatBot.Commands;

using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Wrappers;
using Data.Models;
using Data.Repositories;
using Settings;

using Moq;

using NUnit.Framework;

using TwitchLib.Client.Interfaces;

[TestFixture]
public class TextCommandHandlerTests
{
    private Mock<IRepository<TextCommand>> repository;
    private Mock<ITextCommandReplyLoader> replyLoader;

    private Mock<ITwitchClient> twitchClient;
    private Mock<IChatCommand> chatCommand;

    private TextCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        repository = new Mock<IRepository<TextCommand>>();
        replyLoader = new Mock<ITextCommandReplyLoader>();

        twitchClient = new Mock<ITwitchClient>();
        chatCommand = new Mock<IChatCommand>();

        handler = new TextCommandHandler(repository.Object, replyLoader.Object);
    }

    [TestCase("channel1", "reply1")]
    [TestCase("channel2", "reply2")]
    public void HandleShouldSendLoadedReply(string channel, string reply)
    {
        ApplicationSettings.Default.Channel = channel;
        TextCommand command1 = new TextCommand();
        TextCommand command2 = new TextCommand();
        repository.Setup(r => r.GetAll()).Returns(new[] { command1, command2 });
        replyLoader.Setup(l => l.TryGetReply(command1, chatCommand.Object, out reply)).Returns(false);
        replyLoader.Setup(l => l.TryGetReply(command1, chatCommand.Object, out reply)).Returns(true);

        handler.Handle(twitchClient.Object, chatCommand.Object);

        twitchClient.Verify(c => c.SendMessage(channel, reply, false), Times.Once);
    }

    [Test]
    public void HandleShouldSendNothingIfNoReplyFound()
    {
        TextCommand command = new TextCommand();
        repository.Setup(r => r.GetAll()).Returns(new[] { command });
        string reply;
        replyLoader.Setup(l => l.TryGetReply(command, chatCommand.Object, out reply)).Returns(false);

        handler.Handle(twitchClient.Object, chatCommand.Object);

        twitchClient.Verify(c => c.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Never);
    }
}