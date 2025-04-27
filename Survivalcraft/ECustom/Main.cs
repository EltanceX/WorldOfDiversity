// Game.ComponentGameSystem
//using CefSharp;
using AccessLib;
using DebugMod;
using Engine;
using Engine.Media;
using Game;
using GameEntitySystem;
using GMenuMod;
using System.Diagnostics;
using System.Text;
using TemplatesDatabase;
using Subsystem = GameEntitySystem.Subsystem;
namespace GlassMod
{

    public class ModContent
    {
        public static Component InitComponent(Component component, Entity entity, ValuesDictionary item)
        {
            int value2 = 1;
            if (component == null)
            {
                throw new InvalidOperationException($"Type \"{value2}\" cannot be used as a component because it does not inherit from Component class.");
            }
            var componentInit = ModAccessUtil.CreateNestedMethodCaller<Component, object>("Initialize");
            componentInit(component, new object[] { entity, item });
            return component;
        }
        public static void LoadComponent(Entity entity, List<Component> list, ValuesDictionary valuesDictionary)
        {
            Debugger.Break();
            var item = valuesDictionary.Values.First() as ValuesDictionary;
            Console.WriteLine(item);

            //Component ETerminal = new ETerminal();
            //Component screenMenu = new GamePauseMenu();
            //Component sprint = new ComponentSprint();
            //Component wand = new ComponentWand();
            //Component bluePrint = new ComponentBluePrint();
            //Component bgmusic = new ComponentBgMusic();
            List<Type> ComponentsPreset = new List<Type>() {
                typeof(ETerminal),
                typeof(GamePauseMenu),
                typeof(ComponentSprint),
                typeof(ComponentWand),
                typeof(ComponentBluePrint)
            };
            List<Component> components = new List<Component>();
            foreach (var component in ComponentsPreset)
            {
                components.Add(Activator.CreateInstance(component) as Component);
            }

            foreach (var component in components)
            {
                var Load = component.GetType().GetMethod("Load");
                InitComponent(component, entity, item);
                Load?.Invoke(component, new object[] { component.ValuesDictionary, null });
                list.Add(component);
            }

        }
        public static void ManualComponentLoad(Entity entity, List<Component> list, ValuesDictionary valuesDictionary)
        {
            LoadComponent(entity, list, valuesDictionary);
        }
    }
    public class EntryHook
    {
        public static bool UpdateInputSwitch = false;

