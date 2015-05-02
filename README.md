CleverDock Direct2D experiment
==============================

This branch is an experiment to replace WPF by a custom Direct2D renderer.
The idea was to make use of Direct2D for higher performance rendering of the dock.
I have since stopped working on this since I discovered the reason for the performance problems
on WPF. The window was made to be full screen and that was extremely harmful to the WPF transparent
window rendering, as it did on the Direct2D renderer. Therefore I decided to fix the window size
on the WPF project and keep the awesome theming features.