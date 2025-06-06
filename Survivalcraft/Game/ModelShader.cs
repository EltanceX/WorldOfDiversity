using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Game
{
	public class ModelShader : Shader
	{
		public ShaderParameter m_worldMatrixParameter;

		public ShaderParameter m_worldViewProjectionMatrixParameter;

		public ShaderParameter m_textureParameter;

		public ShaderParameter m_samplerStateParameter;

		public ShaderParameter m_materialColorParameter;

		public ShaderParameter m_emissionColorParameter;

		public ShaderParameter m_alphaThresholdParameter;

		public ShaderParameter m_ambientLightColorParameter;

		public ShaderParameter m_diffuseLightColor1Parameter;

		public ShaderParameter m_directionToLight1Parameter;

		public ShaderParameter m_diffuseLightColor2Parameter;

		public ShaderParameter m_directionToLight2Parameter;

		public ShaderParameter m_fogColorParameter;

		private ShaderParameter m_fogBottomTopDensityParameter;

		private ShaderParameter m_hazeStartDensityParameter;

		public ShaderParameter m_fogYMultiplierParameter;

		public ShaderParameter m_worldUpParameter;

		public int m_instancesCount;

		public readonly ShaderTransforms Transforms;

		public Texture2D Texture
		{
			set
			{
				m_textureParameter.SetValue(value);
			}
		}

		public SamplerState SamplerState
		{
			set
			{
				m_samplerStateParameter.SetValue(value);
			}
		}

		public Vector4 MaterialColor
		{
			set
			{
				m_materialColorParameter.SetValue(value);
			}
		}

		public Vector4 EmissionColor
		{
			set
			{
				m_emissionColorParameter.SetValue(value);
			}
		}

		public float AlphaThreshold
		{
			set
			{
				m_alphaThresholdParameter.SetValue(value);
			}
		}

		public Vector3 AmbientLightColor
		{
			set
			{
				m_ambientLightColorParameter.SetValue(value);
			}
		}

		public Vector3 DiffuseLightColor1
		{
			set
			{
				m_diffuseLightColor1Parameter.SetValue(value);
			}
		}

		public Vector3 DiffuseLightColor2
		{
			set
			{
				m_diffuseLightColor2Parameter.SetValue(value);
			}
		}

		public Vector3 LightDirection1
		{
			set
			{
				m_directionToLight1Parameter.SetValue(-value);
			}
		}

		public Vector3 LightDirection2
		{
			set
			{
				m_directionToLight2Parameter.SetValue(-value);
			}
		}

		public Vector3 FogColor
		{
			set
			{
				m_fogColorParameter.SetValue(value);
			}
		}

		public Vector3 FogBottomTopDensity
		{
			set
			{
				m_fogBottomTopDensityParameter.SetValue(value);
			}
		}

		public Vector2 HazeStartDensity
		{
			set
			{
				m_hazeStartDensityParameter.SetValue(value);
			}
		}

		public float FogYMultiplier
		{
			set
			{
				m_fogYMultiplierParameter.SetValue(value);
			}
		}

		public Vector3 WorldUp
		{
			set
			{
				m_worldUpParameter.SetValue(value);
			}
		}

		public int InstancesCount
		{
			get
			{
				return m_instancesCount;
			}
			set
			{
				if (value < 0 || value > Transforms.MaxWorldMatrices)
				{
					throw new InvalidOperationException("Invalid instances count.");
				}
				m_instancesCount = value;
			}
		}

		public ModelShader(string vsc, string psc, bool useAlphaThreshold, int maxInstancesCount = 1)
		   : base(vsc, psc, PrepareShaderMacros(useAlphaThreshold, maxInstancesCount))
		{
			SetParameter();
			Transforms = new ShaderTransforms(maxInstancesCount);
		}

		public ModelShader(string vsc, string psc, bool useAlphaThreshold, int maxInstancesCount = 1, ShaderMacro[] shaderMacros = null)
			: base(vsc, psc, PrepareShaderMacros(useAlphaThreshold, maxInstancesCount, shaderMacros))
		{
			SetParameter();
			Transforms = new ShaderTransforms(maxInstancesCount);
		}

		public void SetParameter()
		{
			m_worldMatrixParameter = GetParameter("u_worldMatrix");
			m_worldViewProjectionMatrixParameter = GetParameter("u_worldViewProjectionMatrix");
			m_textureParameter = GetParameter("u_texture");
			m_samplerStateParameter = GetParameter("u_samplerState");
			m_materialColorParameter = GetParameter("u_materialColor");
			m_emissionColorParameter = GetParameter("u_emissionColor");
			m_alphaThresholdParameter = GetParameter("u_alphaThreshold", allowNull: true);
			m_ambientLightColorParameter = GetParameter("u_ambientLightColor");
			m_diffuseLightColor1Parameter = GetParameter("u_diffuseLightColor1");
			m_directionToLight1Parameter = GetParameter("u_directionToLight1");
			m_diffuseLightColor2Parameter = GetParameter("u_diffuseLightColor2");
			m_directionToLight2Parameter = GetParameter("u_directionToLight2");
			m_fogColorParameter = GetParameter("u_fogColor");
			m_fogBottomTopDensityParameter = GetParameter("u_fogBottomTopDensity");
			m_hazeStartDensityParameter = GetParameter("u_hazeStartDensity");
			m_fogYMultiplierParameter = GetParameter("u_fogYMultiplier");
			m_worldUpParameter = GetParameter("u_worldUp");
		}

        public override void PrepareForDrawingOverride()
		{
			Transforms.UpdateMatrices(m_instancesCount, worldView: false, viewProjection: false, worldViewProjection: true);
			m_worldViewProjectionMatrixParameter.SetValue(Transforms.WorldViewProjection, InstancesCount);
			m_worldMatrixParameter.SetValue(Transforms.World, InstancesCount);
		}

		public static ShaderMacro[] PrepareShaderMacros(bool useAlphaThreshold, int maxInstancesCount, ShaderMacro[] shaderMacros = null)
		{
			var list = new List<ShaderMacro>();
			if (useAlphaThreshold)
			{
				list.Add(new ShaderMacro("ALPHATESTED"));
			}
			list.Add(new ShaderMacro("MAX_INSTANCES_COUNT", maxInstancesCount.ToString(CultureInfo.InvariantCulture)));
			if (shaderMacros != null && shaderMacros.Length > 0)
			{
				foreach (ShaderMacro shaderMacro in shaderMacros)
				{
					list.Add(shaderMacro);
				}
			}
			return list.ToArray();
		}
	}
}
