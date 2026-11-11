# Who Are You UNDER PRESSURE? — Build Plan

The game design lives in the game idea document. This file is only about **building** it: the stages, what is done, and what is still to decide.

Tick a box by changing `- [ ]` to `- [x]`.

---

## Current status

- **Where we are:** Stage 1b, first test version, group A done (steps 1–3). The Input Actions are on the 2026-11-09 controls (Grab on left mouse, W zoom, Space use, A/D feet, Shift breathe, Enter, Esc, arrows). Zoom works all the time and looking is slower while zoomed. A rough breathing test works: `Stress` (bars as plain text), `Breathing` (Shift, judging, lowers stress) and `BreathBox` (the bar). Stamping still runs on the old system (`StampHolder` now on left mouse) until `Grabbable` replaces it. Johnny's dummy model sits in the chair, and his real right arm follows the hand with IK. The stamp and the hand have different reaches, so the stamp is sometimes too far from the palm: fixed by `Grabbable`.
- **Next step:** step 4 of "First test version: build order": `Grabbable` with zones, the hand following the held object, and leaning only when the hand can't reach (steps 4–5), split into small parts.

---

## Agreed setup

| Topic | Decision |
|---|---|
| Unity version | 6000.4.0f1 |
| Render pipeline | URP |
| Version control | Git + GitHub, commit after every change |
| Extra backup | Physical backup from time to time |
| Question data | CSV files (edited in a spreadsheet, saved as CSV) |
| Projects | Demo first, in its own project. When the demo is done, copy it to start the full game (own GitHub repository, without the Library folder). |
| Build order | Demo rooms in playing order: office, game show, tower, circus. First a small test version: title screen and office only |
| Trailers | One for the demo, one for the full game |
| Build plan | This file, kept in the project and in Git |

---

# Part 1 — The demo

The demo is a **small complete game**, not a slice of the full game: a beginning, an end, statistics, achievements and reasons to play again. One run of four rooms, 36 to 38 questions (office 6–8, then 10 per room), about 30 minutes (measured in playtests). No money. Saves after each room. See "The demo" in the game idea document.

**Why the demo exists:** to learn game making, to give people something to play while the full game is made, and to test the idea.

**Two kinds of builds:**
- **Test versions:** rough, for a few friends. First the office (Stage 1b), then all four rooms (playtests in Stages 2 and 5). They test the idea and the controls. No polish, no extras.
- **The release demo:** the same rooms, finished around the edges (Stage 5). "Complete" means finished, not bigger: no extra rooms or questions.

## Stage 0 — Project setup

- [x] Create a new 3D (URP) project in Unity 6000.4.0f1
- [x] Create the GitHub repository
- [x] Add a Unity `.gitignore` (so Library, Temp and other generated folders are not uploaded)
- [x] Set up Git LFS for large files (audio, models, textures)
- [x] First commit and push
- [x] Folder structure (Scripts, Scenes, Data, Audio, Art, Prefabs)
- [x] Check that the Input System package is active
- [x] Add this build plan to the project
- [ ] First physical backup

## Stage 1 — Core systems, built with the office

Every room follows the same rhythm: **question → choice → result → transition → next question.** These systems are built once and used by every room.

- [x] Question flow, with a transition step each room fills in its own way
- [x] Questions loaded from CSV files
- [x] Choice log: every decision recorded as an event, with the question's tags
- [ ] Choice log: record the stress level (bars) with every answer
- [x] Voice line player with subtitles
  - [x] `VoiceLines.csv` (id, speaker, one column per language) and its loader
  - [x] `VoicePlayer`: plays a clip, shows its subtitle, waits until it ends
  - [x] Intro and end lines as lists on `QuestionFlow`
  - [x] `before`, `voice` and `react` columns in the question CSV
  - [x] `tagsNone` column (tags for declining everything)
  - [x] 15 real office questions in `Office.csv` *(cut to 6–8 in "The office opening")*
  - [x] Her special reactions as code in `OfficeRoom`: the Q12 contradiction and the second decline-all
- [x] Johnny on his own audio channel: his mumbles and breathing play at the same time as her voice, not after it
- [x] Johnny's captions: a second subtitle text just above hers, smaller and grey or italic, in square brackets ("[muffled protest]"). Breathing gets a caption only when it changes
- [x] Johnny's breathing: stress levels set by the script *(replaced on 2026-11-09 by the stress system, see "Stress and breathing")*
  - [x] `JohnnyBreathing`: own AudioSource, loops random variants of one state, quick fade on change, pauses while he makes a one-time sound
  - [x] `breath:2`, `breath:5_03` (start with that clip) and `breath:-1` (silent) in any clip list
  - [x] One-time sounds: `JOH_CRY` plays a random `JOH_CRY_..`, `JOH_CRY_02` that exact clip
  - [x] `breath` column in the question CSV: `A:2|any:1` *(replaced by the `stress` column)*
  - [x] Falling: `BreathFalling` rules per room *(replaced: stress never falls by itself)*
- [ ] Tune the intro: pause lengths between her lines, how much Johnny mumbles
- [ ] ~~Wake-up animation~~ Replaced on 2026-11-01 by the dark wake-up, and on 2026-11-09 by "The office opening".
- [x] Office voice script written ("Demo voice lines: Office and ride") *(new draft: "Office script (draft)" in the Controls and Stress doc)*
- [x] Office voice lines: AI placeholders
- [X] Johnny's sounds (mumbles, breathing), recorded by me -PAIN for game show still to do
- [ ] Johnny's manual breaths (one in, one out, a few variants each) for breathing with Shift
- [ ] Final voices recorded by real people (later)
- [ ] On-screen hint system (replaces "controls shown on screen or on objects"): show a hint, hide it when done, show it again after doing nothing
- [ ] Skip: hold Enter about 1 second, a small circle fills; only for lines already heard
- [ ] Debug tools: jump to any room, skip questions, set the stress level

### Controls

Agreed 2026-10-20, redesigned 2026-11-01, redesigned again 2026-11-09 for the demo. **The object controls the hand, not the player.** Comfort comes before realism. Left hand on the keys: little finger Shift (breathe), ring finger A (left foot), middle finger W (zoom), index finger D (right foot), thumb Space (use). Right hand on the mouse.

