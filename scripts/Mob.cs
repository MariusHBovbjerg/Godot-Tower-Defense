using Godot;
using System;

public partial class Mob : RigidBody2D
{
	public static readonly string GROUP_NAME = "mobs";
	private Player _player;

	public MobStats Stats = new()
	{
		CoinDropValue = 10,
		CashDropValue = 10
	};

	public bool CanDamage = true;

	public Sprite2D Sprite;

	public Timer DamageCooldown;

	public Mob(Sprite2D sprite, CollisionShape2D collisionShape, Timer damageCooldown, Vector2 position)
	{
		Position = position;
		AddToGroup(GROUP_NAME);
		Sprite = sprite;
		DamageCooldown = damageCooldown;
		damageCooldown.Timeout += OnDamageCooldownTimeout;
		AddChild(Sprite);
		AddChild(collisionShape);
		AddChild(damageCooldown);

		ContinuousCd = CcdMode.CastRay;
		ContactMonitor = true;
		MaxContactsReported = 100;
		GravityScale = 0;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
		Stats.CurrentHealth = Stats.MaxHealth.GetValue();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (_player != null && IsInstanceValid(_player))
		{
			// Calculate the direction vector from the mob to the player
			Vector2 direction = (_player.GlobalPosition - Position).Normalized();

			// Set the mob's velocity based on the direction vector
			state.LinearVelocity = direction * (int)Stats.Speed.GetValue();

			// Rotate the mob to face the player
			state.AngularVelocity = 0; // Stop any existing rotation
			Rotation = direction.Angle();
		}
	}

	public double DoDamage(double health)
	{
		CanDamage = false;
		DamageCooldown.Start();
		return health - Stats.Damage.GetValue();
	}

	public void TakeDamage(double damage)
	{
		Stats.CurrentHealth -= damage;
		if (Stats.CurrentHealth <= 0)
		{
			// Add some cash and coins to the player's balance
			_player.Stats.CashBalance += (int)(Stats.CashDropValue * _player.Stats.CashMultiplier.GetValue());
			_player.Stats.CoinBalance += (int)(Stats.CoinDropValue * _player.Stats.CoinMultiplier.GetValue());

			QueueFree();
		}
	}

	private void OnDamageCooldownTimeout()
	{
		CanDamage = true;
	}
}
