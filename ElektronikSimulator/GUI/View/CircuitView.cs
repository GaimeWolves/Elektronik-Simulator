using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESim.Util;
using ESim.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ESim.GUI.View
{
    public class CircuitView
    {
        private static readonly int NoZoom = 100;
        private static readonly int MinZoom = 10;
        private static readonly int MaxZoom = 250;

        private readonly Texture2D Dot;

        private int size;
        private Vector2 offset;

        public CircuitView(GraphicsDevice graphics)
        {
            size = NoZoom;
            offset = new Vector2();
            Dot = Utilities.CreateTexture(graphics, 2, 2, (p) => Color.White);
        }

        public void Draw(SpriteBatch batch, GraphicsDevice graphics)
        {
            batch.Begin();
            for (int x = -1; x <= graphics.Viewport.Width / size + 1; x++)
                for (int y = -1; y <= graphics.Viewport.Height / size + 1; y++)
                    batch.Draw(
                        Dot,
                        new Vector2(
                            offset.X % size + x * size - Dot.Width / 2,
                            offset.Y % size + y * size - Dot.Height / 2
                            ),
                        Color.Black
                        );

            batch.Draw(
                Dot,
                new Rectangle(
                    (int)offset.X - (int)(Dot.Width * 1.5),
                    (int)offset.Y - (int)(Dot.Height * 1.5),
                    Dot.Width * 3,
                    Dot.Height * 3
                    ),
                Color.Blue);
            batch.End();
        }

        public void Update(GameTime gameTime)
        {
            if (InputHandler.IsKeyPressed(Keys.LeftControl) || InputHandler.IsKeyPressed(Keys.RightControl))
            {
                if (InputHandler.DeltaScrollWheel != 0)
                    Zoom((int)InputHandler.DeltaScrollWheel);

                if (InputHandler.IsKeyPressed(Keys.Up))
                    Zoom(1);
                else if (InputHandler.IsKeyPressed(Keys.Down))
                    Zoom(-1);
            }

            if (InputHandler.IsMouseButtonPressed(MouseButton.Left))
            {
                if (InputHandler.DeltaMousePosition.Length() > 0)
                    offset -= InputHandler.DeltaMousePosition;
            }
        }

        private void Zoom(int value) => size = Math.Max(MinZoom, Math.Min(MaxZoom, size + value));
    }
}
