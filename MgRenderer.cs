﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using OpenWheels.Rendering;
using Vector2 = System.Numerics.Vector2;
using SamplerState = OpenWheels.Rendering.SamplerState;
using BlendState = OpenWheels.Rendering.BlendState;

namespace OpenWheels.MonoGame
{
    public class MgRenderer : IRenderer
    {
        private int _nextTextureId;
        private int _nextFontId;

        public GraphicsDevice GraphicsDevice { get; }
        private readonly Dictionary<int, Texture2D> _textures;
        private readonly Dictionary<int, SpriteFont> _fonts;

        private readonly Effect _effect;

        private DynamicVertexBuffer _vertexBuffer;
        private DynamicIndexBuffer _indexBuffer;

        private int _vertexCount;
        private int _indexCount;

        public MgRenderer(GraphicsDevice gd)
        {
            GraphicsDevice = gd;
            _textures = new Dictionary<int, Texture2D>();
            _fonts = new Dictionary<int, SpriteFont>();

            // note that this effect already performs the 2D transformation
            // so we don't need it in the batcher
            _effect = new SpriteEffect(gd);
        }

        public int AddTexture(Texture2D texture)
        {
            var id = GetTextureId();
            _textures.Add(id, texture);
            return id;
        }

        private int GetTextureId()
        {
            var id = _nextTextureId;
            _nextTextureId++;
            return id;
        }

        public int AddFont(SpriteFont font)
        {
            var id = GetFontId();
            _fonts.Add(id, font);
            return id;
        }

        private int GetFontId()
        {
            var id = _nextFontId;
            _nextFontId++;
            return id;
        }

        #region IRenderer Implementation

        public Point2 GetTextureSize(int texture)
        {
            var tex = _textures[texture];
            return new Point2(tex.Width, tex.Height);
        }

        public Vector2 GetTextSize(string text, int font)
        {
            var sf = _fonts[font];
            var vec = sf.MeasureString(text);
            return new Vector2(vec.X, vec.Y);
        }

        public Rectangle GetViewport()
        {
            var rect = GraphicsDevice.Viewport.Bounds;
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        private void EnsureBufferSize(int vbs, int ibs)
        {
            if (_vertexBuffer == null || _vertexBuffer.VertexCount < vbs)
            {
                _vertexBuffer?.Dispose();
                _vertexBuffer = new DynamicVertexBuffer(GraphicsDevice, VertexPositionColorTexture.VertexDeclaration, vbs, BufferUsage.WriteOnly);
            }

            if (_indexBuffer == null || _indexBuffer.IndexCount < ibs)
            {
                _indexBuffer?.Dispose();
                _indexBuffer = new DynamicIndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, ibs, BufferUsage.WriteOnly);
            }
        }

        public void BeginRender(Vertex[] vertexBuffer, int[] indexBuffer, int vertexCount, int indexCount)
        {
            EnsureBufferSize(vertexCount, indexCount);
            _vertexBuffer.SetData(vertexBuffer, 0, vertexCount);
            _indexBuffer.SetData(indexBuffer, 0, indexCount);
            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            GraphicsDevice.Indices = _indexBuffer;
        }

        public void DrawBatch(GraphicsState state, int startIndex, int indexCount, object batchUserData)
        {
            SetGraphicsState(state);
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, startIndex, indexCount / 3);
        }

        public void EndRender()
        {
        }

        #endregion

        #region Helper Methods

        private void SetGraphicsState(GraphicsState state)
        {
            switch (state.SamplerState)
            {
                case SamplerState.PointWrap:
                    GraphicsDevice.SamplerStates[0] = Microsoft.Xna.Framework.Graphics.SamplerState.PointWrap;
                    break;
                case SamplerState.PointClamp:
                    GraphicsDevice.SamplerStates[0] = Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp;
                    break;
                case SamplerState.LinearWrap:
                    GraphicsDevice.SamplerStates[0] = Microsoft.Xna.Framework.Graphics.SamplerState.LinearWrap;
                    break;
                case SamplerState.LinearClamp:
                    GraphicsDevice.SamplerStates[0] = Microsoft.Xna.Framework.Graphics.SamplerState.LinearClamp;
                    break;
                case SamplerState.AnisotropicWrap:
                    GraphicsDevice.SamplerStates[0] = Microsoft.Xna.Framework.Graphics.SamplerState.AnisotropicWrap;
                    break;
                case SamplerState.AnisotropicClamp:
                    GraphicsDevice.SamplerStates[0] = Microsoft.Xna.Framework.Graphics.SamplerState.AnisotropicClamp;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (state.BlendState)
            {
                case BlendState.AlphaBlend:
                    GraphicsDevice.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend;
                    break;
                case BlendState.Opaque:
                    GraphicsDevice.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Opaque;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            if (state.UseScissorRect)
                GraphicsDevice.ScissorRectangle = state.ScissorRect.ToMg();

            _effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Textures[0] = _textures[state.Texture];
        }

        #endregion
    }
}
