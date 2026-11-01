# Who Are You UNDER PRESSURE? — Build Plan

The game design lives in the game idea document. This file is only about **building** it: the stages, what is done, and what is still to decide.

Tick a box by changing `- [ ]` to `- [x]`.

---

## Current status

- **Where we are:** Stage 1 — the office in grey boxes. Right hand, picking up, turning, zoom and stamping with ink all work (still on the old controls). On 2026-11-01 the controls were redesigned for the whole game (two hands, feet, no Shift) and the office was reworked into the place where he learns his body. See "Controls" and "The office (reworked)".
- **Next step:** step 1 of "Office rework: build order": the new Input Actions. Then looking without Shift, and the second hand.

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
| Build order | Demo rooms in playing order: office, game show, tower, circus |
| Trailers | One for the demo, one for the full game |
| Build plan | This file, kept in the project and in Git |

---

# Part 1 — The demo

The demo is one run of four rooms, 40 questions, about 10 to 15 minutes. No money, no saving. See "The demo" in the game idea document.

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
- [x] Voice line player with subtitles
  - [x] `VoiceLines.csv` (id, speaker, one column per language) and its loader
  - [x] `VoicePlayer`: plays a clip, shows its subtitle, waits until it ends
  - [x] Intro and end lines as lists on `QuestionFlow`
  - [x] `before`, `voice` and `react` columns in the question CSV
  - [x] `tagsNone` column (tags for declining everything)
  - [x] 15 real office questions in `Office.csv`
  - [x] Her special reactions as code in `OfficeRoom`: the Q12 contradiction and the second decline-all
- [x] Johnny on his own audio channel: his mumbles and breathing play at the same time as her voice, not after it
- [x] Johnny's captions: a second subtitle text just above hers, smaller and grey or italic, in square brackets ("[muffled protest]"). Breathing gets a caption only when it changes
- [x] Johnny's breathing: stress levels set by the script (see "How Johnny's breathing works" below)
  - [x] `JohnnyBreathing`: own AudioSource, loops random variants of one state, quick fade on change, pauses while he makes a one-time sound
  - [x] `breath:2`, `breath:5_03` (start with that clip) and `breath:-1` (silent) in any clip list
  - [x] One-time sounds: `JOH_CRY` plays a random `JOH_CRY_..`, `JOH_CRY_02` that exact clip
  - [x] `breath` column in the question CSV: `A:2|any:1`
  - [x] Falling: `BreathFalling` rules per room (from, to, after quiet questions). A state with no rule never falls
- [ ] Tune the intro: pause lengths between her lines, how much Johnny mumbles
- [ ] ~~Wake-up animation~~ Replaced on 2026-11-01 by the dark wake-up with three lights (see "The office (reworked)"). The player looks around himself instead of an animation.
- [x] Office voice script written ("Demo voice lines: Office and ride")
- [x] Office voice lines: AI placeholders
- [X] Johnny's sounds (mumbles, breathing), recorded by me -PAIN for game show still to do
- [ ] Final voices recorded by real people (later)
- [ ] Controls shown on screen or on objects
- [ ] Debug tools: jump to any room, skip questions

### How Johnny's breathing works

Agreed 2026-10-18, changed 2026-10-20. All built.

- **A stress level** picks which breathing plays. It follows the script, not the player's state: every question and answer says where it goes, so we check that the breathing sounds right whatever he answers.
- **States, by number:** -1 silent, 0 idle, 1 bit stressed (like idle, a bit louder and faster), 2 fast breathing, 3 stressed (with a little mumble), 4 scared (uneven breathing), 5 pain (crying and mumbling), 6 really painful (mumbled curses and crying). Numbers only, no names in the code; the list is also a comment at the top of `JohnnyBreathing.cs`. The office uses -1, 0, 1 and 2.
- **Two kinds of rules:** *set* (go exactly to a level, a fixed moment) and *answer-based* (answer A → 3, B → 2). Later they can live in a `stress` column in the question CSV, written like `react`: `A:3|B:2|any:…`.
- **Falling:** counted in questions, not seconds. Always *go to a set state*, never "down 1", so he can never fall into a state we didn't choose. Example: no new event for 2 questions → go to 1.
- **Room change:** back to 0 or 1.
- **Pain** (a lost finger): the pain sound plays, then the pain level starts. We make sure the way back down (pain → panic → …) never happens where we don't want it.
- **Variants:** each state can have several breathing clips. One is picked at random, never the same twice in a row. File names: `JOH_BREATH_<state>_<number>`, two-digit numbers (`JOH_BREATH_2_01`), so new variants are just new files. His other sounds work the same way (`JOH_CRY_01`, `JOH_LAUGH_01`…), with one caption row per group in `VoiceLines.csv` (`JOH_CRY`). Her lines are never random.
- **Switching levels:** start with a quick fade. Later, try transition breaths (from pain: one deep breath, then the lower level; from panic: two breaths in…).

