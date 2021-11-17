[English](https://github.com/34j/ZoomCloser/blob/master/README.ar.md)

# ZoomCloser

Windows software that prevents you from forgetting to leave the Zoom meeting.

![Sample Gif](https://github.com/34j/ZoomCloser/blob/master/ExampleFast.gif)

## install

[Releases](https://github.com/34j/ZoomCloser/releases)から最新のものをダウンロードします。
一度起動すると自動でスタートアップに登録され、次回から起動時に起動します。自動的な起動を解除するにはタスクマネージャからスタートアップを無効化してください。

## Requirements

-   .NET Framework 4.8
-   Windows10 64bit

## function

-   Get the number of participants in the Zoom meeting. When the number of participants is less than or equal to (maximum number of participants) \* (constant percentage), the standard exit shortcut (Alt + Q → Enter) keyboard input is sent.
-   Mute the meeting audio on the Windows side.
-   Record the meeting.
-   Record the entry time and exit time.

## License notes

Note that the code in this repository is MIT licensed, but some dependencies are not MIT licensed.
