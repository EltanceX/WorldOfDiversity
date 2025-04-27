using DebugMod;
using Engine;
using Engine.Audio;
using Engine.Media;
using Game;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplatesDatabase;

namespace GlassMod
{
    public class ComponentBgMusic : Component, IUpdateable
    {
        public UpdateOrder UpdateOrder => UpdateOrder.Default;
        public ManualResetEvent resetEvent = new ManualResetEvent(false);
        public List<BgMusicData> musicDatas = [];
        public bool ToDispose = false;
        public double MusicBeginTime = 0;
        public double WaitBeginTime = 0;
        public int NextIndex = 0;
        public int NextInterval = 10;
        public int IntervalTimeCost = 0;
        public StreamingSound BgMusic;
        public System.Random random = new System.Random();

        float dts = 0;

        public void Update(float dt)
        {
            //return;
            if (BgMusic != null) BgMusic.Volume = 1;
            dts += dt;
            if (dts < 1) return;
            dts = 0;

            if (musicDatas == null || musicDatas.Count == 0)
            {
                return;
            }


            double time = util.getTime() / 1000;
            if (BgMusic == null)
            {
                //Debugger.Break();
                WaitBeginTime = time;
                NextIndex = random.Next(musicDatas.Count);
                NextInterval = random.Next(5, 10);
                BgMusic = BackgroundMusicManager.GetMusic(musicDatas[NextIndex].streamingSource, 0);
            }
            else
            {
                ScreenLog.Info($"{BgMusic.State}, {MusicBeginTime + musicDatas[NextIndex].Seconds - time}");
                switch (BgMusic.State)
                {
                    case SoundState.Disposed:
                    case SoundState.Playing:
                        if (time - 1 > MusicBeginTime + musicDatas[NextIndex].Seconds)
                        {
                            //Debugger.Break();
                            BgMusic.Stop();
                            BgMusic.Dispose();
                            //musicDatas[NextIndex].streamingSource.Dispose();
                            BgMusic = null;
                        }
                        break;
                    case SoundState.Paused:
                        break;
                    case SoundState.Stopped:
                        //Debugger.Break();
                        if (time > WaitBeginTime + NextInterval)
                        {
                            BgMusic.Play();
                            MusicBeginTime = time;
                        }
                        break;
                }
            }
        }
        public void InitMusicData()
        {
            musicDatas.Clear();
            foreach (var info in BackgroundMusicManager.MusicPaths)
            {
                StreamingSource streamingSource = ContentManager.Get<StreamingSource>(info.Key);
                if (streamingSource == null)
                {
                    Log.Warning($"Music {info.Key} Not Found!");
                    continue;
                }
                var data = info.Value;
                musicDatas.Add(data);
                if (data.Initialized) continue;
                data.Path = info.Key;
                data.streamingSource = streamingSource;
                data.Initialized = true;
            }
        }
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            //Debugger.Break();
            InitMusicData();
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
