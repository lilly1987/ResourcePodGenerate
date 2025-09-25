using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Lilly.ResourcePodGenerate
{
    public class ModUI : Mod
    {
        public static ModUI self;
        public static Settings settings;

        public ModUI(ModContentPack content) : base(content)
        {
            self = this;
            MyLog.Message($"ST");

            settings = GetSettings<Settings>();// 주의. MainSettings의 patch가 먼저 실행됨      

            MyLog.Message($"ED");
        }

        Vector2 scrollPosition;
        string tmp;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            var rect = new Rect(0, 0, inRect.width - 16, 1000);

            Widgets.BeginScrollView(inRect, ref scrollPosition, rect);

            Listing_Standard listing = new Listing_Standard();

            listing.Begin(rect);

            listing.GapLine();

            // ---------

            listing.CheckboxLabeled($"Debug", ref Settings.onDebug);

            TextFieldNumeric(listing, ref Settings.resourcePodGenerateMin, "총 시장가치 최소");
            TextFieldNumeric(listing, ref Settings.resourcePodGenerateMax, "총 시장가치 최대");
            TextFieldNumeric(listing, ref Settings.resourcePodGenerateStackMin, "포드별 스택 최소");
            TextFieldNumeric(listing, ref Settings.resourcePodGenerateStackMax, "포드별 스택 최대");
            TextFieldNumeric(listing, ref Settings.resourcePodGeneratePodMax, "포드 갯수 최대");
            // ---------

            listing.GapLine();

            listing.End();

            Widgets.EndScrollView();
        }

        public override string SettingsCategory()
        {
            return "Resource Pod Generate Patch".Translate();
        }

        public void TextFieldNumeric<T>(Listing_Standard listing, ref T num, string label = "", string tipSignal = "") where T : struct
        {
            listing.Label(label.Translate(), tipSignal: tipSignal.Translate());
            tmp = num.ToString();
            listing.TextFieldNumeric<T>(ref num, ref tmp);
        }
    }
}
