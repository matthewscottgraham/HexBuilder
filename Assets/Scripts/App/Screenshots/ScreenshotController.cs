using System;
using System.Diagnostics;
using System.IO;
using App.Services;
using UnityEngine;

namespace App.Screenshots
{
    public class ScreenshotController : IDisposable
    {
        private const string ScreenshotDirectory = "Screenshots";
        private IOController _ioController = ServiceLocator.Instance.Get<IOController>();

        public void Dispose()
        {
            _ioController = null;
        }

        public void TakeScreenshot()
        {
            var texture = ScreenCapture.CaptureScreenshotAsTexture(1);
            var fileData = texture.EncodeToJPG();
             var imagePath = _ioController.SaveImage(
                 fileData, ScreenshotDirectory, "screenshot", "jpg");
             Application.OpenURL(Path.GetDirectoryName(imagePath));
        }
        
        public void TakeGamePreviewImage(string relativePath, string fileName)
        {
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);
            var tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();

            tex = ResizeTexture(tex, tex.width / 4, tex.height / 4);

            var bytes = tex.EncodeToJPG();
            _ioController.SaveImage(bytes, relativePath, fileName, "jpg");
        }
        
        private static Texture2D ResizeTexture(Texture2D source, int width, int height)
        {
            var rt = RenderTexture.GetTemporary(width, height);
            RenderTexture.active = rt;

            // Copy texture to RT (GPU scaling)
            Graphics.Blit(source, rt);

            // Read RT back into a new Texture2D
            var result = new Texture2D(width, height, source.format, false);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            return result;
        }
    }
}
