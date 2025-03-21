using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

namespace Imagine.WebAR
{
    public class TextureDownloader : MonoBehaviour
    {
        [DllImport("__Internal")] private static extern void DownloadWebGLTexture(byte[] img, int size, string filename, string extension);

        private enum FileExtension { PNG, JPEG};
        [SerializeField] private FileExtension fileExt = FileExtension.PNG;

        public void DownloadTexture(Texture2D texture)
        {

#if UNITY_WEBGL && !UNITY_EDITOR
            if(fileExt == FileExtension.PNG)
            {
                var bytes = texture.EncodeToPNG();
                DownloadWebGLTexture(bytes, bytes.Length, texture.name, ".png");
            }
            else if (fileExt == FileExtension.JPEG)
            {
                var bytes = texture.EncodeToJPG();
                DownloadWebGLTexture(bytes, bytes.Length, texture.name, ".jpeg");
            }
#else
            Debug.Log("Texture downloads only available in WebGL builds");
#endif

        }
    }
}
