// <copyright file="Movie.cs" company="MovieApp">
// Copyright (c) MovieApp. All rights reserved.
// </copyright>


namespace MovieApp.DataLayer.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public class Movie
{

    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Synopsis { get; set; } = string.Empty;

    public int ReleaseYear { get; set; }

    public int DurationMinutes { get; set; }

    public string? PosterUrl { get; set; }

    public string PrimaryGenre { get; set; } = string.Empty;

    [NotMapped]
    public string Genre { get => PrimaryGenre; set => PrimaryGenre = value; }

    public List<Genre> Genres { get; set; } = new List<Genre>();

    [NotMapped]
    public string GenreDisplay =>
        Genres != null && Genres.Any()
            ? string.Join(", ", Genres.Select(g => g.Name))
            : (string.IsNullOrEmpty(PrimaryGenre) ? "No Genre" : PrimaryGenre);

    public decimal Rating { get; set; }

    [NotMapped]
    public double AverageRating
    {
        get => (double)Rating;
        set => Rating = (decimal)value;
    }

    public decimal Price { get; set; }

    public bool IsOnSale { get; set; }

    public decimal? ActiveSaleDiscountPercent { get; set; }

    public ActiveSale? ActiveSale { get; set; }

    public bool HasActiveSale => ActiveSaleDiscountPercent is decimal d && d > 0;

    [NotMapped]
    public string OriginalPriceText => Price.ToString("0.00");

    [NotMapped]
    public string DiscountedPriceText => GetEffectivePrice().ToString("0.00");

    public List<Actor> Actors { get; set; } = new List<Actor>();

    public List<Director> Directors { get; set; } = new List<Director>();

  
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

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

    public override string ToString() => $"{Title} ({ReleaseYear}) — {Genre}";
}
