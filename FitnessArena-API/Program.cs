using Microsoft.AspNetCore.Authentication.JwtBearer;
using FitnessArena_API.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//looping lazy
builder.Services.AddControllers().AddNewtonsoftJson(
    o => o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

//db context
builder.Services.AddDbContext<fitnessgarageContext>
    (
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("con"))
    );

//cors
builder.Services.AddCors(
    o => {
        o.AddPolicy("cors",
        builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
    });
//jwt configs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    o => {
        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thesecretkeyisfitness_Garage"))
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors("cors");

app.UseAuthentication();

app.UseAuthorization();



app.MapControllers();

app.Run();
