# AdvancedUIButton

A professional UI button system for Unity — drop-in replacement for the built-in Button with multi-state transitions, ScriptableObject styles, ripple effect, and 4 interaction modes.

## Requirements

- Unity 2021.3 LTS or later (Unity 6 recommended)
- TextMeshPro (required — used for label transitions)
- No other dependencies — compatible with Built-in RP, URP, and HDRP

## Quick Setup

1. Add **Component → AdvancedUI → Advanced UI Button** to a UI GameObject
2. Create child `Image` (Background) and `TextMeshProUGUI` (Label), assign them in the **Graphics** tab
3. Go to **Tools → AdvancedUI → Generate Style Presets**, then assign a preset in the **Style** tab

For the full feature walkthrough, see [DOCUMENTATION.md](DOCUMENTATION.md).

## Features

- 7 button states with independent color, scale, and audio transitions per state
- Multi-graphic support: Background, Label, Icon each animate independently
- 11 easing functions including BackOut, ElasticOut, ExpoOut, CircOut
- 4 interaction modes: Standard, Toggle, Radio, Hold
- `UIButtonToggleGroup` for mutual exclusion between Radio buttons
- Pool-based ripple effect with RectMask2D/Mask clipping
- ScriptableObject styles — define once, update globally across all buttons
- 5 built-in presets: Primary, Secondary, Danger, Outline, Ghost
- Audio feedback with pitch randomization
- `ignoreTimeScale` support for pause-safe menus
- Assembly Definitions for clean compilation isolation
- One-click demo scene: **Tools → AdvancedUI → Create Demo Scene**

## File Structure

```
Assets/AdvancedUIButton/
├── Runtime/          Core components and modules
├── Editor/           Custom Inspector and editor tools
├── Demo/             Demo scene helpers (exclude from production builds)
├── Presets/          Generated style .asset files
├── README.md
├── DOCUMENTATION.md
└── CHANGELOG.md
```

## License

Copyright (c) 2025 AdvancedUI. All rights reserved.
Distributed via Unity Asset Store. See Asset Store EULA for terms of use.
