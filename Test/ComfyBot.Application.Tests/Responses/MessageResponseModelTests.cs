namespace ComfyBot.Application.Tests.Responses
{
    using System.ComponentModel;

    using ComfyBot.Application.Responses;

    using NUnit.Framework;

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

            Assert.AreEqual(1, this.model.Replies.Count);
        }

        [Test]
        public void AddLooseKeywordCommandShouldAddItem()
        {
            this.model.AddLooseKeywordCommand.Execute();

            Assert.AreEqual(1, this.model.LooseKeywords.Count);
        }

        [Test]
        public void AddExactKeywordCommandShouldAddItem()
        {
            this.model.AddExactKeywordCommand.Execute();

            Assert.AreEqual(1, this.model.ExactKeywords.Count);
        }

        [Test]
        public void AddAllKeywordCommandShouldAddItem()
        {
            this.model.AddAllKeywordCommand.Execute();

            Assert.AreEqual(1, this.model.AllKeywords.Count);
        }

        [Test]
        public void AddUserCommandShouldAddItem()
        {
            this.model.AddUserCommand.Execute();

            Assert.AreEqual(1, this.model.Users.Count);
        }

        [Test]
        public void RemoveUserShouldRemoveItem()
        {
            TextModel textModel = new TextModel();
            this.model.Users.Add(textModel);

            this.model.RemoveUserCommand.Execute(textModel);

            Assert.AreEqual(0, this.model.Users.Count);
        }

        [Test]
        public void RemoveAllKeywordShouldRemoveItem()
        {
            TextModel textModel = new TextModel();
            this.model.AllKeywords.Add(textModel);

            this.model.RemoveAllKeywordCommand.Execute(textModel);

            Assert.AreEqual(0, this.model.AllKeywords.Count);
        }

        [Test]
        public void RemoveLooseKeywordShouldRemoveItem()
        {
            TextModel textModel = new TextModel();
            this.model.LooseKeywords.Add(textModel);

            this.model.RemoveLooseKeywordCommand.Execute(textModel);

            Assert.AreEqual(0, this.model.LooseKeywords.Count);
        }

        [Test]
        public void RemoveExactKeywordShouldRemoveItem()
        {
            TextModel textModel = new TextModel();
            this.model.ExactKeywords.Add(textModel);

            this.model.RemoveExactKeywordCommand.Execute(textModel);

            Assert.AreEqual(0, this.model.ExactKeywords.Count);
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
            this.model.Users.Add(textModel);

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