using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Features.Variables;
using ComfyBot.Application.Patterns.Outcomes;
using ComfyBot.Application.Shared;
using ComfyBot.Application.Shared.Extensions;
using ComfyBot.Application.Shared.Services;
using ComfyBot.Application.Shared.Wrappers;

namespace ComfyBot.Application.Variables;

public class VariablesTabViewModel : InitializableTab
{
    private readonly IScopedServiceProvider provider;
    private readonly IMessageBox messageBox;

    public VariablesTabViewModel(IScopedServiceProvider provider, IMessageBox messageBox)
    {
        this.provider = provider;
        this.messageBox = messageBox;

        this.AddVariableCommand = new DelegateCommand(this.AddVariable);
        this.RemoveVariableCommand = new ParameterCommand(this.RemoveVariable);
    }

    public DelegateCommand AddVariableCommand { get; }

    public ParameterCommand RemoveVariableCommand { get; set; }

    public ObservableCollection<VariableModel> Variables { get; set; } = [];

    protected override void Initialize()
    {
        using ScopedService<IQueryHandler<GetVariables.Query, GetVariables.Result>> getHandler = this.provider.Create<IQueryHandler<GetVariables.Query, GetVariables.Result>>();
        Outcome<GetVariables.Result> outcome = getHandler.Service.Handle(new GetVariables.Query()).Result;

        if (!outcome.IsSuccess)
        {
            return;
        }

        foreach (GetVariables.VariableEntry entry in outcome.Payload.Entries)
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

        using ScopedService<ICommandHandler<UpdateVariable.Command>> updateHandler = this.provider.Create<ICommandHandler<UpdateVariable.Command>>();
        updateHandler.Service.Handle(command).Wait();
    }

    private void AddVariable()
    {
        Guid id = Guid.NewGuid();
        this.Variables.Add(new VariableModel { Id = id.ToString() });

        using ScopedService<ICommandHandler<AddVariable.Command>> addHandler = this.provider.Create<ICommandHandler<AddVariable.Command>>();
        addHandler.Service.Handle(new AddVariable.Command(id)).Wait();
    }

    private void RemoveVariable(object parameter)
    {
        VariableModel model = (VariableModel)parameter;

        if (this.messageBox.Show(GetDeletionMessage(model), "Delete variable", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            this.Variables.Remove(model);
            using ScopedService<ICommandHandler<RemoveVariable.Command>> removeHandler = this.provider.Create<ICommandHandler<RemoveVariable.Command>>();
            removeHandler.Service.Handle(new RemoveVariable.Command(Guid.Parse(model.Id))).Wait();
        }
    }

    private static string GetDeletionMessage(VariableModel model)
    {
        return $"Do you want to delete the variable '{model.Name}'?";
    }
}