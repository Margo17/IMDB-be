using FluentValidation;
using IMDB.Application.Models;

namespace IMDB.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(o => o.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}