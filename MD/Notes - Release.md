# Current test run scratchpad
- Need to update Cost on button to reflect this change.
- Consider an upgrade to SBD.
  - First level: half damage
---

# New Feature Implementations

Add Conflict: Near-Harmless/ All Equipment traits

---

# Fixes
- Traits with Contraindications still show up in upgrade menu.
  - This is apparently an issue with RogueLibs.

## Stove 
- Can't grill again. Operating Bar Error 2.
  - FlamingBarrel works fine, but also shows message.
  - Therefore, Operating Bar Error is not the terminus of the stove code. It's failing sometime after that.

```
Stove:

[Message: Bunny Mod] ObjectReal_Interact: Stove (1510)
[Message: Bunny Mod] Stove (1510): ObjectReal_PressedButton
[Message: Bunny Mod] PlayfieldObject_Operating Stove (1510):
[Message: Bunny Mod] Agent = Playerr
[Message: Bunny Mod] Logging error
[Message: Bunny Mod] Stove (1510): ObjectReal_FinishedOperating
[Error  : Unity Log] Operating Bar Error 2

FB:

[Message: Bunny Mod] FlamingBarrel (1621): ObjectReal_FinishedOperating
[Error  : Unity Log] Operating Bar Error 2
```

# Balance

## Telemancy
- Focused casting should give a base boost to accuracy. Maybe a floor to certain rolls?
- Need AV indicator & stop charging at full charge 
- Felt too accurate with just Magic Training 1. Miscast didn't trigger once. Ensure that sub-1 float values for miscast chance aren't rounding to zero for an int operation.
- Testing:
	- Initiate
	- Wild Caster
    - Focused Caster
    - Magic Training
    - WC + MT
    - FC + MT

---

# Finalization
- Rename *only when the code is complete*.
- Complete and win a run with each new feature, and successfully load a return to home base. This would be a full cycle of the code you can expect to run into.
- Make a promo character for each special ability or trait group, to promote the mod and community at large.
- Update Sprites
- Increment the Version Number!

## Release Notes 

- Added Chronomancy & Pyromancy special abilities
- Reworked Telemancy completely, and renamed existing general magic traits
- Stoves now explode like Molotovs when destroyed. Trying to cook on it near the owner may have them interrupt you.
- Fixed various bugs