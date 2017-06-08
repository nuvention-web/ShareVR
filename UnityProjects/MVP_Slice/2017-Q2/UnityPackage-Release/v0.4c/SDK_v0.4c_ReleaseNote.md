## ShareVR Unity Plugin v0.4c Release

### Targeting Environment
Unity Engine >= 5.6 with HTC Vive

### What's New in SDK v0.4c?
#### New Features
- **Backend Analytics Framework** - In ShareVR SDK v0.4c, we have a built-in log and video metadata framework setup which will automatically collect usage and configuration data to our AWS backend (DynamoDB).

- **No SteamVR Requirement** - (Experimental) ShareVR plugin's core feature no longer rely on SteamVR and thus does not require SteamVR in the solution. A new native API framework based on OpenVR and Unity's native VR support API is adopted.

- **No SteamVR Requirement** - (Experimental) Since SDK v0.4c, ShareVR plugin's core feature no longer rely on SteamVR and thus does not require SteamVR in the solution. A new native API framework based on OpenVR and Unity's native VR support API is adopted.

- **Fully Sealed DLL Solution** - ShareVR's core code is now sealed into a single DLL package which make it easier to distribute and helps protect code from being modified or copied.

- **Custom Camera Support** - If you want to use your own camera as the dedicated spectator camera, simply choose *Custom Camera* in the Camera Follow Regime and drag your camera gameobject to the target. This allows you to fully customize your own camera trajectory and interaction.

- **Camera Preview Panel Record Status Indicator** - In order for players to see the status of recording, we added a blinking red frame on the camera preview panel. It will blinking red during recording and stay invisible when not recording.

  <img src="./Imgs/v0.4c_PreviewRecStatus.gif" alt="Camera Preview Panel Status Indicator" width="300">

#### Bug Fix and Improvements
- Fixed a bug that will cause instability in Unity 5.6

###### ShareVR Team
6.7.2017
