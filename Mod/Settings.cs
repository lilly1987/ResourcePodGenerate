using Verse;

namespace Lilly.ResourcePodGenerate
{
    public class Settings : ModSettings
    {
        public static bool onDebug = true;

        public static float resourcePodGenerateMin = 10000f;
        public static float resourcePodGenerateMax = 1000000f;
        public static int resourcePodGenerateStackMin = 100;
        public static int resourcePodGenerateStackMax = 10000;
        public static int resourcePodGeneratePodMax = 100;
        public override void ExposeData()
        {
            MyLog.Message($"ST {Scribe.mode}");

            base.ExposeData();
            if (Scribe.mode != LoadSaveMode.Saving && Scribe.mode != LoadSaveMode.LoadingVars)
            {
                MyLog.Message($"ED {Scribe.mode}");
                return;
            }
            Scribe_Values.Look(ref onDebug, "onDebug", false);
            Scribe_Values.Look(ref resourcePodGenerateMin, "resourcePodGenerateMin", resourcePodGenerateMin);
            Scribe_Values.Look(ref resourcePodGenerateMax, "resourcePodGenerateMax", resourcePodGenerateMax);
            Scribe_Values.Look(ref resourcePodGenerateStackMin, "resourcePodGenerateStackMin", resourcePodGenerateStackMin);
            Scribe_Values.Look(ref resourcePodGenerateStackMax, "resourcePodGenerateStackMax", resourcePodGenerateStackMax);
            Scribe_Values.Look(ref resourcePodGeneratePodMax, "resourcePodGeneratePodMax", resourcePodGeneratePodMax);
            ModPatch.Patch(true);

            MyLog.Message($"ED {Scribe.mode}");
        }


    }
}
