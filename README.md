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

```json
{
  "using": "Your_Pet"
}
```

## 二. 创建并注册你的桌宠类

在`Your_Pet.py`中创建并注册你的桌宠类, 注意: 类名称应该与主程序名称一致.

### 2.1 导入必要模块

```python
from Backend.Desktop_Pet import Desktop_Pet
from Backend.registry import register
```

### 2.2 创建并注册类

```python
@ register
class Your_Pet(Desktop_Pet):

    def __init__(self):
        super().__init__()
```

注意: 你的桌宠类应继承自`Desktop_Pet`, 并使用`@register`装饰类用于注册类.

## 三. 桌宠类可使用或访问的方法或属性

| 方法或属性                                  |         数据类型          | 参数类型  |        返回值类型        | 注释                  |
|:---------------------------------------|:---------------------:|:-----:|:-------------------:|---------------------|
| `self.mouse_event.mouse_x`             |          int          |   -   |          -          | 鼠标当前的 X 坐标          |
| `self.mouse_event.mouse_y`             |          int          |   -   |          -          | 鼠标当前的 Y 坐标          |
| `self.mouse_event.mouse_left_down`     |         bool          |   -   |          -          | 鼠标左键按下瞬间为 `True`    |
| `self.mouse_event.mouse_left_up`       |         bool          |   -   |          -          | 鼠标左键松开瞬间为 `True`    |
| `self.mouse_event.mouse_right_down`    |         bool          |   -   |          -          | 鼠标右键按下瞬间为 `True`    |
| `self.mouse_event.mouse_right_up`      |         bool          |   -   |          -          | 鼠标右键松开瞬间为 `True`    |
| `self.mouse_event.mouse_left_pressed`  |         bool          |   -   |          -          | 鼠标左键持续按下为 `True`    |
| `self.mouse_event.mouse_right_pressed` |         bool          |   -   |          -          | 鼠标右键持续按下为 `True`    |
| `self.mouse_event.mouse_move`          |         bool          |   -   |          -          | 鼠标移动时为 `True`       |
| `self.mouse_event.mouse_over_pet`      |         bool          |   -   |          -          | 鼠标悬停在桌宠区域为 `True`   |
| `self.mouse_event.no_event`            |         bool          |   -   |          -          | 当前无鼠标行为为 `True`     |
| `self.status.Path`                     |          str          |   -   |          -          | 桌宠当前显示的图像路径         |
| `self.status.SoundPath`                |          str          |   -   |          -          | 当前要播放的音频路径          |
| `self.status.PlaySound`                |         bool          |   -   |          -          | 当前是否要播放音效           |
| `self.status.Width`                    |          int          |   -   |          -          | 桌宠显示区域的宽度(像素)       |
| `self.status.Height`                   |          int          |   -   |          -          | 桌宠显示区域的高度(像素)       |
| `self.status.X`                        |          int          |   -   |          -          | 桌宠左上角的屏幕 X 坐标       |
| `self.status.Y`                        |          int          |   -   |          -          | 桌宠左上角的屏幕 Y 坐标       |
| `self.frame_timer`                     |          int          |   -   |          -          | 当前运行的总帧数            |
| `self.animetion_fps`                   |          int          |   -   |          -          | 帧率                  |
| `self.frame_interval`                  |         float         |   -   |          -          | 帧间隔时长               |
| `self.pre_status`                      | Backend.Status.Status |  -    |         -           | 上一帧的桌宠状态            |
| `self.status.set_path(path)`           |           -           |  str  |          -          | 设置桌宠图像路径            |
| `self.status.set_width(width)`         |           -           | float |          -          | 设置桌宠宽度              |
| `self.status.set_height(height)`       |           -           | float |          -          | 设置桌宠高度              |
| `self.status.set_x(x)`                 |           -           | float |          -          | 设置桌宠左上角的 X 坐标       |
| `self.status.set_y(y)`                 |           -           | float |          -          | 设置桌宠左上角的 Y 坐标       |
| `self.wait_frames(frame_count)`        |           -           |  int  |          -          | 异步等待指定帧数时间          |
| `self.seconds_to_frames(seconds)`      |           -           | float |         int         | 将秒数转换为帧数            |
| `self.frames_to_seconds(frames)`       |           -           |  int  |        float        | 将帧数转换为秒数            |
| `self.get_velocity()`                  |           -           |   -   | tuple[float, float] | 返回桌宠位移速度 `(dx, dy)` |
| `self.play_sound(path)`                |           -           |  str  |          -          | 播放指定路径的音效           |
| `self.get_screen_size()`               |           -           |   -   |   tuple[int, int]   | 获取屏幕尺寸（宽, 高）        |

## 四. 自定义桌宠行为
