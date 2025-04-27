using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.OpenGLES;

namespace Engine.Graphics
{
	public static class GLWrapper
	{
        public static GL GL;

		public static int m_mainFramebuffer;

		public static int m_mainColorbuffer;

		public static int m_arrayBuffer;

		public static int m_elementArrayBuffer;

		public static int m_texture2D;

		public static int[] m_activeTexturesByUnit;

		public static TextureUnit m_activeTextureUnit;

		public static int m_program;

		public static int m_framebuffer;

		public static Vector4? m_clearColor;

		public static float? m_clearDepth;

		public static int? m_clearStencil;

		public static TriangleFace m_cullFace;

		public static FrontFaceDirection m_frontFace;

		public static DepthFunction m_depthFunction;

		public static int? m_colorMask;

		public static bool? m_depthMask;

		public static float m_polygonOffsetFactor;

		public static float m_polygonOffsetUnits;

		public static Vector4 m_blendColor;

		public static BlendEquationModeEXT m_blendEquation;

		public static BlendEquationModeEXT m_blendEquationColor;

		public static BlendEquationModeEXT m_blendEquationAlpha;

		public static BlendingFactor m_blendFuncSource;

		public static BlendingFactor m_blendFuncSourceColor;

		public static BlendingFactor m_blendFuncSourceAlpha;

		public static BlendingFactor m_blendFuncDestination;

		public static BlendingFactor m_blendFuncDestinationColor;

		public static BlendingFactor m_blendFuncDestinationAlpha;

		public static Dictionary<EnableCap, bool> m_enableDisableStates;

		public static bool?[] m_vertexAttribArray;

		public static RasterizerState m_rasterizerState;

		public static DepthStencilState m_depthStencilState;

		public static BlendState m_blendState;

		public static Dictionary<int, SamplerState> m_textureSamplerStates;

		public static Shader m_lastShader;

		public static VertexDeclaration m_lastVertexDeclaration;

		public static IntPtr m_lastVertexOffset;

		public static int m_lastArrayBuffer;

		public static Viewport? m_viewport;

		public static Rectangle? m_scissorRectangle;

		public static bool GL_EXT_texture_filter_anisotropic;

		public static bool GL_OES_packed_depth_stencil;

		public static void Initialize()
        {
#if ANDROID
            GL = GL.GetApi(Window.m_view);
#else
            GL = Window.m_gameWindow.CreateOpenGLES();
#endif
#if DEBUG
            unsafe
            {
                GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero.ToPointer());
                GL.Enable(EnableCap.DebugOutput);
            }
#endif
            int[] bits = new int[6];
            for (int i = 0; i < 6; i++)
            {
                bits[i] = GL.GetInteger((GetPName)(i+3410));
            }
            GL.GetInteger(GetPName.MaxTextureSize, out int maxTextureSize);
            Display.DeviceDescription = $"OpenGL ES, Vendor={GL.GetStringS(StringName.Vendor) ?? string.Empty}, Renderer={GL.GetStringS(StringName.Renderer) ?? string.Empty}, Version={GL.GetStringS(StringName.Version) ?? string.Empty}, R={bits[0]} G={bits[1]} B={bits[2]} A={bits[3]}, D={bits[4]} S={bits[5]}, MaxTextureSize={maxTextureSize}";
            Log.Information("Initialized display device: " + Display.DeviceDescription);
			string @string = GL.GetStringS(StringName.Extensions);
			GL_EXT_texture_filter_anisotropic = @string.Contains("GL_EXT_texture_filter_anisotropic");
			GL_OES_packed_depth_stencil = @string.Contains("GL_OES_packed_depth_stencil");
		}

