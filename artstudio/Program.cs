using artstudio.Configuration;
using artstudio.Models;
using artstudio.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
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
                                                    .AllowAnyMethod()
                                                    .AllowCredentials())); // Permite el uso de cookies));

builder.Services.AddHttpClient();
builder.Services.AddScoped<IInstagramService, InstagramService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
// Configuración de PayU desde variables de entorno
builder.Services.Configure<PayUSettings>(options =>
{
    options.MerchantId = Environment.GetEnvironmentVariable("PayU_MerchantId") ?? throw new ArgumentNullException("PayU_MerchantId");
    options.AccountId = Environment.GetEnvironmentVariable("PayU_AccountId") ?? throw new ArgumentNullException("PayU_AccountId");
    options.ApiKey = Environment.GetEnvironmentVariable("PayU_ApiKey") ?? throw new ArgumentNullException("PayU_ApiKey");
    options.ResponseUrl = Environment.GetEnvironmentVariable("PayU_ResponseUrl") ?? throw new ArgumentNullException("PayU_ResponseUrl");
    options.ConfirmationUrl = Environment.GetEnvironmentVariable("PayU_ConfirmationUrl") ?? throw new ArgumentNullException("PayU_ConfirmationUrl");
    options.TestMode = Environment.GetEnvironmentVariable("PayU_TestMode") == "1"; // true si está en modo sandbox

    // URLs de sandbox y producción
    options.SandboxUrl = Environment.GetEnvironmentVariable("PayU_SandboxUrl") ?? "https://sandbox.checkout.payulatam.com/ppp-web-gateway-payu/";
    options.ProductionUrl = Environment.GetEnvironmentVariable("PayU_ProductionUrl") ?? "https://checkout.payulatam.com/ppp-web-gateway-payu/";
});

// Configuración de Instagram desde variables de entorno
builder.Services.Configure<InstagramSettings>(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("Instagram_ClientId");
    options.ClientSecret = Environment.GetEnvironmentVariable("Instagram_ClientSecret");
});



builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IPayUService, PayUService>();
builder.Services.AddDbContext<MiDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("connectMPDis"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("connectMPDis"))));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.AccessDeniedPath = "/api/auth/denied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication(); // Añadir antes de UseAuthorization
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("AllowAngularOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();