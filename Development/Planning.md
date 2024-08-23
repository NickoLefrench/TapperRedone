# Planning for Tapper Project

## Meta-planning

What is the best way to figure what's left to do, what's done, what's to fix?
For now, we can put things here and/or chat, but we could also do a bit of dev tools
Something Kanban-like, like Miro or Github Projects.
Alternatively, we can maintain some kind of note system, like Obsidian, within this Github repo

## General order of things to do

We don't want to start doing menus when we don't know what the game even plays like. Let's start relatively slow.

### Systems
- [x] Set up player movement
	- [x] Movement is currently physics-based.
	- [x] Movement uses direct key bindings, instead of Axis bindings
- [x] Set up colliders for physics and interactions
	- [x] Currently, both event-based and polling-based collision checking is done - are both necessary?
- [ ] Inventory system
	- [x] Bartender has an inventory and can carry multiple things
	- [ ] Whatever is carried is visible through prefabs spawning on a "tray"
	- [x] Bartender can pick up items from "sources", up to limits
	- [x] Bartender can drop things at "sinks"
	- [ ] Inventory can "craft" (if inventory has X combination, can replace it by Y combination)
- [x] Tunables
	- [x] Think of a way to store any "design" variables - score per drink, time of minigames, etc.
	- [x] Store it in data
- [x] Score mechanics

### Gameplay
- [x] Beer pouring interaction
	- [x] Interact with tap
	- [x] Mini-game
	- [x] Beer added to inventory on end of interaction
	- [x] Can deliver beer to a different interactable (say a trash can)
- [ ] NPCs
	- [x] One static NPC with only beer orders
	- [x] NPC can arrive, wait, order, wait, drink, leave money, leave
	- [ ] Many NPCs, with different places to go to
	- [x] Waiting timer on NPCs
- [ ] Cocktails
	- [ ] Interact with ingredients and carry in inventory
	- [ ] Shaker mini game
	- [ ] NPCs can order cocktails too
	- [ ] NPC reaction to correct/partial/bad cocktail

### UI
- [ ] (optional) Game Flow State Machine to know what screen you're in, manage game running, manage transitions
- [ ] Can load a Pause menu with Escape, which pauses gameplay
- [ ] Create main menu UI, which loads by default and gameplay doesn't
- [ ] New Game button to start game and hide main menu
- [ ] Additional buttons - Settings, Credits, etc.

### Misc
- [ ] Audio
- [ ] Playtesting
- [ ] Tutorial
- [ ] Additional ideas from GDD