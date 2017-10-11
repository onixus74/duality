﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality;
using Duality.Editor;
using Duality.Input;
using Duality.Resources;
using Duality.Drawing;
using Duality.Components;
using Duality.Components.Renderers;

namespace Duality.Samples.Benchmarks
{
	[EditorHintCategory("Benchmarks")]
    public class BenchmarkMultiSpriteRenderer : Renderer, ICmpInitializable
	{
		private int spriteCount = 1000;

		[DontSerialize] private Material sharedMaterial = new Material(DrawTechnique.Mask, Texture.DualityIcon);
		[DontSerialize] private RawList<Vector2> spritePositions = new RawList<Vector2>();
		[DontSerialize] private RawList<float> spriteAngles = new RawList<float>();
		[DontSerialize] private RawList<VertexC1P3T2> vertices = new RawList<VertexC1P3T2>();
		[DontSerialize] private Random random = new Random(1);


		public int SpriteCount
		{
			get { return this.spriteCount; }
			set { this.spriteCount = value; }
		}
		public override float BoundRadius
		{
			get { return MathF.Sqrt(2.0f) * 512.0f + 32.0f; }
		}


		private void SetupSprites()
		{
			this.spritePositions.Count = this.spriteCount;
			this.spriteAngles.Count = this.spriteCount;

			Vector2[] pos = this.spritePositions.Data;
			float[] angle = this.spriteAngles.Data;

			Vector2 spriteBoxSize = new Vector2(1024, 1024);
			for (int i = 0; i < this.spriteCount; i++)
			{
				pos[i] = this.random.NextVector2(
					-spriteBoxSize.X * 0.5f, 
					-spriteBoxSize.Y * 0.5f, 
					spriteBoxSize.X, 
					spriteBoxSize.Y);
				angle[i] = this.random.NextFloat(MathF.RadAngle360);
			}
		}

		public override void Draw(IDrawDevice device)
		{
			Vector3 posTemp = this.GameObj.Transform.Pos;
			float scaleTemp = 1.0f;
			device.PreprocessCoords(ref posTemp, ref scaleTemp);

			int count = this.spritePositions.Count;
			this.vertices.Count = count * 4;

			ColorRgba mainClr = ColorRgba.White;
			Rect spriteRect = Rect.Align(Alignment.Center, 0.0f, 0.0f, 64.0f, 64.0f);
			Rect uvRect = new Rect(this.sharedMaterial.MainTexture.Res.UVRatio);
			float left = uvRect.X;
			float right = uvRect.RightX;
			float top = uvRect.Y;
			float bottom = uvRect.BottomY;

			VertexC1P3T2[] vert = this.vertices.Data;
			Vector2[] pos = this.spritePositions.Data;
			float[] angle = this.spriteAngles.Data;
			for (int i = 0; i < count; i++)
			{
				Vector2 edge1 = spriteRect.TopLeft;
				Vector2 edge2 = spriteRect.BottomLeft;
				Vector2 edge3 = spriteRect.BottomRight;
				Vector2 edge4 = spriteRect.TopRight;

				Vector2 spriteXDot, spriteYDot;
				MathF.GetTransformDotVec(angle[i], out spriteXDot, out spriteYDot);

				MathF.TransformDotVec(ref edge1, ref spriteXDot, ref spriteYDot);
				MathF.TransformDotVec(ref edge2, ref spriteXDot, ref spriteYDot);
				MathF.TransformDotVec(ref edge3, ref spriteXDot, ref spriteYDot);
				MathF.TransformDotVec(ref edge4, ref spriteXDot, ref spriteYDot);

				vert[i * 4 + 0].Pos.X = posTemp.X + (pos[i].X + edge1.X) * scaleTemp;
				vert[i * 4 + 0].Pos.Y = posTemp.Y + (pos[i].Y + edge1.Y) * scaleTemp;
				vert[i * 4 + 0].Pos.Z = posTemp.Z;
				vert[i * 4 + 0].TexCoord.X = left;
				vert[i * 4 + 0].TexCoord.Y = top;
				vert[i * 4 + 0].Color = mainClr;

				vert[i * 4 + 1].Pos.X = posTemp.X + (pos[i].X + edge2.X) * scaleTemp;
				vert[i * 4 + 1].Pos.Y = posTemp.Y + (pos[i].Y + edge2.Y) * scaleTemp;
				vert[i * 4 + 1].Pos.Z = posTemp.Z;
				vert[i * 4 + 1].TexCoord.X = left;
				vert[i * 4 + 1].TexCoord.Y = bottom;
				vert[i * 4 + 1].Color = mainClr;

				vert[i * 4 + 2].Pos.X = posTemp.X + (pos[i].X + edge3.X) * scaleTemp;
				vert[i * 4 + 2].Pos.Y = posTemp.Y + (pos[i].Y + edge3.Y) * scaleTemp;
				vert[i * 4 + 2].Pos.Z = posTemp.Z;
				vert[i * 4 + 2].TexCoord.X = right;
				vert[i * 4 + 2].TexCoord.Y = bottom;
				vert[i * 4 + 2].Color = mainClr;

				vert[i * 4 + 3].Pos.X = posTemp.X + (pos[i].X + edge4.X) * scaleTemp;
				vert[i * 4 + 3].Pos.Y = posTemp.Y + (pos[i].Y + edge4.Y) * scaleTemp;
				vert[i * 4 + 3].Pos.Z = posTemp.Z;
				vert[i * 4 + 3].TexCoord.X = right;
				vert[i * 4 + 3].TexCoord.Y = top;
				vert[i * 4 + 3].Color = mainClr;
			}

			device.AddVertices(this.sharedMaterial, VertexMode.Quads, vert, this.vertices.Count);
		}
		void ICmpInitializable.OnInit(Component.InitContext context)
		{
			if (context == InitContext.Activate && DualityApp.ExecContext == DualityApp.ExecutionContext.Game)
			{
				this.SetupSprites();
			}
		}
		void ICmpInitializable.OnShutdown(Component.ShutdownContext context)
		{
			if (context == ShutdownContext.Deactivate)
			{
				this.vertices.Count = 0;
				this.spritePositions.Count = 0;
				this.spriteAngles.Count = 0;
			}
		}
	}
}
