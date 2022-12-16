namespace ComfyBot.Application.Tests.Shared;

using ComfyBot.Application.Shared;

using NUnit.Framework;

[TestFixture]
public class DelegateCommandTests
{
    private DelegateCommand command;

    private bool actionHasBeenExecuted;

    [Test]
    public void CanExecuteShouldReturnTrueWhenInjectedPredicateIsNull()
    {
        command = new DelegateCommand(TestAction);

        bool result = command.CanExecute(new object());

        Assert.IsTrue(result);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void CanExecuteShouldEvaluatePredicate(bool parameter)
    {
        bool Predicate(object b) => (bool)b;
        command = new DelegateCommand(TestAction, Predicate);

        bool result = command.CanExecute(parameter);

        Assert.AreEqual(parameter, result);
    }

    [Test]
    public void ExecuteShouldExecuteAction()
    {
        command = new DelegateCommand(TestAction);

        command.Execute(new object());

        Assert.IsTrue(actionHasBeenExecuted);
    }

    private void TestAction()
    {
        actionHasBeenExecuted = true;
    }

    [TearDown]
    public void TearDown()
    {
        actionHasBeenExecuted = false;
    }
}