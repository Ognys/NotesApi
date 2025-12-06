namespace NotesApi;

public class NoteModel
{
    static private int staticId = 1;
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreateDate { get; set; }

    public NoteModel()
    {
        Id = staticId++;
        CreateDate = DateTime.Now;
    }
}