| Input | What it does |
|---|---|
| Mouse | Look around. While holding an object: move the object inside its zone |
| Left mouse (hold) | Look at an object and hold: the right hand picks it up. Release: hand and object go back to their idle spots |
| Both mouse buttons (hold) | Two-hand objects (the rope). Left mouse = left hand, right mouse = right hand. Releasing one button drops that side |
| W (hold) | Zoom. Works while holding too. Zoom is also "interact" (lights, collectables) |
| Space | Use the held object (stamp). Game show: his head presses the button |
| A / D | Left foot / right foot. Taught in the circus, where they tilt the board |
| Shift (hold) | Breathe in. Release = breathe out (see "Stress and breathing") |
| Esc | Pause and settings |
| Enter (hold) | Skip lines already heard |
| Arrow keys | Left/right: look sensitivity. Up/down: holding (hand) sensitivity |

- **One hand does the work.** The right hand picks up and uses everything. The left hand is only used for two-hand objects like the rope.
- **While holding, the view is locked to the object's zone.** The camera moves only as far as the zone allows.
- **No Q/E.** Turning the hand is cut for now. Stamps always land straight.
- **No Z/C.** The feet moved to A and D.
- **Gamepad later.** Not for the demo.
- **Room by room:** office = breathing, looking, zoom, right hand. Game show = head (Space). Tower = both hands (both mouse buttons). Circus = feet (A/D).

### Grabbable objects

Agreed 2026-11-09. Every object that can be held gets one shared script, `Grabbable` (working name). The object decides how it reacts, moves and is held. What happens on Space stays in its own small script, like `StampPress` now.

