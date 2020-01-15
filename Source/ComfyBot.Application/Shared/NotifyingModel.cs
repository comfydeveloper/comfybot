namespace ComfyBot.Application.Shared
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using ComfyBot.Application.Annotations;

    public abstract class NotifyingModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}