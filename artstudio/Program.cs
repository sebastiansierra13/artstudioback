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


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.AccessDeniedPath = "/api/auth/denied";
    });


builder.Services.AddAuthorization();


builder.Services.AddCors(options => options.AddPolicy("AllowAngularOrigins",
                                    builder => builder.AllowAnyOrigin()
                                                    .WithOrigins("https://artstudio.com.co")
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod()
                                                    .AllowCredentials())); // Permite el uso de cookies));

builder.Services.AddHttpClient();
builder.Services.AddScoped<IInstagramService, InstagramService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
// Configuración de PayU desde variables de entorno
try
{
    builder.Services.Configure<PayUSettings>(options =>
    {
        options.MerchantId = Environment.GetEnvironmentVariable("PayU_MerchantId");
        options.AccountId = Environment.GetEnvironmentVariable("PayU_AccountId");
        options.ApiKey = Environment.GetEnvironmentVariable("PayU_ApiKey");
        options.ResponseUrl = Environment.GetEnvironmentVariable("PayU_ResponseUrl") ;
        options.ConfirmationUrl = Environment.GetEnvironmentVariable("PayU_ConfirmationUrl");
        // Convertir el valor de la variable de entorno "PayU_TestMode" en booleano
        var testMode = Environment.GetEnvironmentVariable("PayU_TestMode");
        options.TestMode = !string.IsNullOrEmpty(testMode) && testMode == "1";  // Si es "1", lo asigna como true, de lo contrario false
        options.SandboxUrl = Environment.GetEnvironmentVariable("PayU_SandboxUrl") ?? "https://sandbox.checkout.payulatam.com/ppp-web-gateway-payu/";
        options.ProductionUrl = Environment.GetEnvironmentVariable("PayU_ProductionUrl") ?? "https://checkout.payulatam.com/ppp-web-gateway-payu/";
    });

}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}


builder = WebApplication.CreateBuilder(args);

// Agrega esto para asegurarte de que escucha en todas las interfaces
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // Puerto HTTP
    serverOptions.ListenAnyIP(443, listenOptions => listenOptions.UseHttps()); // Puerto HTTPS
});


// Configuración de Instagram desde variables de entorno
builder.Services.Configure<InstagramSettings>(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("Instagram_ClientId") ?? "790062323292797";
    options.ClientSecret = Environment.GetEnvironmentVariable("Instagram_ClientSecret") ?? "2d55fca19782ec6f9f77e5d48b34b943";
});




builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IPayUService, PayUService>();
builder.Services.AddDbContext<MiDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("connectMPDis"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("connectMPDis"))));


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