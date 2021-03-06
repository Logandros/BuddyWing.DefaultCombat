﻿// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Focus : RotationBase
	{
		public override string Name
		{
			get { return "Guardian Focus"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Force Might")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Resolute", ret => Me.IsStunned),
					Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 90),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
					Spell.Buff("Focused Defense", ret => Me.HealthPercent < 70),
					Spell.Buff("Enure", ret => Me.HealthPercent <= 30),
					Spell.Buff("Combat Focus", ret => Me.ActionPoints <= 6 || Me.BuffCount("Singularity") < 3),
					Spell.Cast("Unity", ret => Me.HealthPercent <= 15),
					Spell.Cast("Sacrifice", ret => Me.HealthPercent <= 5)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Saber Throw", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Force Leap", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

					//Interrupts
					Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Awe", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Force Stasis", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),

					//Rotation
					Spell.Cast("Focused Burst", ret => Me.HasBuff("Felling Blow") && Me.BuffCount("Singularity") == 3),
					Spell.Cast("Zealous Leap", ret => !Me.HasBuff("Felling Blow")),
					Spell.Cast("Force Exhaustion", ret => Me.BuffCount("Singularity") < 3),
					Spell.Cast("Blade Storm", ret => Me.HasBuff("Momentum")),
					Spell.Cast("Concentrated Slice"),
					Spell.Cast("Blade Barrage"),
					Spell.Cast("Riposte"),
					Spell.Cast("Sundering Strike", ret => Me.ActionPoints < 7),
					Spell.Cast("Strike"),
					Spell.Cast("Saber Throw", ret => Me.CurrentTarget.Distance >= 0.5f && Me.InCombat),

					//HK-55 Mode Rotation
					Spell.Cast("Charging In", ret => Me.CurrentTarget.Distance >= .4f && Me.InCombat && CombatHotkeys.EnableHK55),
					Spell.Cast("Blindside", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Assassinate", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Rail Blast", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Rifle Blast", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Execute", ret => Me.CurrentTarget.HealthPercent <= 45 && CombatHotkeys.EnableHK55)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
						Spell.Cast("Force Sweep", ret => Me.HasBuff("Felling Blow") && Me.BuffCount("Singularity") == 3),
						Spell.Cast("Cyclone Smash")
						));
			}
		}
	}
}