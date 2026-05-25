using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.DataLayer.Models
{
    /// <summary>
    /// Represents a valid (Genre, Actor, Director) reel combination
    /// derived from a movie that has at least one active screening.
    /// </summary>
    public sealed class ReelCombination
    {
        /// <summary>
        /// Gets Genre of the ReelCombination.
        /// </summary>
        required public Genre Genre { get; init; }

        /// <summary>
        /// Gets Actor of the ReelCombination.
        /// </summary>
        required public Actor Actor { get; init; }

        /// <summary>
        /// Gets Director of the ReelCombination.
        /// </summary>
        required public Director Director { get; init; }
    }
}
