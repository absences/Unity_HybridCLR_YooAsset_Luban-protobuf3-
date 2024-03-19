缝合了热更新工具HybridCLR 配表工具luban yooasset资源管理工具 

当然并不是完全的照搬
针对HybridCLR增加了HybridCLREditor.cs来一键构建Assemblies，增加monohot热更程序集、第三方类库HotfixMain的热更示例。

配表工具主要修改了protobuf 源码，具体使用参照ConfigManager.cs
解析bytes数据与管理配置

yooasset主要增加了工程外的rawfile 目录收集，针对原生资源不放在工程内的情况。

目的整合这些工具在实际使用中的情况

参考链接：
https://github.com/protocolbuffers/protobuf

https://github.com/focus-creative-games/luban

https://github.com/tuyoogame/YooAsset

https://github.com/EllanJiang/GameFramework

https://github.com/focus-creative-games/hybridclr

https://github.com/KumoKyaku/kcp

https://github.com/MirrorNetworking/Telepathy
