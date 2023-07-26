using ImGuiNET;
using Raylib_cs;
using SharpEngine.Math;
using SharpEngineEditor.Manager;
using SharpEngineEditor.Widget;
using SharpEngineEditor.Window;
using Color = SharpEngine.Utils.Color;

namespace SharpEngineEditor;

public class SEEditor
{
    public readonly SceneViewWindow SceneViewWindow;
    
    public SEEditor()
    {
        Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT);
        Raylib.InitWindow(1280, 800, "Sharp Engine Editor");
        Raylib.SetTargetFPS(144);
        
        SEImGui.Setup();

        ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

        SceneViewWindow = new SceneViewWindow();
        SceneViewWindow.Setup(new Vec2I(750, 500));
    }

    public void Run()
    {
        var openNewProject = false;
        var newProjectWindow = new NewProjectWindow();
        
        while (!Raylib.WindowShouldClose())
        {
            SceneViewWindow.Update();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);

            SEImGui.Begin();

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Project"))
                {
                    if (ImGui.MenuItem("New"))
                        openNewProject = true;
                    
                    ImGui.MenuItem("Save", null, false, ProjectManager.CurrentProject != null);
                    ImGui.MenuItem("Export", null, false, ProjectManager.CurrentProject != null);
                    ImGui.Separator();
                    ImGui.MenuItem("Parameters", null, false, ProjectManager.CurrentProject != null);
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }

            SceneViewWindow.Show();

            if (openNewProject)
            {
                ImGui.OpenPopup("New Project");
                openNewProject = false;
            }

            newProjectWindow.Draw();
            
            SEImGui.End();

            Raylib.EndDrawing();
        }
        
        SEImGui.Shutdown();
        
        SceneViewWindow.Dispose();
        
        Raylib.CloseWindow();
    }
}