using Godot;
using System;

public partial class HUD : CanvasLayer
{
	private Label _message;
	private Label _cashLabel;
	private Label _coinLabel;
	private Label _healthLabel;
	private Label _damageLabel;
	private Control _playerStatView;
	private Player _player;
	private ScrollContainer _upgradeContainer;
	private HBoxContainer _upgradeHBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
		_message = GetNode<Label>("Message");
		_playerStatView = GetNode<Control>("PlayerStatView");
		_cashLabel = GetNode<Label>("Cash");
		_coinLabel = GetNode<Label>("Coin");
		_healthLabel = _playerStatView.GetNode<Label>("Health");
		_damageLabel = _playerStatView.GetNode<Label>("Damage");
		_playerStatView.Hide();
		_cashLabel.Hide();

		_upgradeContainer = GetNode<ScrollContainer>("UpgradeMenu");
		_upgradeContainer.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	[Signal]
	public delegate void StartGameEventHandler();

	public void ShowMessage(string text)
	{
		_message.Text = text;
		_message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}

	public async void ShowGameOver()
	{
		_upgradeHBox.QueueFree();
		_playerStatView.Hide();
		_upgradeContainer.Hide();
		ShowMessage("Game Over");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

		_message.Text = "Dodge the Creeps!";
		_message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();
	}

	public void UpdateDetails()
	{
		_healthLabel.Text = $"{_player.Stats.CurrentHealth}/{_player.Stats.MaxHealth.GetValueAsFormattedString()}";
		_damageLabel.Text = _player.Stats.Damage.GetValueAsFormattedString();
		_cashLabel.Text = $"${_player.Stats.CashBalance}";
		_coinLabel.Text = $"{_player.Stats.CoinBalance} Coins";
	}

	public void InitializeNewRound()
	{
		_playerStatView.Show();
		_cashLabel.Show();

		_upgradeHBox = new HBoxContainer();
		var upgradeColumn1 = new VBoxContainer();
		var upgradeColumn2 = new VBoxContainer();
		var upgradeColumn3 = new VBoxContainer();

		_upgradeHBox.AddChild(upgradeColumn1);
		_upgradeHBox.AddChild(upgradeColumn2);
		_upgradeHBox.AddChild(upgradeColumn3);

		_upgradeContainer.AddChild(_upgradeHBox);

		var playerStats = _player.Stats;

		var statPropertiesInPlayerStats = playerStats.GetType().GetProperties();

		var statCount = statPropertiesInPlayerStats.Length;
		var count = 0;

		foreach (var stat in statPropertiesInPlayerStats)
		{
			if (stat.PropertyType == typeof(Stat))
			{
				var entity = (Stat)stat.GetValue(playerStats);

				var upgradeContainer = (count % 3) switch
				{
					0 => upgradeColumn1,
					1 => upgradeColumn2,
					2 => upgradeColumn3,
					_ => throw new Exception("This should never happen")
				};

				Control upgradeComposite = new Control
				{
					CustomMinimumSize = new Vector2(234, 50),
				};

				Label nameLabel = new Label
				{
					Text = entity.Name,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					// Name sits in the top left corner of the upgrade container
					OffsetTop = 0,
					OffsetLeft = 0,
				};

				Label priceLabel = new Label
				{
					Text = $"${entity.NextLevelCost()}",
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					// price is underneath the name
					OffsetTop = 15,
					OffsetLeft = 0,
				};

				Label currentLevelLabel = new Label
				{
					Text = $"Level: {entity.Level}",
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					// level is underneath the price
					OffsetTop = 30,
					OffsetLeft = 0,
				};

				Button upgradeButton = new Button
				{
					// Only show two decimal places
					Text = $"{entity.GetValueAsFormattedString()} -> {entity.GetNextLevelvalueAsFormattedString()}",
					CustomMinimumSize = new Vector2(100, 50),
					// Button sits to the right of the name, price and level
					OffsetTop = 0,
					OffsetLeft = 100,
				};

				upgradeButton.Pressed += () =>
				{
					entity.LevelUp(_player);
					var isMaxLevel = entity.Level == entity.MaxLevel;
					currentLevelLabel.Text = isMaxLevel ? "Max level" : $"Level: {entity.Level}";
					priceLabel.Text = isMaxLevel ? "Max" : $"${entity.NextLevelCost()}";
					upgradeButton.Text = $"{entity.GetValueAsFormattedString()} -> {entity.GetNextLevelvalueAsFormattedString()}";

				};

				// Add the label and button to the upgrade container
				upgradeComposite.AddChild(nameLabel);
				upgradeComposite.AddChild(upgradeButton);
				upgradeComposite.AddChild(priceLabel);
				upgradeComposite.AddChild(currentLevelLabel);

				upgradeContainer.AddChild(upgradeComposite);

				count++;
			}
		}
		_upgradeContainer.Show();
	}

	private void OnStartButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		_message.Hide();
		EmitSignal(SignalName.StartGame);
	}

	private void OnMessageTimerTimeout()
	{
		_message.Hide();
	}
}
