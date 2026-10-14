using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

// IF ANYONE REMOVES THIS CODE OR BYPASSES IT I AM VIOLATING YOU -doge
namespace IPA.Injector
{
    internal static class AntiPiracy
    {
        // TODO: GET SHITTY META SDK -doge
        // GET BACK TO WORK CODE SLAVE -karma
        // no -doge
        private const uint STEAM_APP_ID = 620980;
        // 0 to skip oculus check for now cuz i am NOT doing SHIT for meta's dumb sdk -doge
        private const ulong META_APP_ID = 0;

        public static bool IsInvalid(string path)
        {
            // shit ass code ngl -karma
            // I KNOW -doge

            // steam check first cause valve made it SO FUCKIN EASY THANK YOU VALVE -doge
            if (TrySteamOwnership(path, out bool owned))
            {
                return !owned;
            }

            // then meta/oculus which does ABSOLUTLY NOTHING CAUSE META IS RETARDED -doge
            if (IsLaunchedByMeta())
            {
                return false;
            }

            // doge is retarded -karma
            // i will shit on you karma -doge
            // woah, calm down buddy -karma
            // fuck off -doge
            // TODO: maybe show a message box yelling at the idiot for pirating -doge
            
            return true;
        }

        private static bool TrySteamOwnership(string gamePath, out bool isOwned)
        {
            isOwned = false;
            string appIdFile = null;

            try
            {
                // if this fix doesn't work i am bouta kill doge -karma
                // fuck i do to you gng? -doge
                appIdFile = Path.Combine(gamePath, "steam_appid.txt");
                bool createdAppIdFile = false;
                if (!File.Exists(appIdFile))
                {
                    File.WriteAllText(appIdFile, STEAM_APP_ID.ToString());
                    createdAppIdFile = true;
                }

                string dll64 = Path.Combine(gamePath, "steam_api64.dll");
                string dll32 = Path.Combine(gamePath, "steam_api.dll");

                IntPtr handle = IntPtr.Zero;
                // try steam and if it breaks i am blaming you karma -doge
                // tf did i do? -karma
                if (File.Exists(dll64))
                {
                    handle = LoadLibrary(dll64);
                }
                else if (File.Exists(dll32))
                {
                    // I LOVE ANTI-PIRACY -doge
                    handle = LoadLibrary(dll32);
                }
                else
                {
                    // (insert man shooting basketball from moon here) ahhh code -karma
                    // didn't yo girl not want to pop a titty for you? -doge
                    // TOO FAR BRO -karma
                    handle = LoadLibrary("steam_api64.dll");
                    if (handle == IntPtr.Zero)
                    {
                        handle = LoadLibrary("steam_api.dll");
                    }
                }

                if (handle == IntPtr.Zero)
                    return false; // if some shithead doesn't have steam i'm finna kms -doge

                // do more steam shit -doge
                IntPtr initPtr = GetProcAddress(handle, "SteamAPI_Init");
                if (initPtr == IntPtr.Zero)
                    return false;

                var init = Marshal.GetDelegateForFunctionPointer<SteamAPI_InitDelegate>(initPtr);
                // need to fix offlime mode issue -doge
                // "offlime" :skull: -karma
                // go eat your girl's ass karma -doge
                if (!init())
                    return false;

                // valve loves changing shit -doge
                // i hate how shit my code is -doge
                IntPtr getAppsPtr = GetProcAddress(handle, "SteamAPI_SteamApps_v008");
                if (getAppsPtr == IntPtr.Zero)
                {
                    getAppsPtr = GetProcAddress(handle, "SteamAPI_SteamApps_v007");
                }
                if (getAppsPtr == IntPtr.Zero)
                {
                    getAppsPtr = GetProcAddress(handle, "SteamAPI_SteamApps_v006"); // fallback js in case -karma
                }

                if (getAppsPtr == IntPtr.Zero)
                    return false;

                var getApps = Marshal.GetDelegateForFunctionPointer<SteamAPI_GetSteamAppsDelegate>(getAppsPtr);
                IntPtr apps = getApps();

                IntPtr subscribedPtr = GetProcAddress(handle, "SteamAPI_ISteamApps_BIsSubscribedApp");
                if (subscribedPtr == IntPtr.Zero)
                    return false;

                var isSubscribed = Marshal.GetDelegateForFunctionPointer<SteamAPI_BIsSubscribedAppDelegate>(subscribedPtr);
                // WE FINALLY ACTUALLY CHECK OWNERSHIP! (i hate myself) -doge 
                isOwned = isSubscribed(apps, STEAM_APP_ID);
                return true;
            }
            catch (Exception)
            {
                // maybe log this exception properly instead of swallowing it for debugging shit easier -doge
                // no go fuck yourself -karma
                return false;
            }
            finally
            {
                try
                {
                    if (appIdFile != null && File.Exists(appIdFile))
                    {
                        // BSMANAGER PATCH MAYBE??????? -doge
                        string content = File.ReadAllText(appIdFile).Trim();
                        if (content == STEAM_APP_ID.ToString())
                            File.Delete(appIdFile);
                    }
                }
                catch { /* ignore cleanup failures */ }
            }
        }

        private static bool IsLaunchedByMeta()
        {
            // shit check -karma
            // you made this gng -doge
            // NUH UH -karma

            // checking a bunch of things just to be safe and not have shit hit the fan -doge
            string oc1 = Environment.GetEnvironmentVariable("OculusAppId");
            string oc2 = Environment.GetEnvironmentVariable("OCULUS_APP_ID");
            string oc3 = Environment.GetEnvironmentVariable("OVR_APP_ID");

            if (!string.IsNullOrEmpty(oc1) || !string.IsNullOrEmpty(oc2) || !string.IsNullOrEmpty(oc3))
            {
                return true;
            }

            return false;
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