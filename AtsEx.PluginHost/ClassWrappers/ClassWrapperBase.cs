using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Automatic9045.AtsEx.PluginHost.BveTypes;
using Automatic9045.AtsEx.PluginHost.Resources;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// すべてのクラスラッパーの基本クラスを表します。
    /// </summary>
    public abstract class ClassWrapperBase
    {
        private static readonly ResourceLocalizer Resources = ResourceLocalizer.FromResXOfType<ClassWrapperBase>("PluginHost");

        /// <summary>
        /// オリジナル オブジェクトから指定したラッパー型のインスタンスを生成します。
        /// </summary>
        /// <param name="wrapperType">生成するラッパーの型。</param>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <paramref name="wrapperType"/> で指定したラッパー型のインスタンス。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="wrapperType"/> が <see langword="null"/> です。</exception>
        /// <exception cref="ArgumentException"><paramref name="wrapperType"/> で指定した型はクラスラッパーではありません。<see cref="ClassWrapperBase"/> を継承していません。</exception>
        /// <exception cref="InvalidOperationException"><paramref name="wrapperType"/> で指定した型内に有効なクラスラッパー生成メソッドが定義されていません。</exception>
        public static ClassWrapperBase CreateFromSource(Type wrapperType, object src)
        {
            if (wrapperType is null)
            {
                throw new ArgumentNullException(nameof(wrapperType));
            }
            else if (!wrapperType.IsSubclassOf(typeof(ClassWrapperBase)))
            {
                throw new ArgumentException(string.Format(Resources.GetString("TypeNotClassWrapper").Value, wrapperType.FullName, typeof(ClassWrapperBase).Name), nameof(wrapperType));
            }

            MethodInfo fromSourceMethod = wrapperType.
                GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod).
                FirstOrDefault(method =>
                {
                    if (method.GetCustomAttribute<CreateClassWrapperFromSourceAttribute>() is null) return false;

                    ParameterInfo[] parameters = method.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType == typeof(object) && method.ReturnType == wrapperType;
                });
            return fromSourceMethod is null
                ? throw new InvalidOperationException(string.Format(Resources.GetString("FromSourceMethodNotFound").Value, wrapperType.FullName))
                : (ClassWrapperBase)fromSourceMethod.Invoke(null, new object[] { src });
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// ラッパー型はオリジナル オブジェクトの型から自動で推定されます。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップしたインスタンス。</returns>
        /// <exception cref="InvalidOperationException">ラッパー型内に有効なクラスラッパー生成メソッドが定義されていません。</exception>
        public static ClassWrapperBase CreateFromSource(object src)
        {
            Type wrapperType = BveTypeSet.Instance.GetWrapperTypeOf(src.GetType());
            return CreateFromSource(wrapperType, src);
        }

        /// <summary>
        /// ラップされているオリジナル オブジェクトを取得します。
        /// </summary>
        public dynamic Src { get; }

        /// <summary>
        /// <see cref="ClassWrapperBase"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> が <see langword="null"/> です。</exception>
        public ClassWrapperBase(object src)
        {
            if (src is null) throw new ArgumentNullException();

            Src = src;
        }

        public override bool Equals(object obj)
        {
            return obj is ClassWrapperBase classWrapper && (bool)(Src as object).Equals(classWrapper.Src);
        }

        public override int GetHashCode()
        {
            return (Src as object).GetHashCode();
        }

        public override string ToString()
        {
            return (Src as object).ToString();
        }
    }
}