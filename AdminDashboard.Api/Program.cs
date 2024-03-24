using AdminDashboard.Api.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var sampleTodos = new Todo[] {
    new Todo { Id = 1, Title = "Walk the dog" },
    new Todo { Id = 2, Title = "Do the dishes", DueBy = DateOnly.FromDateTime(DateTime.Now) },
    new Todo { Id = 3, Title = "Do the laundry", DueBy = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) },
    new Todo { Id = 4, Title = "Clean the bathroom" },
    new Todo { Id = 5, Title = "Clean the car", DueBy = DateOnly.FromDateTime(DateTime.Now.AddDays(2)) },
    new Todo { Id = 6, Title = "Buy groceries" },
    new Todo { Id = 7, Title = "Pay bills" },
    new Todo { Id = 8, Title = "Go for a run" },
    new Todo { Id = 9, Title = "Read a book" },
    new Todo { Id = 10, Title = "Call a friend" }
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

todosApi.MapPost("/", (Todo todo) =>
{
    var newTodo = new Todo { Id = sampleTodos.Max(a => a.Id) + 1, Title = todo.Title, DueBy = todo.DueBy, IsComplete = todo.IsComplete };
    sampleTodos = sampleTodos.Append(newTodo).ToArray();
    return Results.Created($"/todos/{newTodo.Id}", newTodo);
});

todosApi.MapDelete("/{id}", (int id) =>
{
    var todo = sampleTodos.FirstOrDefault(a => a.Id == id);
    if (todo is null)
    {
        return Results.NotFound();
    }

    sampleTodos = sampleTodos.Where(a => a.Id != id).ToArray();
    return Results.NoContent();
});

var sampleEmployees = new Employee[] {
    new Employee { Id = 1, FirstName = "John", LastName = "Doe", Position = "Manager" },
    new Employee { Id = 2, FirstName = "Jane", LastName = "Doe", Position = "Developer" },
    new Employee { Id = 3, FirstName = "Bob", LastName = "Smith", Position = "Developer" },
    new Employee { Id = 4, FirstName = "Alice", LastName = "Johnson", Position = "Developer" },
    new Employee { Id = 5, FirstName = "Michael", LastName = "Brown", Position = "Tester" },
    new Employee { Id = 6, FirstName = "Emily", LastName = "Wilson", Position = "Tester" },
    new Employee { Id = 7, FirstName = "David", LastName = "Lee", Position = "Tester" },
    new Employee { Id = 8, FirstName = "Sarah", LastName = "Taylor", Position = "Manager" },
    new Employee { Id = 9, FirstName = "James", LastName = "Anderson", Position = "Manager" },
    new Employee { Id = 10, FirstName = "Olivia", LastName = "Clark", Position = "Manager" }
};

var employeesApi = app.MapGroup("/employees");

app.Run();

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
