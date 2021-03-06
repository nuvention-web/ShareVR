## ShareVR MVP Slice Project
Released version: **1.2 Beta**

Release date: **3/4/2017**

### Implemented Features

- SteamVR plugin updated 1.2.0 -> 1.2.1
- Now ShareVR Control Panel will only show a single start or stop recording button based on capture status.
- Floor and wall material now looks brighter than before.

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Wall1.png?raw=true" width="320">

- Replaced avatar with a animated carton character created by [Supercyan (Free on Unity Asset Store)](https://www.assetstore.unity3d.com/en/#!/content/79870).

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Avatar2.png?raw=true" width="180">
- Added a spotlight that follows player to provide proper lighting.
- Implemented basic motion animation including automatic parametrized idle -> walk -> run transitions.

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Animation1.png?raw=true" width="320">
- Implemented head gaze feature. Now the avatar's head pitch angle will adjust as the player look up/down. When player look left/right, the entire body including head moves.

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Avatar3.png?raw=true" width="180">
  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/Avatar4.png?raw=true" width="180">
- Added avatar offset control. Avatar height offset can be controlled by the **-** and **=** key on your keyboard (on the left of your backspace key).
- **(Beta Feature)** Head gaze and hand coordination are now updated through the animator Invert Kinematics (IK) system.
  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/HandIK.gif?raw=true" width="180">
  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/MVP_Slice/ReleaseNotes/Images/GazeIK.gif?raw=true" width="180">

### Known Issues

- [360 Panorama recording doesn't work](https://github.com/nuvention-web/Team-H/issues/15)
- [360 Panorama recording cause huge frame drop](https://github.com/nuvention-web/Team-H/issues/15)

### TODO

- [x] Test VR Mode on HTC Vive.
- [ ] Test Avatar Hand Invert Kinematics feature with Vive controllers. Fix any offset or alignment issues.
- [YouTube One-click Sharing Feature](https://github.com/nuvention-web/Team-H/issues/16)
- [Implement and test hand integration with HTC Vive controllers](https://github.com/nuvention-web/Team-H/issues/18)
