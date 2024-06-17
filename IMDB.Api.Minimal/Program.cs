using System.Text;
using IMDB.Api.Auth;
using IMDB.Api.Health;
using IMDB.Api.Mapping;
using IMDB.Api.Minimal.Endpoints;
using IMDB.Api.Swagger;
using IMDB.Application;
using IMDB.Application.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddAuthentication(ao =>
{
    ao.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    ao.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    ao.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jbo =>
{
    jbo.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true
    };
});
builder.Services.AddAuthorization(ao =>
{
    ao.AddPolicy(AuthConstants.AdminUserPolicyName,
        p => p.AddRequirements(new AdminAuthRequirement(configuration["ApiKey"]!)));

    ao.AddPolicy(AuthConstants.TrustedMemberPolicyName,
        p => p.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
            ctx.User.HasClaim(c => c is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" }))
        );
});
builder.Services.AddScoped<ApiKeyAuthFilter>();
builder.Services.AddResponseCaching();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddDatabase(configuration["Database:ConnectionString"]!);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("_health");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// app.UseCors() should be invoked before response caching
app.UseResponseCaching();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapApiEndpoints();

DbInitializer dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();