namespace IMDB.Application.Models;

public class GetAllMoviesOptions
{
    public string? Title { get; set; }
    
    public int? Year { get; set; }

    public Guid? UserId { get; set; }
}