using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Features.Referrals.ViewModels
{
    public class ReferralViewModel : INotifyPropertyChanged
    {
        private readonly IAmbassadorService _ambassadorService;
        private readonly ICurrentUserService _currentUserService;

        private string _referralCode = string.Empty;
        private string _statusMessage = string.Empty;
        private string _codeInput = string.Empty;

        public ObservableCollection<ReferralHistoryItem> History { get; } = new ObservableCollection<ReferralHistoryItem>();

        public string ReferralCode
        {
            get => _referralCode;
            private set
            {
                _referralCode = value;
                OnPropertyChanged(nameof(ReferralCode));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public string CodeInput
        {
            get => _codeInput;
            set
            {
                _codeInput = value;
                OnPropertyChanged(nameof(CodeInput));
            }
        }

        public bool IsHistoryEmpty => History.Count == 0;

        public Visibility EmptyMessageVisibility =>
            IsHistoryEmpty ? Visibility.Visible : Visibility.Collapsed;

        public IAsyncRelayCommand LoadDataCommand { get; }

        public IAsyncRelayCommand ApplyCodeCommand { get; }

        public ReferralViewModel(IAmbassadorService ambassadorService, ICurrentUserService currentUserService)
        {
            _ambassadorService = ambassadorService;
            _currentUserService = currentUserService;
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            ApplyCodeCommand = new AsyncRelayCommand(ApplyCodeAsync);
            History.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(IsHistoryEmpty));
                OnPropertyChanged(nameof(EmptyMessageVisibility));
            };
            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            int userId = _currentUserService.UserId;
            if (userId <= 0)
            {
                StatusMessage = "Please log in to view your referrals.";
                return;
            }

            try
            {
                string? code = await _ambassadorService.GetReferralCodeAsync(userId);
                ReferralCode = code ?? string.Empty;

                History.Clear();
                IEnumerable<ReferralHistoryItem> items = await _ambassadorService.GetReferralHistoryAsync(userId);
                foreach (ReferralHistoryItem item in items)
                {
                    History.Add(item);
                }

                StatusMessage = string.IsNullOrEmpty(ReferralCode)
                    ? "You don't have a referral code yet."
                    : string.Empty;
            }
            catch (Exception)
            {
                StatusMessage = "Referral service unavailable. Please try again later.";
            }
        }

        public async Task ApplyCodeAsync()
        {
            string code = (CodeInput ?? string.Empty).Trim();
            if (code.Length == 0)
            {
                StatusMessage = "Enter a referral code.";
                return;
            }

            try
            {
                int userId = _currentUserService.UserId;
                int? ownerId = await _ambassadorService.ResolveCodeToUserIdAsync(code);

                if (ownerId is null)
                {
                    StatusMessage = "Invalid referral code.";
                    return;
                }

                if (ownerId.Value == userId)
                {
                    StatusMessage = "You cannot use your own referral code.";
                    return;
                }

                StatusMessage = "Code accepted. It will be applied when you join an event.";
            }
            catch (Exception)
            {
                StatusMessage = "Referral service unavailable. Please try again later.";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
