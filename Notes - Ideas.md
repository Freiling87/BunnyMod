﻿Formatting in here is gonna be wonky, I've only recently switched from txt to md.

# Planning

Roamers
	Random.InitState - is this behavior? Havent' looked yet
# Ideas
## Classes
### Drug Dealer
- Death to Snitches - Cops will ignore your Pusher attempts. You may attempt to sell to them, but failure will turn them hostile.
- Pusher - You can interact with most NPCs to attempt to sell them any drug item you have (or simplified, just Sugar). If you fail, they become annoyed (Upper Crusters will call the cops immediately). Cops who witness a dealing attempt will go Hostile. If you succeed at a sale, they have a chance to become Hooked. After a certain interval of withdrawal, NPCs will gain the Jonesing status. They'll seek you out in the level and beg you for a particular drug. If you go too long without selling them the requested drug, they'll go hostile, but selling them other types of drugs will keep them at bay for a while. When Jonesing, they will freely give you keys and safe combos if you ask. Jonesing NPCs may also attack other drug dealers, doctors, or scientists if they can't track you down.
### Priest
- Undead Bane - All Vampires, Zombies & Ghosts are hostile on sight.
- Sermon - Activate an Altar to randomly improve relations with NPCs within earshot. Chance of them giving you Tithes.
### Repairman
- One Happy Tamper - Tamper without angering Owner
- Suffers Tools Gladly - Reduce Item durability reduction from tampering (with an Improved version that removes it completely)
### Trapper
- Cheeky Trappy - All hidden traps (so far just landmines) are visible to you. NPCs will no longer path around traps that you place.
		____ - Increase damage from traps.
- Trapper Keeper - You can Interact with traps to add them to your inventory (Bear Traps, Land Mines). 100% chance to deactivate Door Detonators.
## Big Quests
- Get Thee Behind Me! - Kill all Zombies and Vampires on the map. Vampires are armed and travel in teams.
- Slingin' Dope - NPCs with enough money to buy one of your drugs will have an arrow above their head when the Big Quest is open. You need to sell to a certain number of NPCs.
- Z-Team - Zombie disaster occurs every level. Kill all Zombies and all living people carrying the Z-Virus. You get a free Friend Phone per level.
## Gameplay Overhauls

### Election Rebalance
		Influencing the Election
		Reputation Carryover
			Depending on your ending electability for a District, NPCs will have a starting attitude on later floors.
				-10 or lower: Hostile
				-9 to -5: Annoyed
				-4 to +4: Neutral
				+5 to +10: Friendly
				+10 to +15: Loyal
				+15 or higher: Aligned
			Affected NPCs by district:
				Slums: Gang Members
				Industrial: Workers
				Park: Gorillas, Cannibals if you have CWC
				Downtown: Bouncer
				Uptown: Upper Cruster
		Faction Reputations
			Same as Reputation Carryover, but influenced by behaviors
				Blahds
				Crepes
				Mafia
				Police
				Workers, Slum Dwellers, Office Drones, Clerks
				Upper Crusters, Doctors, Investment Bankers, Scientists
### Mugging & Extorting
		Add a 0.5s delay between failure and attack
		Can go Submissive from Mugging (no one will die for pocket change)
	removing traits:
		the same way you can gain + traits in the level up, there should be a chance for removing traits too (maybe called - traits?)
		eg: youre cannibal, level up and leave slums 1. You have a choice between im outtie, low cost jobs, and - malodorous.
	Make all traits accessible through Character Creator
	Alright my idea is that npcs can become "fearful" of the player if you just torture them in some ways
		Such as killing all of their teammates
		Also stuff like eating corpses could give them fear
		Or just continuosly destroying alredy dead npcs could give fear
		When a npc is fearfull they will hide and try to evade you at all times
		If they are from a faction they could search for another npc of that faction and treath it as a safeplace
		There could be npcs that are unable to feel fear such as robots and zombies
	Overall rework of NPC view distance.
		Probably increase the vanilla distance for balance.
		+ Giant, Loud, Lights, etc.
		- Diminutive, Darkness, etc.
	Putting the "Drunk" status effect back into the game!
		how?: well it could only happear if you have certain traits here are some ideas:
		Low tolerance to alcohol - When you drink to much beer you will get the drunk status and you accuracy and melee will go down temporarily but you can recover easily and if you just overdrink you can end up bafing removing all of you status effects wich is both good and bad (for example if you have poison this can save you)
		Bar Brawler - If you drink to much you will get drunk which gives you a + on melee
		Bonus:
			A golden trait for the robot Alcohol Energy Source makes "alcohol into energy"
			Drinking while already at full hp gives you some seconds in the energy level but it cannot make you go up by a level so it just gives you seconds.
