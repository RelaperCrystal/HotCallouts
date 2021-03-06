﻿using GTA;
using LCPD_First_Response.Engine;
using LCPD_First_Response.LCPDFR.API;
using LCPD_First_Response.LCPDFR.Callouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotCallouts.Callouts
{
    [ExperimentMode(typeof(CarTheft), ElementType.Callout), 
    CalloutInfo("CarTheft", ECalloutProbability.High)]
    public class CarTheft : Callout
    {
        LVehicle car;
        LPed suspect;
        LPed passenger;
        public SpawnPoint spawnPoint;

        public override bool OnBeforeCalloutDisplayed()
        {
            this.spawnPoint = Callout.GetSpawnPointInRange(LPlayer.LocalPlayer.Ped.Position, 100, 400);

            if (this.spawnPoint == SpawnPoint.Zero)
            {
                return false;
            }

            this.ShowCalloutAreaBlipBeforeAccepting(this.spawnPoint.Position, 50f);
            this.AddMinimumDistanceCheck(80f, this.spawnPoint.Position);

            string area = Functions.GetAreaStringFromPosition(this.spawnPoint.Position);
            this.CalloutMessage = "We have report of a stolen vehicle driving in " + area + ", available unit please respond.";

            string audioMessage = Functions.CreateRandomAudioIntroString(EIntroReportedBy.Civilians);
            string crimeMessage = "CRIM_A_STOLEN_VEHICLE";
            if (Common.GetRandomBool(0, 2, 1))
            {
                crimeMessage = "CRIM_A_CRIMINAL_IN_A_STOLEN_VEHICLE";
            }

            Functions.PlaySoundUsingPosition(audioMessage + crimeMessage + " IN_OR_ON_POSITION", this.spawnPoint.Position);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            base.OnCalloutAccepted();

            car = LVehicle.FromGTAVehicle(World.CreateVehicle(spawnPoint.Position.Around(10f)));
            car.IsRequiredForMission = true;

            suspect = new LPed(spawnPoint.Position.Around(10f), "M_Y_GALB_LO_02");
            passenger = new LPed(spawnPoint.Position.Around(11f), "M_Y_GALB_LO_02");

            // To avoid NullReferenceException
            if(passenger == null || suspect == null || car == null)
            {
                Functions.AddTextToTextwall("Unable to locate suspect through given location by caller.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                Functions.AddTextToTextwall("Looking out for alert calls.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                return false;
            }

            if(!passenger.Exists() || !suspect.Exists() || !car.Exists())
            {
                Functions.AddTextToTextwall("Caller only heard alarms, notifying nearby patrol officers.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                return false;
            }

            suspect.WarpIntoVehicle(car, VehicleSeat.Driver);
            passenger.WarpIntoVehicle(car, VehicleSeat.RightFront);
            suspect.Task.CruiseWithVehicle(car.GVehicle, 30f, true);
            car.BurstTire(VehicleWheel.FrontLeft);
            car.Door(VehicleDoor.LeftFront).Break();
            car.Door(VehicleDoor.RightFront).Break();
            Blip carBlip = car.AttachBlip();
            carBlip.Icon = BlipIcon.Misc_Waypoint;
            carBlip.RouteActive = true;
            carBlip.Name = "Stolen Vehicle";

            return true;
        }

        public override void Process()
        {
            base.Process();

            if (suspect.HasBeenArrested && passenger.HasBeenArrested)
            {
                Functions.PrintText("Suspect Apprehended!", 7000);
                Functions.AddTextToTextwall("Code 4 - Suspect in custody.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                base.SetCalloutFinished(true, true, true);
            }

            if (!suspect.IsAliveAndWell || !suspect.Exists() && !passenger.IsAliveAndWell || !passenger.Exists())
            {
                Functions.AddTextToTextwall("Code 4 - Suspect netrulized.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                base.SetCalloutFinished(true, true, true);
            }
        }
    }
}
