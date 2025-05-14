
using API.Storage;
using System.Text.Json.Serialization;
using Tubes_KPL.src.Domain.Models;
using Tubes_KPL.src.Application.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error", detail = ex.Message });
    }
});

app.MapGet("/api/tugas", () =>
{
    var tasks = TugasStorage<List<Tugas>>.GetTugas();
    return Results.Ok(tasks);
});

app.MapGet("/api/tugas/{id:int}", (int id) =>
{
    var tasks = TugasStorage<List<Tugas>>.GetTugas();
    var task = tasks.FirstOrDefault(u => u.Id == id);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

app.MapPost("/api/tugas", (Tugas newTugas) =>
{
    var tasks = TugasStorage<List<Tugas>>.GetTugas();

    if (!InputValidator.IsValidDeadline(newTugas.Deadline))
        return Results.BadRequest("Input not valid");

    newTugas.Id = tasks.Count == 0 ? 1 : tasks.Max(u => u.Id) + 1;
    tasks.Add(newTugas);


    TugasStorage<List<Tugas>>.SaveTugas(tasks);
    return Results.Created($"/users/{newTugas.Id}", newTugas);

});

app.MapPut("/api/tugas/{id:int}", (int id, Tugas updatedTugas) =>
{
    var tasks = TugasStorage<List<Tugas>>.GetTugas();

    if (!InputValidator.IsValidDeadline(updatedTugas.Deadline))
        return Results.BadRequest("Input not valid");


    var task = tasks.FirstOrDefault(u => u.Id == id);
    if (task is null) return Results.NotFound();

    if (!InputValidator.IsValidDeadline(updatedTugas.Deadline)) return Results.BadRequest("Input not valid");

    task.Judul = updatedTugas.Judul;
    task.Status = updatedTugas.Status;
    task.Deadline = updatedTugas.Deadline;
    task.Kategori = updatedTugas.Kategori;
    TugasStorage<List<Tugas>>.SaveTugas(tasks);
    return Results.Ok(task);
});

app.MapDelete("/api/tugas/{id:int}", (int id) =>
{
    var tasks = TugasStorage<List<Tugas>>.GetTugas();
    var task = tasks.FirstOrDefault(u => u.Id == id);
    if (task is null) return Results.NotFound();

    tasks.Remove(task);
    TugasStorage<List<Tugas>>.SaveTugas(tasks);
    return Results.Ok($"Task with ID {id} has been deleted successfully.");
});

// filter untuk by params (deadline, status, kategori)
app.MapGet("/api/tugas/filter", (string? status, string? kategori, DateTime? deadline) =>
{
    var tasks = TugasStorage<List<Tugas>>.GetTugas();
    if (status != null)
    {
        tasks = tasks.Where(t => t.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    if (kategori != null)
    {
        tasks = tasks.Where(t => t.Kategori.ToString().Equals(kategori, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    if (deadline != null)
    {
        tasks = tasks.Where(t => t.Deadline.Date == deadline.Value.Date).ToList();
    }
    return Results.Ok(tasks);
});

// Mendapatkan Tugas Berdasarkan Rentang Waktu
app.MapGet("/api/tugas/date-range", (DateTime startDate, DateTime endDate) =>
{
    var tasks = TugasStorage<List<Tugas>>.GetTugas();
    var filteredTasks = tasks.Where(t => t.Deadline.Date >= startDate.Date && t.Deadline.Date <= endDate.Date).ToList();

    return Results.Ok(filteredTasks);
});

app.Run("http://localhost:4000");