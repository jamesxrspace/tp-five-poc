using TPFive.Game.Utils;
using UnityEngine;

namespace TPFive.Game.Extensions
{
    public class ScreenCaptureExtensions
    {
        public static Texture2D CaptureScreenshotAsTexture(int w, int h, bool isAlphaBackground)
        {
            return CaptureScreenshotAsTexture(w, h, isAlphaBackground, CameraCache.Main);
        }

        public static Texture2D CaptureScreenshotAsTexture(int w, int h, bool isAlphaBackground, Camera camera)
        {
            RenderTexture rt = RenderTexture.GetTemporary(w, h, 32);
            camera.targetTexture = rt;
            Texture2D screenshot = new Texture2D(w, h, TextureFormat.ARGB32, false);
            CameraClearFlags clearFlags = camera.clearFlags;
            if (isAlphaBackground)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.clear;
            }

            camera.Render();
            var temp = RenderTexture.active;
            RenderTexture.active = rt;
            screenshot.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            screenshot.Apply();
            camera.targetTexture = null;
            RenderTexture.active = temp;
            RenderTexture.ReleaseTemporary(rt);
            camera.clearFlags = clearFlags;
            return screenshot;
        }

        public static Texture2D CaptureScreenshotRegionAsTexture(Rect regionRect, int w, int h, bool isAlphaBackground, Camera camera)
        {
            RenderTexture rt = RenderTexture.GetTemporary(w, h, 32);
            camera.targetTexture = rt;
            Texture2D screenshot = new Texture2D((int)regionRect.width, (int)regionRect.height, TextureFormat.ARGB32, false);
            CameraClearFlags clearFlags = camera.clearFlags;
            if (isAlphaBackground)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.clear;
            }

            camera.Render();
            var temp = RenderTexture.active;
            RenderTexture.active = rt;
            screenshot.ReadPixels(new Rect(regionRect.x, regionRect.y, regionRect.width, regionRect.height), 0, 0);
            screenshot.Apply();
            camera.targetTexture = null;
            RenderTexture.active = temp;
            RenderTexture.ReleaseTemporary(rt);
            camera.clearFlags = clearFlags;
            return screenshot;
        }
    }
}
