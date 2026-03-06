# Changelog

All notable changes to AdvancedUIButton will be documented in this file.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.0.0] - 2025

### Added

**Core**
- `AdvancedUIButton` component — drop-in replacement for Unity's built-in Button
- 7 button states: Normal, Highlighted, Pressed, Selected, SelectedHighlighted, Focused, Disabled
- 4 interaction modes: Standard, Toggle, Radio, Hold
- Multi-graphic entry system: animate Background, Label, Icon independently per state
- Per-entry color duration and easing overrides
- Scale animation on the button RectTransform with per-state scale values
- `ignoreTimeScale` support for pause-safe UI

**Animation**
- `TweenRunner` — struct-based tween system, zero GC allocation per frame in steady state
- 11 easing functions: Linear, EaseIn, EaseOut, EaseInOut, BackIn, BackOut, BackInOut, ElasticOut, BounceOut, ExpoOut, CircOut

**Modules**
- `UIButtonAnimator` — multi-graphic color and scale transitions
- `UIButtonAudio` — per-state audio feedback with pitch randomization
- `UIButtonRipple` — pool-based ripple effect with RectMask2D/Mask clipping support

**Interaction**
- `UIButtonToggleGroup` — mutual exclusion for Radio/Toggle buttons, AllowNone support, runtime Register/Unregister

**Style system**
- `UIButtonStyle` ScriptableObject — define colors and animation settings once, apply anywhere
- `UIButtonStylePresets` — 5 built-in factory presets: Primary, Secondary, Danger, Outline, Ghost
- Style → Ripple sync: ripple color auto-derived from highlighted background color

**Editor**
- Custom Inspector with tabbed layout: Graphics, Style, Animation, Interaction, Ripple, Audio, Events
- Tooltips on all serialized fields
- Auto Setup Ripple button — creates child GO, adds UIButtonRipple, configures RectTransform
- `UIButtonStylePresetsGenerator` — Tools → AdvancedUI → Generate Style Presets
- `DemoSceneBuilder` — Tools → AdvancedUI → Create Demo Scene

**Demo scene**
- Section 1: Style presets showcase (Primary, Secondary, Danger, Outline, Ghost)
- Section 2: Interaction modes (Standard / Toggle / Radio group / Hold with progress bar)
- Section 3: All States frozen display (Normal, Hover, Pressed, Selected, Focused, Disabled)
- Section 4: Ripple effect (White, Blue, Cyan variants)
- Section 5: Easing showcase (BackOut, ElasticOut, EaseOut, Linear)

**Project**
- Assembly Definition files: `AdvancedUI.Runtime`, `AdvancedUI.Editor`, `AdvancedUI.Demo`
- README.md and DOCUMENTATION.md
- Compatible with Built-in Render Pipeline, URP, and HDRP
- Tested on Unity 2021.3 LTS and Unity 6
