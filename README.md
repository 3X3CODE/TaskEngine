# TaskEngine
TaskEngine is an Among Us mod that enhances your task experience in Among Us. While the mod supports swapping vanilla tasks, the highlight feature is the ability of adding your own **Custom Tasks** into the game.<br>
>[!WARNING]
>This mod is not known to work with **LevelImposter**. However we are researching the possibility.

## Features
- Swap vanilla tasks <br>
- Add custom tasks <br>
<br>
*More features coming soon!*

> [!IMPORTANT]
> **This mod is still in BETA** <br>
> Issues are expected. Fixes are coming soon. <br>
> Any and all feedback is accepted. You can DM me on [Discord](https://discord.com/users/1044631490165755914), or create an issue.

## Installation Guide
- Unzip the **.zip** file in the latest release. <br>
- Copy and paste the Mod file (**TaskEngine.dll**) and the **CustomTasks** folder into your `BepInEx/plugins` folder. <br>
- The mod will automatically generate your **taskConfig.xml** file once you join a practice session. <br>
- See the guides below on how to use the mod's features. <br>

## Usage
TaskEngine provides an easy interface to swap your tasks and create your own custom tasks. <br>
It is reccomended that you use the **UnityExplorer** mod to analyze the game. <br>
>[!IMPORTANT]
>Both of the features require the user to have,
>Basic knowledge of C# and Unity.
>An IDE ( for Custom Tasks ).

**The Unity version required is `2022.3.44f1`**

In each CustomTask and ReplaceTask section in the config, you can set the edit to be active or inactive.
You also have a master active statement to deactivate the entire config.

You can add as many CustomTasks or tasks to replace by simply adding more **CustomTasks** or **ReplaceTasks** blocks into the config.

### Swap Tasks
- **Ship:** this is the map you want your changes to happen. Values are, *Skeld, Mira, Polus, Airship, Fungle*
- **firstTask:** first task you want to swap. Takes a TaskType value.
- **secondTask:** second task, the task you want to swap the first task with. Also takes a TaskType value.
- **firstConsole:** the name of the GameObject that holds the first task Console on the map.
- **secondConsole:** the name of the GameObject that holds the second task Console on the map.
- **overrideSteps:** default false. allows you to change the number of steps in each task and if you want to show the task step. *eg- Shoot Asteroids(0/20)*
- **firstStep:** the step count you would want the first task to have.
- **secondStep:** the step count you would want the second task to have.
- **showFirst:** shows the step of the first task.
- **showSecond:** shows the step of the second task.

### Custom Tasks
To make a custom task, you must have Minigame prefabs and Minigame scripts.
TaskEngine will load them into the game for you.
Please note that the mod only supports two GameObjects and two scripts for now.
By default, the first script is added to the first GameObject, and vice versa for the second. A second script will not exist if you dont have a second GameObject specified. Unfortunately we don't have a way of changine these settings yet.

#### Script making
- Start by creating a new Class Library project and importing the required references. The project does not need a BepInEx plugin class. Example here.
- The main minigame class must be **Public** and must derive from **Minigame**. The main minigame class is **necessary**.
- You can include a second class if your Minigame depends on it, otherwise you will not need to specify this.
- The second class must also be public.
- Make sure your two scripts are well contained so they can find eachother automatically. By default, TaskEngine places the second GameObject under the main one, keep in mind that it is inactive. (You can see a great example of this from how the example task Minigame class automatically finds the script, **Submarine**)
- If you want to analyze the example task further, you can use a tool like **dnSpy** to analyze the `ShootSubmarines.dll` file, or using its source code.
- The unity project for the custom minigame can also be found here.
- Once you are done writing the scripts, be sure to compile it into a **.dll** file.
- Make sure to build your prefabs into AssetBundles, I reccomend using the unity package **AssetBundle Browser** for this.

Once you have your compiled scripts and the unity assetbundles, put them both inside the `CustomTasks` folder.

TaskEngine automatically handles loading your task into the game, however you need to add a new custom task definition inside the **taskConfig.xml** file.

- **Task:** you need to specify your task name here. It is the name shown in the task menu.
- **scriptName:** the name of the main Minigame script. Be sure it's the **Class name** and not the **dll** name.
- **monobehaviourName:** this is the name of your second script. You can leave this empty if you dont have one.
- **firstGameObject:** the name of the main Prefab GameObject. This is the GameObject your first class gets added to.
- **secondGameObject:** the name of the second GameObject. This is also optional.
- **bundleName:** the name of the assetbundle that contains the Prefabs.
- **taskType:** a unique id given to your task. By default, ascending from 130 is fine however, 160 or higher is reccomended.
- **replaceTask:** the TaskType of the base game task you want to replace.
- **consoleName:** the name of the console you want your Custom Task to be part of.
- **maxStep and showStep:** the settings of the maximum step your task has and if it needs to show it.
- **firstScale:** the scale of the firstGameObject.
- **secondScale:** the scale of the secondGameObject.
- **Ship:** the map you want your task to be part of. Ensure the task has the correct TaskType and the Console Name to prevent conflicts.

Need any support regarding these features? reach out to me on Discord. We will be creating a discord server for these mods soon.

> [!WARNING]
> This mod requires **[BepInEx](https://builds.bepinex.dev/projects/bepinex_be)** and **[Reactor](https://github.com/nuclearpowered/reactor)** to function properly. <br>

>[!NOTE]
>This mod is licensed under GPL v3.0. <br>
>[Copyright (C) 2026 3X3C](https://github.com/3X3CODE/TaskEngine/blob/master/LICENSE) <br>

<sub>
  This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.
</sub>
