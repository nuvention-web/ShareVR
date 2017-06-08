# ShareVR (Team H)
Welcome! This is repository of team H. We have 6 team members from different background that are all passiante about VR.

Our team members include (in alphabetical order):
- Adam Muhammad
- Chen Chen
- Ellie Burgess
- Henry Keyser
- Qiqi Zhang (Vivian)

## Project Website
Please visit our [website](https://sharevr.io)

## Q2 Deliverable (SDK v0.4c)
Hello and welcome to Team H (ShareVR)'s final MVP project testing guide! Below are instructions to help you test our MVP project. Hope you enjoy our demo!

### Download and Launch ShareVR MVP Demo
The MVP deliverable is a compiled Unity project in the form of Windows standalone application (Windows only). Below are instructions for downloading and launching ShareVR.

1. Download the most recent standalone app package: [PC version](https://github.com/nuvention-web/Team-H/tree/submission/UnityProjects/MVP_Slice/2017-Q2/StandaloneApp/Windows). Please make sure you have downloaded **both the *.exe file and the *_Data folder**.
2. Once you finished download, simply launch the *.exe* file and you should be able to play.

### Control and Interact
ShareVR is configured to work in both VR and non-VR PC environment. The control, however, is different.

If you are using HTC Vive and have SteamVR installed, ShareVR will automatically open SteamVR and enter VR mode. Otherwise, ShareVR will be in PC mode.

- PC Mode (Keyboard and Mouse)
  - Hold the *right click* of your mouse and move to look around.
  - Use *W/S/A/D* key on your keyboard to move
  - Interact with objects and UI menu using the *left click* of your mouse
  - *Note that if you move your mouse without holding the right click, you can still interact with the scene and the avatar will look around accordingly.*

- VR Mode (SteamVR + HTC Vive)
  - Move and look around use your body
  - Interact with objects and UI menu using the *Trigger* button in your Vive controllers.
  - Use your Vive controllers' *touchpad* to teleport

### How to Capture
- **Start Recording** : You can start to capture a session by press the **X** button on your keyboard.
- **During recording** : You can do anything you want, move around, interact with the objects, etc.
- **Finish recording** : Stop the capture session by press the **X** button on your keyboard again.

You will notice the recording has started by the blinking red frame surrounding the camera preview panel. Once you finished a capture session, a video file will be created under your Documents/ShareVR folder.

<img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/Q1-Deliverable/Images/Q1-Deliverable-Scene3.png?raw=true" width="300">
### Q1 Deliverable Feature Overview
- ShareVR adopted some interactive contents from SteamVR's interaction system.

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/Q1-Deliverable/Images/Q1-Deliverable-Scene1.png?raw=true" width="320">

  The scene has six tables with each one featuring standard interactive objects used by SteamVR. You are able to interact with these objects in both PC and VR mode.

- There is a 3D camera model that's hovering in the air and will follow smoothly you as you move. The camera is a visualization of our in-game spectator camera which will record third person view.

- In the center of the scene you will see a giant screen called "ShareVR Live Feed". This is a live preview of what our spectator camera sees right now.

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/Q1-Deliverable/Images/Q1-Deliverable-Scene2.png?raw=true" width="320">

- We adopted a cute avatar and implemented basic invert kinematics (IK) system to allow gaze and hand tracking. Gaze tracking is used in PC mode to make the avatar always look at your mouse. Hand tracking is used in VR mode to ensure avatar's hands are always synced with player's Vive controllers.

  <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/Q1-Deliverable/Images/Q1-Deliverable-GazeIK.gif?raw=true" width="320">
    <img src="https://github.com/nuvention-web/Team-H/blob/submission/UnityProjects/Q1-Deliverable/Images/Q1-Deliverable-HandIK.gif?raw=true" width="280">

- We implemented an in-app interactive UI (ShareVR Control Panel) to control the capturing and are planning to upgrade that with a more intuitive voice controlled interface in future iterations.

- Captured game play will be encoded and saved as a .MP4 video.
