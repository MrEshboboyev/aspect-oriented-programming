using AspectOrientedProgramming.API.Aspects;
using AspectOrientedProgramming.API.Middleware;
using AspectOrientedProgramming.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging with detailed timestamps and structured logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add a custom logging filter to capture more detailed information
builder.Logging.AddFilter("AspectOrientedProgramming.API", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);

builder.Services.AddControllers();

// NSwag Configuration: Adds the OpenAPI documentation services
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Aspect-Oriented API"; // Set your API Title
    config.Version = "v1";
    // Optional: add a description
    config.Description = "An API demonstrating Aspect-Oriented Programming (AOP) principles.";
});

//builder.Services.AddOpenApi();

// Add ASP.NET Core services
builder.Services.AddHttpContextAccessor();

// Add caching services
builder.Services.AddMemoryCache();

// Register our custom interceptors
builder.Services.AddScoped<LoggingInterceptor>();
builder.Services.AddScoped<PerformanceInterceptor>();
builder.Services.AddScoped<CachingInterceptor>();
builder.Services.AddScoped<ValidationInterceptor>();
builder.Services.AddScoped<SecurityInterceptor>();

// Register our services with proxy support using interfaces
builder.Services.AddProxiedScoped<IProductService, ProductService>(
    typeof(LoggingInterceptor),
    typeof(PerformanceInterceptor),
    typeof(CachingInterceptor),
    typeof(ValidationInterceptor));

builder.Services.AddProxiedScoped<IOrderService, OrderService>(
    typeof(LoggingInterceptor),
    typeof(PerformanceInterceptor),
    typeof(CachingInterceptor),
    typeof(ValidationInterceptor));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // NSwag Middleware: Serves the OpenAPI document (JSON file)
    app.UseOpenApi();

    // NSwag Middleware: Serves the Swagger UI
    // Access at: http://localhost:<port>/swagger
    app.UseSwaggerUi();

    //app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add correlation ID middleware
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
    
    // Seed data through the interface methods
    if (productService is ProductService productServiceImpl)
    {
        productServiceImpl.SeedData();
    }
    
    if (orderService is OrderService orderServiceImpl)
    {
        orderServiceImpl.SeedData();
    }
}

app.Run();
