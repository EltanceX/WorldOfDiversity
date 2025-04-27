using Engine;
using System.Collections.Generic;
using TemplatesDatabase;
using Game;
using Random = Game.Random;
using System.Diagnostics;
using GameEntitySystem;
using Acornima;

namespace GlassMod
{
    public class ConstructionWandBehavior : SubsystemBlockBehavior
    {
        public SubsystemBodies m_subsystemBodies;

        public SubsystemAudio m_subsystemAudio;

        public SubsystemNoise m_subsystemNoise;

        public Random m_random = new();


        public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
        {
            //m_subsystemAudio.PlayRandomSound("Audio/BlockPlaced", 1f, m_random.Float(-0.2f, 0f), ray.Position, 4f, autoDelay: true);
            m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, m_random.Float(-0.2f, 0.2f), ray.Position, 2f, autoDelay: true);

            m_subsystemNoise.MakeNoise(componentMiner.ComponentCreature.ComponentBody, 0.5f, 30f);
            //foreach(Entity entity in Project.Entities)
            //{
            //var ward = entity.Components.Where(x => x is ComponentWand).FirstOrDefault() as ComponentWand;
            var ward = componentMiner.Entity.Components.Where(x => x is ComponentWand).FirstOrDefault() as ComponentWand;
            if (ward == null) return false;
            ward.Use();
            componentMiner.DamageActiveTool(1);
            return true;
            //}
            //return false;
        }


        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            m_subsystemBodies = Project.FindSubsystem<SubsystemBodies>(throwOnError: true);
            m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(throwOnError: true);
            m_subsystemNoise = Project.FindSubsystem<SubsystemNoise>(throwOnError: true);
        }
    }
}
