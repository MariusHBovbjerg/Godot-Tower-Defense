using System;
using Godot;

public enum StatType
{
	Offensive,
	Defensive,
	Economic,
	None
}

public class Stat
{
	public StatType Type;
	public string Name;
	public int StartingValue;
	public float UpgradeMultiplier;
	public float StartingCost;
	public float CostIncreaseValue;
	public int Level;
	public int MaxLevel;
	public Func<int, int, float, double> GetValueCallback;
	public Func<int, float, float, double> NextLevelCostCallback;
	// add a callback for leveling up, as some level ups may have side effects
	public Action<Player>? LevelUpCallback = null;
	public double GetValue()
	{
		return GetValueCallback(Level, StartingValue, UpgradeMultiplier);
	}

	public string GetValueAsFormattedString()
	{
		return FormatThousands(GetValue());
	}

	public double NextLevelvalue()
	{
		return GetValueCallback(Level + 1, StartingValue, UpgradeMultiplier);
	}

	public string GetNextLevelvalueAsFormattedString()
	{
		return FormatThousands(NextLevelvalue());
	}

	// Calculate the current cost to upgrade
	public double NextLevelCost()
	{
		return Math.Floor(NextLevelCostCallback(Level, StartingCost, CostIncreaseValue));
	}

	// Level up the stat
	public void LevelUp(Player player)
	{
		if (Level >= MaxLevel)
			return;

		if (player.Stats.CashBalance >= NextLevelCost())
		{
			player.Stats.CashBalance -= NextLevelCost();
			Level++;
			if (LevelUpCallback != null)
				LevelUpCallback(player);
		}
	}

	private string FormatThousands(double num)
	{
		// Then, if the value is thousands, divide it by 1000 and append a K
		// Do the same for millions, billions, etc.
		if (num >= 1000)
		{
			var suffixes = new string[] { "K", "M", "B", "T" };
			var suffixIndex = 0;
			num /= 1000;
			while (num >= 1000)
			{
				num /= 1000;
				suffixIndex++;
			}
			return $"{Math.Round(num, 2)} {suffixes[suffixIndex]}";
		}
		return Math.Round(num, 2).ToString();
	}
}
