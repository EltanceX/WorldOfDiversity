using Engine;
using Engine.Graphics;
using Game;
using GameEntitySystem;
using System.Diagnostics;
using AccessLib;

namespace DebugMod
{
    public class ET_F3
    {
        public bool enabled = false;
        public LabelWidget label;
        public ComponentPlayer m_componentPlayer;
        public ComponentHealth m_componentHealth;
        public ComponentBody m_componentBody;
        public ComponentInventory m_componentInventory;
        public ComponentCreativeInventory m_componentCreativeInventory;
        public Entity entity;
        public TickTimer tickTimer;
        public Func<ComponentInventory, object[], int> call_SlotValueGetter;
        public Func<ComponentInventory, int> get_ActiveSlotIndexField;
        public Func<ComponentCreativeInventory, object[], int> call_CreSlotValueGetter;
        public Func<ComponentCreativeInventory, int> get_CreActiveSlotIndex;
        public Func<object, long> get_TotalMemoryUsed;
        public Func<object, float> get_AverageCpuFrameTime_Value;
        public Func<object, float> get_AverageFrameTime_Value;
        public Func<ComponentBody, SubsystemTime> get_msubsystemTime;
        public Func<ComponentPlayer, SubsystemGameInfo> get_SubsystemGameInfo;
        public Func<ComponentPlayer, SubsystemTerrain> get_SubsystemTerrain;
        public Func<ComponentPlayer, List<Pickable>> get_Pickables;
        public Func<Component, Dictionary<Entity, bool>> get_ProjEntities;
        public ET_F3(Entity entity)
        {
            #region
            Debugger.Break();
			this.entity = entity;
            tickTimer = new TickTimer(50);
            m_componentPlayer = entity.FindComponent<ComponentPlayer>(throwOnError: true);
            m_componentHealth = entity.FindComponent<ComponentHealth>(throwOnError: true);
            m_componentBody = entity.FindComponent<ComponentBody>(throwOnError: true);
            m_componentInventory = m_componentPlayer.Entity.FindComponent<ComponentInventory>();
            m_componentCreativeInventory = m_componentPlayer.Entity.FindComponent<ComponentCreativeInventory>();



            label = new LabelWidget()
            {
                FontScale = 0.7f,
                Color = Color.White,
                Margin = new Vector2(4f, 0f),
                VerticalAlignment = WidgetAlignment.Stretch,
                Text = "F3 Label"
            };
            m_componentPlayer.GuiWidget.Children.Add(label);

            #endregion
            call_SlotValueGetter = ModAccessUtil.CreateNestedMethodCaller<ComponentInventory, int>("GetSlotValue");
            get_ActiveSlotIndexField = ModAccessUtil.CreateNestedGetter<ComponentInventory, int>("m_activeSlotIndex");
            call_CreSlotValueGetter = ModAccessUtil.CreateNestedMethodCaller<ComponentCreativeInventory, int>("GetSlotValue");
            get_CreActiveSlotIndex = ModAccessUtil.CreateNestedGetter<ComponentCreativeInventory, int>("m_activeSlotIndex");
            get_TotalMemoryUsed = ModAccessUtil.CreateNestedGetter<long>(typeof(PerformanceManager), "m_totalMemoryUsed");
            get_AverageCpuFrameTime_Value = ModAccessUtil.CreateNestedGetter<float>(typeof(PerformanceManager), "m_averageCpuFrameTime", "m_value");
            get_AverageFrameTime_Value = ModAccessUtil.CreateNestedGetter<float>(typeof(PerformanceManager), "m_averageFrameTime", "m_value");
            get_msubsystemTime = ModAccessUtil.CreateNestedGetter<ComponentBody, SubsystemTime>("m_subsystemTime");
            get_SubsystemGameInfo = ModAccessUtil.CreateNestedGetter<ComponentPlayer, SubsystemGameInfo>("PlayerData", "m_subsystemGameInfo");
            get_SubsystemTerrain = ModAccessUtil.CreateNestedGetter<ComponentPlayer, SubsystemTerrain>("m_subsystemTerrain");
            get_Pickables = ModAccessUtil.CreateNestedGetter<ComponentPlayer, List<Pickable>>("m_subsystemPickables", "m_pickables");
            get_ProjEntities = ModAccessUtil.CreateNestedGetter<Component, Dictionary<Entity, bool>>("m_entity", "m_project", "m_entities");
        }
        public void Update()
        {
            if (enabled && tickTimer.Next())
            {
                var position = m_componentPlayer.ComponentBody.Position;
                //var itemId = m_componentInventory.GetSlotValue(m_componentInventory.m_activeSlotIndex);
                var slotIndex = get_ActiveSlotIndexField(m_componentInventory);
                var itemId = call_SlotValueGetter(m_componentInventory, [slotIndex]);

                //var citemId = m_componentCreativeInventory.GetSlotValue(m_componentCreativeInventory.m_activeSlotIndex);
                var cSlotIndex = get_CreActiveSlotIndex(m_componentCreativeInventory);
                var citemId = call_CreSlotValueGetter(m_componentCreativeInventory, [cSlotIndex]);

                var rotate = m_componentBody.Rotation;
                var f_vec = m_componentPlayer.ComponentCreatureModel.EyeRotation.GetForwardVector();
                //var cpu_usage = Game.PerformanceManager.m_averageCpuFrameTime.m_value / Game.PerformanceManager.m_averageFrameTime.m_value;
                var cpu_usage = get_AverageCpuFrameTime_Value(typeof(PerformanceManager)) / get_AverageFrameTime_Value(typeof(PerformanceManager));
                var health = m_componentHealth.Health;

                //var ps = new PerformanceStatistic();
                string text = "";
                text += $"Key: [F1]Log [F3]Info [/]Cmd\n";
                text += $"FPS: {1f / PerformanceManager.AverageFrameTime:0.0}\n";
                text += $"Location: {Math.Floor(position.X)}, {Math.Floor(position.Y)}, {Math.Floor(position.Z)}";
                text += "\n";
                //text += "RAM Total: " + Game.PerformanceManager.m_totalMemoryUsed / 1024 / 1024 + " MB";
                text += "RAM Total: " + get_TotalMemoryUsed(typeof(PerformanceManager)) / 1024 / 1024 + " MB";
                text += "\n";
                text += "GPU MEM  : " + Display.GetGpuMemoryUsage() / 1024 / 1024 + " MB";
                //text += "\n";
                //text += "GPU Total RAM: " + Game.PerformanceManager.m_totalGpuMemoryUsed / 1024 / 1024 + " MB";
                text += "\n";
                text += $"CPU: {util.Progress(cpu_usage, 20)} {Math.Round(cpu_usage, 4) * 100f} %";
                text += "\n";
                text += "System Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                text += "\n";
                //text += "Game Time: " + m_componentBody.m_subsystemTime.GameTime + " s";
                text += "Game Time: " + Math.Round(get_msubsystemTime(m_componentBody).GameTime, 3) + " s";
                text += "\n";
                text += "\n";
                //text += "Game Mode: " + m_componentPlayer.PlayerData.m_subsystemGameInfo.WorldSettings.GameMode;
                text += "Game Mode: " + get_SubsystemGameInfo(m_componentPlayer).WorldSettings.GameMode;
                text += "\n";
                text += $"Health: {util.Progress(health > 1 ? 1 : health, 20)} {Math.Round(health, 2)}";
                text += "\n";
                //text += "Humidity: " + m_componentPlayer.m_subsystemTerrain.Terrain.GetHumidity((int)position.X, (int)position.Z);
                text += "Humidity: " + get_SubsystemTerrain(m_componentPlayer).Terrain.GetHumidity((int)position.X, (int)position.Z);
                text += "\n";
                //text += "Temperature: " + m_componentPlayer.m_subsystemTerrain.Terrain.GetTemperature((int)position.X, (int)position.Z);
                text += "Temperature: " + get_SubsystemTerrain(m_componentPlayer).Terrain.GetTemperature((int)position.X, (int)position.Z);
                text += "\n";
                //text += "Seed    : " + m_componentPlayer.PlayerData.m_subsystemGameInfo.WorldSeed;
                text += "Seed    : " + get_SubsystemGameInfo(m_componentPlayer).WorldSeed;
                text += "\n";
                //text += "Items   : " + m_componentPlayer.m_subsystemPickables.m_pickables.Count;
                text += "Items   : " + get_Pickables(m_componentPlayer).Count;
                text += "\n";
                //text += "Entities: " + m_componentPlayer.Project.m_entities.Count;
                text += "Entities: " + entity.Project.Entities.Count;
                text += "\n";
                text += $"Position: {position.X}, {position.Y}, {position.Z}";
                text += "\n";
                text += $"Rotation: {rotate.X}, {rotate.Y}, {rotate.Z}, {rotate.W}";
                text += "\n";
                text += $"Facing  : {f_vec.X}, {f_vec.Y}, {f_vec.Z}";
                text += "\n";
                //text += $"Slot Index/Value [Sur]: {m_componentInventory.m_activeSlotIndex} / {itemId}";
                text += $"Slot Index/Value [Sur]: {slotIndex} / {itemId}";
                text += "\n";
                //text += $"Slot Index/Value [Cre]: {m_componentCreativeInventory.m_activeSlotIndex} / {citemId}";
                text += $"Slot Index/Value [Cre]: {cSlotIndex} / {citemId}";
                text += "\n";
                text += $"V {EGlobal.Version} [{EGlobal.Date}]";
                text += "\n";
                text += $"World of Diversity - Mysterious Adventures";

                label.Text = text;
                //ScreenLog.Info($"Field Modify Cost: {ps.end()} ms");
            }
        }
        public void Dispose()
        {
            m_componentPlayer.GuiWidget.Children.Remove(label);

        }

        public void SwitchState()
        {
            if (enabled) Disable();
            else Enable();
        }
        public void Enable()
        {
            this.enabled = true;
            label.IsVisible = true;
        }
        public void Disable()
        {
            this.enabled = false;
            label.IsVisible = false;
        }
    }
}
