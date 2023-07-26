using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using Raylib_cs;

namespace SharpEngineEditor;

public static class SEImGui
{
    internal static IntPtr ImGuiContext = IntPtr.Zero;

    private static ImGuiMouseCursor _currentMouseCursor = ImGuiMouseCursor.COUNT;

    private static Dictionary<ImGuiMouseCursor, MouseCursor> _mouseCursorMap = new();

    private static KeyboardKey[]? _keyEnumMap;

    private static Texture2D _fontTexture;

    public static void Setup(bool darkTheme = true)
    {
        _mouseCursorMap = new Dictionary<ImGuiMouseCursor, MouseCursor>();
        _keyEnumMap = Enum.GetValues(typeof(KeyboardKey)) as KeyboardKey[];

        _fontTexture.id = 0;

        BeginInitImGui();

        if (darkTheme)
            ImGui.StyleColorsDark();
        else
            ImGui.StyleColorsLight();

        EndInitImGui();
    }

    public static void BeginInitImGui()
    {
        ImGuiContext = ImGui.CreateContext();
    }

    private static void SetupMouseCursors()
    {
        _mouseCursorMap.Clear();
        _mouseCursorMap[ImGuiMouseCursor.Arrow] = MouseCursor.MOUSE_CURSOR_ARROW;
        _mouseCursorMap[ImGuiMouseCursor.TextInput] = MouseCursor.MOUSE_CURSOR_IBEAM;
        _mouseCursorMap[ImGuiMouseCursor.Hand] = MouseCursor.MOUSE_CURSOR_POINTING_HAND;
        _mouseCursorMap[ImGuiMouseCursor.ResizeAll] = MouseCursor.MOUSE_CURSOR_RESIZE_ALL;
        _mouseCursorMap[ImGuiMouseCursor.ResizeEW] = MouseCursor.MOUSE_CURSOR_RESIZE_EW;
        _mouseCursorMap[ImGuiMouseCursor.ResizeNESW] = MouseCursor.MOUSE_CURSOR_RESIZE_NESW;
        _mouseCursorMap[ImGuiMouseCursor.ResizeNS] = MouseCursor.MOUSE_CURSOR_RESIZE_NS;
        _mouseCursorMap[ImGuiMouseCursor.ResizeNWSE] = MouseCursor.MOUSE_CURSOR_RESIZE_NWSE;
        _mouseCursorMap[ImGuiMouseCursor.NotAllowed] = MouseCursor.MOUSE_CURSOR_NOT_ALLOWED;
    }

    public static unsafe void ReloadFonts()
    {
        ImGui.SetCurrentContext(ImGuiContext);
        var io = ImGui.GetIO();

        int width, height;
        io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out width, out height, out _);

        var image = new Image
        {
            data = pixels,
            width = width,
            height = height,
            mipmaps = 1,
            format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
        };

        _fontTexture = Raylib.LoadTextureFromImage(image);