### Controls

Agreed 2026-10-20, redesigned 2026-11-01 for the whole game. The same in every room: the player learns them in the office. Left hand on the keys (little finger free, ring finger A, middle finger W, index finger D, thumb Space), right hand on the mouse.

| Input | What it does |
|---|---|
| Mouse, no button held | Look around (turn his head) |
| Left mouse button (hold) | The mouse moves his left hand; the camera follows it softly |
| Right mouse button (hold) | The mouse moves his right hand |
| Both mouse buttons (hold) | Both hands move together, the same way, each inside its own area |
| W (hold) | Zoom, only while looking around (no mouse button held) |
| A / D (hold) | Left / right hand grips (a stamp, the rope). Keeps gripping even when that hand isn't active |
| Q / E (hold) | Turn the active hand |
| Space | Use: stamp with the active hand's item. Game show: his head presses the button (tap, or hold to keep his face on it) |
| Z / C | Left foot / right foot press. Office: Z = green (yes), C = red (no) |
| Esc | Pause and settings (later) |

- **No Shift any more.** Not holding a mouse button = looking around. There is no automatic return to a main view: the head stays where he left it, and grabbing a hand turns the camera softly to that hand.
- **Active hand:** the hand whose mouse button is held, or else the last hand used. Q/E and Space work on it, so he can let go of the mouse, look at the paper, and still stamp.
- **Hands stay where they are left**, and keep holding while A or D is held. No automatic reset inside a room; hands reset only at a room change (during the ride).
- **Each hand has its own desk area**; the two areas overlap in the middle. The left hand's turning limits are mirrored from the right hand's.
- **Feet** only press, they never move.
- **Room by room:** office = all of it, learned step by step. Game show = Space (head). Tower = both hands on the rope (A and D), looking and zoom allowed while holding. Circus = feet (Z/C), looking and zoom allowed.

### Stamping and ink

Built 2026-11-01 (on the old controls; it moves to A/D and the active hand in the rework).

