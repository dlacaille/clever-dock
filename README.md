CleverDock
===========

A simple dock for Windows which features extensive theming capabilities and blurry 
reflections. The dock is fully compatible with Windows Vista/7/8/8.1.

***I have stopped maintaning this project. I lost the motivation since a lot of the features I wanted (fluid physics for icons a la Kibadock, window-peek on icons hover, overall performance) were not possible on WPF. I also end up losing a lot of time trying to comprehend XAML layouts and intricacies and doing interop. I am currently trying to re-create this project using C++ and DirectComposition. Hopefully we will have our modern, fast and beautiful Windows dock soon enough!***

This software is still an early alpha and may contain bugs.

![alt tag](https://raw.githubusercontent.com/ldom66/clever-dock/master/screenshot-0.2.0.jpg)

What's new in 0.4.0
-------------------
- Major optimization of the window: the dock is now running at 60FPS!
- Windows demanding attention now bounce on the dock, even when hidden.
- Added settings for tweaking X and Y padding of icons.
- Dock does hide anymore when the mouse is between icons, or in a context menu.

What's new in 0.3.0
-------------------
- Optimized overall dock performance for smoother animations.
- Added collapsing animations when icons are dragged.
- Better theme loading error management.
- Fixed a crash when a window does not have a file name.
- Added the AutoHide feature.
- Fixed a bug with the *Reserve screen space* checkbox.

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
Version 0.4.0 Standalone - Vista/7/8/8.1:<br />
[CleverDock-v0.4.0.zip](https://github.com/ldom66/clever-dock/releases/download/v0.4.0/CleverDock-v0.4.0.zip)

Version 0.3.0 Standalone - Vista/7/8/8.1:<br />
[CleverDock-v0.3.0.zip](https://github.com/ldom66/clever-dock/releases/download/v0.3.0/CleverDock-v0.3.0.zip)

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
> You may not have .Net Framework 4.5. Install it using [this link.](http://www.microsoft.com/fr-ca/download/details.aspx?id=30653)<br />

#####I can not restore/minimize/close a specific application on the dock.
> The app you can't use may have been started as an administrator. The dock must have administrator rights to control these processes. Simply exit CleverDock then start it again by right clicking and selecting *Run as administrator*. You can also check the box *Run this program as an administrator* in the executable's properties window in the *Compatibility* tab.
