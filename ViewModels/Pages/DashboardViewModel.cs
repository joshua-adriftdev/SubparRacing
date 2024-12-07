using System.Diagnostics;
using System.Windows.Controls;

namespace SubparRacing.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        public event Action? OnButtonClick;
        public event Action<string>? OnTextChange;

        [ObservableProperty]
        public string revCount = "No RPM";

        [ObservableProperty]
        public string isWaiting = "VISIBLE";

        [ObservableProperty]
        public string flags = "";

        [ObservableProperty]
        public string flagVisisble = "HIDDEN";

        [ObservableProperty]
        public string colour = "BLACK";

        [RelayCommand]
        private void OnCounterIncrement()
        {
            OnButtonClick?.Invoke();
        }
    }
}
