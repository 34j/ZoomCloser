# ZoomCloser

A small application that obtains the number of participants in a Zoom meeting and automatically exits the Zoom meeting when the number of participants decreases, thereby preventing accidents caused by forgetting to leave.

![Sample Gif](https://github.com/34j/ZoomCloser/blob/master/ExampleFast.gif)

## Installation

Download the latest version from [Releases](https://github.com/34j/ZoomCloser/releases).
Once it is started, it will be automatically registered in the startup and will be launched at startup from the next time. To remove the automatic startup, disable the startup from the Task Manager.

## Requirements

NET Framework 4.8
Windows 10

## Features

- Gets the number of participants from the title of the window using Win32api. Then, when the number of participants in the Zoom meeting is more than 3 and less than (maximum number of participants)*p(random number between 0.4-0.6), it simulates the keyboard input of the standard exit shortcut (Alt+Q→Enter).
- Uses CoreAudioApi to mute Zoom's audio. (It's not a microphone.
- Displays the exit time and other information.

## Note on the license

The code in this repository is MIT licensed, but please note that some of the dependencies do not use the MIT license.


# ZoomCloser

Zoomミーティングの参加者数を取得し、参加者数が減ったときに自動でZoomミーティングを退出することにより、退出忘れによる事故を防止する小さなアプリケーション。

![Sample Gif](https://github.com/34j/ZoomCloser/blob/master/ExampleFast.gif)


## インストール

[Releases](https://github.com/34j/ZoomCloser/releases)から最新のものをダウンロードします。
一度起動すると自動でスタートアップに登録され、次回から起動時に起動します。自動的な起動を解除するにはタスクマネージャからスタートアップを無効化してください。

## 必要条件

.NET Framework 4.8
Windows10

## 機能


- Win32apiを用いてウィンドウのタイトルから参加者数を取得します。そして、Zoomミーティングの参加者数が3人以上かつ(最大参加者数)*p(0.4-0.6の乱数)以下になったとき標準の退出ショートカット(Alt+Q→Enter)のキーボード入力をシミュレートします。
- CoreAudioApiを用いてZoomの音声をミューとします。（マイクではありません。）
- 退出時刻などを表示します。

## ライセンスの注意事項

このリポジトリ内のコードはMITライセンスですが、一部の依存先はMITライセンスを採用していないことに注意してください。
