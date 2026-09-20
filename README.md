# pkg-doctor

包体医生，Unity 游戏包体优化工具。

分析器逻辑已收编进 [AssetStudio_Tuanjie](https://github.com/SiMaLaoShi/AssetStudio_Tuanjie) 的 `--cli --analyze` 模式，
本仓库不再维护 AssetStudio 的源码副本（以 **git submodule** 引入），两个仓库各自独立迭代，不会有交叉修改。

## 同步 AssetStudio

```shell
git submodule update --init --recursive     # 首次拉取
git submodule update --remote AssetStudio   # 同步到最新
```

## 运行方式

```shell
AssetStudioGUI.exe --cli --analyze /path/to/game.apk
```

## 分析 Unity 游戏包体

支持 `.apk` / `.ipa` 文件，或已解包的游戏资源文件夹。

> AssetStudioGUI.exe --cli --analyze /path/to/game.apk

> AssetStudioGUI.exe --cli --analyze /path/to/game.ipa

> AssetStudioGUI.exe --cli --analyze /path/to/game/data/

输出 `pkg.tsv`（10 列）并调用 `pkg.py` / `script.exe` 生成 `pkg.html` 报告。

## 下载预编译版本

https://github.com/taptap/pkg-doctor/releases

## 也可手动编译

- 安装 [FBX SDK 2020.2.1](https://www.autodesk.com/content/dam/autodesk/www/adn/fbx/2020-1/fbx20201_fbxsdk_vs2017_win.exe)
- 若 SDK 装在非默认路径，设置环境变量 `FBX_SDK_DIR`
- 编译器需要 VS2022
- `git submodule update --init --recursive`
- 打开 `AssetStudio\AssetStudio.sln`，选择 Release 模式
- 生成 `AssetStudio\AssetStudioGUI\bin\Release\net6.0-windows\AssetStudioGUI.exe`

## 打包发布

`deploy_net6.bat`：编译产物 + `pkg.py` 打成的 `script.exe` 合并到 `pkg-doctor-net6-yymmdd` 目录。
