# coding=utf-8

"""
AtsEXの機能にアクセスするためのオブジェクトが変数【g】（"Globals" の略です）に代入されています。
以下、g のメンバーの説明です。C# のスタイルで記述しています。

----------------------------------------------------------------------------------------------------

全てのスクリプト（スクリプト言語で開発した拡張機能、車両・マッププラグイン、拡張地上子のスクリプト等）には

	INative Native { get; } // BVEが標準でATSプラグイン向けに提供している機能のラッパー
	IBveHacker BveHacker { get; } // BVE本体を直接操作するための機能

が渡されています。
これらの他に、BVE、AtsEX本体のバージョン情報などがApp.Instance静的プロパティから取得可能です。


●全拡張地上子共通

プラグインから Plugins[PluginType.MapPlugin][(拡張地上子PIの名前)].ExtendedBeacons[(地上子の名前)].Passed イベントを通じて値を取得可能な変数です。
void SetVariable<T>(string name, T value) メソッドで書き込み、T GetVariable<T>(string name) メソッドで読み込みができます。

この変数は拡張地上子スクリプトを跨いで共有することはできません。


※プラグインに紐づいた変数は廃止となりました（β0.17～）。


☆より高レベルな実装をしたい場合は独自構文機能をご利用ください。


●自列車が通過した場合

上記プロパティに加えて

	readonly ExtendedBeaconBase<PassedEventArgs> sender;
	readonly PassedEventArgs e;

が渡されます。


●他列車が通過した場合

上記プロパティに加えて

	readonly ExtendedBeaconBase<TrainPassedEventArgs> sender;
	readonly TrainPassedEventArgs e;

が渡されます。

"""

import sys
import clr
sys.path.append('MapPlugins')
clr.AddReference('ExtendedBeacons')

import System
from System.Windows.Forms import *
import AtsEx.Extensions.ExtendedBeacons as eb


def tick(g):
	# 自・他列車両方の通過の検知をする（Map.txt の地上子定義を参照）ので、sender の型で場合分けをする。
	
	beaconName = g.sender.BeaconName
	beaconTrack = g.sender.DefinedStatement.DefinedStructure.TrackKey
	beaconTargetTrack = g.sender.ObservingTargetTrack
	beaconTargetTrain = g.sender.ObservingTargetTrain
	trainDirection = g.e.Direction
	
	if beaconTargetTrain == eb.ObservingTargetTrain.Myself:
		MessageBox.Show('自列車の通過を検知しました。\n' +
			'\n' +
			'拡張地上子の名前: ' + beaconName.FullName + '\n' +
			'拡張地上子の設置先軌道: ' + beaconTrack + '\n' +
			'拡張地上子の検知対象軌道: ' + str(beaconTargetTrack) + '\n' +
			'拡張地上子の検知対象列車: ' + str(beaconTargetTrain) + '\n' +
			'\n' +
			'自列車の進行方向: ' + str(trainDirection), 'IronPython2 による拡張地上子のサンプル')
		
	elif beaconTargetTrain == eb.ObservingTargetTrain.Trains:
		trainName = g.e.SenderTrainName
		trainTrack = g.e.SenderTrain.TrainInfo.TrackKey
		
		MessageBox.Show('他列車の通過を検知しました。\n' +
			'\n' +
			'拡張地上子の名前: ' + beaconName.FullName + '\n' +
			'拡張地上子の設置先軌道: ' + beaconTrack + '\n' +
			'拡張地上子の検知対象軌道: ' + str(beaconTargetTrack) + '\n' +
			'拡張地上子の検知対象列車: ' + str(beaconTargetTrain) + '\n' +
			'\n' +
			'他列車の名前: ' + trainName + '\n' +
			'他列車の走行軌道: ' + trainTrack + '\n' +
			'他列車の進行方向: ' + str(trainDirection), 'IronPython2 による拡張地上子のサンプル')
		
	else:
		raise System.NotSupportedException('自列車、他列車の通過検知のみをサポートしています。')