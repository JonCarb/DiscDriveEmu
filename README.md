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
| RPCS3        | PS3      |DVD-DL, Blu-Ray               | Sorta (read below) | Decomp ISO, PS3_DISC.SFB with PS3_GAME Dir|

## Notes
* This app is currently Windows only!
* Installation only works for single file discs, real PS2 discs and PS3 discs containing multiple directories and files cannot currently be installed.
* Wii games running off of disc experience major slowdowns; installation may be needed.
* A good disc drive is needed for a smooth user experience
* While not needed, a Blu-ray drive is HIGHLY recommended. You can get away with a regular DVD drive and its related speeds, but you'll miss out on games over 8.5 GB.
* A controller is recommended for a better user experience

## Real PS3 Disc
* Before trying to use real PS3 Discs, make sure you followed the steps in the RPCS3 Emulator itself
* (https://wiki.rpcs3.net/index.php?title=Help:Dumping_PlayStation_3_games#Booting_games_directly_from_a_Blu-ray_Drive_on_Windows/Linux/MacOS)
* Currently, support for PS3 Discs is in a weird state because this application DOES recognize it as a PS3 game and correctly boots RPCS3, but RPCS3 itself currently doesnt support booting discs through the command line.
* A disappointing but current workaround is:
1. Insert the PS3 disc as normal
2. When RPCS3 boots up, it will try and fail the initial command, so it will "freeze". Don't rush it.
3. Once the games library appears in RPCS3, find the correct game you inserted and make sure its thumbnail is loaded.
4. The game should give you the option to restart; click the game, click Restart, and patiently wait.
5. After a short bit, the game should start running, and you can now play off your disc!
* When RPCS3 releases better support for discs, this app will be updated.

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
