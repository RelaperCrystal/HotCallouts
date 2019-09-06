/*
 * 由SharpDevelop创建。
 * 用户： RelaperCrystal
 * 日期: 2019/9/6
 * 时间: 13:46
 */
using System;
using GTA;
using LCPD_First_Response.Engine;
using LCPD_First_Response.LCPDFR.API;

namespace HotCallouts.WorldEvents
{
	/// <summary>
	/// A <see cref="WorldEvent" /> includes content and logic of <see cref="Guy" /> mugging <see cref="Victim" />.
	/// </summary>
	public class HMugging : WorldEvent
	{
		/// <summary>
		/// The Guy who mugging the <see cref="Victim" />.
		/// </summary>
		private LPed Guy;
		
		/// <summary>
		/// The Victim who mugged by <see cref="Guy" />.
		/// </summary>
		private LPed Victim;
		
		/// <summary>
		/// The Pursuit handle.
		/// </summary>
		private LHandle pursuit;
		
		/// <summary>
		/// Defines has the <see cref="Guy" /> or <see cref="Victim" /> is spotted by player.
		/// </summary>
		private bool SpottedByPlayer;
		
		public HMugging() : base("HMugging")
		{
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
                            if (this.Guy == null)
                            {
                                this.Guy = tempPed;
                            }
                            else
                            {
                                this.Victim = tempPed;
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

            if (!this.SpottedByPlayer)
            {
                if (this.Guy.Exists())
                {
                    this.SpottedByPlayer = this.Guy.MakeCameraFocus(3500);
                }
            }

            // Check if both guys have ceased to exist, are dead or have been arrested. If it's true for at least one, end this world event.
            int guysDisposed = 0;
            if (!this.Guy.Exists() || !this.Guy.IsAliveAndWell || this.Guy.HasBeenArrested)
            {
                guysDisposed++;
            }

            if (!this.Victim.Exists() || !this.Victim.IsAliveAndWell || this.Victim.HasBeenArrested)
            {
                guysDisposed++;
            }

            if (guysDisposed > 0)
            {
                this.End();
            }
        }
		
		public override bool CanBeDisposedNow()
        {
            return LPlayer.LocalPlayer.Ped.Position.DistanceTo(this.Guy.Position) > 120;
        }
		
		public override void Initialize()
        {
            base.Initialize();

            // We want to keep exclusive ownership of the peds, so no other script can use them or consider them for its actions. 
            // Note that actions with high priority (such as player arresting a ped) can bypass this exclusive ownership and will call "PedLeftScript"
            // before transferring ownership. Make sure to clean up everything there.
            Functions.SetPedIsOwnedByScript(this.Guy, this, true);
            Functions.SetPedIsOwnedByScript(this.Victim, this, true);

            this.Guy.BlockPermanentEvents = true;
            this.Victim.BlockPermanentEvents = true;
            this.Guy.Task.AlwaysKeepTask = true;
            this.Victim.Task.AlwaysKeepTask = true;
            this.Guy.AttachBlip();
            
            this.Guy.DefaultWeapon = Weapon.Handgun_Glock;
            this.Guy.EquipWeapon();

            this.Guy.Task.AimAt(this.Victim, -1);
            this.Victim.Task.HandsUp(-1);
            
            // Pursuit Handle
            // Yes
            pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(pursuit, Guy);
            Functions.SetPursuitDontEnableCopBlips(pursuit, true);
            Functions.SetPursuitHasBeenCalledIn(pursuit, false);
            Functions.SetPursuitCopsCanJoin(pursuit, false);
            Functions.SetPursuitIsActiveForPlayer(pursuit, false);
        }
		 
		public override void PedLeftScript(LPed ped)
        {
            base.PedLeftScript(ped);

            if (ped == this.Guy)
            {
                this.Guy.Task.ClearAll();
            }

            if (ped == this.Victim)
            {
                this.Victim.Task.ClearAll();
            }
        }
		
		/// <summary>
        /// Called when a world event should be disposed. This is also called when <see cref="WorldEvent.CanBeDisposedNow"/> returns false.
        /// </summary>
        public override void End()
        {
            base.End();

            // If we are still the owner of the ped, so it hasn't been arrested, remove blip.
            if (Functions.IsStillControlledByScript(this.Guy, this))
            {
                if (this.Guy.Exists())
                {
                    this.Guy.DeleteBlip();
                }
            }

            if (Functions.IsStillControlledByScript(this.Victim, this))
            {
                if (this.Victim.Exists())
                {
                    this.Victim.DeleteBlip();
                }
            }

            // Automatically releases the peds and safe to call even though we might not own them anymore.
            Functions.SetPedIsOwnedByScript(this.Guy, this, false);
            Functions.SetPedIsOwnedByScript(this.Victim, this, false);
        }

	}
}
