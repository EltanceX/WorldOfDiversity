using Engine;
using GameEntitySystem;
using Jint.Native;
using System;
using System.Collections.Generic;
using TemplatesDatabase;
using static Game.ComponentLevel;

namespace Game
{
	public enum FactorAdditionType
	{
		Multiply,
		Add
	}
	public class ComponentLevel : ComponentFactors, IUpdateable
	{
		/// <summary>
		/// 这里的Factor类型从struct改为class，是由于模组在修改Factor的时候，通常是需要修改引用的值。
		/// 如果是struct则只能复制并修改值，不能修改引用。
		/// Description: 在玩家信息面板上显示影响因素的名称，如“未患流感”
		/// Value：该影响因素的具体数值
		/// Name：该Factor的索引名称，模组使用Name来在m_xxxFactors列表中查找对应的Factor
		/// FactorAdditionType：该影响因子是乘算还是加算
		/// </summary>
		public class Factor
		{
			public string Description;

			public float Value;

			public string Name;

			public FactorAdditionType FactorAdditionType = FactorAdditionType.Multiply;
		}

		public static string fName = "ComponentLevel";

		public float? m_lastLevelTextValue;

		public ComponentPlayer m_componentPlayer;

		public const float FemaleStrengthFactor = 0.8f;

		public const float FemaleResilienceFactor = 0.8f;

		public const float FemaleSpeedFactor = 1.03f;

		public const float FemaleHungerFactor = 0.7f;

		public virtual void AddExperience(int count, bool playSound)
		{
			if (playSound)
			{
				m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 0.2f, m_random.Float(-0.1f, 0.4f), 0f, 0f);
			}
			for (int i = 0; i < count; i++)
			{
				float num = 0.012f / MathF.Pow(1.08f, MathF.Floor(m_componentPlayer.PlayerData.Level - 1f));
				if (MathF.Floor(m_componentPlayer.PlayerData.Level + num) > MathF.Floor(m_componentPlayer.PlayerData.Level))
				{
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.0, delegate
					{
						m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(fName, 1), Color.White, blinking: true, playNotificationSound: false);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.0, delegate
					{
						m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, -0.2f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.15000000596046448, delegate
					{
						m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, -0.03333333f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.30000001192092896, delegate
					{
						m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, 142f / (339f * (float)Math.PI), 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.45000001788139343, delegate
					{
						m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, 23f / 60f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.75, delegate
					{
						m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, -0.03333333f, 0f, 0f);
					});
					Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.5 + 0.90000003576278687, delegate
					{
						m_subsystemAudio.PlaySound("Audio/ExperienceCollected", 1f, 23f / 60f, 0f, 0f);
					});
				}
				m_componentPlayer.PlayerData.Level += num;
			}
		}

