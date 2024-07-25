using NSwag.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// The following highlighted code adds the database context to the dependency injection (DI) container and enables displaying database-related exceptions:
builder.Services.AddDbContext<NetCoreApiDb>(opt => opt.UseInMemoryDatabase("NetCoreApiList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// Configure Swagger middleware
// Enables the API Explorer, which is a service that provides metadata about the HTTP API. The API Explorer is used by Swagger to generate the Swagger document.
builder.Services.AddEndpointsApiExplorer();

// Adds the Swagger OpenAPI document generator to the application services and configures it to provide more information about the API, such as its title and version.
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

var app = builder.Build();


//  enables the Swagger middleware for serving the generated JSON document and the Swagger UI. 
// Swagger is only enabled in a development environment. 
// Enabling Swagger in a production environment could expose potentially sensitive details about the API's structure and implementation.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}



app.MapGet("/NetCoreApiitems", async (NetCoreApiDb db) =>
    await db.NetCoreApis.ToListAsync());

app.MapGet("/NetCoreApiitems/complete", async (NetCoreApiDb db) =>
    await db.NetCoreApis.Where(t => t.IsComplete).ToListAsync());


// ASP.NET Core automatically serializes the object to JSON and writes the JSON into the body of the response message.
//  The response code for this return type is 200 OK, assuming there are no unhandled exceptions. 
// Unhandled exceptions are translated into 5xx errors.

// The return types can represent a wide range of HTTP status codes. 
// For example, GET /todoitems/{id} can return two different status values:
// If no item matches the requested ID, the method returns a 404 status NotFound error code.
// Otherwise, the method returns 200 with a JSON response body. Returning item results in an HTTP 200 response.
app.MapGet("/NetCoreApiitems/{id}", async (int id, NetCoreApiDb db) =>
    await db.NetCoreApis.FindAsync(id)
        is NetCoreApi NetCoreApi
            ? Results.Ok(NetCoreApi)
            : Results.NotFound());

app.MapPost("/NetCoreApiitems", async (NetCoreApi NetCoreApi, NetCoreApiDb db) =>
{
    db.NetCoreApis.Add(NetCoreApi);
    await db.SaveChangesAsync();

    return Results.Created($"/NetCoreApiitems/{NetCoreApi.Id}", NetCoreApi);
});


// This method is similar to the MapPost method, except it uses HTTP PUT. 
// A successful response returns 204 (No Content).
//  According to the HTTP specification, a PUT request requires the client to send the entire updated entity, not just the changes. 
// To support partial updates, use HTTP PATCH.
app.MapPut("/NetCoreApiitems/{id}", async (int id, NetCoreApi inputNetCoreApi, NetCoreApiDb db) =>
{
    var NetCoreApi = await db.NetCoreApis.FindAsync(id);

    if (NetCoreApi is null) return Results.NotFound();

    NetCoreApi.Name = inputNetCoreApi.Name;
    NetCoreApi.IsComplete = inputNetCoreApi.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/NetCoreApiitems/{id}", async (int id, NetCoreApiDb db) =>
{
    if (await db.NetCoreApis.FindAsync(id) is NetCoreApi NetCoreApi)
    {
        db.NetCoreApis.Remove(NetCoreApi);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

// http://localhost:5143/swagger/index.html

app.Run();