## ShareVR MVP Slice Project
Released version: **1.1**

Release date: **2/25/2017**

### Implemented Features

- Integrated [VRCapture](https://www.assetstore.unity3d.com/en/#!/content/75654) plugin to the project.
- Modified scene to apply VRCapture. Now when you click the *Start Record* button on the ShareVR Control Panel, it will automatically start to record
  2D video from the spectator camera. Once you click the *Stop Record* button, it will automatically encode and save the captured frames in to a MP4 video.
  For PC version, the recorded video file is located in */ShareVR_v1_1_Data/VRCapture*. For Mac version, it should be in a subfolder named *VRCapture* under */ShareVR_v1_1.app/Contents*.

### Known Issues

- 360 Panorama recording feature works in the VRCapture plugin demo scene but not in our scene. Lauch recording in 360 panorama mode will cause Unity Editor to collapse.
  The reason why this is happening is unknown right now. Need to get it fixed.
  
### TODO

- The *Share* button in the ShareVR control panel currently does nothing. Will look into [YouTube Data API](https://developers.google.com/youtube/v3/) or similar
  APIs to implement one button YouTube upload feature.
  
- The recording API (VRCapture) is not very efficient and will cause obvious frame drop especially when recording 360 panorama video.
  We need to have conprehensive knowledge of how it works and carefully redesign/optimize the process.
