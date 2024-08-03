using Godot;
using System;

public interface IEntityBuilder
{
	Node2D Build(Texture2D texture, Vector2 scale, Vector2 position);
}
