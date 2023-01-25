# AtsEX 本体のビルド手順

## 【！！注意！！】プラグインを開発するための手順ではありません

**AtsEX 本体のソースコードを編集するための手順です。AtsEX を利用してプラグインを開発する場合はこちらではなく[リリース](https://github.com/automatic9045/AtsEX/releases)を参照してください。**

## 手順

### 1. リポジトリをクローンする

もちろんご自身のGitHubアカウントでフォークしてからクローンされても構いません。

### 2. ローカルで参照しているアセンブリを配置する

以下のアセンブリはNuGetを介さずに参照しているため、手動での配置が必要です。

#### Mackoy.IInputDevice.DLL (Mackoy氏)

BVE本体（`BveTs.exe`）と同じフォルダ内にある`Mackoy.IInputDevice.DLL`を`AtsEx.PluginHost\LocalReferences\BveTs`内にコピーしてください。

#### Mackoy.XmlInterfaces.DLL (Mackoy氏)

BVE本体（`BveTs.exe`）と同じフォルダ内にある`Mackoy.XmlInterfaces.DLL`を`AtsEx.PluginHost\LocalReferences\BveTs`内にコピーしてください。

### 3. （既にソリューションをVisual Studioで開いている場合は）Visual Studioを再起動する

### 4. NuGet経由で参照しているアセンブリをダウンロードする

一度ビルドすると自動でダウンロードされます。