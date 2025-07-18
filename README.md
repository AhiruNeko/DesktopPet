# 可自定义的桌宠框架

可自定义资产与Python脚本创建自定义桌宠

## 一. 构建自定义桌宠项目

### 1.1 项目结构

在`UserPets`文件夹中放置你的桌宠文件夹`Your_Pet`, 你的桌宠文件夹结构应该如下:

```text
Your_Pet/
├── assets/      // 桌宠资产文件夹
├── config.json  // 桌宠配置文件
└── Your_Pet.py  // 桌宠主程序
```

### 1.2 文件及其配置

注意: 桌宠文件夹名称应与桌宠主程序名称一致.

你可以在`settings.json`中通过设置`using`字段的值来设置运行时需要展示的桌宠. 例如:

```setting.json
{
    "using": "Your_Pet"
}
```
