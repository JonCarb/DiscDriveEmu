# DiscDriveEmu 💿
<a id="readme-top"></a>
### About The Project
A simple GUI that reads discs with retro console files and executes them with the correct emulator. A GameCube game opens with Dolphin, a PS3 game opens with RPCS3, and so on. Some games may not run well from disc, so DiscDriveEmu includes a built-in installation mode that lets you use a modern storage drive while still recreating the nostalgic feel of inserting a disc to play. 

![DDE Main Screen](Images/DDEMainScreen.png?raw=true)

## Supported Emulators
| Emulators    | Console  |Compatible Discs               | Can Play Real Disc | File Types Supported                      |
|--------------|----------|------------------------------|--------------------|-------------------------------------------|
| Dolphin      | Gamecube |Mini-DVD, DVD, DVD-DL, Blu-Ray| No                 | ISO, CISO, GCZ                            |
| Dolphin      | Wii      |DVD, DVD-DL, Blu-Ray          | No                 | ISO, RVZ, WBFS, WIA, WAD                  |
| Cemu         | Wii U    |Blu-ray                       | No                 | WUX, WUA, WUD, RPX                        |
| Xemu         | Xbox     |DVD, DVD-DL, Blu-Ray          | No                 | XISO                                      |
| Xenia        | Xbox 360 |DVD-DL, Blu-Ray               | No                 | ISO, XEX                                  |
| PCSX2        | PS2      |DVD, DVD-DL, Blu-Ray          | YES                | ISO, (original file format)               |
| RPCS3        | PS3      |DVD-DL, Blu-Ray               | No                 | Decomp ISO, PS3_DISC.SFB with PS3_GAME Dir|

## Notes
* This app is currently Windows only!
* Installation only works for single file discs, real PS2 discs and PS3 discs containing multiple directories and files cannot currently be installed.
* Wii games running off of disc experience major slowdowns; installation may be needed.
* A good disc drive is needed for a smooth user experience
* While not needed, a Blu-ray drive is HIGHLY recommended. You can get away with a regular DVD drive and its related speeds, but you'll miss out on games over 8.5 GB.
* A controller is recommended for a better user experience

## Source Code Installation
1. Install .NET (8 or above) https://dotnet.microsoft.com/en-us/download 
2. Clone the Repo https://github.com/JonCarb/DiscDriveEmu.git
3. Open it in your desired IDE and have fun
   
## Installation
1. Install .NET (8 or above) https://dotnet.microsoft.com/en-us/download
2. Download the link on the release page and run the .exe
* Release page: https://github.com/JonCarb/DiscDriveEmu/releases
3. Look at Usage below before inserting a disc

## Usage
* Please note that this program doesn't come with the needed emulators. You will need to download and set them up yourself.
* If help is needed for emulation installation, EmuDeck (Windows Version) is worth checking out https://www.emudeck.com/

![Settings Page](Images/SettingsPaths.png?raw=true)

* Once the emulators are able to run on their own, they can now be paired with the app. This process is simple as you only need to add and select where the emulator exe is in your system. 
* Once the paths are correct and visible, you can now insert a disc into your connected disc drive. 
* This is done through the setting page and to get to it, you can click the settings button or press B/Circle From your controller.
* For a streamlined process, make sure the disc and you're legally obtained games are burned with the supported file types for their respective console/emulator.

## Resources used
* CD Model Used - Very Simple CD- Disc by Blender3D: https://skfb.ly/6zvYJ
* PS/Xbox Buttons Icons - ConsoleMods: https://consolemods.org/wiki/Category:Button_Icons

## Special Thanks
-  Without your hard work of the amazing emulation community, literally none of this project would be possible so thank you!
### Inspirations - Check them out!: 
* https://github.com/LMauricius/EmuDiscer
* https://github.com/wisnia87r/The-Orange-Disk-
