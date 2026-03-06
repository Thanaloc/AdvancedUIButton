# AdvancedUIButton

A professional UI button system for Unity uGUI. Drop-in replacement for Unity's built-in Button with extended state machine, multi-graphic transitions, ripple effect, and ScriptableObject-based styling.

## Features

- 7 button states: Normal, Highlighted, Pressed, Selected, Focused, SelectedHighlighted, Disabled
- Multi-graphic support: animate Background, Label, Icon independently with per-graphic duration and easing
- 9 easing functions: Linear, EaseIn/Out/InOut, BackOut, ElasticOut, BounceOut, and more
- Scale animation on the button RectTransform
- Ripple effect with dynamic size calculation and pool-based zero-allocation spawning
- 4 interaction modes: Standard, Toggle, Radio, Hold
- UIButtonToggleGroup for mutual exclusion between Radio buttons
- ScriptableObject styles: define once, apply anywhere, update globally
- 5 built-in presets: Primary, Secondary, Danger, Outline, Ghost
- Full audio support with pitch randomization
- ignoreTimeScale support for pause-safe UI
- Custom Inspector with tabbed layout, tooltips, and one-click ripple setup
- Assembly Definition files for clean compilation isolation
- Unity 6 / URP / HDRP / Built-in compatible

## Requirements

- Unity 2022.3 LTS or later (Unity 6 recommended)
- TextMeshPro
- No other dependencies

## Installation

1. Import the package into your project
2. Add `AdvancedUI/Advanced UI Button` to any GameObject via Add Component
3. Add your Graphic components (Image for background, TextMeshPro for label)
4. Assign them in the Graphics tab of the Inspector

## Quick Start

### Creating a button

```
1. Create a UI GameObject (right-click Hierarchy -> UI -> Empty)
2. Add Component -> AdvancedUI -> Advanced UI Button
3. Add a child Image (Background) and a child TextMeshProUGUI (Label)
4. In the Graphics tab, add two entries:
   - Entry 0: target = Background Image, role = Background
   - Entry 1: target = Label TMP, role = Label
5. Assign a style in the Style tab or configure colors manually
```

### Applying a style

```csharp
// From code
AdvancedUIButton btn = GetComponent<AdvancedUIButton>();
btn.ApplyStyle(UIButtonStylePresets.Primary());

// Or assign a .asset file in the Inspector Style tab
```

### Generating built-in style presets

```
Tools -> AdvancedUI -> Generate Style Presets
```

This creates 5 .asset files in `Assets/AdvancedUIButton/Presets/`.

### Generating the demo scene

```
Tools -> AdvancedUI -> Create Demo Scene
```

## API Reference

### AdvancedUIButton

| Member | Description |
|---|---|
| `CurrentState` | The current ButtonState |
| `IsToggleOn` | True when the button is in Selected state |
| `Mode` | The active InteractionMode |
| `HoldConfig` | Access to hold settings and events at runtime |
| `ApplyStyle(UIButtonStyle)` | Applies a style asset |
| `SetSelected(bool)` | Sets the toggle selection state |
| `ToggleSelection()` | Flips the current selection state |
| `SetInteractable(bool)` | Enables or disables the button |
| `ForceRefresh()` | Forces an immediate visual refresh |

### Events (Inspector)

| Event | When |
|---|---|
| `OnHoverEnter` | Pointer enters the button |
| `OnHoverExit` | Pointer exits the button |
| `OnPress` | Button is pressed down |
| `OnRelease` | Button is released |
| `OnSelected` | Button becomes selected |
| `OnDeselected` | Button becomes deselected |
| `OnHoldComplete` | Hold duration fully elapsed |

### Events (C#)

```csharp
btn.OnHoverEnterEvent      += () => { };
btn.OnHoverExitEvent       += () => { };
btn.OnPressEvent           += () => { };
btn.OnReleaseEvent         += () => { };
btn.OnSelectionChangedEvent += isOn => { };
```

### UIButtonToggleGroup

| Member | Description |
|---|---|
| `Current` | The currently selected button |
| `SelectButton(btn)` | Selects a specific button |
| `SelectIndex(int)` | Selects by index |
| `DeselectAll()` | Clears selection (requires AllowNone) |
| `Register(btn)` | Adds a button at runtime |
| `Unregister(btn)` | Removes a button at runtime |

### UIButtonRipple

| Member | Description |
|---|---|
| `Spawn(Vector2)` | Spawns a ripple at screen position |
| `ApplyStyle(UIButtonStyle)` | Syncs color from style (if SyncWithStyle enabled) |

## Interaction Modes

| Mode | Behavior |
|---|---|
| Standard | Normal click button |
| Toggle | Click to select, click again to deselect |
| Radio | Click to select, cannot deselect alone -- use with UIButtonToggleGroup |
| Hold | Must hold for full duration to trigger OnHoldComplete |

## Style System

Styles are `UIButtonStyle` ScriptableObjects. Each style defines:
- Background colors per state
- Label colors per state
- Icon colors per state
- Animation settings (duration, easing, scale values)
- Audio clips

A style is applied once and updates all registered graphic entries that have a matching role (Background, Label, Icon). Custom role entries are never overwritten.

## Assembly Definitions

| Assembly | Contents |
|---|---|
| `AdvancedUI.Runtime` | All runtime scripts |
| `AdvancedUI.Editor` | Custom Inspector, scene builder, preset generator |
| `AdvancedUI.Demo` | Demo scene helpers (not required in production) |

## File Structure

```
Assets/AdvancedUIButton/
├── Runtime/
│   ├── Core/
│   │   ├── AdvancedUIButton.cs
│   │   └── ButtonDefinitions.cs
│   ├── Modules/
│   │   ├── UIButtonAnimator.cs
│   │   ├── UIButtonAudio.cs
│   │   └── UIButtonRipple.cs
│   ├── Animation/
│   │   ├── TweenRunner.cs
│   │   └── EasingFunctions.cs
│   ├── Interaction/
│   │   └── UIButtonToggleGroup.cs
│   ├── ScriptableObjects/
│   │   ├── UIButtonStyle.cs
│   │   └── UIButtonStylePresets.cs
│   └── AdvancedUI.Runtime.asmdef
├── Editor/
│   ├── AdvancedUIButtonEditor.cs
│   ├── UIButtonStylePresetsGenerator.cs
│   ├── DemoSceneBuilder.cs
│   └── AdvancedUI.Editor.asmdef
├── Demo/
│   ├── DemoHelpers.cs
│   ├── AdvancedUI.Demo.asmdef
│   └── AdvancedUIButton_Demo.unity
└── Presets/
    ├── Style_Primary.asset
    ├── Style_Secondary.asset
    ├── Style_Danger.asset
    ├── Style_Outline.asset
    └── Style_Ghost.asset
```

## Changelog

### 1.0.0
- Initial release
