namespace ComfyBot.Application.Shared
{
    public abstract class InitializableTab
    {
        private bool isSelected;
        private bool isInitialized;

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                this.isSelected = value;
                if (!this.isInitialized && value)
                {
                    this.isInitialized = true;
                    this.Initialize();
                }
            }
        }

        protected abstract void Initialize();
    }
}