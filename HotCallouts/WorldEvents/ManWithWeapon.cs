using GTA;
using LCPD_First_Response.Engine;
using LCPD_First_Response.LCPDFR.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotCallouts.WorldEvents
{
    public class ManWithWeapon : WorldEvent
    {
        private LPed guy;
        private bool spotted;

        public ManWithWeapon() : base("ManWithWeapon")
        {
            Log.Debug("Man with weapon created", "HotCallouts");
        }

        public override bool CanStart(Vector3 position)
        {
            // Select the ped
            foreach (Ped ped in World.GetAllPeds())
            {
                if (ped != null && ped.Exists())
                {
                    if (ped.Position.DistanceTo(position) < 30f)
                    {
                        LPed tempPed = LPed.FromGTAPed(ped);
                        if (!Functions.DoesPedHaveAnOwner(tempPed) && !tempPed.IsPlayer && tempPed.IsAliveAndWell && !tempPed.IsOnStreet
                            && !ped.isInVehicle())
                        {
                            if (this.guy == null)
                            {
                                this.guy = tempPed;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        
        public override void Process()
        {
            base.Process();

            if (!this.spotted)
            {
                if (this.guy.Exists())
                {
                    this.spotted = this.guy.MakeCameraFocus(3500);
                }
            }
        }

        public override void Initialize()
        {
            try
            {
                base.Initialize();

                // We want to keep exclusive ownership of the peds, so no other script can use them or consider them for its actions. 
                // Note that actions with high priority (such as player arresting a ped) can bypass this exclusive ownership and will call "PedLeftScript"
                // before transferring ownership. Make sure to clean up everything there.
                Functions.SetPedIsOwnedByScript(this.guy, this, true);

                this.guy.BlockPermanentEvents = true;
                this.guy.Task.AlwaysKeepTask = true;
                this.guy.AttachBlip();

                this.guy.DefaultWeapon = Weapon.Handgun_Glock;
                this.guy.EquipWeapon();
            }
            catch (Exception ex)
            {
                Log.Error("A exception occoured when starting", "HotCallouts] [ManWithWeapon");
                Log.Error(ex.ToString(), "HotCallouts] [ManWithWeapon");
            }
        }

        public override void PedLeftScript(LPed ped)
        {
            Log.Info("PedLeftScript received", "HotCallouts] [hMugging");
            if (ped == this.guy)
            {
                this.guy.Task.ClearAll();
                Functions.SetPedIsOwnedByScript(ped, this, false);
            }
            else
            {
                Log.Warning("A ped that should not be owned by Man With a Weapon World Event is reported LeftScript.", "HotCallouts");
                Log.Warning("The mod will still try to make a left script, but the mod may crash.", "HotCallouts");
                Functions.SetPedIsOwnedByScript(ped, this, false);
            }
        }

        public override bool CanBeDisposedNow()
        {
            return LPlayer.LocalPlayer.Ped.Position.DistanceTo(this.guy.Position) > 120;
        }
    }
}
