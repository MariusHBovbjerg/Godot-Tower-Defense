using Godot;
using System;

public partial class Bullet : RigidBody2D
{
	public void ConfigureBullet(double damage, int speed, Godot.Vector2 position, Mob target)
	{
		this.damage = damage;
		this.speed = speed;
		Position = position;
		this.target = target;
	}


	private double damage;
	private int speed;

	public Mob target;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (target != null)
		{
			Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
			state.LinearVelocity = direction * speed;
		}
	}

	private void OnEnteredBody(Mob body)
	{
		QueueFree();
		body.TakeDamage(damage);
	}
}

