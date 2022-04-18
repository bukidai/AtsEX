using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automatic9045.AtsEx.Export
{

    /// <summary>列車のスペックに関する構造体</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct VehicleSpec
    {
        /// <summary>ブレーキ段数</summary>
        public int BrakeNotches;
        /// <summary>力行ノッチ段数</summary>
        public int PowerNotches;
        /// <summary>ATS確認段数</summary>
        public int AtsNotch;
        /// <summary>常用最大段数</summary>
        public int B67Notch;
        /// <summary>編成車両数</summary>
        public int Cars;
    };
    /// <summary>列車状態に関する構造体</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct VehicleState
    {
        /// <summary>列車位置[m]</summary>
        public double Location;
        /// <summary>列車速度[km/h]</summary>
        public float Speed;
        /// <summary>0時からの経過時間[ms]</summary>
        public int Time;
        /// <summary>BC圧力[kPa]</summary>
        public float BcPressure;
        /// <summary>MR圧力[kPa]</summary>
        public float MrPressure;
        /// <summary>ER圧力[kPa]</summary>
        public float ErPressure;
        /// <summary>BP圧力[kPa]</summary>
        public float BpPressure;
        /// <summary>SAP圧力[kPa]</summary>
        public float SapPressure;
        /// <summary>電流[A]</summary>
        public float Current;
    };
    /// <summary>ハンドル位置に関する構造体</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Handles
    {
        /// <summary>ブレーキハンドル位置</summary>
        public int Brake;
        /// <summary>力行ノッチハンドル位置</summary>
        public int Power;
        /// <summary>レバーサーハンドル位置</summary>
        public int Reverser;
        /// <summary>定速制御状態</summary>
        public int ConstantSpeed;
    };
    /// <summary>Beaconに関する構造体</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct BeaconData
    {
        /// <summary>Beaconの番号</summary>
        public int Num;
        /// <summary>対応する閉塞の現示番号</summary>
        public int Sig;
        /// <summary>対応する閉塞までの距離[m]</summary>
        public float Z;
        /// <summary>Beaconの第三引数の値</summary>
        public int Data;
    };
    /// <summary>レバーサー位置</summary>
    internal enum ReverserPosition
    {
        /// <summary>後進</summary>
        B = -1,
        /// <summary>中立</summary>
        N = 0,
        /// <summary>前進</summary>
        F = 1,
    }
    /// <summary>定速制御の状態</summary>
    internal enum ConstantSpeed
    {
        /// <summary>前回の状態を継続する</summary>
        Continue = 0,
        /// <summary>有効化する</summary>
        Enable = 1,
        /// <summary>無効化する</summary>
        Disable = 2,
    }
    /// <summary>警笛の種類</summary>
    internal enum HornType
    {
        /// <summary>Primary Horn</summary>
        Primary = 0,
        /// <summary>Secondary Horn</summary>
        Secondary = 1,
        /// <summary>Music Horn</summary>
        Music = 2,
    }
    /// <summary>サウンド再生に関する操作の情報</summary>
    internal enum SoundPlayType
    {
        /// <summary>繰り返し再生する</summary>
        Loop = 0,
        /// <summary>一度だけ再生する</summary>
        Once = 1,
        /// <summary>前回の状態を継続する</summary>
        Continue = 2,
        /// <summary>再生を停止する</summary>
        Stop = -1000,
    }
    /// <summary>ハンドルの初期位置設定</summary>
    internal enum DefaultBrakePosition
    {
        /// <summary>常用ブレーキ(B67?)</summary>
        Service = 0,
        /// <summary>非常ブレーキ位置</summary>
        Emergency = 1,
        /// <summary>抜き取り位置</summary>
        Removed = 2,
    }
    /// <summary>ATS Keys</summary>
    internal enum ATSKeys
    {
        /// <summary>ATSKey_S (Default : Space)</summary>
        S = 0,
        /// <summary>ATSKey_A1 (Default : Insert)</summary>
        A1 = 1,
        /// <summary>ATSKey_A2 (Default : Delete)</summary>
        A2 = 2,
        /// <summary>ATSKey_B1 (Default : Home)</summary>
        B1 = 3,
        /// <summary>ATSKey_B2 (Default : End)</summary>
        B2 = 4,
        /// <summary>ATSKey_C1 (Default : PageUp)</summary>
        C1 = 5,
        /// <summary>ATSKey_C2 (Default : PageDown)</summary>
        C2 = 6,
        /// <summary>ATSKey_D (Default : D2)</summary>
        D = 7,
        /// <summary>ATSKey_E (Default : D3)</summary>
        E = 8,
        /// <summary>ATSKey_F (Default : D4)</summary>
        F = 9,
        /// <summary>ATSKey_G (Default : D5)</summary>
        G = 10,
        /// <summary>ATSKey_H (Default : D6)</summary>
        H = 11,
        /// <summary>ATSKey_I (Default : D7)</summary>
        I = 12,
        /// <summary>ATSKey_J (Default : D8)</summary>
        J = 13,
        /// <summary>ATSKey_K (Default : D9)</summary>
        K = 14,
        /// <summary>ATSKey_L (Default : D0)</summary>
        L = 15,
    }



    /// <summary>処理を実装するクラス</summary>
    internal static class AtsCore
    {
        private const int Version = 0x00020000;

        private static AtsExActivator Activator;

        static AtsCore()
        {
#if DEBUG
            MessageBox.Show("AtsEX ATSプラグイン拡張キット\n\nデバッグモードで読み込まれました。");
#endif

            Activator = new AtsExActivator();
            Activator.UpdateIfNotLatest();
            Activator.ResolveAssemblies();
        }

        /// <summary>Called when this plugin is loaded</summary>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void Load() => AtsMain.Load(Activator);

        /// <summary>Called when this plugin is unloaded</summary>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void Dispose() => AtsMain.Dispose();

        /// <summary>Called when the version number is needed</summary>
        /// <returns>plugin version number</returns>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static int GetPluginVersion() => Version;

        /// <summary>Called when set the Vehicle Spec</summary>
        /// <param name="vehicleSpec">Set Spec</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetVehicleSpec(VehicleSpec vehicleSpec)
        {
            AtsMain.VehicleSpec = vehicleSpec;
            AtsMain.SetVehicleSpec(vehicleSpec);
        }

        /// <summary>Called when car is put</summary>
        /// <param name="defaultBrakePosition">Default Brake Position (Refer to InitialPos class)</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void Initialize(int defaultBrakePosition) => AtsMain.Initialize(defaultBrakePosition);

        /// <summary>Called in every refleshing the display</summary>
        /// <param name="vehicleState">State</param>
        /// <param name="panel">Panel (Pointer of int[256])</param>
        /// <param name="sound">Sound (Pointer of int[256])</param>
        /// <returns></returns>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static Handles Elapse(VehicleState vehicleState, int[] panel, int[] sound)
        {
            AtsMain.Elapse(vehicleState, panel, sound);
            return AtsMain.Handle;
        }

        /// <summary>Called when Power notch is moved</summary>
        /// <param name="notch">Notch Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetPower(int notch)
        {
            AtsMain.Handle.Power = notch;
            AtsMain.SetPower(notch);
        }

        /// <summary>Called when Brake Notch is moved</summary>
        /// <param name="notch">Brake notch Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetBrake(int notch)
        {
            AtsMain.Handle.Brake = notch;
            AtsMain.SetBrake(notch);
        }

        /// <summary>Called when Reverser is moved</summary>
        /// <param name="position">Reverser Position</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetReverser(int position)
        {
            AtsMain.Handle.Reverser = position;
            AtsMain.SetReverser(position);
        }

        /// <summary>Called when Key is Pushed</summary>
        /// <param name="atsKeyCode">Pushed Key Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void KeyDown(int atsKeyCode)
        {
            AtsMain.IsKeyDown[atsKeyCode] = true;
            AtsMain.KeyDown(atsKeyCode);
        }

        /// <summary>Called when Key is Released</summary>
        /// <param name="atsKeyCode">Released Key Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void KeyUp(int atsKeyCode)
        {
            AtsMain.IsKeyDown[atsKeyCode] = false;
            AtsMain.KeyUp(atsKeyCode);
        }

        /// <summary>Called when the Horn is Blown</summary>
        /// <param name="hornType">Blown Horn Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void HornBlow(int hornType) => AtsMain.HornBlow((HornType)hornType);

        /// <summary>Called when Door is opened</summary>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void DoorOpen()
        {
            AtsMain.IsDoorClosed = false;
            AtsMain.DoorOpen();
        }

        /// <summary>Called when Door is closed</summary>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void DoorClose()
        {
            AtsMain.IsDoorClosed = true;
            AtsMain.DoorClose();
        }

        /// <summary>Called when the Signal Showing Number is changed</summary>
        /// <param name="signal">Signal Showing Number</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetSignal(int signal) => AtsMain.SetSignal(signal);

        /// <summary>Called when passed above the Beacon</summary>
        /// <param name="beaconData">Beacon info</param>
        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static void SetBeaconData(BeaconData beaconData) => AtsMain.SetBeaconData(beaconData);
    }
}
