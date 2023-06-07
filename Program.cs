using Microsoft.OpenApi.Models;

using urlshrink.Data;

// Create app builder
var builder = WebApplication.CreateBuilder(args);
// Configure app builder to use Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "URLShrink API", Description = "Shortens arbitrary URLs", Version = "v1" });
    c.EnableAnnotations();
});

// Create app from builder factory
var app = builder.Build();

// Add Swagger endpoints to app
app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "URLShrink API");
});

// Add API routes
app.MapPost("/api/url", (IncomingData data) => {
    // Create a new short URL
    try
    {
        string shorter = UrlData.AddUrl(data.url);
        return Results.Ok(new OutgoingData(shorter));
    }
    catch(TimeoutException te)
    {
        Console.WriteLine(te);
        return Results.Problem(statusCode: 500);
    }
})
.Produces<OutgoingData>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

app.MapGet("/", (HttpContext http) => {
    // Return the homepage
    var homePage = File.ReadAllText(@"./public/index.html");
    http.Response.Headers.ContentType = "text/html";
    return homePage;
})
.Produces(StatusCodes.Status200OK, contentType: "text/html");

app.MapGet("/{*rest}", (HttpContext http, string rest) => {
    bool viewOnly = rest.EndsWith('-');
    if(viewOnly) rest = rest.Substring(0, rest.Length - 1);
    var original = UrlData.GetOriginal(rest);
    if(original == null) return Results.NotFound();
    if(viewOnly) return Results.Text(original);
    else return Results.Redirect(original, false, false);
})
.Produces(StatusCodes.Status200OK, contentType: "text/plain")
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status302Found);

// Start listening for HTTP traffic
app.Run();
