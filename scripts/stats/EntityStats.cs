using System;
using Godot;

public partial class EntityStats
{
	public double Health { get; set; }
	public double Damage { get; set; }
}

public partial class PlayerStats : EntityStats
{
	public bool isDead { get; set; }
	public double CashBalance { get; set; }
	public double CoinBalance { get; set; }
	public double CashMultiplier { get; set; }
	public double CoinMultiplier { get; set; }
	public double FiringRate { get; set; }
	public int CriticalHitChance { get; set; }
	public int CriticalHitDamageFactor { get; set; }
	public int Range { get; set; }
	public int MultiShotChance { get; set; }
	public int MultiShotCount { get; set; }
	public int RapidFireChance { get; set; }
	public int RapidFireDuration { get; set; }
	public int BounceChance { get; set; }
	public int BounceCount { get; set; }
	public int BounceRange { get; set; }
	public int ShotSpeed { get; set; }
}

public partial class MobStats : EntityStats
{
	public int Speed { get; set; }
	public int CoinDropBaseValue { get; set; }
	public int CashDropBaseValue { get; set; }
}
