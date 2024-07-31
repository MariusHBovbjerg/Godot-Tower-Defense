using Godot;
using System;

public partial class Mob : RigidBody2D
{
	private Player _player;

	public MobStats Stats = new MobStats
	{
		Health = 100,
		Damage = 10,
		Speed = 100,
		CoinDropBaseValue = 10,
		CashDropBaseValue = 10
	};

	public bool CanDamage = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (_player != null)
		{
			// Calculate the direction vector from the mob to the player
			Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

			// Set the mob's velocity based on the direction vector
			state.LinearVelocity = direction * Stats.Speed;

			// Rotate the mob to face the player
			state.AngularVelocity = 0; // Stop any existing rotation
			Rotation = direction.Angle();
		}
	}

	public double DoDamage(double health)
	{
		CanDamage = false;
		GetNode<Timer>("DamageCooldown").Start();
		return health - Stats.Damage;
	}

	public void TakeDamage(double damage)
	{
		Stats.Health -= damage;
		if (Stats.Health <= 0)
		{
			// Add some cash and coins to the player's balance
			_player.Stats.CashBalance += Stats.CashDropBaseValue;
			_player.Stats.CoinBalance += Stats.CoinDropBaseValue;

			QueueFree();
		}
	}

	private void OnDamageCooldownTimeout()
	{
		CanDamage = true;
	}
}
