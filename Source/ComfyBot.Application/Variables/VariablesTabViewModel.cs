using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Features.Variables;
using ComfyBot.Application.Shared;
using ComfyBot.Application.Shared.Extensions;
using ComfyBot.Application.Shared.Wrappers;

namespace ComfyBot.Application.Variables;

public class VariablesTabViewModel : InitializableTab
{
    private readonly IQueryHandler<GetVariables.Query, GetVariables.Result> getHandler;
    private readonly ICommandHandler<AddVariable.Command> addHandler;
    private readonly ICommandHandler<UpdateVariable.Command> updateHandler;
    private readonly ICommandHandler<RemoveVariable.Command> removeHandler;
    private readonly IMessageBox messageBox;

    public VariablesTabViewModel(
        IQueryHandler<GetVariables.Query, GetVariables.Result> getHandler,
        ICommandHandler<AddVariable.Command> addHandler,
        ICommandHandler<UpdateVariable.Command> updateHandler,
        ICommandHandler<RemoveVariable.Command> removeHandler,
        IMessageBox messageBox)
    {
        this.getHandler = getHandler;
        this.addHandler = addHandler;
        this.updateHandler = updateHandler;
        this.removeHandler = removeHandler;
        this.messageBox = messageBox;

        this.AddVariableCommand = new DelegateCommand(this.AddVariable);
        this.RemoveVariableCommand = new ParameterCommand(this.RemoveVariable);
    }

    public DelegateCommand AddVariableCommand { get; }

    public ParameterCommand RemoveVariableCommand { get; set; }

    public ObservableCollection<VariableModel> Variables { get; set; } = [];

    protected override void Initialize()
    {
        GetVariables.Result result = this.getHandler.Handle(new GetVariables.Query()).Result;

        foreach (GetVariables.VariableEntry entry in result.Entries)
        {
            VariableModel model = new()
            {
                Id = entry.Id.ToString(),
                Name = entry.Name,
                Value = entry.Value
            };

            this.Variables.Add(model);
        }

        this.Variables.RegisterCollectionItemChanged(this.OnVariableUpdate);
    }

    private void OnVariableUpdate(object sender, PropertyChangedEventArgs e)
    {
        VariableModel model = (VariableModel)sender;

        UpdateVariable.Command command = new(
            Guid.Parse(model.Id),
            model.Name,
            model.Value);

        this.updateHandler.Handle(command);
    }

    private void AddVariable()
    {
        Guid id = Guid.NewGuid();
        this.Variables.Add(new VariableModel { Id = id.ToString() });

        this.addHandler.Handle(new AddVariable.Command(id));
    }

    private void RemoveVariable(object parameter)
    {
        VariableModel model = (VariableModel)parameter;

        if (this.messageBox.Show(GetDeletionMessage(model), "Delete variable", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            this.Variables.Remove(model);
            this.removeHandler.Handle(new RemoveVariable.Command(Guid.Parse(model.Id)));
        }
    }

    private static string GetDeletionMessage(VariableModel model)
    {
        return $"Do you want to delete the variable '{model.Name}'?";
    }
}