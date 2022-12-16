namespace ComfyBot.Application.Tests.Responses;

using System.ComponentModel;

using ComfyBot.Application.Responses;
using ComfyBot.Application.Shared;

using NUnit.Framework;

[TestFixture]
public class MessageResponseModelTests
{
    private MessageResponseModel model;

    [SetUp]
    public void Setup()
    {
        model = new MessageResponseModel();
    }

    [Test]
    public void AddReplyCommandShouldAddReply()
    {
        model.AddReplyCommand.Execute();

        Assert.AreEqual(1, model.Replies.Count);
    }

    [Test]
    public void AddLooseKeywordCommandShouldAddItem()
    {
        model.AddLooseKeywordCommand.Execute();

        Assert.AreEqual(1, model.LooseKeywords.Count);
    }

    [Test]
    public void AddExactKeywordCommandShouldAddItem()
    {
        model.AddExactKeywordCommand.Execute();

        Assert.AreEqual(1, model.ExactKeywords.Count);
    }

    [Test]
    public void AddAllKeywordCommandShouldAddItem()
    {
        model.AddAllKeywordCommand.Execute();

        Assert.AreEqual(1, model.AllKeywords.Count);
    }

    [Test]
    public void AddUserCommandShouldAddItem()
    {
        model.AddUserCommand.Execute();

        Assert.AreEqual(1, model.Users.Count);
    }

    [Test]
    public void RemoveUserShouldRemoveItem()
    {
        TextModel textModel = new TextModel();
        model.Users.Add(textModel);

        model.RemoveUserCommand.Execute(textModel);

        Assert.AreEqual(0, model.Users.Count);
    }

    [Test]
    public void RemoveAllKeywordShouldRemoveItem()
    {
        TextModel textModel = new TextModel();
        model.AllKeywords.Add(textModel);

        model.RemoveAllKeywordCommand.Execute(textModel);

        Assert.AreEqual(0, model.AllKeywords.Count);
    }

    [Test]
    public void RemoveLooseKeywordShouldRemoveItem()
    {
        TextModel textModel = new TextModel();
        model.LooseKeywords.Add(textModel);

        model.RemoveLooseKeywordCommand.Execute(textModel);

        Assert.AreEqual(0, model.LooseKeywords.Count);
    }

    [Test]
    public void RemoveExactKeywordShouldRemoveItem()
    {
        TextModel textModel = new TextModel();
        model.ExactKeywords.Add(textModel);

        model.RemoveExactKeywordCommand.Execute(textModel);

        Assert.AreEqual(0, model.ExactKeywords.Count);
    }

    [Test]
    public void RemoveReplyShouldRemoveItem()
    {
        TextModel textModel = new TextModel();
        model.Replies.Add(textModel);

        model.RemoveReplyCommand.Execute(textModel);

        Assert.AreEqual(0, model.Replies.Count);
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
        model.PropertyChanged += TestMethod;
        TextModel textModel = new TextModel();
        model.Users.Add(textModel);

        textModel.Text = text;

        Assert.AreEqual(expected, result);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void TimeoutSetterShouldSetValue(int timeout)
    {
        model.Timeout = timeout;

        Assert.AreEqual(timeout, model.Timeout);
    }

    [Test]
    public void TimeoutSetterShouldNotifyPropertyChange()
    {
        bool result = false;
        void TestMethod(object sender, PropertyChangedEventArgs e)
        {
            result = true;
        }
        model.PropertyChanged += TestMethod;

        model.Timeout = 1;

        Assert.IsTrue(result);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void PrioritySetterShouldSetValue(int priority)
    {
        model.Priority = priority;

        Assert.AreEqual(priority, model.Priority);
    }

    [Test]
    public void PrioritySetterShouldNotifyPropertyChange()
    {
        bool result = false;
        void TestMethod(object sender, PropertyChangedEventArgs e)
        {
            result = true;
        }
        model.PropertyChanged += TestMethod;

        model.Timeout = 1;

        Assert.IsTrue(result);
    }
}