using Godot;
using System;
using System.Collections.Generic;
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

	public Timer ShootingTimer;
	private Timer _rapidFireTimer;
	private bool _canShoot = true;
	private bool _isRapidFiring = false;

	public Sprite2D RangeIndicator { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		isDead = true;

		ShootingTimer = GetNode<Timer>("ShootingTimer");
		_rapidFireTimer = GetNode<Timer>("RapidFireTimer");
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
		ShootingTimer.Start();
	}

	private void OnShootingTimerTimeout()
	{
		ShootingTimer.Start();
		if (!_canShoot)
			return;

		// Find all valid mobs within range.
		Dictionary<Mob, float> mobTargets = FindValidMobTargets();

		if (mobTargets.Count == 0)
			return;

		var random = new Random();
		HandleRapidFire(random);

		mobTargets = HandleMultiShot(random, mobTargets);

		mobTargets.Select(x => x.Key).ToList().ForEach(mob =>
		{
			Bullet bullet = BulletScene.Instantiate<Bullet>();
			GetParent().AddChild(bullet);

			Vector2 direction = (mob.Position - Position).Normalized();
			bullet.ConfigureBullet(Stats.Damage.GetValue(), (float)Stats.ShotSpeed.GetValue(), Position + direction * 25, mob);
		});
	}

	private Dictionary<Mob, float> FindValidMobTargets()
	{
		var mobs = GetTree().GetNodesInGroup("mobs");
		var bullets = GetTree().GetNodesInGroup("bullets");
		Dictionary<Mob, float> mobTargets = new();

		foreach (Node node in mobs)
		{
			if (node is Mob mobNode && IsInstanceValid(mobNode))
			{
				var distance = mobNode.Position.DistanceTo(Position);
				// Evaluate if mob is within player range.
				if (distance > Stats.Range.GetValue())
					continue;

				// count how many bullets already target this mob, add their damage together and compare to the mob's health.
				// If the mob would die, target another mob.
				var mobHealth = mobNode.Stats.CurrentHealth;
				var bulletDamage = bullets.Where(b => (b as Bullet).target == mobNode).Sum(b => (b as Bullet).damage);

				if (mobHealth <= bulletDamage)
					continue;

				mobTargets.Add(mobNode, distance);
			}
		}

		return mobTargets;
	}

	private Dictionary<Mob, float> HandleMultiShot(Random random, Dictionary<Mob, float> mobTargets)
	{
		if (Stats.MultiShotChance.GetValue() > 0)
		{
			var chance = random.Next(0, 100);
			if (chance < Stats.MultiShotChance.GetValue())
			{
				var multiShotCount = (int)Stats.MultiShotCount.GetValue() + 1;
				if (mobTargets.Count > multiShotCount)
				{
					mobTargets = mobTargets.OrderBy(x => x.Value).Take(multiShotCount).ToDictionary(x => x.Key, x => x.Value);
				}
			}
			else
			{
				// If the roll is unsuccessful, only target the closest mob.
				if (mobTargets.Count > 1)
				{
					var closestMob = mobTargets.OrderBy(x => x.Value).First();
					mobTargets.Clear();
					mobTargets.Add(closestMob.Key, closestMob.Value);
				}
			}
		}
		else
		{
			// If the player doesn't have the ability, only target the closest mob.
			if (mobTargets.Count > 1)
			{
				var closestMob = mobTargets.OrderBy(x => x.Value).First();
				mobTargets.Clear();
				mobTargets.Add(closestMob.Key, closestMob.Value);
			}
		}

		return mobTargets;
	}

	private void HandleRapidFire(Random random)
	{
		if (Stats.RapidFireChance.GetValue() > 0 && !_isRapidFiring)
		{
			var chance = random.Next(0, 100);
			if (chance < Stats.RapidFireChance.GetValue())
			{
				_isRapidFiring = true;
				ShootingTimer.WaitTime = 1 / (Stats.FiringRate.GetValue() * 5);
				_rapidFireTimer.WaitTime = Stats.RapidFireDuration.GetValue();
				_rapidFireTimer.Start();
			}
		}
	}

	private void OnFastFireTimeout()
	{
		_isRapidFiring = false;
		ShootingTimer.WaitTime = 1 / Stats.FiringRate.GetValue();
	}
}
