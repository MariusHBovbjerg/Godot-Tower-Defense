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
	private Control _upgradeMenu;
	private ScrollContainer _upgradeScrollContainer;
	private StatType _activeMenuType = StatType.Offensive;

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

	public void InitializeNewRound()
	{
		_playerStatView.Show();
		_cashLabel.Show();
		(_upgradeMenu, _upgradeScrollContainer) = UpgradeMenu.InitializeUpgradeMenu(_player, _activeMenuType,
		() =>
		{
			if (IsInstanceValid(_upgradeScrollContainer))
				_upgradeScrollContainer.QueueFree();

			if (_activeMenuType == StatType.Offensive)
			{
				_activeMenuType = StatType.None;
				return;
			}
			_activeMenuType = StatType.Offensive;
			_upgradeScrollContainer = UpgradeMenu.CreateUpgrades(StatType.Offensive, _upgradeMenu, _player);
		},
		() =>
		{
			if (IsInstanceValid(_upgradeScrollContainer))
				_upgradeScrollContainer.QueueFree();

			if (_activeMenuType == StatType.Defensive)
			{
				_activeMenuType = StatType.None;
				return;
			}
			_activeMenuType = StatType.Defensive;
			_upgradeScrollContainer = UpgradeMenu.CreateUpgrades(StatType.Defensive, _upgradeMenu, _player);
		},
		() =>
		{
			if (IsInstanceValid(_upgradeScrollContainer))
				_upgradeScrollContainer.QueueFree();

			if (_activeMenuType == StatType.Economic)
			{
				_activeMenuType = StatType.None;
				return;
			}
			_activeMenuType = StatType.Economic;
			_upgradeScrollContainer = UpgradeMenu.CreateUpgrades(StatType.Economic, _upgradeMenu, _player);
		});
		AddChild(_upgradeMenu);
		UpdateDetails();
	}

	public async void ShowGameOver()
	{
		_upgradeMenu.QueueFree();
		_playerStatView.Hide();
		ShowMessage("Game Over");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		GetTree().CallGroup("bullets", Node.MethodName.QueueFree);

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
