@echo off
rem 分析 Unity 包体：内部走 AssetStudio 的 --cli --analyze。
"%~dp0AssetStudio\AssetStudioGUI\bin\Release\net6.0-windows\AssetStudioGUI.exe" --cli --analyze %*
