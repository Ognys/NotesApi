namespace NotesApi;

public record RequestRegister(string UserName, string Password);
public record RequestLogin(string UserName, string Password);
