using NordicRCC.Application.Api.TimeSeries;
using NordicRCC.Domain;
using NordicRCC.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDomain();
builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();

	// An interactive frontend for the OpenAPI document, at /scalar.
	app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapTimeSeriesDataEndpoints();

app.Run();

public partial class Program;
