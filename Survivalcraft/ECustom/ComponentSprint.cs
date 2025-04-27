using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEntitySystem;
using TemplatesDatabase;
using Engine.Input;
using DebugMod;
using System.Diagnostics;

namespace GlassMod
{
    public class ComponentSprint : Component, IUpdateable
    {
        public ComponentPlayer player;
        public ComponentGui componentGui;
        public float DefaultViewAngle;
        public float DefaultWalkSpeed;

        public double LastWDownTime = 0d;

        public double UpdatingTime = 300d;
        public double UpdateTimeInterval = 250d;
        public double UpdateEndTimeInterval = 300d;
        public float Sensitivity = 300f;
        public double SprintBeginTime = 0d;
        public double SprintEndTime = 0d;
        public bool UpdateFinished = false;
        public bool Sprinting = false;
        public bool SprintEnding = false;

        public float ViewAngleDelta = 0.3f;
        public float WalkSpeedDelta = 1.6f;
        public UpdateOrder UpdateOrder => UpdateOrder.Default;
        public bool AndroidForwardMove()
        {
            if (componentGui == null || EGlobal.Platform != EGlobal.Platforms.Android) return false;
            var TouchMoveRose = componentGui.MoveRoseWidget as TouchMoveRoseWidget;
            if (TouchMoveRose == null) return false;
            return TouchMoveRose.SprintForwardOnce;
        }
        public bool AndroidMoving()
        {
            if (componentGui == null || EGlobal.Platform != EGlobal.Platforms.Android) return false;
            var TouchMoveRose = componentGui.MoveRoseWidget as TouchMoveRoseWidget;
            if (TouchMoveRose == null) return false;
            return TouchMoveRose.MovingForward;
        }
        public void Update(float dt)
        {
            bool WDownOnce = Keyboard.IsKeyDownOnce(Key.W) || AndroidForwardMove();
            if (!Sprinting && WDownOnce)
            {
                double time = util.getTime();

                if (time - LastWDownTime < Sensitivity)
                {
                    BeginSprint();
                    LastWDownTime = time;
                    return;
                }

                LastWDownTime = time;
            }

            if (Sprinting)
            {
                if (WDownOnce)
                {
                    LastWDownTime = util.getTime();
                    EndSprint();
                    return;
                }
                if (Keyboard.IsKeyDown(Key.W) || AndroidMoving()) UpdateSprint();
                else if (SprintEnding) UpdateEndSprint();
                else BeginEndSprint();
            }
        }
        public void BeginSprint()
        {
            double time = util.getTime();
            Sprinting = true;
            SprintBeginTime = time;
        }
        public void BeginEndSprint()
        {
            double time = util.getTime();
            SprintEndTime = time;
            SprintEnding = true;
        }
        public void UpdateSprint()
        {
            if (UpdateFinished) return;
            double time = util.getTime();
            double DeltaTime = time - SprintBeginTime;
            if (DeltaTime > UpdateTimeInterval)
            {
                UpdateFinished = true;
                return;
            }
            float process = (float)Math.Sqrt(DeltaTime / UpdateTimeInterval);
            SettingsManager.ViewAngle = DefaultViewAngle + ViewAngleDelta * process;
            player.ComponentLocomotion.WalkSpeed = DefaultWalkSpeed + WalkSpeedDelta * process;
        }
        public void UpdateEndSprint()
        {
            double time = util.getTime();
            double DeltaTime = time - SprintEndTime;
            if (DeltaTime > UpdateEndTimeInterval)
            {
                EndSprint();
                RecoverToDefault();
                return;
            }
            float process = (float)(1d - Math.Sqrt(DeltaTime / UpdateEndTimeInterval));
            SettingsManager.ViewAngle = DefaultViewAngle + ViewAngleDelta * process;
            player.ComponentLocomotion.WalkSpeed = DefaultWalkSpeed + WalkSpeedDelta * process;
        }
        public void EndSprint()
        {
            SettingsManager.ViewAngle = DefaultViewAngle;
            UpdateFinished = false;
            Sprinting = false;
            SprintEnding = false;
        }
        public void RecoverToDefault()
        {
            SettingsManager.ViewAngle = DefaultViewAngle;
            player.ComponentLocomotion.WalkSpeed = DefaultWalkSpeed;
        }
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            player = Entity.FindComponent<ComponentPlayer>(throwOnError: true);
            componentGui = Entity.FindComponent<ComponentGui>(throwOnError: false);
            DefaultViewAngle = SettingsManager.ViewAngle;
            DefaultWalkSpeed = player.ComponentLocomotion.WalkSpeed;
        }
    }
}
