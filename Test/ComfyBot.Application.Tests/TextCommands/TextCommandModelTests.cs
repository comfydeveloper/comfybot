namespace ComfyBot.Application.Tests.TextCommands
{
    using System.ComponentModel;

    using ComfyBot.Application.Shared;
    using ComfyBot.Application.TextCommands;

    using NUnit.Framework;

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

            Assert.AreEqual(1, this.model.Replies.Count);
        }

        [Test]
        public void RemoveReplyShouldRemoveItem()
        {
            TextModel textModel = new TextModel();
            this.model.Replies.Add(textModel);

            this.model.RemoveReplyCommand.Execute(textModel);

            Assert.AreEqual(0, this.model.Replies.Count);
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
            TextModel textModel = new TextModel();
            this.model.Replies.Add(textModel);

            textModel.Text = text;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void AddTextCommandShouldAddReply()
        {
            this.model.AddTextCommand.Execute();

            Assert.AreEqual(1, this.model.Commands.Count);
        }

        [Test]
        public void RemoveTextShouldRemoveItem()
        {
            TextModel textModel = new TextModel();
            this.model.Commands.Add(textModel);

            this.model.RemoveTextCommand.Execute(textModel);

            Assert.AreEqual(0, this.model.Commands.Count);
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
            TextModel textModel = new TextModel();
            this.model.Commands.Add(textModel);

            textModel.Text = text;

            Assert.AreEqual(expected, result);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void TimeoutSetterShouldSetValue(int timeout)
        {
            this.model.Timeout = timeout;

            Assert.AreEqual(timeout, this.model.Timeout);
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

            Assert.IsTrue(result);
        }
    }
}