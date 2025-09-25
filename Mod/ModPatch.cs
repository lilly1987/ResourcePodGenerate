using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Lilly.ResourcePodGenerate
{
    // Mod 로딩후에 작동
    [StaticConstructorOnStartup]
    public class ModPatch
    {
        public static string harmonyId = "Lilly.ResourcePodGeneratePatch";
        public static Harmony harmony=null;
        public static IEnumerable<Type> nestedPatchTypes;

        static ModPatch()
        {
            MyLog.Message($"ST");

            // 서브 클래스 중 HarmonyPatch가 붙은 것만 필터링
            nestedPatchTypes = typeof(ModPatch).GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(t => t.GetCustomAttributes(typeof(HarmonyPatch), false).Any());

            Patch();

            MyLog.Message($"ED");
        }

        public static void Patch(bool repatch=false)
        {
            if (repatch)
            {
                Unpatch();
            }
            if (harmony != null) return;
            harmony = new Harmony(harmonyId);
            foreach (var patchType in nestedPatchTypes)
            {
                try
                {
                    //harmony.PatchAll();
                    harmony.CreateClassProcessor(patchType).Patch();
                    MyLog.Message($"{patchType.Name} Patch <color=#00FF00FF>Succ</color>");
                }
                catch (System.Exception e)
                {
                    MyLog.Error($"Patch Fail");
                    MyLog.Error(e.ToString());
                    MyLog.Error($"Patch Fail");
                }
            }
        }

        public static void Unpatch()
        {
            MyLog.Message($"{harmonyId}/UnPatch");
            if (harmony == null) return;
            harmony.UnpatchAll(harmonyId);
            harmony = null;
        }


        [HarmonyPatch(typeof(ThingSetMaker_ResourcePod), nameof(ThingSetMaker_ResourcePod.Generate), new Type[] { typeof(ThingSetMakerParams), typeof(List<Thing>) })]// 됨
        public static class Patch_ThingSetMaker_ResourcePod_Generate
        { 
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
            {
                MyLog.Message($"Transpile", Settings.onDebug);
                var codes = new List<CodeInstruction>(instructions);
                var newCodes = new List<CodeInstruction>();

                for (int i = 0; i < codes.Count; i++)
                {
                    var instruction = codes[i];
                    try
                    {
                        // 총 시장가치 합계
                        if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 150f)
                        {
                            MyLog.Message($"succ1", Settings.onDebug);
                            //instruction.operand = 10000f;
                            instruction.operand = Settings.resourcePodGenerateMin;
                        }
                        else if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 600f)
                        {
                            MyLog.Message($"succ2", Settings.onDebug);
                            instruction.operand = Settings.resourcePodGenerateMax;
                        }

                        // 포드별 갯수 범위
                        else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte sb && sb == 20) // 
                        {
                            MyLog.Message($"succ3", Settings.onDebug);
                            instruction.opcode = OpCodes.Ldc_I4;
                            instruction.operand = Settings.resourcePodGenerateStackMin;
                        }
                        else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte sb2 && sb2 == 40)
                        {
                            MyLog.Message($"succ4", Settings.onDebug);
                            instruction.opcode = OpCodes.Ldc_I4;
                            instruction.operand = Settings.resourcePodGenerateStackMax;
                        }

                        // 포드 최대 갯수
                        else if (instruction.opcode == OpCodes.Ldc_I4_7)
                        {
                            MyLog.Message($"succ5", Settings.onDebug);
                            instruction.opcode = OpCodes.Ldc_I4;
                            instruction.operand = Settings.resourcePodGeneratePodMax;
                        }
                    }
                    catch (Exception e)
                    {
                        MyLog.Error($"Transpile Fail");
                        MyLog.Error(e.ToString());
                        MyLog.Error($"Transpile Fail");
                    }

                    newCodes.Add(instruction);
                    //MyLog.Warning($"{instruction.opcode} : {instruction.operand}");

                    // 종류 다양화
                    if (instruction.opcode == OpCodes.Stloc_3)
                    {
                         MyLog.Message($" succ6", Settings.onDebug);
                        // thingDef = ThingSetMaker_ResourcePod.RandomPodContentsDef(false);
                        var methodInfo = AccessTools.Method(typeof(ThingSetMaker_ResourcePod), "RandomPodContentsDef");
                        newCodes.Add(new CodeInstruction(OpCodes.Ldc_I4_0)); // false
                        newCodes.Add(new CodeInstruction(OpCodes.Call, methodInfo)); // 메서드 호출
                        newCodes.Add(new CodeInstruction(OpCodes.Stloc_0)); // thingDef 저장 (로컬 변수 0번이라고 가정)
                    }
                }

                return newCodes;
            }
        }
    }
}
