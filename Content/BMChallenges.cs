using RogueLibsCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using BunnyMod;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace BunnyMod.Content
{
	public class BMChallenges
	{
		public static void InitializeChallenges()
		{
			// On Ice until RL3 has Challenge support :(

			//#region Buildings

			//CustomMutator CityOfSteel = RogueLibs.CreateCustomMutator(cChallenge.CityOfSteel, true,
			//	new CustomNameInfo("Buildings: City Of Steel"),
			//	new CustomNameInfo(
			//		"A gleaming city of steel! The world of the future, today. Mankind's dream in... Wow, it *really* smells like steel cleaner. Like, it fucking stinks. This is pungent."));
			//CityOfSteel.Available = false;
			//CityOfSteel.Conflicting.AddRange(cChallenge.Walls);
			//CityOfSteel.IsActive = false;

			//CustomMutator GreenLiving = RogueLibs.CreateCustomMutator(cChallenge.GreenLiving, true,
			//	new CustomNameInfo("Buildings: Green Living"),
			//	new CustomNameInfo(
			//		"The Mayor has retrofitted most buildings to eco-friendly plant-based construction. Caterpillars crawl on your face while you sleep, but the air is mighty fresh!"));
			//GreenLiving.Available = false;
			//GreenLiving.Conflicting.AddRange(cChallenge.Walls);
			//GreenLiving.IsActive = false;

			//CustomMutator Panoptikopolis = RogueLibs.CreateCustomMutator(cChallenge.Panoptikopolis, true,
			//	new CustomNameInfo("Buildings: Panoptikopolis"),
			//	new CustomNameInfo(
			//		"Locals just call it The Pan for short. Authoritarian surveillance measures mandate that most buildings have to be built with glass walls. If you have nothing to hide, what are you worried about, citizen?"));
			//Panoptikopolis.Available = false;
			//Panoptikopolis.Conflicting.AddRange(cChallenge.Walls);
			//Panoptikopolis.IsActive = false;

			//CustomMutator ShantyTown = RogueLibs.CreateCustomMutator(cChallenge.ShantyTown, true,
			//	new CustomNameInfo("Buildings: Shanty Town"),
			//	new CustomNameInfo("Looks like we're short on Steel.\n\nHard mode for Firefighters, easy mode for arsonists. Fun mode for psychopaths."));
			//ShantyTown.Available = false;
			//ShantyTown.Conflicting.AddRange(cChallenge.Walls);
			//ShantyTown.IsActive = false;

			//CustomMutator SpelunkyDory = RogueLibs.CreateCustomMutator(cChallenge.SpelunkyDory, true,
			//	new CustomNameInfo("Buildings: Spelunky Dory"),
			//	new CustomNameInfo("You and your fellow citizens live in a disgusting cave complex, but you keep a stiff upper lip about it!"));
			//SpelunkyDory.Available = false;
			//SpelunkyDory.Conflicting.AddRange(cChallenge.Walls);
			//SpelunkyDory.IsActive = false;

			//CustomMutator Buildings_Hide = RogueLibs.CreateCustomMutator(cChallenge.Buildings_Hide, true,
			//	new CustomNameInfo("Buildings [-]"),
			//	new CustomNameInfo("Click to hide group"));
			//CustomMutator Buildings_Show = RogueLibs.CreateCustomMutator(cChallenge.Buildings_Show, true,
			//	new CustomNameInfo("Buildings [+]"),
			//	new CustomNameInfo("Click to show group"));

			//CustomMutator[] buildingMutators = new CustomMutator[] { CityOfSteel, GreenLiving, Panoptikopolis, ShantyTown, SpelunkyDory };
			//Buildings_Hide.Available = false;
			//Buildings_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Buildings_Hide.Available = false;
			//	Buildings_Show.Available = true;

			//	foreach (CustomMutator m in buildingMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//Buildings_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Buildings_Hide.Available = true;
			//	Buildings_Show.Available = false;

			//	foreach (CustomMutator m in buildingMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion

			//#region Overhauls

			//CustomMutator ArcologyEcology = RogueLibs.CreateCustomMutator(cChallenge.ArcologyEcology, true,
			//	new CustomNameInfo("Exteriors: Arcology Ecology"),
			//	new CustomNameInfo(
			//		"Sustainable Eco-homes! Trees! Less pollution! What's not to love?\n\n(Answer: It's still miserable.)\n\n- Public floors are grass\n- Adds nature features to public areas"));
			//ArcologyEcology.Available = false;
			//ArcologyEcology.Conflicting.AddRange(cChallenge.AffectsFloors);
			//ArcologyEcology.IsActive = false;

			//CustomMutator Exteriors_Hide = RogueLibs.CreateCustomMutator(cChallenge.Exteriors_Hide, true,
			//	new CustomNameInfo("Exteriors [-]"),
			//	new CustomNameInfo("Click to hide group\n\nYes, there's only one mutator in this category right now. Modding is hard :("));
			//CustomMutator Exteriors_Show = RogueLibs.CreateCustomMutator(cChallenge.Exteriors_Show, true,
			//	new CustomNameInfo("Exteriors [+]"),
			//	new CustomNameInfo("Click to show group\n\nYes, there's only one mutator in this category right now. Modding is hard :("));

			//CustomMutator[] ExteriorsMutators = new CustomMutator[] { ArcologyEcology };
			//Exteriors_Hide.Available = false;
			//Exteriors_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Exteriors_Hide.Available = false;
			//	Exteriors_Show.Available = true;

			//	foreach (CustomMutator m in ExteriorsMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//Exteriors_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Exteriors_Hide.Available = true;
			//	Exteriors_Show.Available = false;

			//	foreach (CustomMutator m in ExteriorsMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion

			//#region Fire

			//CustomMutator GasolineHumidity = RogueLibs.CreateCustomMutator(cChallenge.GasolineHumidity, true,
			//	new CustomNameInfo("Fire: Gasoline Humidity"),
			//	new CustomNameInfo("The weather for today is... all fucked up.\n\nFires spread faster."));
			//GasolineHumidity.Available = false;
			//GasolineHumidity.Conflicting.AddRange(new string[] { cChallenge.Mildfire });
			//GasolineHumidity.IsActive = false;

			//CustomMutator Mildfire = RogueLibs.CreateCustomMutator(cChallenge.Mildfire, true,
			//	new CustomNameInfo("Fire: Mildfire"),
			//	new CustomNameInfo("Fire spreads slower and doesn't last as long."));
			//Mildfire.Available = false;
			//Mildfire.Conflicting.AddRange(new string[] { cChallenge.GasolineHumidity, cChallenge.NapalmSprings });
			//Mildfire.IsActive = false;

			//CustomMutator NapalmSprings = RogueLibs.CreateCustomMutator(cChallenge.NapalmSprings, true,
			//	new CustomNameInfo("Fire: Napalm Springs"),
			//	new CustomNameInfo("Fire lasts longer and spreads further."));
			//NapalmSprings.Available = false;
			//NapalmSprings.Conflicting.AddRange(new string[] { cChallenge.Mildfire });
			//NapalmSprings.IsActive = false;

			//CustomMutator Fire_Hide = RogueLibs.CreateCustomMutator(cChallenge.Fire_Hide, true,
			//	new CustomNameInfo("Fire [-]"),
			//	new CustomNameInfo("Click to hide group"));
			//CustomMutator Fire_Show = RogueLibs.CreateCustomMutator(cChallenge.Fire_Show, true,
			//	new CustomNameInfo("Fire [+]"),
			//	new CustomNameInfo("Click to show group"));

			//CustomMutator[] fireMutators = new CustomMutator[] { GasolineHumidity, NapalmSprings, Mildfire };
			//Fire_Hide.Available = false;
			//Fire_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Fire_Hide.Available = false;
			//	Fire_Show.Available = true;

			//	foreach (CustomMutator m in fireMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//Fire_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Fire_Hide.Available = true;
			//	Fire_Show.Available = false;

			//	foreach (CustomMutator m in fireMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion

			//#region Features

			//CustomMutator BadNeighborhoods = RogueLibs.CreateCustomMutator(cChallenge.BadNeighborhoods, true,
			//	new CustomNameInfo("Features: Bad Neighborhoods"),
			//	new CustomNameInfo("This place sure went to shit, didn't it?\n\n- Small chance for any given window to start out broken"));
			//BadNeighborhoods.Available = false;
			//BadNeighborhoods.Conflicting.AddRange(new string[] { cChallenge.PoliceState, cChallenge.MACITS });
			//BadNeighborhoods.IsActive = false;

			//CustomMutator BroughtBackFountain = RogueLibs.CreateCustomMutator(cChallenge.BroughtBackFountain, true,
			//	new CustomNameInfo("Features: Broughtback Fountain"),
			//	new CustomNameInfo(
			//		"\"He could smell Jack - the intensely familiar odor of cigarettes, musky sweat, and a faint sweetness like grass, and with it the rushing cold of the fountain.\"\n\n- Adds Fountains\n- Purely aesthetic for now"));
			//BroughtBackFountain.Available = false;
			//BroughtBackFountain.Conflicting.AddRange(new string[] { });
			//BroughtBackFountain.IsActive = false;

			//CustomMutator CartOfTheDeal = RogueLibs.CreateCustomMutator(cChallenge.CartOfTheDeal, true,
			//	new CustomNameInfo("Features: Cart of the Deal"),
			//	new CustomNameInfo(
			//		"A lot of people, very important people, are saying the City has the best Vendor Carts. The best folks, just tremendous. Don't we love our Vendor Carts?"));
			//CartOfTheDeal.Available = false;
			//CartOfTheDeal.Conflicting.AddRange(new string[] { });
			//CartOfTheDeal.IsActive = false;

			//CustomMutator FloralerFlora = RogueLibs.CreateCustomMutator(cChallenge.FloralerFlora, true,
			//	new CustomNameInfo("Features: Floral-er Flora"),
			//	new CustomNameInfo(
			//		"A mutator just for leaves? You got it!\n\n- Just adds leaves underneath bushes, hedge walls, plants and trees. Looks nice.\n- May be a performance strain for plebs"));
			//FloralerFlora.Available = false;
			//FloralerFlora.Conflicting.AddRange(new string[] { });
			//FloralerFlora.IsActive = false;

			//CustomMutator LakeItOrLeaveIt = RogueLibs.CreateCustomMutator(cChallenge.LakeItOrLeaveIt, true,
			//	new CustomNameInfo("Features: Lake it or Leave it"),
			//	new CustomNameInfo("Don't like large inland bodies of water? Too fuckin' bad, buddy!"));
			//LakeItOrLeaveIt.Available = false;
			//LakeItOrLeaveIt.Conflicting.AddRange(new string[] { });
			//LakeItOrLeaveIt.IsActive = false;

			//CustomMutator LitterallyTheWorst = RogueLibs.CreateCustomMutator(cChallenge.LitterallyTheWorst, true,
			//	new CustomNameInfo("Features: Litter-aly the Worst"),
			//	new CustomNameInfo(
			//		"Civic Pride went and died!\n\n- Generates trash on the ground and around trashcans\n- May be a performance strain for plebs"));
			//LitterallyTheWorst.Available = false;
			//LitterallyTheWorst.Conflicting.AddRange(new string[] { cChallenge.PoliceState, cChallenge.ArcologyEcology, cChallenge.MACITS });
			//LitterallyTheWorst.IsActive = false;

			//CustomMutator PowerWhelming = RogueLibs.CreateCustomMutator(cChallenge.PowerWhelming, true,
			//	new CustomNameInfo("Features: Power Whelming"),
			//	new CustomNameInfo("You're not gonna be *over*whelmed, but you will see Power Boxes in every district. And that's something, I guess."));
			//PowerWhelming.Available = false;
			//PowerWhelming.Conflicting.AddRange(new string[] { });
			//PowerWhelming.IsActive = false;

			//CustomMutator SkywayDistrict = RogueLibs.CreateCustomMutator(cChallenge.SkywayDistrict, true,
			//	new CustomNameInfo("Skyway District"),
			//	new CustomNameInfo(
			//		"The Canal water Downtown was sold off for a pretty penny, so now there are just deep, empty holes where it used to be. It's a hazard, but the profit was massive!"));
			//SkywayDistrict.Available = false;
			//SkywayDistrict.Conflicting.AddRange(new string[] { });
			//SkywayDistrict.IsActive = false;

			//CustomMutator SurveillanceSociety = RogueLibs.CreateCustomMutator(cChallenge.SurveillanceSociety, true,
			//	new CustomNameInfo("Features: Surveillance Society"),
			//	new CustomNameInfo(
			//		"Those cameras? For your safety.\n\nOh, the turrets? For their safety.\n\nThe invasion of privacy and midnight raids? What's your name, citizen?\n\n- Spawns Security Cameras & Turrets in public, aligned with The Law"));
			//SurveillanceSociety.Available = false;
			//SurveillanceSociety.Conflicting.AddRange(new string[] { });
			//SurveillanceSociety.IsActive = false;

			//CustomMutator ThePollutionSolution = RogueLibs.CreateCustomMutator(cChallenge.ThePollutionSolution, true,
			//	new CustomNameInfo("Features: The Pollution Solution"),
			//	new CustomNameInfo(
			//		"We've finally solved pollution! Make more, dump it everywhere, and then ignore it. Done.\n\n- Adds pollution features to levels\n- Lakes have 80% chance of being poisoned"));
			//ThePollutionSolution.Available = false;
			//ThePollutionSolution.Conflicting.AddRange(new string[] { });
			//ThePollutionSolution.IsActive = false;

			//CustomMutator Features_Hide = RogueLibs.CreateCustomMutator(cChallenge.Features_Hide, true,
			//	new CustomNameInfo("Features [-]"),
			//	new CustomNameInfo("Click to hide group"));
			//CustomMutator Features_Show = RogueLibs.CreateCustomMutator(cChallenge.Features_Show, true,
			//	new CustomNameInfo("Features [+]"),
			//	new CustomNameInfo("Click to show group"));

			//CustomMutator[] FeaturesMutators = new CustomMutator[]
			//{
			//	BadNeighborhoods, BroughtBackFountain, CartOfTheDeal, FloralerFlora, LakeItOrLeaveIt, LitterallyTheWorst, PowerWhelming, SkywayDistrict,
			//	SurveillanceSociety, ThePollutionSolution
			//};
			//Features_Hide.Available = false;
			//Features_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Features_Hide.Available = false;
			//	Features_Show.Available = true;

			//	foreach (CustomMutator m in FeaturesMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//Features_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Features_Hide.Available = true;
			//	Features_Show.Available = false;

			//	foreach (CustomMutator m in FeaturesMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion

			//#region Knockback

			//CustomMutator BoringPhysics = RogueLibs.CreateCustomMutator(cChallenge.BoringPhysics, true,
			//	new CustomNameInfo("Knockback: Boring Physics"),
			//	new CustomNameInfo("Knockback is reduced almost to zero. Nothing is gonna make this game realistic, ya know.\n\n- Knockback set to 10%"));
			//BoringPhysics.Available = false;
			//BoringPhysics.Conflicting.AddRange(new string[] { cChallenge.SaveTheWalls, vChallenge.BigKnockback, cChallenge.WallWallopWorld });
			//BoringPhysics.IsActive = false;

			//CustomMutator SaveTheWalls = RogueLibs.CreateCustomMutator(cChallenge.SaveTheWalls, true,
			//	new CustomNameInfo("Knockback: Save The Walls"),
			//	new CustomNameInfo(
			//		"There isn't much nature to protect in the City, so the Green party is a little confused. But due to the lobbying, there's now a city ordinance against punching people through walls, which is probably a good thing.\n\n- Knockback set to 50%."));
			//SaveTheWalls.Available = false;
			//SaveTheWalls.Conflicting.AddRange(new string[] { cChallenge.BoringPhysics, vChallenge.BigKnockback, cChallenge.WallWallopWorld });
			//SaveTheWalls.IsActive = false;

			//CustomMutator WallWallopWorld = RogueLibs.CreateCustomMutator(cChallenge.WallWallopWorld, true,
			//	new CustomNameInfo("Knockback: Wall Wallop World"),
			//	new CustomNameInfo(
			//		"Someone - not naming names here, but you know who you are - put a bunch of Muscly Pills onto the Fud Factory conveyor belt. And now we all have to suffer for it. Thanks a lot.\n\n- Knockback set to 500%"));
			//WallWallopWorld.Available = false;
			//WallWallopWorld.Conflicting.AddRange(new string[] { cChallenge.BoringPhysics, cChallenge.SaveTheWalls, vChallenge.BigKnockback });
			//WallWallopWorld.IsActive = false;

			//CustomMutator Knockback_Hide = RogueLibs.CreateCustomMutator(cChallenge.Knockback_Hide, true,
			//	new CustomNameInfo("Knockback [-]"),
			//	new CustomNameInfo("Click to hide group"));
			//CustomMutator Knockback_Show = RogueLibs.CreateCustomMutator(cChallenge.Knockback_Show, true,
			//	new CustomNameInfo("Knockback [+]"),
			//	new CustomNameInfo("Click to show group"));

			//CustomMutator[] KnockbackMutators = new CustomMutator[] { BoringPhysics, SaveTheWalls, WallWallopWorld };
			//Knockback_Hide.Available = false;
			//Knockback_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Knockback_Hide.Available = false;
			//	Knockback_Show.Available = true;

			//	foreach (CustomMutator m in KnockbackMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//Knockback_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Knockback_Hide.Available = true;
			//	Knockback_Show.Available = false;

			//	foreach (CustomMutator m in KnockbackMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion

			//#region Map Size

			//CustomMutator ACityForAnts = RogueLibs.CreateCustomMutator(cChallenge.ACityForAnts, true,
			//	new CustomNameInfo("MapSize: A City for Ants?!"),
			//	new CustomNameInfo("Yes, that is indeed what it is, figuratively speaking.\n\n- Map size set to 12.5%"));
			//ACityForAnts.Available = false;
			//ACityForAnts.Conflicting.AddRange(cChallenge.MapSize);
			//ACityForAnts.IsActive = false;

			//CustomMutator Claustropolis = RogueLibs.CreateCustomMutator(cChallenge.Claustropolis, true,
			//	new CustomNameInfo("MapSize: Claustrophobia"),
			//	new CustomNameInfo("Damn, this city is cramped! Who's Claus, anyway?\n\n- Map size set to 37.5%"));
			//Claustropolis.Available = false;
			//Claustropolis.Conflicting.AddRange(cChallenge.MapSize);
			//Claustropolis.IsActive = false;

			//CustomMutator Megalopolis = RogueLibs.CreateCustomMutator(cChallenge.Megalopolis, true,
			//	new CustomNameInfo("MapSize: Megalopolis"),
			//	new CustomNameInfo(
			//		"This town has so gotten big. You remember when it was just a small Mega-Arcology. Now it's a Mega-Mega-Arcology.\n\n- Map size set to 150%"));
			//Megalopolis.Available = false;
			//Megalopolis.Conflicting.AddRange(cChallenge.MapSize);
			//Megalopolis.IsActive = false;

			//CustomMutator Ultrapolis = RogueLibs.CreateCustomMutator(cChallenge.Ultrapolis, true,
			//	new CustomNameInfo("MapSize: Ultrapolis"),
			//	new CustomNameInfo("You get vertigo when you look up. This city is MASSIVE.\n\n- Map size set to 200%"));
			//Ultrapolis.Available = false;
			//Ultrapolis.Conflicting.AddRange(cChallenge.MapSize);
			//Ultrapolis.IsActive = false;

			//CustomMutator MapSize_Hide = RogueLibs.CreateCustomMutator(cChallenge.MapSize_Hide, true,
			//	new CustomNameInfo("MapSize [-]"),
			//	new CustomNameInfo("Click to hide group"));
			//CustomMutator MapSize_Show = RogueLibs.CreateCustomMutator(cChallenge.MapSize_Show, true,
			//	new CustomNameInfo("MapSize [+]"),
			//	new CustomNameInfo("Click to show group"));

			//CustomMutator[] MapSizeMutators = new CustomMutator[] { ACityForAnts, Claustropolis, Megalopolis, Ultrapolis };
			//MapSize_Hide.Available = false;
			//MapSize_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	MapSize_Hide.Available = false;
			//	MapSize_Show.Available = true;

			//	foreach (CustomMutator m in MapSizeMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//MapSize_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	MapSize_Hide.Available = true;
			//	MapSize_Show.Available = false;

			//	foreach (CustomMutator m in MapSizeMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion

			//#region Overhauls

			//CustomMutator AnCapistan = RogueLibs.CreateCustomMutator(cChallenge.AnCapistan, true,
			//	new CustomNameInfo("Overhaul: AnCapistan"),
			//	new CustomNameInfo(
			//		"Freedom, at last! Freedom to starve in the gutter and watch your children wallow in the poverty you could never escape. Keep on dreaming and you'll make it someday! Get out there and earn some bootstraps, you inspiring entrepeneur! #RespectTheHustle #LiveToGrind #PleaseHelpImStarving"));
			//AnCapistan.Available = false;
			//AnCapistan.Conflicting.AddRange(vChallenge.AddsLawEnforcement);
			//AnCapistan.Conflicting.AddRange(cChallenge.Overhauls);
			//AnCapistan.Conflicting.AddRange(new string[] { vChallenge.MixedUpLevels, vChallenge.NoGuns, cChallenge.ArcologyEcology });
			//AnCapistan.IsActive = false;
			//AnCapistan.OnEnabled += () =>
			//{
			//	BadNeighborhoods.IsActive = true;
			//	CartOfTheDeal.IsActive = true;
			//	LitterallyTheWorst.IsActive = true;
			//	ThePollutionSolution.IsActive = true;
			//	SurveillanceSociety.IsActive = false;
			//};

			//CustomMutator DiscoCityDanceoff = RogueLibs.CreateCustomMutator(cChallenge.DiscoCityDanceoff, true,
			//	new CustomNameInfo("Overhaul: Disco City Dance-off"),
			//	new CustomNameInfo(
			//		"Here's the skinny: this freaky deaky Mayor is just bad vibes, man. Time to make this city copacetic, can you dig it? Outta sight!"));
			//DiscoCityDanceoff.Available = false;
			//DiscoCityDanceoff.Conflicting.AddRange(cChallenge.Overhauls);
			//DiscoCityDanceoff.IsActive = false;
			//DiscoCityDanceoff.OnEnabled += () => { };

			//CustomMutator MACITS = RogueLibs.CreateCustomMutator(cChallenge.MACITS, true,
			//	new CustomNameInfo("Overhaul: Mostly Automated Comfortable Inclusive Terrestrial Socialism"),
			//	new CustomNameInfo(
			//		"The Revolution is complete! No more living in privation to feed the fat capitalist pigs! Now all that's left is to have another Revolution and fuck it all up!"));
			//MACITS.Available = false;
			//MACITS.Conflicting.AddRange(cChallenge.AddCriminals);
			//MACITS.Conflicting.AddRange(cChallenge.Overhauls);
			//MACITS.Conflicting.AddRange(vChallenge.RemovesLawEnforcement);
			//MACITS.Conflicting.Add(vChallenge.MixedUpLevels);
			//MACITS.IsActive = false;
			//MACITS.OnEnabled += () =>
			//{
			//	BadNeighborhoods.IsActive = false;
			//	LitterallyTheWorst.IsActive = false;
			//	ThePollutionSolution.IsActive = false;
			//};

			//CustomMutator PoliceState = RogueLibs.CreateCustomMutator(cChallenge.PoliceState, true,
			//	new CustomNameInfo("Overhaul: Police State"),
			//	new CustomNameInfo("The Mayor has decided to get tough on crime. What's crime? Whatever we want it to be."));
			//PoliceState.Available = false;
			//PoliceState.Conflicting.AddRange(cChallenge.AddCriminals);
			//PoliceState.Conflicting.AddRange(vChallenge.AddsNonhumans);
			//PoliceState.Conflicting.AddRange(cChallenge.Overhauls);
			//PoliceState.Conflicting.AddRange(vChallenge.RemovesLawEnforcement);
			//PoliceState.Conflicting.AddRange(vChallenge.Zombies);
			//PoliceState.Conflicting.Add(vChallenge.MixedUpLevels);
			//PoliceState.IsActive = false;
			//PoliceState.OnEnabled += () =>
			//{
			//	BadNeighborhoods.IsActive = false;
			//	LitterallyTheWorst.IsActive = false;
			//	ThePollutionSolution.IsActive = false;
			//	SurveillanceSociety.IsActive = true;
			//};

			//#endregion

			//#region Population

			//CustomMutator GhostTown = RogueLibs.CreateCustomMutator(cChallenge.GhostTown, true,
			//	new CustomNameInfo("Population: Ghost Town"),
			//	new CustomNameInfo(
			//		"No one walks the streets in this city. Don't act all innocent, I know what you do to people in this game!\n\n- Wandering population set to 0%"));
			//GhostTown.Available = false;
			//GhostTown.Conflicting.AddRange(cChallenge.Population);
			//GhostTown.IsActive = false;

			//CustomMutator HordeAlmighty = RogueLibs.CreateCustomMutator(cChallenge.HordeAlmighty, true,
			//	new CustomNameInfo("Population: Horde Almighty"),
			//	new CustomNameInfo(
			//		"The City administration is trying out a contraception ban to combat the high death rate. Hope it works, because they didn't think of a \"Plan B!\" Get it? I'm here all week, folks.\n\n- Wandering population set to 200%\n- You might get pregnant"));
			//HordeAlmighty.Available = false;
			//HordeAlmighty.Conflicting.AddRange(cChallenge.Population);
			//HordeAlmighty.IsActive = false;

			//CustomMutator LetMeSeeThatThrong = RogueLibs.CreateCustomMutator(cChallenge.LetMeSeeThatThrong, true,
			//	new CustomNameInfo("Population: Let Me See That Throng"),
			//	new CustomNameInfo(
			//		"The City's HR department is still working out an effective Eugenics program. For now, people are breeding out of control.\n\n- Wandering population set to 400%"));
			//LetMeSeeThatThrong.Available = false;
			//LetMeSeeThatThrong.Conflicting.AddRange(cChallenge.Population);
			//LetMeSeeThatThrong.IsActive = false;

			//CustomMutator SwarmWelcome = RogueLibs.CreateCustomMutator(cChallenge.SwarmWelcome, true,
			//	new CustomNameInfo("Population: Swarm Welcome"),
			//	new CustomNameInfo(
			//		"This whole city feels like a crowded subway. Pickpocketing is bad enough, but the frottage is out of control!\n\n- Wandering population set to 800%"));
			//SwarmWelcome.Available = false;
			//SwarmWelcome.Conflicting.AddRange(cChallenge.Population);
			//SwarmWelcome.IsActive = false;

			//CustomMutator Population_Hide = RogueLibs.CreateCustomMutator(cChallenge.Population_Hide, true,
			//	new CustomNameInfo("Population [-]"),
			//	new CustomNameInfo("Click to hide group"));
			//CustomMutator Population_Show = RogueLibs.CreateCustomMutator(cChallenge.Population_Show, true,
			//	new CustomNameInfo("Population [+]"),
			//	new CustomNameInfo("Click to show group"));

			//CustomMutator[] PopulationMutators = new CustomMutator[] { GhostTown, HordeAlmighty, LetMeSeeThatThrong, SwarmWelcome };
			//Population_Hide.Available = false;
			//Population_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Population_Hide.Available = false;
			//	Population_Show.Available = true;

			//	foreach (CustomMutator m in PopulationMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//Population_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Population_Hide.Available = true;
			//	Population_Show.Available = false;

			//	foreach (CustomMutator m in PopulationMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion

			//#region Quest Count

			//CustomMutator RushinRevolution = RogueLibs.CreateCustomMutator(cChallenge.RushinRevolution, true,
			//	new CustomNameInfo("QuestCount: Rushin' Revolution"),
			//	new CustomNameInfo(
			//		"There are decades where nothing happens; and there are weeks where decades happen. And then there are days where you just don't have time for this shit.\n\nNo quests. Bum rush the Mayor. Take a long weekend."));
			//RushinRevolution.Available = false;
			//RushinRevolution.Conflicting.AddRange(cChallenge.QuestCount);
			//RushinRevolution.IsActive = false;

			//CustomMutator SingleMinded = RogueLibs.CreateCustomMutator(cChallenge.SingleMinded, true,
			//	new CustomNameInfo("QuestCount: Single-minded"),
			//	new CustomNameInfo("Your Resistance HR profile says \"Not a good multi-tasker.\" They only give you one job per Floor."));
			//SingleMinded.Available = false;
			//SingleMinded.Conflicting.AddRange(cChallenge.QuestCount);
			//SingleMinded.IsActive = false;

			//CustomMutator Workhorse = RogueLibs.CreateCustomMutator(cChallenge.Workhorse, true,
			//	new CustomNameInfo("QuestCount: Workhorse"),
			//	new CustomNameInfo(
			//		"You made the mistake of being reliable. Now the Resistance sends you all the work. You don't mind, because the long hours are an excuse to avoid your family."));
			//Workhorse.Available = false;
			//Workhorse.Conflicting.AddRange(cChallenge.QuestCount);
			//Workhorse.IsActive = false;

			//CustomMutator QuestCount_Hide = RogueLibs.CreateCustomMutator(cChallenge.QuestCount_Hide, true,
			//	new CustomNameInfo("QuestCount [-]"),
			//	new CustomNameInfo("Click to hide group"));
			//CustomMutator QuestCount_Show = RogueLibs.CreateCustomMutator(cChallenge.QuestCount_Show, true,
			//	new CustomNameInfo("QuestCount [+]"),
			//	new CustomNameInfo("Click to show group"));

			//CustomMutator[] QuestCountMutators = new CustomMutator[] { RushinRevolution, SingleMinded, Workhorse };
			//QuestCount_Hide.Available = false;
			//QuestCount_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	QuestCount_Hide.Available = false;
			//	QuestCount_Show.Available = true;

			//	foreach (CustomMutator m in QuestCountMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//QuestCount_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	QuestCount_Hide.Available = true;
			//	QuestCount_Show.Available = false;

			//	foreach (CustomMutator m in QuestCountMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion

			//#region Quest Rewards

			//CustomMutator DoublePlyRewards = RogueLibs.CreateCustomMutator(cChallenge.DoublePlyRewards, true,
			//	new CustomNameInfo("Rewards: Double-Ply Rewards"),
			//	new CustomNameInfo(
			//		"The Resistance is running low on... everything. But please, accept this stack of coupons! They are totally really actually valid.\n\nThe smell? That's *value*!"));
			//DoublePlyRewards.Available = false;
			//DoublePlyRewards.Conflicting.Add(vChallenge.MoneyRewards);
			//DoublePlyRewards.Conflicting.AddRange(new string[] { cChallenge.UnpaidInternship });
			//DoublePlyRewards.IsActive = false;

			//CustomMutator UnpaidInternship = RogueLibs.CreateCustomMutator(cChallenge.UnpaidInternship, true,
			//	new CustomNameInfo("Rewards: Unpaid Internship"),
			//	new CustomNameInfo(
			//		"The double-experience you're getting working for the Resistance is worth more than any reward, they say. But so far, you're mainly learning one thing: Work for people who pay you. At least they provide lunch, though?"));
			//UnpaidInternship.Available = false;
			//UnpaidInternship.Conflicting.Add(vChallenge.MoneyRewards);
			//UnpaidInternship.Conflicting.AddRange(new string[] { cChallenge.DoublePlyRewards });
			//UnpaidInternship.IsActive = false;

			//CustomMutator QuestRewards_Hide = RogueLibs.CreateCustomMutator(cChallenge.QuestRewards_Hide, true,
			//	new CustomNameInfo("QuestRewards [-]"),
			//	new CustomNameInfo("Click to hide group"));
			//CustomMutator QuestRewards_Show = RogueLibs.CreateCustomMutator(cChallenge.QuestRewards_Show, true,
			//	new CustomNameInfo("QuestRewards [+]"),
			//	new CustomNameInfo("Click to show group"));

			//CustomMutator[] QuestRewardsMutators = new CustomMutator[] { DoublePlyRewards, UnpaidInternship };
			//QuestRewards_Hide.Available = false;
			//QuestRewards_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	QuestRewards_Hide.Available = false;
			//	QuestRewards_Show.Available = true;

			//	foreach (CustomMutator m in QuestRewardsMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//QuestRewards_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	QuestRewards_Hide.Available = true;
			//	QuestRewards_Show.Available = false;

			//	foreach (CustomMutator m in QuestRewardsMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion

			//#region Roamers

			//CustomMutator HoodlumsWonderland = RogueLibs.CreateCustomMutator(cChallenge.HoodlumsWonderland, true,
			//	new CustomNameInfo("Roamers: Hoodlum's Wonderland"),
			//	new CustomNameInfo(
			//		"The annual charity drive for the Blahds and Crepes happened to overlap this year. They're in tough competition to sell the most cookies! Roaming gang spawns are increased. By a lot."));
			//HoodlumsWonderland.Available = false;
			//HoodlumsWonderland.Conflicting.AddRange(new string[] { });
			//HoodlumsWonderland.IsActive = false;

			//CustomMutator MobTown = RogueLibs.CreateCustomMutator(cChallenge.MobTown, true,
			//	new CustomNameInfo("Roamers: Mob Town"),
			//	new CustomNameInfo("Turns out the Union Membership is high here for all the wrong reasons. The Mob is in every district."));
			//MobTown.Available = false;
			//MobTown.Conflicting.AddRange(new string[] { });
			//MobTown.IsActive = false;

			//CustomMutator YoungMenIntheNeighborhood = RogueLibs.CreateCustomMutator(cChallenge.YoungMenInTheNeighborhood, true,
			//	new CustomNameInfo("Roamers: Young Men in the Neighborhood"),
			//	new CustomNameInfo(
			//		"Beause the young gentlemen in the hood are always polite; If you start acting rude, we'll set you right!\nYour friendly local Gangbangers now roam every district."));
			//YoungMenIntheNeighborhood.Available = false;
			//YoungMenIntheNeighborhood.Conflicting.AddRange(new string[] { });
			//YoungMenIntheNeighborhood.IsActive = false;

			//CustomMutator Roamers_Hide = RogueLibs.CreateCustomMutator(cChallenge.Roamers_Hide, true,
			//	new CustomNameInfo("Roamers [-]"),
			//	new CustomNameInfo("Click to hide group"));
			//CustomMutator Roamers_Show = RogueLibs.CreateCustomMutator(cChallenge.Roamers_Show, true,
			//	new CustomNameInfo("Roamers [+]"),
			//	new CustomNameInfo("Click to show group"));

			//CustomMutator[] RoamersMutators = new CustomMutator[] { HoodlumsWonderland, MobTown, YoungMenIntheNeighborhood };
			//Roamers_Hide.Available = false;
			//Roamers_Hide.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Roamers_Hide.Available = false;
			//	Roamers_Show.Available = true;

			//	foreach (CustomMutator m in RoamersMutators)
			//		m.Available = false;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};
			//Roamers_Show.ScrollingMenu_PushedButton = (menu, button) =>
			//{
			//	GC.audioHandler.Play(GC.playerAgent, vAudioClip.ClickButton);

			//	Roamers_Hide.Available = true;
			//	Roamers_Show.Available = false;

			//	foreach (CustomMutator m in RoamersMutators)
			//		m.Available = true;

			//	GC.mainGUI.scrollingMenuScript.OpenScrollingMenu();

			//	return false;
			//};

			//#endregion
		}
	}
}