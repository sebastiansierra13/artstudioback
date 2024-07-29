using artstudio.Models;
using artstudio.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MiDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("connectMPDis"),
    ServerVersion.Parse("8.4.0-mysql")));

builder.Configuration.AddJsonFile("appsettings.json");


builder.Services.AddCors(options => options.AddPolicy("AllowAngularOrigins",
                                    builder => builder.AllowAnyOrigin()
                                                    .WithOrigins("http://localhost:4200")
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod()));

builder.Services.AddHttpClient();
builder.Services.AddScoped<IInstagramService, InstagramService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();