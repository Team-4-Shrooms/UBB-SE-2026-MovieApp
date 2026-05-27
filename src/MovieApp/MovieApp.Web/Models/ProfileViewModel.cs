using System.Collections.Generic;
using MovieApp.DataLayer.Models;

namespace MovieApp.Web.Models
{
    public sealed class ProfileViewModel
    {
        public string Username { get; set; } = string.Empty;
        public UserStats? Stats { get; set; }
        public List<Badge> EarnedBadges { get; set; } = new List<Badge>();
        public List<Badge> AllBadges { get; set; } = new List<Badge>();
    }
}
