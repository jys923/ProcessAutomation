using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using Serilog;

namespace OpenGLTestCs
{
    public static class Program
    {
        private static void Main()
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(512, 512),
                Title = "LearnOpenTK - Creating a Window",

                //
                Profile = ContextProfile.Compatability,
                
                // This is needed to run on macos
                //Flags = ContextFlags.ForwardCompatible,
            };

            // To create a new window, create a class that extends GameWindow, then call Run() on it.
            using (var window = new Simple3DWindow(GameWindowSettings.Default, nativeWindowSettings))
            {
                //Init();
                window.Run();
            }

            // And that's it! That's all it takes to create a window with OpenTK.
        }
    }
}