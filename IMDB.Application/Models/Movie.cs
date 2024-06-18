using System.Text.RegularExpressions;

namespace IMDB.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }

    public required string Title { get; set; }

    public string Slug => GenerateSlug();

    public float? Rating { get; set; }

    public int? UserRating { get; set; }

    public required int Year { get; set; }

    public required List<string> Genres { get; init; } = [];

    private string GenerateSlug()
    {
        string sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower()
            .Replace(' ', '-');

        return $"{sluggedTitle}-{Year}";
    }

    [GeneratedRegex("[^0-9 A-Z a-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}