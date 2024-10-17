namespace SubparRacing.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty]
        public string revCount = "No RPM";

        [ObservableProperty]
        public string isWaiting = "VISIBLE";

        [ObservableProperty]
        public string flags = "";

        [ObservableProperty]
        public string yellow = "HIDDEN";

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }
    }
}