- **Press:** Space pushes the held stamp down onto whatever is under it, paper or desk (`StampPress`, a ray straight down). It stays down while Space is held and comes up when Space is let go. While pressing, the hand can't move or turn, and the stamp can't be let go.
- **Ink mark:** appears where the stamp touched, turned like the stamp (`InkMark` prefab: a Quad with the stamp's ink material, `Ink_Green` / `Ink_Red`, URP Unlit, Transparent; an image can be put in the Base Map later). A mark on a paper becomes the paper's child; desk marks stay under `OfficeSet` for the whole room. Each new mark is drawn on top (Render Queue 3000 + count).
- **Ink pools:** each stamp has 5 uses, each mark fainter (1, 0.8, 0.6, 0.4, 0.2), the 6th press leaves nothing. Going back to its spot refills it.
- **Agreed, not built yet:** a "Stamp here:" box on each paper, about twice the stamp's size. A stamp counts when about 75% of its face is inside the box; its outline goes darker. Only the first counting stamp on a paper is its answer; more stamps still leave marks. Green counts once per question: green on a second paper leaves a mark and she says "You can only accept one paper." Stamping a paper again with the other colour plays her overwrite lines (see "New voice lines").

### The arm (grey boxes)

Built 2026-10-31. Everything is a child of `HandArea` (on the desk surface, middle of the desk).

- **Shoulder** (fixed) → **Elbow** → **Wrist** → **Hand** (→ Palm, Grip).
- `DeskHand` moves the **Wrist** inside the area (1.2 × 0.6 m, height 0.06 above the desk). Only the wrist is kept in the area.
- `ArmPose` moves the **Elbow** forward and back with the wrist (`forwardFollow` 0.7; `sideFollow` 0, so left/right doesn't move it) and stretches the UpperArm and Forearm cubes between the points, in `LateUpdate`.
- The **Hand** always points forward; `HandTurn` turns it around the wrist with Q/E.
- A held stamp becomes a child of **Grip** (an empty object with scale 1, never a scaled cube). Each `Stamp` remembers its spot and glides itself.
- **Second hand (rework step 3):** a mirrored left arm with its own `HandArea`, overlapping the right one. `DeskHand`, `ArmPose`, `HandTurn`, `StampHolder` and `StampPress` become one per hand.
- Later, with the real arm model: Unity's Animation Rigging "Two Bone IK" can bend a real elbow so the arm never stretches. Shoulder, Elbow and Wrist are the points it needs.

### The office (grey boxes)

- [x] Grey-box room (`OfficeSet`): floor, walls, desk, chair, green and red stamps, camera at his eyes. Materials in `Art/Materials`
- [x] `Paper` prefab (sheet + 3D TextMeshPro) with the `Paper` script
- [x] Papers on the desk: `OfficeRoom.ShowQuestion` lays 1 to 4 papers in a row on `PaperCenter` (0.24 m apart), after her `before` lines and before her question line. A single paper shows the statement, the others one option each
- [x] Fixed state: the mouse moves his wrist in a desk area (`DeskHand`); the camera follows it softly (`SeatedLook`)
- [x] Hold Shift to look around (`SeatedLook`); letting go turns the camera back to the main view *(replaced in the rework: no Shift)*
- [x] Grey-box right arm: shoulder, elbow, wrist, hand (`ArmPose`)
- [x] Picking up a stamp: hold the left mouse button; it glides into the hand in the hand's rotation, and glides back to its own spot when let go (`StampHolder`, `Stamp`) *(moves to A/D in the rework)*
- [x] Turning the hand with Q and E (`HandTurn`)
- [x] Zoom with the right mouse button while holding Shift; letting go of either zooms out (`CameraZoom`) *(moves to W in the rework)*
- [x] Stamping with Space: press and hold, ink marks on paper or desk, ink pools with 5 fading uses (`StampPress`, `InkMark`)
- [x] 15 questions in CSV
- [ ] The radio her voice comes from (maybe 3D sound)
- [ ] Johnny's mouth sewn shut: mumbles and breathing only
- [ ] Papers move away, next question appears (now they just disappear)
- [ ] Ending line: "Great, now let's start the main test."

### The office (reworked)

Decided 2026-11-01. The office is where he learns his body: look, zoom, hands, feet. Each later room uses a part of it.

1. **Dark wake-up.** Dark room, hands and feet locked, some mumbles. On-screen text: look around with the mouse, zoom with W. Three small lights: zooming on each one flicks its switch and more light comes on. After the third, her first line ("Oh, you're finally awake.").
2. **Hands, about 5 paper questions.** The right hand is freed first. The left hand is freed when the first paper lands on the left side of the desk.
3. **Feet, about 4 yes/no questions.** She tells him to press the buttons under his feet. Z = green (yes), C = red (no). The first press is final.

#### Office rework: build order

- [ ] 1. New Input Actions: hold left/right mouse (select hand), A/D (grip), W (zoom), Space (use), Q/E (turn), Z/C (feet). Remove `LookAround` and `Pickup`
- [ ] 2. Looking without Shift: no button = free look, a hand button = follow that hand. Zoom on W, only while looking
- [ ] 3. The second hand: mirrored left arm and area; active hand (button held, else last used); both buttons move both hands
- [ ] 4. Gripping with A/D (keeps holding when the hand isn't active); Space and Q/E on the active hand
- [ ] 5. "Stamp here:" box: the paper reads its stamp (about 75% inside, first counting stamp only, green once), outline goes darker
- [ ] 6. Answering by stamps instead of number keys, with her overwrite and "only one paper" lines
- [ ] 7. Foot buttons (Z/C) and a yes/no question type in the CSV; first press final
- [ ] 8. Dark wake-up with the three lights
- [ ] 9. Freeing the body step by step: everything locked in the dark, then the right hand, the left hand, the feet

#### Office content

- [ ] Cut the office to about 5 paper questions and about 4 yes/no foot questions (`Office.csv` and the voice script)
- [ ] New voice lines (AI placeholders first):
  - Overwrite lines, counted per room: one line for tries 1–3, another for tries 4–5, the 6th: "As I can see this simple test might be too much for you." After that she stays quiet
  - "You can only accept one paper." (green on a second paper)
  - Instructions: looking and zoom (or on-screen text only), the hands, the foot buttons
  - "Oh, you're finally awake." after the third light

## Stage 2 — The game show

- [ ] Timer: starts after she finishes speaking, 20 down to 10 seconds per part (moved from Stage 1: the office has no timer)
- [ ] Head button: Space presses it with his head (tap, or hold to keep his face on the button)
- [ ] The first questions cost nothing; fingers are taken from question 4 or 5 (to decide), one per press
- [ ] At most 6 fingers: thumb and index finger stay on both hands, so he can still hold the rope in the tower. Fingers taken from one hand first, then the other
- [ ] Missing fingers and bleeding stay for the rest of the run
- [ ] Host voice and jingle
- [ ] Flashing lights, with a reduce-flashing setting
- [ ] 10 questions in CSV

### First playtest

- [ ] Friends play office and game show without help
- [ ] Do they argue about their choices?
- [ ] Measure how long each room takes
- [ ] Decide: continue as planned, change the design, or cut

## Stage 3 — The tower

- [ ] Both hands on the rope before each question: left hand holds with A, right hand with D. Letting go of a key drops that side's person
- [ ] Looking around and zooming allowed while holding
- [ ] Rope is visual only
- [ ] Questions 9 and 10 cost hands (or arms)
- [ ] A lost hand cannot hold its side on question 10
- [ ] 10 questions in CSV

## Stage 4 — The circus

- [ ] Feet (Z/C) tilt the board between three states (left, middle, right)
- [ ] Looking around and zooming allowed
- [ ] Picking the tilted side again makes him fall
- [ ] Timeout: board stays put
- [ ] Falling ends the run ("who fell")
- [ ] 10 questions in CSV, placed so some kind answers sit on the dangerous side

## Stage 5 — The demo, put together

- [ ] Tunnel rides between rooms, with a different line from her on each ride
- [ ] Demo title builder: modifier, rank and ending (80 titles)
- [ ] Demo description: "You are:" plus one sentence per part
- [ ] Share line ("Under pressure, I'm the …")
- [ ] End screen and the screen pointing to the full game
- [ ] Last line: "Great, Johnny, welcome to the team. I have high hopes for you."
- [ ] Title screen and settings (subtitles, volume, photosensitivity, mouse sensitivity for looking and for the hands)
- [ ] Content and photosensitivity warnings
- [ ] Real art and sound for all four rooms
- [ ] Second playtest

## Stage 6 — Demo release

- [ ] Demo trailer
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
- [ ] Title screen, three save slots, test selection screen (Closed / Open / Finished)
- [ ] Autosave after every question and every test
- [ ] Subject number: Steam ID + slot + run
- [ ] Opening scene ("Where am I?")
- [ ] Shared chair-on-rails system (Courtroom, Bunker, Casino)
- [ ] Debug tools: fake history, random runs to check titles

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
- [x] **Zoom in the fixed state.** Decided 2026-11-01: zoom only while looking around (now: W, with no mouse button held).
- [ ] **Sensitivity settings (later).** Separate settings for looking around and for moving the hand, on the settings screen. Today they are two Inspector values (`SeatedLook` and `DeskHand`).
- [x] **Turned ink mark.** Decided and built 2026-11-01: the mark lands where the stamp touched, turned like the stamp.
- [ ] **Paper timing.** Papers appear before her question line. Check that it feels right in play (moving it is one line in `QuestionFlow`).
- [ ] **Ending line.** "Great, now let's start the main test." already plays in `endLines`. Tick the office item?
- [ ] **Game idea document.** Add: the radio; the new controls (two hands, feet, no Shift); the reworked office (dark wake-up, hands, feet, about 9 questions); stamping rules (box, overwrite lines, ink pools); game show head button and the 6-finger limit; tower rope held with both hands.
- [ ] **Paper questions with two hands.** Where papers lie (left/right) so both hands are needed, and which question frees the left hand.
- [ ] **Office answer times (demo).** Do they count for a modifier? What does declining every paper make him?
- [ ] **Demo design points.** See "Still to decide for the demo" in the game idea document.
- [ ] **Bell Tower grab window (full game).** Does the 10-second rope grab happen before her line, or during it?
- [ ] **Test order for the full game** (decide when the demo is done).
- [x] **Johnny's stress levels.** Decided 2026-10-20: -1 to 6, see "How Johnny's breathing works". The office uses -1, 0, 1 and 2: it should stress him, not make him panic.

---

## Log

| Date | What happened |
|---|---|
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