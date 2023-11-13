using System.Linq;
using System.Reflection;

using HarmonyLib;

using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics.Actions;

using UnityModManagerNet;

namespace JoyfulRaptureFix
{
    public static class Main
    {
        internal static UnityModManager.ModEntry Mod;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Mod = modEntry;

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        [HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary")]
        public static class LibraryScriptableObject_LoadDictionary_Patch
        {
            private static bool s_loaded = false;

            public static void Postfix()
            {
                if (s_loaded)
                {
                    return;
                }

                s_loaded = true;

                BlueprintAbility joyfulRapture = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>(
                    "15a04c40f84545949abeedef7279751a"
                );

                var dispel = (ContextActionDispelMagic)
                    joyfulRapture
                        .GetComponent<AbilityEffectRunAction>()
                        .Actions.Actions.First(
                            a =>
                                a.name
                                == "$ContextActionDispelBuffs$b4781573-55ad-4e71-9dd9-75a0c38652e0"
                        );

                // The blueprint only sets the descriptor for Petrified, despite the spell description
                // claiming it removes all emotion effects. This sets it instead to everything
                // Unbreakable Heart suppresses (except Confusion, which isn't an emotion effect),
                // and additionally the Negative Emotion descriptor. This is probably redundant,
                // since Fear, Shaken, etc. all seem to be tagged as Negative Emotion effects, but
                // it's suspicious that Unbreakable Heart specifies these directly.
                dispel.Descriptor =
                    SpellDescriptor.Fear
                    | SpellDescriptor.Shaken
                    | SpellDescriptor.Frightened
                    | SpellDescriptor.Petrified
                    | SpellDescriptor.NegativeEmotion;
            }
        }
    }
}
