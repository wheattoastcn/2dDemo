Assets/
├── _Project/                        # 所有项目自研资产
│   ├── Scenes/                      # [程序/TA 共用] 场景文件
│   │   ├── Levels/                  #   - 关卡场景，建议一人一个场景，避免冲突
│   │   └── Test/                    #   - 程序测试场景 / TA 表现测试场景
│   │
│   ├── Scripts/                     # [引擎程序主负责]
│   │   ├── Runtime/                 #   游戏运行时逻辑
│   │   │   ├── Core/               #     框架：GameManager, PoolManager, 事件/消息等
│   │   │   ├── Gameplay/           #     玩法：塔、敌人、子弹、关卡等
│   │   │   ├── UI/                 #     UI逻辑
│   │   │   └── Utilities/          #     工具类、扩展方法
│   │   └── Editor/                 #     编辑器工具、Inspector扩展
│   │
│   ├── Art/                         # [技术美术主负责] 美术展示资产
│   │   ├── Materials/              #     材质球
│   │   ├── Shaders/                #     Shader文件/Shader Graph
│   │   ├── Textures/               #     贴图（可能需要按图集再分子文件夹）
│   │   ├── Sprites/                #     2D精灵切图与图集
│   │   │   ├── Towers/            #       防御塔相关
│   │   │   ├── Enemies/           #       敌人
│   │   │   ├── Environment/       #       地图、背景、装饰
│   │   │   ├── Projectiles/       #       子弹/飞行道具
│   │   │   └── UI/                #       UI用图
│   │   ├── Animations/             #     动画片段与Animator Controller
│   │   ├── VFX/                    #     特效预制体（粒子、动画特效）
│   │   └── Profiles/              #     后处理Profile、2D Renderer设置等
│   │
│   ├── Prefabs/                     # [程序+TA 共用] 预制体是关键分工界面
│   │   ├── Towers/                  #     塔的预制体
│   │   ├── Enemies/                 #     敌人预制体
│   │   ├── Projectiles/            #     子弹
│   │   ├── Environment/            #     场景布景、Tilemap节点等
│   │   └── UI/                     #     UI面板、HUD元素
│   │
│   ├── Data/                        # [程序主负责，TA可参与填充]
│   │   └── ScriptableObjects/       #     配置数据：塔属性、敌人波次、关卡数据
│   │
│   ├── Audio/                       # [共用，可由TA或音效人员管理]
│   │   ├── SFX/
│   │   └── Music/
│   │
│   └── Settings/                    # [程序/TA 共用] URP/2D Renderer等全局设置
│       └── Renderer/               #      2D Renderer Data等
│
├── Plugins/                         # 第三方插件（保持原样）
├── StreamingAssets/                 # 需要按原样打包的文件（如配置json等）
└── Resources/                       # 尽可能不用，改用Addressables（见后文）