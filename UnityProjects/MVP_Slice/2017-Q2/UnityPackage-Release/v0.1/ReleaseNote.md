## ShareVR Unity Plugin v0.1
Welcome to ShareVR Unity SDK v0.1. Thanks for taking time trying out our plugin. The quick start tutorial will help you quickly setup our plugin. If you have any question, please don't hesitate to contact us at team@sharevr.io

### Getting Started
*ShareVR requires the SteamVR plugin loaded in your project*

#### Import ShareVR and Load the Example Scene
- Make sure you have SteamVR plugin in your project
- Import the ShareVR Unity Plugin (currently distributed as a separate UnityPackage)
- Under Assets/ShareVR/Scenes/ open the example scene
- Hit play then you should be able to start testing
  - By default, the recording can be started and stopped by pressing the X key in your keyboard. We provided other mapping options including some that use the Vive Controllers. You can change them in the ShareVR gameobject.
  - **Press X key to start record**. Once you started recording, a ShareVRCameraRig game object will be instantiated with a visible camera model (you can toggle the model display on/off and change the scale of the model in the ShareVR gameobject). You should be able to see the camera started following you and the little green light on the camera indicates it's recording now.
  - Feel free to move around and teleport. The ShareVRCameraRig smoothly follows you as you move.
  - **Press X again to stop recording**. A video file will be created in the "ShareVR" folder under current Windows user's Documents folder.
  - You can record as many times as you want and each session will create a separate video file.


#### Apply ShareVR to Your Own Game
Our goal is make our ShareVR Unity plugin easy-to-use yet still adaptive and customizable so that it could fit well in to your own application. Here's an example of how apply ShareVR to the SteamVR InteractionSystem example scene.

- Open SteamVR interaction system example scene located at: Assets/SteamVR/InteractionSystem/Samples/Scenes/Interactions_Example
- To use ShareVR, you just need to proceed with the two step below.
  1.  Under Assets/ShareVR/Prefabs/, drag the ShareVR gameobject to anywhere in the scene hierarchy.
  2.  Click the ShareVR gameobject in the scene hierarchy. Reference the target gameobject that you want the spectator camera to track and record by draging the target gameobject into "Player Head Game Object".
  3. (optional) If you are using the avatar ("ShareVR/Show Player Avatar" checked), you need to make sure the main camera ignores the avatar so that the avatar body won't occludes with your main camera. Simply find your main camera (in the InteractionSystem example, it's located at Player/SteamVRObjects/VRCamera) and uncheck "IgnoreInMainCamera" in the Culling Mask list.
- Now you are done with the quick setup. Hit play and you should be able to record just like our example scene.


Note that We have some additional options listed below if you want to customize the plugin to better fit into your game. We kept all the available options in the ShareVR gameobject to make it easier to find. Below are the available options with description:
- *Show Camera Model* - toggles whether or not a 3D model of the recording camera will be shown. Useful if you want to see where the camera is.
- *Camera Model Scale* - If you are using the camera model, this adjusts the scale of the model so it won't be too small or big.
- *Show Player Avatar* - toggles whether or not a player avatar will be shown in the scene. We currently have a generic free avatar for you to use if you don't have an avatar for the player. Note that you can easily replace it to any 3D model you like. Please contact us if you have any specific need for the avatar.
- *Avatar Scale* - adjusts the scale of the avatar
- *Avatar Offset* - adjusts the offset between the avatar to your main player (the Player Head Game Object). Change this if the avatar appears to be off.
- *Toggle Recording Input* - use this to change your desired key mapping that triggers the start / stop recording event.
- *Player Head Game Object* - reference your main target gameobject here and the ShareVRCameraRig will track and record it. You cannot leave this empty as it will cause error.
- *Player Hand Transforms* - (optional) if you are using the avatar and would like the hand of the avatar to track your Vive Controllers, reference your hand gameobject here otherwise it's fine to leave it blank.
- *Show Debug Message* - use this to track the status of the plugin. Debug messages will be shown in your Unity Editor Console
- *ShareVR Video Format* - change it if you want to record with a different resolution or frame rate. The default is 960x540 at 30 FPS to balance between video quality and performance. We are working hard optimizing our infrastructure to reduce performance cost.

The ShareVR Team
4/8/2017
