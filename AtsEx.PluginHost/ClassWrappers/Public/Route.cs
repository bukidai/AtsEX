﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SlimDX;

using FastMember;
using TypeWrapping;

using AtsEx.PluginHost.BveTypes;

namespace AtsEx.PluginHost.ClassWrappers
{
    /// <summary>
    /// マップに関する情報にアクセスするための機能を提供します。
    /// </summary>
    public class Route : ClassWrapperBase
    {
        [InitializeClassWrapper]
        private static void Initialize(BveTypeSet bveTypes)
        {
            ClassMemberSet members = bveTypes.GetClassInfoOf<Route>();

            DrawLimitLocationGetMethod = members.GetSourcePropertyGetterOf(nameof(DrawLimitLocation));
            DrawLimitLocationSetMethod = members.GetSourcePropertySetterOf(nameof(DrawLimitLocation));

            StructuresGetMethod = members.GetSourcePropertyGetterOf(nameof(Structures));
            StructuresField = members.GetSourceFieldOf(nameof(Structures));

            StationsGetMethod = members.GetSourcePropertyGetterOf(nameof(Stations));

            PreTrainObjectsGetMethod = members.GetSourcePropertyGetterOf(nameof(PreTrainObjects));

            SectionsGetMethod = members.GetSourcePropertyGetterOf(nameof(Sections));

            SoundsGetMethod = members.GetSourcePropertyGetterOf(nameof(Sounds));

            Sounds3DGetMethod = members.GetSourcePropertyGetterOf(nameof(Sounds3D));

            StructureModelsGetMethod = members.GetSourcePropertyGetterOf(nameof(StructureModels));

            DrawDistanceObjectsGetMethod = members.GetSourcePropertyGetterOf(nameof(DrawDistanceObjects));

            GetTrackMatrixMethod = members.GetSourceMethodOf(nameof(GetTrackMatrix));
        }

        /// <summary>
        /// オリジナル オブジェクトから <see cref="Route"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        protected Route(object src) : base(src)
        {
        }

        /// <summary>
        /// オリジナル オブジェクトからラッパーのインスタンスを生成します。
        /// </summary>
        /// <param name="src">ラップするオリジナル オブジェクト。</param>
        /// <returns>オリジナル オブジェクトをラップした <see cref="Route"/> クラスのインスタンス。</returns>
        [CreateClassWrapperFromSource]
        public static Route FromSource(object src) => src is null ? null : new Route(src);

        private static FastMethod DrawLimitLocationGetMethod;
        private static FastMethod DrawLimitLocationSetMethod;
        /// <summary>
        /// ストラクチャーが設置される限界の距離程 [m] を取得・設定します。通常は最後の駅の 10km 先の位置になります。
        /// </summary>
        /// <remarks>
        /// この数値を変更しても BVE には反映されません。
        /// </remarks>
        public double DrawLimitLocation
        {
            get => DrawLimitLocationGetMethod.Invoke(Src, null);
            set => DrawLimitLocationSetMethod.Invoke(Src, new object[] { value });
        }

        private static FastMethod StructuresGetMethod;
        private static FastField StructuresField;
        /// <summary>
        /// Structure マップ要素、Repeater マップ要素で設置されたストラクチャーを取得します。
        /// </summary>
        /// <remarks>
        /// Structure.Load ステートメントから読み込まれたストラクチャーの 3D モデルのリストの取得には <see cref="StructureModels"/> を使用してください。
        /// </remarks>
        /// <remarks>
        /// このクラスからストラクチャーを編集しても BVE には反映されません。ストラクチャーを動かしたい場合は他列車を使用してください。
        /// </remarks>
        public StructureSet Structures
        {
            get => StructureSet.FromSource(StructuresGetMethod.Invoke(Src, null));
            internal set => StructuresField.SetValue(Src, value.Src);
        }

        private static FastMethod StationsGetMethod;
        /// <summary>
        /// 停車場のリストを取得します。
        /// </summary>
        public StationList Stations => StationList.FromSource(StationsGetMethod.Invoke(Src, null));

        private static FastMethod PreTrainObjectsGetMethod;
        /// <summary>
        /// 先行列車の通過時刻のリストを取得します。
        /// </summary>
        public PreTrainObjectList PreTrainObjects => PreTrainObjectList.FromSource(PreTrainObjectsGetMethod.Invoke(Src, null));

