using DebugMod;
using Game;
using NAudio.SoundFont;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Net;
using static Game.SubsystemPathfinding;
using Random = System.Random;

namespace GlassMod
{
    public class RuinsGenerator
    {
        public static Thread thread = null;
        public static List<(TerrainChunk, TerrainContentsGenerator24)> WaitingForGenerate = new();
        public static bool RequestStop = false;
        public static ManualResetEvent resetEvent = new ManualResetEvent(false);
        //public static bool GenerateEnabled = true;
        public static bool ShoudGenerate(int seed, TerrainChunk chunk)
        {
            Random random = new Random(seed);

            // 使用区块坐标和种子生成随机数
            int xSeed = (random.Next() >> 2) + 1;
            int zSeed = (random.Next() >> 2) + 1;
            int newSeed = ((xSeed * chunk.Coords.X + zSeed * chunk.Coords.Y) ^ seed);
            // 返回true表示在此区块生成遗迹
            int next = new Random(newSeed).Next(1000);
            return next > 500 && next < 503; // 2%的几率
        }
        public static void LoadNotLoaded(SubsystemTerrain subsystemTerrain, TerrainChunk chunk)
        {
            //double realTime19 = Time.RealTime;
            if (subsystemTerrain.TerrainSerializer.LoadChunk(chunk))
            {
                chunk.ThreadState = TerrainChunkState.InvalidLight;
                chunk.WasUpgraded = true;
                //double realTime20 = Time.RealTime;
                chunk.IsLoaded = true;
                //m_statistics.LoadingCount++;
                //m_statistics.LoadingTime += realTime20 - realTime19;
            }
            else
            {
                chunk.ThreadState = TerrainChunkState.InvalidContents1;
                chunk.WasUpgraded = true;
            }
        }
        public static void WaitingThread()
        {
            while (true)
            {
                if (RequestStop)
                {
                    Debugger.Break();
                    break;
                }
                if (WaitingForGenerate.Count == 0) break;
                for (int i = 0; i < WaitingForGenerate.Count; i++)
                {
                    bool AllCompleted = true;
                    (TerrainChunk chunk, TerrainContentsGenerator24 generator24) = WaitingForGenerate[i];
                    var subsystemTerrain = generator24.m_subsystemTerrain;
                    var terrain = subsystemTerrain.Terrain;
                    var chunkPos = chunk.Coords;
                    for (int x = 0; x < 7; x++)
                    {
                        for (int z = 0; z < 7; z++)
                        {
                            var chunkat = terrain.GetChunkAtCoords(chunkPos.X + x, chunkPos.Y + z);
                            if (chunkat == null)
                            {
                                chunkat = terrain.AllocateChunk(chunkPos.X + x, chunkPos.Y + z);
                            }
                            if (chunkat.State == TerrainChunkState.NotLoaded)
                            {
                                AllCompleted = false;
                                //LoadNotLoaded(subsystemTerrain, chunkat);
                            }
                            if (chunk.State < TerrainChunkState.Valid) AllCompleted = false;
                        }
                    }
                    if (AllCompleted)
                    {
                        //Debugger.Break();
                        Debugger.Break();
                        WaitingForGenerate.RemoveAt(i);
                        if (EGlobal.GenerateEnabled) Generate(chunk, generator24);
                    }
                }


                Thread.Sleep(200);
            }
            WaitingForGenerate.Clear();
            thread = null;
            resetEvent.Set();
        }
        public static void OnChunkGenerate(TerrainChunk chunk, TerrainContentsGenerator24 generator24)
        {
            var seed = generator24.m_seed;
            var shouldGenerate = ShoudGenerate(seed, chunk);
            //ScreenLog.Info($"{shouldGenerate}, [{chunk.Coords.X}, {chunk.Coords.Y}] [{chunk.Center.X}, {chunk.Center.Y}]");

            if (!shouldGenerate) return;

            var chunkpos = chunk.Coords;
            foreach (var item in WaitingForGenerate)
            {
                if (item.Item1.Coords == chunkpos) return;
            }

            WaitingForGenerate.Add((chunk, generator24));
            if (thread == null || thread.ThreadState == System.Threading.ThreadState.Stopped)
            {
                thread = new Thread(WaitingThread);
                thread.Start();
            }


        }
        public static void Generate(TerrainChunk chunk, TerrainContentsGenerator24 generator24)
        {

            var entities = generator24.m_subsystemTerrain.Project.Entities;
            ComponentPlayer player = null;
            foreach (var entity in entities)
            {
                ComponentPlayer find = entity.Components.Where(x => x is ComponentPlayer).FirstOrDefault() as ComponentPlayer;
                if (find != null)
                {
                    player = find;
                    break;
                }
            }
            if (player == null) return;
            var bpos = new BlockPos(chunk.Center.X, 60.0f, chunk.Center.Y);
            var top = chunk.Terrain.GetTopHeight(bpos.x, bpos.z);
            var top2 = chunk.Terrain.CalculateTopmostCellHeight(bpos.x, bpos.z);
            //ScreenLog.Info($"Player Found, Height: {top}, {top2}");
            WFCGameBridge.Generate(player, new BlockPos(chunk.Center.X, top2, chunk.Center.Y));
        }
    }
}
