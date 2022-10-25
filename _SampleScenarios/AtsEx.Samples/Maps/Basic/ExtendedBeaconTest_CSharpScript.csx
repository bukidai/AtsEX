/*

全てのスクリプト（スクリプト言語で開発した車両・マッププラグイン、拡張地上子のスクリプトのどちらも）には

	IApp App { get; }
	IScenarioService ScenarioService { get; }
	IBveHacker BveHacker { get; }

が渡されています。


●全拡張地上子共通

以下の2種類の変数機能が提供されています。用途に合わせてお選びください。


◇拡張地上子に紐づいた変数

プラグインから BveHacker.ExtendedBeacons[(地上子の名前)].Passed イベントを通じて値を取得可能な変数です。
void SetVariable<T>(string name, T value) メソッドで書き込み、T GetVariable<T>(string name) メソッドで読み込みができます。

この変数は拡張地上子スクリプトを跨いで共有することはできません。
下記「プラグインに紐づいた変数」を使用するか、それが適切でない要件の場合はプラグインとして実装した方が良い可能性が高いです。


◇プラグインに紐づいた変数

プラグインのメインクラスから T GetExtendedBeaconPluginVariable<T>(string name) メソッドを通じて取得可能な変数です。
void SetPluginVariable<T>(PluginType pluginType, string pluginIdentifier, string name, T value) メソッドで書き込み、T GetPluginVariable<T>(PluginType pluginType, string pluginIdentifier, string name) メソッドで読み込みができます。

この変数は全ての拡張地上子スクリプトで共有されます。
これは、例えば「この地上子からこの地上子までは～～～の区間」のような設定を行い、その区間内に限ってプラグインから何らかの処理を加える……などの用途に向けたものです。
安易にグローバルな変数を定義することは、保守性を著しく損なうため推奨しません。使用の際はご注意ください。


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

MessageBox.Show("他列車の通過を検知しました。\n" +
	"\n" +
	$"拡張地上子の名前: {beaconName}\n" +
	$"拡張地上子の設置先軌道: {beaconTrack}\n" +
	"\n" +
	$"他列車の名前: {trainName}\n" +
	$"他列車の走行軌道: {trainTrack}\n" +
	$"他列車の進行方向: {trainDirection}", "C# スクリプトによる拡張地上子のサンプル");