### The Gun Fetishism Mod:
		Mechanics:
		- Bullets are smaller, faster & deal more damage √
		- Ranged increases accuracy & ROF, reduces Windup, NO effect on gun damage
		- Accuracy reduced while moving, & for followup shots due to recoil
		- Chance to inflict Slow for 1-2 seconds based on weapon damage
		- Gun sprites are blacker and scarier :scream:
		- Armor plays a larger role in not dying
		- Cover:
			- Most furniture that used to ignore bullets can now serve as cover. This includes broken windowframes.
			- This must be tracked from the shooter, not the hider. Distance from obstacle / Shooting Skill = Chance to accidentally hit obstacle instead of shoot past it.
			- Softer objects are unlikely to stop bullets, but may weaken or divert their path.
			- Mockup stats
				Destructability: 
				Obstruction: Chance to be hit by a bullet if at Cover range. 
					Slow: Slow the projectile and reduce its damage
					Divert: Divert the path of the projectile by a few degrees. https://en.wikipedia.org/wiki/Ricochet for variable list, pretty good info.
					Block:
		Mods:
		- Silencer has varying effect, depending on the loudness of weapon
		- ROF & Accuracy mods removed completely (replaced below)
		- Autofire Mod: Full auto fire. Illegal to cops and bots. AR, Pistol, Shotgun
		- Bipod: Increases stationary accuracy. AR, LMG, Rifle
		- Binary Trigger: Fires two shots in fast succession. Not illegal. AR, Pistol
		- Flash Hider: View distance/stealth is reworked, enemies can't see you as well in the dark. Muzzle flash reduced. Any
		- FMJ Ammo: Lower damage, but ignores armor. All but Shotgun
		- Foregrip: Reduces recoil for AR, Machinegun, Shotgun & SMG. Increases fire speed for Shotgun. 
		- Hacksaw: Expands weapon spread, and damage at close range. Shotgun
		- JHP Ammo: Increases damage, but much weaker against armor. All but Shotgun
		- Muzzle Brake: Reduces recoil. Any
		- +P Ammo: Increases damage & projectile speed, increases recoil. Any
		- Red Dot: Reduces accuracy penalties of movement and follow-up shots. AR, Pistol, Shotgun, SMG
		- Scope: Allows you to move reticle further away from your character (similar to using Laptop, etc.). Increases stationary accuracy. AR, Rifle, Shotgun
		Guns:
		- AR: +Damage +Accuracy, single-fire, highly moddable.
		- LMG: ++Damage +ROF +++Recoil, ---Accuracy benefits greatly from stationary fire.
		- Pea-Shooter: -Damage +ROF +Accuracy ---Recoil
		- PDW: +++ROF -Recoil -Damage -Accuracy.
		- Rifle: +++Damage ++Accuracy, Windup before firing.
		Traits:
		- Double-Tapper: Ranged weapon equivalent to Backstabber, but only works in close range.
### Rap Sheet
		in downtown and uptown, police houses(?) will have a comms desk. If you kill 5 officers, witnessing cops will head for the comms desk and say that you are wanted, giving you the wanted trait. However, you can hack comms desk to make it say that you were pardoned by the mayor. This gives you above the law for the rest of the group of 3 floors (if done on 4-3, only for one floor, if done on 4-2, for 2 floors, etc). After that, the cops find out you were lying and you are wanted, even if not previously wanted.
		in downtown and uptown, police houses(?) will have a comms desk. If you kill 5 officers, witnessing cops will head for the comms desk and say that you are wanted, giving you the wanted trait.
		Like the hack and pardon thing is good but it doesn't sound like it's 100% there
		The 5 officers threshold is a little high. You could say if you kill any officer and a cop witnesses it, or Upper Crusters might do it if they see you kill anyone
		once activated, gives wanted trait to player
		Or, if there's a computer in a cop station, that's what they use
		I guess if a Cop is Hostile and you leave their sight/they stop chasing, they might go do it
		But if you don't leave their site, they'll try to take you down
### Rogue Vision
			I'd like to make a few modifications to gameplay to make this Mutator more playable. I prefer it for Stealth runs, but the inability to peek around corners is sort of an Achilles Heel. However, modding Field of Vision is probably too ambitious for my level (not to mention RV isn't a game mode that gets a lot of attention), so this will likely remain a pipe dream.
			Visual indicators of where sounds are coming from. This would include doors opening/closing, security cameras swiveling, etc. 
			The ability to see Cameras' field of vision if you've hacked them, or an item specific to that.
			A Special ability that places or picks up a Spy Camera for observing patrols. Or, a moveable Scout Drone.
			A Mirror you can use to peek around corners.
