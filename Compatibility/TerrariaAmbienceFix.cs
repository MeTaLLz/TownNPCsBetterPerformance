using Terraria;
using Terraria.ModLoader;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Reflection;

namespace TownNPCsFreeze.Compatibility
{
    public class TerrariaAmbienceFix : ModSystem
    {
        private ILHook _ilHook;

        public override void PostSetupContent()
        {
            if (!ConfigCache.TerrariaAmbienceFix) return;

            if (!ModLoader.TryGetMod("TerrariaAmbience", out Mod mod))
                return;

            var targetType = mod.Code.GetType("TerrariaAmbience.Sounds.SoundFilters.SoundFilterSystem");
            if (targetType == null)
            {
                FileLogger.Warn("TerrariaAmbienceFix", "SoundFilterSystem type not found");
                return;
            }

            var method = targetType.GetMethod("PostUpdateEverything",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
            {
                FileLogger.Warn("TerrariaAmbienceFix", "PostUpdateEverything method not found");
                return;
            }

            try
            {
                _ilHook = new ILHook(method, PostUpdateEverything_IL);
            }
            catch (Exception ex)
            {
                FileLogger.Error("TerrariaAmbienceFix", $"ILHook failed: {ex.Message}");
            }
        }

        public override void Unload()
        {
            _ilHook?.Dispose();
        }

        private void PostUpdateEverything_IL(ILContext il)
        {
            // Inserting a check at the beginning of the method:
            // if (Main.dedServ) return;

            // This prevents the method from running on a dedicated server,
            // where sound processing is not needed.

            var cursor = new ILCursor(il);

            var continueLabel = cursor.DefineLabel();

            cursor.Emit(OpCodes.Ldsfld, typeof(Main).GetField("dedServ", BindingFlags.Public | BindingFlags.Static));
            cursor.Emit(OpCodes.Brfalse, continueLabel); // if false (not server) continue
            cursor.Emit(OpCodes.Ret); // if true (server) return

            cursor.MarkLabel(continueLabel);
        }
    }
}

/*
   The problem is:
   System.NullReferenceException: Object reference not set to an instance of an object.
   at TerrariaAmbience.Sounds.SoundFilters.SoundFilterSystem.PostUpdateEverything() in TerrariaAmbience\Sounds\SoundFilters\SoundFilterSystem.cs:line 123

   On a dedicated server, Main.LocalPlayer and AudioConfig may be null,
   causing an error in PostUpdateEverything and breaking TNBP's stability.
*/