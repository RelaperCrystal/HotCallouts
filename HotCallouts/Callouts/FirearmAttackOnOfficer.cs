/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2019/9/6
 * 时间: 12:03
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using GTA;
using LCPD_First_Response.Engine;
using LCPD_First_Response.LCPDFR.API;
using LCPD_First_Response.LCPDFR.Callouts;

namespace HotCallouts.Callouts
{
	/// <summary>
	/// Description of FirearmAttackOnOfficer.
	/// </summary>
	[CalloutInfo("FirearmAttackOnOfficer", ECalloutProbability.High)]
	public class FirearmAttackOnOfficer : Callout
	{
		LPed target;
		LPed officer;
		
		Blip officerBlip;
		Blip targetBlip;
		
		LHandle pursuit;
		
		bool IsCombatSetAleradySet;
		
		SpawnPoint spawnPoint;
		public FirearmAttackOnOfficer()
		{
			this.CalloutMessage = "We have a firearm attack on a officer, available units please respond.";
		}
		
		public override bool OnBeforeCalloutDisplayed()
		{
			spawnPoint = Callout.GetSpawnPointInRange(LPlayer.LocalPlayer.Ped.Position, 100, 400);
			if(this.spawnPoint == SpawnPoint.Zero)
			{
				return false;
			}
			
			this.ShowCalloutAreaBlipBeforeAccepting(this.spawnPoint.Position, 50f);
            this.AddMinimumDistanceCheck(80f, this.spawnPoint.Position);
            
            string area = Functions.GetAreaStringFromPosition(this.spawnPoint.Position);
            this.CalloutMessage = "We have report of Firearm Attack on an officer in " + area + ", all units please respond.";
            
            string audioMessage = Functions.CreateRandomAudioIntroString(EIntroReportedBy.Officers);
            string crimeMessage = "CRIM_A_FIREARM_ATTACK_ON_AN_OFFICER";
            if (Common.GetRandomBool(0, 2, 1))
            {
                crimeMessage = "CRIM_AN_OFFICER_ASSAULT";
            }
            
            Functions.PlaySoundUsingPosition(audioMessage + crimeMessage + " IN_OR_ON_POSITION", this.spawnPoint.Position);
			
			return base.OnBeforeCalloutDisplayed();
		}
		
		public override bool OnCalloutAccepted()
		{
			
			
			target = new LPed(spawnPoint.Position, "M_Y_GBIK_LO_01");
			officer = new LPed(target.Position.Around(10.0f), "M_Y_COP");
			
			officer.EquipWeapon();
			target.EquipWeapon();
			
			officerBlip = officer.AttachBlip();
			targetBlip = target.AttachBlip();
			
			officerBlip.Friendly = true;
			
			targetBlip.Icon = BlipIcon.Misc_CopHeli;
			targetBlip.Scale = 0.5f;
			targetBlip.Color = BlipColor.Red;
			targetBlip.RouteActive = true;
			targetBlip.Name = "Suspect";
			
			
			
			Functions.AddToScriptDeletionList(target, this);
			Functions.SetPedIsOwnedByScript(target, this, true);
			
			Functions.AddToScriptDeletionList(target, this);
			Functions.SetPedIsOwnedByScript(target, this, true);
			
			return base.OnCalloutAccepted();
		}
		
		/// <summary>
		/// Running repeatedly when script is still running.
		/// </summary>
		public override void Process()
		{
			base.Process();
			
			if(LPlayer.LocalPlayer.Ped.Position.DistanceTo(target.Position) <= 10)
			{
				targetBlip.RouteActive = false;
				
				target.Task.FightAgainst(officer);
				officer.Task.FightAgainst(target);
			}
			
			if(target.HasBeenArrested)
			{
				Functions.PrintText("Suspect Apprehended!", 7000);
				Functions.AddTextToTextwall("Code 4 - Suspect in custody.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
				base.SetCalloutFinished(true, true, true);
			}
			else if(!target.IsAliveAndWell || !target.Exists())
			{
				Functions.AddTextToTextwall("Code 4 - Suspect netrulized.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
				base.SetCalloutFinished(true, true, true);
			}
			
		 
		}
		
		public override void PedLeftScript(LPed ped)
		{
			Functions.SetPedIsOwnedByScript(ped, this, false);
			Functions.RemoveFromDeletionList(ped, this);
			
			base.PedLeftScript(ped);
		}
	}
}
