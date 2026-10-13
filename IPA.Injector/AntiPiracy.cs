using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
 
// IF ANYONE REMOVES THIS CODE OR BYPASSES IT I AM VIOLATING YOU -doge.
namespace IPA.Injector
{
    internal static class AntiPiracy
    {
        private const uint STEAM_APP_ID = 620980;
        private const ulong META_APP_ID = 0;        // 0 to skip oculus check for now cuz i am NOT doing SHIT for meta's dumb sdk -doge

        public static bool IsInvalid(string path)
        {
            // shit ass code ngl -karma
            // I KNOW -doge
            Console.WriteLine("[DEBUG] Checking game ownership..."); 
            // who tf put this here -doge
            // me -karma
            // fuck were you on gng -doge
            
            // steam check first cause valve made it SO FUCKIN EASY THANK YOU VALVE -doge
            if (IsLaunchedBySteam())
            {
                return !IsOwnedOnSteam();
            }

            // then meta/oculus which does ABSOLUTLY NOTHING CAUSE META IS RETARDED -doge
            if (IsLaunchedByMeta())
            {
                return !IsOwnedOnMeta();
            }
            // doge is retarded -karma
            // i will shit on you karma -doge
            // TODO: maybe show a message box yelling at the idiot for pirating -doge
            return true;
        }

        private static bool IsLaunchedBySteam()
        {
            // use steam vars maybe -karma
            bool hasSteamEnv = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SteamAppId"))
                || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SteamGameId"));
                
            bool processHasSteam = Process.GetCurrentProcess().ProcessName.Contains("steam", StringComparison.OrdinalIgnoreCase);

            return hasSteamEnv || processHasSteam;
        }

        private static bool IsLaunchedByMeta()
        {
            // shit check -karma
            // you made this gng -doge
            // NUH UH -karma
            string oculusPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Oculus");
            
            // checking a bunch of things just to be safe and not have shit hit the fan -doge
            var env1 = Environment.GetEnvironmentVariable("OculusAppId");
            var env2 = Environment.GetEnvironmentVariable("OCULUS_APP_ID");
            
            return !string.IsNullOrEmpty(env1)
                || !string.IsNullOrEmpty(env2)
                || Directory.Exists(oculusPath);
        }

        private static bool IsOwnedOnSteam()
        {
            try
            {
                // try steam and if it breaks i am blaming you karma -doge
                // tf did i do? -karma
                IntPtr steamApiHandle = LoadLibrary("steam_api64.dll");
                if (steamApiHandle == IntPtr.Zero) {
                    steamApiHandle = LoadLibrary("steam_api.dll"); // try this also -doge
                }

                if (steamApiHandle == IntPtr.Zero)
                    return false;

                // do more steam shit -doge
                IntPtr initPtr = GetProcAddress(steamApiHandle, "SteamAPI_Init");
                if (initPtr == IntPtr.Zero) return false;

                var initDelegate = Marshal.GetDelegateForFunctionPointer<SteamAPI_InitDelegate>(initPtr);
                if (!initDelegate())
                {
                    // need to fix offlime mode issue -doge
                    // "offlime" :skull: -karma
                    // go eat your girl's ass karma -doge
                    return false;
                }

                // valve loves changing shit -doge
                IntPtr getSteamAppsPtr = GetProcAddress(steamApiHandle, "SteamAPI_SteamApps_v008");
                if (getSteamAppsPtr == IntPtr.Zero)
                {
                    // fallback js in case -karma
                    getSteamAppsPtr = GetProcAddress(steamApiHandle, "SteamAPI_SteamApps_v007");
                }

                if (getSteamAppsPtr == IntPtr.Zero) return false;

                var getAppsDelegate = Marshal.GetDelegateForFunctionPointer<SteamAPI_GetSteamAppsDelegate>(getSteamAppsPtr);
                IntPtr steamAppsInstance = getAppsDelegate();

                IntPtr isSubscribedPtr = GetProcAddress(steamApiHandle, "SteamAPI_ISteamApps_BIsSubscribedApp");
                if (isSubscribedPtr == IntPtr.Zero) return false;

                var isSubscribedDelegate = Marshal.GetDelegateForFunctionPointer<SteamAPI_BIsSubscribedAppDelegate>(isSubscribedPtr);
                
                // WE FINALLY ACTUALLY CHECK OWNERSHIP! (i hate myself) -doge
                bool owned = isSubscribedDelegate(steamAppsInstance, STEAM_APP_ID);
                return owned;
            }
            catch (Exception ex)
            {
                // maybe log this exception properly instead of swallowing it for debugging shit easier -doge
                // no go fuck yourself -karma
                return false;
            }
        }

        private static bool IsOwnedOnMeta()
        {
            // Honestly meta checks are super fuckin annoying to implement fully without their whole SDK package -doge
            // once i get accepted for the sdk then we can torture ourselves with that -doge
            // WE? TF YOU MEAN WE? -karma
            if (META_APP_ID == 0) return true;
            
            return IsLaunchedByMeta();
        }

        #region Native imports and delegates

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        // DO NOT CHANGE ORDER OR IT WILL CRASH (found it out during debugging) AND I WILL VIOLATE WHOEVER CHANGES IT -karma
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool SteamAPI_InitDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr SteamAPI_GetSteamAppsDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool SteamAPI_BIsSubscribedAppDelegate(IntPtr self, uint appId);

        #endregion
    }
}