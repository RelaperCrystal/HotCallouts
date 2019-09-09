/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2019/9/5
 * 时间: 14:38
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using LCPD_First_Response.Engine;
using LCPD_First_Response.LCPDFR.API;
using LCPD_First_Response.LCPDFR.Callouts;
using GTA;

namespace HotCallouts.Callouts
{
	/// <summary>
	/// Description of DangerousDriver.
	/// </summary>
	[CalloutInfo("DangerousDriver", ECalloutProbability.Medium)]
	public class DangerousDriver : Callout
	{
		Blip blip;
		LPed driver;
		LVehicle car;
		
		private string[] criminalModels = new string[10] { "M_Y_GBIK_LO_01", "M_Y_GBIK_HI_01", "M_Y_GRUS_LO_01", "M_Y_GRU2_LO_01", "M_Y_GMAF_LO_01", "M_Y_GMAF_HI_01", "M_Y_GTRI_LO_01", "M_Y_GTRI_LO_02", "M_Y_GALB_LO_01", "M_Y_GALB_LO_02" };
		
		public SpawnPoint spawnPoint;
		
		/// <summary>
		/// Make a new <see cref="DangerousDriver" />
		/// </summary>
		public DangerousDriver()
		{
			this.CalloutMessage = "We have report of Dangerous Driver, confirming information.";
		}
		
		public override bool OnBeforeCalloutDisplayed()
		{
			this.spawnPoint = Callout.GetSpawnPointInRange(LPlayer.LocalPlayer.Ped.Position, 100, 400);
			
			if(this.spawnPoint == SpawnPoint.Zero)
			{
				return false;
			}
			
			this.ShowCalloutAreaBlipBeforeAccepting(this.spawnPoint.Position, 50f);
            this.AddMinimumDistanceCheck(80f, this.spawnPoint.Position);
            
            string area = Functions.GetAreaStringFromPosition(this.spawnPoint.Position);
            this.CalloutMessage = "We have report of Dangerous Driver in " + area + ", available unit please respond.";
            
            string audioMessage = Functions.CreateRandomAudioIntroString(EIntroReportedBy.Civilians);
            string crimeMessage = "CRIM_A_RECKLESS_DRIVER";
            if (Common.GetRandomBool(0, 2, 1))
            {
                crimeMessage = "CRIM_A_SPEEDING_FELONY";
            }
            
            Functions.PlaySoundUsingPosition(audioMessage + crimeMessage + " IN_OR_ON_POSITION", this.spawnPoint.Position);
            
			return base.OnBeforeCalloutDisplayed();
		}
		
		public override bool OnCalloutAccepted()
		{
			base.OnCalloutAccepted();
			
			driver = new LPed(spawnPoint.Position, "M_Y_STREET_01");
			Vehicle temp = World.CreateVehicle(spawnPoint.Position.Around(5.0f));
			car = LVehicle.FromGTAVehicle(temp);
			
			if(driver.Exists() || car.Exists())
			{
				driver.CanResistArrest = true;
				driver.Task.WarpIntoVehicle(car.GVehicle, VehicleSeat.Driver);
				driver.Task.CruiseWithVehicle(car.GVehicle, 20f, false);
				blip = driver.AttachBlip();
                blip.Icon = BlipIcon.Building_PoliceStation;
                blip.Color = BlipColor.Red;
                blip.Name = "Suspect";
                blip.RouteActive = true;
                
                Functions.AddToScriptDeletionList(driver, this);
                Functions.SetPedIsOwnedByScript(driver, this, true);
                
                Functions.AddToScriptDeletionList(car, this);
			}
			else
			{
				return false;
			}
			return true;
		}
		
		public override void Process()
		{
			base.Process();
			
			if(driver.HasBeenArrested)
			{
				Functions.PrintText("Suspect Apprehended!", 7000);
				Functions.AddTextToTextwall("Code 4 - Suspect in custody.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
				base.SetCalloutFinished(true, true, true);
			}
			
			if(!driver.IsAliveAndWell || !driver.Exists())
			{
				Functions.AddTextToTextwall("Code 4 - Suspect netrulized.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
				base.SetCalloutFinished(true, true, true);
			}
		}
		
		/// <summary>
		/// Called when callout is becoming end, by calling <see cref="SetCalloutFinished()" />
		/// </summary>
		public override void End()
        {
            base.End();

            if (this.blip != null && this.blip.Exists())
            {
                this.blip.Delete();
            }
        }
		
		/// <summary>
		/// Called when ped left script, such as being arrested etc. In this method, the ped will be REMOVED from
		/// This script, and the from the Deletion List.
		/// </summary>
		/// <param name="ped">The ped that being removed.</param>
		public override void PedLeftScript(LPed ped)
		{
			Functions.RemoveFromDeletionList(ped, this);
			Functions.SetPedIsOwnedByScript(ped, this, false);
			
			base.PedLeftScript(ped);
		}
		
		
	}
}