        private static FastMethod SectionsGetMethod;
        /// <summary>
        /// 閉塞の一覧を取得します。
        /// </summary>
        public MapFunctionList Sections => MapFunctionList.FromSource(SectionsGetMethod.Invoke(Src, null));

        private static FastMethod SoundsGetMethod;
        /// <summary>
        /// Sound.Load ステートメントから読み込まれたサウンドのリストを取得します。
        /// </summary>
        /// <remarks>
        /// Sound3D.Load ステートメントから読み込まれたサウンドのリストの取得には <see cref="Sounds3D"/> プロパティを使用してください。
        /// </remarks>
        /// <value>キーがサウンド名、値がサウンドを表す <see cref="Sound"/> の <see cref="WrappedSortedList{TKey, TValue}"/>。</value>
        /// <seealso cref="Sounds3D"/>
        public WrappedSortedList<string, Sound> Sounds
        {
            get
            {
                IDictionary dictionarySrc = SoundsGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, Sound>(dictionarySrc);
            }
        }

        private static FastMethod Sounds3DGetMethod;
        /// <summary>
        /// Sound3D.Load ステートメントから読み込まれたサウンドのリストを取得します。
        /// </summary>
        /// <remarks>
        /// Sound.Load ステートメントから読み込まれたサウンドのリストの取得には <see cref="Sounds"/> プロパティを使用してください。
        /// </remarks>
        /// <value>キーがサウンド名、値がサウンドを表す <see cref="Sound"/> の <see cref="WrappedSortedList{TKey, TValue}"/>。</value>
        /// <seealso cref="Sounds"/>
        public WrappedSortedList<string, Sound[]> Sounds3D
        {
            get
            {
                IDictionary dictionarySrc = Sounds3DGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, Sound[]>(dictionarySrc, new Sounds3DConverter());
            }
        }

        private static FastMethod StructureModelsGetMethod;
        /// <summary>
        /// Structure.Load ステートメントから読み込まれたストラクチャーの 3D モデルのリストを取得します。
        /// </summary>
        /// <remarks>
        /// 設置されたストラクチャーのリストの取得には <see cref="Structures"/> を使用してください。
        /// </remarks>
        /// <value>キーがストラクチャー名、値がストラクチャーの 3D モデルを表す <see cref="Model"/> の <see cref="WrappedSortedList{TKey, TValue}"/>。</value>
        /// <seealso cref="Structures"/>
        public WrappedSortedList<string, Model> StructureModels
        {
            get
            {
                IDictionary dictionarySrc = StructureModelsGetMethod.Invoke(Src, null);
                return new WrappedSortedList<string, Model>(dictionarySrc);
            }
        }

        private static FastMethod DrawDistanceObjectsGetMethod;
        /// <summary>
        /// DrawDistance.Change ステートメントにより設置された、最長描画距離を指定するためのオブジェクトを取得します。
        /// </summary>
        public MapFunctionList DrawDistanceObjects => MapFunctionList.FromSource(DrawDistanceObjectsGetMethod.Invoke(Src, null));

        private static FastMethod GetTrackMatrixMethod;
        /// <summary>
        /// ストラクチャーを軌道上の指定した距離程へ配置するためのワールド変換行列を取得します。
        /// </summary>
        /// <param name="mapObject">配置するストラクチャーを表す <see cref="LocatableMapObject"/>。</param>
        /// <param name="location">配置先の距離程 [m]。</param>
        /// <param name="targetBlockLocation">配置先のストラクチャー描画ブロックの距離程 [m]。</param>
        /// <returns>ワールド変換行列を表す <see cref="Matrix"/>。</returns>
        public Matrix GetTrackMatrix(LocatableMapObject mapObject, double location, double targetBlockLocation)
            => GetTrackMatrixMethod.Invoke(Src, new object[] { mapObject.Src, location, targetBlockLocation });

        private class Sounds3DConverter : ITwoWayConverter<object, Sound[]>
        {
            public Sound[] Convert(object value)
            {
                Array srcArray = value as Array;
                Sound[] result = new Sound[srcArray.Length];
                for (int i = 0; i < srcArray.Length; i++)
                {
                    object srcArrayItem = srcArray.GetValue(i);
                    result[i] = srcArrayItem is null ? null : Sound.FromSource(srcArrayItem);
                }

                return result;
            }

            public object ConvertBack(Sound[] value)
            {
                object[] result = new object[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    result[i] = value[i]?.Src;
                }

                return result;
            }
        }
    }
}
