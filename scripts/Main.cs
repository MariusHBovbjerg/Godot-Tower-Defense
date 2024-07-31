using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	private HUD _hud;
	private Player _player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hud = GetNode<HUD>("HUD");
		_player = GetNode<Player>("Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_hud.UpdateDetails(_player);
	}

	public void GameOver()
	{
		_hud.ShowGameOver();
		GetNode<Timer>("MobTimer").Stop();
	}

	public void NewGame()
	{
		_player.Start();

		GetNode<Timer>("StartTimer").Start();
	}

	private void OnMobTimerTimeout()
	{
		// Create a new instance of the Mob scene.
		Mob mob = MobScene.Instantiate<Mob>();

		// Choose a random location on Path2D.
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		// Set the mob's direction perpendicular to the path direction.
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		// Set the mob's position to a random location.
		mob.Position = mobSpawnLocation.Position;

		// Add some randomness to the direction.
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		// Choose the velocity.
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}

	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
	}

	private void OnStartGame()
	{
		NewGame();
	}
}
