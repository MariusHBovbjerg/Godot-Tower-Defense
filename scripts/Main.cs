using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	private HUD _hud;
	private Player _player;
	private EntitySpawner _entitySpawner;
	private Texture2D _mobTexture = GD.Load<Texture2D>("res://art/basic.png");
	private RandomRadialSpawnStrategy _randomRadialSpawnStrategy;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hud = GetNode<HUD>("HUD");
		_player = GetNode<Player>("Player");
		_entitySpawner = new EntitySpawner(OnEntityReadyHandler);
		_randomRadialSpawnStrategy = new RandomRadialSpawnStrategy(_player.Position, 500);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_hud.UpdateDetails();
	}

	public void OnEntityReadyHandler(Node2D entity)
	{
		AddChild(entity);
	}

	public void InitializeNewRound()
	{
		_player.InitializeNewRound();
		_hud.InitializeNewRound();
		GetNode<Timer>("StartTimer").Start();
	}

	public void StopRound()
	{
		_hud.ShowGameOver();

		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		GetTree().CallGroup("bullets", Node.MethodName.QueueFree);

		GetNode<Timer>("MobTimer").Stop();
	}

	private void OnMobTimerTimeout()
	{
		_entitySpawner.spawnEntityUsingStrategy(
			new MobBuilder(),
			_mobTexture,
			new Vector2(0.3f, 0.3f),
			_randomRadialSpawnStrategy);
	}

	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MobTimer").Start();
	}

	private void OnStartGame()
	{
		InitializeNewRound();
	}
}
