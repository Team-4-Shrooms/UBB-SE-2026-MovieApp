using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Models
{
    public sealed class Reward
    {
        public int RewardId { get; set; }

        /// <summary>
        /// Gets the category or type of the reward.
        /// </summary>
        required public string RewardType { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the reward has been used.
        /// </summary>
        public bool RedemptionStatus { get; set; }

        /// <summary>
        /// Gets or sets the logic scope where this reward can be applied.
        /// </summary>
        public string ApplicabilityScope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the monetary or percentage discount value.
        /// </summary>
        public double DiscountValue { get; set; }

        /// <summary>
        /// Gets the unique identifier of the user who owns this reward.
        /// </summary>
        required public int OwnerUserId { get; init; }

        /// <summary>
        /// Gets or sets the specific event this reward is linked to, if any.
        /// </summary>
        public int? EventId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the reward is available for redemption.
        /// </summary>
        public bool IsAvailable => !this.RedemptionStatus;

        /// <summary>
        /// Marks the reward as successfully redeemed.
        /// </summary>
        public void Redeem()
        {
            this.RedemptionStatus = true;
        }
    }
}
