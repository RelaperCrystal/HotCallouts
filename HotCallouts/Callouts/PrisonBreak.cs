using GTA;
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
    CalloutInfo("PrisonBreak", ECalloutProbability.High)]
    public class PrisonBreak : Callout
    {
        LVehicle car;
        LPed suspect;
        LPed passenger;
        LPed prisoner;
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
            this.CalloutMessage = "Report of a prisoner transport truck has been hijacked in " + area + ", available unit please respond.";

            string audioMessage = Functions.CreateRandomAudioIntroString(EIntroReportedBy.Officers);
            string crimeMessage = "CRIM_PRISON_BREAK";
            if (Common.GetRandomBool(0, 2, 1))
            {
                crimeMessage = "CRIM_A_CRIMINAL_RESISTING_ARREST";
            }

            Functions.PlaySoundUsingPosition(audioMessage + crimeMessage + " IN_OR_ON_POSITION", this.spawnPoint.Position);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            base.OnCalloutAccepted();

            car = new LVehicle(World.GetNextPositionOnStreet(spawnPoint.Position), "PSTOCKADE");
            car.IsRequiredForMission = true;

            suspect = new LPed(spawnPoint.Position.Around(10f), "M_Y_GALB_LO_02");
            passenger = new LPed(spawnPoint.Position.Around(11f), "M_Y_GALB_LO_02");
            prisoner = new LPed(spawnPoint.Position.Around(11.5f), "M_Y_PRISON");

            // To avoid NullReferenceException
            if(passenger == null || suspect == null || car == null)
            {
                Functions.AddTextToTextwall("The suspect has escaped.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                Functions.AddTextToTextwall("Notifying DOC.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                return false;
            }

            if(!passenger.Exists() || !suspect.Exists() || !car.Exists())
            {
                Functions.AddTextToTextwall("The real-time GPS has been disabled. Notifying DOC.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                return false;
            }

            suspect.WarpIntoVehicle(car, VehicleSeat.Driver);
            passenger.WarpIntoVehicle(car, VehicleSeat.RightFront);
            prisoner.WarpIntoVehicle(car, VehicleSeat.RightRear);
            suspect.Task.CruiseWithVehicle(car.GVehicle, 30f, false);
            suspect.EquipWeapon();
            passenger.EquipWeapon();
            suspect.WillDoDrivebys = true;
            passenger.WillDoDrivebys = true;
            prisoner.CanResistArrest = false;
            prisoner.AlwaysSurrender = true;
            car.DisablePullover = true;
            Blip carBlip = car.AttachBlip();
            carBlip.Icon = BlipIcon.Misc_Waypoint;
            carBlip.RouteActive = true;
            carBlip.Name = "Hijacked Transport Truck";
            Functions.PrintHelp("When you sighted the truck, order the driver out of the truck. The prisoner will not resist.");

            return true;
        }

        public override void Process()
        {
            base.Process();

            if (suspect.HasBeenArrested && passenger.HasBeenArrested && prisoner.HasBeenArrested)
            {
                Functions.PrintText("Suspect Apprehended!", 7000);
                Functions.AddTextToTextwall("Code 4 - Suspect in custody.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                base.SetCalloutFinished(true, true, true);
            }

            if (!suspect.IsAliveAndWell || !suspect.Exists() && !passenger.IsAliveAndWell || !passenger.Exists() && !prisoner.IsAliveAndWell || !prisoner.Exists())
            {
                Functions.AddTextToTextwall("Code 4 - Suspect netrulized.", Functions.GetStringFromLanguageFile("POLICE_SCANNER_CONTROL"));
                base.SetCalloutFinished(true, true, true);
            }
        }
    }
}
