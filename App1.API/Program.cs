using App1.API.Requirement;
using Microsoft.AspNetCore.Authorization;
using SharedLib.Configurations;
using SharedLib.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CustomTokenConfiguration>(builder.Configuration.GetSection("TokenOptions"));
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenConfiguration>();

builder.Services.AddCustomTokenAuth(tokenOptions);
builder.Services.AddScoped<IAuthorizationHandler, BirthTimeRequirementHandler>();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Sivaslilar", policy =>
    {
        policy.RequireClaim("city", "sivas");
    });
    opt.AddPolicy("AgePolicy", policy =>
    {
        policy.Requirements.Add(new BirthDateRequirement(18));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
