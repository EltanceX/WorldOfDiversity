using Engine;
using Game;
using GameEntitySystem;

namespace GlassMod
{
    public class BasicHook
    {
        //是否开启投影矩阵范围限制
        public static bool BaseProjectionMatrixScopeLimit = false;
        public static void OnWidgetLoad(ref Widget widget)
        {
            //支持方向触摸键偏移
            if (widget is MoveRoseWidget) widget = Activator.CreateInstance<TouchMoveRoseWidget>();
        }
        public static void OnNormalMovementJump(ComponentLocomotion locomotion, ref Vector3 velocity)
        {
            if (locomotion.m_componentPlayer == null) return;
            var ex = SurvivalcraftEx.GetPlayerExData(locomotion.m_componentPlayer);
            var hasValue = locomotion.m_componentCreature.ComponentBody.StandingOnValue.HasValue;
            if (hasValue) ex.SuperJumped = 0;
            if (ex == null || !ex.SuperJumpFunctionEnabled || locomotion.JumpOrder <= 0) return;

            if (!hasValue && ++ex.SuperJumped <= ex.SuperJumpTimes) velocity.Y = 4.5f;
        }

        public static void OnAfterInventorySlotWidgetCotr(InventorySlotWidget inventorySlotWidget, List<Widget> widgets)
        {
            //方块图标
            inventorySlotWidget.m_blockIconWidget = new BlockIconWidget
            {
                HorizontalAlignment = WidgetAlignment.Center,
                VerticalAlignment = WidgetAlignment.Center,
                Margin = new Vector2(8f, 2f) //原始值2, 2
            };
            //耐久条
            inventorySlotWidget.m_healthBarWidget = new ValueBarWidget2
            {
                LayoutDirection = LayoutDirection.Horizontal,
                HorizontalAlignment = WidgetAlignment.Near,
                VerticalAlignment = WidgetAlignment.Far,
                BarsCount = 1,
                FlipDirection = true,
                LitBarColor = new Color(32, 128, 0),
                UnlitBarColor = new Color(24, 24, 24, 64),
                BarSize = new Vector2(12f, 12f),
                BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/ProgressBar"),
                Margin = new Vector2(4f, 4f)
            };
            for (int i = 0; i < widgets.Count; i++)
            {
                var widget = widgets[i];
                if (widget is BlockIconWidget) widgets[i] = inventorySlotWidget.m_blockIconWidget;
                if (widget is ValueBarWidget) widgets[i] = inventorySlotWidget.m_healthBarWidget;
            }
        }
        /// <returns>true: block vanilla </returns>
        public static bool OnGUIBackEvent(Entity entity)
        {
            GamePauseMenu settingsScreen = entity.Components.Where(x => x is GamePauseMenu).FirstOrDefault() as GamePauseMenu;
            if (settingsScreen == null) return false;
            settingsScreen.OnBackEvent();
            return true;
        }
    }
}
