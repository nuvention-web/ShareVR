
Setup SteamVR and Create Minimal VR Project on HTC Vive
=============================
This tutorial helps you walk through the process of setting up a minimal SteamVR project for HTC Vive.

Install Unity3D
---------------------
You want to install the most recent version of Unity3D. You can download it from [this link](https://store.unity.com/download?ref=personal).

_Hint: Unity has a built-in lightweight IDE called **MonoDevelop** for editing and debugging scripting languages including C# and JavaScript.
When installing Unity3D, it will try to install Visual Studio for you as well but you don't really need it.
Simply uncheck "Visual Studio Community Tools" option._

Import SteamVR Plugin
---------------------
Now that you have Unity installed, let's try to setup our first SteamVR project with the following steps.
*I am assuming you have tried the [Roll-a-ball tutorial](https://unity3d.com/learn/tutorials/projects/roll-ball-tutorial) and skipped 
some basics already.*

1. Open Unity (register for an account if prompted or simply use offline mode)
2. Create a new project
3. Specify a containing folder where you want to store your project. Note that Unity will create a subfolder with the name of your project
and store all the files in that folder.
4. Open *Window -> Asset Store* or simply hit *Ctrl + 9*. You will see a new asset store window opened. Now search *SteamVR*,
find "SteamVR Plugin" and click download. 
5. Once download is finished, you will see a *Import Unity Package* window poped up. Click *All* and then click *Import*. It it ever
show a *API Update Required* window, simply click *I Made a Backup. Go Ahead!*.
6. When finished, you will see a *SteamVR_Settings* window pop up, click *Accept All* and you are all done with importing SteamVR.

Now you will see a folder named "SteamVR" under your Assets folder. All the related files will be stored under SteamVR. Note if you ever see the *SteamVR_Settings* window pop up again reminding you to disable Unity3D splash screen, simple hit *Ignore* to ignore it.

Testing SteamVR Interaction Example
---------------------
Now that you have SteamVR imported to your project, you can test the SteamVR interaction system example provided by the plugin.

1. In *Project* tab on your main screen, browse into *Assets/SteamVR/InteractionSystem/Samples/Scenes/* and double click to open *Interaction_Example* scene.
2. Now you will see the scene loaded in your project. You have two way to test this example:

  a. **If you are testing it with HTC Vive** Start SteamVR through Steam. And go back to Unity and hit run. Now put on Vive you will be able to walk around and interact with the environment.
  
  b. **If you are testing it without VR HMD** Simply hit run in Unity and you should be able to interact with the environment using your keyboard and mouse (hold right-click to look around, left click to interact, W/S/A/D to move).
