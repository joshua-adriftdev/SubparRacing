using SubparRacing.ViewModels.Pages;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.Intrinsics.Arm;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace SubparRacing.Views.Pages
{
    public partial class LayoutPage : INavigableView<LayoutViewModel>
    {
        public LayoutViewModel ViewModel { get; }

        public int value = 0;

        public LayoutPage(LayoutViewModel viewModel)
        {
            ViewModel = viewModel;

            DataContext = this;

            App.TelemetryService.OnStart += OnStart;

            viewModel.OnButtonClick += OnButtonClick;

            InitializeComponent();
        }


        private void OnStart()
        {
            Debug.WriteLine("No longer waiting");
            
        }

        private void OnButtonClick() {
            DisplayLayout layout = App.DisplayLayoutManager.LoadLayout();
            DisplayElement e = new DisplayElement();
            e.ID = "EXAMPLE";
            e.Label = "From PC";
            e.type = DisplayElementType.LABELLEDDATAPOINT;
            e.X = 0;
            e.Y = 280;
            layout.elements.Add(e);

            App.DisplayLayoutManager.SaveLayout(layout);
        }

        public void OnTextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine("A THING HAS HAPPENED (Label Editied)");
            Wpf.Ui.Controls.TextBox? textBox = sender as Wpf.Ui.Controls.TextBox;



            if (textBox != null)
            {
                // Get the new text value
                string newText = textBox.Text;

                // Use the new text value as needed
                Debug.WriteLine("New text: " + newText);
            }
            else
            {
                Debug.WriteLine("Text box = null");
            }
        }

    }
}
