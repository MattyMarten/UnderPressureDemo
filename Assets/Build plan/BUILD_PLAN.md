# Who Are You UNDER PRESSURE? — Build Plan

The game design lives in the game idea document. This file is only about **building** it: the stages, what is done, and what is still to decide.

Tick a box by changing `- [ ]` to `- [x]`.

---

## Current status

- **Where we are:** Stage 1 — Johnny's breathing works: states set from clip lists (`breath:2`), random variants, one-time sounds. Scripts sorted into folders.
- **Next step:** the `breath` column in the question CSV (answer-based breathing), then falling (after quiet questions, go to a set state).

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
- [ ] Timer (not in the office): starts after she finishes speaking, 20 down to 10 seconds per part
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
- [ ] Johnny's breathing: stress levels set by the script (see "How Johnny's breathing works" below)
  - [x] `JohnnyBreathing`: own AudioSource, loops random variants of one state, quick fade on change, pauses while he makes a one-time sound
  - [x] `breath:2`, `breath:5_03` (start with that clip) and `breath:-1` (silent) in any clip list
  - [x] One-time sounds: `JOH_CRY` plays a random `JOH_CRY_..`, `JOH_CRY_02` that exact clip
  - [ ] `breath` column in the question CSV: `A:2|any:1`
  - [ ] Falling: after quiet questions, go to a set state
- [ ] Tune the intro: pause lengths between her lines, how much Johnny mumbles
- [x] Office voice script written ("Demo voice lines: Office and ride")
- [x] Office voice lines: AI placeholders
- [X] Johnny's sounds (mumbles, breathing), recorded by me -PAIN for game show still to do
- [ ] Final voices recorded by real people (later)
- [ ] Controls shown on screen or on objects
- [ ] Debug tools: jump to any room, skip questions

### How Johnny's breathing works

Agreed 2026-10-18, changed 2026-10-20. Set rules and variants built; the CSV column and falling not yet.

- **A stress level** picks which breathing plays. It follows the script, not the player's state: every question and answer says where it goes, so we check that the breathing sounds right whatever he answers.
- **States, by number:** -1 silent, 0 idle, 1 bit stressed (like idle, a bit louder and faster), 2 fast breathing, 3 stressed (with a little mumble), 4 scared (uneven breathing), 5 pain (crying and mumbling), 6 really painful (mumbled curses and crying). Numbers only, no names in the code; the list is also a comment at the top of `JohnnyBreathing.cs`. The office uses -1, 0, 1 and 2.
- **Two kinds of rules:** *set* (go exactly to a level, a fixed moment) and *answer-based* (answer A → 3, B → 2). Later they can live in a `stress` column in the question CSV, written like `react`: `A:3|B:2|any:…`.
- **Falling:** counted in questions, not seconds. Always *go to a set state*, never "down 1", so he can never fall into a state we didn't choose. Example: no new event for 2 questions → go to 1.
- **Room change:** back to 0 or 1.
- **Pain** (a lost finger): the pain sound plays, then the pain level starts. We make sure the way back down (pain → panic → …) never happens where we don't want it.
- **Variants:** each state can have several breathing clips. One is picked at random, never the same twice in a row. File names: `JOH_BREATH_<state>_<number>`, two-digit numbers (`JOH_BREATH_2_01`), so new variants are just new files. His other sounds work the same way (`JOH_CRY_01`, `JOH_LAUGH_01`…), with one caption row per group in `VoiceLines.csv` (`JOH_CRY`). Her lines are never random.
- **Switching levels:** start with a quick fade. Later, try transition breaths (from pain: one deep breath, then the lower level; from panic: two breaths in…).

### The office (grey boxes)

- [ ] Desk, papers, green and red stamps
- [ ] One-paper questions (accept or decline)
- [ ] Questions with 2 to 4 papers, one option on each (only one can be accepted)
- [ ] Stamping: green once per question, red as often as needed; the question is done when every paper has a stamp
- [ ] Papers go away, next question appears
- [ ] Johnny's mouth sewn shut: mumbles and breathing only
- [x] 15 questions in CSV
- [ ] Ending line: "Great, now let's start the main test."

## Stage 2 — The game show

- [ ] Foot button
- [ ] First one or three questions free (to decide), then one finger per press
- [ ] Fingers taken from one hand first, then the other (at most seven, or nine with one free press)
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

- [ ] Two hold buttons, one per person; letting go drops that person
- [ ] Rope is visual only
- [ ] Questions 9 and 10 cost hands (or arms)
- [ ] A lost hand cannot hold its side on question 10
- [ ] 10 questions in CSV

## Stage 4 — The circus

- [ ] Foot buttons tilt the board between three states (left, middle, right)
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
- [ ] Title screen and settings (subtitles, volume, photosensitivity)
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

- [ ] **Tower with missing fingers (demo).** If the game show took five fingers from one hand, can that hand still hold a rope?
- [ ] **Free presses (demo).** One or three free presses at the game show?
- [ ] **Office answer times (demo).** Do they count for a modifier? What does declining every paper make him?
- [ ] **Demo design points.** See "Still to decide for the demo" in the game idea document.
- [ ] **Bell Tower grab window (full game).** Does the 10-second rope grab happen before her line, or during it?
- [ ] **Test order for the full game** (decide when the demo is done).
- [x] **Johnny's stress levels.** Decided 2026-10-20: -1 to 6, see "How Johnny's breathing works". The office uses -1, 0, 1 and 2: it should stress him, not make him panic.

---

## Log

| Date | What happened |
|---|---|
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