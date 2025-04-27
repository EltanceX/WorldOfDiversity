using Engine;
using Engine.Audio;
using Engine.Media;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Game.GridPanelWidget;

namespace GlassMod
{
    public class BgMusicData
    {
        public StreamingSource streamingSource;
        public int Seconds = 0;
        public int Weight = 0;
        public string Path;
        public bool Initialized = false;
        public float VolumeAttenuation = 0;
        public int ToSeconds(int minutes, int seconds)
        {
            return minutes * 60 + seconds;
        }
        public BgMusicData(int minutes, int seconds, float volumeAttenuation = 0)
        {
            Seconds = ToSeconds(minutes, seconds);
            VolumeAttenuation = volumeAttenuation;
        }
    }
    public class BackgroundMusicManager
    {
        public static double m_fadeStartTime;
        public static float Volume
        {
            get
            {
                if (m_volume.HasValue) return m_volume.Value;
                return SettingsManager.MusicVolume * 0.6f;
            }
            set
            {
                m_volume = value;
            }
        }
        public static float? m_volume = null;
        public static StreamingSound m_sound;
        public static StreamingSound m_fadeSound;
        public static Dictionary<string, BgMusicData> MusicPaths = new Dictionary<string, BgMusicData>() {
            { "Music/Background/big_big_world", new BgMusicData(3, 27, -0.18f) },//...
            //{ "Music/Background/xu", new BgMusicData(1, 2) }
            { "Music/Background/winter_the_wind_can_be_still", new BgMusicData(2, 50, -0.02f) },//星露谷
            { "Music/Background/summer_tropicala", new BgMusicData(3, 22, -0.12f) },//星露谷
            //{ "Music/Background/spring_the_valley_comes_alive", new BgMusicData(4, 22) },
            //{ "Music/Background/フタリボッチ", new BgMusicData(3, 3) },
            { "Music/Background/only_each_other", new BgMusicData(3, 3, -0.08f) },//少终
            //{ "Music/Background/君と過ごす日々", new BgMusicData(3, 23) },
            { "Music/Background/days_spent_with_you", new BgMusicData(3, 23, -0.06f) },//少终
            //{ "Music/Background/弾ム心", new BgMusicData(2, 50) },
            { "Music/Background/bom_heart", new BgMusicData(2, 50, -0.08f) }, //少终
            { "Music/Background/living_mice", new BgMusicData(2, 57, 0.02f) }, //MC
            { "Music/Background/follow_the_stream", new BgMusicData(4, 0, 0.02f) }, //城市天际线2
        };



        public static void PlayMusic(string name, float startPercentage)
        {
            if (string.IsNullOrEmpty(name))
            {
                StopMusic();
            }
            else
            {
                try
                {
                    StopMusic();
                    m_fadeStartTime = Time.FrameStartTime + 2.0;
                    float volume = (m_fadeSound != null) ? 0f : Volume;
                    StreamingSource streamingSource = ContentManager.Get<StreamingSource>(name);
                    streamingSource = streamingSource.Duplicate();
                    streamingSource.Position = (long)(MathUtils.Saturate(startPercentage) * (streamingSource.BytesCount / streamingSource.ChannelsCount / 2)) / 16 * 16;
                    m_sound = new StreamingSound(streamingSource, volume, 1f, 0f, isLooped: false, disposeOnStop: true, 1f);
                    m_sound.Play();
                }
                catch
                {
                    Log.Warning("Error playing music \"{0}\".", name);
                }
            }
        }
        public static StreamingSound GetMusic(StreamingSource streamingSource, float startPercentage)
        {
            try
            {
                StopMusic();
                m_fadeStartTime = Time.FrameStartTime + 2.0;
                float volume = (m_fadeSound != null) ? 0f : Volume;
                streamingSource = streamingSource.Duplicate();
                streamingSource.Position = (long)(MathUtils.Saturate(startPercentage) * (streamingSource.BytesCount / streamingSource.ChannelsCount / 2)) / 16 * 16;
                m_sound = new StreamingSound(streamingSource, volume, 1f, 0f, isLooped: false, disposeOnStop: true, 1f);
                //m_sound.Play();
                return m_sound;
            }
            catch
            {
                Log.Warning("Error playing music ");
            }
            return null;
        }

        public static void StopMusic()
        {
            if (m_sound != null)
            {
                if (m_fadeSound != null)
                {
                    m_fadeSound.Dispose();
                }
                m_sound.Stop();
                m_fadeSound = m_sound;
                m_sound = null;
            }
        }
    }
}
