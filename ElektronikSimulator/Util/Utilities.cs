using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESim.Util
{
    public static class Utilities
    {
        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            Texture2D texture = new Texture2D(device, width, height);
            
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
                data[pixel] = paint(pixel);
            
            texture.SetData(data);

            return texture;
        }
    }
}
