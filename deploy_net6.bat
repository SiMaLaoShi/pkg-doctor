@echo off
rem 打包 net6 发布包。AssetStudio 是 submodule，路径相对本脚本。
setlocal

set ROOT=%~dp0
set SUB=%ROOT%AssetStudio

pyinstaller.exe -F "%SUB%\pkg.py" -n script.exe
set fname=%date:~2,2%%date:~5,2%%date:~8,2%
set fname=%fname: =0%
set OUTPUT=pkg-doctor-net6-%fname%
rmdir /S /Q %OUTPUT%
mkdir %OUTPUT%
set SRC=%SUB%\AssetStudioGUI\bin\Release\net6.0-windows

rem 整目录递归拷贝：net6 产物含 runtimes/、Dependencies/、x86/、x64/ 子目录
robocopy %SRC% %OUTPUT% /E /NFL /NDL /NJH /NJS
robocopy dist %OUTPUT% *

endlocal
