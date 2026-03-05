# AdvancedUIButton
> A professional, modular button system for Unity UI — built as an Asset Store release.

---

## Overview

`AdvancedUIButton` is a drop-in replacement for Unity's native `Button` component. It extends `UnityEngine.UI.Button` with a full state machine, animated transitions, audio feedback, toggle/radio/hold interaction modes, and a custom tabbed Inspector — all with zero external dependencies.

---

## Features

- **7 button states** — Normal, Highlighted, Pressed, Selected, Selected+Highlighted, Focused, Disabled
- **Multi-graphic entries** — colorize background, label, icon independently per state
- **Animated transitions** — color and scale with 9 built-in easing functions
- **4 interaction modes** — Standard, Toggle, Radio, Hold
- **Toggle groups** — tab bars and radio buttons out of the box
- **Audio module** — per-state clips with optional pitch randomization
- **ScriptableObject styles** — one asset to style all buttons in a project
- **C# events + UnityEvents** — both exposed for maximum flexibility
- **Unscaled time support** — UI works correctly when the game is paused
- **Zero external dependencies** — no DOTween, no third-party packages
- **Unity 2021+ compatible**

---

## Project Structure

```
AdvancedUIButton/
├── Runtime/
│   ├── Core/
│   │   ├── AdvancedUIButton.cs       # Main component
│   │   └── ButtonDefinitions.cs      # Enums, interfaces, shared types
│   ├── Animation/
│   │   ├── TweenRunner.cs            # Generic coroutine tween engine
│   │   └── EasingFunctions.cs        # 9 easing functions, zero allocation
│   ├── Modules/
│   │   ├── UIButtonAnimator.cs       # Color + scale transitions
│   │   └── UIButtonAudio.cs          # Audio feedback per state
│   ├── Interaction/
│   │   └── UIButtonToggleGroup.cs    # Radio/tab group management
│   └── ScriptableObjects/
│       └── UIButtonStyle.cs          # Style asset definition
├── Editor/
│   └── AdvancedUIButtonEditor.cs     # Custom tabbed Inspector
├── Samples/                          # Demo scenes (coming)
└── Documentation/                    # Full docs (coming)
```

---

## Installation

1. Copy the `AdvancedUIButton/` folder into your Unity project under `Assets/`
2. Unity will compile the scripts automatically
3. Add `AdvancedUIButton` to any UI GameObject via `Add Component → AdvancedUI → Advanced UI Button`

> The `Editor/` folder must remain named exactly `Editor` for Unity to exclude it from builds.

---

## Quick Start

### Basic button

```csharp
// Listen to events from code
var btn = GetComponent<AdvancedUIButton>();

btn.OnHoverEnterEvent     += () => Debug.Log("Hover in");
btn.OnSelectionChangedEvent += isOn => Debug.Log($"Selected: {isOn}");
btn.onClick.AddListener(() => Debug.Log("Clicked"));
```

### Toggle / Radio

Set **Interaction Mode** to `Toggle` or `Radio` in the Inspector.  
Assign a `UIButtonToggleGroup` to manage mutual exclusion across buttons.

```csharp
// Control selection from code
btn.SetSelected(true);
btn.ToggleSelection();
```

### Hold button

Set **Interaction Mode** to `Hold` and configure **Hold Duration**.  
Bind `OnHoldProgress` to drive a progress bar fill.

```csharp
btn.EditorHoldSettings.OnHoldProgress.AddListener(t => progressBar.fillAmount = t);
btn.EditorHoldSettings.OnHoldComplete.AddListener(() => Debug.Log("Hold complete"));
```

### Applying a style at runtime

```csharp
[SerializeField] private UIButtonStyle _style;

void Start()
{
    GetComponent<AdvancedUIButton>().ApplyStyle(_style);
}
```

---

## States

| State                | Triggered when                            |
|----------------------|-------------------------------------------|
| `Normal`             | Default                                   |
| `Highlighted`        | Pointer over, interactable                |
| `Pressed`            | Pointer down                              |
| `Selected`           | Toggled on                                |
| `SelectedHighlighted`| Toggled on + pointer over                 |
| `Focused`            | Keyboard / gamepad navigation focus       |
| `Disabled`           | `interactable = false`                    |

---

## Easing Functions

`Linear` · `EaseIn` · `EaseOut` · `EaseInOut` · `BackIn` · `BackOut` · `BackInOut` · `ElasticOut` · `BounceOut`

---

## Roadmap

- [x] Core state machine
- [x] Multi-graphic color transitions
- [x] Scale animation
- [x] Audio module
- [x] Toggle / Radio / Hold modes
- [x] Toggle groups
- [x] ScriptableObject styles
- [x] Custom Inspector
- [ ] Ripple effect
- [ ] Built-in style presets (Primary, Secondary, Danger, Outline, Ghost)
- [ ] Demo scenes
- [ ] Full documentation

---

## Requirements

- Unity 2021.3 LTS or newer
- Unity UI package (com.unity.ugui)

---

## License

Private — all rights reserved.  
Not for redistribution prior to Asset Store release.
