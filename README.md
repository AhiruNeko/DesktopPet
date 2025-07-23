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

注意: 所有桌宠资产(`.png`, `.gif`, `.mp3`等)都要放在`assets`内, 并且, 在你的主程序中, 为桌宠设置显示路径或播放路径时, 路径是以你的资产文件夹`assets`为根目录的.

### 1.2 文件及其配置

注意: 桌宠文件夹名称应与桌宠主程序名称一致.

你可以在`DesktopPet/settings.json`中通过设置`using`字段的值来设置运行时需要展示的桌宠. 例如:

```json
{
  "using": "Your_Pet"
}
```

### 1.3 桌宠配置文件

在`DesktopPet/UserPets/Your_Pet`中需包含一个桌宠配置文件`config.json`, 内容如下:

```json
{
  "name": "Your_Pet",
  "default": "default.png",
  "width": 200,
  "height": 200,
  "x": 1000,
  "y": 1000
}
```

其中:

- `name`必填, 且应与桌宠文件名一致
- `default`必填, 用于设置默认的桌宠路径
- 其余内容选填, 用于设置初始大小和位置

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

注意: 你的桌宠类应继承自`Desktop_Pet`, 并使用`@register`装饰器用于注册类.

## 三. 桌宠类可使用或访问的方法或属性

| 方法或属性                                  |         数据类型          | 参数类型  |        返回值类型        | 注释                  |
|:---------------------------------------|:---------------------:|:-----:|:-------------------:|---------------------|
| `self.mouse_event.mouse_x`             |         float         |   -   |          -          | 鼠标当前的 X 坐标          |
| `self.mouse_event.mouse_y`             |         float         |   -   |          -          | 鼠标当前的 Y 坐标          |
| `self.mouse_event.mouse_left_down`     |         bool          |   -   |          -          | 鼠标左键按下瞬间为 `True`    |
| `self.mouse_event.mouse_left_up`       |         bool          |   -   |          -          | 鼠标左键松开瞬间为 `True`    |
| `self.mouse_event.mouse_right_down`    |         bool          |   -   |          -          | 鼠标右键按下瞬间为 `True`    |
| `self.mouse_event.mouse_right_up`      |         bool          |   -   |          -          | 鼠标右键松开瞬间为 `True`    |
| `self.mouse_event.mouse_left_pressed`  |         bool          |   -   |          -          | 鼠标左键持续按下为 `True`    |
| `self.mouse_event.mouse_right_pressed` |         bool          |   -   |          -          | 鼠标右键持续按下为 `True`    |
| `self.mouse_event.mouse_move`          |         bool          |   -   |          -          | 鼠标移动时为 `True`       |
| `self.mouse_event.mouse_over_pet`      |         bool          |   -   |          -          | 鼠标悬停在桌宠区域为 `True`   |
| `self.mouse_event.no_event`            |         bool          |   -   |          -          | 当前无鼠标行为为 `True`     |
| `self.status.Path`                     |          str          |   -   |          -          | 桌宠当前的显示路径           |
| `self.status.SoundPath`                |          str          |   -   |          -          | 当前要播放的音频路径          |
| `self.status.PlaySound`                |         bool          |   -   |          -          | 当前是否要播放音效           |
| `self.status.Width`                    |          int          |   -   |          -          | 桌宠显示区域的宽度(像素)       |
| `self.status.Height`                   |          int          |   -   |          -          | 桌宠显示区域的高度(像素)       |
| `self.status.X`                        |         float         |   -   |          -          | 桌宠左上角的屏幕 X 坐标       |
| `self.status.Y`                        |         float         |   -   |          -          | 桌宠左上角的屏幕 Y 坐标       |
| `self.frame_timer`                     |          int          |   -   |          -          | 当前运行的总帧数            |
| `self.animetion_fps`                   |          int          |   -   |          -          | 帧率                  |
| `self.frame_interval`                  |         float         |   -   |          -          | 帧间隔时长               |
| `self.pre_status`                      | Backend.Status.Status |   -   |          -          | 上一帧的桌宠状态            |
| `self.status.set_path(path)`           |           -           |  str  |          -          | 设置桌宠的显示路径           |
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

### 4.1 导入相关库

```python
from Backend.utils import interaction
import Backend.Interaction_Result
```

### 4.2 自定义交互

#### 4.2.1 使用预设交互

在`Desktop_Pet`类中已经有一些预设的交互函数了, 分别是:

