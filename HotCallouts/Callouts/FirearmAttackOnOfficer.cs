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
    public enum EFirearmAttackType
    {
        Attack,
        TakingAim,
        DrawGun
    }
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

        EFirearmAttackType CallType;
		
		SpawnPoint spawnPoint;
		public FirearmAttackOnOfficer()
		{
			this.CalloutMessage = "We have a firearm attack on a officer, available units please respond.";
		}
		
		public override bool OnBeforeCalloutDisplayed()
		{
            CallType = (EFirearmAttackType)Common.GetRandomEnumValue(typeof(EFirearmAttackType));
			spawnPoint = Callout.GetSpawnPointInRange(LPlayer.LocalPlayer.Ped.Position, 100, 400);
			if(this.spawnPoint == SpawnPoint.Zero)
			{
				return false;
			}
			
			this.ShowCalloutAreaBlipBeforeAccepting(this.spawnPoint.Position, 50f);
            this.AddMinimumDistanceCheck(80f, this.spawnPoint.Position);
            
            switch(CallType)
            {
                case EFirearmAttackType.Attack :
                    string area = Functions.GetAreaStringFromPosition(this.spawnPoint.Position);
                    this.CalloutMessage = "We have report of Firearm Attack on an officer in " + area + ", all units please respond.";
            
                    string audioMessageAt = Functions.CreateRandomAudioIntroString(EIntroReportedBy.Officers);
                    string crimeMessageAt = "CRIM_A_FIREARM_ATTACK_ON_AN_OFFICER";
                    if (Common.GetRandomBool(0, 2, 1))
                    {
                        crimeMessageAt = "CRIM_AN_OFFICER_ASSAULT";
                    }
            
                    Functions.PlaySoundUsingPosition(audioMessageAt + crimeMessageAt + " IN_OR_ON_POSITION", this.spawnPoint.Position);
                    break;
                case EFirearmAttackType.TakingAim :
                    string areaTA = Functions.GetAreaStringFromPosition(this.spawnPoint.Position);
                    this.CalloutMessage = "We have report of a suspect taking aim at an officer in " + areaTA + ", all units please respond.";
            
                    string audioMessageTA = Functions.CreateRandomAudioIntroString(EIntroReportedBy.Officers);
                    string crimeMessageTA = "CRIM_A_FIREARM_ATTACK_ON_AN_OFFICER";
                    if (Common.GetRandomBool(0, 2, 1))
                    {
                        crimeMessageTA = "CRIM_AN_OFFICER_ASSAULT";
                    }
            
                    Functions.PlaySoundUsingPosition(audioMessageTA + crimeMessageTA + " IN_OR_ON_POSITION", this.spawnPoint.Position);
                    break;
                case EFirearmAttackType.DrawGun :
                    string areaDG = Functions.GetAreaStringFromPosition(this.spawnPoint.Position);
                    this.CalloutMessage = "We have report of a suspect holding a gun to officer, in " + areaDG + ".";

                    string audioMessageDG = Functions.CreateRandomAudioIntroString(EIntroReportedBy.Officers);
                    string crimeMessageDG = "CRIM_A_TAKING_AIM_AT_OFFICER_WITH_A_FIREARM";
                    Functions.PlaySoundUsingPosition(audioMessageDG + crimeMessageDG + " IN_OR_ON_POSITION", this.spawnPoint.Position);
                    break;
            }
			return base.OnBeforeCalloutDisplayed();
		}
		
		public override bool OnCalloutAccepted()
		{
            
            target = new LPed(spawnPoint.Position, "M_Y_GBIK_LO_01");
            officer = new LPed(target.Position.Around(10.0f), "M_Y_COP");
            switch(CallType)
            {
                case EFirearmAttackType.Attack :
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

                    target.Task.FightAgainst(officer);
				    officer.Task.FightAgainst(target);

                    Functions.PrintText("Get to the ~y~crime scene~w~ ASAP!", 7000);
                    Functions.AddTextToTextwall("Be advised, we have another unit responding.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                    Functions.RequestPoliceBackupAtPosition(spawnPoint.Position);
                    break;
                case EFirearmAttackType.TakingAim :
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

                    officer.Task.AimAt(target.GPed, -1);
                    target.Task.AimAt(officer.GPed, -1);
                    Functions.PrintText("Get to the ~crime scene~w~.", 7000);
                    break;
                case EFirearmAttackType.DrawGun :
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

                    officer.Task.AimAt(target, -1);
                    Functions.PrintText("Get to the ~crime scene~w~.", 7000);
                    break;
            }

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
			else if(!officer.IsAliveAndWell)
            {
                Functions.AddTextToTextwall("All units, we ahave an officer down.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
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

            if (this.officerBlip != null && this.officerBlip.Exists())
            {
                this.officerBlip.Delete();
            }
            
            if(this.targetBlip != null && this.targetBlip.Exists())
            {
            	this.targetBlip.Delete();
            }
        }
	}
}
