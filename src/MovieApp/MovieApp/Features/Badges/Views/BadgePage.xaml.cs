using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MovieApp.Features.Badges.ViewModels;

namespace MovieApp.Features.Badges.Views
{
    public sealed partial class BadgePage : Page
    {
        public BadgeViewModel ViewModel { get; }

        public BadgePage()
        {
            ViewModel = App.Services.GetRequiredService<BadgeViewModel>();
            this.InitializeComponent();
        }
    }
}