		public static void InitializeCache()
		{
			m_arrayBuffer = -1;
			m_elementArrayBuffer = -1;
			m_texture2D = -1;
			m_activeTexturesByUnit = new int[8]
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			};
			m_activeTextureUnit = (TextureUnit)(-1);
			m_program = -1;
			m_framebuffer = -1;
			m_clearColor = null;
			m_clearDepth = null;
			m_clearStencil = null;
			m_cullFace = 0;
			m_frontFace = 0;
			m_depthFunction = (DepthFunction)(-1);
			m_colorMask = null;
			m_depthMask = null;
			m_polygonOffsetFactor = 0f;
			m_polygonOffsetUnits = 0f;
			m_blendColor = new Vector4(float.MinValue);
			m_blendEquation = (BlendEquationModeEXT)(-1);
			m_blendEquationColor = (BlendEquationModeEXT)(-1);
			m_blendEquationAlpha = (BlendEquationModeEXT)(-1);
			m_blendFuncSource = (BlendingFactor)(-1);
			m_blendFuncSourceColor = (BlendingFactor)(-1);
			m_blendFuncSourceAlpha = (BlendingFactor)(-1);
			m_blendFuncDestination = (BlendingFactor)(-1);
			m_blendFuncDestinationColor = (BlendingFactor)(-1);
			m_blendFuncDestinationAlpha = (BlendingFactor)(-1);
			m_enableDisableStates = [];
			m_vertexAttribArray = new bool?[16];
			m_rasterizerState = null;
			m_depthStencilState = null;
			m_blendState = null;
			m_textureSamplerStates = [];
			m_lastShader = null;
			m_lastVertexDeclaration = null;
			m_lastVertexOffset = IntPtr.Zero;
			m_lastArrayBuffer = -1;
			m_viewport = null;
			m_scissorRectangle = null;
		}

		public static bool Enable(EnableCap state)
		{
			if (!m_enableDisableStates.TryGetValue(state, out bool value) || !value)
			{
				GL.Enable(state);
				m_enableDisableStates[state] = true;
				return true;
			}
			return false;
		}

		public static bool Disable(EnableCap state)
		{
			if (!m_enableDisableStates.TryGetValue(state, out bool value) | value)
			{
				GL.Disable(state);
				m_enableDisableStates[state] = false;
				return true;
			}
			return false;
		}

		public static bool IsEnabled(EnableCap state)
		{
			if (!m_enableDisableStates.TryGetValue(state, out bool value))
			{
				value = GL.IsEnabled(state);
				m_enableDisableStates[state] = value;
			}
			return value;
		}

		public static void ClearColor(Vector4 color)
		{
			Vector4 value = color;
			Vector4? clearColor = m_clearColor;
			if (value != clearColor)
			{
				GL.ClearColor(color.X, color.Y, color.Z, color.W);
				m_clearColor = color;
			}
		}

		public static void ClearDepth(float depth)
		{
			if (depth != m_clearDepth)
			{
				GL.ClearDepth(depth);
				m_clearDepth = depth;
			}
		}

		public static void ClearStencil(int stencil)
		{
			if (stencil != m_clearStencil)
			{
				GL.ClearStencil(stencil);
				m_clearStencil = stencil;
			}
		}

		public static void CullFace(TriangleFace cullFace)
		{
			if (cullFace != m_cullFace)
			{
				GL.CullFace(cullFace);
				m_cullFace = cullFace;
			}
		}

		public static void FrontFace(FrontFaceDirection frontFace)
		{
			if (frontFace != m_frontFace)
			{
				GL.FrontFace(frontFace);
				m_frontFace = frontFace;
			}
		}

		public static void DepthFunc(DepthFunction depthFunction)
		{
			if (depthFunction != m_depthFunction)
			{
				GL.DepthFunc(depthFunction);
				m_depthFunction = depthFunction;
			}
		}

		public static void ColorMask(int colorMask)
		{
			colorMask &= 0xF;
			if (colorMask != m_colorMask)
			{
				GL.ColorMask((colorMask & 8) != 0, (colorMask & 4) != 0, (colorMask & 2) != 0, (colorMask & 1) != 0);
				m_colorMask = colorMask;
			}
		}

		public static bool DepthMask(bool depthMask)
		{
			if (depthMask != m_depthMask)
			{
				GL.DepthMask(depthMask);
				m_depthMask = depthMask;
				return true;
			}
			return false;
		}

		public static void PolygonOffset(float factor, float units)
		{
			if (factor != m_polygonOffsetFactor || units != m_polygonOffsetUnits)
			{
				GL.PolygonOffset(factor, units);
				m_polygonOffsetFactor = factor;
				m_polygonOffsetUnits = units;
			}
		}

		public static void BlendColor(Vector4 blendColor)
		{
			if (blendColor != m_blendColor)
			{
				GL.BlendColor(blendColor.X, blendColor.Y, blendColor.Z, blendColor.W);
				m_blendColor = blendColor;
			}
		}

		public static void BlendEquation(BlendEquationModeEXT blendEquation)
		{
			if (blendEquation != m_blendEquation)
			{
				GL.BlendEquation(blendEquation);
				m_blendEquation = blendEquation;
				m_blendEquationColor = (BlendEquationModeEXT)(-1);
				m_blendEquationAlpha = (BlendEquationModeEXT)(-1);
			}
		}

		public static void BlendEquationSeparate(BlendEquationModeEXT blendEquationColor, BlendEquationModeEXT blendEquationAlpha)
		{
			if (blendEquationColor != m_blendEquationColor || blendEquationAlpha != m_blendEquationAlpha)
			{
				GL.BlendEquationSeparate(blendEquationColor, blendEquationAlpha);
				m_blendEquationColor = blendEquationColor;
				m_blendEquationAlpha = blendEquationAlpha;
				m_blendEquation = (BlendEquationModeEXT)(-1);
			}
		}

		public static void BlendFunc(BlendingFactor blendFuncSource, BlendingFactor blendFuncDestination)
		{
			if (blendFuncSource != m_blendFuncSource || blendFuncDestination != m_blendFuncDestination)
			{
				GL.BlendFunc(blendFuncSource, blendFuncDestination);
				m_blendFuncSource = blendFuncSource;
				m_blendFuncDestination = blendFuncDestination;
				m_blendFuncSourceColor = (BlendingFactor)(-1);
				m_blendFuncSourceAlpha = (BlendingFactor)(-1);
				m_blendFuncDestinationColor = (BlendingFactor)(-1);
				m_blendFuncDestinationAlpha = (BlendingFactor)(-1);
			}
		}

		public static void BlendFuncSeparate(BlendingFactor blendFuncSourceColor, BlendingFactor blendFuncDestinationColor, BlendingFactor blendFuncSourceAlpha, BlendingFactor blendFuncDestinationAlpha)
		{
			if (blendFuncSourceColor != m_blendFuncSourceColor || blendFuncDestinationColor != m_blendFuncDestinationColor || blendFuncSourceAlpha != m_blendFuncSourceAlpha || blendFuncDestinationAlpha != m_blendFuncDestinationAlpha)
			{
				GL.BlendFuncSeparate(blendFuncSourceColor, blendFuncDestinationColor, blendFuncSourceAlpha, blendFuncDestinationAlpha);
				m_blendFuncSourceColor = blendFuncSourceColor;
				m_blendFuncSourceAlpha = blendFuncSourceAlpha;
				m_blendFuncDestinationColor = blendFuncDestinationColor;
				m_blendFuncDestinationAlpha = blendFuncDestinationAlpha;
				m_blendFuncSource = (BlendingFactor)(-1);
                m_blendFuncDestination = (BlendingFactor)(-1);
			}
		}

		public static void VertexAttribArray(int index, bool enable)
		{
            uint uIndex = (uint)index;
			if (enable && (!m_vertexAttribArray[index].HasValue || !m_vertexAttribArray[index].Value))
			{
				GL.EnableVertexAttribArray(uIndex);
				m_vertexAttribArray[index] = true;
			}
			else if (!enable && (!m_vertexAttribArray[index].HasValue || m_vertexAttribArray[index].Value))
			{
				GL.DisableVertexAttribArray(uIndex);
				m_vertexAttribArray[index] = false;
			}
		}

		public static void BindTexture(TextureTarget target, int texture, bool forceBind)
		{
            uint uTexture = (uint)texture;
			if (target == TextureTarget.Texture2D)
			{
				if (forceBind || texture != m_texture2D)
				{
					GL.BindTexture(target, uTexture);
					m_texture2D = texture;
					if (m_activeTextureUnit >= 0)
					{
						m_activeTexturesByUnit[(int)(m_activeTextureUnit - 33984)] = texture;
					}
				}
			}
			else
			{
				GL.BindTexture(target, uTexture);
			}
		}

		public static void ActiveTexture(TextureUnit textureUnit)
		{
			if (textureUnit != m_activeTextureUnit)
			{
				GL.ActiveTexture(textureUnit);
				m_activeTextureUnit = textureUnit;
			}
		}

		public static void BindBuffer(BufferTargetARB target, int buffer)
		{
            uint uBuffer = (uint)buffer;
			switch (target)
			{
				case BufferTargetARB.ArrayBuffer:
					if (buffer != m_arrayBuffer)
					{
						GL.BindBuffer(target, uBuffer);
						m_arrayBuffer = buffer;
					}
					break;
				case BufferTargetARB.ElementArrayBuffer:
					if (buffer != m_elementArrayBuffer)
					{
						GL.BindBuffer(target, uBuffer);
						m_elementArrayBuffer = buffer;
					}
					break;
				default:
					GL.BindBuffer(target, uBuffer);
					break;
			}
		}

		public static void BindFramebuffer(int framebuffer)
		{
			if (framebuffer != m_framebuffer)
			{
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)framebuffer);
				m_framebuffer = framebuffer;
			}
		}

		public static void UseProgram(int program)
		{
			if (program != m_program)
			{
				GL.UseProgram((uint)program);
				m_program = program;
			}
		}

		public static void DeleteProgram(int program)
		{
			if (m_program == program)
			{
				m_program = -1;
			}
			GL.DeleteProgram((uint)program);
		}

		public static void DeleteTexture(int texture)
		{
			if (m_texture2D == texture)
			{
				m_texture2D = -1;
			}
			for (int i = 0; i < m_activeTexturesByUnit.Length; i++)
			{
				if (m_activeTexturesByUnit[i] == texture)
				{
					m_activeTexturesByUnit[i] = -1;
				}
			}
			m_textureSamplerStates.Remove(texture);
			GL.DeleteTexture((uint)texture);
		}

		public static void DeleteFramebuffer(int framebuffer)
		{
			if (m_framebuffer == framebuffer)
			{
				m_framebuffer = -1;
			}
			GL.DeleteFramebuffers(1, (uint)framebuffer);
		}

		public static void DeleteBuffer(BufferTargetARB target, int buffer)
		{
			if (target == BufferTargetARB.ArrayBuffer)
			{
				if (m_arrayBuffer == buffer)
				{
					m_arrayBuffer = -1;
				}
				if (m_lastArrayBuffer == buffer)
				{
					m_lastArrayBuffer = -1;
				}
			}
			if (target == BufferTargetARB.ElementArrayBuffer && m_elementArrayBuffer == buffer)
			{
				m_elementArrayBuffer = -1;
			}
			GL.DeleteBuffers(1u, (uint)buffer);
		}

		public static void ApplyViewportScissor(Viewport viewport, Rectangle scissorRectangle, bool isScissorEnabled)
		{
			if (!m_viewport.HasValue || viewport.X != m_viewport.Value.X || viewport.Y != m_viewport.Value.Y || viewport.Width != m_viewport.Value.Width || viewport.Height != m_viewport.Value.Height)
			{
				int y = (Display.RenderTarget == null) ? (Display.BackbufferSize.Y - viewport.Y - viewport.Height) : viewport.Y;
				GL.Viewport(viewport.X, y, (uint)viewport.Width, (uint)viewport.Height);
			}
			if (!m_viewport.HasValue || viewport.MinDepth != m_viewport.Value.MinDepth || viewport.MaxDepth != m_viewport.Value.MaxDepth)
			{
				GL.DepthRange(viewport.MinDepth, viewport.MaxDepth);
			}
			m_viewport = viewport;
			if (!isScissorEnabled)
			{
				return;
			}
			if (m_scissorRectangle.HasValue)
			{
				Rectangle value = scissorRectangle;
				Rectangle? scissorRectangle2 = m_scissorRectangle;
				if (!(value != scissorRectangle2))
				{
					return;
				}
			}
			if (Display.RenderTarget == null)
			{
				scissorRectangle.Top = Display.BackbufferSize.Y - scissorRectangle.Top - scissorRectangle.Height;
			}
			GL.Scissor(scissorRectangle.Left, scissorRectangle.Top, (uint)scissorRectangle.Width, (uint)scissorRectangle.Height);
			m_scissorRectangle = scissorRectangle;
		}

		public static void ApplyRasterizerState(RasterizerState state)
		{
			if (state != m_rasterizerState)
			{
				m_rasterizerState = state;
				switch (state.CullMode)
				{
					case CullMode.None:
						Disable(EnableCap.CullFace);
						break;
					case CullMode.CullClockwise:
						Enable(EnableCap.CullFace);
						CullFace(TriangleFace.Back);
						FrontFace((Display.RenderTarget != null) ? FrontFaceDirection.CW : FrontFaceDirection.Ccw);
						break;
					case CullMode.CullCounterClockwise:
						Enable(EnableCap.CullFace);
						CullFace(TriangleFace.Back);
						FrontFace((Display.RenderTarget != null) ? FrontFaceDirection.Ccw : FrontFaceDirection.CW);
						break;
				}
				if (state.ScissorTestEnable)
				{
					Enable(EnableCap.ScissorTest);
				}
				else
				{
					Disable(EnableCap.ScissorTest);
				}
				if (state.DepthBias != 0f || state.SlopeScaleDepthBias != 0f)
				{
					Enable(EnableCap.PolygonOffsetFill);
					PolygonOffset(state.SlopeScaleDepthBias, state.DepthBias);
				}
				else
				{
					Disable(EnableCap.PolygonOffsetFill);
				}
			}
		}

		public static void ApplyDepthStencilState(DepthStencilState state)
		{
			if (state == m_depthStencilState)
			{
				return;
			}
			m_depthStencilState = state;
			if (state.DepthBufferTestEnable || state.DepthBufferWriteEnable)
			{
				Enable(EnableCap.DepthTest);
				if (state.DepthBufferTestEnable)
				{
					DepthFunc(TranslateCompareFunction(state.DepthBufferFunction));
				}
				else
				{
					DepthFunc(DepthFunction.Always);
				}
				DepthMask(state.DepthBufferWriteEnable);
			}
			else
			{
				Disable(EnableCap.DepthTest);
			}
		}

		public static void ApplyBlendState(BlendState state)
		{
			if (state == m_blendState)
			{
				return;
			}
			m_blendState = state;
			if (state.ColorBlendFunction == BlendFunction.Add && state.ColorSourceBlend == Blend.One && state.ColorDestinationBlend == Blend.Zero && state.AlphaBlendFunction == BlendFunction.Add && state.AlphaSourceBlend == Blend.One && state.AlphaDestinationBlend == Blend.Zero)
			{
				Disable(EnableCap.Blend);
				return;
			}
            BlendEquationModeEXT all = TranslateBlendFunction(state.ColorBlendFunction);
            BlendEquationModeEXT all2 = TranslateBlendFunction(state.AlphaBlendFunction);
            BlendingFactor all3 = TranslateBlendSrc(state.ColorSourceBlend);
            BlendingFactor all4 = TranslateBlendDest(state.ColorDestinationBlend);
            BlendingFactor all5 = TranslateBlendSrc(state.AlphaSourceBlend);
            BlendingFactor all6 = TranslateBlendDest(state.AlphaDestinationBlend);
			if (all == all2 && all3 == all5 && all4 == all6)
			{
				BlendEquation(all);
				BlendFunc(all3, all4);
			}
			else
			{
				BlendEquationSeparate(all, all2);
				BlendFuncSeparate(all3, all4, all5, all6);
			}
			BlendColor(state.BlendFactor);
			Enable(EnableCap.Blend);
		}

		public static void ApplyRenderTarget(RenderTarget2D renderTarget)
		{
			if (renderTarget != null)
			{
				BindFramebuffer(renderTarget.m_frameBuffer);
			}
			else
			{
				BindFramebuffer(m_mainFramebuffer);
			}
		}

		public unsafe static void ApplyShaderAndBuffers(Shader shader, VertexDeclaration vertexDeclaration, IntPtr vertexOffset, int arrayBuffer, int? elementArrayBuffer)
		{
			shader.PrepareForDrawing();
			BindBuffer(BufferTargetARB.ArrayBuffer, arrayBuffer);
			if (elementArrayBuffer.HasValue)
			{
				BindBuffer(BufferTargetARB.ElementArrayBuffer, elementArrayBuffer.Value);
			}
			UseProgram(shader.m_program);
			if (shader != m_lastShader || vertexOffset != m_lastVertexOffset || arrayBuffer != m_lastArrayBuffer || vertexDeclaration.m_elements != m_lastVertexDeclaration.m_elements)
			{
				Shader.VertexAttributeData[] vertexAttribData = shader.GetVertexAttribData(vertexDeclaration);
				for (int i = 0; i < vertexAttribData.Length; i++)
				{
					if (vertexAttribData[i].Size != 0)
					{
						GL.VertexAttribPointer((uint)i, vertexAttribData[i].Size, vertexAttribData[i].Type, vertexAttribData[i].Normalize, (uint)vertexDeclaration.VertexStride, (vertexOffset + vertexAttribData[i].Offset).ToPointer());
						VertexAttribArray(i, enable: true);
					}
					else
					{
						VertexAttribArray(i, enable: false);
					}
				}
				m_lastShader = shader;
				m_lastVertexDeclaration = vertexDeclaration;
				m_lastVertexOffset = vertexOffset;
				m_lastArrayBuffer = arrayBuffer;
			}
			int num = 0;
			int num2 = 0;
			ShaderParameter shaderParameter;
			while (true)
			{
				if (num2 >= shader.m_parameters.Length)
				{
					return;
				}
				shaderParameter = shader.m_parameters[num2];
				if (shaderParameter.IsChanged)
				{
					switch (shaderParameter.Type)
					{
						case ShaderParameterType.Float:
							GL.Uniform1(shaderParameter.Location, (uint)shaderParameter.Count, shaderParameter.Value);
							shaderParameter.IsChanged = false;
							break;
						case ShaderParameterType.Vector2:
							GL.Uniform2(shaderParameter.Location, (uint)shaderParameter.Count, shaderParameter.Value);
							shaderParameter.IsChanged = false;
							break;
						case ShaderParameterType.Vector3:
							GL.Uniform3(shaderParameter.Location, (uint)shaderParameter.Count, shaderParameter.Value);
							shaderParameter.IsChanged = false;
							break;
						case ShaderParameterType.Vector4:
							GL.Uniform4(shaderParameter.Location, (uint)shaderParameter.Count, shaderParameter.Value);
							shaderParameter.IsChanged = false;
							break;
						case ShaderParameterType.Matrix:
							GL.UniformMatrix4(shaderParameter.Location, (uint)shaderParameter.Count, transpose: false, shaderParameter.Value);
							shaderParameter.IsChanged = false;
							break;
						default:
							throw new InvalidOperationException("Unsupported shader parameter type.");
						case ShaderParameterType.Texture2D:
						case ShaderParameterType.Sampler2D:
							break;
					}
				}
				if (shaderParameter.Type == ShaderParameterType.Texture2D)
				{
					if (num >= 8)
					{
						throw new InvalidOperationException("Too many simultaneous textures.");
					}
					ActiveTexture((TextureUnit)(33984 + num));
					if (shaderParameter.IsChanged)
					{
						GL.Uniform1(shaderParameter.Location, num);
					}
					ShaderParameter obj = shader.m_parameters[num2 + 1];
					var texture2D = (Texture2D)shaderParameter.Resource;
					var samplerState = (SamplerState)obj.Resource;
					if (texture2D != null)
					{
						if (samplerState == null)
						{
							break;
						}
						if (m_activeTexturesByUnit[num] != texture2D.m_texture)
						{
							BindTexture(TextureTarget.Texture2D, texture2D.m_texture, forceBind: true);
						}
						if (!m_textureSamplerStates.TryGetValue(texture2D.m_texture, out SamplerState value) || value != samplerState)
						{
							BindTexture(TextureTarget.Texture2D, texture2D.m_texture, forceBind: false);
							if (GL_EXT_texture_filter_anisotropic)
							{
                                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, (samplerState.FilterMode == TextureFilterMode.Anisotropic) ? samplerState.MaxAnisotropy : 1f);
                            }
							GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TranslateTextureFilterModeMin(samplerState.FilterMode, texture2D.MipLevelsCount > 1));
							GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TranslateTextureFilterModeMag(samplerState.FilterMode));
							GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TranslateTextureAddressMode(samplerState.AddressModeU));
							GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TranslateTextureAddressMode(samplerState.AddressModeV));
