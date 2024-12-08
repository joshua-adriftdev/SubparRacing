using System.Diagnostics;
using System.Windows.Controls;

namespace SubparRacing.ViewModels.Pages
{
    public partial class LayoutViewModel : ObservableObject
    {
        public event Action? OnButtonClick;
        public event Action<string>? OnTextChange;

        [ObservableProperty]
        public string revCount = "No RPM";

        [RelayCommand]
        private void SaveData()
        {
            OnButtonClick?.Invoke();
            Debug.WriteLine("Sending Data");

        }
    }
}
