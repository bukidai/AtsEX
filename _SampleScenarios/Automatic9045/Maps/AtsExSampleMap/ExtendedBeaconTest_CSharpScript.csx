/*

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

*/

// 他列車の通過の検知しかしない（Map.txt の地上子定義を参照）ので e は必ず TrainPassedEventArgs 型。

string beaconName = sender.Name;
string beaconTrack = sender.DefinedStructure.TrackKey;
string trainName = e.SenderTrainName;
string trainTrack = e.SenderTrain.TrainInfo.TrackKey;
Direction trainDirection = e.Direction;

MessageBox.Show($"他列車の通過を検知しました。\n" +
	"\n" +
	"拡張地上子の名前: {beaconName}\n" +
	"拡張地上子の設置先軌道: {beaconTrack}\n" +
	"\n" +
	"他列車の名前: {trainName}\n" +
	"他列車の走行軌道: {trainTrack}\n" +
	"他列車の進行方向: {trainDirection}", "C# スクリプトによる拡張地上子のサンプル");