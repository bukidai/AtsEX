using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost
{
    /// <summary>
    /// <see cref="PluginHost"/> がラッパーを提供する BVE の型と、そのメンバーを提供します。
    /// </summary>
    public interface IBveTypeMemberCollection
    {
        /// <summary>
        /// <see cref="PluginHost"/> が提供するラッパー型を取得します。
        /// </summary>
        Type WrapperType { get; }

        /// <summary>
        /// ラップ元の BVE オリジナル型を取得します。
        /// </summary>
        Type OriginalType { get; }


        /// <summary>
        /// <paramref name="wrapperName"/> で指定した名前のラッパープロパティのオリジナルの get アクセサを取得します。
        /// </summary>
        /// <param name="wrapperName">ラッパープロパティの名前。</param>
        /// <returns>オリジナルプロパティの get アクセサの <see cref="MethodInfo"/>。</returns>
        MethodInfo GetSourcePropertyGetterOf(string wrapperName);

        /// <summary>
        /// <paramref name="wrapperName"/> で指定した名前のラッパーメソッドのオリジナルの set アクセサを取得します。
        /// </summary>
        /// <param name="wrapperName">ラッパープロパティの名前。</param>
        /// <returns>オリジナルプロパティの set アクセサの <see cref="MethodInfo"/>。</returns>
        MethodInfo GetSourcePropertySetterOf(string wrapperName);

        /// <summary>
        /// <paramref name="wrapperName"/> で指定した名前のラッパープロパティのオリジナルフィールドを取得します。
        /// </summary>
        /// <param name="wrapperName">ラッパープロパティの名前。</param>
        /// <returns>オリジナルフィールドの <see cref="FieldInfo"/>。</returns>
        FieldInfo GetSourceFieldOf(string wrapperName);

        /// <summary>
        /// <paramref name="wrapperName"/> で指定した名前のラッパーメソッドのオリジナルを取得します。
        /// </summary>
        /// <param name="wrapperName">ラッパーメソッドの名前。</param>
        /// <param name="parameters">ラッパーメソッドの引数のリスト。パラメータが無い場合は <see cref="Type.EmptyTypes"/>を指定します。
        /// オーバーロードしていない場合は <see langword="null"/> を指定して省略することもできます。</param>
        /// <returns>オリジナルメソッドの <see cref="MethodInfo"/>。</returns>
        MethodInfo GetSourceMethodOf(string wrapperName, Type[] parameters = null);
    }
}
