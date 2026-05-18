// <copyright file="Movie.cs" company="MovieApp">
// Copyright (c) MovieApp. All rights reserved.
// </copyright>

// =============================================================================
// PR CONFLICT RESOLUTION NOTES  (Movie-Model-Unification-Ianis)
// =============================================================================
// Rule: 927/1 field names win all conflicts.
//
//   Id               – identical, no conflict
//   Title            – identical, no conflict
//   Description      – both had it; 927 wins (no default) → added = string.Empty
//   ReleaseYear      – identical, no conflict
//   PosterUrl        – 927: string?  /  925: string  → 927 wins: string?
//   Synopsis         – 927 only → kept
//   DurationMinutes  – 925 only, no conflict → ADDED
//   Rating           – 927: decimal Rating  /  925: double AverageRating
//                      → 927 wins; AverageRating kept as [NotMapped] double alias
//   Price            – 927 only → kept
//   PrimaryGenre     – 927: string  /  925: List<Genre>
//                      → BOTH kept; string for DB/pricing, List for display
//   Genre            – 927 [NotMapped] alias → kept
//   IsOnSale         – 927 only → kept
//   ActiveSaleDiscountPercent – 927 only → kept
//   ActiveSale       – 927 only → kept
//   Actors           – 925 only → ADDED (needs Actor.cs copied to DataLayer/Models)
//   Directors        – 925 only → ADDED (needs Director.cs copied to DataLayer/Models)
//   Reviews          – 925 only → ADDED (needs Review.cs copied to DataLayer/Models)
//   Comments         – 925 only → ADDED (needs Comment.cs copied to DataLayer/Models)
//   GenreDisplay     – 925 computed [NotMapped] → ADDED, falls back to PrimaryGenre
//   HasActiveSale / pricing methods / ToString – 927 → kept
// =============================================================================

namespace MovieApp.DataLayer.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

/// <summary>
/// Unified Movie entity. Merges 927/1 DataLayer/Models/Movie.cs (field-name authority)
/// with 925/1 Core/Models/Movie.cs (Genres, Actors, Directors, Reviews, Comments, DurationMinutes).
/// </summary>
public class Movie
{
    // -------------------------------------------------------------------------
    // Identity
    // -------------------------------------------------------------------------

    public int Id { get; set; }

    // -------------------------------------------------------------------------
    // Core metadata
    // -------------------------------------------------------------------------

    public string Title { get; set; } = string.Empty;

    /// <summary>Plot summary. 927 field name wins; both versions used "Description".</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Extended synopsis. 927 only.</summary>
    public string Synopsis { get; set; } = string.Empty;

    public int ReleaseYear { get; set; }

    /// <summary>Running time in minutes. Added from 925, no conflict.</summary>
    public int DurationMinutes { get; set; }

    /// <summary>Poster image URL. 927 wins with nullable string.</summary>
    public string? PosterUrl { get; set; }

    // -------------------------------------------------------------------------
    // Genre / Classification
    // -------------------------------------------------------------------------

    /// <summary>
    /// Primary genre as a plain string — used by pricing/sale logic (927).
    /// 927 field name wins over 925's collection-only approach.
    /// </summary>
    public string PrimaryGenre { get; set; } = string.Empty;

    /// <summary>[NotMapped] backward-compat alias for PrimaryGenre.</summary>
    [NotMapped]
    public string Genre { get => PrimaryGenre; set => PrimaryGenre = value; }

    /// <summary>
    /// Rich Genre-entity collection from 925.
    /// Use for navigation/display; PrimaryGenre for simple string lookups.
    /// </summary>
    public List<Genre> Genres { get; set; } = new List<Genre>();

    /// <summary>
    /// Comma-separated display string. Falls back to PrimaryGenre
    /// when the Genres collection is empty.
    /// </summary>
    [NotMapped]
    public string GenreDisplay =>
        Genres != null && Genres.Any()
            ? string.Join(", ", Genres.Select(g => g.Name))
            : (string.IsNullOrEmpty(PrimaryGenre) ? "No Genre" : PrimaryGenre);

    // -------------------------------------------------------------------------
    // Rating
    // -------------------------------------------------------------------------

    /// <summary>
    /// Movie rating (e.g. 8.5 / 10).
    /// 927 type (decimal) and name (Rating) win over 925's double AverageRating.
    /// </summary>
    public decimal Rating { get; set; }

    /// <summary>[NotMapped] 925 compatibility alias for Rating.</summary>
    [NotMapped]
    public double AverageRating
    {
        get => (double)Rating;
        set => Rating = (decimal)value;
    }

    // -------------------------------------------------------------------------
    // Pricing & Sales  (927 only)
    // -------------------------------------------------------------------------

    public decimal Price { get; set; }

    public bool IsOnSale { get; set; }

    public decimal? ActiveSaleDiscountPercent { get; set; }

    public ActiveSale? ActiveSale { get; set; }

    public bool HasActiveSale => ActiveSaleDiscountPercent is decimal d && d > 0;

    [NotMapped]
    public string OriginalPriceText => Price.ToString("0.00");

    [NotMapped]
    public string DiscountedPriceText => GetEffectivePrice().ToString("0.00");

    // -------------------------------------------------------------------------
    // Cast & Crew  (925 only — added, no conflict)
    // Requires Actor.cs and Director.cs in MovieApp.DataLayer.Models
    // -------------------------------------------------------------------------

    public List<Actor> Actors { get; set; } = new List<Actor>();

    public List<Director> Directors { get; set; } = new List<Director>();

    // -------------------------------------------------------------------------
    // User Interaction  (925 only — added, no conflict)
    // Requires Review.cs and Comment.cs in MovieApp.DataLayer.Models
    // -------------------------------------------------------------------------

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    // -------------------------------------------------------------------------
    // Business methods  (927)
    // -------------------------------------------------------------------------

    public decimal GetEffectivePrice()
    {
        return HasActiveSale
            ? decimal.Round(
                Price * (1 - (ActiveSaleDiscountPercent!.Value / 100m)),
                2,
                MidpointRounding.AwayFromZero)
            : Price;
    }

    public decimal GetDiscountedPrice(decimal discountPercentage)
    {
        return Price * (1 - (discountPercentage / 100.0m));
    }

    // -------------------------------------------------------------------------
    // Overrides
    // -------------------------------------------------------------------------

    /// <summary>927 format wins: "Title (Year) — Genre".</summary>
    public override string ToString() => $"{Title} ({ReleaseYear}) — {Genre}";
}
