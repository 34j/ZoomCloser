[English](https://github.com/34j/ZoomCloser/blob/master/README.en.md)

# ZoomCloser

Windows software that prevents you from forgetting to leave the Zoom meeting.

![Sample](https://github.com/34j/ZoomCloser/blob/master/Example.png)

## install

Download the latest from Releases.[Releases](https://github.com/34j/ZoomCloser/releases)
一度プログラム起動すると、自動でスタートアップに登録され、次回からコンピュータの起動時に自動的に起動します。自動的な起動を解除するにはタスクマネージャからスタートアップを無効化してください。

## necessary conditions

-   .NET 6.0
-   Windows10.0.17763.0

## function

-   Get the number of participants in the Zoom meeting. Simulates the keyboard input of the default "Leave Meeting" shortcut key (Alt + Q → Enter) when the number of participants is less than or equal to (maximum number of participants) \* (constant percentage).
-   Mute the meeting audio with a Windows feature instead of a Zoom feature.
-   Record the meeting.
-   Record the time you joined the meeting and the time you left the meeting.

## License notes

Note that the code in this repository is MIT licensed, but some dependencies are not MIT licensed.
