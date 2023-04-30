# HFramework
![](https://img.shields.io/badge/version-2.0.2-blue)

## 目录

- HFramework
  - FrameworkEntry
  - Audio
  - Data
  - Event
  - Fsm
  - GlobalMono
  - Input
  - ObjectPool
  - ReferencePool
  - Procedure
  - Resource
  - Scene
  - UI
  - Singleton
  - Utils
- ThirdPersonController
  - ThirdPersonCam
  - ThirdPersonController

## 项目规范

### FrameworkEntry
- 框架启动函数 `HEntry.Init()`

### Data
- Excel文件夹：`Asset/Excel/`
- Excel生成类文件夹：`Asset/Scripts/Generate/Data/InfoClass/`
- Excel生成数据容器文件夹：`Asset/Scripts/Generate/Data/Container/`
- Excel生成二进制数据文件夹：`StreamingAssets/Binary/`

### Event
- 新增自定义事件需添加进ClientEvent枚举中

### UI
- 面板预制体文件夹：`Asset/Resource/UI/`
- 控件预制体文件夹（可重定向）：`Asset/Resource/UI/Controls/`
- 控件自动生成代码路径（可重定向）：`Asset/Scripts/Generate/UIControls`
- 控件关键字（可重定向）：`_`
- 重定向路径Editor面板：`Tools -> UI -> 生成控件设置`