### Stealth Overhaul
		Visual detection
			Variable name is called "hardToSeeFromDistance", trait is same but capitalized
			base = 1
			Blends in Nicely or Goon = 1.7
			Upgraded or Super-special = 2.5
		Light Rules
			Hack computer to switch off lights
			Depending on the District, lights have a chance to be flickering. More frequent in Slums, decreases as you progress.
			Destroying a generator shuts off Lights in the Chunk? (How do you handle multiple buildings per chunk, or multiple generators per building?)
		Bodies
			Seeing a body for the first time should send other property owners into full search mode (like when an alarm goes off while hacking). 
				Any way to keep them in a longer panic state? Finding a body should be disastrous for stealth, not a momentary setback
					Maybe have Alarm buttons on every level, and they run to press one if they find a body. 
						This would need to be a mutator
			If they're unconcious, they'll wake them up after searching the area
		Light Variables
			Agent.extraLight
			Agent.hasLight
			Agent.LightingTypeMedNon
			Agent.lightBrightnessTiming
			Agent.MostRecentLandPosLight
			Agent.MostRecentLandPosLight2
			Agent.rogueLight
			****Agent.SetLightBrightness
			gc.lightingType
			gc.spawnerMain.SetLighting()
			ObjectReal.noLight
			SpawnerMain 7100
			TileInfo.TurnOffLights
			TileInfo.noLightColliders
		Hiding in a box should slightly reduce movement speed (by like 0.5)
### Make Keys, Money & Safe Combos not take up a slot
	
