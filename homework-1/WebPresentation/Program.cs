using Core.Services;
using Core.Services.AdsServices;
using Core.Services.DemandServices;
using Core.Services.PredictionServices;
using DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Services;

string? salesHistoryFilePath = Environment.GetEnvironmentVariable("SALES_HISTIRY_FILE_PATH");
string? salesSeasonsFilePath = Environment.GetEnvironmentVariable("SALES_SEASONS_FILE_PATH");

connection.AddAdsServiceExtensions()
    .AddPredictionServiceExtensions()
    .AddDemandServiceExtensions()
    .AddDataAccessExtensions(salesHistoryFilePath,salesSeasonsFilePath);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();