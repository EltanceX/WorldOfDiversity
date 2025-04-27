using System;
using Engine.Media;
using Silk.NET.OpenAL;

namespace Engine.Audio
{
	public class Sound : BaseSound
	{
        public SoundBuffer m_soundBuffer;

		public SoundBuffer SoundBuffer => m_soundBuffer;

		public override void Dispose()
		{
			base.Dispose();
			if (m_soundBuffer != null)
			{
                m_soundBuffer.UseCount-=1;
                m_soundBuffer = null;
			}
		}

		internal void Initialize(SoundBuffer soundBuffer)
		{
			ArgumentNullException.ThrowIfNull(soundBuffer);
			m_soundBuffer = soundBuffer;
            m_soundBuffer.UseCount+=1;
        }

		public Sound(SoundBuffer soundBuffer, float volume = 1f, float pitch = 1f, float pan = 0f, bool isLooped = false, bool disposeOnStop = false)
		{
			ArgumentNullException.ThrowIfNull(soundBuffer);
            if (Mixer.m_isInitialized)
            {
                Mixer.AL.SetSourceProperty((uint)m_source, SourceInteger.Buffer, soundBuffer.m_buffer);
                Mixer.CheckALError();
            }
            Initialize(soundBuffer);
			base.ChannelsCount = soundBuffer.ChannelsCount;
			base.SamplingFrequency = soundBuffer.SamplingFrequency;
			base.Volume = volume;
			base.Pitch = pitch;
			base.Pan = pan;
			base.IsLooped = isLooped;
			base.DisposeOnStop = disposeOnStop;
			Mixer.m_soundsToStopPoll.Add(this);
		}

		public Sound(StreamingSource streamingSource, SoundBuffer soundBuffer, float volume = 1f, float pitch = 1f, float pan = 0f, bool isLooped = false, bool disposeOnStop = false)
		{
			ArgumentNullException.ThrowIfNull(soundBuffer);
            Mixer.AL.SetSourceProperty((uint)m_source, SourceInteger.Buffer, soundBuffer.m_buffer);
			Mixer.CheckALError();
			Initialize(soundBuffer);
			base.ChannelsCount = soundBuffer.ChannelsCount;
			base.SamplingFrequency = soundBuffer.SamplingFrequency;
			base.Volume = volume;
			base.Pitch = pitch;
			base.Pan = pan;
			base.IsLooped = isLooped;
			base.DisposeOnStop = disposeOnStop;
			Mixer.m_soundsToStopPoll.Add(this);
		}
        /// <summary>
        /// 在指定位置播放音频
        /// </summary>
        /// <param name="direction">相对于玩家的相对位置</param>
		internal override unsafe void InternalPlay(Vector3 direction)
		{
            if (m_source != 0)
            {
                uint source = (uint)m_source;
                Mixer.AL.SetSourceProperty(source, SourceVector3.Position, direction.X, direction.Y, direction.Z);
                Mixer.AL.SetSourceProperty(source, SourceBoolean.Looping, m_isLooped);
                Mixer.AL.SourcePlay(source);
            }
            Mixer.CheckALError();
		}

		internal override void InternalPause()
		{
            if (m_source != 0)
            {
                Mixer.AL.SourcePause((uint)m_source);
                Mixer.CheckALError();
            }
        }

		internal override void InternalStop()
		{
            if (m_source != 0)
            {
                Mixer.AL.SourceRewind((uint)m_source);
                Mixer.CheckALError();
            }
        }

		internal override void InternalDispose()
		{
			base.InternalDispose();
			Mixer.m_soundsToStopPoll.Remove(this);
		}
    }
}