		/// <summary>
		/// 生成玩家的所有关于力量的因素
		/// </summary>
		public override void GenerateStrengthFactors()
		{
			base.GenerateStrengthFactors();
			m_strengthFactors.Add(new Factor
			{
				Name = "PlayerClass",
				Value = (m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.8f : 1f,
				Description = m_componentPlayer.PlayerData.PlayerClass.ToString()
			});
			float level = m_componentPlayer.PlayerData.Level;
			float num3 = 1f + (0.05f * MathF.Floor(Math.Clamp(level,1f,21f) - 1f));
			m_strengthFactors.Add(new Factor
			{
				Name = "Level",
				Value = num3,
				Description = string.Format(LanguageControl.Get(fName,2),MathF.Floor(level).ToString())
			});
			float stamina = m_componentPlayer.ComponentVitalStats.Stamina;
			float num5 = MathUtils.Lerp(0.5f, 1f, MathUtils.Saturate(4f * stamina)) * MathUtils.Lerp(0.9f, 1f, MathUtils.Saturate(stamina));
			m_strengthFactors.Add(new Factor
			{
				Name = "Stamina",
				Value = num5,
				Description = string.Format(LanguageControl.Get(fName,3),$"{stamina * 100f:0}")
			});
			m_strengthFactors.Add(new Factor
			{
				Name = "IsSick",
				Value = m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f,
				Description = m_componentPlayer.ComponentSickness.IsSick ? LanguageControl.Get(fName,4) : LanguageControl.Get(fName,5)
			});
			m_strengthFactors.Add(new Factor
			{
				Name = "IsPuking",
				Value = (!m_componentPlayer.ComponentSickness.IsPuking) ? 1 : 0,
				Description = m_componentPlayer.ComponentSickness.IsPuking ? LanguageControl.Get(fName,6) : LanguageControl.Get(fName,7)
			});
			m_strengthFactors.Add(new Factor
			{
				Name = "HasFlu",
				Value = m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f,
				Description = m_componentPlayer.ComponentFlu.HasFlu ? LanguageControl.Get(fName,8) : LanguageControl.Get(fName,9)
			});
			m_strengthFactors.Add(new Factor
			{
				Name = "IsCoughing",
				Value = (!m_componentPlayer.ComponentFlu.IsCoughing) ? 1 : 0,
				Description = m_componentPlayer.ComponentFlu.IsCoughing ? LanguageControl.Get(fName,10) : LanguageControl.Get(fName,11)
			});
			float num15 = (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless) ? 1.25f : 1f;
			m_strengthFactors.Add(new Factor
			{
				Name = "GameMode",
				Value = num15,
				Description = string.Format(LanguageControl.Get(fName,12),m_subsystemGameInfo.WorldSettings.GameMode.ToString())
			});
		}

		/// <summary>
		/// 生成玩家所有关于防御的因素
		/// </summary>
		public override void GenerateResilienceFactors()
		{
			base.GenerateResilienceFactors();
			m_resilienceFactors.Add(new Factor
			{
				Name = "PlayerClass",
				Value = (m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.8f : 1f,
				Description = m_componentPlayer.PlayerData.PlayerClass.ToString()
			});
			float level = m_componentPlayer.PlayerData.Level;
			float num3 = 1f + (0.05f * MathF.Floor(Math.Clamp(level,1f,21f) - 1f));
			m_resilienceFactors.Add(new Factor
			{
				Name = "Level",
				Value = num3,
				Description = string.Format(LanguageControl.Get(fName,2),MathF.Floor(level).ToString())
			});
			m_resilienceFactors.Add(new Factor
			{
				Name = "IsSick",
				Value = m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f,
				Description = m_componentPlayer.ComponentSickness.IsSick ? LanguageControl.Get(fName,4) : LanguageControl.Get(fName,5)
			});
			m_resilienceFactors.Add(new Factor
			{
				Name = "HasFlu",
				Value = m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f,
				Description = m_componentPlayer.ComponentFlu.HasFlu ? LanguageControl.Get(fName,8) : LanguageControl.Get(fName,9)
			});
			float num9 = 1f;
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
			{
				num9 = 1.5f;
			}
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Survival)
			{
				num9 = 1.25f;
			}
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				num9 = float.PositiveInfinity;
			}
			m_resilienceFactors.Add(new Factor
			{
				Name = "GameMode",
				Value = num9,
				Description = string.Format(LanguageControl.Get(fName,12),m_subsystemGameInfo.WorldSettings.GameMode.ToString())
			});
		}

