using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MovieApp.Features.Ambassadors.ViewModels;

namespace MovieApp.Features.Ambassadors.Views
{
    public sealed partial class AmbassadorPage : Page
    {
        public AmbassadorViewModel ViewModel { get; }

        public AmbassadorPage()
        {
            ViewModel = App.Services.GetRequiredService<AmbassadorViewModel>();
            this.InitializeComponent();
        }
    }
}
