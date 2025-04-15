using API.Bancaria.Command;
using API.Bancaria.Handlers;
using API.Bancaria.Infrastrutuce;
using API.Bancaria.Query;
using API.Bancaria.Repository;
using API.Bancaria.Response;
using Application.Bancaria.Handlers;
using Domain.Bancaria.Repositories;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using SQLitePCL;

Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Bancaria",
        Version = "v1",
        Description = "API para operações bancárias"
    });
});

string connectionString = builder.Configuration.GetConnectionString("SqliteConnection") ?? "Data Source=banco.db";
Console.WriteLine($"String de conexão: {connectionString}");

Infrastructure.DataBase.Database.Initialize(connectionString);

builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>(provider =>
    new ContaCorrenteRepository(connectionString));

builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>(provider =>
    new MovimentoRepository(connectionString));

builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>(provider =>
    new IdempotenciaRepository(connectionString));

builder.Services.AddTransient<IRequestHandler<ConsultarSaldoQuery, SaldoContaCorrenteResponse>, ConsultarSaldoHandler>();

builder.Services.AddTransient<IRequestHandler<RealizarMovimentacaoCommand, MovimentacaoResponse>, RealizarMovimentacaoHandler>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

// Usar CORS antes de outros middlewares
app.UseCors("AllowAll");

// Configure o middleware na ordem correta
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Bancaria v1");
    });
}

//// Adicione um redirecionamento para o Swagger na rota raiz
//app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            Console.WriteLine($"Erro: {contextFeature.Error}");

            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Ocorreu um erro interno no servidor.",
                Detail = contextFeature.Error.Message
            });
        }
    });
});

app.Run();