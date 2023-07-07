using Prism.Mvvm;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.ViewModels
{
    public class Base:BindableBase
    {

        public ITimeLogService? TimeLogService { get; set; }

    }
}
