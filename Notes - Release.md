# New Feature Implementations

All done for this release!

---

# Fixes
- Traits with Contraindications still show up in upgrade menu.
  - This is apparently an issue with RogueLibs.

## Chronomancy
- REALLY need a review, possibly with a flowchart of the boolean flags. Need strict definitions and to be fully aware of when they're set and read. Otherwise this is a ticking timebomb for bugs.
- Per Cherry's testing, game kept speeding up more and more. He sent a test log of this, which appeared to double the timescale every time the ability was activated, without reverting it.
    - It's possible that LMB Click/Hold is setting some boolean during cooldown or miscast, which is then flipped incorrectly when the cooldown/miscast wears off.
	- Was able to replicate this, it occurs during miscast.
      - Attempt done. Test.
- Convert timescale to be a single scale, rather than opposite operations for miscast.
  - Done. Test.

## Finalization
- Verify that all methods created are actually used, because you made them en masse as a framework before explicit need.
- Last chance to rename these special powers.

## FlamingBarrel & Stove
- Cancelled message
```
    [Message: Bunny Mod] FlamingBarrel (1156): ObjectReal_FinishedOperating
    [Error  : Unity Log] Operating Bar Error 2
```

  - This message originates  in PlayfieldObject.Operating. Since we're getting the "Canceled" message, it's at least getting past that part. We don't want it going down that lemma. My best initial guess is that we need to make stoppedOperating false somehow. Conditions that enable StoppedOperating to be true:
		- PFO's objectsprite is not on agent's interactionhelper's triggerlist && not operatingFar. I think Triggerlist might be a list of all PFOs within interaction range.
		- InteractingAgent == null
		- interactingagent.muststopoperating = true;
		- interactingagent != myAgent
		- this.ServerSaysStopOperating

## Magic General
- Cooldowns should not have any AV indicators, unless they're subtle. They're very routine so it should be unobtrusive. Recharge should only have AV if you're burned out and just recovered, not for routine recharge.

## Pyromancy
- Cooldown triggers on miscast or runout, not just on release. Need to ensure that player can hold down and it'll keep recharging.

- BurnedOut stays True, can't cast anymore

- Equipped Weapon appears in duplicate/larger form (like fist) when using
	- Attempted with agent.gun.HideGun(); No luck there

## Telemancy

- Feels pretty much bug-free.
- Balancing
  - Felt super-accurate and miscast never happened. Make sure those are still occurring after the remake.


## Stove

##### Dialogue

Removed the dialogue and relationship checks. Grilling will make noise, that's it.

##### Stove blinks when damaged but never actually blows up
```
		[Message: Bunny Mod] Stove (1153): Stove_DamagedObject
		[Message: Bunny Mod] Stove_DamagedObject: Lemma 1
		[Message: Bunny Mod] Stove (1153): Stove_AboutToExplode
		[Message: Bunny Mod] Stove_AboutToExplode: lastHitByagent = Custom
		[Error  : Unity Log] NullReferenceException: Object reference not set to an instance of an object
		Stack trace:
		ObjectReal.PlayAnim (System.String animType, Agent causerAgent) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		BunnyMod.BunnyObjects+<Stove_AboutToExplode>d__35.MoveNext () (at <fecc5c55716d4603b8afa99d5c133627>:0)
		UnityEngine.SetupCoroutine.InvokeMoveNext (System.Collections.IEnumerator enumerator, System.IntPtr returnValueAddress) (at <73b499366e5241bda47e5da76897738b>:0)
		UnityEngine.MonoBehaviour:StartCoroutine(IEnumerator)
		BunnyMod.BunnyObjects:Stove_DamagedObject(PlayfieldObject, Single, Stove)
		Stove:DamagedObject(PlayfieldObject, Single)
		ObjectReal:Damage(PlayfieldObject, Boolean)
		BulletHitbox:HitObject(GameObject, Boolean)
		BulletHitbox:OnTriggerEnter2D(Collider2D)
```
- // 202103031538 If this doesn't work, just comment both these lines out.
	- Done. Test.

##### On Exit to Base, then freeze on loading screen:
```
		[Message: Bunny Mod] Stove (1104): Stove_RevertAllVars
		[Error  : Unity Log] Error in ResetObjectReal: Stove (1104) (Stove) - Stove
```
Never was able to replicate this particular error, but Revert routinely shows an error that seems to have no effect. Added a bunch of logs to the method.

---

# Balance

## Telemancy
- Focused casting should give a base boost to accuracy. Maybe a floor to certain rolls?
- Need AV indicator & stop charging at full charge 
- Testing:
	- Initiate
	- Wild Caster
    - Focused Caster
    - Magic Training
    - WC + MT
    - FC + MT

---
	
# Release Notes 

## BEFORE RELEASE:
	Complete and win a run with each new feature, and successfully load a return to home base. This would be a full cycle of the code you can expect to run into.
	Make a promo character for each special ability or trait group, to promote the mod and community at large.
	Increment the Version number!
	Update Sprites

## Change notes

- Added Chronomancy & Pyromancy special abilities
- Reworked Telemancy completely, and renamed existing general magic traits
- Stoves now explode like Molotovs when destroyed. Trying to cook on it near the owner may have them interrupt you.
- Fixed various bugs