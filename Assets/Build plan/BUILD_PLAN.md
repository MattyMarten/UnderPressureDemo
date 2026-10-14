# Who Are You UNDER PRESSURE? — Build Plan

The game design lives in the game idea document. This file is only about **building** it: the stages, what is done, and what is still to decide.

Tick a box by changing `- [ ]` to `- [x]`.

---

## Current status

- **Where we are:** Stage 0 done (except the first physical backup).
- **Next step:** Stage 1 — core systems, starting with the office.

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

- [ ] Question flow, with a transition step each room fills in its own way
- [ ] Timer: starts after she finishes speaking, 20 down to 10 seconds per part
- [ ] Questions loaded from CSV files
- [ ] Choice log: every decision recorded as an event, with the question's tags
- [ ] Voice line player with subtitles
- [ ] Controls shown on screen or on objects
- [ ] Debug tools: jump to any room, skip questions

### The office (grey boxes)

- [ ] Desk, papers, green and red stamps
- [ ] One-statement papers (accept or decline)
- [ ] Two-option papers (stamping one accepts it and declines the other)
- [ ] Paper goes away, new one appears
- [ ] 10 questions in CSV
- [ ] Ending line: "Great, now let's start the main test."

## Stage 2 — The game show

- [ ] Foot button
- [ ] First three questions free, then one finger per press
- [ ] Fingers taken from one hand first, then the other (at most seven)
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
- [ ] **Demo design points.** See "Still to decide for the demo" in the game idea document.
- [ ] **Bell Tower grab window (full game).** Does the 10-second rope grab happen before her line, or during it?
- [ ] **Test order for the full game** (decide when the demo is done).

---

## Log

| Date | What happened |
|---|---|
| 2026-10-14 | Plan changed: demo first, rooms in order, two projects. |
| 2026-10-13 | Build plan created. |
