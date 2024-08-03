using System;
using Godot;

public partial class CombatantStats
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