- **Inspector settings:** Zone (the invisible box it can move in while held), Hands (one or two), Grip point (empty child where the hand holds it, like `Grip` now), Hover height and follow speed, Idle spot (where it glides back when let go).
- **How it moves:** a ray from the camera hits the desk, and the object hovers just above that point, under the gaze, not in the middle of the screen. Looking outside the zone stops it at the zone's edge.
- **Reaching and leaning (agreed 2026-11-12):** pressing left mouse doesn't move the camera; the hand reaches out to the object and picks it up. The head still turns with the mouse while holding. Only when the object goes past the hand's reach do the body and camera lean forward, just enough for the hand to follow (up to a maximum, smoothed so it doesn't wobble). On release the object glides to its spot, the hand slowly goes back to its idle spot, and the body and camera lean back.
- **Examples:** stamp = `Grabbable` + `StampPress`. Rope = `Grabbable` set to two hands + a rope script.
- **Old scripts:** `DeskHand` gets its target from the object instead of the mouse. `Stamp`, `StampPress` and `InkMark` mostly stay. `HandTurn` goes to Spare.

### Stress and breathing

Agreed 2026-11-09. Replaces "How Johnny's breathing works" (2026-10-18/20). Stress makes decisions harder; staying calm is the player's job: answer in time *and* manage the breathing.

**Stress level: 10 bars**, shown on a small device on the desk (a screen with text on top, the bars below).

| Bars | Name | Effect |
|---|---|---|
| 0 | Stable | None |
| 1–4 | Elevated | Slight hand shake |
| 5–8 | Critical | Blur and stronger shake |
| 9–10 | Overload | Heavy blur, heavy shake, her voice harder to hear |

No adrenaline effect.

**Stress goes up: added, not set.** It never falls by itself; only breathing lowers it.
- One-time add: a hard choice or seeing someone hurt. A `stress` column in the question CSV, like `A:+2|B:+1`.
- Add over time: the room's code adds stress while something happens.
- Pain is separate: losing a finger plays its pain sounds as a one-time event and also adds a big jump of stress.
- Each room sets its own starting value.

**Breathing lowers it.** Hold Shift 4 seconds to breathe in, release for 6 seconds to breathe out. A small box on screen fills during the in and empties during the out.
- Each half is judged on its own: Perfect (off by 0.2 s or less), Good (0.5 s or less), Bad (more).
- A cycle (in + out) gets the worse of its two halves.
- A check happens after N cycles ("cycles per check", an Inspector value, may differ per room). The check gets the worst of its cycles: Perfect = −2 bars, Good = −1 bar, Bad = no change.
- First test: 1 cycle per check. Switch to 2 if stress drops too fast (with 2: 10 bars to 0 takes at least 100 seconds).
- **Decided in the breathing test (2026-11-12):**
  - The bar is horizontal, under the result word, and fills from the middle out to both edges during the in (full at 4 s), and shrinks back during the out (empty at 6 s). Following it: release when full, press again when empty.
  - The bar is hidden while he isn't breathing and fades in (0.2 s) when Shift is pressed.
  - The result word shows in the middle of the screen, only the word: Perfect green, Good blue, Bad red. It stays 0.8 s. Off by more than 1 s: no word (no flashing on quick taps).
  - A Bad in cancels the breath: the cycle is already Bad, so there is no out; the bar disappears and a new press starts a new breath.
  - The out shrinks from where the in stopped (no jump to full) and still empties at 6 s.
  - The out ends when Shift is pressed again. No press within 7 s (6 + 1): the breath counts as Bad, quietly, and he waits for a new breath.

**Sound.** Johnny's breathing loops by stress level, as now (`JohnnyBreathing`, `ClipVariants`, file names `JOH_BREATH_<state>_<number>` stay). While the player breathes with Shift, the loop stops and his manual in and out breaths play instead. His one-time sounds (`JOH_CRY` …) work as before.

**Recorded.** The stress level at every answer goes into the choice log (bonus titles later, and her demo ending).

### Stamping and ink

Built 2026-11-01 on the old controls. Changed 2026-11-09: the stamp is picked up with left mouse (`Grabbable`), there is no turning, and stamps always land straight.

- **Press:** Space pushes the held stamp down onto whatever is under it, paper or desk (`StampPress`, a ray straight down). It stays down while Space is held and comes up when Space is let go. While pressing, the stamp can't move or be let go.
- **Ink mark:** appears where the stamp touched, turned like the stamp (`InkMark` prefab: a Quad with the stamp's ink material, `Ink_Green` / `Ink_Red`, URP Unlit, Transparent; an image can be put in the Base Map later). A mark on a paper becomes the paper's child; desk marks stay under `OfficeSet` for the whole room. Each new mark is drawn on top (Render Queue 3000 + count).
- **Ink pools:** each stamp has 5 uses, each mark fainter (1, 0.8, 0.6, 0.4, 0.2), the 6th press leaves nothing. Going back to its spot refills it.
- **Agreed, not built yet:** a "Stamp here:" box on each paper, about twice the stamp's size. A stamp counts when about 75% of its face is inside the box; its outline goes darker. Only the first counting stamp on a paper is its answer; more stamps still leave marks. Green counts once per question: green on a second paper leaves a mark and she says "You can only accept one paper." Stamping a paper again with the other colour plays her overwrite lines.
- **One stamp first, then two:** on question 1 only the green stamp is on the desk, in a small stand. From question 2 a hatch in the desk opens and the red stamp rises out.

### The arm (grey boxes)

Built 2026-10-31. Everything is a child of `HandArea` (on the desk surface, middle of the desk).

- **Shoulder** (fixed) → **Elbow** → **Wrist** → **Hand** (→ Palm, Grip).
- `DeskHand` moves the **Wrist** inside the area (1.2 × 0.6 m, height 0.06 above the desk). Only the wrist is kept in the area. *(Changes: its target comes from the held object.)*
- `ArmPose` moves the **Elbow** forward and back with the wrist (`forwardFollow` 0.7; `sideFollow` 0, so left/right doesn't move it) and stretches the UpperArm and Forearm cubes between the points, in `LateUpdate`.
- The **Hand** always points forward. `HandTurn` goes to Spare (no Q/E).
- A held stamp becomes a child of **Grip** (an empty object with scale 1, never a scaled cube). Each `Stamp` remembers its spot and glides itself.
- **The left hand is tied for the whole office.** The second hand (a mirrored left arm) is built with the tower.
- **Real arm: done.** Johnny's model has an `ArmRig` with `RightArmIK` (Animation Rigging "Two Bone IK") and a `RealHandTarget`, so his real right arm follows the grey-box hand. The grey-box arm cubes are hidden.
- **Problem:** the stamp and the hand have different reaches, so the stamp is sometimes too far from the palm. The whole stamp and hand system is reworked with `Grabbable` (build steps 3–4): the held object's grip point should be the one target the real hand follows, so hand and stamp can never drift apart.

### Johnny's body (dummy model)

A dummy model to work on animations and see how it looks; it can be swapped for the real model later.

- [x] Dummy model: Remy from Mixamo as `Johnny.fbx`, with materials and textures (`Art/Characters`)
- [x] Sitting idle animation (`Johnny_SittingIdle`) and `JohnnyAnimator`
- [x] Animation Rigging package (1.4.1): right arm IK (`ArmRig`, `RightArmIK`, `RealHandTarget`)
- [x] `HideHead`: shrinks the head bone so the camera doesn't see inside his face (his shadow has no head)
- [ ] Right hand follows the held object's grip point (with the `Grabbable` rework)
- [ ] Left hand tied to the chair for the whole office (a pose)
- [ ] Wake-up: panicking while tied, then calmer as stress goes down (poses or animations)
- [ ] Leaning forward while holding, only when the hand can't reach (a lean pose or the spine bone)
- [ ] Hand shake on the real hands, by stress level
- [ ] Left arm IK for the rope (tower)
- [ ] Missing fingers and bleeding (game show). Maybe the same bone-shrinking trick as `HideHead`; decide when building
- [ ] Head pressing the button (game show) and feet tilting the board (circus)
- [ ] Swap in the real Johnny model later

### The office (grey boxes)

- [x] Grey-box room (`OfficeSet`): floor, walls, desk, chair, green and red stamps, camera at his eyes. Materials in `Art/Materials`
- [x] `Paper` prefab (sheet + 3D TextMeshPro) with the `Paper` script
- [x] Papers on the desk: `OfficeRoom.ShowQuestion` lays 1 to 4 papers in a row on `PaperCenter` (0.24 m apart), after her `before` lines and before her question line. A single paper shows the statement, the others one option each
- [x] Fixed state: the mouse moves his wrist in a desk area (`DeskHand`); the camera follows it softly (`SeatedLook`) *(replaced 2026-11-09: free look, the object follows the gaze)*
- [x] Hold Shift to look around (`SeatedLook`) *(replaced: no button = look; Shift is now breathing)*
- [x] Grey-box right arm: shoulder, elbow, wrist, hand (`ArmPose`)
- [x] Picking up a stamp with the left mouse button (`StampHolder`, `Stamp`) *(becomes `Grabbable`: look at it and hold left mouse)*
- [x] Turning the hand with Q and E (`HandTurn`) *(cut 2026-11-09; script in `Scripts/Spare` since 2026-11-11)*
- [x] Zoom with the right mouse button while holding Shift (`CameraZoom`) *(moves to W, works while holding too)*
- [x] Stamping with Space: press and hold, ink marks on paper or desk, ink pools with 5 fading uses (`StampPress`, `InkMark`)
- [x] Input Actions on the 2026-11-01 controls *(replaced in build step 1 by the 2026-11-09 controls)*
- [x] No button held = look around (`SeatedLook`); a mouse button still pulls the view back to the main view *(stays until `Grabbable`, build step 4)*
- [x] Gripping a stamp with D (`StampHolder`) and zoom on W, only while looking (`CameraZoom`) *(since build step 1 `StampHolder` grabs with left mouse until `Grabbable`; since build step 2 zoom works while holding)*
- [x] 15 questions in CSV
- [ ] The radio her voice comes from (maybe 3D sound)
- [ ] Johnny's mouth sewn shut: mumbles and breathing only
- [ ] Papers come from a printer and leave through a shredder in the desk (now they just disappear); see build steps 9–10
- [ ] Ending line: "Great, now let's start the main test."

### The office opening

Decided 2026-11-09. Replaces "The office (reworked)" (2026-11-01). The office teaches breathing, looking, zooming and the right hand, one at a time, each only when it's needed. No foot questions in the office: feet are taught in the circus. Target length about 3–4 minutes. Full script draft: "Office script (draft)" in the Controls and Stress doc.

**Teaching rule:** she says what to do, the screen says which button. She stays a test supervisor, never a tutorial voice. Before she speaks, all help is on-screen text only.

1. **Wake-up and panic.** Black for 2–3 seconds, then one very weak light above him. `JOH_MUM_WAKE`, then fast breathing. Stress starts at about 6–8 bars; hands and feet tied; looking allowed but blurred. The stress device and a short text teach breathing (4 in, 6 out, hold Shift, follow the box). Goal: 4 bars or less. Help: the hint shows again after about 10 seconds of doing nothing; after a few bad results, "Follow the box." She says nothing about breathing.
2. **Look and zoom: the three lights.** Screen: "Move the mouse to look around." (+ the left/right arrow hint), then "Hold W to zoom." Three light switches on the walls, inside his head-turning limits. Zoom and hold about 2–3 seconds fills a small circle, a switch clicks, more of the room lights up. After the third: desk, stamps and radio are visible.
3. **Her first line**, from the radio: "Oh, you're finally awake." Then a short introduction of the test, not of herself. She doesn't give her name.
4. **The right hand.** Strap sound, his right hand is free; the left stays tied. First paper, one hint at a time, each once: she says "Use the green stamp", screen "Look at a stamp and hold left mouse." (+ the up/down arrow hint) → "Move it over the box. Press Space to stamp." → "Let go of left mouse to put it back." Only the green stamp is on the desk for question 1, in a small stand.
5. **The test.** 6–8 paper questions cut from the 15. From question 2 a hatch in the desk opens and the red stamp rises out ("Now you make a choice.") and she explains the stamp rules. Questions add 0 or +1 stress, tricky ones +2. After the wake-up the office never reaches Critical. One collectable in the room: a mark or note from an earlier subject.
6. **The end.** "Great, now let's start the main test." Lights out, the player is moved to the game show set in the dark, lights on there.

#### Office content

- [ ] Cut `Office.csv` to 6–8 questions (draft: 8) and add a `stress` column
- [ ] Move unused lines to Spare (list in "Lines no longer used" in the script draft)
- [ ] New voice lines (AI placeholders first):
  - Introduction: "Oh, you're finally awake.", "Thank you for participating in our selection program.", "Don't bother trying to talk. It won't work.", "You'll be given a few statements. Answer them with the stamps."
  - "Use the green stamp.", "Now you make a choice.", "I hope so.", "Quick. Good." (optional)
  - "Every paper needs a stamp." (short version) and "You can only accept one paper."
  - Overwrite lines, counted per room: tries 1–3 "Please make up your mind.", tries 4–5 "This is a simple test. Choose once.", 6th "I can see this simple test might be too much for you." After that she stays quiet
  - Existing lines said without "Johnny" (`OFF_RULES_04`, `OFF_END_01a`)

## Stage 1b — First test version

Office and title screen only, for a few close friends. It ends with a black screen after the office, nothing else. "Looking good" means mood first: lighting and sound do most of the work; detailed models wait until after the test.

### First test version: build order

Replaces "Office rework: build order" (2026-11-01). Changed 2026-11-10: breathing is tried early, and one question works from start to end before the opening is built.

**A. Controls and a quick breathing test**
- [x] 1. Input Actions to the new controls:
  - Keep: Look (mouse), Zoom (W), Use (Space)
  - Change: LeftHand (left mouse) becomes Grab, for the right hand. RightHand (right mouse) stays, for the rope's second hand. LeftFoot/RightFoot move from Z/C to A/D
  - Remove: GripLeft, GripRight, TurnHand (Q/E). `HandTurn` goes to Spare
  - Add: Breathe (Shift), Skip (Enter), Pause (Esc), LookSensitivity (left/right arrow), HoldSensitivity (up/down arrow)
  - Update the scripts that read the old actions (`SeatedLook`, `StampHolder`, `HandTurn`) so nothing breaks
  - *Done 2026-11-11: Breathe is on Left Shift only. `HandTurn` moved to `Scripts/Spare`. `StampHolder` grabs with left mouse (while held, as soon as a stamp is in reach) until step 4.*
- [x] 2. Free look: no button = look around, inside the head-turning limits. Zoom on W, also while holding; zoomed look sensitivity scales down automatically
  - *Done 2026-11-11: `CameraZoom` zooms all the time; `SeatedLook` multiplies the look sensitivity by current / normal Field of View (half as fast at full zoom). What the camera does while holding stays as it is until step 4.*
- [x] 3. Rough breathing test: Shift, the on-screen box and the stress number as plain text. Play it, tune the 4/6 timing and the judging. If it doesn't feel good, change the design before building the opening around it
  - *Done 2026-11-12: `Stress` (Core, bars 0–10, `Add`/`Lower`, plain text, test buttons in the ⋮ menu), `Breathing` (on the Stress object) and `BreathBox` (bar with a Canvas Group). Decisions in "Stress and breathing". Timing kept at 4/6 for now.*

**B. Grabbing, stamping and one full question**
- [ ] 4. `Grabbable` with zones (the stamp and hand rework): look at an object and hold left mouse to pick it up; it follows the gaze inside its zone; the camera is limited to the zone while holding; release = it glides back to its idle spot. The real hand (`RealHandTarget`) follows the object's grip point, so hand and stamp always have the same reach. Replaces `StampHolder` and the mouse-driven `DeskHand`. Papers and stamps only ever land inside the hand's reach
- [ ] 5. Lean forward only when needed: the camera stays in place when grabbing; when the held object goes past the hand's reach, the body and camera lean forward just enough (up to a maximum, smoothly), and back on release. The body leans with it (a lean pose on the Animator, or turning the spine bone), so the shoulder and arm still match. See "Reaching and leaning" in "Grabbable objects"
- [ ] 6. Stamping on `Grabbable`: `StampPress` and `InkMark` on the new grab, stamps always straight
- [ ] 7. "Stamp here:" box: the paper reads its stamp (about 75% inside, first counting stamp only, green once), outline goes darker
- [ ] 8. Answering by stamps instead of number keys, with her overwrite and "only one paper" lines
- [ ] 9. The printer: prints the papers fast, each slides out and along a fixed path to its spot (no physics). The printer sound tells a question is coming
- [ ] 10. The shredder: after the answer, the papers slide into a slot in the desk and are shredded (fixed path, shred sound)
- [ ] 11. The stamps: the green stamp stands on the desk from the start; on question 2 a hatch opens and the red stamp rises out. A fake hole: a black square on the table and a sliding lid; the stamp rises from under the table through it, so the bought table needs no changes. If it looks wrong, a small box with a flip lid instead
- [ ] 12. One question from start to end: printer → stamp → shredder → next question. Play it until the loop feels good

**C. Room layout (grey boxes, real sizes)**
- [ ] 13. Place everything at its real size: desk, the hand's reach, printer, shredder slot, hatch, radio, stress device, the three light switches, the window, the collectable. Check what he sees when the lights come on and how far he can turn his head

**D. Stress and breathing, finished**
- [ ] 14. Stress level (0–10, add only), the `stress` column in the CSV, stress in the choice log. Move the old `BreathFalling` (still on an object in the scene) to Spare
- [ ] 15. Stress device on the desk (text + 10 bars) and the effects: blur, her voice harder to hear
- [ ] 16. Breathing, finished: the box, judging, "cycles per check" in the Inspector, manual breath sounds
- [ ] 17. Hand shake on the real hand, by stress level

**E. The opening**
- [ ] 18. Hint system (show, hide when done, show again after doing nothing)
- [ ] 19. Johnny's body: left hand tied to the chair for the whole office, panic pose or animation in the dark, calmer as stress goes down
- [ ] 20. Dark wake-up: very weak light, starting stress and goal, breathing hints
- [ ] 21. The three lights: zoom-and-hold with a filling circle, switch click, room gets brighter
- [ ] 22. Freeing the right hand (strap sound), radio as her sound source
- [ ] 23. The office script with the cut questions; new voice lines written and recorded (AI placeholders) as we build, and changed if needed
- [ ] 24. One collectable: a mark or note from an earlier subject (zoom and hold 5 seconds)

**F. Menus, look and the build**
- [ ] 25. Pause menu (Esc): Resume, Options, Quit
- [ ] 26. Sensitivity on the arrow keys, with a small popup that says which way it changed ("Look sensitivity increased: 7", "Look sensitivity decreased: 6")
- [ ] 27. Boot flow, small for the first test: brightness (with a "barely visible" symbol) and sensitivity, warnings, title screen (desk, chair, one small light; Play, Options, Credits, Exit), contract signed with the mouse (any scribble + Accept), fade to black, wake-up. The rest of the first-launch settings later
- [ ] 28. Choice log saved to a file at the end (answers, answer times, stress), so testers can send it back
- [ ] 29. Room design: bought or downloaded assets for the desk, walls and items; light, sound and mood (rain outside the window)
- [ ] 30. End: black screen after "Here we go." Windows build, played through once, mouse locked to the window, checked on another screen size
- [ ] 31. Zipped (rar) and sent as a Google Drive or OneDrive link

Not in the first test: skip (Enter), saving.

### What to learn from testers

- [ ] Did they understand breathing without help?
- [ ] Were the controls comfortable? Did they find the sensitivity arrows?
- [ ] How long did the wake-up and the office take?
- [ ] Did her character come across?

Watching them play, or asking them to record their screen, tells more than their answers.

## Stage 2 — The game show

- [ ] Timer: starts after she finishes speaking, 20 down to 10 seconds per part (the office has no timer)
- [ ] Head button: Space presses it with his head (tap, or hold to keep his face on the button)
- [ ] The first questions cost nothing; fingers are taken from question 4 or 5 (to decide), one per press
- [ ] At most 6 fingers: thumb and index finger stay on both hands, so he can still hold the rope in the tower. Fingers taken from one hand first, then the other
- [ ] Losing a finger: pain sounds as a one-time event and a big stress jump
- [ ] Missing fingers and bleeding stay for the rest of the run
- [ ] Host voice and jingle (my voice, loud, WWE style); she says at most one short, dry line through the radio
- [ ] Flashing lights, with a reduce-flashing setting
- [ ] 3 collectables
- [ ] 10 questions in CSV, with a `stress` column

### First playtest

- [ ] Friends play office and game show without help
- [ ] Do they argue about their choices?
- [ ] Measure how long each room takes
- [ ] Decide: continue as planned, change the design, or cut

## Stage 3 — The tower

- [ ] The second hand: a mirrored left arm with its own area
- [ ] The rope as a two-hand `Grabbable`: left mouse holds the left person, right mouse the right person. Releasing a button drops that side's person
- [ ] View limited to the rope's zone while holding (looking and zoom inside it)
- [ ] Rope is visual only
- [ ] Stress builds up slowly over the room
- [ ] Questions 9 and 10 cost hands (or arms)
- [ ] A lost hand cannot hold its side on question 10
- [ ] 3 collectables
- [ ] 10 questions in CSV

## Stage 4 — The circus

- [ ] Feet: A = left foot, D = right foot; they tilt the board between three states (left, middle, right). Taught here
- [ ] Looking around and zooming allowed
- [ ] Picking the tilted side again makes him fall
- [ ] Timeout: board stays put
- [ ] Falling ends the run ("who fell")
- [ ] 3 collectables
- [ ] 10 questions in CSV, placed so some kind answers sit on the dangerous side

## Stage 5 — The demo as a small game

Everything here reads from the choice log, so it is cheap compared with a new room. Build it after the four rooms work.

### Between the rooms
- [ ] Blackout transitions between rooms: lights out, move to the next set, lights on (replaces the tunnel rides)

### The end
- [ ] Her demo ending: the run is scored warm, neutral or cold (or died), and she says one of four lines. Replaces "Great, Johnny, welcome to the team. I have high hopes for you."
  - Warm: "We have a long road ahead of us, and I have high hopes for you." (maybe the one time she says "Johnny")
  - Neutral: "This one might work. We'll see."
  - Cold: "Sadly, this one is a failure as well."
  - Died (fell in the circus): "Another failure."
- [ ] Demo title builder: modifier, rank and ending (80 titles), built up on screen
- [ ] Demo description: "You are:" plus one sentence per part
- [ ] Share line ("Under pressure, I'm the …")
- [ ] Your statistics: answers, answer times, stress (answers in Critical or Overload), fingers and hands lost, who was saved, collectables found ("4 / 10 found")
- [ ] "How others answered" for a few questions, if the demo can have its own Steam stats (check when setting up Steamworks)
- [ ] Subject number on screen: a random number per run ("Subject #48213")
- [ ] The screen pointing to the full game

### Reasons to play again
- [ ] Collection screen: the title pieces (5 modifiers, 4 ranks, 4 endings, the secret title), the full titles you got as a list, the notes and marks found
- [ ] In-game achievements, kept in the save file and shown on the collection screen. They describe the player like the titles do (for example "Answered every question in Stable", "Lost no fingers", "Fell in the circus", "Declined everything in the office", "Found all 10 notes")

### Saving
- [ ] Save file: written after the office, after the game show, after the tower and at the end, with a "saving" sign
- [ ] What it keeps for the run: the next room, missing fingers and hands, the choice log, collectables found. Continue starts at the next room (each room sets its own starting stress)
- [ ] What it keeps between runs: titles and title pieces, achievements, collectables, lines already heard (for skip), settings

### Finished around the edges
- [ ] Settings: subtitles, volume, photosensitivity, look and holding sensitivity
- [ ] Skip (hold Enter) for lines already heard
- [ ] Credits
- [ ] Steady volume mix
- [ ] Real art and sound for all four rooms
- [ ] Second playtest: the whole demo, measured from start to end

## Stage 6 — Demo release

- [ ] Demo trailer
- [ ] Steamworks in the demo (learning it before the full game): connect Steam to Unity, save to the full game's Steam Cloud so demo saves carry over, global stats if a demo can have them
- [ ] Steam page for the full game, with the demo on it
- [ ] Demo release

---

# Part 2 — The full game

## Stage 7 — Full game project and shared systems

- [ ] Copy the demo project, new GitHub repository
- [ ] Stage manager: ten tests, tiers, 15-second tier breaks
- [ ] Money counter and the bill (fingers, hands, revivals, nails, knives, debt)
- [ ] Full title builder: modifiers, base names, "who is also / who's actually / but is actually", Frozen levels, "a" or "an"
- [ ] Full description builder
- [ ] Statistics page after each test
- [ ] Steam achievements, also given from the demo save when a demo player starts the full game (Valve recommends no achievements in the demo itself)
- [ ] Title screen, three save slots, test selection screen (Closed / Open / Finished)
- [ ] Autosave after every question and every test, with a "saving" sign (or only after each test: decide when the full game starts)
- [ ] Subject number: hash of Steam ID + slot + run, 4–6 digits (never the Steam ID itself)
- [ ] Her affection state: changes only what she says between questions
- [ ] Opening scene ("Where am I?")
- [ ] Shared chair-on-rails system (Courtroom, Bunker, Casino)
- [ ] Debug tools: fake history, random runs to check titles
- [ ] Gamepad support

## Stage 8 — The ten tests

Order to be decided (see "To decide").

- [ ] The Bell Tower Test
- [ ] The Metro Test
- [ ] The Game Show Test
- [ ] The Circus Test
- [ ] The Courtroom Test
- [ ] The Masquerade Test
- [ ] The Bunker Test
- [ ] The Casino Test
- [ ] The Boiler Room Test
- [ ] The Graveyard

## Stage 9 — The ending

- [ ] The two buttons
- [ ] Green ending cutscene, captured again
- [ ] Red ending: black screen, final title and statistics
- [ ] Next run: Johnny on a rope in the Bell Tower (saved flag, swapped model)

## Stage 10 — Full release

- [ ] Full game trailer
- [ ] Steam Cloud saves
- [ ] Steam stats: "how others answered" for 20 to 30 dilemmas
- [ ] Steam disclosures: content survey, age rating, AI use if needed
- [ ] Final polish and volume mix

---

## To decide

- [x] **Tower with missing fingers (demo).** Decided 2026-11-01: the game show leaves thumb and index finger on both hands (at most 6 fingers lost), so both hands can still hold the rope.
- [ ] **Game show: first finger question.** Fingers from question 4 or question 5? From 5, the 6 presses (questions 5–10) match the 6 fingers exactly; from 4, one press must cost nothing.
- [x] **Zoom in the fixed state.** Decided 2026-11-01, changed 2026-11-09: zoom on W works while looking and while holding.
- [x] **Sensitivity settings.** Decided 2026-11-09: look and holding sensitivity on the arrow keys and in the settings; zoomed look sensitivity scales down automatically. Not the mouse wheel.
- [x] **Turned ink mark.** Decided and built 2026-11-01. Since 2026-11-09 stamps always land straight.
- [ ] **Paper timing.** Papers appear before her question line. Check that it feels right in play (moving it is one line in `QuestionFlow`).
- [ ] **Ending line.** "Great, now let's start the main test." already plays in `endLines`. Tick the office item?
- [x] **Game idea document.** Updated 2026-11-09 with the new controls, stress and breathing, her character, the office opening and the boot flow.
- [x] **Paper questions with two hands.** Dropped 2026-11-09: the left hand is tied for the whole office.
- [x] **The left hand in the office.** Decided 2026-11-09: tied for the whole office; the second hand comes in the tower.
- [ ] **Office answer times (demo).** Do they count for a modifier? What does declining every paper make him?
- [ ] **Stress numbers.** How much each question and room adds, and how fast breathing lowers it (the 4/6 timing, the pauses, bars per result). Tune in playtests.
- [ ] **Stress bars → breathing sound.** Which of Johnny's breathing states (-1 to 6) plays at which number of bars.
- [ ] **Accidental foot presses on A/D.** Feet work only after her question ends, or a short hold (about 0.2 s) counts as a press.
- [x] **Skip in the demo.** Decided 2026-11-10: a small save file remembers which lines were heard.
- [x] **What the collectables are.** Decided 2026-11-10: marks and notes from earlier subjects (scratched tally marks, a name or subject number, short messages, pages). Each unlocks a page on the collection screen and is kept in the save file.
- [x] **The red stamp's hatch.** Decided 2026-11-10: a fake hole (black square and sliding lid on the table, stamp rising from below), so the bought table needs no changes.
- [x] **Saving in the demo.** Decided 2026-11-10: saves after each room and at the end; titles, achievements, collectables, lines heard and settings are kept between runs.
- [ ] **Saving in the full game.** After every question (idea document) or only after each test?
- [ ] **Demo achievements.** Which ones, and how many.
- [ ] **"How others answered" in the demo.** Can a demo have its own Steam stats? Which questions?
- [ ] **Her demo ending.** How the choice log is scored into warm, neutral and cold (answers in Critical or Overload, skipped or timed-out answers, average answer time per room, office declines and overwrites, fingers given).
- [ ] **Her ride lines.** With blackout transitions instead of rides: where her ride lines go, if anywhere.
- [ ] **Warnings.** A short plain screen, or clauses in the contract.
- [ ] **The window.** Does a window make sense in a room he's locked in? A real one with rain, a painted one, or one that's slightly wrong. Decide with the room design.
- [ ] **Reduce blur and shake.** A setting for players who feel sick?
- [ ] **Key rebinding** before release (French keyboards and accessibility)?
- [ ] **Toggle grab** instead of hold, if fingers get tired in the tower (not a focus now).
- [ ] **Demo design points.** See "Still to decide for the demo" in the game idea document.
- [ ] **Bell Tower grab window (full game).** Does the 10-second rope grab happen before her line, or during it?
- [ ] **Test order for the full game** (decide when the demo is done).
- [x] **Johnny's stress levels.** Decided 2026-10-20 (-1 to 6), replaced 2026-11-09 by the 10-bar stress system.

---

## Log

| Date | What happened |
|---|---|
| 2026-11-11/12 | Build steps 1–3 done. Step 1: Input Actions on the 2026-11-09 controls (LeftHand renamed to Grab; GripLeft, GripRight and TurnHand removed; feet on A/D; Breathe on Left Shift, Skip, Pause, LookSensitivity and HoldSensitivity added); `HandTurn` to `Scripts/Spare`; `SeatedLook` and `StampHolder` read Grab. Step 2: zoom while holding, look slower while zoomed (Field of View ratio); holding left as it is until `Grabbable`. Step 3: rough breathing test with `Stress`, `Breathing` and `BreathBox`; bar fills from the middle, hidden while not breathing, result word in the middle for 0.8 s in green/blue/red, no word when more than 1 s off, a Bad in cancels the breath, the out shrinks from where the in stopped, quiet Bad after 7 s. Grabbing redesigned: the camera stays when grabbing, the hand reaches, the body leans only when the object is past the hand's reach (steps 4–5). Sensitivity popup says "increased"/"decreased". |
| 2026-11-10 | The demo becomes a small complete game (goals: learn, give people something to play, test the idea), about 30 minutes. Two kinds of builds: rough test versions for friends, and the finished release demo. Stage 5 rebuilt: her ending, title and description, your statistics, "how others answered" if possible, a collection screen (title pieces, titles got, notes found), in-game achievements, saving after each room and at the end. Steamworks learned in the demo: cloud saves shared with the full game; achievements given in the full game from the demo save. Red stamp hatch decided: fake hole. |
| 2026-11-10 | First test version reworked into 31 steps in six groups. A rough breathing test right after the controls; one question works from start to end before the opening. New: lean forward while holding (camera and body), papers from a printer along fixed paths and out through a shredder in the desk (no physics), green stamp in a stand and the red stamp rising from a hatch on question 2 (fake hole first, so the bought table needs no changes), room layout in grey boxes at real sizes before the opening, Johnny's tied left hand and wake-up poses, pause menu, choice log saved to a file for testers, small boot flow, room design with bought assets, build sent as a rar on Google Drive or OneDrive. Collectables are marks and notes from earlier subjects. Demo saving suggested: no mid-run saves, only collectables, lines heard and settings. Voice lines are written as we build. |
| up to 2026-11-09 | Found in the project but not logged before: Input Actions changed to the 2026-11-01 controls (old rework step 1), `SeatedLook` without Shift, gripping with D, zoom on W. Johnny's dummy model (Mixamo Remy) with sitting idle, `JohnnyAnimator`, right arm IK with Animation Rigging (`ArmRig`, `RightArmIK`, `RealHandTarget`) and `HideHead`; grey-box arm cubes hidden. Problem: stamp and hand have different reaches, so the stamp is sometimes too far from the palm; the stamp and hand system will be reworked. |
| 2026-11-09 | Controls redesigned again for the demo (6 inputs): objects lead the hand. Look at an object and hold left mouse, the right hand picks it up and it follows the gaze inside its zone (`Grabbable`); the view is limited to the zone while holding. Both mouse buttons for two-hand objects (rope). W zoom (also while holding, also "interact"), Space use, A/D feet (circus), Shift breathe, Enter hold to skip heard lines, Esc pause, arrows for sensitivity. No Q/E, no Z/C, left hand only for the rope. New stress system: 10 bars (Stable, Elevated, Critical, Overload) with shake, blur and muffled voice; stress is added (`stress` CSV column, room code, pain jumps), never falls by itself; breathing with Shift (4 in, 6 out) judged Perfect/Good/Bad lowers it; stress recorded with every answer. Office becomes a 3–4 minute opening: dark wake-up with high stress teaching breathing, three lights turned on by zoom-and-hold, her introduction, right hand freed, 6–8 paper questions (green stamp alone first), lights-out transition. Her character defined (judges how he chooses, not what; cold when explaining, alive when reacting; almost never says "Johnny"), with four demo ending lines. Boot flow: first-launch settings, warnings, title screen, contract. Collectables: 1 in the office, 3 per room. Tunnel rides replaced by blackout transitions. First test version planned: title screen and office for a few friends. Recorded in the doc "Under Pressure — Controls and Stress". |
| 2026-11-01 | Controls redesigned for the whole game and the office reworked into the place where he learns his body (see "Controls" and "The office (reworked)"). No Shift: no mouse button = look around, holding left/right mouse = move that hand (both = both hands), A/D grip and keep gripping, W zooms only while looking, Q/E and Space on the active hand (button held, else last used), Z/C feet (office: Z green/yes, C red/no, final). Hands stay where left, reset only at a room change. Office: dark wake-up with three lights turned on by zooming, about 5 paper questions (right hand freed first, then left), about 4 yes/no foot questions; replaces the wake-up animation. Game show: head presses the button with Space (tap or hold); at most 6 fingers, thumb and index finger stay so the tower rope can be held; fingers from question 4 or 5. Tower: both hands on the rope with A and D. Circus: feet. Stamping rules decided (see "Stamping and ink"). Build order for the rework in nine steps. |
| 2026-11-01 | Stamping built (old controls). New `StampPress` (on HandArea): Space presses the held stamp down onto what a ray finds under it; stays down while held; `Depth` lowers the wrist in `DeskHand`, `IsPressing` locks `DeskHand`, `HandTurn` and letting go in `StampHolder`. New `InkMark` prefab and script: a Quad with a copy of the stamp's ink material, alpha by ink strength, Render Queue 3000 + mark count so the newest is on top. `Stamp`: `inkMaterial`, `inkUses` 5, `UseInk`, refilled when it arrives home, `IsGliding`. Ink materials `Ink_Green`, `Ink_Red` (URP Unlit, Transparent). |
| 2026-10-31 | Zoom built. New `CameraZoom` on the camera: holding the right mouse button while looking around (Shift) changes the Field of View smoothly (`SmoothDamp`) from the Inspector value (60) to `zoomedFieldOfView` (30). Letting go of the button or of Shift zooms out. `zoomWithoutShift` off. Look sensitivity stays the same while zoomed. |
| 2026-10-31 | Fixed state, Shift look, grey-box arm, picking up and hand turning built. `SeatedLook` has two states: fixed (main view, follows the Wrist softly with `SmoothDamp`, `followAmount` 0.3) and Shift (free look). New in `Scripts/Player`: `DeskHand` (wrist in `HandArea`, mouse sensitivity 0.0001), `ArmPose` (elbow follows the wrist forward/back, cubes stretched between points), `StampHolder` + `Stamp` (left mouse picks up the nearest stamp within 10 cm; it glides into `Grip` and becomes its child, and glides home when let go), `HandTurn` (Q/E, 80° left, 20° right, 120°/s). Input Actions cleaned up: Player map has only Look, LookAround (Shift), Pickup (left mouse), Zoom (right mouse), Stamp (Space), TurnHand (Q/E as one axis). Controls changed: Space stamps, right mouse zooms, Q/E turn the hand. Grey-box hands are now built instead of skipped. |
| 2026-10-20 | Office grey boxes started. `OfficeSet` with desk, chair, walls, stamps and the camera at his eyes (0, 1.2, 0, tilted 40° down). `SeatedLook` turns the camera with the mouse (Input Actions `Player/Look`), with limits; Max Up 70 because "straight ahead" is 40° down. `Paper` prefab and script; `RoomBase.ShowQuestion` (new, called between `before` and `voice`) lets `OfficeRoom` lay 1 to 4 papers on `PaperCenter`. `OfficeRoom.AskQuestion` waits until stamping is built; `KeyboardTestRoom` still answers with number keys and calls `base.Transition` to remove the papers. Controls agreed (fixed state, Shift to look, middle mouse to zoom). New ideas: the radio, the freed right hand, the wake-up animation. |
| 2026-10-20 | Breathing finished. `breath` column added to the question CSV (`A:2|any:1|none:2`), applied right after his answer, before her reaction; `QuestionLoader.ReadAnswerList` reads both `react` and `breath`. `BreathFalling` on the QuestionFlow object: rules per room (office: 2 → 1 after 1 quiet question, 1 → 0 after 2); a quiet question has no `breath:` anywhere, and setting the same state again resets the count. Timer moved to Stage 2. |
| 2026-10-20 | Johnny's breathing built. New `JohnnyBreathing` (own AudioSource) and `ClipVariants` (random variant, never twice in a row). States -1 to 6 by number; clips named `JOH_BREATH_<state>_<number>`. `breath:2`, `breath:5_03` and `breath:-1` work in any clip list; the caption shows only when the state changes, from rows `JOH_BREATH_0`, `_1`, `_2` in `VoiceLines.csv`. Johnny's one-time sounds: `JOH_CRY` picks a random variant, `JOH_CRY_02` an exact one, caption from the group row; his breathing pauses while they play. Intro uses `breath:2` after the mouth line and `breath:0` before "Let's begin." Falling changed to "go to a set state". Old `JOH_BREATH_FAST`/`SLOW` moved to Spare. Scripts sorted into Core, Data, Audio, Rooms and Testing. |
| 2026-10-18 | Johnny's own channel and captions built. `JohnnyVoice` has its own AudioSource and caption text; `VoicePlayer` sends every line whose speaker is `Johnny` there and doesn't wait for it, so she talks over him. `wait:1.5` in any clip list (intro, end, CSV columns) is a pause; the intro starts with `JOH_MUM_WAKE`, `wait:3`. Captions come from `VoiceLines.csv` without brackets, the code adds them; an empty cell means no caption. His caption stays for his whole clip, independent of her subtitle. Subtitle and caption rebuilt as UI TextMeshPro (not the 3D kind); Canvas Scaler match 0.5. Mumble captions are placeholders. |
| 2026-10-18 | Her special reactions built. `RoomBase` has a new `PickReaction` a room can replace; `OfficeRoom` uses it. Q12 contradiction ("You said you work well under pressure.") plays only if he accepted Q7 and picks B (freeze) or C (panic) on Q12. Decline-all: silence on questions with no `none` reaction, then "None of them. I see.", then "None again." `ChoiceLog.Find` looks up an earlier answer. `KeyboardTestRoom` now builds on `OfficeRoom` for testing. Reactions stay fixed: one clip per answer, never random. Q12 keeps "I panic." Input Actions come later, with the real stamping. Claude sends changed scripts as files; I copy them in myself. |
| 2026-10-18 | Johnny gets his own audio channel so he can mumble and breathe while she talks. His sounds get captions in a second text above hers. Breathing driven by script-set stress levels with random variants per level (see "How Johnny's breathing works"). |
| 2026-10-16 | Voice player steps 1 to 3 done. Clips live in `Assets/Audio/Resources/Voice/<Room>/`, one folder per room plus a shared `Johnny` folder, each clip in one folder only. Spare clips in `Assets/Audio/Spare` (not in the build). Import settings: mono, Compressed In Memory, Vorbis. |
| 2026-10-16 | Voice player planned. Subtitles live in `VoiceLines.csv` (id, speaker, one column per language, `en` first). Question CSV gets `before`, `voice` and `react` columns, clips split by `\|`; `react` reads like `A:…\|B:…\|none:…\|any:…`, silence if no entry. Q12 contradiction and second decline-all reactions are code in `OfficeRoom`. Intro and end lines are lists on `QuestionFlow`. New `tagsNone` column for declining everything: Q13 `clever`, Q14 `clever\|more lives`. Office tags only on Q9 (doctor `useful`, brother `family`), Q10 (pregnant woman `more lives`, you `self`) and Q14 (stranger `family`). Tags are only for titles; titles decided after the tests are built. Q1 text: "I only tell the truth." `OFF_RULES_06b` is a spare. |
| 2026-10-16 | Office set to 15 questions (1 to 4 papers each). Voice script and AI placeholder voices done. One fixed voice version per line. Johnny's mouth is sewn in the demo. Host is in the demo. "Every paper needs a stamp" line added. |
| 2026-10-14 | CSV question format agreed. Office: four-choice papers, stamping rules, no timer, secret title for declining everything. |
| 2026-10-14 | Plan changed: demo first, rooms in order, two projects. |
| 2026-10-13 | Build plan created. |