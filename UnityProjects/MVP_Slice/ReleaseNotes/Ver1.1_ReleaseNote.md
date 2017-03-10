## ShareVR MVP Slice Project
Released version: **1.1**

Release date: **2/25/2017**

### Implemented Features

- Integrated [VRCapture](https://www.assetstore.unity3d.com/en/#!/content/75654) plugin to the project.
- Modified scene to apply VRCapture. Now when you click the *Start Record* button on the ShareVR Control Panel, it will automatically start to record
  2D video from the spectator camera. Once you click the *Stop Record* button, it will automatically encode and save the captured frames in to a MP4 video. The recorded video file is located in your Documents folder.
  For PC, it's in */Documents/VRCapture/*. For Mac, it should be in */Users/yourname/* where *yourname* will be your username.
- Spectator camera is now **200%** it's original scale!
- Recording status now gets displayed on the top-left corner of the ShareVR Live Feed panel in realtime. When recording, it will also show how many frames have been captured so far.

### Known Issues

- [360 Panorama recording doesn't work](https://github.com/nuvention-web/Team-H/issues/15)
- [360 Panorama recording cause huge frame drop](https://github.com/nuvention-web/Team-H/issues/15)

### TODO

- [YouTube One-click Sharing Feature](https://github.com/nuvention-web/Team-H/issues/16)
- Update SteamVR plugin 1.2.0 -> 1.2.1
- Inactivate the opposite button when started/stopped recording.
