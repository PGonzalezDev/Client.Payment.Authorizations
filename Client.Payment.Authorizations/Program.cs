using Client.Payment.Authorizations.Requests.Authorization;
using Client.Payment.Authorizations.Responses;
using Client.Payments.Authorizations.Context;
using Client.Payments.Authorizations.Services.Repositories;
using Client.Payments.Authorizations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTO = Client.Payments.Authorizations.Services.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ClientPaymentAuthorizationContext>(opt =>
    opt.UseInMemoryDatabase("ClientPaymentAuthorizations")
);

// Repositories
builder.Services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IApprovedAuthorizationRepository, ApprovedAuthorizationRepository>();

// Services
builder.Services.AddScoped<IPaymentsAuthorizerService, PaymentsAuthorizerService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddScoped<IReverseAuthorizationsService, ReverseAuthorizationsService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/healtly", () => Results.Ok("Health"))
    .WithOpenApi();

app.MapGet("/get-all-clients", async (
    [FromHeader(Name = "x-api-key")] Guid apiKey,
    [FromHeader(Name = "x-api-token")] string apiToken,
    ClientPaymentAuthorizationContext context,
    IConfiguration configuration
) =>
{
    Guid validKey = new Guid(configuration["x-api-key"]);
    string validToken = configuration["x-api-token"];
    
    if (apiKey.CompareTo(validKey) != 0 || apiToken.CompareTo(validToken) != 0)
    {
        return Results.Unauthorized();
    }
    
    var clients = await context.Clients
        .ToArrayAsync();

    return Results.Ok(clients);
})
.WithName("Clients");

app.MapGet("/get-reverse-authorizations", async (
    [FromHeader(Name = "x-api-key")] Guid apiKey,
    [FromHeader(Name = "x-api-token")] string apiToken,
    IConfiguration configuration,
    ClientPaymentAuthorizationContext context
) =>
{
    Guid validKey = new Guid(configuration["x-api-key"]);
    string validToken = configuration["x-api-token"];

    if (apiKey.CompareTo(validKey) != 0 || apiToken.CompareTo(validToken) != 0)
    {
        return Results.Unauthorized();
    }

    var reverseAuth = context.Authorizations
            .Where(x => x.ReversedAuthorizationId.HasValue);

    return Results.Ok(reverseAuth);
})
.WithName("GetReverseAuth");

app.MapPost("/reverser-authorizations", async (
    [FromHeader(Name = "x-api-key")] Guid apiKey,
    [FromHeader(Name = "x-api-token")] string apiToken,
    IConfiguration configuration,
    IReverseAuthorizationsService reverseAuthorizationsService
) =>
{
    Guid validKey = new Guid(configuration["x-api-key"]);
    string validToken = configuration["x-api-token"];

    if (apiKey.CompareTo(validKey) != 0 || apiToken.CompareTo(validToken) != 0)
    {
        return Results.Unauthorized();
    }

    await reverseAuthorizationsService.ReverseAuthorizationsProcessAsync();

    return Results.Ok();
})
.WithName("ReverserAuth");

app.MapPost("/create-authorization", async (
    [FromBody] CreateAuthorizationRequest request,
    IAuthorizationService service
) =>
{
    var response = await service.AddAuthorizationAsync(request.ToEntity());

    return Results.Ok(response);
})
.WithName("CreateAuthorization")
.WithOpenApi();

app.MapPost("/confirm-authorization", async (
    [FromBody] ConfirmAuthorizationRequest request,
    IAuthorizationService service
) =>
{
    var resultDto = await service.ConfirmAuthorizationAsync(request.ToDto());

    if(resultDto.ResultCode != DTO.ResultCodeEnum.Ok)
    {
        return resultDto.ResultCode switch
        {
            DTO.ResultCodeEnum.BadRequest => Results.BadRequest(resultDto.ErrorMsg),
            DTO.ResultCodeEnum.NotFound => Results.NotFound(resultDto.ErrorMsg),
            DTO.ResultCodeEnum.UnprocessableEntity => Results.UnprocessableEntity(resultDto.ErrorMsg),
            _ => Results.StatusCode(500)
        };
    }

    return Results.Ok(new ConfirmAuthorizationResponse(resultDto));
})
.WithName("ConfirmAuthorization")
.WithOpenApi();

app.Run();
