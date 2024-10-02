using System.ComponentModel;
using ComfyBot.Application.Shared;
using ComfyBot.Application.TextCommands;
using FluentAssertions;
using NUnit.Framework;

namespace ComfyBot.Application.Tests.TextCommands;

[TestFixture]
public class TextCommandModelTests
{
    private TextCommandModel model;

    [SetUp]
    public void Setup()
    {
        this.model = new TextCommandModel();
    }

    [Test]
    public void AddReplyCommandShouldAddReply()
    {
        this.model.AddReplyCommand.Execute();

        this.model.Replies.Count.Should().Be(1);
    }

    [Test]
    public void RemoveReplyShouldRemoveItem()
    {
        TextModel textModel = new();
        this.model.Replies.Add(textModel);

        this.model.RemoveReplyCommand.Execute(textModel);

        this.model.Replies.Should().BeEmpty();
    }

    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("a", true)]
    public void ChangeToAddedListElementShouldInvokePropertyChangeEvent(string text, bool expected)
    {
        bool result = false;
        void TestMethod(object sender, PropertyChangedEventArgs e)
        {
            result = true;
        }

        this.model.PropertyChanged += TestMethod;
        TextModel textModel = new();
        this.model.Replies.Add(textModel);

        textModel.Text = text;

        this.model.Replies.Count.Should().Be(1);
    }

    [Test]
    public void AddTextCommandShouldAddReply()
    {
        this.model.AddTextCommand.Execute();

        this.model.Commands.Count.Should().Be(1);
    }

    [Test]
    public void RemoveTextShouldRemoveItem()
    {
        TextModel textModel = new();
        this.model.Commands.Add(textModel);

        this.model.RemoveTextCommand.Execute(textModel);

        this.model.Commands.Should().BeEmpty();
    }

    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("a", true)]
    public void ChangeToAddedTextElementShouldInvokePropertyChangeEvent(string text, bool expected)
    {
        bool result = false;
        void TestMethod(object sender, PropertyChangedEventArgs e)
        {
            result = true;
        }

        this.model.PropertyChanged += TestMethod;
        TextModel textModel = new();
        this.model.Commands.Add(textModel);

        textModel.Text = text;

        result.Should().Be(expected);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void TimeoutSetterShouldSetValue(int timeout)
    {
        this.model.Timeout = timeout;

        this.model.Timeout.Should().Be(timeout);
    }

    [Test]
    public void TimeoutSetterShouldNotifyPropertyChange()
    {
        bool result = false;
        void TestMethod(object sender, PropertyChangedEventArgs e)
        {
            result = true;
        }

        this.model.PropertyChanged += TestMethod;

        this.model.Timeout = 1;

        result.Should().BeTrue();
    }
}