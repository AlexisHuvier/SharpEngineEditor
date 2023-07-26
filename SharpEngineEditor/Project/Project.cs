using Newtonsoft.Json;

namespace SharpEngineEditor.Project;

public class Project
{
    public string Folder { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Version { get; set; }

    public void Save()
    {
        File.WriteAllText(Path.Join(Folder, "project.json"), JsonConvert.SerializeObject(this));
    }
}