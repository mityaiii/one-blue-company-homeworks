using FluentValidation;
using ProductService.Api.AutoMapper;
using ProductService.Api.Interceptors;
using ProductService.Api.Services;
using ProductService.Core.DependencyInjection;
using ProductService.DataAccess.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(o =>
    {
        o.Interceptors.Add<ExceptionInterceptor>();
        o.Interceptors.Add<LoggingInterceptor>();
        o.Interceptors.Add<ValidationInterceptor>();
    })
    .AddJsonTranscoding();

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

builder.Services.AddAutoMapper(typeof(ProductTypeMapper), typeof(DateTimeMapper), typeof(ProductReplyToProductMapper));
builder.Services.AddRepositories();
builder.Services.AddProductService();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<ProductServiceGrpc>();

app.Run();