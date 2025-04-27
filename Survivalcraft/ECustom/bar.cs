using DebugMod;
using Engine;
using Engine.Graphics;
using GMLForAPI;
using System;

namespace Game
{
    public class ValueBarWidget2 : ValueBarWidget
    {

        public override void Draw(DrawContext dc)
        {
            //BaseBatch baseBatch = (BarSubtexture == null) ? dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None) : ((BaseBatch)dc.PrimitivesRenderer2D.TexturedBatch(BarSubtexture.Texture, useAlphaTest: false, 0, DepthStencilState.None, null, null, TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp));
            BaseBatch baseBatch = dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None);
            int num = 0;
            int start = 0;
            if (baseBatch is TexturedBatch2D)
            {
                num = ((TexturedBatch2D)baseBatch).TriangleVertices.Count;
            }
            else
            {
                start = ((FlatBatch2D)baseBatch).LineVertices.Count;
                num = ((FlatBatch2D)baseBatch).TriangleVertices.Count;
            }
            Vector2 zero = Vector2.Zero;
            if (m_layoutDirection == LayoutDirection.Horizontal)
            {
                zero.X += Spacing / 2f;
            }
            else
            {
                zero.Y += Spacing / 2f;
            }
            int num2 = HalfBars ? 1 : 2;
            for (int i = 0; i < 2 * BarsCount; i += num2)
            {
                bool flag = i % 2 == 0;
                //float num3 = 0.5f * i;
                //float num4 = 0f;
                //num4 = (!FlipDirection) ? Math.Clamp((Value - (num3 / BarsCount)) * BarsCount, 0f, 1f) : Math.Clamp((Value - ((BarsCount - num3 - 1f) / BarsCount)) * BarsCount, 0f, 1f);
                //if (!BarBlending)
                //{
                //    num4 = MathF.Ceiling(num4);
                //}
                //float s = (m_flashCount > 0f) ? (1f - MathF.Abs(MathF.Sin(m_flashCount * (float)Math.PI))) : 1f;
                //Color c = LitBarColor;
                //if (LitBarColor2 != Color.Transparent && BarsCount > 1)
                //{
                //    c = Color.Lerp(LitBarColor, LitBarColor2, num3 / (BarsCount - 1));
                //}
                //Color color = Color.Lerp(UnlitBarColor, c, num4) * s * GlobalColorTransform;
                //if (HalfBars)
                //{
                //    if (flag)
                //    {
                //        Vector2 zero2 = Vector2.Zero;
                //        Vector2 v = (m_layoutDirection == LayoutDirection.Horizontal) ? new Vector2(0.5f, 1f) : new Vector2(1f, 0.5f);
                //        if (baseBatch is TexturedBatch2D)
                //        {
                //            Vector2 topLeft = BarSubtexture.TopLeft;
                //            var texCoord = new Vector2(MathUtils.Lerp(BarSubtexture.TopLeft.X, BarSubtexture.BottomRight.X, v.X), MathUtils.Lerp(BarSubtexture.TopLeft.Y, BarSubtexture.BottomRight.Y, v.Y));
                //            ((TexturedBatch2D)baseBatch).QueueQuad(zero + (zero2 * BarSize), zero + (v * BarSize), 0f, topLeft, texCoord, color);
                //        }
                //        else
                //        {
                //            ((FlatBatch2D)baseBatch).QueueQuad(zero + (zero2 * BarSize), zero + (v * BarSize), 0f, color);
                //        }
                //    }
                //    else
                //    {
                //        Vector2 v2 = (m_layoutDirection == LayoutDirection.Horizontal) ? new Vector2(0.5f, 0f) : new Vector2(0f, 0.5f);
                //        Vector2 one = Vector2.One;
                //        if (baseBatch is TexturedBatch2D)
                //        {
                //            var texCoord2 = new Vector2(MathUtils.Lerp(BarSubtexture.TopLeft.X, BarSubtexture.BottomRight.X, v2.X), MathUtils.Lerp(BarSubtexture.TopLeft.Y, BarSubtexture.BottomRight.Y, v2.Y));
                //            Vector2 bottomRight = BarSubtexture.BottomRight;
                //            ((TexturedBatch2D)baseBatch).QueueQuad(zero + (v2 * BarSize), zero + (one * BarSize), 0f, texCoord2, bottomRight, color);
                //        }
                //        else
                //        {
                //            ((FlatBatch2D)baseBatch).QueueQuad(zero + (v2 * BarSize), zero + (one * BarSize), 0f, color);
                //        }
                //    }
                //}
                //else
                //{
                Vector2 zero3 = Vector2.Zero;
                    Vector2 one2 = Vector2.One;
                    //if (baseBatch is TexturedBatch2D)
                    //{
                        //Vector2 topLeft2 = BarSubtexture.TopLeft;
                        //Vector2 bottomRight2 = BarSubtexture.BottomRight;
                        //((TexturedBatch2D)baseBatch).QueueQuad(zero + (zero3 * BarSize), zero + (one2 * BarSize), 0f, topLeft2, bottomRight2, color);
                        //ScreenLog.Info(m_value);
                        var size2 = BarSize;
                        size2.X *= 5f;
                        size2.X *= m_value;
                        size2.Y = 2f;
                        //((TexturedBatch2D)baseBatch).QueueQuad(zero + (zero3 * BarSize), zero + (one2 * size2), 0f, topLeft2, bottomRight2, Color.Red);
                        Vector2 begin = zero;
                        begin.Y += 8;
                        begin.X += 2;
                        ((FlatBatch2D)baseBatch).QueueQuad(begin, begin + (one2 * size2), 0f, GUtil.ProgressToColor(m_value));
                    //}
                    //else
                    //{
                        //((FlatBatch2D)baseBatch).QueueQuad(zero + (zero3 * BarSize), zero + (one2 * BarSize), 0f, color);
                        //((FlatBatch2D)baseBatch).QueueRectangle(zero + (zero3 * BarSize), zero + (one2 * BarSize), 0f, Color.MultiplyColorOnly(color, 0.75f));
                    //}
                //}
                if (!flag || !HalfBars)
                {
                    if (m_layoutDirection == LayoutDirection.Horizontal)
                    {
                        zero.X += BarSize.X + Spacing;
                    }
                    else
                    {
                        zero.Y += BarSize.Y + Spacing;
                    }
                }
            }
            //if (baseBatch is TexturedBatch2D)
            //{
            //    ((TexturedBatch2D)baseBatch).TransformTriangles(GlobalTransform, num);
            //}
            //else
            //{
                ((FlatBatch2D)baseBatch).TransformLines(GlobalTransform, start);
                ((FlatBatch2D)baseBatch).TransformTriangles(GlobalTransform, num);
            //}
            m_flashCount = MathUtils.Max(m_flashCount - (4f * Time.FrameDuration), 0f);
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            IsDrawRequired = true;
            DesiredSize = m_layoutDirection == LayoutDirection.Horizontal
                ? new Vector2((BarSize.X + Spacing) * BarsCount, BarSize.Y)
                : new Vector2(BarSize.X, (BarSize.Y + Spacing) * BarsCount);
        }
    }
}
