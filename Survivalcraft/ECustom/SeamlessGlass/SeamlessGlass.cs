using DebugMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Engine;
using Engine.Graphics;

namespace GlassMod
{
    public class SeamlessGlass : PresetAlphaCubeBlock
    {
        public enum DTFace
        {
            YP = 5,
            YN = 4,
            XP = 3,
            XN = 2,
            ZP = 1,
            ZN = 0,
        }
        public const int Index = 910;

        public SeamlessGlass() : base("Textures/Blocks2")
        {
            DefaultDisplayName = "SeamlessGlass";
            DefaultDescription = "Seamless connected glass";
            DefaultCategory = "Construction";
            CraftingId = "SeamlessGlass";
            DefaultDropContent = Index;
            Behaviors = typeof(SeamlessGlassBehavior).Name;
            this.IsTransparent = true;
            RequiredToolLevel = 0;
            DefaultSoundMaterialName = "Glass";
            this.handTexture = ContentManager.Get<Texture2D>("Textures/SeamlessGlass");
            IsDiggingTransparent = false;
            MaxStacking = 100;
            DigMethod = BlockDigMethod.Quarry;
            DigResilience = 1;
            //LightAttenuation = 100;

        }
        public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
        {
            //return base.CreateDebrisParticleSystem(subsystemTerrain,position,value,strength);
            //int data = Terrain.ExtractData(value);
            //Color fabricColor = SubsystemPalette.GetFabricColor(subsystemTerrain,GetColor(data));

            return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, DestructionDebrisScale, Color.White, DefaultTextureSlot, handTexture);
        }
        public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel, float playerLevel)
        {
            return base.GetAdHocCraftingRecipe(subsystemTerrain, ingredients, heatLevel, playerLevel);
        }
        public override IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
        {
            //return base.GetProceduralCraftingRecipes();
            var craftingRecipe = new CraftingRecipe
            {
                ResultCount = 2,
                //ResultValue = Terrain.MakeBlockValue(182,0,SetColor(0,color)),
                ResultValue = BlocksManager.GetBlockIndex<SeamlessGlass>(),
                RemainsCount = 0,
                //RemainsValue = Terrain.MakeBlockValue(90),
                RequiredHeatLevel = 0f,
                Description = "无缝连接的玻璃"//LanguageControl.Get(GetType().Name,1)
            };
            craftingRecipe.Ingredients[0] = "glass";
            craftingRecipe.Ingredients[1] = "glass";
            craftingRecipe.Ingredients[2] = "glass";
            //craftingRecipe.Ingredients[4] = "paintbucket:" + color.ToString(CultureInfo.InvariantCulture);
            yield return craftingRecipe;
        }
        public static int ExtractBit(int n, int loc)
        {
            //int bit1 = (number >> 0) & 1; // 提取右 1 位 (0b000001, 1)
            //int bit3 = (number >> 2) & 1; // 提取右 3 位 (0b000100, 1)
            return (n >> loc) & 1;
        }
        public static int ExtractBit(int n, DTFace loc)
        {
            return ExtractBit(n, (int)loc);
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            if (value == 910 || value == 16270) return 0;
            int blockValue = Terrain.ExtractData(value);
            //ScreenLog.Info($"{value}: {face} | {blockValue} {blockValue:b}");
            //if(blockValue!=0)Debugger.Break();
            int l = 0;
            int b = 0;
            int r = 0;
            int t = 0;
            //0b0000 左下右上
            switch (face)
            {
                case 0://z+ 
                    l = ExtractBit(blockValue, DTFace.XN) << 3;
                    b = ExtractBit(blockValue, DTFace.YN) << 2;
                    r = ExtractBit(blockValue, DTFace.XP) << 1;
                    t = ExtractBit(blockValue, DTFace.YP) << 0;
                    break;
                case 1://x+
                    l = ExtractBit(blockValue, DTFace.ZP) << 3;
                    b = ExtractBit(blockValue, DTFace.YN) << 2;
                    r = ExtractBit(blockValue, DTFace.ZN) << 1;
                    t = ExtractBit(blockValue, DTFace.YP) << 0;
                    break;
                case 2: //z-
                    l = ExtractBit(blockValue, DTFace.XP) << 3;
                    b = ExtractBit(blockValue, DTFace.YN) << 2;
                    r = ExtractBit(blockValue, DTFace.XN) << 1;
                    t = ExtractBit(blockValue, DTFace.YP) << 0;
                    break;
                case 3://x-
                    l = ExtractBit(blockValue, DTFace.ZN) << 3;
                    b = ExtractBit(blockValue, DTFace.YN) << 2;
                    r = ExtractBit(blockValue, DTFace.ZP) << 1;
                    t = ExtractBit(blockValue, DTFace.YP) << 0;
                    break;
                case 4://y+

                    l = ExtractBit(blockValue, DTFace.XN) << 3;
                    b = ExtractBit(blockValue, DTFace.ZP) << 2;
                    r = ExtractBit(blockValue, DTFace.XP) << 1;
                    t = ExtractBit(blockValue, DTFace.ZN) << 0;
                    break;
                case 5://y- (同y+)
                    l = ExtractBit(blockValue, DTFace.XN) << 3;
                    b = ExtractBit(blockValue, DTFace.ZN) << 2;
                    r = ExtractBit(blockValue, DTFace.XP) << 1;
                    t = ExtractBit(blockValue, DTFace.ZP) << 0;
                    break;
            }
            return l | b | r | t;
        }
    }
    
}
