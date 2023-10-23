using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BveTypes.ClassWrappers
{
    /// <summary>
    /// Direct3D9 によって描画することが可能であるコンポーネントなどを表します。
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// このオブジェクトに描画します。
        /// </summary>
        void Draw();

        /// <summary>
        /// Direct3D9 デバイスがロストした時に呼び出されます。
        /// </summary>
        void OnDeviceLost();

        /// <summary>
        /// Direct3D9 デバイスがリセットされた時に呼び出されます。
        /// </summary>
        void OnDeviceReset();
    }
}
