using System.ComponentModel;
using ComfyBot.Application.Shared;
using ComfyBot.Application.TextCommands;
using NUnit.Framework;

namespace ComfyBot.Application.Tests.TextCommands;

[TestFixture]
public class TextCommandModelTests
{
    private TextCommandModel model;

    [SetUp]
    public void Setup()
    {
        model = new TextCommandModel();
    }

    [Test]
    public void AddReplyCommandShouldAddReply()
    {
        model.AddReplyCommand.Execute();

        Assert.AreEqual(1, model.Replies.Count);
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
        model.Replies.Add(textModel);

        textModel.Text = text;

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void AddTextCommandShouldAddReply()
    {
        model.AddTextCommand.Execute();

        Assert.AreEqual(1, model.Commands.Count);
    }

    [Test]
    public void RemoveTextShouldRemoveItem()
    {
        TextModel textModel = new TextModel();
        model.Commands.Add(textModel);

        model.RemoveTextCommand.Execute(textModel);

        Assert.AreEqual(0, model.Commands.Count);
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
        model.PropertyChanged += TestMethod;
        TextModel textModel = new TextModel();
        model.Commands.Add(textModel);

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
}