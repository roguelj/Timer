using Serilog;

namespace Timer.Shared.ViewModels
{
    public abstract class AboutViewModel : Base
    {

        // member variables
        private string _title = string.Empty;
        private string _text = string.Empty;
        private string _viewVersion = string.Empty;
        private string _sharedVersion = string.Empty;


        // bound properties
        public string Title
        {
            get => this._title;
            set => this.SetProperty(ref this._title, value);
        }

        public string Text
        {
            get => this._text;
            set => this.SetProperty(ref this._text, value);
        }

        public string ViewVersion
        {
            get => this._viewVersion;
            set => this.SetProperty(ref this._viewVersion, value);
        }

        public string SharedVersion
        {
            get => this._sharedVersion;
            set => this.SetProperty(ref this._sharedVersion, value);
        }


        // constructor
        public AboutViewModel(ILogger logger) : base(logger) {}


    }

}
