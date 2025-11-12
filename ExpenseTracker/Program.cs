using ExpenseTracker.Data;
using ExpenseTracker.Data.Services;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddCors(opt => opt.AddPolicy("AllowAll", 
    opt => opt.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin())
);

//JWT Configuration for Authentication
var jwtSetting = builder.Configuration.GetSection("Jwt");


builder.Services.AddAuthentication((options) =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer((options) => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSetting["Issuer"],
        ValidAudience = jwtSetting["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting["Key"])),
    };
});
builder.Services.AddAuthorization();

var conc = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conc));
builder.Services.AddScoped<ITransactionServices, TransactionServices>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<PasswordHasher<User>>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) //// For Production int need to be commented
//{
    app.UseSwagger();
    app.UseSwaggerUI();
///}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
