namespace ComfyBot.Application.Shared
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Annotations;

    public abstract class NotifyingModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}