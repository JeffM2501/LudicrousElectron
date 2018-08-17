using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using LudicrousElectron.Engine.Graphics;
using LudicrousElectron.GUI.Geometry;


namespace LudicrousElectron.GUI.Drawing.Sprite
{
    public static class SlicedSprite
    {
        private static void AddQuad(PrimitivBuffer target, Vector2 minV, Vector2 maxV, Vector2 minT, Vector2 maxT)
        {
            target.Vertex(minV.X, minV.Y);
            target.UVs.Add(new Vector2(minT.X, maxT.Y));

            target.Vertex(maxV.X, minV.Y);
            target.UVs.Add(new Vector2(maxT.X, maxT.Y));

            target.Vertex(maxV.X, maxV.Y);
            target.UVs.Add(new Vector2(maxT.X, minT.Y));

            target.Vertex(minV.X, maxV.Y);
            target.UVs.Add(new Vector2(minT.X, minT.Y));
        }

        public static void FourSlice(PrimitivBuffer target, RelativeRect rect, Vector2 textureBounds)
        {
            var origin = rect.GetPixelOrigin();
            var size = rect.GetPixelSize();

            float texWidth = textureBounds.X;
            float texHeight = textureBounds.Y;

            if (size.X <= texWidth || size.Y <= texHeight) // it's too small to do pixel based quad sprite, let it do an axis stretch, it's the best we can hope for
            {
                StrechedBuffer.Stretched(target, rect);
                return;
            }

            float halfWidth = texWidth * 0.5f;
            float halfHeight = texHeight * 0.5f;

            target.Clear();
            target.DrawType = PrimitiveType.Quads;


            // lower
            Vector2 minV = new Vector2(origin.X, origin.Y);
            Vector2 maxV = new Vector2(origin.X + halfWidth, origin.Y + halfHeight);

            Vector2 minT = new Vector2(0, 0.5f);
            Vector2 maxT = new Vector2(0.5f, 1);

            // left side
            AddQuad(target, minV, maxV, minT, maxT);


            // center
            minV = new Vector2(origin.X + halfWidth, origin.Y);
            maxV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + halfHeight);

            minT = new Vector2(0.5f, 0.5f);
            maxT = new Vector2(0.5f, 1);
            AddQuad(target, minV, maxV, minT, maxT);

            // right
            minV = new Vector2(origin.X + size.Y - halfWidth, origin.Y);
            maxV = new Vector2(origin.X + size.Y, origin.Y + halfHeight);

            minT = new Vector2(0.5f, 0.5f);
            maxT = new Vector2(1, 1);
            AddQuad(target, minV, maxV, minT, maxT);


            // middle
            // left  
            minV = new Vector2(origin.X, origin.Y + halfHeight);
            maxV = new Vector2(origin.X + halfWidth, origin.Y + size.Y - halfHeight);

            minT = new Vector2(0, 0.5f);
            maxT = new Vector2(0.5f, 0.5f);

            AddQuad(target, minV, maxV, minT, maxT);

            // center
            minV = new Vector2(origin.X + halfWidth, origin.Y + halfHeight);
            maxV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + size.Y - halfHeight);

            minT = new Vector2(0.5f, 0.5f);
            maxT = new Vector2(0.5f, 0.5f);
            AddQuad(target, minV, maxV, minT, maxT);

