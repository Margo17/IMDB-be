using FluentValidation;
using IMDB.Application.Models;

namespace IMDB.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] AcceptableSortFields = ["title", "year"];

    public GetAllMoviesOptionsValidator()
    {
        RuleFor(o => o.Year)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(o => o.SortField)
            .Must(sf => sf is null || AcceptableSortFields.Contains(sf, StringComparer.OrdinalIgnoreCase))
            .WithMessage("You can only sort by 'title' or 'year'");
    }
}