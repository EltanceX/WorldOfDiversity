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
using System.Threading;
using System.Threading.Tasks;
using TemplatesDatabase;

namespace GlassMod
{
    public class SubsystemBgMusic : Subsystem
    {
        public ManualResetEvent resetEvent = new ManualResetEvent(false);
        public CancellationTokenSource cancellationTokenSource;
        public Task task;
        public bool ToDispose = false;

        public List<BgMusicData> musicDatas = [];
        //public float CurrentAttenuation = 0;
        public double MusicBeginTime = 0;
        public double WaitBeginTime = 0;
        public int NextIndex = 0;
        public int NextInterval = 10;
        public int IntervalTimeCost = 0;
        public StreamingSound BgMusic;
        public System.Random random = new System.Random();
        public float BaseVolume = 0.24f;
        public float SettingVolume = 1f;

        public void InitMusicData()
        {
            musicDatas.Clear();
            foreach (var info in BackgroundMusicManager.MusicPaths)
            {
                StreamingSource streamingSource = ContentManager.Get<StreamingSource>(info.Key, throwOnNotFound: false);
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
        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);

            ToDispose = false;
            resetEvent.Reset();
            InitMusicData();
            if (musicDatas.Count == 0) return;
            cancellationTokenSource = new CancellationTokenSource();
            task = new Task(ThreadTask, cancellationTokenSource.Token);
            task.Start();
        }
        public void ThreadTask()
        {

            while (!ToDispose)
            {

                if (BgMusic != null) BgMusic.Volume = (BaseVolume + musicDatas[NextIndex].VolumeAttenuation) * SettingVolume;
                if (musicDatas == null || musicDatas.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                double time = util.getTime() / 1000;
                if (BgMusic == null)
                {
                    //Debugger.Break();
                    WaitBeginTime = time;
                    NextIndex = random.Next(musicDatas.Count);
                    //NextIndex = 4;
                    NextInterval = random.Next(45, 160);
                    BgMusic = BackgroundMusicManager.GetMusic(musicDatas[NextIndex].streamingSource, 0);

                    continue;
                }
                //ScreenLog.Info($"{BgMusic.State}, {MusicBeginTime + musicDatas[NextIndex].Seconds - time}");
                switch (BgMusic.State)
                {
                    case SoundState.Disposed:
                    case SoundState.Playing:
                        //音乐时间终止
                        if (time - 1 > MusicBeginTime + musicDatas[NextIndex].Seconds)
                        {
                            //Debugger.Break();
                            BgMusic.Stop();
                            BgMusic.Dispose();
                            BgMusic = null;
                        }
                        break;
                    case SoundState.Paused:
                        break;
                    case SoundState.Stopped:
                        //创建音乐但未播放
                        if (time > WaitBeginTime + NextInterval)
                        {
                            BgMusic.Play();
                            MusicBeginTime = time;
                        }
                        break;
                }




                Thread.Sleep(100);
            }

            //Debugger.Break();
            if (BgMusic != null)
            {
                BgMusic.Stop();
                BgMusic.Dispose();
                BgMusic = null;
            }
            ToDispose = false;
            resetEvent.Set();
            return;
        }
        public override async void Dispose()
        {
            base.Dispose();
            ToDispose = true;
            resetEvent.WaitOne(500);
            cancellationTokenSource.Cancel();

            Debugger.Break();
            await task;
            task?.Dispose();
            resetEvent.Reset();
            //ToDispose = false;
        }
    }
}
