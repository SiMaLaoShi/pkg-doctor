## 修改

- AssetStudio 以 **git submodule** 形式引入（`AssetStudio/` → [SiMaLaoShi/AssetStudio_Tuanjie](https://github.com/SiMaLaoShi/AssetStudio_Tuanjie)），不再内嵌副本
- 升级为[FBX SDK 2020.2.1](https://damassets.autodesk.net/content/dam/autodesk/www/adn/fbx/2020)（AssetStudio需要）
- 编译器版本为Vs2022（AssetStudio需要）

## 同步 AssetStudio

```shell
git submodule update --init --recursive     # 首次拉取
git submodule update --remote AssetStudio   # 同步到最新
```

## 运行方式

```shell
AssetStudioGUI.exe --cli --analyze /path/to/game.apk
```

分析器（`ExportVizFile` / `pkg.tsv` 输出）已收编进 AssetStudio_Tuanjie 的 `--analyze` 模式，
本仓库不再维护 AssetStudio 的源码副本，因此不会有交叉修改。

# pkg-doctor

包体医生，Unity 及 Unreal 游戏包体优化工具。

# 分析 Unity 游戏包体

## 下载预编译版本
https://github.com/taptap/pkg-doctor/releases

## 也可手动生成 pkg-doctor.exe
- 安装 [FBX SDK 2020.2.1](https://www.autodesk.com/content/dam/autodesk/www/adn/fbx/2020-1/fbx20201_fbxsdk_vs2017_win.exe)
- 若 SDK 装在非默认路径，设置环境变量 `FBX_SDK_DIR`
- `git submodule update --init --recursive`
- 打开 `AssetStudio\AssetStudio.sln`
- 选择 Release 模式
- 生成 `AssetStudio\AssetStudioGUI\bin\Release\net472\AssetStudioGUI.exe`

## 分析 Unity 游戏 apk 或 ipa

> AssetStudioGUI.exe --cli --analyze /path/to/game.apk

> AssetStudioGUI.exe --cli --analyze /path/to/game.ipa

## 分析 Unity 游戏资源文件夹

> AssetStudioGUI.exe --cli --analyze /path/to/game/data/

# 分析 Unreal 游戏包体 [开发 ing]

## 生成 pkg-doctor.exe
- 进入 *Engine\Source\Programs* 目录
- mklink /D UnrealPakViewer /path/to/pkg-doctor/UnrealPakViewer
- 重新生成解决方案编译