        io.Fonts.SetTexID(new IntPtr(_fontTexture.id));
    }

    public static void EndInitImGui()
    {
        SetupMouseCursors();

        ImGui.SetCurrentContext(ImGuiContext);
        ImGui.GetIO().Fonts.AddFontDefault();

        var io = ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyboardKey.KEY_TAB;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyboardKey.KEY_LEFT;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyboardKey.KEY_RIGHT;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyboardKey.KEY_UP;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyboardKey.KEY_DOWN;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyboardKey.KEY_PAGE_UP;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyboardKey.KEY_PAGE_DOWN;
        io.KeyMap[(int)ImGuiKey.Home] = (int)KeyboardKey.KEY_HOME;
        io.KeyMap[(int)ImGuiKey.End] = (int)KeyboardKey.KEY_END;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyboardKey.KEY_DELETE;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyboardKey.KEY_BACKSPACE;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyboardKey.KEY_ENTER;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyboardKey.KEY_ESCAPE;
        io.KeyMap[(int)ImGuiKey.Space] = (int)KeyboardKey.KEY_SPACE;
        io.KeyMap[(int)ImGuiKey.A] = (int)KeyboardKey.KEY_A;
        io.KeyMap[(int)ImGuiKey.C] = (int)KeyboardKey.KEY_C;
        io.KeyMap[(int)ImGuiKey.V] = (int)KeyboardKey.KEY_V;
        io.KeyMap[(int)ImGuiKey.X] = (int)KeyboardKey.KEY_X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)KeyboardKey.KEY_Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)KeyboardKey.KEY_Z;

        ReloadFonts();
    }

    private static void NewFrame()
    {
        var io = ImGui.GetIO();

        if (Raylib.IsWindowFullscreen())
        {
            var monitor = Raylib.GetCurrentMonitor();
            io.DisplaySize = new Vector2(Raylib.GetMonitorWidth(monitor), Raylib.GetMonitorHeight(monitor));
        }
        else
            io.DisplaySize = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        io.DisplayFramebufferScale = new Vector2(1, 1);
        io.DeltaTime = Raylib.GetFrameTime();

        io.KeyCtrl = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL);
        io.KeyShift = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);
        io.KeyAlt = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT);
        io.KeySuper = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SUPER) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SUPER);

        if (io.WantSetMousePos)
            Raylib.SetMousePosition((int)io.MousePos.X, (int)io.MousePos.Y);
        else
            io.MousePos = Raylib.GetMousePosition();

        io.MouseDown[0] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON);
        io.MouseDown[1] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON);
        io.MouseDown[2] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_MIDDLE_BUTTON);

        if (Raylib.GetMouseWheelMove() > 0)
            io.MouseWheel += 1;
        else if (Raylib.GetMouseWheelMove() < 0)
            io.MouseWheel -= 1;

        if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) == 0)
        {
            var imguiCursor = ImGui.GetMouseCursor();
            if (imguiCursor != _currentMouseCursor || io.MouseDrawCursor)
            {
                _currentMouseCursor = imguiCursor;
                if (io.MouseDrawCursor || imguiCursor == ImGuiMouseCursor.None)
                {
                    Raylib.HideCursor();
                }
                else
                {
                    Raylib.ShowCursor();

                    if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) == 0)
                    {
                        Raylib.SetMouseCursor(!_mouseCursorMap.ContainsKey(imguiCursor)
                            ? MouseCursor.MOUSE_CURSOR_DEFAULT
                            : _mouseCursorMap[imguiCursor]);
                    }
                }
            }
        }
    }


    private static void FrameEvents()
    {
        var io = ImGui.GetIO();

        foreach (var key in _keyEnumMap!)
        {
            io.KeysDown[(int)key] = Raylib.IsKeyDown(key);
        }

        var pressed = (uint)Raylib.GetCharPressed();
        while (pressed != 0)
        {
            io.AddInputCharacter(pressed);
            pressed = (uint)Raylib.GetCharPressed();
        }
    }

    public static void Begin()
    {
        ImGui.SetCurrentContext(ImGuiContext);

        NewFrame();
        FrameEvents();
        ImGui.NewFrame();
    }

    private static void EnableScissor(float x, float y, float width, float height)
    {
        Rlgl.rlEnableScissorTest();
        Rlgl.rlScissor((int)x, Raylib.GetScreenHeight() - (int)(y + height), (int)width, (int)height);
    }

    private static void TriangleVert(ImDrawVertPtr idxVert)
    {
        var color = ImGui.ColorConvertU32ToFloat4(idxVert.col);

        Rlgl.rlColor4f(color.X, color.Y, color.Z, color.W);
        Rlgl.rlTexCoord2f(idxVert.uv.X, idxVert.uv.Y);
        Rlgl.rlVertex2f(idxVert.pos.X, idxVert.pos.Y);
    }

    private static void RenderTriangles(uint count, uint indexStart, ImVector<ushort> indexBuffer,
        ImPtrVector<ImDrawVertPtr> vertBuffer, IntPtr texturePtr)
    {
        if (count < 3)
            return;

        uint textureId = 0;
        if (texturePtr != IntPtr.Zero)
            textureId = (uint)texturePtr.ToInt32();

        Rlgl.rlBegin(DrawMode.TRIANGLES);
        Rlgl.rlSetTexture(textureId);

        for (var i = 0; i <= (count - 3); i += 3)
        {
            if (Rlgl.rlCheckRenderBatchLimit(3))
            {
                Rlgl.rlBegin(DrawMode.TRIANGLES);
                Rlgl.rlSetTexture(textureId);
            }

            var indexA = indexBuffer[(int)indexStart + i];
            var indexB = indexBuffer[(int)indexStart + i + 1];
            var indexC = indexBuffer[(int)indexStart + i + 2];

            ImDrawVertPtr vertexA = vertBuffer[indexA];
            ImDrawVertPtr vertexB = vertBuffer[indexB];
            ImDrawVertPtr vertexC = vertBuffer[indexC];

            TriangleVert(vertexA);
            TriangleVert(vertexB);
            TriangleVert(vertexC);
        }

        Rlgl.rlEnd();
    }

    private delegate void Callback(ImDrawListPtr list, ImDrawCmdPtr cmd);

    private static void RenderData()
    {
        Rlgl.rlDrawRenderBatchActive();
        Rlgl.rlDisableBackfaceCulling();

        var data = ImGui.GetDrawData();

        for (var l = 0; l < data.CmdListsCount; l++)
        {
            var commandList = data.CmdListsRange[l];

            for (var cmdIndex = 0; cmdIndex < commandList.CmdBuffer.Size; cmdIndex++)
            {
                var cmd = commandList.CmdBuffer[cmdIndex];

                EnableScissor(cmd.ClipRect.X - data.DisplayPos.X, cmd.ClipRect.Y - data.DisplayPos.Y,
                    cmd.ClipRect.Z - (cmd.ClipRect.X - data.DisplayPos.X),
                    cmd.ClipRect.W - (cmd.ClipRect.Y - data.DisplayPos.Y));
                if (cmd.UserCallback != IntPtr.Zero)
                {
                    var cb = Marshal.GetDelegateForFunctionPointer<Callback>(cmd.UserCallback);
                    cb(commandList, cmd);
                    continue;
                }

                RenderTriangles(cmd.ElemCount, cmd.IdxOffset, commandList.IdxBuffer, commandList.VtxBuffer,
                    cmd.TextureId);

                Rlgl.rlDrawRenderBatchActive();
            }
        }

        Rlgl.rlSetTexture(0);
        Rlgl.rlDisableScissorTest();
        Rlgl.rlEnableBackfaceCulling();
    }

    public static void End()
    {
        ImGui.SetCurrentContext(ImGuiContext);
        ImGui.Render();
        RenderData();
    }

    public static void Shutdown()
    {
        Raylib.UnloadTexture(_fontTexture);
        ImGui.DestroyContext();
    }

    public static void Image(Texture2D image)
    {
        ImGui.Image(new IntPtr(image.id), new Vector2(image.width, image.height), new Vector2(0, 1), new Vector2(1, 0));
    }

    public static void ImageSize(Texture2D image, int width, int height)
    {
        ImGui.Image(new IntPtr(image.id), new Vector2(width, height), new Vector2(0, 1), new Vector2(1, 0));
    }

    public static void ImageSize(Texture2D image, Vector2 size)
    {
        ImGui.Image(new IntPtr(image.id), size, new Vector2(0, 1), new Vector2(1, 0));
    }

    public static void ImageRect(Texture2D image, int destWidth, int destHeight, Rectangle sourceRect)
    {
        var uv0 = new Vector2();
        var uv1 = new Vector2();

        if (sourceRect.width < 0)
        {
            uv0.X = -(sourceRect.x / image.width);
            uv1.X = uv0.X - Math.Abs(sourceRect.width) / image.width;
        }
        else
        {
            uv0.X = sourceRect.x / image.width;
            uv1.X = uv0.X + sourceRect.width / image.width;
        }

        if (sourceRect.height < 0)
        {
            uv0.Y = -(sourceRect.y / image.height);
            uv1.Y = uv0.Y - Math.Abs(sourceRect.height) / image.height;
        }
        else
        {
            uv0.Y = sourceRect.y / image.height;
            uv1.Y = uv0.Y + sourceRect.height / image.height;
        }

        (uv0.Y, uv1.Y) = (uv1.Y, uv0.Y);

        ImGui.Image(new IntPtr(image.id), new Vector2(destWidth, destHeight), uv0, uv1);
    }
}