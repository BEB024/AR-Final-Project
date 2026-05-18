# Broken Hoops — AR Basketball Game

## Project Description

**Broken Hoops** is an Augmented Reality basketball game built in **Unity 6 / Unity 6000.3.8f1** using **AR Foundation**, **XR Simulation**, and the **Android SDK**.

The project goes beyond a basic AR object placement demo by combining markerless plane placement, marker-based image tracking, basketball throwing mechanics, scoring, timers, UI panels, customization options, and multiple game modes.

The player can place a basketball hoop in AR either by tapping a detected floor plane or by pointing the camera at a tracked basketball marker. A basketball spawns in front of the player, and the player can grab, drag, flick, and throw the ball toward the hoop.

---

## Core Idea

The game is designed around a simple but expandable AR basketball experience:

- The player places a hoop in the real or simulated AR environment.
- A basketball spawns in front of the camera using a socket anchor.
- The player throws the ball toward the hoop.
- If the ball passes through the score trigger, the score increases.
- The ball respawns after scoring or missing.
- Different game modes change the goal, challenge, and difficulty.

---

## Main Features

### Markerless Hoop Placement

The player scans the environment and taps on a detected horizontal plane to place the hoop.

This uses:

- `AR Plane Manager`
- `AR Raycast Manager`
- `ARInputHandler`
- `ARPlaneHoopPlacement`

The markerless system allows the player to freely place the basketball hoop in the scanned AR space.

---

### Marker-Based Hoop Placement

The player can point the camera at a tracked image marker to spawn the hoop.

This uses:

- `AR Tracked Image Manager`
- `XR Reference Image Library`
- `SimulatedTrackedImage` for Unity Editor testing
- `ARImageTracker`

The marker-based system makes the hoop appear when the correct image is detected.

---

### Basketball Throwing

The ball is spawned in front of the player using a `BallSocketAnchor` attached to the AR camera.

The player can click, drag, and release to throw the ball. On Android, this becomes touch/flick input.

Main components:

- `BasketballController`
- `BallSpawnManager`
- `BallGrabThrowInput`
- `Rigidbody`
- `Sphere Collider`

The ball uses Unity physics so it can travel through the scene, collide, fall, score, and respawn.

---

### Score Detection

The hoop prefab contains an invisible trigger collider positioned inside the rim.

When the basketball passes through the trigger:

- The ball is marked as scored.
- The score increases.
- The hoop can play feedback.
- The ball is destroyed or disabled.
- A new ball respawns.

Main script:

- `ScoreTrigger`

---

## Game Modes

### Sandbox Mode

The player can shoot freely with no timer.

### Time Trial Mode

The player chooses a time limit and tries to score as many baskets as possible before the timer ends.

### Flight Style Mode

A harder version of Time Trial where the ball trajectory is randomized after throwing. This makes the ball harder to control and adds challenge.

### Trick Shot Mode

A challenge-based mode focused on rewarding unique or difficult shots. This mode can be expanded with combo scoring, special shot recognition, and trick-shot challenges.

---

## UI Features

The game includes a gameplay canvas with multiple panels:

```text
GameplayCanvas
├── ScoreText
├── TimerText
├── HUDPanel
├── SetupPanel
├── CountdownPanel
├── EndGamePanel
├── BasketballSelectionPanel
├── BackboardColorPanel
└── SettingsPanel
```

### UI Functions

- Shows the current score.
- Shows the timer or current game mode.
- Displays placement instructions.
- Confirms hoop placement.
- Shows a countdown before the game starts.
- Displays end-game results.
- Allows basketball selection.
- Allows backboard color selection.
- Allows settings control.

---

## Customization Features

### Basketball Selection

Players can switch between different basketball models or materials.

Example variants:

- Default basketball
- Black basketball
- Custom colored basketball
- Special themed basketball

### Backboard Color Selection

Players can change the hoop backboard color using the UI.

### Jukebox Music

A jukebox button allows random music from a playlist to play during gameplay.

---

## Main Scripts

### AR Scripts

```text
ARInputHandler.cs
ARPlaneHoopPlacement.cs
ARImageTracker.cs
```

### Gameplay Scripts

```text
GameSessionSettings.cs
BrokenHoopsGameManager.cs
HoopManager.cs
BallSpawnManager.cs
BasketballController.cs
BallGrabThrowInput.cs
ScoreTrigger.cs
GameplayUIManager.cs
```

### Optional / Extended Scripts

```text
PlayerRadiusMonitor.cs
TrickShotChallengeManager.cs
JukeboxManager.cs
```

---

## Recommended Scene Hierarchy

```text
AR_Game
├── AR Session
├── XR Origin (AR)
│   └── Camera Offset
│       └── Main Camera
│           └── BallSocketAnchor
│
├── EventSystem
├── XR Interaction Manager
├── GameSessionSettings
├── AR Input Handler
│
├── ARManagers
│   ├── ARPlaneHoopPlacement
│   └── ARImageTracker
│
├── GameManagers
│   ├── BrokenHoopsGameManager
│   ├── HoopManager
│   ├── BallSpawnManager
│   └── BallGrabThrowInput
│
├── GameplayCanvas
│   ├── ScoreText
│   ├── TimerText
│   ├── HUDPanel
│   ├── SetupPanel
│   ├── CountdownPanel
│   ├── EndGamePanel
│   ├── BasketballSelectionPanel
│   ├── BackboardColorPanel
│   └── SettingsPanel
│
└── Directional Light
```

---

## Important AR Setup

### XR Origin

The `XR Origin (AR)` must include:

