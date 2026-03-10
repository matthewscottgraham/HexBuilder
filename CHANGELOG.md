# Changelog

## V0.8
* Changed title and company name for build.
* Fixed saving and loading of features and waterfalls.
* When a face feature is placed on a river, or vice/versa, it will remove the existing feature.
* Added more settlement models for variation.
* Added a tool cursor that changes colour if current tile is invalid for tool.
* Improved the water shaders and models, and added vegetation to the river tiles.
* Hex top models made more interesting
* Added grass to the wilderness tiles.
* Added falling dust particles
* Added nightime lights
* the cursor hover no longer works when dragging or rotating the camera

## V0.7
* Visual tweaks, such as fog, better day night, colour temps, new colour palette
* Added splash screen
* Bug fixes for screenshot mode
* Updated icons and map previews
* Updated UI look and feel
* Added ocean tile features
* Added feature picker so you have more control
* Added an eraser tool

## V0.6
* The camera can now 'focus' on the highlighted hex by pressing the F key.
* The camera can now be rotated by holding down the right mouse button, or by halding Alt on windows / option on mac.
* Refactored tools. They are no longer an interface, but base / derived classes. Most tools now use the radius slider.
* Added modes for tools, that can be Toggle, Add or Subtract. These are on a per tool basis.
* Bug fixing for tools, tweens
* features are no longer rendered underwater
* Added additional screenshot tools for modifying the Field of View, Depth of field, time of day and capturing the UI or not.
* created a custom UI component that acts like radio buttons but looks like buttons. I didnt like the built in one.
* model and rendering tweaks
* screenshot camera is now a separate camera so that the game cameras state is not modified while taking screenshots
* tweaked camera rig feel
* Added an action that is invoked when the active tab in the menubar changes
* Abstracted out the file handling so that I can have a WebGL specific controller
* Removed additional save slots and redid the map chooser window. It now shows map preview images.
* Added a screensaver mode that animates the camera around and looks awesome