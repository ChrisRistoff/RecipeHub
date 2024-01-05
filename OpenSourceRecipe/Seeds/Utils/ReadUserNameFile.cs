using Newtonsoft.Json;

namespace OpenSourceRecipes.Utils;
public class MyUserObject
{
    public string Username { get; set; } = "";
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
    public string ProfileImg { get; set; } = "";
    public bool Status { get; set; } = false;
    public string Bio { get; set; } = "";
}

public class ReadUserFunc
{
    public List<MyUserObject> ReadUserFile()
    {
        Console.WriteLine("------------------------");
        Console.WriteLine("Reading Users");
        string currentDirectory = Environment.CurrentDirectory;
        string filePath = @"Seeds/data/users.json";
        filePath = Path.Combine(currentDirectory, filePath);
        try
        {
            string jsonContent = File.ReadAllText(filePath);
            MyUserObject[]? usersArray = JsonConvert.DeserializeObject<MyUserObject[]>(jsonContent);
            Console.WriteLine("Successfully read Users");
            if (usersArray != null) return usersArray.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        return new List<MyUserObject>();
    }
}
