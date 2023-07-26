using ImGuiNET;

namespace SharpEngineEditor.Window;

public class NewProjectWindow
{
    public string Name = "";
    public string Author = "";

    public void Draw()
    {
        if(ImGui.BeginPopupModal("New Project"))
        {
            ImGui.InputText("Project Name", ref Name, 256);
            ImGui.InputText("Project Author", ref Author, 256);

            if(ImGui.Button("Cancel"))
                ImGui.CloseCurrentPopup();
            ImGui.SameLine();
            if(ImGui.Button("Create"))
                ImGui.CloseCurrentPopup();
            ImGui.EndPopup();
        }
    }
}