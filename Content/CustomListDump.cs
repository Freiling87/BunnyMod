using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using RogueLibsCore;
using UnityEngine;
using System.Collections;
using HarmonyLib;
using System.Reflection;
using BunnyMod.Content.Traits;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using Light2D;

namespace BunnyMod.Content
{
	public static class CustomListDump
	{
		public static List<string> customThrowables = new List<string>()
		{
				cItem.Brick,
				cItem.BeerCan,
				cItem.Lunchbox,
				cItem.ManholeCover,
				cItem.Sawblade,
				cItem.ThrowingKnife,
				cItem.WhiskeyBottle
		};

		// https://colordesigner.io/color-mixer
		public static Color32 arenaRingColor = new Color32(167, 76, 134, 200);
		public static Color32 blueColor = new Color32(62, 62, 255, 200);
		public static Color32 cyanColor = new Color32(0, 113, 159, 200);
		public static Color32 cyanGreenColor = new Color32(0, 180, 143, 200);
		public static Color32 defaultColor = new Color32(161, 161, 161, 105);
		public static Color32 discoBlueColor = new Color32(64, 64, 255, 200);
		public static Color32 discoGreenColor = new Color32(85, 170, 0, 200);
		public static Color32 discoOrangeColor = new Color32(255, 188, 64, 200);
		public static Color32 discoPurpleColor = new Color32(140, 52, 173, 200);
		public static Color32 discoRedColor = new Color32(255, 85, 85, 200);
		public static Color32 discoYellowColor = new Color32(255, 255, 85, 200);
		public static Color32 fireStationColor = new Color32(125, 87, 248, 111);
		public static Color32 greenColor = new Color32(0, 159, 60, 200);
		public static Color32 homeColor = new Color32(199, 174, 120, 160);
		public static Color32 homeColorMayorVillage = new Color32(212, 122, 244, 160);
		public static Color32 homeColorUptown = new Color32(205, 173, 219, 85);
		public static Color32 labColor = new Color32(64, 224, 255, 180);
		public static Color32 lakeColor = new Color32(0, 213, 255, 85);
		public static Color32 lightBlueColor = new Color32(124, 151, 189, 180);
		public static Color32 lightBlueColorMayorVillage = new Color32(44, 106, 193, 180);
		public static Color32 mallColor = new Color32(255, 255, 255, 80);
		public static Color32 pinkColor = new Color32(159, 0, 148, 200);
		public static Color32 pinkWhiteColor = new Color32(208, 163, 255, 120);
		public static Color32 poolColor = new Color32(0, 213, 255, 85);
		public static Color32 poolColorLighter = new Color32(144, 237, 255, 85);
		public static Color32 privateClubColor = new Color32(163, 178, 110, 160);
		public static Color32 purpleColor = new Color32(111, 0, 159, 200);
		public static Color32 redColor = new Color32(159, 0, 0, 200);
		public static Color32 whiteColor = new Color32(255, 255, 255, 120);
		public static Color32 zooColor = new Color32(0, 255, 181, 85);

		public static List<Color32> discoColors = new List<Color32>()
		{
				discoBlueColor,
				discoGreenColor,
				discoOrangeColor,
				discoPurpleColor,
				discoRedColor,
				discoYellowColor,
		};
	}
}
