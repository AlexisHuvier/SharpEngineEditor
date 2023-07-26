using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using SharpEngine;
using SharpEngine.Math;
using Color = SharpEngine.Utils.Color;

namespace SharpEngineEditor.Window;

public class SceneViewWindow: IDisposable
{
    public RenderTexture2D ViewTexture;
    public bool Focused = false;
    public Scene? Scene = null;

    public void Setup(Vec2I size)
    {
        ViewTexture = Raylib.LoadRenderTexture(size.X, size.Y);
    }
    
    public void Dispose()
    {
        Raylib.UnloadRenderTexture(ViewTexture);
        GC.SuppressFinalize(this);
    }

    public void Resize(Vec2I size)
    {
        Raylib.UnloadRenderTexture(ViewTexture);
        ViewTexture = Raylib.LoadRenderTexture(size.X, size.Y);
    }

    public void Show()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vec2.Zero);
        ImGui.SetNextWindowSizeConstraints(new Vector2(900, 600), new Vector2(900, 600));

        if (ImGui.Begin("View", ImGuiWindowFlags.NoScrollbar))
        {
            Focused = ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows);
            
            var size = ImGui.GetContentRegionAvail();

            SEImGui.ImageSize(ViewTexture.texture, size);
            
            ImGui.End();
        }
        ImGui.PopStyleVar();
    }

    public void Update()
    {
        Raylib.BeginTextureMode(ViewTexture);
        Raylib.ClearBackground(Color.Aqua);
        
        Scene?.DrawEntities();
        Scene?.DrawWidgets();
        
        Raylib.EndTextureMode();
    }
}