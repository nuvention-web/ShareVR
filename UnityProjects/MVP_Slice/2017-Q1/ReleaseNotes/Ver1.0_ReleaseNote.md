## ShareVR MVP Slice Project
Released version: **1.0**

Release date: **2/20/2017**

### Implemented Features
Version 1.0 implemented most basic features without any cloud or database support. It will help demonstrate our idea of easy and intuitive in-app/in-game capture from third person prospective.

<img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Scene1.png?raw=true" width="180">

- VR interaction features including teleport (VR only), interactable objects and world space UI canvas are adopted from SteamVR's interaction system example scene.

- Automatic SteamVR detection to support both VR and PC mode.

- Visible spectator camera that automatically follow and look at player.
  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Scene4.png?raw=true" width="180">

- Live camera capture feed display in the scene.

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Scene3.png?raw=true" width="180">

- Basic interactiable UI menu to turn on/off spectator camera follow and live feed feature

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Scene2.png?raw=true" width="180">

- I have created a very basic (and funny) player avatar in the demo. It's easy to subsitute that with any 3D mesh including Henry's idea of realistic personalized 3D body of the player himself/herself. Furthermore, it's possible to have a system that controls what avatar will appear in the captured video/image to enable player customization.

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Avatar1.png?raw=true" width="128">

### Known Issues

### TODO

- There's no recording functionality right now. The spectator camera will only update the live feed panel but won't record any actual frames. Will implement video recording and saving feature in the next release.

- Henry suggested while testing the demo that the spectator camera needs to be larger in order for people to actually notice the existance of it and the fact that it's following them.

- Inactivate the opposite button when start/stop recording.
