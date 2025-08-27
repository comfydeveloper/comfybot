using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Features.Variables;
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
        using var getHandler = this.provider.Create<IQueryHandler<GetVariables.Query, GetVariables.Result>>();
        GetVariables.Result result = getHandler.Service.Handle(new GetVariables.Query()).Result;

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

        using var updateHandler = this.provider.Create<ICommandHandler<UpdateVariable.Command>>();
        updateHandler.Service.Handle(command);
    }

    private void AddVariable()
    {
        Guid id = Guid.NewGuid();
        this.Variables.Add(new VariableModel { Id = id.ToString() });

        using var addHandler = this.provider.Create<ICommandHandler<AddVariable.Command>>();
        addHandler.Service.Handle(new AddVariable.Command(id));
    }

    private void RemoveVariable(object parameter)
    {
        VariableModel model = (VariableModel)parameter;

        if (this.messageBox.Show(GetDeletionMessage(model), "Delete variable", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            this.Variables.Remove(model);
            using var removeHandler = this.provider.Create<ICommandHandler<RemoveVariable.Command>>();
            removeHandler.Service.Handle(new RemoveVariable.Command(Guid.Parse(model.Id)));
        }
    }

    private static string GetDeletionMessage(VariableModel model)
    {
        return $"Do you want to delete the variable '{model.Name}'?";
    }
}