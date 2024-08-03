using Godot;
using System;

public partial class Bullet : RigidBody2D
{
	public static readonly string GROUP_NAME = "bullets";
	public void ConfigureBullet(double damage, float speed, Vector2 position, Mob target)
	{
		if (!IsInstanceValid(target))
			QueueFree();

		this.damage = damage;
		this.speed = speed;
		Position = position;
		this.target = target;
	}

	public double damage;
	public float speed;

	public Mob target;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsInstanceValid(target))
			QueueFree();
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (target != null)
		{
			Vector2 direction = (target.Position - Position).Normalized();

			state.LinearVelocity = direction * speed;
		}
	}

	private void OnEnteredBody(Node2D body)
	{
		if (body is not Mob)
			return;

		QueueFree();
		if (IsInstanceValid(body))
			(body as Mob).TakeDamage(damage);
	}
}
