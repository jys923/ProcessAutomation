using Silk.NET.OpenGL;
using Silk.NET.Windowing;

public class HsnBufferCreator : IDisposable
{
    private GL gl;
    private IWindow window;
    private uint textureId;
    private int width;
    private int height;
    private bool initiated = false;
    private byte[] buffer;
    private byte[] envdataBuffer;
    private int bufferSize;
    private int envdataBufferSize;

    public HsnBufferCreator(int width, int height)
    {
        this.width = width;
        this.height = height;
        ResizeWindow(width, height);
    }

    public void Dispose()
    {
        Destroy();
    }

    public bool Initiate()
    {
        if (initiated)
            return true; // 이미 초기화됨

        var options = WindowOptions.Default;
        options.PreferredDepthBufferBits = 24;
        options.PreferredSamples = 4;
        options.PreferredColorFormat = new ColorFormat(32, 32, 32, 32);
        window = Silk.NET.Windowing.Window.Create(options);

        window.Load += OnLoad;
        window.Render += OnRender;
        window.Run();

        initiated = true;
        return true;
    }

    private void OnLoad()
    {
        gl = GL.GetApi(window.GLContext);
        gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        gl.Enable(EnableCap.Texture2D);

        textureId = gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, textureId);

        gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
    }

    private void OnRender(double obj)
    {
        if (!initiated)
            return;

        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        gl.BindTexture(TextureTarget.Texture2D, textureId);

        // 렌더링 로직 구현
        // OpenGL 명령어를 사용하여 텍스처를 그리거나 버퍼를 수정합니다.

        gl.ReadPixels(0, 0, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, buffer);

        // 버퍼를 비트맵 파일로 저장
        //SaveBmp("output.bmp", buffer, width, height);
    }

    public bool RenderBitmap()
    {
        if (!initiated)
            return false;

        window.RenderFrame();
        return true;
    }

    private void ResizeWindow(int width, int height)
    {
        if (this.width != width || this.height != height)
        {
            this.width = width;
            this.height = height;
            bufferSize = width * height * 4;

            buffer = new byte[bufferSize];
        }
    }

    //private bool SaveBmp(string filename, byte[] image, int width, int height)
    //{
    //    using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
    //    {
    //        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
    //        IntPtr ptr = bmpData.Scan0;
    //        Marshal.Copy(image, 0, ptr, image.Length);
    //        bitmap.UnlockBits(bmpData);
    //        bitmap.Save(filename, ImageFormat.Bmp);
    //    }

    //    return true;
    //}

    public void Destroy()
    {
        if (!initiated)
            return;

        gl.DeleteTexture(textureId);
        window.Dispose();

        buffer = null;
        envdataBuffer = null;
        bufferSize = 0;
        envdataBufferSize = 0;
        initiated = false;
    }
}
