

#if MonoMod
using Game;
using GameEntitySystem;
using GlassMod;
using HarmonyLib;
using System.Diagnostics;
using System.Reflection;
using TemplatesDatabase;

namespace GMLForAPI
{
    public class PatchingEntrance
    {
        public static Harmony harmony = new Harmony("com.glass.mainpatch");
        public static Assembly[] LoadedAssemblies;
        public static Dictionary<string, Assembly> Assemblies = new Dictionary<string, Assembly>();
        public static void UpdateAssemblies()
        {
            Assemblies.Clear();
            LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in LoadedAssemblies)
            {
                var name = assembly.FullName;
                Assemblies.Add(assembly.ManifestModule.Name.ToLower(), assembly);
            }
        }
        public static void Entrance()
        {
            Debugger.Break();
            UpdateAssemblies();

            Assembly Survivalcraft = Assemblies["survivalcraft.dll"];
            Assembly Engine = Assemblies["engine.dll"];
            Assembly? GameEntitySystem = null;
            try
            {
                GameEntitySystem = Assemblies["gameentitysystem.dll"];
            }
            catch (Exception ex) { }
            var a = Survivalcraft.GetTypes();
            Type Entity = Survivalcraft.GetType("GameEntitySystem.Entity") ?? GameEntitySystem.GetType("GameEntitySystem.Entity");
            Type Project = Survivalcraft.GetType("GameEntitySystem.Project") ?? GameEntitySystem.GetType("GameEntitySystem.Project");
            Type Program = Survivalcraft.GetType("Game.Program");

            Type GComponentInput = Survivalcraft.GetType("Game.ComponentInput");
            MethodInfo UpdateInputFromMouseAndKeyboard = GUtil.GetMethodByName(GComponentInput, "UpdateInputFromMouseAndKeyboard");

            MethodInfo CreateEntity = GUtil.GetMethodByName(Project, "CreateEntity");
            MethodInfo Run = GUtil.GetMethodByName(Program, "Run");
            MethodInfo InternalLoadEntity = GUtil.GetNonPublicMethod(Entity, "InternalLoadEntity");
            var aa = typeof(CustomPatch).GetMethod(nameof(CustomPatch.OnEveryAfterRun));
            harmony.Patch(Run, postfix: typeof(CustomPatch).GetMethod(nameof(CustomPatch.OnEveryAfterRun)));
            harmony.Patch(UpdateInputFromMouseAndKeyboard, prefix: typeof(CustomPatch).GetMethod(nameof(CustomPatch.OnUpdateInputFromMouseAndKeyboard)));
            harmony.Patch(CreateEntity, postfix: typeof(CustomPatch).GetMethod(nameof(CustomPatch.OnAfterCreateEntity)));
            harmony.Patch(InternalLoadEntity, postfix: typeof(CustomPatch).GetMethod(nameof(CustomPatch.OnAfterInternalLoadEntity)));
            Debugger.Break();
        }

    }



    public class CustomPatch
    {
        public static void OnEveryAfterRun()
        {
            EntryHook.OnEveryAfterRun();
            //Debugger.Break();
        }
        public static bool OnUpdateInputFromMouseAndKeyboard(WidgetInput input)
        {
            return EntryHook.OnUpdateInputFromMouseAndKeyboard(input); // 返回 false 阻止原方法执行
        }
        public static void OnAfterCreateEntity(ValuesDictionary valuesDictionary, Project __instance, Entity __result)
        {
            EntryHook.OnAfterCreateEntity(__instance, valuesDictionary, __result);
        }
        public static void OnAfterInternalLoadEntity(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap, Entity __instance)
        {
            EntryHook.OnAfterInternalLoadEntity(__instance, valuesDictionary, idToEntityMap);
        }
    }
}
#else

using DebugMod;

namespace GMLForAPI
{
    public class PatchingEntrance
    {
        public static void Entrance()
        {
            ScreenLog.Info("MonoMod Patching Disabled. Using Inner Hooks...");
        }
    }
}

#endif