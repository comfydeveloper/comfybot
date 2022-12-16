namespace ComfyBot.Application.Tests.Shared
{
    using ComfyBot.Application.Shared;

    using NUnit.Framework;

    [TestFixture]
    public class ParameterCommandTests
    {
        private ParameterCommand command;

        private bool actionHasBeenExecuted;

        [Test]
        public void CanExecuteShouldReturnTrueWhenInjectedPredicateIsNull()
        {
            command = new ParameterCommand(TestAction);

            bool result = command.CanExecute(new object());

            Assert.IsTrue(result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanExecuteShouldEvaluatePredicate(bool parameter)
        {
            bool Predicate(object b) => (bool)b;
            command = new ParameterCommand(TestAction, Predicate);

            bool result = command.CanExecute(parameter);

            Assert.AreEqual(parameter, result);
        }

        [Test]
        public void ExecuteShouldExecuteAction()
        {
            command = new ParameterCommand(TestAction);

            command.Execute(new object());

            Assert.IsTrue(actionHasBeenExecuted);
        }

        private void TestAction(object parameter)
        {
            actionHasBeenExecuted = true;
        }

        [TearDown]
        public void TearDown()
        {
            actionHasBeenExecuted = false;
        }
    }
}