/*
 * 由SharpDevelop创建。
 * 用户： RelaperCrystal
 * 日期: 2019/9/7
 * 时间: 11:15
 */
using System;
using GTA;
using LCPD_First_Response.Engine;
using LCPD_First_Response.LCPDFR.API;
using LCPD_First_Response.LCPDFR.Callouts;

namespace HotCallouts.Callouts
{
	/// <summary>
	/// Description of KillingSpree.
	/// </summary>
	[CalloutInfo("TerroristAttack", ECalloutProbability.Low)]
	public class TerroristAttack : Callout
	{
		LPed target;
		Blip targetBlip;
		
		SpawnPoint spawnPoint;
		
		public TerroristAttack()
		{
			this.CalloutMessage = "All available units, we have a terrorist attack.";
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
            this.CalloutMessage = "Attention all units, terrorist attack in progress at " + area + ", all units please respond.";
            
            string audioMessage = Functions.CreateRandomAudioIntroString(EIntroReportedBy.Civilians);
            string crimeMessage = "CRIM_A_TERRORIST_ACTIVITY";
            if (Common.GetRandomBool(0, 2, 1))
            {
                crimeMessage = "CRIM_A_CRIMINAL_ON_A_GUN_SPREE";
            }
            
            Functions.PlaySoundUsingPosition(audioMessage + crimeMessage + " IN_OR_ON_POSITION", this.spawnPoint.Position);
			
			return base.OnBeforeCalloutDisplayed();
		}
		
		public override bool OnCalloutAccepted()
		{
			target = new LPed(spawnPoint.Position, "M_Y_STREET_01", LPed.EPedGroup.Criminal);
            target.Weapons.AssaultRifle_M4.Ammo = 99999;
            target.Weapons.Select(Weapon.Rifle_M4);
            target.RelationshipGroup = RelationshipGroup.Criminal;
			target.ChangeRelationship(RelationshipGroup.Civillian_Male, Relationship.Hate);
			target.ChangeRelationship(RelationshipGroup.Civillian_Female, Relationship.Hate);
			target.ChangeRelationship(RelationshipGroup.Criminal, Relationship.Hate);
			target.ChangeRelationship(RelationshipGroup.Cop, Relationship.Hate);
			target.Task.FightAgainstHatedTargets(-1);
			
			Functions.SetPedIsOwnedByScript(target, this, true);
			Functions.AddToScriptDeletionList(target, this);

            Functions.AddTextToTextwall("Dispatch 4 units from " + Functions.GetAreaStringFromPosition(LPlayer.LocalPlayer.Ped.Position), Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
            Functions.PlaySoundUsingPosition("ATTENTION_ALL_UNITS DFROM_DISPATCH_4_UNITS_FROM POSITION", this.spawnPoint.Position);
            Functions.RequestPoliceBackupAtPosition(spawnPoint.Position);
            Functions.RequestPoliceBackupAtPosition(spawnPoint.Position);
            Functions.RequestPoliceBackupAtPosition(spawnPoint.Position);
            Functions.RequestPoliceBackupAtPosition(spawnPoint.Position);

            Functions.PrintHelp("You may call NOOSE team if required.");
			
			return base.OnCalloutAccepted();
		}
		
		public override void Process()
		{
			base.Process();
			
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
		
		public override void End()
		{
			base.End();
			
			if(this.targetBlip != null && this.targetBlip.Exists())
			{
				this.targetBlip.Delete();
			}
		}
	}
}
