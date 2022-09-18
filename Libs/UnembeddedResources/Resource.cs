using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnembeddedResources
{
    /// <summary>
    /// カルチャに紐付けられた、任意の型のリソースを表します。
    /// </summary>
    /// <typeparam name="T">リソースの型。</typeparam>
    public class Resource<T>
    {
        /// <summary>
        /// このリソースが紐付けられたカルチャを取得します。
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// リソースの値を取得します。
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// <see cref="Resource{T}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="culture">このリソースを紐付けるカルチャ。</param>
        /// <param name="value">リソースの値。</param>
        public Resource(CultureInfo culture, T value)
        {
            Culture = culture;
            Value = value;
        }
    }
}
