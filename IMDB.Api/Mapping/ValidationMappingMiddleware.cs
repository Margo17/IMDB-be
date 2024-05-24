using FluentValidation;
using IMDB.Contracts.Responses;

namespace IMDB.Api.Mapping;

public class ValidationMappingMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            ValidationFailureResponse validationFailureResponse = new ValidationFailureResponse()
            {
                Errors = ex.Errors.Select(e => new ValidationResponse()
                {
                    PropertyName = e.PropertyName,
                    Message = e.ErrorMessage
                })
            };
            
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
    }
}