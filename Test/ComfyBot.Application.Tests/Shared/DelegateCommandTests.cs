using ComfyBot.Application.Shared;
using FluentAssertions;
using NUnit.Framework;

namespace ComfyBot.Application.Tests.Shared;

[TestFixture]
public class DelegateCommandTests
{
    private DelegateCommand command;

    private bool actionHasBeenExecuted;

    [Test]
    public void CanExecuteShouldReturnTrueWhenInjectedPredicateIsNull()
    {
        this.command = new DelegateCommand(this.TestAction);

        bool result = this.command.CanExecute(new object());

        result.Should().BeTrue();
    }

    [TestCase(true)]
    [TestCase(false)]
    public void CanExecuteShouldEvaluatePredicate(bool parameter)
    {
        bool Predicate(object b) => (bool)b;
        this.command = new DelegateCommand(this.TestAction, Predicate);

        bool result = this.command.CanExecute(parameter);

        result.Should().Be(parameter);
    }

    [Test]
    public void ExecuteShouldExecuteAction()
    {
        this.command = new DelegateCommand(this.TestAction);

        this.command.Execute(new object());

        this.actionHasBeenExecuted.Should().BeTrue();
    }

    private void TestAction()
    {
        this.actionHasBeenExecuted = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.actionHasBeenExecuted = false;
    }
}