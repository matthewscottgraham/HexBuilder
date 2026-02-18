# Changelog

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