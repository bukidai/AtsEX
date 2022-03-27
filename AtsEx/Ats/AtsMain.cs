using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Automatic9045.AtsEx.PluginHost;

namespace Automatic9045.AtsEx.Ats
{
    /// <summary>メインの機能をここに実装する。</summary>
    internal static class AtsMain
    {
        public static VehicleSpec VehicleSpec { get; set; }

        /// <summary>Is the Door Closed TF</summary>
        public static bool IsDoorClosed { get; set; } = false;

        /// <summary>Current State of Handles</summary>
        public static Handles Handle = default;

        public static VehicleState VehicleState { get; set; } = default;

        /// <summary>Current Key State</summary>
        public static bool[] IsKeyDown { get; set; } = new bool[16];

        public static AtsEx AtsEx { get; private set; }

        public static void Load()
        {
#if DEBUG
            MessageBox.Show("AtsEX ATSプラグイン拡張キット\n\nデバッグモードで読み込まれました。");
#endif

            Process targetProcess = Process.GetCurrentProcess();
            AppDomain targetAppDomain = AppDomain.CurrentDomain;
            Assembly targetAssembly = Assembly.GetEntryAssembly();

            if (targetAssembly is null)
            {
                ShowErrorDialog("BVE 本体が読み込めないフォーマットです。");
            }
            else if (!targetAssembly.GetTypes().Any(t => t.Namespace == "Mackoy.Bvets"))
            {
                ShowErrorDialog("BVE 本体と異なるプロセスで実行することはできません。", "https://automatic9045.github.io/contents/bve/AtsEX/faq/#diff-process");
            }
            
            AssemblyResolver assemblyResolver = new AssemblyResolver(targetAppDomain);
            assemblyResolver.Register(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "0Harmony.dll"));
            assemblyResolver.Register(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "atsex.pihost.dll"));
            assemblyResolver.Register(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "atsex.bvetypes.dll"));

            AtsEx = new AtsEx(targetProcess, targetAppDomain, targetAssembly);


            void ShowErrorDialog(string message, string faqUrl = null)
            {
                if (faqUrl is null)
                {
                    MessageBox.Show(message, $"エラー - AtsEX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show($"{message}\n\nこのエラーに関する情報を表示しますか？\n（ブラウザで Web サイトが開きます）", $"エラー - AtsEX", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Process.Start(faqUrl);
                    }
                }

                throw new NotSupportedException(message);
            }
        }

        public static void Dispose()
        {
            AtsEx?.Dispose();
        }

        public static void SetVehicleSpec(VehicleSpec vehicleSpec)
        {

        }

        public static void Initialize(int defaultBrakePosition)
        {
            AtsEx?.Started((BrakePosition)defaultBrakePosition);
        }

        public static void Elapse(int[] panel, int[] sound)
        {
            AtsEx?.Elapse();
        }

        public static void SetPower(int notch)
        {

        }

        public static void SetBrake(int notch)
        {

        }

        public static void SetReverser(int position)
        {

        }
        public static void KeyDown(int atsKeyCode)
        {

        }

        public static void KeyUp(int atsKeyCode)
        {

        }

        public static void DoorOpen()
        {

        }
        public static void DoorClose()
        {

        }
        public static void HornBlow(HornType hornType)
        {

        }
        public static void SetSignal(int signal)
        {

        }
        public static void SetBeaconData(BeaconData beaconData)
        {

        }
    }
}
