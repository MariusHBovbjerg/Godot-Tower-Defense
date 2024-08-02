using System;
using Godot;

public partial class EntityStats
{
	public Stat MaxHealth { get; set; } =
		new Stat
		{
			Type = StatType.Defensive,
			Name = "Max Health",
			StartingValue = 100,
			UpgradeMultiplier = 1.1f,
			StartingCost = 100,
			CostIncreaseValue = 1.1f,
			Level = 1,
			MaxLevel = 10,
			GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
			NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
		};

	public double CurrentHealth { get; set; }

	public Stat Damage { get; set; } =
		new Stat
		{
			Type = StatType.Offensive,
			Name = "Damage",
			StartingValue = 10,
			UpgradeMultiplier = 1.1f,
			StartingCost = 100,
			CostIncreaseValue = 1.1f,
			Level = 1,
			MaxLevel = 10,
			GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
			NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
		};
}

public partial class PlayerStats : EntityStats
{
	public double CashBalance { get; set; }
	public double CoinBalance { get; set; }
	public Stat CashMultiplier { get; set; } = new()
	{
		Type = StatType.Economic,
		Name = "CashMultiplier",
		StartingValue = 1,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat CoinMultiplier { get; set; } = new()
	{
		Type = StatType.Economic,
		Name = "CoinMultiplier",
		StartingValue = 1,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat FiringRate { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "FiringRate",
		StartingValue = 10,
		UpgradeMultiplier = 1.15f,
		StartingCost = 5,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat CriticalHitChance { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "CriticalHitChance",
		StartingValue = 0,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue + (level - 1) * upgradeMultiplier,
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat CriticalHitDamageFactor { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "CriticalHitDamageFactor",
		StartingValue = 1,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat Range { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "Range",
		StartingValue = 50,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1)),
		LevelUpCallback = (player) =>
		{
			var range = player.Stats.Range.GetValue();
			// The scale is about half the range, so we need to divide by 200
			player.RangeIndicator.Scale = new Vector2((float)range / 150, (float)range / 150);
		},
	};

	public Stat MultiShotChance { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "MultiShotChance",
		StartingValue = 0,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat MultiShotCount { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "MultiShotCount",
		StartingValue = 1,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 5,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => (int)(startingValue + (level - 1) * upgradeMultiplier),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost + (level - 1) * costIncreaseValue)
	};

	public Stat RapidFireChance { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "RapidFireChance",
		StartingValue = 0,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat RapidFireDuration { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "RapidFireDuration",
		StartingValue = 1,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat BounceChance { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "BounceChance",
		StartingValue = 0,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat BounceCount { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "BounceCount",
		StartingValue = 1,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat BounceRange { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "BounceRange",
		StartingValue = 1,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};

	public Stat ShotSpeed { get; set; } = new()
	{
		Type = StatType.Offensive,
		Name = "ShotSpeed",
		StartingValue = 100,
		UpgradeMultiplier = 1.025f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 50,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};
}

public partial class MobStats : EntityStats
{
	public Stat Speed { get; set; } = new Stat
	{
		Type = StatType.Offensive,
		Name = "Speed",
		StartingValue = 100,
		UpgradeMultiplier = 1.1f,
		StartingCost = 10,
		CostIncreaseValue = 1.1f,
		Level = 1,
		MaxLevel = 10,
		GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
		NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
	};
	public int CoinDropValue { get; set; }
	public int CashDropValue { get; set; }
}
