@echo off
rem 打包 net472 发布包。AssetStudio 是 submodule，路径相对本脚本。
setlocal

set ROOT=%~dp0
set SUB=%ROOT%AssetStudio

pyinstaller.exe -F "%SUB%\pkg.py" -n script.exe
set fname=%date:~2,2%%date:~5,2%%date:~8,2%
set fname=%fname: =0%
set OUTPUT=pkg-doctor-net472-%fname%
rmdir /S /Q %OUTPUT%
mkdir %OUTPUT%
set SRC=%SUB%\AssetStudioGUI\bin\Release\net472

robocopy %SRC% %OUTPUT% *.dll
robocopy %SRC% %OUTPUT% *.py
robocopy %SRC% %OUTPUT% *.exe
robocopy %SRC% %OUTPUT% *.config
robocopy %SRC%\x86 %OUTPUT%\x86 *.dll
robocopy %SRC%\x64 %OUTPUT%\x64 *.dll
robocopy dist %OUTPUT% *

endlocal
