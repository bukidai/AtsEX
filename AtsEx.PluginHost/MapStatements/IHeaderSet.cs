﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtsEx.PluginHost.MapStatements
{
    /// <summary>
    /// ヘッダーの一覧を表します。
    /// </summary>
    public interface IHeaderSet
    {
        /// <summary>
        /// 指定した識別子 (名前) を持つヘッダーの一覧を取得します。
        /// </summary>
        /// <param name="identifier">ヘッダーの識別子。</param>
        /// <returns>識別子 <paramref name="identifier"/> を持つヘッダーの一覧。</returns>
        IReadOnlyList<IHeader> GetAll(Identifier identifier);
    }
}
