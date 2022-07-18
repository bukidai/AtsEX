# coding=utf-8

"""
AtsEXの機能にアクセスするためのオブジェクトが変数【g】（"Globals" の略です）に代入されています。
以下、g のメンバーの説明です。C# のスタイルで記述しています。

----------------------------------------------------------------------------------------------------

全てのスクリプト（スクリプト言語で開発した車両・マッププラグイン、拡張地上子のスクリプトのどちらも）には

	IApp App { get; }
	IBveHacker BveHacker { get; }

が渡されています。


●自列車が通過した場合

App、BveHackerに加えて

	readonly ExtendedBeaconBase<PassedEventArgs> sender;
	readonly PassedEventArgs e;

が渡されます。


●他列車が通過した場合

App、BveHackerに加えて

	readonly ExtendedBeaconBase<TrainPassedEventArgs> sender;
	readonly TrainPassedEventArgs e;

が渡されます。

"""

import System
from System.Windows.Forms import *
import Automatic9045.AtsEx.PluginHost.ExtendedBeacons as eb


def tick(g):
	# 自・他列車両方の通過の検知をする（Map.txt の地上子定義を参照）ので、sender の型で場合分けをする。
	
	beaconName = g.sender.Name
	beaconTrack = g.sender.DefinedStructure.TrackKey
	beaconTargetTrack = g.sender.ObservingTargetTrack
	beaconTargetTrain = g.sender.ObservingTargetTrain
	trainDirection = g.e.Direction
	
	if beaconTargetTrain == eb.ObservingTargetTrain.Myself:
		MessageBox.Show('自列車の通過を検知しました。\n' +
			'\n' +
			'拡張地上子の名前: ' + beaconName + '\n' +
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
			'拡張地上子の名前: ' + beaconName + '\n' +
			'拡張地上子の設置先軌道: ' + beaconTrack + '\n' +
			'拡張地上子の検知対象軌道: ' + str(beaconTargetTrack) + '\n' +
			'拡張地上子の検知対象列車: ' + str(beaconTargetTrain) + '\n' +
			'\n' +
			'他列車の名前: ' + trainName + '\n' +
			'他列車の走行軌道: ' + trainTrack + '\n' +
			'他列車の進行方向: ' + str(trainDirection), 'IronPython2 による拡張地上子のサンプル')
		
	else:
		raise System.NotSupportedException('自列車、他列車の通過検知のみをサポートしています。')