using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Interfaces.Services
{
    public interface ISlotMachineResultService
    {
        /// <summary>
        /// Prepares the final spin result model based on selected criteria and matching events.
        /// </summary>
        /// <param name="userIdentifier">The unique identifier of the spinning user.</param>
        /// <param name="genre">The genre selected by the slot reels.</param>
        /// <param name="actor">The actor selected by the slot reels.</param>
        /// <param name="director">The director selected by the slot reels.</param>
        /// <param name="matchingEvents">The collection of events that match the reel combination.</param>
        /// <param name="jackpotMovie">The specific movie result if a jackpot is achieved.</param>
        /// <returns>A model representing the result of the spin.</returns>
        Task<SlotMachineResult> PrepareSpinResultAsync(
            int userIdentifier,
            Genre genre,
            Actor actor,
            Director director,
            List<MovieEvent> matchingEvents,
            Movie? jackpotMovie);
    }
}
