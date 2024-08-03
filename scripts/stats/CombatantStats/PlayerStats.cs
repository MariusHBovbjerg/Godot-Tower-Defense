using System;
using Godot;

public partial class PlayerStats : CombatantStats
{
    public double CashBalance { get; set; }
    public double CoinBalance { get; set; }
    public Stat CashMultiplier { get; set; } = new()
    {
        Type = StatType.Economic,
        Name = "Cash Multiplier",
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
        Name = "Coin Multiplier",
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
        Name = "Firing Rate",
        StartingValue = 1,
        UpgradeMultiplier = 1.15f,
        StartingCost = 5,
        CostIncreaseValue = 1.1f,
        Level = 1,
        MaxLevel = 10,
        GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
        NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1)),
        LevelUpCallback = (player) =>
        {
            player.ShootingTimer.WaitTime = 1 / player.Stats.FiringRate.GetValue();
        }
    };

    public Stat CriticalHitChance { get; set; } = new()
    {
        Type = StatType.Offensive,
        Name = "Critical Hit %",
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
        Name = "Critical Factor",
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
        StartingValue = 100,
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
            player.RangeIndicator.Scale = new Vector2((float)range / 150, (float)range / 150);
        },
    };

    public Stat MultiShotChance { get; set; } = new()
    {
        Type = StatType.Offensive,
        Name = "Multi Shot %",
        StartingValue = 0,
        UpgradeMultiplier = 3,
        StartingCost = 10,
        CostIncreaseValue = 1.1f,
        Level = 0,
        MaxLevel = 10,
        GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue + level * upgradeMultiplier,
        NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
    };

    public Stat MultiShotCount { get; set; } = new()
    {
        Type = StatType.Offensive,
        Name = "Multi Shot Count",
        StartingValue = 0,
        UpgradeMultiplier = 1,
        StartingCost = 10,
        CostIncreaseValue = 1.1f,
        Level = 0,
        MaxLevel = 5,
        GetValueCallback = (level, startingValue, upgradeMultiplier) => (int)(startingValue + level * upgradeMultiplier),
        NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost + (level - 1) * costIncreaseValue)
    };

    public Stat RapidFireChance { get; set; } = new()
    {
        Type = StatType.Offensive,
        Name = "Rapid Fire %",
        StartingValue = 0,
        UpgradeMultiplier = 5.0f,
        StartingCost = 10,
        CostIncreaseValue = 1.1f,
        Level = 0,
        MaxLevel = 10,
        GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue + level * upgradeMultiplier,
        NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
    };

    public Stat RapidFireDuration { get; set; } = new()
    {
        Type = StatType.Offensive,
        Name = "Rapid Fire Duration",
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
        Name = "Bounce %",
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
        Name = "Bounce Count",
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
        Name = "Bounce Range",
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
        Name = "Shot Speed",
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