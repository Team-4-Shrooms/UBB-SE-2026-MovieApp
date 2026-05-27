namespace MovieApp.Web.Models;

using System.Collections.Generic;
using MovieApp.DataLayer.Models;

public sealed class AmbassadorDashboardViewModel
{
    public bool IsAmbassador { get; init; }

    public string? ReferralCode { get; init; }

    public int RewardBalance { get; init; }

    public IEnumerable<ReferralHistoryItem> History { get; init; } = [];
}