            // right
            minV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + halfHeight);
            maxV = new Vector2(origin.X + size.Y,  origin.Y + size.Y - halfHeight);

            minT = new Vector2(0.5f, 0.5f);
            maxT = new Vector2(1, 0.5f);
            AddQuad(target, minV, maxV, minT, maxT);


            // upper
            // left  
            minV = new Vector2(origin.X, origin.Y + size.Y - halfHeight);
            maxV = new Vector2(origin.X + halfWidth, origin.Y + size.Y );

            minT = new Vector2(0, 0);
            maxT = new Vector2(0.5f, 0.5f);

            AddQuad(target, minV, maxV, minT, maxT);

            // center
            minV = new Vector2(origin.X + halfWidth, origin.Y + size.Y - halfHeight);
            maxV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + size.Y);

            minT = new Vector2(0.5f, 0);
            maxT = new Vector2(0.5f, 0.5f);
            AddQuad(target, minV, maxV, minT, maxT);

            // right
            minV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + size.Y - halfHeight);
            maxV = new Vector2(origin.X + size.Y, origin.Y + size.Y );

            minT = new Vector2(0.5f, 0);
            maxT = new Vector2(1, 0.5f);
            AddQuad(target, minV, maxV, minT, maxT);
        }

        public class NineSpriteSliceInfo
        {
            public float LeftSlice = 1.0f / 3.0f;
            public float RightSlice = 2.0f / 3.0f;
            public float TopSlice = 2.0f / 3.0f;
            public float BottomSlice = 1.0f / 3.0f;

            public static readonly NineSpriteSliceInfo Defaut = new NineSpriteSliceInfo();
        }

        public static void NineSlice(PrimitivBuffer target, RelativeRect rect, Vector2 textureBounds, NineSpriteSliceInfo sliceInfo = null)
        {
            var origin = rect.GetPixelOrigin();
            var size = rect.GetPixelSize();

            if (sliceInfo == null)
                sliceInfo = NineSpriteSliceInfo.Defaut;

            float texWidth = textureBounds.X;
            float texHeight = textureBounds.Y;

            if (size.X <= texWidth || size.Y <= texHeight) // it's too small to do pixel based quad sprite, let it do an axis stretch, it's the best we can hope for
            {
                StrechedBuffer.Stretched(target, rect);
                return;
            }

            float halfWidth = texWidth * 0.5f;
            float halfHeight = texHeight * 0.5f;

            target.Clear();
            target.DrawType = PrimitiveType.Quads;


            // lower
            Vector2 minV = new Vector2(origin.X, origin.Y);
            Vector2 maxV = new Vector2(origin.X + halfWidth, origin.Y + halfHeight);

            Vector2 minT = new Vector2(0, sliceInfo.TopSlice);
            Vector2 maxT = new Vector2(sliceInfo.LeftSlice, 1);

            // left side
            AddQuad(target, minV, maxV, minT, maxT);


            // center
            minV = new Vector2(origin.X + halfWidth, origin.Y);
            maxV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + halfHeight);

            minT = new Vector2(sliceInfo.LeftSlice, sliceInfo.TopSlice);
            maxT = new Vector2(sliceInfo.RightSlice, 1);
            AddQuad(target, minV, maxV, minT, maxT);

            // right
            minV = new Vector2(origin.X + size.Y - halfWidth, origin.Y);
            maxV = new Vector2(origin.X + size.Y, origin.Y + halfHeight);

            minT = new Vector2(sliceInfo.RightSlice, sliceInfo.TopSlice);
            maxT = new Vector2(1, 1);
            AddQuad(target, minV, maxV, minT, maxT);


            // middle
            // left  
            minV = new Vector2(origin.X, origin.Y + halfHeight);
            maxV = new Vector2(origin.X + halfWidth, origin.Y + size.Y - halfHeight);

            minT = new Vector2(0, sliceInfo.BottomSlice);
            maxT = new Vector2(sliceInfo.LeftSlice, sliceInfo.TopSlice);

            AddQuad(target, minV, maxV, minT, maxT);

            // center
            minV = new Vector2(origin.X + halfWidth, origin.Y + halfHeight);
            maxV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + size.Y - halfHeight);

            minT = new Vector2(sliceInfo.LeftSlice, sliceInfo.BottomSlice);
            maxT = new Vector2(sliceInfo.RightSlice, sliceInfo.TopSlice);
            AddQuad(target, minV, maxV, minT, maxT);

            // right
            minV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + halfHeight);
            maxV = new Vector2(origin.X + size.Y, origin.Y + size.Y - halfHeight);

            minT = new Vector2(sliceInfo.RightSlice, sliceInfo.BottomSlice);
            maxT = new Vector2(1, sliceInfo.TopSlice);
            AddQuad(target, minV, maxV, minT, maxT);

            // upper
            // left  
            minV = new Vector2(origin.X, origin.Y + size.Y - halfHeight);
            maxV = new Vector2(origin.X + halfWidth, origin.Y + size.Y);

            minT = new Vector2(0, 0);
            maxT = new Vector2(sliceInfo.LeftSlice, sliceInfo.BottomSlice);

            AddQuad(target, minV, maxV, minT, maxT);

            // center
            minV = new Vector2(origin.X + halfWidth, origin.Y + size.Y - halfHeight);
            maxV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + size.Y);

            minT = new Vector2(sliceInfo.LeftSlice, 0);
            maxT = new Vector2(sliceInfo.RightSlice, sliceInfo.BottomSlice);
            AddQuad(target, minV, maxV, minT, maxT);

            // right
            minV = new Vector2(origin.X + size.Y - halfWidth, origin.Y + size.Y - halfHeight);
            maxV = new Vector2(origin.X + size.Y, origin.Y + size.Y);

            minT = new Vector2(sliceInfo.RightSlice, 0);
            maxT = new Vector2(1, sliceInfo.BottomSlice);
            AddQuad(target, minV, maxV, minT, maxT);
        }
    }
}
