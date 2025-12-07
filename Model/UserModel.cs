namespace NotesApi;

public class UserModel
{
    static private int staticId = 1;
    public int Id { get; set; }
    public string UserName { get; set; }
    public string HashPassword { get; set; }

    public UserModel(string username, string hashpassword)
    {
        UserName = username;
        HashPassword = hashpassword;
        Id = staticId++;
    }
}