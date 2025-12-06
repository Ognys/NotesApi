using NotesApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/auth/register", (RequestRegister rr) => {
    
});

app.Run();
