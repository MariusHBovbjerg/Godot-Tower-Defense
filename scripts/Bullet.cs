using Godot;
using System;

public partial class Bullet : RigidBody2D
{
	public void ConfigureBullet(double damage, float speed, Godot.Vector2 position, Mob target)
	{
		if (!IsInstanceValid(target))
			Free();
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
			Free();
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (target != null)
		{
			Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();

			state.LinearVelocity = direction * (float)speed;
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

