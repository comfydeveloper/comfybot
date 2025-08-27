using ComfyBot.Application.Shared;

namespace ComfyBot.Application.Variables;

public class VariableModel : NotifyingModel
{
    private string name;
    private string value;

    public string Id { get; set; }

    public string Name
    {
        get => this.name;
        set
        {
            this.name = value;
            this.OnPropertyChanged();
        }
    }

    public string Value
    {
        get => this.value;
        set
        {
            this.value = value;
            this.OnPropertyChanged();
        }
    }
}