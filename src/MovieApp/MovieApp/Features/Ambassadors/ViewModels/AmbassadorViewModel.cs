using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Features.Ambassadors.ViewModels
{
    public partial class AmbassadorViewModel : ObservableObject
    {
        private readonly IAmbassadorService _ambassadorService;

        [ObservableProperty]
        private ObservableCollection<AmbassadorProfile> _ambassadors = new();

        public AmbassadorViewModel(IAmbassadorService ambassadorService)
        {
            _ambassadorService = ambassadorService;
            LoadAmbassadorsAsync();
        }

        [RelayCommand]
        private async Task LoadAmbassadorsAsync()
        {
            var data = await _ambassadorService.GetAllAmbassadorsAsync();
            Ambassadors.Clear();
            foreach (var profile in data)
            {
                Ambassadors.Add(profile);
            }
        }
    }
}
