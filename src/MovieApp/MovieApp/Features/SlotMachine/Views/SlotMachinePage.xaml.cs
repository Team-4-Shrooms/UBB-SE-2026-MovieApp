using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.Features.SlotMachine.ViewModels;

namespace MovieApp.Features.SlotMachine.Views
{
    public sealed partial class SlotMachinePage : Page
    {
        public SlotMachineViewModel ViewModel { get; } =
            App.Services.GetRequiredService<SlotMachineViewModel>();

        public SlotMachinePage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadStateCommand.ExecuteAsync(null);
        }
    }
}
