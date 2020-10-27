using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;
using HarmonyLib;
using UnityEngine;
using RogueLibsCore;

namespace BunnyMod
{
	/*See:
	 * int Agent.DoingMugging
	 * AgentInteractions.MugMoney - if player, else ObjectAction
	 * Relationships.ProtectOwnedLight
	 * AgentInteractions.ThreatenMoney
	 */

	public class BunnyBehaviors
	{
		#region generic
		public void Awake()
		{
			Initialize_Names();
		}
		public void FixedUpdate()
		{
		}
		public void Initialize_Names()
		{
		}
		#endregion

		#region AgentInteractions
		#endregion
		
		#region ObjectMult
		public static void ObjectMult_CallRpcSetGangMugging(int myGang, ObjectMult __instance)
		{
		}
		public static void ObjectMult_CallRpcSetGangMuggingOff()
		{
		}
		public static void ObjectMult_InvokeRpcRpcSetGangMugging()
		{
		}
		public static void ObjectMult_InvokeRpcRpcSetGangMuggingOff()
		{
		}
		public static void ObjectMult_SetGangMugging()
		{
		}
		public static void ObjectMult_SetGangMuggingOff()
		{
		}
		public static void ObjectMult_RpcSetGangMugging()
		{
		}
		public static void ObjectMult_RpcSetGangMuggingOff()
		{
		}
		#endregion
	}
}
