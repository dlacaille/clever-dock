CleverDock
===========

A simple dock for Windows which features extensive theming capabilities and blurry 
reflections. The dock is fully compatible with Windows Vista/7/8/8.1.

This software is still an early alpha and may contain bugs.

![alt tag](https://raw.githubusercontent.com/ldom66/clever-dock/master/screenshot-0.2.0.jpg)

What's new in 0.2.0
-------------------
- Eliminated missing blurred icons.
- Enhanced window detection algorithms (no more missing windows hopefully).
- New theme engine! Loads uncompiled .xaml files under Themes/ folder which can be modified to your liking.
- New Dark version of the default "Metal" theme.
- Settings file now in .json format for better readability and performance.
- Bug fixes in the dock icons context menus.
- Better screen size detection and dock placement.
- Added an option to reserve a screen edge for the dock.
- Themes update instantly as the .xaml files are saved.

Download
--------
Version 0.2.0 Standalone - Vista/7/8/8.1:<br />
[CleverDock-v0.2.0.zip](https://github.com/ldom66/clever-dock/releases/download/v0.2.0/CleverDock-v0.2.0.zip)

How to install
--------------
1. Download the zip above
2. uncompress it somewhere on your computer *- C:\CleverDock\ For example -*. 
3. Execute the file named **CleverDock.exe**

Troubleshooting
---------------
#####The dock worked once but now it crashes everytime i try to open it.
> There may be an issue with your configuration file. Try to erase config.json and restart the dock.

#####I closed the dock from task manager / The dock crashed and now I have no task bar.
> Start CleverDock using the task manager (in the File menu and then Start). Then right-click on CleverDock and click "Exit CleverDock"
> If that did not work, open the task manager and close explorer, then use the file menu to start explorer.exe again.

#####CleverDock can not start, it crashes as soon as I try to open it.
> You may not have .Net Framework 4.5. Try to install it using [This link](http://www.microsoft.com/fr-ca/download/details.aspx?id=30653)
> If this issue persists, try to start the [debug version](https://github.com/ldom66/clever-dock/releases/download/v0.2.0/CleverDock-v0.2.0-Debug.zip) and contact me with the details of the crash.
