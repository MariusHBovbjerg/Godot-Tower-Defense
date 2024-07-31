using Godot;
using System;

public partial class Player : RigidBody2D
{
	public PlayerStats Stats = new PlayerStats
	{
		Health = 100,
		Damage = 100,
		CashBalance = 0,
		CoinBalance = 0,
		CashMultiplier = 1,
		CoinMultiplier = 1,
		FiringRate = 1,
		CriticalHitChance = 0,
		CriticalHitDamageFactor = 2,
		Range = 100,
		MultiShotChance = 0,
		MultiShotCount = 2,
		RapidFireChance = 0,
		RapidFireDuration = 2,
		BounceChance = 0,
		BounceCount = 2,
		BounceRange = 100,
		ShotSpeed = 500
	};

	[Export]
	public PackedScene BulletScene { get; set; }

	[Signal]
	public delegate void DeathEventHandler();

	private Timer _shootingTimer;
	private bool _canShoot = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		Stats.isDead = true;

		_shootingTimer = GetNode<Timer>("ShootingTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Stats.Health <= 0 && !Stats.isDead)
		{
			Stats.CashBalance = 0;
			Stats.isDead = true;
			Hide(); // Player disappears when it dies.
			EmitSignal(SignalName.Death);
			_canShoot = false;
			// Must be deferred as we can't change physics properties on a physics callback.
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}

		if (_shootingTimer.WaitTime != Stats.FiringRate)
			_shootingTimer.WaitTime = Stats.FiringRate;
	}

	private void OnBodyEntered(Mob body)
	{
		if (body.CanDamage)
			Stats.Health = body.DoDamage(Stats.Health);
	}

	public void Start()
	{
		Stats.Health = 100;
		Stats.isDead = false;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		_canShoot = true;
		_shootingTimer.WaitTime = Stats.FiringRate;
		_shootingTimer.Start();
	}

	private void OnShootingTimerTimeout()
	{
		_shootingTimer.Start();
		if (!_canShoot)
			return;

		// Grab a mob from the mobs group.
		var mobs = GetTree().GetNodesInGroup("mobs");

		// Find the closest mob to the player.
		Mob mob = null;

		foreach (Node node in mobs)
		{
			if (node is Mob mobNode)
			{
				if (mob == null)
					mob = mobNode;
				else
				{
					if (mob.Position.DistanceTo(Position) > mobNode.Position.DistanceTo(Position))
						mob = mobNode;
				}
			}
		}

		if (mob == null)
			return;

		Bullet bullet = BulletScene.Instantiate<Bullet>();
		GetParent().AddChild(bullet);

		Vector2 direction = (mob.Position - Position).Normalized();
		bullet.ConfigureBullet(Stats.Damage, Stats.ShotSpeed, Position + direction * 25, mob);
	}
}
