using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MovieApp.Features.Referrals.ViewModels;

namespace MovieApp.Features.Referrals.Views
{
    public sealed partial class ReferralPage : Page
    {
        public ReferralViewModel ViewModel { get; } = App.Services.GetRequiredService<ReferralViewModel>();

        public ReferralPage()
        {
            this.InitializeComponent();
        }
    }
}
