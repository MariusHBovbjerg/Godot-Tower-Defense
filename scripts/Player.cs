using Godot;
using System;
using System.Linq;

public partial class Player : RigidBody2D
{
	public PlayerStats Stats = new PlayerStats
	{
		CashBalance = 10000,
		CoinBalance = 0,
		Damage = new Stat
		{
			Type = StatType.Offensive,
			Name = "Damage",
			StartingValue = 987,
			UpgradeMultiplier = 10.1f,
			StartingCost = 10,
			CostIncreaseValue = 1.1f,
			Level = 1,
			MaxLevel = 10,
			GetValueCallback = (level, startingValue, upgradeMultiplier) => startingValue * Math.Pow(upgradeMultiplier, level - 1),
			NextLevelCostCallback = (level, cost, costIncreaseValue) => (int)(cost * Math.Pow(costIncreaseValue, level - 1))
		},
	};

	public bool isDead { get; set; }

	[Export]
	public PackedScene BulletScene { get; set; }

	[Signal]
	public delegate void DeathEventHandler();

	private Timer _shootingTimer;
	private bool _canShoot = true;

	public Sprite2D RangeIndicator { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		isDead = true;

		_shootingTimer = GetNode<Timer>("ShootingTimer");
		var range = Stats.Range.GetValue();
		RangeIndicator = GetNode<Sprite2D>("RangeIndicator");
		RangeIndicator.Scale = new Vector2((float)range / 150, (float)range / 150);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Stats.CurrentHealth <= 0 && !isDead)
		{
			Stats.CashBalance = 0;
			isDead = true;
			Hide(); // Player disappears when it dies.
			EmitSignal(SignalName.Death);
			_canShoot = false;
			// Must be deferred as we can't change physics properties on a physics callback.
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}

		var FiringRate = Stats.FiringRate.GetValue();
		var inverseFiringRate = 1 / FiringRate;
		if (_shootingTimer.WaitTime != inverseFiringRate)
			_shootingTimer.WaitTime = inverseFiringRate;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not Mob)
			return;

		if ((body as Mob).CanDamage)
			Stats.CurrentHealth = (body as Mob).DoDamage(Stats.CurrentHealth);
	}

	public void InitializeNewRound()
	{
		Stats.CurrentHealth = Stats.MaxHealth.GetValue();
		isDead = false;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		_canShoot = true;
		_shootingTimer.WaitTime = 1 / Stats.FiringRate.GetValue();
		_shootingTimer.Start();
	}

	private void OnShootingTimerTimeout()
	{
		_shootingTimer.Start();
		if (!_canShoot)
			return;

		// Grab a mob from the mobs group.
		var mobs = GetTree().GetNodesInGroup("mobs");
		var bullets = GetTree().GetNodesInGroup("bullets");

		// Find the closest mob to the player.
		Mob mob = null;

		foreach (Node node in mobs)
		{
			if (node is Mob mobNode && IsInstanceValid(mobNode))
			{
				// Evaluate if mob is within player range.
				if (mobNode.Position.DistanceTo(Position) > Stats.Range.GetValue())
					continue;

				// count how many bullets already target this mob, add their damage together and compare to the mob's health.
				// If the mob would die, target another mob.
				var mobHealth = mobNode.Stats.CurrentHealth;
				var bulletDamage = bullets.Where(b => (b as Bullet).target == mobNode).Sum(b => (b as Bullet).damage);

				if (mobHealth <= bulletDamage)
					continue;

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
		bullet.ConfigureBullet(Stats.Damage.GetValue(), (float)Stats.ShotSpeed.GetValue(), Position + direction * 25, mob);
	}
}
