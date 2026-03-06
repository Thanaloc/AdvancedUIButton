# AdvancedUIButton — User Documentation

## Table of Contents

0. [Requirements](#0-requirements)
1. [Setting up your first button](#1-setting-up-your-first-button)
2. [Graphics and roles](#2-graphics-and-roles)
3. [Styles and presets](#3-styles-and-presets)
4. [Animation settings](#4-animation-settings)
5. [Interaction modes](#5-interaction-modes)
6. [Toggle groups](#6-toggle-groups)
7. [Ripple effect](#7-ripple-effect)
8. [Audio](#8-audio)
9. [Scripting reference](#9-scripting-reference)
10. [Tips and common setups](#10-tips-and-common-setups)

---

## 0. Requirements

| Requirement | Details |
|---|---|
| Unity version | 2021.3 LTS minimum — Unity 6 recommended |
| TextMeshPro | Required. Used for all label graphic transitions. |
| Render pipeline | Compatible with Built-in RP, URP, and HDRP. No shader changes needed. |
| Other packages | None. No external dependencies. |

---

## 1. Setting up your first button

**Step 1** — In the Hierarchy, right-click and create an empty UI GameObject inside a Canvas.

**Step 2** — Add Component -> AdvancedUI -> Advanced UI Button.

**Step 3** — Create two child GameObjects:
- `Background` with an Image component
- `Label` with a TextMeshProUGUI component

Make both RectTransforms stretch to fill the parent.

**Step 4** — In the Inspector, go to the **Graphics** tab. Add two entries using the + button:
- Entry 0: drag Background Image into Target, set Role to Background
- Entry 1: drag Label TMP into Target, set Role to Label

**Step 5** — Go to the **Style** tab and assign one of the preset .asset files, or configure colors manually in the Graphics tab.

Your button is ready. Hit Play and hover, click, and interact with it.

---

## 2. Graphics and roles

The **Graphics** tab lists all Graphic components the button will animate. Each entry has:

- **Target** — any Graphic subclass (Image, TextMeshProUGUI, MPImage, etc.)
- **Role** — determines how styles are applied:
  - `Background` — receives background colors from a style
  - `Label` — receives label colors from a style
  - `Icon` — receives icon colors from a style
  - `Custom` — never overwritten by styles, colors are yours to manage
- **Colors** — one color per state (Normal, Highlighted, Pressed, Selected, SelectedHighlighted, Focused, Disabled)
- **Duration** — transition duration in seconds for this entry specifically
- **Easing** — easing function for this entry's transitions

Each entry animates independently. You can have a Background that transitions slowly with BackOut easing while a Label transitions instantly.

**Copy / Paste** — use the C and P buttons on each entry to copy colors from one entry and paste them onto another.

---

## 3. Styles and presets

A `UIButtonStyle` is a ScriptableObject that bundles colors and animation settings. Applying a style updates all matching graphic entries at once.

### Generating built-in presets

Go to **Tools -> AdvancedUI -> Generate Style Presets**. This creates 5 .asset files in `Assets/AdvancedUIButton/Presets/`:

| Preset | Use case |
|---|---|
| Primary | Main call-to-action |
| Secondary | Supporting actions |
| Danger | Destructive or irreversible actions |
| Outline | Lighter alternative to Primary |
| Ghost | Minimal, text-only interactions |

### Assigning a style

In the Inspector **Style** tab, drag a .asset file into the Style field and click **Apply Style**.

Any change to the .asset file is immediately reflected on all buttons using it.

### Applying a style from code

```csharp
// Use a built-in preset
btn.ApplyStyle(UIButtonStylePresets.Primary());

// Use a .asset file loaded from Resources
UIButtonStyle style = Resources.Load<UIButtonStyle>("MyButtonStyle");
btn.ApplyStyle(style);
```

### Creating your own style

Right-click in the Project panel -> Create -> AdvancedUI -> Button Style. Configure all state colors and animation settings, then assign it to buttons.

---

## 4. Animation settings

Animation settings are configured in the **Animation** tab or inside a Style asset.

### Color transitions

- **Duration** — time in seconds to transition between states. 0 = instant.
- **Easing** — curve applied to the transition.

These can be set globally via a style or per-entry in the Graphics tab.

### Scale animation

The button's RectTransform is scaled on state changes.

- **Enable Scale** — toggle scale animation on or off
- **Scale per state** — set the target scale for each state
- **Duration / Easing** — control the feel of the scale transition

Recommended easing: `BackOut` for a springy feel, `EaseOut` for smooth.

### Easing types

| Easing | Feel |
|---|---|
| Linear | Constant speed |
| EaseIn | Starts slow, ends fast |
| EaseOut | Starts fast, ends slow (default) |
| EaseInOut | Slow at both ends |
| BackIn | Pulls back before moving |
| BackOut | Overshoots then settles — springy |
| BackInOut | Pull back then overshoot |
| ElasticOut | Bouncy, elastic snap |
| BounceOut | Physical bounce at end |
| ExpoOut | Explosive start, very long smooth tail |
| CircOut | Arc-shaped, smooth and round |

### Ignore Time Scale

When enabled, all animations use `unscaledDeltaTime` and remain responsive when `Time.timeScale = 0`. Enabled by default.

---

## 5. Interaction modes

Set in the **Interaction** tab.

### Standard

Default Unity button behavior. `onClick` fires on release.

### Toggle

Click to select, click again to deselect. The button stays in `Selected` state between clicks. Use `OnSelected` and `OnDeselected` events to react.

```csharp
btn.SetSelected(true);      // select from code
btn.ToggleSelection();      // flip current state
bool isOn = btn.IsToggleOn; // read current state
```

### Radio

Like Toggle but cannot be deselected by clicking again. Designed to be used with `UIButtonToggleGroup` so that selecting one button automatically deselects the others.

### Hold

The button fires `OnHoldComplete` only after the user holds it for the full `Duration`. `OnHoldProgress` fires every frame with a normalized value (0 to 1). Releasing early resets progress to 0.

- **Duration** — how long the user must hold in seconds
- **Cancel On Exit** — if enabled, moving the pointer out of the button cancels the hold

```csharp
btn.HoldConfig.OnHoldProgress.AddListener(t => progressBar.fillAmount = t);
btn.HoldConfig.OnHoldComplete.AddListener(() => Debug.Log("Confirmed"));
```

---

## 6. Toggle groups

`UIButtonToggleGroup` ensures only one Radio or Toggle button in a group is selected at a time.

### Setup

1. Add a `UIButtonToggleGroup` component to any GameObject in the scene
2. Set each button's mode to Radio (or Toggle)
3. Assign the `UIButtonToggleGroup` reference on each button in the Interaction tab
4. Add the buttons to the group's Buttons list

### Allow None

When enabled, clicking the selected button deselects it. When disabled, at least one button is always selected.

### From code

```csharp
UIButtonToggleGroup group = GetComponent<UIButtonToggleGroup>();

group.SelectIndex(0);          // select first button
group.SelectButton(someBtn);   // select specific button
group.DeselectAll();           // clear selection (requires AllowNone)

group.Register(newBtn);        // add button at runtime
group.Unregister(oldBtn);      // remove button at runtime

AdvancedUIButton current = group.Current; // get current selection
```

---

## 7. Ripple effect

The ripple is a separate component that you add to a child GameObject of the button.

### Auto setup

In the Inspector **Ripple** tab, click **Auto Setup Ripple**. This creates the child GameObject, adds the `UIButtonRipple` component, configures its RectTransform, and assigns it to the button automatically.

### Manual setup

1. Create a child GameObject on your button
2. Add `UIButtonRipple` to it
3. Set its RectTransform to stretch (anchor min 0,0 -- anchor max 1,1)
4. Position it in the hierarchy between Background and Label so it renders correctly
5. Drag it into the Ripple field on the button

### Clipping

To clip the ripple to the button shape, add one of these to the button's root GameObject:

- `RectMask2D` -- rectangular clipping, best performance
- `Mask` (with a Sprite Image as base) -- clips to sprite shape, required for rounded corners

### Sync with Style

When enabled, `ApplyStyle()` automatically sets the ripple color to a tinted version of the style's highlighted background color. Disable it to use a custom color.

### Configuration

- **Color** -- ripple color and alpha. Alpha around 0.25-0.35 works well.
- **Duration** -- how long the ripple animation takes
- **Max Pool Size** -- how many simultaneous ripples are allowed

---

## 8. Audio

Configure in the **Audio** tab. Requires an `AudioSource` component assigned to any persistent GameObject in the scene.

- **On Hover Enter** -- clip played when pointer enters
- **On Press** -- clip played on press
- **On Select / Deselect** -- clips for toggle state changes
- **On Disabled** -- clip played when the button enters Disabled state
- **Volume** -- playback volume multiplier
- **Pitch** -- base pitch
- **Randomize Pitch** -- adds a random offset each play to avoid repetition
- **Pitch Variance** -- max random pitch offset

Styles can include audio clips. If you apply a style, its clips are assigned to the matching fields.

---

## 9. Scripting reference

### Listening to events

```csharp
var btn = GetComponent<AdvancedUIButton>();

// Inspector UnityEvents are configured in the Events tab.
// For code, use C# events:
btn.OnHoverEnterEvent       += OnHover;
btn.OnHoverExitEvent        += OnHoverEnd;
btn.OnPressEvent            += OnPress;
btn.OnReleaseEvent          += OnRelease;
btn.OnSelectionChangedEvent += isOn => Debug.Log("Selected: " + isOn);
```

### Controlling the button

```csharp
btn.SetInteractable(false);   // disable
btn.SetInteractable(true);    // enable
btn.SetSelected(true);        // select (Toggle/Radio mode)
btn.ToggleSelection();        // flip selection
btn.ApplyStyle(someStyle);    // apply style
btn.ForceRefresh();           // force immediate visual update
```

### Reading state

```csharp
ButtonState state = btn.CurrentState;
bool isSelected   = btn.IsToggleOn;
InteractionMode m = btn.Mode;
```

### ButtonState enum

```
Normal, Highlighted, Pressed, Selected,
SelectedHighlighted, Focused, Disabled
```

---

## 10. Tips and common setups

### Confirm button (Hold mode)

Use Hold mode with a progress Image using `fillAmount` driven by `OnHoldProgress`. Set Cancel On Exit to true so accidental hovers don't trigger it.

```csharp
btn.HoldConfig.OnHoldProgress.AddListener(t => fillImage.fillAmount = t);
btn.HoldConfig.OnHoldComplete.AddListener(ConfirmAction);
```

### Tab bar (Radio group)

Create 3-5 Radio buttons, assign them all to a UIButtonToggleGroup with AllowNone disabled. Subscribe to each button's `OnSelected` to switch the active panel.

### Animated nav menu (Toggle mode)

Use Toggle mode with scale animation for nav items. SelectedHighlighted state gives you an extra color for when the selected item is hovered.

### Pause-safe UI

Enable `Ignore Time Scale` on all buttons in your pause menu. Transitions will remain smooth even when `Time.timeScale = 0`.

### Custom Graphic support

Any class that inherits from `UnityEngine.UI.Graphic` works as a target. This includes third-party components like MPImage (MagicaUI), Shapes (Freya Holmer), and TMP_Text subclasses.

### Performance

- The tween system uses struct-based values -- zero GC allocation per frame in steady state
- Ripple uses an object pool -- no Instantiate/Destroy per click
- The Hold coroutine stops automatically when released and restarts only on press
---

## 11. Known Limitations

**No native automatic layout support**
`AdvancedUIButton` uses scale animation on the RectTransform. This conflicts with Unity's automatic layout groups (`HorizontalLayoutGroup`, `VerticalLayoutGroup`, `GridLayoutGroup`), which override RectTransform size every frame. To use buttons inside a layout group, disable scale animation or use a wrapper GameObject for the layout and place the button inside it without a layout element on the button itself.

**Ripple pool is fixed at initialization**
The ripple pool allocates all slots on `Awake`. If you change `Max Pool Size` in the Inspector after the scene has started, the change takes effect only after reloading the scene. This is by design to avoid mid-session allocations.

**No nested Selectable support**
Unity does not support nested `Selectable` components (a Selectable inside another Selectable). Placing an `AdvancedUIButton` inside another `AdvancedUIButton` in the hierarchy will cause unpredictable pointer event routing. Use sibling or parent/child GameObjects without overlapping Selectables.
