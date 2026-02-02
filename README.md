# Hex Builder

## Overview
Years ago, when I was first learning Unity, I attempted an mostly failed to make a game that was a mashup of the board games Catan, Carcassonne and Scrabble. I found that the most enjoyable part of that project, was the "Level Editor". This is a toy that mostly scratches that same itch, while being a much better made program, that could reasonably be extended to create other Hex Tile based games in the future.

<p align="center">
	<img src="images/ScreenshotA.jpg" alt="Screenshot A" height="250"/>
	<img src="images/ScreenshotB.jpg" alt="Screenshot B" height="250"/>
	<img src="images/ScreenshotC.jpg" alt="Screenshot C" height="250"/>
</p>

## Current Features
- Tools for manual terrain manipulation, using an area of effect.
- Placement of features on faces, edges or vertices
- Serialization / Deserialization of game data.
- particle effects - gpu or cpu particles, dependent on platform
- day/night cycle
- audio

## Dependencies
- **Unity 6.2**
  - Other Unity 6.x versions may work but are untested
- **Target Platforms**
  - Windows
  - macOS
  - WebGL (with limitations)

No external Unity packages or plugins are required beyond the standard Unity install.

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

## Roadmap
- **v0.1** - Working prototype
- **v0.2** - Visual polish and rendering improvements
- **v0.3** - Current - Landscape presets and procedural terrain generation
- **v0.4** - UI styling, better music, Smarter feature placement and more feature options.

## References
- Screen Based outline shader based on this tutorial by Digvijaysinh Gohil: https://www.youtube.com/watch?v=nc3a3THBFrg
- Service Locator based on a tutorial from Git-Ammend: https://www.youtube.com/watch?v=D4r5EyYQvwY
- Event Bus based on a tutorial from Git-Ammend: https://www.youtube.com/watch?v=4_DTAnigmaQ
- Tweening based on a tutorial by Sasquatch B Studios: https://www.youtube.com/watch?v=43o0FzU55V4