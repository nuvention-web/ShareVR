## ShareVR MVP Slice Project Tutorial
Current version: **1.0**

Last update date: **2/20/2017**

Testing Status:

| Platform      | Deployed Method | Status  |
|:-------------:|:---------------:|:-----:|
| PC Mode       | Executable      | **Pass** |
| PC Mode       | Unity Editor    | **Pass** |
| VR Mode (SteamVR + Vive)  | Executable      | *Pending* |
| VR Mode (SteamVR + Vive)  | Unity Editor    | *Pending* |

### Setup Environment and Testing MVP Project
There are two ways to test the project. Please start with method A as it's the most simple way to start the demo. Adam and Henry, please try to test method B after you tried method A. Method B will open the entire project from Unity which allows you to look into all the details and codes.

#### Method A - Executable

1. I have compiled an executable of the project using Unity. Please follow [this link](https://github.com/nuvention-web/Team-H/tree/MVP-Slice/UnityProjects/MVP_Slice/Executables) and make sure download **both the *.exe file and the data folder**.
2. Once you have downloaded the file, simple launch it and you should be able to play.

#### Method B - Unity Editor

1. The entire project is uploaded [here](https://github.com/nuvention-web/Team-H/tree/MVP-Slice/UnityProjects/MVP_Slice/ShareVR). Try download the entire **ShareVR** folder.
2. Once you downloaded it. Launch Unity Engine and open the project by finding the *ShareVR* folder you have just downloaded. It will probably take some time to open the project.
3. Once it's finished, you should have the *MVP_Slice* scene loaded.

  <img src="https://github.com/nuvention-web/Team-H/blob/MVP-Slice/UnityProjects/MVP_Slice/Tutorial/Images/OpenScene1.png" width="128">
  
  If not, you can open the main scene in the *Project* tab.
  
  <img src="https://github.com/nuvention-web/Team-H/blob/MVP-Slice/UnityProjects/MVP_Slice/Tutorial/Images/OpenScene2.png" width="256">

4. Once the scene is loaded, you can simple hit play button at the near the top center of your editor to launch the play mode.

### How to Control and Interact
- PC Mode (Keyboard and Mouse)

  - Look around by holding the *right click* of your mouse.
  - Move around using *W/S/A/D* on your keyboard
  - Interact with objects and UI menu using the *left click* of your mouse
  
- VR Mode (SteamVR + HTC Vive) 

  - Move and look around use your body
  - Interact with objects and UI menu using your Vive controller by pulling the *Trigger*.
  - Use your *touchpad* to teleport

### Implemented Features
#### Version 1.0

Version 1.0 implemented most basic features without any cloud or database support. It will help demonstrate our idea of easy and intuitive in-app/in-game capture from third person prospective.
  
<img src="https://github.com/nuvention-web/Team-H/blob/MVP-Slice/UnityProjects/MVP_Slice/Tutorial/Images/Scene1.png" width="180">
  
- [x] VR interaction features including teleport (VR only), interactable objects and world space UI canvas are adopted from SteamVR's interaction system example scene.

- [x] Automatic SteamVR detection to support both VR and PC mode.

- [x] Visible spectator camera that automatically follow and look at player.

  <img src="https://github.com/nuvention-web/Team-H/blob/MVP-Slice/UnityProjects/MVP_Slice/Tutorial/Images/Scene4.png" width="180">

- [x] Live camera capture feed display in the scene.

  <img src="https://github.com/nuvention-web/Team-H/blob/MVP-Slice/UnityProjects/MVP_Slice/Tutorial/Images/Scene3.png" width="180">

- [x] Basic interactiable UI menu to turn on/off spectator camera follow and live feed feature

  <img src="https://github.com/nuvention-web/Team-H/blob/MVP-Slice/UnityProjects/MVP_Slice/Tutorial/Images/Scene2.png" width="180">
  
- [ ] I have created a very basic (and funny) player avatar in the demo. It's easy to subsitute that with any 3D mesh including Henry's idea of realistic personalized 3D body of the player himself/herself. Furthermore, it's possible to have a system that controls what avatar will appear in the captured video/image to enable player customization.

  <img src="https://github.com/nuvention-web/Team-H/blob/MVP-Slice/UnityProjects/MVP_Slice/Tutorial/Images/Avatar1.png" width="128">