```text
XR Origin
Input Action Manager
AR Plane Manager
AR Raycast Manager
AR Tracked Image Manager
```

### AR Plane Manager

Used for markerless placement.

Recommended settings:

```text
Detection Mode: Horizontal
Plane Prefab: AR Default Plane
```

### AR Raycast Manager

Used to detect where the player taps on scanned AR planes.

### AR Tracked Image Manager

Used for marker-based placement.

Recommended settings:

```text
Serialized Library: BrokenHoopsReferenceImageLibrary
Max Number Of Moving Images: 2
Tracked Image Prefab: None
```

The tracked image prefab is left empty because the project uses script-based spawning and parenting.

---

## Reference Image Library

The project uses an `XR Reference Image Library` containing the basketball marker image.

Example setup:

```text
Name: Basketball
Texture: Basketball marker texture
Specify Size: Enabled
Physical Size: 1m x 1m for simulation
```

For real Android testing, the physical size should match the printed marker size.

Example:

```text
20 cm x 20 cm printed marker = 0.2 x 0.2 meters
```

---

## Unity Editor Simulation

The project can be tested inside Unity using XR Simulation.

### Requirements

- XR Simulation enabled in XR Plug-in Management.
- A custom XR Simulation environment.
- A `SimulatedTrackedImage` object inside the active XR Environment prefab.
- The simulated image must use the same texture as the Reference Image Library.
- The AR Session and XR Origin transforms should be reset.

### Important Transform Reset

If image tracking does not work, reset these objects:

```text
AR Session
Position: 0, 0, 0
Rotation: 0, 0, 0
Scale: 1, 1, 1
```

```text
XR Origin (AR)
Position: 0, 0, 0
Rotation: 0, 0, 0
Scale: 1, 1, 1
```

```text
Camera Offset
Position: 0, 0, 0
Rotation: 0, 0, 0
Scale: 1, 1, 1
```

This is important because incorrect AR origin transforms can break tracking alignment and make tracked image content appear missing or offset.

---

## Controls

### Unity Editor

```text
Right Mouse + WASD = Move around XR Simulation environment
Mouse click / drag = Tap or throw
```

### Android

```text
Touch tap = Place hoop on detected plane
Touch drag / flick = Throw basketball
Camera view = Detect marker image
```

---

## Gameplay Flow

### Markerless Flow

```text
Scan floor
↓
Tap detected plane
↓
Hoop spawns
↓
Confirm placement
↓
Countdown starts
↓
Ball spawns
↓
Player shoots
↓
Score or miss
↓
Ball respawns
```

### Marker-Based Flow

```text
Point camera at basketball marker
↓
AR Tracked Image Manager detects marker
↓
Hoop spawns on marker
↓
Confirm placement
↓
Countdown starts
↓
Ball spawns
↓
Player shoots
↓
Score or miss
↓
Ball respawns
```

---

## Prefabs

### Basketball Prefab

The basketball prefab starts as a simple imported 3D model and then receives Unity components.

Recommended components:

```text
BasketballPrefab
├── Mesh Filter
├── Mesh Renderer
├── Rigidbody
├── Sphere Collider
├── XR Grab Interactable
├── BasketballController
├── Trail Renderer optional
└── Audio Source optional
```

### Hoop Prefab

The hoop prefab should contain:

```text
HoopPrefab
├── Backboard
├── Rim
├── Net optional
├── ScoreTrigger
└── HoopController
```

The `ScoreTrigger` object should have:

```text
Box Collider
Is Trigger = true
ScoreTrigger script
```

---

## Known Issues and Fixes

### Markerless works but marker-based does not

Check:

```text
AR Session transform reset
XR Origin transform reset
AR Tracked Image Manager enabled
Reference Image Library assigned
SimulatedTrackedImage exists in the active XR Environment
SimulatedTrackedImage texture matches the library texture
Physical size values match
```

### Serialized Library shows None during Play Mode

This can happen because AR Foundation converts the serialized library into a runtime library during Play Mode. The important test is whether the tracked image is detected and content spawns.

### Ball does not spawn

Check:

```text
BallSpawnManager has SocketAnchor assigned
Basketball prefab is assigned
BasketballController is on the basketball root
Hoop placement was confirmed
GameManager started the game
```

### Throwing does not work

Check:

```text
BallGrabThrowInput has ARInputHandler assigned
BallSpawnManager assigned
Main Camera assigned
Game is running
Ball is not already released
```

---

## Technologies Used

```text
Unity 6000.3.8f1
AR Foundation
XR Simulation
XR Interaction Toolkit
Android SDK
ARCore XR Plugin
TextMeshPro
Blender for 3D models
```

---

## Project Goal

The goal of **Broken Hoops** is to demonstrate a complete AR gameplay experience using Unity 6. The project combines:

```text
AR plane detection
AR image tracking
Physics-based throwing
Interactive UI
Game modes
Customization
Scoring
Simulation and Android-ready AR setup
```

This makes the project stronger than a basic AR placement demo because it includes both technical AR requirements and actual gameplay design.

---

## Future Improvements

Possible future additions:

```text
Better trick-shot detection
Online leaderboard
More basketball skins
More hoop styles
Sound effects for scoring and bouncing
Particle effects for perfect shots
Improved Android touch controls
Multiplayer score challenges
More polished UI animations
```

---

## Final Notes

This project was developed as an AR final project using Unity 6. The main focus is to create a playable AR basketball experience that supports both markerless and marker-based interaction while using physics, UI, and gameplay systems to create a complete game loop.
