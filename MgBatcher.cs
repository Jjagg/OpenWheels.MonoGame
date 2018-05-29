using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using OpenWheels.Rendering;

namespace OpenWheels.MonoGame
{
    public class MgBatcher : Batcher
    {
        private readonly Dictionary<string, int> _textureIds;
        private readonly MgRenderer _renderer;

        public MgBatcher(GraphicsDevice gd)
        {
            _textureIds = new Dictionary<string, int>();

            _renderer = new MgRenderer(gd);
            Renderer = _renderer;
        }

        public void RegisterTexture(Texture2D texture)
        {
            RegisterTexture(texture, texture.Name);
        }

        public void RegisterTexture(Texture2D texture, string name)
        {
            var id = _renderer.AddTexture(texture);
            _textureIds.Add(name, id);
        }

        public void SetTexture(string texture)
        {
            var id = _textureIds[texture];
            SetTexture(id);
        }
    }
}
