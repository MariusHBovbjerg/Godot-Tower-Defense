using System;
using System.Linq;
using Godot;

public static class UpgradeMenu
{
	public static (Control, ScrollContainer) InitializeUpgradeMenu(Player player, StatType activeMenuType, Action OffensiveButtonPressed, Action DefenseButtonPressed, Action EconomicButtonPressed)
	{
		var upgradeMenu = new Control
		{
			Size = new Vector2(720, 380),
			Position = new Vector2(0, 700),
		};

		var offensiveTypeButton = CreateButton("Offensive", new Vector2(0, 320), () => OffensiveButtonPressed());
		var defensiveTypeButton = CreateButton("Defensive", new Vector2(240, 320), () => DefenseButtonPressed());
		var economicTypeButton = CreateButton("Economic", new Vector2(480, 320), () => EconomicButtonPressed());

		upgradeMenu.AddChild(offensiveTypeButton);
		upgradeMenu.AddChild(defensiveTypeButton);
		upgradeMenu.AddChild(economicTypeButton);

		// Initialize the upgrade menu with the offensive stats
		var scrollContainer = CreateUpgrades(activeMenuType, upgradeMenu, player);

		return (upgradeMenu, scrollContainer);
	}

	private static Button CreateButton(string text, Vector2 position, Action onPressed)
	{
		var button = new Button
		{
			Text = text,
			Size = new Vector2(240, 60),
			Position = position
		};
		button.Pressed += onPressed;
		return button;
	}

	public static ScrollContainer CreateUpgrades(StatType statType, Control upgradeMenu, Player player)
	{
		var upgradeScrollContainer = new ScrollContainer
		{
			Size = new Vector2(720, 320),
		};

		upgradeMenu.AddChild(upgradeScrollContainer);

		var upgradeHBox = new HBoxContainer();
		upgradeScrollContainer.AddChild(upgradeHBox);

		var upgradeColumn1 = new VBoxContainer();
		var upgradeColumn2 = new VBoxContainer();
		var upgradeColumn3 = new VBoxContainer();

		upgradeHBox.AddChild(upgradeColumn1);
		upgradeHBox.AddChild(upgradeColumn2);
		upgradeHBox.AddChild(upgradeColumn3);

		var playerStats = player.Stats;

		var statPropertiesInPlayerStats = playerStats.GetType().GetProperties();

		var count = 0;

		foreach (var stat in statPropertiesInPlayerStats)
		{
			if (stat.PropertyType == typeof(Stat))
			{
				var entity = (Stat)stat.GetValue(playerStats);

				if (entity.Type != statType)
					continue;

				ProcessStatProperty(entity, count, player, upgradeColumn1, upgradeColumn2, upgradeColumn3);
				count++;
			}
		}

		return upgradeScrollContainer;
	}

	private static void ProcessStatProperty(Stat entity, int count, Player player, VBoxContainer upgradeColumn1, VBoxContainer upgradeColumn2, VBoxContainer upgradeColumn3)
	{
		var upgradeContainer = (count % 3) switch
		{
			0 => upgradeColumn1,
			1 => upgradeColumn2,
			2 => upgradeColumn3,
			_ => throw new Exception("This should never happen")
		};

		Control upgradeComposite = CreateUpgradeComposite();

		Label nameLabel = CreateLabel(entity.Name, 0);
		Label priceLabel = CreateLabel($"${entity.NextLevelCost()}", 15);
		Label currentLevelLabel = CreateLabel($"Level: {entity.Level}", 30);

		Button upgradeButton = CreateUpgradeButton(entity);
		ConfigureUpgradeButton(upgradeButton, entity, player, currentLevelLabel, priceLabel);

		AddUpgradeComponents(upgradeComposite, nameLabel, upgradeButton, priceLabel, currentLevelLabel);

		upgradeContainer.AddChild(upgradeComposite);
	}

	private static Control CreateUpgradeComposite()
	{
		return new Control
		{
			CustomMinimumSize = new Vector2(234, 50),
			OffsetTop = 10,
		};
	}

	private static Label CreateLabel(string text, int offsetTop, int offsetLeft = 0)
	{
		return new Label
		{
			Text = text,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			OffsetTop = offsetTop,
			OffsetLeft = offsetLeft,
		};
	}

	private static Button CreateUpgradeButton(Stat entity)
	{
		return new Button
		{
			Text = $"{entity.GetValueAsFormattedString()} -> {entity.GetNextLevelvalueAsFormattedString()}",
			CustomMinimumSize = new Vector2(100, 50),
			OffsetTop = 0,
			OffsetLeft = 100,
		};
	}

	private static void ConfigureUpgradeButton(Button upgradeButton, Stat entity, Player player, Label currentLevelLabel, Label priceLabel)
	{
		upgradeButton.Pressed += () =>
		{
			entity.LevelUp(player);
			var isMaxLevel = entity.Level == entity.MaxLevel;

			currentLevelLabel.Text = $"Level: {entity.Level}";
			priceLabel.Text = isMaxLevel ? "Max" : $"${entity.NextLevelCost()}";
			upgradeButton.Text = isMaxLevel ? $"{entity.GetValueAsFormattedString()}" : $"{entity.GetValueAsFormattedString()} -> {entity.GetNextLevelvalueAsFormattedString()}";
		};
	}

	private static void AddUpgradeComponents(Control upgradeComposite, Label nameLabel, Button upgradeButton, Label priceLabel, Label currentLevelLabel)
	{
		upgradeComposite.AddChild(nameLabel);
		upgradeComposite.AddChild(upgradeButton);
		upgradeComposite.AddChild(priceLabel);
		upgradeComposite.AddChild(currentLevelLabel);
	}
}
