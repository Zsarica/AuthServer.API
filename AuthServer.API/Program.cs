using AuthServer.API.Controllers;
using AuthServer.Core.Configure;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UOW;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Service.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib.Configurations;
using SharedLib.SharedServices;
using SharedLib.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IAuthenticationService,AuthenticationService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),
        sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("AuthServer.Data");
        });
    
});

builder.Services.AddIdentity<AppUser, IdentityRole>(setupAction =>
{
    setupAction.User.RequireUniqueEmail = true;
    setupAction.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


builder.Services.Configure<CustomTokenConfiguration>(builder.Configuration.GetSection("TokenOptions"));
builder.Services.Configure<List<ClientProp>>(builder.Configuration.GetSection("ClientProp"));



builder.Services.AddAuthentication(configureOptions =>
{
    configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,opt =>
{
    var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenConfiguration>();
    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
        ValidateIssuerSigningKey=true,
        ValidateAudience=true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew=TimeSpan.Zero
    };
});



builder.Services.AddControllers().AddFluentValidation(opt =>
{
    opt.RegisterValidatorsFromAssemblyContaining<AuthController>();
});
builder.Services.UseCustomValidationResponse();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{

}
app.UseCustomException();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
