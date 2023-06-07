using Microsoft.OpenApi.Models;

// Create app builder
var builder = WebApplication.CreateBuilder(args);
// Configure app builder to use Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "URLShrink API", Description = "Shortens arbitrary URLs", Version = "v1" });
});

// Create app from builder factory
var app = builder.Build();

// Add Swagger endpoints to app
app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "URLShrink API");
});

// Add routes
app.MapGet("/", () => "Hello World!");

// Start listening for HTTP traffic
app.Run();
