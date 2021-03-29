﻿# New Feature Implementations

All done for this release!

# Fixes

- Traits with Contraindications still show up in upgrade menu.

## Chronomancy
- REALLY need a review, possibly with a flowchart of the boolean flags. Need strict definitions and to be fully aware of when they're set and read. Otherwise this is a ticking timebomb for bugs.
- Per Cherry's testing, game kept speeding up more and more. He sent a test log of this, which appeared to double the timescale every time the ability was activated, without reverting it.
  - It's possible that LMB Click/Hold is setting some boolean during cooldown or miscast, which is then flipped incorrectly when the cooldown/miscast wears off.
- Convert timescale to be a single scale, rather than opposite operations for miscast.

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

- Equipped Weapon appears in duplicate/larger form (like fist) when using
	- Attempted with agent.gun.HideGun(); No luck there

## Stove

##### Dialogue

May need to just delete most of this - make grilling make noise, but don't go any deeper than that. If the owner hears it, they hear it.

Use near Owner

Neutral owner said "E_Can you not do that?" So it is retrieving the value, but still...?
```
			Message: Bunny Mod] ObjectReal_Interact: Stove (1734)
			[Message: Bunny Mod] Stove 1
			[Message: Bunny Mod] Stove_OwnerWatching: Stove (1734): Stove_UnfriendlyOwnerWatching
			[Message: Bunny Mod] Stove_FindOwner: startingChunk = 4; OwnerID = 2
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 0; OwnerID = 0
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 1; OwnerID = 0
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 2; OwnerID = 0
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 3; OwnerID = 1
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 4; OwnerID = 0
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 5; OwnerID = 1
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 6; OwnerID = 1
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 7; OwnerID = 1
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 8; OwnerID = 1
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 9; OwnerID = 0
			[Message: Bunny Mod] Stove_FindOwner: Checking Agent 10; OwnerID = 2
			[Message: Bunny Mod] Stove_FindOwner: Found Stove Owner: Scientist
			[Message: Bunny Mod] Stove_OwnerWatching 1
			[Message: Bunny Mod] Stove_OwnerWatching 2a
			[Message: Bunny Mod] Stove_OwnerWatching: Stove (1734): Owner Neutral
			[Message: Bunny Mod] Stove 6
			[Message: Bunny Mod] ObjectReal_Interact: noticingOwner Scientist; RelStatus: "Neutral"
			[Message: Bunny Mod] Stove 8a```

In multi-apartment units, sometimes the wrong owner will be called. They don't object to being in the room, so not sure what's going on. 

Use FAR AWAY from owner, 2x2 house chunk

```
		[Message: Bunny Mod] ObjectReal_Interact: Stove (1677)
		[Message: Bunny Mod] Stove 1
		[Message: Bunny Mod] Stove_OwnerWatching: Stove (1677): Stove_UnfriendlyOwnerWatching
		[Message: Bunny Mod] Stove_FindOwner: startingChunk = 12; OwnerID = 1
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 0; OwnerID = 0
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 1; OwnerID = 0
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 2; OwnerID = 0
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 3; OwnerID = 0
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 4; OwnerID = 1
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 5; OwnerID = 0
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 6; OwnerID = 1
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 7; OwnerID = 0
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 8; OwnerID = 1
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 9; OwnerID = 0
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 10; OwnerID = 0
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 11; OwnerID = 1
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 12; OwnerID = 2
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 13; OwnerID = 1
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 14; OwnerID = 0
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 15; OwnerID = 1
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 16; OwnerID = 1
		[Message: Bunny Mod] Stove_FindOwner: Checking Agent 17; OwnerID = 1
		[Message: Bunny Mod] Stove_FindOwner: Found Stove Owner: Hobo
		[Message: Bunny Mod] Stove_OwnerWatching 1
		[Message: Bunny Mod] Stove_OwnerWatching 2a
		[Message: Bunny Mod] Stove_OwnerWatching: Stove (1677): Owner Neutral
		[Message: Bunny Mod] Stove_OwnerWatching 3a
		[Message: Bunny Mod] Stove_OwnerWatching 2b
		[Message: Bunny Mod] Stove 6
		[Message: Bunny Mod] Stove 7b: Owner null

This interrupted use, so you'll need to set a threshold of distance.
Test w/ No owner

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
// 202103031538 If this doesn't work, just comment both these lines out.

##### On Exit to Base, then freeze on loading screen:
```
		[Message: Bunny Mod] Stove (1104): Stove_RevertAllVars
		[Error  : Unity Log] Error in ResetObjectReal: Stove (1104) (Stove) - Stove
