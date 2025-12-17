# Hex Builder

## Overview
**Hex Builder** is a Unity-based hex tile building sandbox.  
It is designed primarily as a **tool and technical foundation** rather than a traditional game, with an emphasis on grid logic, tile placement, and extensibility.

The project can be used as:
- A standalone creative toy
- A reference implementation for hex-grid systems in Unity
- A starting point for building hex-tile-based games or editors

<p align="center">
	<img src="images/ScreenshotA.jpg" alt="Screenshot A" height="250"/>
	<img src="images/ScreenshotB.jpg" alt="Screenshot B" height="250"/>
	<img src="images/ScreenshotC.jpg" alt="Screenshot C" height="250"/>
</p>

## Motivation
This project grew out of an earlier Unity prototype that attempted to combine mechanics from **Catan**, **Carcassonne**, and **Scrabble**. While the original game was unsuccessful, the most compelling part turned out to be the level editor and tile manipulation systems.

Hex Builder is a focused reimplementation of that idea:
- Smaller scope
- Cleaner architecture
- Built with reuse and iteration in mind

Rather than forcing a game design on top, this project intentionally leaves space for experimentation and extension.

## Dependencies
- **Unity 6.2**
  - Other Unity 6.x versions may work but are untested
- **Target Platforms**
  - Windows
  - macOS
  - WebGL (with limitations)

_No external Unity packages or plugins are required beyond the standard Unity install._

## Project Structure
This repository is a complete Unity project.

Notable areas of interest:
- `Assets/Scripts/App/Main.cs` — Bootstrapper for the entire application. Start here.
- `Assets/Scenes/Game/Game.cs` — Bootstrapper for the Game. All game logic stems from here.

## How to Run

### Open in Unity
1. Clone or download this repository.
2. Open **Unity Hub**.
3. Select **Open Project** and choose the repository root.
4. Open the main scene and press **Play**.

### Play a Build
Prebuilt binaries and a WebGL version are available on Itch.io:

**Itch.io page:** https://msgraham.itch.io/hexbuilder

### Controls
- Click-Drag to move the camera
- Left Click to use the selected tool on the face/edge/vertex under the mouse.

## Current Features
- Cubic Coordinates and a radial playspace.
- Tools for manual terrain manipulation, using an area of effect.
- Placement of features on faces, edges or vertices
- Serialization / Deserialization of game data.

## Roadmap
- **v0.1** - Current - Working prototype
- **v0.2** - Visual polish and rendering improvements
- **v0.3** - Landscape presets and procedural terrain generation
- **v0.4** - Quality-of-life improvements, user options etc

## References
- Screen Based outline shader based on this tutorial by Digvijaysinh Gohil: https://www.youtube.com/watch?v=nc3a3THBFrg
- Service Locator based on a tutorial from Git-Ammend: https://www.youtube.com/watch?v=D4r5EyYQvwY
- Event Bus based on a tutorial from Git-Ammend: https://www.youtube.com/watch?v=4_DTAnigmaQ
- Tweening based on a tutorial by Sasquatch B Studios: https://www.youtube.com/watch?v=43o0FzU55V4