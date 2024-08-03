
using System;

public partial class MobStats : CombatantStats
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
