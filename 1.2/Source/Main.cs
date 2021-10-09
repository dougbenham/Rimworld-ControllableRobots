using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ControllableRobots
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("doug.ControllableRobots");
            harmony.PatchAll();

            /*
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.FullName.StartsWith("ReverseCommands"))
                {
                    var rcTools = asm.GetType("ReverseCommands.Tools");
                    if (rcTools != null)
                    {
                        var pawnUsableMethod = rcTools.GetMethod("PawnUsable");
                        if (pawnUsableMethod != null)
                        {
                            var prefix = typeof(Tools_PawnUsable_Patch).GetMethod("Hook");
                            harmony.Patch(pawnUsableMethod, prefix: new HarmonyMethod(prefix));
                        }

                        foreach (var nt in rcTools.GetNestedTypes(BindingFlags.NonPublic))
                        {
                            var getPawnActions = nt.GetMethod("<GetPawnActions>b__4_0", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (getPawnActions != null)
                            {
                                var prefix = typeof(Tools_GetPawnActions_Patch).GetMethod("Hook");
                                harmony.Patch(getPawnActions, prefix: new HarmonyMethod(prefix));
                                return;
                            }
                        }
                        Log.Error("GetPawnActions missing");
                    }
                }
            }*/
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
    public static class FloatMenuMakerMap_CanTakeOrder_Patch
    {
        [HarmonyPrefix]
        public static bool Hook(ref bool __result, Pawn pawn)
        {
            if (pawn.def.GetType().Name == "X2_ThingDef_AIRobot")
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddJobGiverWorkOrders_NewTmp")]
    public static class FloatMenuMakerMap_AddJobGiverWorkOrders_NewTmp_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Hook(IEnumerable<CodeInstruction> instructions)
        {
            /*
             * Skip this check:
              			if (pawn.thinker.TryGetMainTreeThinkNode<JobGiver_Work>() == null)
			            {
				            return;
			            }
             */
            var r = false;
            foreach (var i in instructions)
            {
                if (i.opcode == OpCodes.Ret && !r)
                {
                    r = true;
                    continue;
                }
                yield return i;
            }
        }
    }

    //flag = (selectedObjects.FirstOrDefault((object o) => o is Pawn && (o as Pawn).IsColonist) != null);

    /*
    public static class Tools_GetPawnActions_Patch
    {
        public static bool Hook(ref bool __result, object o)
        {
            if (o is Pawn p && p.def.GetType().Name == "X2_ThingDef_AIRobot")
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    public static class Tools_PawnUsable_Patch
    {
        public static bool Hook(ref bool __result, Pawn pawn)
        {
            if (pawn.def.GetType().Name == "X2_ThingDef_AIRobot")
            {
                __result = !pawn.Dead && pawn.Spawned && !pawn.Downed && pawn.Map == Find.CurrentMap;
                return false;
            }

            return true;
        }
    }*/
}
