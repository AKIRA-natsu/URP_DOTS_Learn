# 修改记录

**Version: 0.0.0**

* 建立项目
  * URP项目
  * [ECS Setup](https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/getting-started-installation.html)
  * [DOTS Setup](https://docs.unity3d.com/Packages/com.unity.entities@0.17/manual/install_setup.html)
* 文件夹
  * Res：资源
  * Plugins：插件
  * Scripts：脚本
    * Runtime：运行时
    * Editor：编辑器下代码
    * Test：测试
* 添加通用代码
  * Attribute
    * SelectionPop：常量数据的序列化
      ![img](./Assets/Res/ReadmeLinks/Version_0.0.0/1.png)
      ![img](./Assets/Res/ReadmeLinks/Version_0.0.0/2.png)
  * Common
    * GameData：数据
    * ISystem：System接口
    * Singleton：CSharp单例
  * Editor
    * GameDataWindow：后续添加内容，因为常量类写在程序集里面，方便后面就算不知道位置在那里也能修改和查看
      ![img](./Assets/Res/ReadmeLinks/Version_0.0.0/3.png)

**Version: 0.0.1**

* 日志管理
  * LogConfig
    * 日志颜色
    * 日志详细扩展
    * 是否打日志的bool选择
    * 配置的路径：Resources/Configs/LogConfig.Asset
      ![](./Assets/Res/ReadmeLinks/Version_0.0.1/1.png)
      ![](./Assets/Res/ReadmeLinks/Version_0.0.1/2.png)
  * LogSystem
    * 富文本日志
    * 双击日志跳转设置

**Version: 0.0.4**

* 模块下载管理
  * GitObject
    * 对应从Github页面抓取的json类
  * ModuleDownloadConfig（存在局限）
    * 面板+Window获得路径
      ![](./Assets/Res/ReadmeLinks/Version_0.0.4/1.png)
    * git路径必须带上 tree/[分支名]
    * 日志打印路径
      ![](./Assets/Res/ReadmeLinks/Version_0.0.4/3.png)
    * Window显示Path所有节点
      ![](./Assets/Res/ReadmeLinks/Version_0.0.4/2.png)
    * Check检查文件是否存在
    * Download下载，按照git的目录下载到对应目录（图片以自己分支EventSystem为例）
      ![](./Assets/Res/ReadmeLinks/Version_0.0.4/4.png)
      ![](./Assets/Res/ReadmeLinks/Version_0.0.4/5.png)
    * 加载以后LoadModule变成DeleteModule，可以一次性删除文件（在原来文件没有被移动的情况）
      ![](./Assets/Res/ReadmeLinks/Version_0.0.4/6.png)
    * 遗留问题（未解决）：下载之后提示InvalidOperationException: Stack empty.的报错，并且够不会isloaded的勾不会勾上
    * 还需要更多测试！

**Version: 0.1.1**

* AB包管理
  * AB包面板 Unity官网面板
    ![](./Assets/Res/ReadmeLinks/Version_0.1.1/1.png)
  * AssetBundleConfig
    ![](./Assets/Res/ReadmeLinks/Version_0.1.1/2.png)
    * Simulation：编辑器下读取AB模拟运行，非勾选状态下直接AssetDataBase.LoadAsset
    * Use Web Request Test：给未来服务器下载准备，用WebRequest Url读取测试AB包
    * paths：AB包路径（暂停StreamingAssets下）
  * 简单测试PC和Android平台运行

