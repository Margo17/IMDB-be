using System.Text;
using IMDB.Api;
using IMDB.Api.Mapping;
using IMDB.Application;
using IMDB.Application.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
        p => p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));

    ao.AddPolicy(AuthConstants.TrustedMemberPolicyName,
        p => p.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
            ctx.User.HasClaim(c => c is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" }))
        );
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

DbInitializer dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();