## Items
- Beer Can (1) - Generated when a Beer is consumed, or found on its own in trash cans. Thrown weapon, does minimal damage for funsies. Like a more useless Rock.
- Door Locker (2) - Use on a Door to Lock it from both sides - activate the door to unlock it.
- Doughnut (1) - Moderate healing. Can be given to Cops to make them Friendly, or to avoid paying a Fine.
- Fire Mine (4) - Behaves like a Molotov when it explodes.
- Fancy Hat (3) - Increased chances for persuasion. Maybe a Fedora, because I want you to cringe.
- Fear Syringe (1) - (Maybe not possible since there's no fear for player characters)
- Gas Trap Kit (6) - Combine with a Syringe to make a Gas Trap that you can place like a Land Mine.
- Holy Symbol (4) - When in your inventory, all Undead NPCs slowly take damage when they're near you. They are more likely to flee during combat.
- Holy Water Flask (2) - Thrown weapon that gives a Poison condition to Zombies & Werewolves. Can also be combined with a Water Gun, Air Vent, or Water Filter.
- Lunchbox (1) - Thrown item. Deals rock-level damage, drops a food item on impact.
- Mini Turret (3) - It's a Turret.
- Oil Bottle (1) - Thrown weapon that creates a splash of oil upon impact.
- Rifle (5) - High damage, slow rate of fire with warmup. Hopefully can extend range to beyond normal screen, like when using the Laptop.
- Riot Armor (3) - Resists bullets and melee damage.
- Riot Helmet (2) - Just another aesthetic option.
- Screwdriver (1) - A stabbing weapon slightly weaker than the knife. Would be used for extended tampering options.
- Spear (1) - Thrusting weapon, longest melee reach. Deals damage equal to Knife.
- Sugar Processor (6) - Similar to Bomb Processor. 
- Teleportation Trap (2) - Floor trap.
- Toxic Slime Capsule (1) - Thrown item. Inflicts poison and leaves a Goop Splash.
- Water Gun - Extended to combine with Oil Can, so you can shoot Oil.
- Whiskey Bottle (1) - Generated when Whiskey is consumed, or found on its own in trash cans. Thrown weapon, does minimal damage for funsies. Like a more useless Rock.
- Wire Cutters - Deactivate cameras instantly with no chance of failure. Destroy Barbed Wire fences. Other stuff?
- Z-Virus (2) - Solely for putting in Vents or Water Pumps. Doesn't do damage, but infects people with Zombiism.
- Magic items:
	- Mana Crystals (decay slowly but unpredictably, increase recharge speed)
	- Miscast Crystals (Rare, but absorb a miscast and shatter)
	- Ultrachungus Crystals (Temporarily boost your ability to obscene levels)
- BackPack - +inventory slots
- Body Spray: Anti-cologne, basically gives ideological clash to whoever uses it. (This would be cool in assassin quests if you could apply it to your target.)
## NPCs
	Artist
	Farmer
	Detective
	Junkie
	Reporter
	Security Guard
	Tourist
		Travels in groups, occasional goon
		Behaves like Upper Cruster
## NPC Goals
### Accosting
- Cannibal - If you have CWC, they will ask to take a bite of you. You can agree or not. That's it.
- Cop - Might solicit a little bribe. If you have Cop Debt, it goes towards that total.
- Gang Member - Will attempt to mug you like Mobsters, but for a smaller amount ($15 * gang size).
- Office Drone - Will blab about sports. You can choose to blab back and get a new relation, from Annoyed to Loyal.
- Slum Dweller
	- Will occasionally beg you for money. Percent chance of outcome based on donation:

| Donation | Hostile | Annoyed | Neutral | Friendly | Loyal | Aligned | Item Equivalent |
|----------|---------|---------|---------|----------|-------|---------|-----------------|
|$0|10|55|35|0|0|0|--|
|$5|0|5|25|65|5|0|Fud|
|$10|0|0|5|65|25|5|Cigarettes or Beer|
|$20|0|0|0|45|45|10|Whiskey|
|$50|0|0|0|0|0|100|Sugar|
|"Fuck off"|100|0|0|0|0|0|Banana Peel|

	- When done, randomly generate dialogue:
		- "i can feel the shadows consuming me, quck i need your cash!"
		- "Gimme money so i can have my last fix before rapture 2.0 destroys us all!"
		- "c'mon you gotta protect me, the Illuminati spy-cams are coming!"
		- "they injected me with liquid Antichrist"
		- "fud 0 is real! Help fund my research to figure out where big fud is hiding it!"
		- "i control the elements! Gimme your money or perish!"
		- Alignment line: (only on a level that is a multiple of 2: EG: 1-2, 2-2, 3-2...) "Can't you feel it? a disaster is coming!"
		- Alignment line: "the gates of [silly call of Cthulhu sounding name] are going to open any second now, i can feel it!"
		- Alignment line "i can see them... the bars above peoples heads, theyre up to something, i know it!"
		- Alignment line:"beds are too soft to be real, its a ploy to get us in our most vulnerable state!"
		- Alignment line: "I'm getting closer to finding the big brother that watches over us all, i just need to figure out what 'someone' is code for..."
		- imma refer to lines where they aggro on you cus you gave them food or didnt pay them as aggro lines
		- Aggro line: "Illuminati scum!"
		- Aggro line: "i knew it! You captured michal jackson, didnt you? WHERE ARE YOU HIDING HIM?!?"
- Scientist - Asks you to take a random pill or syringe. Gives you $20.
- Wrestler - Challenges you to a duel, just like the player character. If you refuse, he gets annoyed.
### Random Actions
- Jock - Might decide to punch you in the face. You know, as a prank.
### Cops
- If there's a Computer in a Police Station, and an Annoyed or Hostile Cop stops chasing you, they'll go there and enter your information into the database. If they're successful, all cops on the level will match their attitude towards you if theirs isn't currently worse.
	- You can Hack the computer to undo this change.

## NPC Interactions
- Bouncers - You can come in if for free if they see you smoke cigarettes. Because obviously it means you're cool.
- Cops  
	- Will attempt to fine you for petty crimes like destroying trash cans, instead of going hostile.
	- Bribe is a % chance to succeed, annoy, or turn them hostile.
- Cop Bot - Tamper to deactivate them (Requires One Happy Tamper or Tech Expert). Maybe this will be a % chance for them to trust you, like robbing/extorting.
- Scientists - Hire to put poison in a Vent
- Thieves - Hire to unlock Safes
- Workers - Hire to Tamper

## Mutators
- Zoom Levels
	- https://discord.com/channels/187414758536773632/433748059172896769/815314079408980000
- All Walls Are Wood
- All Walls Are Steel
- Major disaster behavior changes
	- Cops don't enforce property damage laws
	- Some NPCs will join your party for safety
- Riot
	- Add Arsonists to spawns
	- Rioters shouldn't ignore cops
     	- You can change this in LevelFeelings.Riot2, cops aren't mentioned there
- Mafia spawn on all levels
	- Would be extremely easy, just set LevelData.levelFeatures.Contains("Mafia") to true
- District-related spawns
	- LevelEditor.CreateLevelFeatureList
	
## Objects
- Cash Register - A tough locked object containing money, that may be unlocked through the chunk's Computer, Hacked, Lockpicked, or pried open with a Crowbar. Easier than a Safe, yes, but almost always within the view of their owner.
- Chemistry Set - Sacrifice a syringe to identify that type.
- Dumpster - A big metal dumpster that you can hide in, for settings where Bushes or Bathtubs aren't appropriate.
- Oil Barrel - A Rust-colored barrel which creates an oil pool when destroyed.
- Kitchen Fryer - When destroyed, leaves a pool of oil.
- Pay Phone - Call the Resistance to send a Specialist over. Choose between Hacker, Thief, and Goon. 
- Refrigerator - Tamper to make it Run after a 10s countdown
- Safe - Open with Detonator.
- Security Door - Steel door, not openable with lockpick or crowbar. Can only be opened via a connected computer.
- Television - Tamper to make it increase in volume immediately, and explode after a 10s countdown
- Trampoline - Does annoying jumping behavior. I hate it already.
- Other Objects - Simply for visual variety when making custom chunks. Statues, park benches, filing cabinets, glass tables, office chairs, paintings, gym equipment, beer taps, curtains, etc. 

## Object interactions
- Tampering
	- Air Conditioning Unit - Tamper to Release Gas without access to main computer (not sure about this one). 
	- Toilet - Tamper to...? Maybe spray water like hydrant, useful for keeping hallways clear. Or maybe some other behavior, not sure yet.
	- Bed - Fucking explode if someone tries to sleep in it
- Alarms on Chests/Safes, Doors & Windows
	- Common in high-security chunks.
	- If one is present, interacting will give you a chance to attempt to disarm the alarm. This might use a tool (wire cutters?) as a bonus to success. 
	- You can hack them, of course, or disable them from a computer.
	- Or, you can just open or destroy the object and set the alarm off. 
	- This would add a small, simple layer to intrusion that currently is sort of a glaring omission in terms of portraying real-world stuff.
		
## Special Abilities
- Beggar | 2 Points | Beg random people for something. Either get a food item or cigarettes. 50/50 Chance. Success will make Agent friendly, failure will annoy them.
		You can reattempt but they might go hostile.
### Magic
- Chronomancy
	- Uniques
		- Increase your attack speed
		- Normal attack speed when time is slowed down - fast punch like Goku
- Electromancy
	- Split only triggers when it hits a wall, not an agent.
- Morphomancy: Assume the appearance of someone else, and gain all of their relations to other NPCs
	- Having someone else's appearance costs 5 mana per second
	- Miscasting turns you into a gorilla temporarily
- Pyromancy
	- Custom Firebomb explosion without glass sound
		- This was a can of worms when I tried it with HammerTime. All we need is to stop the glass sound, so maybe that'l be simpler.
	- Fan of Fire - 3 flames per shot // Fan of Fire + - 5 per shot
	- These should both be set to an angle proportionate to their speed. If the speed is high, keep them narrow. If it's low, keep them wide and more like a fan. They also decrease 
	- Ring of Fire - Shorter range, but 360 degrees
- Fireball
	- Would need a new Projectile type
- Hematomancy 
	- Blood Magic
- Kinetomancy  
	- Telekinesis
- Megaleiomancy 
	- Charm Person
- Necromancy
	- Normal Use
		- 1 Summon hostile Zombies from corpses / Turn ghosts into small number of crystals
		- 2 Zombies are Neutral to you / Turn ghosts into medium number of crystals
		- 3 Zombies will join your party / Turn ghosts into large number of crystals
		- When close to a ghost, you can turn them into mana crystals
	- Miscast 
		- turns all of them hostile, or summons hostile ghosts
	- Also:
		- https://www.reddit.com/r/streetsofrogue/comments/lhdwnx/my_third_entree_for_medieval_themed_characters/
- Telemancy
	- EMP on teleport?
	- Stun on teleport?
## Traits
- Fatass
	- Slower movement
	- Can't wear armor
	- Stomp damage
### Path traits
Not sure whether it's better to categorize these as acts or Paths. Paths would be a set of bonus/malus XP events for particular acts in a category.
- Chaotic Acts
	- Break Tombstone
	- Bribe Police
	- Influence Election
	- Mugging
	- Extortion
	- Install Malware
	- Kill non-Guilty NPC (Have guilty-vision)
	- Burn Walls
	- Set Fire
- Orderly Acts
	- Arrest Guilty NPC
	- Kill Guilty NPC
	- Knockout Guilty NPC
	- Extinguish Fire
	- Goon Quest
- Kind Acts
	- Offering Motivation
	- Spreading Happy Waves
	- Giving money to begging Slum Dwellers (planned feature)
- Just Acts
- Unjust Acts
	- Kill fleeing enemy
	- Kill Sleeping NPCs
	- Mugging
	- Possession
	- Extortion
- Fire Acts
	- Kill Fireman
	- Kill anyone with fire
	- Burn object
	- Burn corpse
	- Burn wall
- Water Acts
	- Put out fire
- Acts of Commerce
	- Spend Money
	- Get Money
- Uncategorized Acts
	- Use of Poison Syringe, Cyanide, Rage Poison, Haterator
	- Cannibalism, Blood Drinking
	- Use of Confusion Syringe, Sleep Darts, Chloroform

- Path of Fire
	- You can only regain health by burning corpses. 
	- Take damage in Water.
	- Firefighters are always hostile and will use their water cannons against you.
	- Perpetual Particle Effect on character - flames or smoke
	- Big Quest - Burn a certain number of corpses
- Path of Shadow
	- +No Alarms
	- -Alarms
	- +Backstabbing
- Path of Justice - Basically just Cop XP portion split off for smaller trait
	- +Arrest Guilty
	- +Kill Guilty (less)
	- -Kill innocent
	- +Freeing Slaves or Gorillas
### Others
- hit-and-run (4): gain speed for 5s after a kill.
- adrenaline rush (4): gain strength for 5s after a kill.
- Alcoholic (-4) - Might be interesting to have an Addict trait but with alcohol.
- Animal Whisperer - Gorillas and Werewolves are Loyal
- Bonkist (2) - Backstabbing with a blunt weapon leaves the target Dizzy, if they survive. (Considering making this a small chance with all blunt hits, not just backstabs)
- Extortion - Property owners will be more reluctant to give into extortion based on how much property they own. Destruction of that property will make them more likely to give in, just like hitting them would.
- Eye Poker - Chance to blind enemies when you hit them with an Unarmed attack.
- Far-Sighted - Can only use Ranged weapons.
- Fast Metabolism (-2) - Less healing from food and alcohol.
- Good Arm - Increased Throw range.
- Generally Unpleasant (7) - All NPCs start out Annoyed.
- Hungry Boy (3) - More healing from food and alcohol.
- Machine Shaker - Chance of a free transaction when using a Vending Machine.
- Mugger - Mugging acts more like Extortion. Dealing damage can make the target Submissive. If it's not already, it is treated as a violent crime by police.
- Mugger+ - When mugging someone, you get their inventory items too. 
- Rap Sheet (-3) - Cops and Supercops start out annoyed with you.
- Spectral Fist - You can hit Ghosts with your unarmed attacks. Extra damage to all Undead.
- Student of the Blade - Increased damage with Sharp weapons.
- Veiled Threats (-2) - When you attempt to Bribe, Extort, Mug, or Threaten, a failure will turn the target Annoyed instead of Hostile. 
- Whiffist (3) - Small chance for Melee or Thrown attacks to miss you completely.
- Seperation Anxiety; cost: -3
	- If you have no followers, you suffer lowered accuracy, melee, and speed. (decrease of 1 in each stat)
- Arrogant; cost: -4
	- People get mad at you a lot more easily (when people would normally become annoyed, they instead bacome hostile
- SACRIFICE
	- You can only regain health by burning corpses
- Annoying;
	- Everyone's annoyed to begin with
- Arthritis - Your weapon swing is slower, and fire rate is slower too. -1 point. Cancels: Stubby fingers, sausage fingers, near harmless, pacifist.
- Chubby - You lose health when eating unhealthy foods, and when you take any drugs. -2 points.
- Annoying - Everyone except prisoners and      objectives are in an annoyed state. -3 Points. Cancels: The Law, Random Reverance, Friend of the common folk, friend of the family.
- Infamous - Eveyone except Cops, Goons, Scientists and Non-Humans are initially hostile to you. -5 points. Cancels: Scientist Slayer, Specist, Blahd Crusher, Crepe slayer, Friend of the common folk, Random Reverence, Friend of the family.
- Unsavable - Cannot heal, even through level ups. -10 points. Cancels: Medical professional, strict cannibal, jugalarious, addict, [insert anything that heals you here.]
- chemical resistance (2): status effects cannot affect you
- aquaphobia (-1): take damage in water
- food intolerance (-3): cannot eat food
- New Trait - Masochist (-5)
	- Like Addict, But You Don't Need Drugs.You Need Damage.Make Damage Yourself Every 60 Seconds.If 60 Seconds Pass Without Damaging Yourself, You Will Lose 3 Health Per 2 Seconds.
- silent fingers (4): doing most actions produce significantly less noise, or no noise whatsoever, with the exception of alarms. (eg: arresting, breaking windows , enslaving, and (shooting guns?) make significantly less noise).
- Crit-prone
	- Enemies have a better chance of crit against you
			Or just add that to Unlucky
- Needy (-6 Points)
	- You have needs like the musician, but they are not beneficial to you or anybody else. (You need to go to the toilet, etc)
	- You need to use the nice toilet. Which always happens to be in some inconvenient place like the closet of a gang hideout. That timer is rapidly ticking down, and if you don't go to that specific toilet, you'll have an accident, and no NPC or shop owner, etc will want to talk to you.

## Scraps I need to put in the right place in this file

**Hacking Overhaul**

- Greatly increase the possibilities, risks, rewards, and trait investment for hacking
- Ground Rules
	- Attempt: Every action while hacking has a % chance of success displayed next to its button. 
	- System: The collection of Objects under one Owner ID in a particular chunk that has a Computer. If there's no Computer, the object is a System unto itself.
	- Heat: This is a baseline increased difficulty to all Attempts in a System. It is increased whenever you pass an Attempt, more when you fail one. Remote access leads to higher heat than using a machine in person. Some Objects when not part of a System will have their own Heat, like ATMs. Refrigerators, not so much.
	- Password: Occasionally found in a Computer's owner's pocket. Might also be found hidden somewhere in a Computer. Submissive NPCs will give you their passwords. Pretty much a Safe Combo except for Computers.
	- PayData: Questgivers will occasionally request this, which you need to retrieve from an intact Computer. Hackers will buy PayData, sometimes a better offer than completing your Quest. PayData has a small chance of appearing without a Quest attached.
- A failed Attempt in a System may have different results, depending on how much your Attempt failed by, compared to Heat:
	1. Can reattempt, Heat+.
	2. Action Locked, Heat++.
	3. Action Locked, Heat++, any Hacker owners search.
	4. Action Locked, Heat+++, triggers Alarm, owners search.
	5. System Locked, Heat++++, triggers Alarm, owners search, and a squad of Cop Bots is deployed to search for you.
	6. Computer is destroyed, triggers Alarm, owners search, and a squad of Cop Bots is deployed to search for you.
- Traits:
	- Tech Expert is replaced by several traits. What's not in here is covered by Explosives Expert and Mechanical Expert (separate overhauls)
	- Cyber-Intruder - Improve your Attempt rolls.
	- Data Broker - Increase sell price of PayData, and its chance of appearing in a System.
	- IOT God - Enables non-Computer Object hack actions. Many of the vanilla hack actions (Like "Refrigerator Run") would be hidden behind this trait.
	- IP Ghost - Reduces initial Heat & slows its increase.
- Generic Computer Actions:
	- Access PayData: Relatively difficult but valuable, so you'll probably need to make sure you're prepared.
	- Access System: The initial entry to a System is technically an Attempt. This also applies if you're trying to acces a system that has been Locked.
	- Lights: Turns on or off all Lights in the Chunk. (See Stealth Overhaul)
	- Cut Power: Shuts down the power for the Chunk. Can be brought back up with a Generator or Power Box.
	- Deactivate Alarm: Just what it sounds like.
	- Enter Password: Deactivates System's Heat permanently, but you can still roll an Attempt failure!
	- Guess Password: A very small chance. Feeling lucky?
	- Increase Permissions: Improves success rate of all further Attempts in the system, but slightly increases your Heat.
	- Invert Credentials: Immediately flip the ownership of all cameras and turrets at once.
	- Maintenance Mode: Disable Heat on a target Object in the System.
	- Recover Password: Very high-risk.
	- Route IP: Select another computer in the vicinity to route your access through. If Cop Bots are deployed, they'll go there instead.
	- Trigger Alarm: Just what it sounds like.
	- Unlock Action: Unlocks most actions previously locked (Except Wipe Audits). Increases Heat substantially.
	- Wipe Audit Trail: Reset's Systems Heat to 0. Can only be done once per System.
- Chunk-specific Computer Actions:
	- Apartments - Sprinkler Test: All Stoves in the chunk leak oil for a few seconds, then burst into flames.
	- Arcade - Cash Out: Unlock cash compartments in all Arcade Games, Jukeboxes, Pool Tables in the chunk. Each has $3-$5 inside. All become busted, though. Show Off: Any Hackers in the chunk become Loyal to you.
	- Arena - Audience Participation Night: Release Rage Poison in vents.
	- Armory - Red Alert: Cameras and turrets target everyone, gas is released, alarms go off continuously, huge explosion after countdown.
	- Bank - High chance of PayData. Heist Alert: Cause Supercops to swarm the level, searching the Bank first.
	- Bar - Drink Specials: Buying a round is cut to half-price.
	- Bathhouse - Deep Cleanse: Poison the water.
	- Bathroom - Brown Alert: All toilets self-destruct with a Poison explosion.
	- Broadcasting Station - ___?
	- Cabin - Play Music (https://www.youtube.com/watch?v=puVYtkh-LO4)
	- Casino - Card Counter: All owners become Annoyed at the targeted NPC. Card Cheat: All owners become Hostile to the targeted NPC.
	- Cave - ___?
	- Church - ___?
	- City Park - Block Party: All NPCs who were wandering the level are now wandering this chunk. 
	- Confiscation Center - ___?
	- Dance Club - Now Punch to the Left: Mind control everyone dancing like the Alien's improved SA.
	- Deportation Center - ___?
	- Drug Den - He's Wearing a Wire: Owners become hostile to all non-Owners in the chunk. Codeword Flamingo: A small squad of Cops invades the chunk.
	- Farm - Overclock Produce: All Trees, Bushes, Plants & Giant Plants in the chunk burst into flames after a countdown.
	- Fire Station - Union Strike: Firefighters no longer put out fires in this level. Training Exercise: Release an Arsonist into the level.
	- Gated Community - Eek! Poor People!: One or two Supercops start wandering this chunk, wandering on patrol. 
	- Graveyard - Where's the Good Stuff Buried: Increase your chance of finding Money when destroying a Gravestone.
	- Greenhouse - Sup-R-Gro Treatment: Release Gigantizer gas in vents.
	- Hedge Maze - Code Theseus: Werewolf Squad starts patrolling this chunk, hostile to anyone.
	- Hideout - ___?
	- Hospital - Emergency Pandemic Response: Release Cyanide gas in vents.
	- Hotel - Premium Continental Breakfast Extravaganza: Get a banana.
	- House - Silent Alarm: Summons a couple of cops to investigate the Chunk. They may or may not kill the homeowner depending on, you know... "factors."
	- Ice Rink - Overclock A/C: Releases Freezy Gas from the Vents. 
	- Lab - Delta Experiment: Releases a random Gas from the Vents. 
	- Mall - Raise the Rent: All Vendors have a slight discount.
	- Mansion - Amazon Order: If the Owner is alive, a Slave is generated for them. The People's Mansion: A swarm of Slum Dwellers invades the chunk.
	- Mayor's House - ___?
	- Mayor's Office - ___?
	- Military Outpost - Strangelove Protocol: Owners are Hostile to everyone else.
	- Movie Theater - ___?
	- Music Hall - Mandatory Moshpit: Berserk everyone in the chunk, as long as Speakers, turntable, and Musician are still intact. Go to 11: Blows wind like the Air Gun from Speakers, deafens like the flute.
	- Office Building - Extremely Casual Friday: Owners all become naked and Friendly.
	- Pit - ___?
	- Podium Park - Intro Music: Everyone assembles here before the podium is used. Might be a good distraction.
	- Police Outpost/Police Station - All Clear: Set all Hostile Police to Annoyed. APB: All Police hostile to a target person. Lockdown Protocol: Deactivate or Activate Lockdown walls.
	- Prison - Prisoner Discipline Initiative: Owners break into cells one by one and kill the inhabitants.
	- Private Club - VIP Card: Gain Bouncer access. Owners are Loyal, others are Friendly.
	- Shack - Blahd Bash / Crepe Crush: All gang members of a chosen type converge on this shack. If they don't see hostiles, they leave and resume their previous activities.
	- Shop - Raise the Rent: All Vendors have a slight discount.
	- Slave Shop - Free Slaves; Detonate Slaves
	- Uptown House - Silent Alarm: Summons a couple of cops to investigate the Chunk. They may or may not kill the homeowner depending on, you know... "factors."
	- Zoo - Animal Liberation Front: A squad of Gorillas invades the chunk and kills the owners.
- Object hacking Actions (Why IOT is a bad idea)
	- Air Conditioner - Release Gas can now be done directly from A/C.
	- Alarm Button - ___?
	- Ammo Dispenser - On the House: One free Refill. Red Alert: Shoot bullets everywhere and explode
	- Arcade Game - Cash Out: Release $3-5
	- ATM - No longer Tossable nor destructible with melee, fire or bullets. Good chance of PayData. High heat, and alarm sensitive to hacking & tampering. Cash Transfer: Release ~$100. 
	- Augmentation Booth - Dispense a free can of XP Juice.
	- CloneMachine - Spit out a shapeshifter
	- Crusher - Increase crush speed; Deactivate
	- Door - If it has an Alarm or trap (See Alarm overhaul), reset/set off/deactivate it.
	- Elevator - ___ ?
	- FireSpewer - Deactivate; Leak Oil; Start; Overheat (nonstop fire and explosion)
	- FlameGrate - Deactivate; Leak Oil; Start; Constant flame mode
	- Generator - Activate/Deactivate (Similar to PowerBox, local to Chunk)
	- Generator2 - Activate/Deactivate/Overload (Similar to PowerBox, local to Chunk)
	- Goodie Dispenser - Maybe just replace this with a Shop machine - works like Shopkeeper. Goodie Dispenser sucks.
	- Jukebox - Cash Out: Release $3-5
	- Lamp - Separate Ground Wire: Electrocutes anyone who touches it. 
	- LaserEmitter - Activate/Deactivate; Change Mode
	- LoadoutMachine - ___?
	- LockdownWall - Activate/Deactivate/Disable
	- MetalDetector - ___?
	- Mine - Deactivate for pickup; Detonate; Incendiary Mode
	- PawnShopMachine - ___?
	- PoliceBox - ___?
	- PoolTable - Cash Out: Release $3-5
	- PowerBox - Reactivate if shut down
	- Refrigerator - Dispense Ice: Shoots Freeze Rays in random directions, then explodes.
	- Safe - ___?
	- SatelliteDish - ___?
	- SecurityCam - ___?
	- SlotMachine - Cash Out: Release ~$100. (Needs balance: ideas?)
	- Speaker - Go to 11: Blows wind like the Air Gun, deafens like the flute.
	- Stove - Overload Gas Line: Leak oil for 5s, then burst into flames.
	- Television - Ludovico Protocol: Mind Control a person within visual range of the TV.
	- Turntables - Mandatory Moshpit: Berserk everyone in the chunk, as long as Speakers, turntable, and Musician are still intact.
	- Turret - ___?
	- WaterPump - ___?
	- Window: If it has an Alarm or trap (See Alarm overhaul), reset/set off/deactivate it.

## Shelved

- Fridge not showing button text for showchest

- Object interactions show a "Cancelled" message even though they went through successfully.

- Auto-trayed items may include those you can't use/equip

Show Chronomancy Timescale in statuses
		A:	// interactingAgent.statusEffects.myStatusEffectDisplay.RefreshStatusEffectText();
			// Used as a void, just to refresh all active in list. Should be pretty simple.
			// Just make a postfix here that tacks the timescale message at the top of the list if possible.
			// However, you may need to make a new object of type StatusEffectDisplayPiece
		B:	// Might not even need a patch. First, try
			StatusEffectDisplay.AddDisplayPiece and .RemoveDisplayPiece
		C: BuffDisplay.AddStatusEffect

BULLET MODDING (Re Electromancy & others)
	Unused variables:
		string substance
		int damageMod