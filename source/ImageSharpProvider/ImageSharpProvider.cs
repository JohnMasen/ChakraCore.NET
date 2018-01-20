using System;
using ChakraCore.NET;
using ChakraCore.NET.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace ImageSharpProvider
{
    public class ImageSharpProvider : IPluginInstaller
    {
        public void Install(JSValue context)
        {
            Image<Argb32> image = new Image<Argb32>(100,100);
        }

    }
}