		/// <summary>
		/// 生成玩家所有关于速度的因素
		/// </summary>
		public override void GenerateSpeedFactors()
		{
			base.GenerateSpeedFactors();
			m_speedFactors.Add(new Factor
			{
				Name = "PlayerClass",
				Value = (m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 1.03f : 1f,
				Description = m_componentPlayer.PlayerData.PlayerClass.ToString()
			});
			float level = m_componentPlayer.PlayerData.Level;
			float num3 = 1f + (0.02f * MathF.Floor(Math.Clamp(level, 1f, 21f) - 1f));
			m_speedFactors.Add(new Factor
			{
				Name = "Level",
				Value = num3,
				Description = string.Format(LanguageControl.Get(fName,2),MathF.Floor(level).ToString())
			});
			float clothingFactor = 1f;
			foreach(ClothingSlot clothingSlot in ClothingSlot.ClothingSlots.Values)
			{
				foreach(int clothe in m_componentPlayer.ComponentClothing.GetClothes(clothingSlot))
				{
					GenerateClothingSpeedFactors(clothe);
				}
			}
			float stamina = m_componentPlayer.ComponentVitalStats.Stamina;
			float num4 = MathUtils.Lerp(0.5f, 1f, MathUtils.Saturate(4f * stamina)) * MathUtils.Lerp(0.9f, 1f, MathUtils.Saturate(stamina));
			m_speedFactors.Add(new Factor
			{
				Name = "Stamina",
				Value = num4,
				Description = string.Format(LanguageControl.Get(fName,3),$"{stamina * 100f:0}")
			});
			m_speedFactors.Add(new Factor
			{
				Name = "IsSick",
				Value = m_componentPlayer.ComponentSickness.IsSick ? 0.75f : 1f,
				Description = m_componentPlayer.ComponentSickness.IsSick ? LanguageControl.Get(fName,4) : LanguageControl.Get(fName,5)
			});
			m_speedFactors.Add(new Factor
			{
				Name = "IsPuking",
				Value = (!m_componentPlayer.ComponentSickness.IsPuking) ? 1 : 0,
				Description = m_componentPlayer.ComponentSickness.IsPuking ? LanguageControl.Get(fName,6) : LanguageControl.Get(fName,7)
			});
			m_speedFactors.Add(new Factor
			{
				Name = "HasFlu",
				Value = m_componentPlayer.ComponentFlu.HasFlu ? 0.75f : 1f,
				Description = m_componentPlayer.ComponentFlu.HasFlu ? LanguageControl.Get(fName,8) : LanguageControl.Get(fName,9)
			});
			m_speedFactors.Add(new Factor
			{
				Name = "IsCoughing",
				Value = (!m_componentPlayer.ComponentFlu.IsCoughing) ? 1 : 0,
				Description = m_componentPlayer.ComponentFlu.IsCoughing ? LanguageControl.Get(fName,10) : LanguageControl.Get(fName,11)
			});
		}

		/// <summary>
		/// 生成玩家所有关于饥饿的因素
		/// </summary>
		public override void GenerateHungerFactors()
		{
			base.GenerateHungerFactors();
			m_hungerFactors.Add(new Factor
			{
				Name = "PlayerClass",
				Value = (m_componentPlayer.PlayerData.PlayerClass == PlayerClass.Female) ? 0.7f : 1f,
				Description = m_componentPlayer.PlayerData.PlayerClass.ToString()
			});
			float level = m_componentPlayer.PlayerData.Level;
			float num3 = 1f - (0.01f * MathF.Floor(Math.Clamp(level, 1f, 21f) - 1f));
			m_hungerFactors.Add(new Factor
			{
				Name = "Level",
				Value = num3,
				Description = string.Format(LanguageControl.Get(fName,2),MathF.Floor(level).ToString())
			});
			float num5 = 1f;
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Harmless)
			{
				num5 = 0.66f;
			}
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Survival)
			{
				num5 = 0.75f;
			}
			if (m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				num5 = 0f;
			}
			m_hungerFactors.Add(new Factor
			{
				Name = "GameMode",
				Value = num5,
				Description = string.Format(LanguageControl.Get(fName,12),LanguageControl.Get("GameMode", m_subsystemGameInfo.WorldSettings.GameMode.ToString()))
			});
		}

		public override void Update(float dt)
		{
			if (m_subsystemTime.PeriodicGameTimeEvent(180.0, 179.0))
			{
				AddExperience(1, playSound: false);
			}
			if (!m_lastLevelTextValue.HasValue || m_lastLevelTextValue.Value != MathF.Floor(m_componentPlayer.PlayerData.Level))
			{
				m_componentPlayer.ComponentGui.LevelLabelWidget.Text = string.Format(LanguageControl.Get(fName, 2), MathF.Floor(m_componentPlayer.PlayerData.Level).ToString());
				m_lastLevelTextValue = MathF.Floor(m_componentPlayer.PlayerData.Level);
			}
			m_componentPlayer.PlayerStats.HighestLevel = MathUtils.Max(m_componentPlayer.PlayerStats.HighestLevel, m_componentPlayer.PlayerData.Level);
			base.Update(dt);
			ModsManager.HookAction("OnLevelUpdate", modLoader =>
			{
				modLoader.OnLevelUpdate(this);
				return false;
			});
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_componentPlayer = Entity.FindComponent<ComponentPlayer>(throwOnError: true);
		}

		public void GenerateClothingSpeedFactors(int clothingValue)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(clothingValue)];
			ClothingData clothingData = block.GetClothingData(clothingValue);
			if(clothingData != null && clothingData.MovementSpeedFactor != 1f)
			{
				m_speedFactors.Add(new Factor
				{
					Name = "Clothing " + clothingValue.ToString(),
					Value = clothingData.MovementSpeedFactor,
					Description = clothingData.DisplayName
				});
			}
		}

		[Obsolete("Use GenerateClothingSpeedFactors.")]
		public static void AddClothingFactor(int clothingValue, ref float clothingFactor, ICollection<Factor> factors)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(clothingValue)];
			ClothingData clothingData = block.GetClothingData(clothingValue);
			if (clothingData != null && clothingData.MovementSpeedFactor != 1f)
			{
				clothingFactor *= clothingData.MovementSpeedFactor;
				factors?.Add(new Factor
				{
					Value = clothingData.MovementSpeedFactor,
					Description = clothingData.DisplayName
				});
			}
		}
	}
}
