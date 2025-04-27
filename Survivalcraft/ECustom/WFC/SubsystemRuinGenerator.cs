using GameEntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplatesDatabase;

namespace GlassMod
{
    public class SubsystemRuinGenerator : Subsystem
    {
        public override void Save(ValuesDictionary valuesDictionary)
        {
            base.Save(valuesDictionary);
        }
        public override void OnEntityRemoved(Entity entity)
        {
            base.OnEntityRemoved(entity);
            //RemoveWaitingThread();
        }

        public override void Dispose()
        {
            base.Dispose();
            RemoveWaitingThread();
        }

        public override void Load(ValuesDictionary valuesDictionary)
        {
            RuinsGenerator.RequestStop = false;

            base.Load(valuesDictionary);
        }
        public override void Initialize(Project project, ValuesDictionary valuesDictionary)
        {
            base.Initialize(project, valuesDictionary);
        }


        public static void RemoveWaitingThread()
        {
            if (RuinsGenerator.thread == null) return;
            RuinsGenerator.RequestStop = true;
            RuinsGenerator.resetEvent.WaitOne(3000);
            if (RuinsGenerator.thread != null)
            {
                //RuinsGenerator.RequestStop = false;
                //RuinsGenerator.thread.Abort();
                RuinsGenerator.thread = null;
            }
            RuinsGenerator.resetEvent.Reset();
        }
    }
}