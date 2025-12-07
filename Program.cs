using NotesApi;
using BCrypt.Net;
using Microsoft.AspNetCore.Server.HttpSys;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http.HttpResults;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


List<UserModel> listUsers = new List<UserModel> { };
List<NoteModel> listNote = new List<NoteModel> { };

app.MapGet("/users", () => listUsers);

app.MapPost("/auth/register", (RequestRegister rr) =>
{

    if (rr.UserName.Length < 3 || rr.Password.Length < 6)
        return Results.BadRequest();

    for (int i = 0; i < listUsers.Count; i++)
    {
        if (rr.UserName == listUsers[i].UserName)
            return Results.Conflict();
    }

    listUsers.Add(new UserModel(rr.UserName, BCrypt.Net.BCrypt.HashPassword(rr.Password)));
    return Results.Ok(new { username = rr.UserName });

});

app.MapPost("/auth/login", (RequestLogin rl) =>
{
    for (int i = 0; i < listUsers.Count; i++)
    {
        if (rl.UserName == listUsers[i].UserName)
            if (BCrypt.Net.BCrypt.Verify(rl.Password, listUsers[i].HashPassword))
                return Results.Ok(new { token = ServiceApi.GenerJwt(listUsers[i].UserName, listUsers[i].Id) });
            else
                break;
    }

    return Results.Unauthorized();
});

app.MapPost("/notes", (NoteModel noteModel, HttpContext hc) =>
{
    if (!hc.Request.Headers.TryGetValue("Authorization", out var authHeader))
        return Results.Unauthorized();

    string header = authHeader.ToString();

    if (!header.StartsWith("Bearer "))
        return Results.Unauthorized();

    int id = ServiceApi.GetIdJwt(hc);

    noteModel.UserId = id;

    listNote.Add(noteModel);
    return Results.Ok(listNote[^1]);

});

app.MapGet("/notes", (HttpContext hc) =>
{
    if (!hc.Request.Headers.TryGetValue("Authorization", out var authHeader))
        return Results.Unauthorized();

    string header = authHeader.ToString();

    if (!header.StartsWith("Bearer "))
        return Results.Unauthorized();


    int id = ServiceApi.GetIdJwt(hc);

    var userNotes = listNote.Where(n => n.UserId == id).ToList();
    return Results.Ok(userNotes);

});

app.MapPut("/notes/{id:int}", (PutNotes putnotes, int id, HttpContext hc) =>
{
    if (!hc.Request.Headers.TryGetValue("Authorization", out var authHeader))
        return Results.Unauthorized();

    string header = authHeader.ToString();

    if (!header.StartsWith("Bearer "))
        return Results.Unauthorized();


    int userid = ServiceApi.GetIdJwt(hc);

    for (int i = 0; i < listNote.Count; i++)
    {
        if (listNote[i].UserId == userid && listNote[i].Id == id)
        {
            listNote[i].Title = putnotes.Title;
            listNote[i].Description = putnotes.Description;
            return Results.Ok(listNote[i]);
        }
    }

    return Results.Forbid();

});

app.MapDelete("/notes/{id:int}", (int id, HttpContext hc) =>
{
    if (!hc.Request.Headers.TryGetValue("Authorization", out var authHeader))
        return Results.Unauthorized();

    string header = authHeader.ToString();

    if (!header.StartsWith("Bearer "))
        return Results.Unauthorized();

    int userid = ServiceApi.GetIdJwt(hc);

    for (int i = 0; i < listNote.Count; i++)
    {
        if (listNote[i].UserId == userid && listNote[i].Id == id)
        {
            listNote.RemoveAt(i);
            return Results.Ok();
        }
    }

    return Results.Forbid();
});

app.Run();
