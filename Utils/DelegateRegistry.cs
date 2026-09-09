using System;
using System.Reflection;
using Terraria;

namespace TownNPCsFreeze
{
    public static class DelegateRegistry
    {
        public static Action<NPC, int, int> TeleportHome { get; private set; }
        public static Func<NPC, int, int, int, int, bool> IsInGoodRestingSpot { get; private set; }
        public static MethodInfo FindGoodRestingSpot { get; private set; }
        public static Action<NPC, int> UpdateNetworkCode { get; private set; }

        public static void Initialize()
        {
            TeleportHome = CreateDelegate<Action<NPC, int, int>>(
                "AI_007_TownEntities_TeleportToHome", typeof(int), typeof(int));

            IsInGoodRestingSpot = CreateDelegate<Func<NPC, int, int, int, int, bool>>(
                "AI_007_TownEntities_IsInAGoodRestingSpot", typeof(int), typeof(int), typeof(int), typeof(int));

            FindGoodRestingSpot = typeof(NPC).GetMethod("AI_007_FindGoodRestingSpot",
                BindingFlags.NonPublic | BindingFlags.Instance, null,
                [typeof(int), typeof(int), typeof(int).MakeByRefType(), typeof(int).MakeByRefType()], null);

            UpdateNetworkCode = CreateDelegate<Action<NPC, int>>(
                "UpdateNetworkCode", typeof(int));
        }

        private static T CreateDelegate<T>(string methodName, params Type[] paramTypes) where T : Delegate
        {
            var method = typeof(NPC).GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance, null, paramTypes, null);
            return (T)Delegate.CreateDelegate(typeof(T), method);
        }
    }
}