```
Never was able to replicate this particular error, but Revert routinely shows an error that seems to have no effect. Added a bunch of logs to the method.


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

# Slated for Future Release

Mugging
	On attempt interact after beg:

		[Message: Bunny Mod] AgentInteractions_DetermineButtons: agent = Hobo34; GangMugging: 2
		[Message: Bunny Mod] PlayfieldObject_determineMoneyCost: transactionType = Hobo_GiveMoney1; PFO = Hobo (1104)
		[Message: Bunny Mod] PlayfieldObject_DetermineMoneyCost: num = 0; LevelMult = 1; gangsizeMult = 0
		[Error  : Unity Log] NullReferenceException: Object reference not set to an instance of an object
		Stack trace:
		AgentInteractions.AddButton (System.String buttonName, System.Int32 moneyCost, System.String extraCost) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		AgentInteractions.AddButton (System.String buttonName, System.Int32 moneyCost) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		BunnyMod.BunnyBehaviors.AgentInteractions_DetermineButtons (Agent agent, Agent interactingAgent, System.Collections.Generic.List`1[T] buttons1, System.Collections.Generic.List`1[T] buttonsExtra1, System.Collections.Generic.List`1[T] buttonPrices1, AgentInteractions __instance) (at <9db256fc31bb4b59aac3e9936a28b6a2>:0)
		AgentInteractions.DetermineButtons (Agent agent, Agent interactingAgent, System.Collections.Generic.List`1[T] buttons1, System.Collections.Generic.List`1[T] buttonsExtra1, System.Collections.Generic.List`1[T] buttonPrices1) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		Agent.DetermineButtons () (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		PlayfieldObject.Interact (Agent agent) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		Agent.Interact (Agent otherAgent) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		InteractionHelper.UpdateInteractionHelper () (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		Updater.UpdateInterface () (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		Updater.Update () (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		[Message: Bunny Mod] ChronomancyRollTimescale: 2.75
		[Message: Bunny Mod] Timescale: 0.3636364

Scary Guns
	Can equip, but they won't shoot
		Player trying to shoot:
			[Error  : Unity Log] Error in GunUpdate: Playerr (Agent)
		AI:
			[Error  : Unity Log] AI Update Error: Guard (1212) (Agent)

Slot Machine
	For $1 and $100, button text not found.
		[Message: Bunny Mod] ObjectReal_Interact: SlotMachine (1324)
		[Info   : Unity Log] Window Lose Focus
		[Info   : Unity Log] Window Gain Focus
		[Error  : Unity Log] AmbiguousMatchException: Ambiguous match found.
		Stack trace:
		System.RuntimeType.GetMethodImpl (System.String name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConv, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) (at <c79628fadf574d3a8feae0871fad28ef>:0)
		System.Type.GetMethod (System.String name, System.Reflection.BindingFlags bindingAttr) (at <c79628fadf574d3a8feae0871fad28ef>:0)
		HarmonyLib.AccessTools.DeclaredMethod (System.Type type, System.String name, System.Type[] parameters, System.Type[] generics) (at <3838bbc8277e4732833e7764fa18b67b>:0)
		Rethrow as AmbiguousMatchException: Ambiguous match for ObjectReal::PressedButton($)
		HarmonyLib.AccessTools.DeclaredMethod (System.Type type, System.String name, System.Type[] parameters, System.Type[] generics) (at <3838bbc8277e4732833e7764fa18b67b>:0)
		BunnyMod.BunnyObjects.SlotMachine_PressedButton (System.String buttonText, System.Int32 buttonPrice, SlotMachine __instance) (at <fecc5c55716d4603b8afa99d5c133627>:0)
		SlotMachine.PressedButton (System.String buttonText, System.Int32 buttonPrice) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		WorldSpaceGUI.PressedButton (System.Int32 buttonNum) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		WorldSpaceGUI.PressedButtonMouse (System.Int32 buttonNum) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		UnityEngine.Events.InvokableCall`1[T1].Invoke (T1 args0) (at <73b499366e5241bda47e5da76897738b>:0)
		UnityEngine.Events.CachedInvokableCall`1[T].Invoke (System.Object[] args) (at <73b499366e5241bda47e5da76897738b>:0)
		UnityEngine.Events.UnityEvent.Invoke () (at <73b499366e5241bda47e5da76897738b>:0)
		UnityEngine.UI.Button.Press () (at <8f550e8b53374632ad4d1095b05684fb>:0)
		UnityEngine.UI.Button.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) (at <8f550e8b53374632ad4d1095b05684fb>:0)
		UnityEngine.EventSystems.ExecuteEvents.Execute (UnityEngine.EventSystems.IPointerClickHandler handler, UnityEngine.EventSystems.BaseEventData eventData) (at <8f550e8b53374632ad4d1095b05684fb>:0)
		UnityEngine.EventSystems.ExecuteEvents.Execute[T] (UnityEngine.GameObject target, UnityEngine.EventSystems.BaseEventData eventData, UnityEngine.EventSystems.ExecuteEvents+EventFunction`1[T1] functor) (at <8f550e8b53374632ad4d1095b05684fb>:0)
		UnityEngine.EventSystems.ExecuteEvents:Execute(GameObject, BaseEventData, EventFunction`1)
		Rewired.Integration.UnityUI.RewiredStandaloneInputModule:ProcessMousePress(MouseButtonEventData)
		Rewired.Integration.UnityUI.RewiredStandaloneInputModule:ProcessMouseEvent(Int32, Int32)
		Rewired.Integration.UnityUI.RewiredStandaloneInputModule:ProcessMouseEvents()
		Rewired.Integration.UnityUI.RewiredStandaloneInputModule:Process()
		UnityEngine.EventSystems.EventSystem:Update()

		[Warning: RogueLibs] SPECIAL RELEASE 0
		[Error  : Unity Log] AmbiguousMatchException: Ambiguous match found.
		Stack trace:
		System.RuntimeType.GetMethodImpl (System.String name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConv, System.Type[] types, System.Reflection.ParameterModifier[] modifiers) (at <c79628fadf574d3a8feae0871fad28ef>:0)
		System.Type.GetMethod (System.String name, System.Reflection.BindingFlags bindingAttr) (at <c79628fadf574d3a8feae0871fad28ef>:0)
		HarmonyLib.AccessTools.DeclaredMethod (System.Type type, System.String name, System.Type[] parameters, System.Type[] generics) (at <3838bbc8277e4732833e7764fa18b67b>:0)
		Rethrow as AmbiguousMatchException: Ambiguous match for ObjectReal::PressedButton($)
		HarmonyLib.AccessTools.DeclaredMethod (System.Type type, System.String name, System.Type[] parameters, System.Type[] generics) (at <3838bbc8277e4732833e7764fa18b67b>:0)
		BunnyMod.BunnyObjects.SlotMachine_PressedButton (System.String buttonText, System.Int32 buttonPrice, SlotMachine __instance) (at <fecc5c55716d4603b8afa99d5c133627>:0)
		SlotMachine.PressedButton (System.String buttonText, System.Int32 buttonPrice) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		WorldSpaceGUI.PressedButton (System.Int32 buttonNum) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		WorldSpaceGUI.PressedButtonMouse (System.Int32 buttonNum) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
		UnityEngine.Events.InvokableCall`1[T1].Invoke (T1 args0) (at <73b499366e5241bda47e5da76897738b>:0)
		UnityEngine.Events.CachedInvokableCall`1[T].Invoke (System.Object[] args) (at <73b499366e5241bda47e5da76897738b>:0)
		UnityEngine.Events.UnityEvent.Invoke () (at <73b499366e5241bda47e5da76897738b>:0)
		UnityEngine.UI.Button.Press () (at <8f550e8b53374632ad4d1095b05684fb>:0)
		UnityEngine.UI.Button.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) (at <8f550e8b53374632ad4d1095b05684fb>:0)
		UnityEngine.EventSystems.ExecuteEvents.Execute (UnityEngine.EventSystems.IPointerClickHandler handler, UnityEngine.EventSystems.BaseEventData eventData) (at <8f550e8b53374632ad4d1095b05684fb>:0)
		UnityEngine.EventSystems.ExecuteEvents.Execute[T] (UnityEngine.GameObject target, UnityEngine.EventSystems.BaseEventData eventData, UnityEngine.EventSystems.ExecuteEvents+EventFunction`1[T1] functor) (at <8f550e8b53374632ad4d1095b05684fb>:0)
		UnityEngine.EventSystems.ExecuteEvents:Execute(GameObject, BaseEventData, EventFunction`1)
		Rewired.Integration.UnityUI.RewiredStandaloneInputModule:ProcessMousePress(MouseButtonEventData)
		Rewired.Integration.UnityUI.RewiredStandaloneInputModule:ProcessMouseEvent(Int32, Int32)
		Rewired.Integration.UnityUI.RewiredStandaloneInputModule:ProcessMouseEvents()
		Rewired.Integration.UnityUI.RewiredStandaloneInputModule:Process()
		UnityEngine.EventSystems.EventSystem:Update()

Wall Mutator
	[Message: Bunny Mod] PoolsScene_SpawnWall:
	[Error  : Unity Log] NullReferenceException: Object reference not set to an instance of an object
	Stack trace:
	BunnyMod.BunnyMutators.PoolsScene_SpawnWall (System.Boolean isFront, wallMaterialType myWallMaterial, UnityEngine.Vector3 myPos, TileData myTileData, System.Int32 streamingSize, System.Int32 streamingOffset, System.Boolean buildingStreamingChunk) (at <472e30f1a0d34a678fcff623fc89ad8a>:0)
	PoolsScene.SpawnWall (System.Boolean isFront, wallMaterialType myWallMaterial, UnityEngine.Vector3 myPos, TileData myTileData, System.Int32 streamingSize, System.Int32 streamingOffset, System.Boolean buildingStreamingChunk) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
	TileInfo.BuildWallObject (TileData myTileData, StreamingTileArray myStreamingTileArray, System.Boolean buildingStreamingChunk) (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
	LoadLevel+<loadStuff2>d__137.MoveNext () (at <eab0dd80d8294b91bfcaaa0356cbf5dd>:0)
	UnityEngine.SetupCoroutine.InvokeMoveNext (System.Collections.IEnumerator enumerator, System.IntPtr returnValueAddress) (at <73b499366e5241bda47e5da76897738b>:0)

Banana Lover ++: You can't eat anything but bananas, but when you do, you do a fun lil' dance // Cost: $1200
	Need more stupid joke traits