using System;
using ChakraCore.NET;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace ImageSharpProvider
{
    public class ImageSharpProvider : INativePlugin
    {
        public void Install(ChakraContext context)
        {
            Image<Argb32> image = new Image<Argb32>(100,100);
        }
    }
}
