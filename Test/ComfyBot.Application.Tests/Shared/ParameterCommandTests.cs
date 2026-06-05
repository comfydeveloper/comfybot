using ComfyBot.Application.Shared;
using NUnit.Framework;
using Shouldly;

namespace ComfyBot.Application.Tests.Shared;

[TestFixture]
public class ParameterCommandTests
{
    private ParameterCommand command;

    private bool actionHasBeenExecuted;

    [Test]
    public void CanExecuteShouldReturnTrueWhenInjectedPredicateIsNull()
    {
        this.command = new ParameterCommand(this.TestAction);

        bool result = this.command.CanExecute(new object());

        result.ShouldBeTrue();
    }

    [TestCase(true)]
    [TestCase(false)]
    public void CanExecuteShouldEvaluatePredicate(bool parameter)
    {
        bool Predicate(object b) => (bool)b;
        this.command = new ParameterCommand(this.TestAction, Predicate);

        bool result = this.command.CanExecute(parameter);

        result.ShouldBe(parameter);
    }

    [Test]
    public void ExecuteShouldExecuteAction()
    {
        this.command = new ParameterCommand(this.TestAction);

        this.command.Execute(new object());

        this.actionHasBeenExecuted.ShouldBeTrue();
    }

    private void TestAction(object parameter)
    {
        this.actionHasBeenExecuted = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.actionHasBeenExecuted = false;
    }
}