using Godot;
using System;

public partial class Mob : RigidBody2D
{
	private Player _player;

	public MobStats Stats = new()
	{
		CoinDropValue = 10,
		CashDropValue = 10
	};

	public bool CanDamage = true;

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
			Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

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
		GetNode<Timer>("DamageCooldown").Start();
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

			Free();
		}
	}

	private void OnDamageCooldownTimeout()
	{
		CanDamage = true;
	}
}
