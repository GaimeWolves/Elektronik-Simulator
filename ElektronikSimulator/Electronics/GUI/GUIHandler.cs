using ESim.Electronics.GUI.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESim.Electronics.GUI
{
    public class GUIHandler
    {
        private CircuitView viewport;

        public GUIHandler(GraphicsDevice graphics)
        {
            viewport = new CircuitView(graphics);
        }

        public void Draw(SpriteBatch batch, GraphicsDevice graphics)
        {
            viewport.Draw(batch, graphics);
        }

        public void Update(GameTime gameTime)
        {
            viewport.Update(gameTime);
        }

        public void OnWindwowResized()
        {

        }
    }
}
