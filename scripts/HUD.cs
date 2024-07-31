using Godot;
using System;

public partial class HUD : CanvasLayer
{
	private Label _message;
	private Label _cashLabel;
	private Label _coinLabel;
	private Label _healthLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_message = GetNode<Label>("Message");
		_cashLabel = GetNode<Label>("Cash");
		_coinLabel = GetNode<Label>("Coin");
		_healthLabel = GetNode<Label>("Health");
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
		ShowMessage("Game Over");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

		_message.Text = "Dodge the Creeps!";
		_message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();
	}

	public void UpdateDetails(Player player)
	{
		_healthLabel.Text = player.Stats.Health.ToString();
		_cashLabel.Text = player.Stats.CashBalance.ToString();
		_coinLabel.Text = player.Stats.CoinBalance.ToString();
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
