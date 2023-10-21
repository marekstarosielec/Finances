namespace Finances.DataAccess;

public class DataFile
{
    public string FileName { get; set; }

    public string Location { get; set; }

    public string FileNameWithLocation { get => Path.Combine(Location, FileName); }

    public DataFile(string fileName, string location)
    {
        FileName = fileName;
        Location = location;
    }
}