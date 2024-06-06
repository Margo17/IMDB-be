using FluentValidation;
using IMDB.Application.Models;
using IMDB.Application.Repositories;

namespace IMDB.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;
    
    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        
        RuleFor(m => m.Id)
            .NotEmpty();

        RuleFor(m => m.Genres)
            .NotEmpty();

        RuleFor(m => m.Title)
            .NotEmpty();

        RuleFor(m => m.Year)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
        
        RuleFor(m => m.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists in the system");
    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken cancellationToken = default)
    {
        Movie? existingMovie = await _movieRepository.GetBySlugAsync(slug);
        
        if (existingMovie is not null)
        {
            return existingMovie.Id == movie.Id;
        }

        return existingMovie is null;
    }
}