using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using BusinessLogic.Ports;
using Serilog;
using System.Reflection;
using System.Text;
using BusinessLogic.Adapter.Marvel;
using BusinessLogic.Factory;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Mapper;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.WorkUnit;
using Infraestructure.Filters;
using BusinessLogic.Adapter.ComicsFavorito;
using Domain.Interface;
using Microsoft.IdentityModel.Tokens;
using Repository.ComicFavorito;
using Repository.Usuario;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.json");
builder.Configuration.AddJsonFile($"responsemessage.json", optional: false, reloadOnChange: true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Configurar Entity Framework con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Infrastructure")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Registro de Repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFavoriteComicsRepository, FavoriteComicRepository>();

// Registro de Servicios
builder.Services.AddScoped<IMarvelService, MarvelService>();
builder.Services.AddScoped<IFavoriteComicsService, FavoriteComicService>();
builder.Services.AddScoped<IFactoryLogic, FactoryLogic>();

// Registro del Unit of Work (si estás usando uno)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddSingleton<AsyncPolicy>(serviceProvider =>
    Policy.Handle<HttpRequestException>()
          .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

builder.Services.AddHttpClient("ExternalServiceClient")
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt)));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Validación DTOs
builder.Services.AddMvc(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Marvel API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 44321; 
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marvel API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers().AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory);
app.Run();
