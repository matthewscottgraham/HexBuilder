using System;
using System.Collections;
using System.IO;
using App.Services;
using UnityEngine;

namespace App.Screenshots
{
    public class ScreenshotController : MonoBehaviour, IDisposable
    {
        private const string ScreenshotDirectory = "Screenshots";
        private IOController _ioController = ServiceLocator.Instance.Get<IOController>();

        public void Dispose()
        {
            _ioController = null;
        }

        public void TakeScreenshot()
        {
            StartCoroutine(TakeScreenshotCoroutine(
                ScreenshotDirectory, 
                "screenshot", 
                false, 
                true
                ));
        }
        
        public void TakeGamePreviewImage(string relativePath, string fileName)
        {
            StartCoroutine(TakeScreenshotCoroutine(
                relativePath, 
                fileName, 
                true, 
                false));
        }

        private IEnumerator TakeScreenshotCoroutine(string relativePath, string fileName, bool resize = false, bool openDirectory = false)
        {
            yield return new WaitForEndOfFrame();
            
            var texture = ScreenCapture.CaptureScreenshotAsTexture(1);
            if (resize)
            {
                texture = ResizeTexture(texture, texture.width / 4, texture.height / 4);
            }

            var fileData = texture.EncodeToJPG();
            var imagePath = _ioController.SaveImage(fileData, relativePath, fileName, "jpg");

            if (openDirectory) OpenDirectory(imagePath);
        }

        private static void OpenDirectory(string imagePath)
        {
            var imageDirectory = new FileInfo(imagePath).Directory;
            if (imageDirectory == null || !imageDirectory.Exists) return;
            var directoryUrl = new Uri(imageDirectory.FullName).AbsoluteUri;
            Application.OpenURL(directoryUrl);
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
