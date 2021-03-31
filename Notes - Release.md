# New Feature Implementations

All done for this release!

---

# Fixes
- Traits with Contraindications still show up in upgrade menu.
  - This is apparently an issue with RogueLibs.

## Chronomancy
- Miscast does not reset timescale back to normal after reversion.
  - // 202103301945
```
[Message: Bunny Mod] ChronomancyRollTimescale: 0.25
[Message: Bunny Mod] ChronomancyStartMiscast: 0.25
[Error  : Unity Log] DivideByZeroException: Attempted to divide by zero.
Stack trace:
BunnyMod.BunnyAbilities+<ChronomancyStartMiscast>d__23.MoveNext () (at <79149b024a354f66ad2245260c0036ff>:0)
--- End of stack trace from previous location where exception was thrown ---
System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw () (at <c79628fadf574d3a8feae0871fad28ef>:0)
System.Runtime.CompilerServices.AsyncMethodBuilderCore+<>c.<ThrowAsync>b__6_0 (System.Object state) (at <c79628fadf574d3a8feae0871fad28ef>:0)
UnityEngine.UnitySynchronizationContext+WorkRequest.Invoke () (at <73b499366e5241bda47e5da76897738b>:0)
UnityEngine.UnitySynchronizationContext:ExecuteTasks()
```

## FinishedOperating "Cancelled" Message
- Cancelled message
```
    [Message: Bunny Mod] FlamingBarrel (1156): ObjectReal_FinishedOperating
    [Error  : Unity Log] Operating Bar Error 2
```

  - This message originates  in PlayfieldObject.Operating. Since we're getting the "Canceled" message, it's at least getting past that part. We don't want it going down that lemma. My best initial guess is that we need to make stoppedOperating false somehow. Conditions that enable StoppedOperating to be true:
		- PFO's objectsprite is not on agent's interactionhelper's triggerlist && not operatingFar. I think Triggerlist might be a list of all PFOs within interaction range.
		- InteractingAgent == null
		- interactingagent.mustStopOperating = true;
		- interactingagent != myAgent
		- this.ServerSaysStopOperating

## Magic General
- Cooldowns should not have any AV indicators, unless they're subtle. They're very routine so it should be unobtrusive. Recharge should only have AV if you're burned out and just recovered, not for routine recharge.

## Pyromancy
- Cooldown triggers on miscast or runout, not just on release. Need to ensure that player can hold down and it'll keep recharging.

- BurnedOut stays True, can't cast anymore
  - This occurs during Miscast. Does not *appear* to happen if RMB is pressed after miscast - only the miscast triggers, from the look of it.
    - // 202103301952
      - Fixed
- Trying to cast at zero mana plays that horrible sound

- Equipped Weapon appears in duplicate/larger form (like fist) when using
	- Tried removing HideGun from StartCast. 

## Stove 
- Can't grill again. FML. Operating Bar Error 2.

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