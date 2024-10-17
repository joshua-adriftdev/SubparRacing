using SubparRacing.ViewModels.Pages;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Wpf.Ui.Controls;

namespace SubparRacing.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public int value = 0;

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;

            DataContext = this;

            App.TelemetryService.OnStart += OnStart;
            App.TelemetryService.OnTelemetry += UpdateTelemetryDisplay;

            InitializeComponent();
        }

        private void OnStart()
        {
            Debug.WriteLine("No longer waiting");
            ViewModel.IsWaiting = "HIDDEN";
        }

        private void UpdateTelemetryDisplay()
        {
            TelemetryService telemetryService = App.TelemetryService;

            ViewModel.RevCount = "RPM: " + telemetryService.irsdk.Data.GetValue("RPM");

            uint bitField = telemetryService.irsdk.Data.GetBitField("SessionFlags");
            List<string> flags = GetActiveFlags(bitField);
            ViewModel.Flags = string.Join(",", flags);

            bool yellow = (bitField & Flags.Caution) != 0 || (bitField & Flags.CautionWaving) != 0 || (bitField & Flags.Yellow) != 0 || (bitField & Flags.YellowWaving) != 0;
            ViewModel.Yellow = yellow ? "VISIBLE" : "HIDDEN";

        }

        class Flags
        {
            public const uint Checkered = 0x0001;
            public const uint White = 0x0002;
            public const uint Green = 0x0004;
            public const uint Yellow = 0x0008;
            public const uint Red = 0x0010;
            public const uint Blue = 0x0020;
            public const uint Debris = 0x0040;
            public const uint Crossed = 0x0080;
            public const uint YellowWaving = 0x0100;
            public const uint OneLapToGreen = 0x0200;
            public const uint GreenHeld = 0x0400;
            public const uint TenToGo = 0x0800;
            public const uint FiveToGo = 0x1000;
            public const uint RandomWaving = 0x2000;
            public const uint Caution = 0x4000;
            public const uint CautionWaving = 0x8000;

            // Drivers black flags
            public const uint Black = 0x010000;
            public const uint Disqualify = 0x020000;
            public const uint Servicible = 0x040000; // Car is allowed service (not a flag)
            public const uint Furled = 0x080000;
            public const uint Repair = 0x100000;

            // Start lights
            public const uint StartHidden = 0x10000000;
            public const uint StartReady = 0x20000000;
            public const uint StartSet = 0x40000000;
            public const uint StartGo = 0x80000000;
        }

        static List<string> GetActiveFlags(uint sessionFlags)
        {
            var flagNames = new List<string>();

            if ((sessionFlags & Flags.Checkered) != 0)
                flagNames.Add("Checkered");
            if ((sessionFlags & Flags.White) != 0)
                flagNames.Add("White");
            if ((sessionFlags & Flags.Green) != 0)
                flagNames.Add("Green");
            if ((sessionFlags & Flags.Yellow) != 0)
                flagNames.Add("Yellow");
            if ((sessionFlags & Flags.Red) != 0)
                flagNames.Add("Red");
            if ((sessionFlags & Flags.Blue) != 0)
                flagNames.Add("Blue");
            if ((sessionFlags & Flags.Debris) != 0)
                flagNames.Add("Debris");
            if ((sessionFlags & Flags.Crossed) != 0)
                flagNames.Add("Crossed");
            if ((sessionFlags & Flags.YellowWaving) != 0)
                flagNames.Add("Yellow Waving");
            if ((sessionFlags & Flags.OneLapToGreen) != 0)
                flagNames.Add("One Lap to Green");
            if ((sessionFlags & Flags.GreenHeld) != 0)
                flagNames.Add("Green Held");
            if ((sessionFlags & Flags.TenToGo) != 0)
                flagNames.Add("Ten to Go");
            if ((sessionFlags & Flags.FiveToGo) != 0)
                flagNames.Add("Five to Go");
            if ((sessionFlags & Flags.RandomWaving) != 0)
                flagNames.Add("Random Waving");
            if ((sessionFlags & Flags.Caution) != 0)
                flagNames.Add("Caution");
            if ((sessionFlags & Flags.CautionWaving) != 0)
                flagNames.Add("Caution Waving");

            // Driver black flags
            if ((sessionFlags & Flags.Black) != 0)
                flagNames.Add("Black Flag");
            if ((sessionFlags & Flags.Disqualify) != 0)
                flagNames.Add("Disqualified");
            if ((sessionFlags & Flags.Servicible) != 0)
                flagNames.Add("Servicible");
            if ((sessionFlags & Flags.Furled) != 0)
                flagNames.Add("Furled");
            if ((sessionFlags & Flags.Repair) != 0)
                flagNames.Add("Repair");

            // Start lights
            if (((uint)sessionFlags & Flags.StartHidden) != 0)
                flagNames.Add("Start Hidden");
            if (((uint)sessionFlags & Flags.StartReady) != 0)
                flagNames.Add("Start Ready");
            if (((uint)sessionFlags & Flags.StartSet) != 0)
                flagNames.Add("Start Set");
            if (((uint)sessionFlags & Flags.StartGo) != 0)
                flagNames.Add("Start Go");

            return flagNames;
        }
    }
}