#if !ANDROID
							GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinLod, samplerState.MinLod);
							GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLod, samplerState.MaxLod);
#endif
							m_textureSamplerStates[texture2D.m_texture] = samplerState;
						}
					}
					else if (m_activeTexturesByUnit[num] != 0)
					{
						BindTexture(TextureTarget.Texture2D, 0, forceBind: true);
					}
					num++;
					shaderParameter.IsChanged = false;
				}
				num2++;
			}
			throw new InvalidOperationException($"Associated SamplerState is not set for texture \"{shaderParameter.Name}\".");
		}

		public static void Clear(RenderTarget2D renderTarget, Vector4? color, float? depth, int? stencil)
		{
            ClearBufferMask all = ClearBufferMask.None;
			if (color.HasValue)
			{
				all |= ClearBufferMask.ColorBufferBit;
				ClearColor(color.Value);
				ColorMask(15);
			}
			if (depth.HasValue)
			{
				all |= ClearBufferMask.DepthBufferBit;
				ClearDepth(depth.Value);
				if (DepthMask(depthMask: true))
				{
					m_depthStencilState = null;
				}
			}
			if (stencil.HasValue)
			{
				all |= ClearBufferMask.StencilBufferBit;
				ClearStencil(stencil.Value);
			}
			if (all != 0)
			{
				ApplyRenderTarget(renderTarget);
				if (Disable(EnableCap.ScissorTest))
				{
					m_rasterizerState = null;
				}
				GL.Clear(all);
			}
		}

		public static void HandleContextLost()
		{
			try
			{
				Log.Information("Device lost");
				Display.HandleDeviceLost();
				GC.Collect();
				InitializeCache();
				Display.Resize();
				Display.HandleDeviceReset();
				Log.Information("Device reset");
			}
			catch (Exception ex)
			{
				Log.Error("Failed to recreate graphics resources. Reason: {0}", ex.Message);
			}
		}

		public static void TranslateVertexElementFormat(VertexElementFormat vertexElementFormat, out VertexAttribPointerType type, out bool normalize)
		{
			switch (vertexElementFormat)
			{
				case VertexElementFormat.Single:
					type = VertexAttribPointerType.Float;
					normalize = false;
					break;
				case VertexElementFormat.Vector2:
					type = VertexAttribPointerType.Float;
					normalize = false;
					break;
				case VertexElementFormat.Vector3:
					type = VertexAttribPointerType.Float;
					normalize = false;
					break;
				case VertexElementFormat.Vector4:
					type = VertexAttribPointerType.Float;
					normalize = false;
					break;
				case VertexElementFormat.Byte4:
					type = VertexAttribPointerType.UnsignedByte;
					normalize = false;
					break;
				case VertexElementFormat.NormalizedByte4:
					type = VertexAttribPointerType.UnsignedByte;
					normalize = true;
					break;
				case VertexElementFormat.Short2:
					type = VertexAttribPointerType.Short;
					normalize = false;
					break;
				case VertexElementFormat.NormalizedShort2:
					type = VertexAttribPointerType.Short;
					normalize = true;
					break;
				case VertexElementFormat.Short4:
					type = VertexAttribPointerType.Short;
					normalize = false;
					break;
				case VertexElementFormat.NormalizedShort4:
					type = VertexAttribPointerType.Short;
					normalize = true;
					break;
				default:
					throw new InvalidOperationException("Unsupported vertex element format.");
			}
		}

		public static DrawElementsType TranslateIndexFormat(IndexFormat indexFormat)
		{
			return indexFormat switch
			{
				IndexFormat.SixteenBits => DrawElementsType.UnsignedShort,
				IndexFormat.ThirtyTwoBits => DrawElementsType.UnsignedInt,
				_ => throw new InvalidOperationException("Unsupported index format."),
			};
		}

		public static ShaderParameterType TranslateActiveUniformType(UniformType type)
		{
			return type switch
			{
				UniformType.Float => ShaderParameterType.Float,
				UniformType.FloatVec2 => ShaderParameterType.Vector2,
				UniformType.FloatVec3 => ShaderParameterType.Vector3,
				UniformType.FloatVec4 => ShaderParameterType.Vector4,
				UniformType.FloatMat4 => ShaderParameterType.Matrix,
				UniformType.Sampler2D => ShaderParameterType.Texture2D,
				_ => throw new InvalidOperationException("Unsupported shader parameter type."),
			};
		}

		public static Silk.NET.OpenGLES.PrimitiveType TranslatePrimitiveType(PrimitiveType primitiveType)
		{
			return primitiveType switch
			{
				PrimitiveType.LineList => Silk.NET.OpenGLES.PrimitiveType.Lines,
				PrimitiveType.LineStrip => Silk.NET.OpenGLES.PrimitiveType.LineStrip,
				PrimitiveType.TriangleList => Silk.NET.OpenGLES.PrimitiveType.Triangles,
				PrimitiveType.TriangleStrip => Silk.NET.OpenGLES.PrimitiveType.TriangleStrip,
                PrimitiveType.Points => Silk.NET.OpenGLES.PrimitiveType.Points,
                PrimitiveType.LineLoop => Silk.NET.OpenGLES.PrimitiveType.LineLoop,
                PrimitiveType.TriangleFan => Silk.NET.OpenGLES.PrimitiveType.TriangleFan,
				_ => throw new InvalidOperationException("Unsupported primitive type."),
			};
		}

		public static TextureMinFilter TranslateTextureFilterModeMin(TextureFilterMode filterMode, bool isMipmapped)
		{
			switch (filterMode)
			{
				case TextureFilterMode.Point:
					if (!isMipmapped)
					{
						return TextureMinFilter.Nearest;
					}
					return TextureMinFilter.NearestMipmapNearest;
				case TextureFilterMode.Linear:
					if (!isMipmapped)
					{
						return TextureMinFilter.Linear;
					}
					return TextureMinFilter.LinearMipmapLinear;
				case TextureFilterMode.Anisotropic:
					if (!isMipmapped)
					{
						return TextureMinFilter.Linear;
					}
					return TextureMinFilter.LinearMipmapLinear;
				case TextureFilterMode.PointMipLinear:
					if (!isMipmapped)
					{
						return TextureMinFilter.Nearest;
					}
					return TextureMinFilter.NearestMipmapLinear;
				case TextureFilterMode.LinearMipPoint:
					if (!isMipmapped)
					{
						return TextureMinFilter.Linear;
					}
					return TextureMinFilter.LinearMipmapNearest;
				case TextureFilterMode.MinPointMagLinearMipPoint:
					if (!isMipmapped)
					{
						return TextureMinFilter.Nearest;
					}
					return TextureMinFilter.NearestMipmapNearest;
				case TextureFilterMode.MinPointMagLinearMipLinear:
					if (!isMipmapped)
					{
						return TextureMinFilter.Nearest;
					}
					return TextureMinFilter.NearestMipmapLinear;
				case TextureFilterMode.MinLinearMagPointMipPoint:
					if (!isMipmapped)
					{
						return TextureMinFilter.Linear;
					}
					return TextureMinFilter.LinearMipmapNearest;
				case TextureFilterMode.MinLinearMagPointMipLinear:
					if (!isMipmapped)
					{
						return TextureMinFilter.Linear;
					}
					return TextureMinFilter.LinearMipmapLinear;
				default:
					throw new InvalidOperationException("Unsupported texture filter mode.");
			}
		}

		public static TextureMagFilter TranslateTextureFilterModeMag(TextureFilterMode filterMode)
		{
			return filterMode switch
			{
				TextureFilterMode.Point => TextureMagFilter.Nearest,
				TextureFilterMode.Linear => TextureMagFilter.Linear,
				TextureFilterMode.Anisotropic => TextureMagFilter.Linear,
				TextureFilterMode.PointMipLinear => TextureMagFilter.Nearest,
				TextureFilterMode.LinearMipPoint => TextureMagFilter.Nearest,
				TextureFilterMode.MinPointMagLinearMipPoint => TextureMagFilter.Linear,
				TextureFilterMode.MinPointMagLinearMipLinear => TextureMagFilter.Linear,
				TextureFilterMode.MinLinearMagPointMipPoint => TextureMagFilter.Nearest,
				TextureFilterMode.MinLinearMagPointMipLinear => TextureMagFilter.Nearest,
				_ => throw new InvalidOperationException("Unsupported texture filter mode."),
			};
		}

		public static TextureWrapMode TranslateTextureAddressMode(TextureAddressMode addressMode)
		{
			return addressMode switch
			{
				TextureAddressMode.Clamp => TextureWrapMode.ClampToEdge,
				TextureAddressMode.Wrap => TextureWrapMode.Repeat,
				_ => throw new InvalidOperationException("Unsupported texture address mode."),
			};
		}

		public static DepthFunction TranslateCompareFunction(CompareFunction compareFunction)
		{
			return compareFunction switch
			{
				CompareFunction.Always => DepthFunction.Always,
				CompareFunction.Equal => DepthFunction.Equal,
				CompareFunction.Greater => DepthFunction.Greater,
				CompareFunction.GreaterEqual => DepthFunction.Gequal,
				CompareFunction.Less => DepthFunction.Less,
				CompareFunction.LessEqual => DepthFunction.Lequal,
				CompareFunction.Never => DepthFunction.Never,
				CompareFunction.NotEqual => DepthFunction.Notequal,
				_ => throw new InvalidOperationException("Unsupported texture address mode."),
			};
		}

		public static BlendEquationModeEXT TranslateBlendFunction(BlendFunction blendFunction)
		{
			return blendFunction switch
			{
				BlendFunction.Add => BlendEquationModeEXT.FuncAdd,
				BlendFunction.Subtract => BlendEquationModeEXT.FuncSubtract,
				BlendFunction.ReverseSubtract => BlendEquationModeEXT.FuncReverseSubtract,
				_ => throw new InvalidOperationException("Unsupported blend function."),
			};
		}

		public static BlendingFactor TranslateBlendSrc(Blend blend)
		{
			return blend switch
			{
				Blend.Zero => 0,
				Blend.One => (BlendingFactor)1,
				Blend.SourceColor => BlendingFactor.SrcColor,
				Blend.InverseSourceColor => BlendingFactor.OneMinusSrcColor,
				Blend.DestinationColor => BlendingFactor.DstColor,
				Blend.InverseDestinationColor => BlendingFactor.OneMinusDstColor,
				Blend.SourceAlpha => BlendingFactor.SrcAlpha,
				Blend.InverseSourceAlpha => BlendingFactor.OneMinusSrcAlpha,
				Blend.DestinationAlpha => BlendingFactor.DstAlpha,
				Blend.InverseDestinationAlpha => BlendingFactor.OneMinusDstAlpha,
				Blend.BlendFactor => BlendingFactor.ConstantColor,
				Blend.InverseBlendFactor => BlendingFactor.OneMinusConstantColor,
				Blend.SourceAlphaSaturation => BlendingFactor.SrcAlphaSaturate,
				_ => throw new InvalidOperationException("Unsupported blend."),
			};
        }
        public static BlendingFactor TranslateBlendDest(Blend blend)
        {
            return blend switch
            {
                Blend.Zero => 0,
                Blend.One => (BlendingFactor)1,
                Blend.SourceColor => BlendingFactor.SrcColor,
                Blend.InverseSourceColor => BlendingFactor.OneMinusSrcColor,
                Blend.DestinationColor => BlendingFactor.DstColor,
                Blend.InverseDestinationColor => BlendingFactor.OneMinusDstColor,
                Blend.SourceAlpha => BlendingFactor.SrcAlpha,
                Blend.InverseSourceAlpha => BlendingFactor.OneMinusSrcAlpha,
                Blend.DestinationAlpha => BlendingFactor.DstAlpha,
                Blend.InverseDestinationAlpha => BlendingFactor.OneMinusDstAlpha,
                Blend.BlendFactor => BlendingFactor.ConstantColor,
                Blend.InverseBlendFactor => BlendingFactor.OneMinusConstantColor,
                Blend.SourceAlphaSaturation => BlendingFactor.SrcAlphaSaturate,
                _ => throw new InvalidOperationException("Unsupported blend."),
            };
        }

        public static InternalFormat TranslateDepthFormat(DepthFormat depthFormat)
		{
			return depthFormat switch
			{
				DepthFormat.Depth16 => InternalFormat.DepthComponent16,
#if ANDROID
                DepthFormat.Depth24Stencil8 => GL_OES_packed_depth_stencil ? InternalFormat.Depth24Stencil8 : InternalFormat.DepthComponent16,
#else
				DepthFormat.Depth24Stencil8 => InternalFormat.Depth24Stencil8,
#endif
                _ => throw new InvalidOperationException("Unsupported DepthFormat."),
			};
        }
#if DEBUG
        static readonly DebugProc DebugMessageDelegate = (source, type, id, severity, length, pMessage, param) =>
        {
            if (type == GLEnum.DebugTypeOther)
            {
                return;
            }
            string message = Marshal.PtrToStringAnsi(pMessage, length);
            Console.WriteLine($"[{type.ToString().Substring(9)}] {message}");
            if (type == GLEnum.DebugTypeError)
            {
                Debugger.Break();
            }
        };
#endif
        [Conditional("DEBUG")]
		public static void CheckGLError()
		{
		}
	}
}