        //public static bool PreventIngameKeyevent = false;
        public static List<string> IngameKeypreventOccupy = new List<string>();
        public static void PreventIngameKeyevent(string uuid)
        {
            if (IngameKeypreventOccupy.Contains(uuid)) return;
            IngameKeypreventOccupy.Add(uuid);
        }
        public static void CancelPreventIngameKeyevent(string uuid)
        {
            if (IngameKeypreventOccupy.Contains(uuid))
            {
                IngameKeypreventOccupy.Remove(uuid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether to Contine Input</returns>
        public static bool OnUpdateInputFromMouseAndKeyboard(WidgetInput input)
        {
            //键盘不存在
            //if(!KeyboardMenu.Instance.Visible || IngameKeypreventOccupy.Count >= 0){
            if (IngameKeypreventOccupy.Count == 0)
            {
                if (UpdateInputSwitch)
                {
                    UpdateInputSwitch = false;

                    //恢复鼠标
                    input.m_isMouseCursorVisible = false;
                }
                return true;
            }

            //键盘存在
            UpdateInputSwitch = true;
            input.m_isMouseCursorVisible = true;
            return false;
        }

        /// <summary>
        /// 初始化子系统
        /// </summary>
        /// <param name="subsystemType">typeof(Subsystem)</param>
        /// <returns>Subsystem Initialized</returns>
        public static Subsystem CreateSubsystemToInsert(Type subsystemType)
        {
            var subsystem = (Subsystem)Activator.CreateInstance(subsystemType);
            var values = new ValuesDictionary();
            var objType = new DatabaseObjectType(subsystemType.Name, "", "", 1, true, false, 100, true);

            var newList = new List<DatabaseObjectType>();
            //objType.m_allowedNestingParents = new List<DatabaseObjectType>();
            //create
            ModAccessUtil.SetFieldOnceSlow<DatabaseObjectType, List<DatabaseObjectType>>(objType, "m_allowedNestingParents", newList);
            values.DatabaseObject = new DatabaseObject(objType, subsystemType.Name);
            //subsystem.m_valuesDictionary = values;
            ModAccessUtil.SetFieldOnceSlow<Subsystem, ValuesDictionary>(subsystem, "m_valuesDictionary", values);

            return subsystem;
        }
        /// <summary>
        /// 创建并插入子系统
        /// </summary>
        /// <param name="proj">项目</param>
        /// <param name="dictionary">xml字典</param>
        /// <param name="m_subsystems">Project子系统存储</param>
        public static void OnSubsystemAdded(Project proj, Dictionary<string, Subsystem> dictionary, List<Subsystem> m_subsystems)
        {
            List<Type> subsystemTypes = new List<Type> {
                typeof(TVKeyboardBlockBehavior),
                typeof(KonataBlockBehavior),
                typeof(SeamlessGlassBehavior),
                typeof(SubsystemTorcherinoBlockBehavior),
                typeof(SubsystemFlareBlockBehavior),
                typeof(ConstructionWandBehavior),
                typeof(SubsystemBgMusic),
                typeof(BluePrintBlockBehavior),
                typeof(SubsystemRuinGenerator)
            };
            for (int i = 0; i < subsystemTypes.Count; i++)
            {
                Type sType = subsystemTypes[i];
                Subsystem subsystem = CreateSubsystemToInsert(sType);
                //subsystem.m_project = proj;
                ModAccessUtil.SetFieldOnceSlow<Subsystem, Project>(subsystem, "m_project", proj);
                dictionary.Add(sType.Name, subsystem);
                m_subsystems.Add(subsystem);
            }
        }
        public static void OnAfterCreateEntity(Project project, ValuesDictionary valuesDictionary, Entity entity)
        {
            //Debugger.Break();
            var Components = ModAccessUtil.GetFieldOnceSlow<Entity, List<Component>>(entity, "m_components");
            var Player = Components.Find(x => x is ComponentPlayer) as ComponentPlayer;
            if (Player == null) return;
            UpdatePlayers(Player);
            SurvivalcraftEx.BindPlayer(Player);

            var values = ModAccessUtil.GetFieldOnceSlow<Entity, ValuesDictionary>(entity, "m_valuesDictionary");
            ModContent.ManualComponentLoad(entity, Components, values);

        }
        //public static void OnAfterLoadEntities(Project project, EntityDataList entityDataList, Dictionary<Entity, bool> list)
        //{
        //    //Debugger.Break();
        //    var GetComponent = ModAccessUtil.CreateNestedGetter<Entity, List<Component>>("m_components");
        //    var GetValues = ModAccessUtil.CreateNestedGetter<Entity, ValuesDictionary>("m_valuesDictionary");
        //    foreach (Entity entity in list.Keys)
        //    {
        //        List<Component> Components = GetComponent(entity);
        //        ValuesDictionary values = GetValues(entity);
        //        var Player = Components.Find(x => x is ComponentPlayer) as ComponentPlayer;
        //        if (Player == null) continue;
        //        UpdatePlayers(Player);

        //        ModContent.ManualComponentLoad(entity, Components, values);
        //        return;
        //    }
        //}
        public static void OnAfterInternalLoadEntity(Entity entity, ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            //Debugger.Break();
            var GetComponent = ModAccessUtil.CreateNestedGetter<Entity, List<Component>>("m_components");
            var GetValues = ModAccessUtil.CreateNestedGetter<Entity, ValuesDictionary>("m_valuesDictionary");
            //foreach (Entity entity in list.Keys)
            //{
            List<Component> Components = GetComponent(entity);
            ValuesDictionary values = GetValues(entity);
            var Player = Components.Find(x => x is ComponentPlayer) as ComponentPlayer;
            if (Player == null) return;
            UpdatePlayers(Player);
            SurvivalcraftEx.BindPlayer(Player);

            ModContent.ManualComponentLoad(entity, Components, values);

            //return;
            //}
        }
        public static void UpdatePlayers(ComponentPlayer player)
        {
            ScreenLog.player = player;
        }
        //public static Component InitComponent(Component component,Entity entity,ValuesDictionary item)
        //{
        //	int value2 = 1;
        //	if(component == null)
        //	{
        //		throw new InvalidOperationException($"Type \"{value2}\" cannot be used as a component because it does not inherit from Component class.");
        //	}
        //	component.Initialize(entity,item);
        //	return component;
        //	//list.Add(new KeyValuePair<int,Component>(value3,component));
        //}
        //public static void LoadComponent(Entity entity,List<KeyValuePair<int,Component>> list)
        //{
        //	var item = list[0].Value.ValuesDictionary;

        //	Component kbMenu = new KeyboardMenu();
        //	Component ETerminal = new ComponentMainEntrance();
        //	Component screenMenu = new ScreenMenu();

        //	list.Add(new KeyValuePair<int,Component>(0,InitComponent(kbMenu,entity,item)));
        //	list.Add(new KeyValuePair<int,Component>(0,InitComponent(ETerminal,entity,item)));
        //	list.Add(new KeyValuePair<int,Component>(0,InitComponent(screenMenu,entity,item)));

        //	//entity.m_components.Add(InitComponent(kbMenu,entity,item));
        //}
        //public static void LoadComponent(Entity entity,List<KeyValuePair<int,Component>> list,ValuesDictionary valuesDictionary)
        //{
        //	var item = valuesDictionary.Values.First() as ValuesDictionary;

        //	Component ETerminal = new ComponentMainEntrance();

        //	list.Add(new KeyValuePair<int,Component>(0,InitComponent(ETerminal,entity,item)));

        //	//entity.m_components.Add(InitComponent(kbMenu,entity,item));
        //}
        public static void OnClose()
        {
            Debugger.Break();
            EGlobal.WindowClosing = true;
            //Thread.Sleep(5000);


        }
        public static bool FirstInitialize = true;
        public static int RunTimes = 0;
        public static void OnEveryAfterRun()
        {
            if (++RunTimes > 165)
            {
                RunTimes = 0;
                Engine.Window.Title = "生存战争2.4 API1.81 - 多样性世界·神秘冒险";
            }
            //Debugger.Break();
            if (!FirstInitialize) return;
            FirstInitialize = false;

            Engine.Window.Closed += OnClose;

            Log.AddLogSink(new ET_LogSink());
            ScreenLog.Info("LogSink Executed");
            CommandInput.Initialize();

            try
            {
                string o = ContentManager.Get<string>("Fonts2/Pericles", "lst");
                var fontlst = ContentManager.Resources["Fonts2/Pericles.lst"];
                var fontpng = ContentManager.Resources["Fonts2/Pericles.png"];
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(o));
                LabelWidget.BitmapFont = BitmapFont2.Initialize(fontpng.ContentStream, ms);
                LabelWidget.BitmapFont.Scale = 0.8f;
                ScreenLog.Info("字体[2.2 MC像素字体]已加载");
            }
            catch (Exception ex)
            {
                ScreenLog.Info(ex);
                ScreenLog.Info("字体[2.2 MC像素字体]加载失败，使用默认字体.");

            }
            //			BlocksManager.LoadBlocksData(@"Class Name;DefaultDisplayName;DefaultCategory;Behaviors;DisplayOrder;DefaultIconBlockOffset;DefaultIconViewOffset;DefaultIconViewScale;FirstPersonScale;FirstPersonOffset;FirstPersonRotation;InHandScale;InHandOffset;InHandRotation;CraftingId;DefaultCreativeData;IsCollidable;IsPlaceable;IsDiggingTransparent;IsPlacementTransparent;DefaultIsInteractive;IsEditable;IsNonDuplicable;IsGatherable;HasCollisionBehavior;KillsWhenStuck;IsFluidBlocker;IsTransparent;DefaultShadowStrength;LightAttenuation;DefaultEmittedLightAmount;ObjectShadowStrength;DefaultDropContent;DefaultDropCount;DefaultExperienceCount;RequiredToolLevel;MaxStacking;SleepSuitability;FrictionFactor;Density;NoAutoJump;NoSmoothRise;FuelHeatLevel;FuelFireDuration;DefaultSoundMaterialName;ShovelPower;QuarryPower;HackPower;DefaultMeleePower;DefaultMeleeHitProbability;DefaultProjectilePower;ToolLevel;PlayerLevelRequired;Durability;IsAimable;IsStickable;AlignToVelocity;ProjectileSpeed;ProjectileDamping;ProjectileTipOffset;DisintegratesOnHit;ProjectileStickProbability;DefaultHeat;FireDuration;ExplosionResilience;DefaultExplosionPressure;DefaultExplosionIncendiary;IsExplosionTransparent;DigMethod;DigResilience;ProjectileResilience;DefaultNutritionalValue;DefaultSicknessProbability;FoodType;DefaultRotPeriod;DefaultTextureSlot;DestructionDebrisScale;DefaultDescription
            //FirearmsTableBlock;枪械台;Items;;1;0,0,0;1, 1, 1;1;0.4;0.5, -0.5, -0.6;0, 40, 0;0.5;0,0.1,-0.26;0, 45, 0;firearmstable;0;TRUE;TRUE;FALSE;FALSE;TRUE;FALSE;FALSE;FALSE;FALSE;FALSE;FALSE;TRUE;12;0;0;1;#;1;;0;1;0.5;1;1.5;FALSE;FALSE;0;0;Metal;1;1;1;1;;1;1;;-1;FALSE;FALSE;FALSE;0;;0;FALSE;0;;20;5;;;FALSE;Quarry;5;1;;;;;180;1;枪械台用于合成枪械mod武器");
        }
        //public static void OnProjectDisposed()
        //{
        //    //base.OnProjectDisposed();
        //    //ScreenLog.Info("正在停止浏览器...");
        //    EntryHook.IngameKeypreventOccupy.Clear();

        //}
        //public static void OnProjectLoaded()
        //{
        //    //base.OnProjectLoaded(project);
        //    //ScreenLog.Info("Allowing Webview...");
        //}
    }
}

