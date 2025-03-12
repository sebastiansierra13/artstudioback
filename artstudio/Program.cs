using artstudio.Configuration;
using artstudio.Models;
using artstudio.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuración de JSON en controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de la base de datos
builder.Services.AddDbContext<MiDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("connectMPDis"),
    ServerVersion.Parse("8.4.0-mysql")));

builder.Configuration.AddJsonFile("appsettings.json");

// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.AccessDeniedPath = "/api/auth/denied";
    });

builder.Services.AddAuthorization();

// Configuración de CORS para permitir solicitudes desde Angular
builder.Services.AddCors(options => options.AddPolicy("AllowAngularOrigins",
                                    policy => policy.WithOrigins("https://artstudio.com.co")
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod()
                                                    .AllowCredentials()));

builder.Services.AddHttpClient();
builder.Services.AddScoped<IInstagramService, InstagramService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Configuración de PayU desde variables de entorno
builder.Services.Configure<PayUSettings>(options =>
{
    options.MerchantId = Environment.GetEnvironmentVariable("PayU_MerchantId");
    options.AccountId = Environment.GetEnvironmentVariable("PayU_AccountId");
    options.ApiKey = Environment.GetEnvironmentVariable("PayU_ApiKey");
    options.ResponseUrl = Environment.GetEnvironmentVariable("PayU_ResponseUrl");
    options.ConfirmationUrl = Environment.GetEnvironmentVariable("PayU_ConfirmationUrl");
    options.TestMode = Environment.GetEnvironmentVariable("PayU_TestMode") == "1";
    options.SandboxUrl = Environment.GetEnvironmentVariable("PayU_SandboxUrl") ?? "https://sandbox.checkout.payulatam.com/ppp-web-gateway-payu/";
    options.ProductionUrl = Environment.GetEnvironmentVariable("PayU_ProductionUrl") ?? "https://checkout.payulatam.com/ppp-web-gateway-payu/";
});

// Configuración de Kestrel (Solo HTTP en producción)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5003); // Solo HTTP
});

// Configuración de Instagram desde variables de entorno
builder.Services.Configure<InstagramSettings>(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("Instagram_ClientId") ?? "790062323292797";
    options.ClientSecret = Environment.GetEnvironmentVariable("Instagram_ClientSecret") ?? "2d55fca19782ec6f9f77e5d48b34b943";
});

// Agregar servicios
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IPayUService, PayUService>();

var app = builder.Build();

// Configuración de Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowAngularOrigins");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
