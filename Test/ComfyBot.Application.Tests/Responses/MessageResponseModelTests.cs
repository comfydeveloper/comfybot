using System.ComponentModel;
using ComfyBot.Application.Responses;
using ComfyBot.Application.Shared;
using FluentAssertions;
using NUnit.Framework;

namespace ComfyBot.Application.Tests.Responses;

[TestFixture]
public class MessageResponseModelTests
{
    private MessageResponseModel model;

    [SetUp]
    public void Setup()
    {
        this.model = new MessageResponseModel();
    }

    [Test]
    public void AddReplyCommandShouldAddReply()
    {
        this.model.AddReplyCommand.Execute();

        this.model.Replies.Count.Should().Be(1);
    }

    [Test]
    public void AddLooseKeywordCommandShouldAddItem()
    {
        this.model.AddLooseKeywordCommand.Execute();

        this.model.LooseKeywords.Count.Should().Be(1);
    }

    [Test]
    public void AddExactKeywordCommandShouldAddItem()
    {
        this.model.AddExactKeywordCommand.Execute();

        this.model.ExactKeywords.Count.Should().Be(1);
    }

    [Test]
    public void AddAllKeywordCommandShouldAddItem()
    {
        this.model.AddAllKeywordCommand.Execute();

        this.model.AllKeywords.Count.Should().Be(1);
    }

    [Test]
    public void AddUserCommandShouldAddItem()
    {
        this.model.AddUserCommand.Execute();

        this.model.Users.Count.Should().Be(1);
    }

    [Test]
    public void RemoveUserShouldRemoveItem()
    {
        TextModel textModel = new();
        this.model.Users.Add(textModel);

        this.model.RemoveUserCommand.Execute(textModel);

        this.model.Users.Should().BeEmpty();
    }

    [Test]
    public void RemoveAllKeywordShouldRemoveItem()
    {
        TextModel textModel = new();
        this.model.AllKeywords.Add(textModel);

        this.model.RemoveAllKeywordCommand.Execute(textModel);

        this.model.AllKeywords.Should().BeEmpty();
    }

    [Test]
    public void RemoveLooseKeywordShouldRemoveItem()
    {
        TextModel textModel = new();
        this.model.LooseKeywords.Add(textModel);

        this.model.RemoveLooseKeywordCommand.Execute(textModel);

        this.model.LooseKeywords.Should().BeEmpty();
    }

    [Test]
    public void RemoveExactKeywordShouldRemoveItem()
    {
        TextModel textModel = new();
        this.model.ExactKeywords.Add(textModel);

        this.model.RemoveExactKeywordCommand.Execute(textModel);

        this.model.ExactKeywords.Should().BeEmpty();
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
        this.model.Users.Add(textModel);

        textModel.Text = text;

        Assert.AreEqual(expected, result);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void TimeoutSetterShouldSetValue(int timeout)
    {
        this.model.TimeoutInSeconds = timeout;

        Assert.AreEqual(timeout, this.model.TimeoutInSeconds);
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

        this.model.TimeoutInSeconds = 1;

        Assert.IsTrue(result);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void PrioritySetterShouldSetValue(int priority)
    {
        this.model.Priority = priority;

        Assert.AreEqual(priority, this.model.Priority);
    }

    [Test]
    public void PrioritySetterShouldNotifyPropertyChange()
    {
        bool result = false;
        void TestMethod(object sender, PropertyChangedEventArgs e)
        {
            result = true;
        }

        this.model.PropertyChanged += TestMethod;

        this.model.TimeoutInSeconds = 1;

        Assert.IsTrue(result);
    }
}