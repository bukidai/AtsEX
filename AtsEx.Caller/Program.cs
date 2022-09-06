using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.Native;

namespace Automatic9045.AtsEx.Caller
{
    /// <summary>処理を実装するクラス</summary>
    internal static class AtsCore
    {
        private const int Version = 0x00020000;

        private static Assembly Assembly = Assembly.GetExecutingAssembly();

        static AtsCore()
        {
            try
            {
                string configLocation = Path.Combine(Path.GetDirectoryName(Assembly.Location), "AtsEx.Caller.txt");
                using (StreamReader sr = new StreamReader(configLocation))
                {
                    string atsExLocation = Path.Combine(Path.GetDirectoryName(Assembly.Location), sr.ReadLine());

                    AssemblyResolver assemblyResolver = new AssemblyResolver(AppDomain.CurrentDomain);
                    assemblyResolver.Register(atsExLocation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to initialize AtsEX Caller.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>Called when this plugin is loaded</summary>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void Load() => Export.Load(Assembly);

        /// <summary>Called when this plugin is unloaded</summary>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void Dispose() => Export.Dispose();

        /// <summary>Called when the version number is needed</summary>
        /// <returns>plugin version number</returns>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static int GetPluginVersion() => Version;

        /// <summary>Called when set the Vehicle Spec</summary>
        /// <param name="vehicleSpec">Set Spec</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetVehicleSpec(VehicleSpec vehicleSpec) => Export.SetVehicleSpec(vehicleSpec);

        /// <summary>Called when car is put</summary>
        /// <param name="defaultBrakePosition">Default Brake Position (Refer to InitialPos class)</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void Initialize(int defaultBrakePosition) => Export.Initialize(defaultBrakePosition);

        /// <summary>Called in every refleshing the display</summary>
        /// <param name="vehicleState">State</param>
        /// <param name="panel">Panel (Pointer of int[256])</param>
        /// <param name="sound">Sound (Pointer of int[256])</param>
        /// <returns></returns>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static AtsHandles Elapse(VehicleState vehicleState, int[] panel, int[] sound) => Export.Elapse(vehicleState, panel, sound);

        /// <summary>Called when Power notch is moved</summary>
        /// <param name="notch">Notch Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetPower(int notch) => Export.SetPower(notch);

        /// <summary>Called when Brake Notch is moved</summary>
        /// <param name="notch">Brake notch Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetBrake(int notch) => Export.SetBrake(notch);

        /// <summary>Called when Reverser is moved</summary>
        /// <param name="position">Reverser Position</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetReverser(int position) => Export.SetReverser(position);

        /// <summary>Called when Key is Pushed</summary>
        /// <param name="atsKeyCode">Pushed Key Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void KeyDown(int atsKeyCode) => Export.KeyDown(atsKeyCode);

        /// <summary>Called when Key is Released</summary>
        /// <param name="atsKeyCode">Released Key Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void KeyUp(int atsKeyCode) => Export.KeyUp(atsKeyCode);

        /// <summary>Called when the Horn is Blown</summary>
        /// <param name="hornType">Blown Horn Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void HornBlow(int hornType) => Export.HornBlow((HornType)hornType);

        /// <summary>Called when Door is opened</summary>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void DoorOpen() => Export.DoorOpen();

        /// <summary>Called when Door is closed</summary>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void DoorClose() => Export.DoorClose();

        /// <summary>Called when the Signal Showing Number is changed</summary>
        /// <param name="signal">Signal Showing Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetSignal(int signal) => Export.SetSignal(signal);

        /// <summary>Called when passed above the Beacon</summary>
        /// <param name="beaconData">Beacon info</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetBeaconData(BeaconData beaconData) => Export.SetBeaconData(beaconData);
    }
}
