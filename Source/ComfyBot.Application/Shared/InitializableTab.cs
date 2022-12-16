namespace ComfyBot.Application.Shared
{
    public abstract class InitializableTab
    {
        private bool isSelected;
        private bool isInitialized;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (!isInitialized && value)
                {
                    isInitialized = true;
                    Initialize();
                }
            }
        }

        protected abstract void Initialize();
    }
}