|            交互函数            | 参数类型 | 注释                            |
|:--------------------------:|------|-------------------------------|
|     `self.dragging()`      | -    | 使得桌宠可以被鼠标按住左键拖动               |
| `self.left_pressed(path)`  | str  | 当桌宠被鼠标左键按下时, 设置桌宠的显示路径为`path` |
| `self.right_pressed(path)` | str  | 当桌宠被鼠标右键按下时, 设置桌宠的显示路径为`path` |

你可以在你的桌宠类中直接注册这些交互:

```python
class Your_Pet(Desktop_Pet):
    ...
    @interaction()
    def left_pressed(self):
        return super().left_pressed("your_asset_path")
```

注意: 应使用`@interaction()`来将函数标记为桌宠的交互函数.

#### 4.2.2 自定义编写交互函数

除了使用预设的交互函数, 你也可以自己编写更加复杂的交互函数. 

每个交互函数都需要返回一个用于标识交互结果的值, 类型为`Interaction_Result`. 不同的交互结果所指代的含义有所不同:

| 交互结果                          | 含义                                       |
|-------------------------------|------------------------------------------|
| `Interaction_Result.Pass`     | 交互未命中(不发生), 继续下一个交互的判定                   |
| `Interaction_Result.SUCCESS`  | 交互发生, 不再进行其他交互的判定(即该交互不得与其他交互同时发生)       |
| `Interaction_Result.FAIL`     | 交互命中但是执行失败, 不再进行其他交互的判定(即该交互不得与其他交互同时发生) |
| `Interaction_Result.CONTINUE` | 交互发生, 但是继续进行其他交互的判定(即该交互可以与其他交互同时发生)     |

注意: 当且仅当所有注册的交互函数都未命中(即返回`Interaction_Result.PASS`)时才会将桌宠的显示路径设置为默认路径.

例如, 我们希望鼠标位于桌宠上时能把桌宠的显示路径设置为一个新的路径`new_path`并播放声音`sound.mp3`, 我们可以这么实现:

```python
class Your_Pet(Desktop_Pet):
    ...
    @interaction()
    def mouse_over_pet_interaction(self):
        if self.mouse_event.mouse_over_pet:
            self.status.set_path("new_path")
            self.play_sound("sound.mp3")
            return Interaction_Result.SUCCESS
        return Interaction_Result.PASS
```

#### 4.2.2 添加交互动画

在交互中我们可以添加动画. 例如, 我们希望用左键点击桌宠时桌宠能以30像素每秒向左移动3秒, 此时就需要在交互函数中添加这个动画:

```python
class Your_Pet(Desktop_Pet):
    ...
    @interaction()
    def mouse_over_pet_interaction(self):
        if self.mouse_event.mouse_left_down and self.mouse_event.mouse_over_pet:
            self.start_motion(self.animation)
            return Interaction_Result.SUCCESS
        return Interaction_Result.PASS

    async def animation(self):
        for _ in range(90):
            self.status.X -= 1
            await self.wait_frames(1)
```

注意: 动画函数`animation`应为异步函数, 并在交互函数中使用`self.start_motion`启动该动画. 动画的默认帧率为30fps.

#### 4.2.3 交互及其动画的优先级

可以用`@interaction(priority=n)`来设置交互优先级, 值越小越优先执行, 优先级默认为0. 

所有交互函数会按照优先级排序并依次检查是否触发, 并根据返回值判断是否要继续执行后续的交互函数. 例如:

- 当优先级高的交互函数被触发并返回`Interaction_Result.SUCCESS`时, 其他更低的优先级的函数将**不再被触执行**.
- 当优先级高的交互函数未触发并返回`Interaction_Result.PASS`时, 其他更低的优先级的函数将**继续被执行**.
- 当优先级高的交互函数被触发并返回`Interaction_Result.CONTINUE`时, 其他更低的优先级的函数**将继续被执行**.

在启动动画时, 也可以通过`self.start_motion(async_func, priority=n)`来设置动画优先级, 值越小越优先执行, 优先级默认为0.

动画优先级规则如下:

- 低优先级的动画可被高优先级的交互打断, 高优先级的动画无法被低优先级的交互打断.
- 低优先级的动画可被高优先级的动画打断.

### 4.3 自定义实时行为

可以通过覆写父类方法`self.monitor`来设置实时行为. 例如, 需要实时监控桌宠速度, 如果速度大于10就显示图片`too_fast.png`, 否则显示`default.png`:

```python
class Your_Pet(Desktop_Pet):
    ...
    def monitor(self):
        vx, vy = self.get_velocity()
        if vx ** 2 + vy ** 2 > 100:
            self.status.set_path("too_fast.png")
        else:
            self.status.set_path("default.png")
```

实时行为会每帧都执行, 并且享有最高优先级, 优先于一切交互的执行.
