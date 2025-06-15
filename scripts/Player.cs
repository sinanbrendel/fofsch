using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float baseSpeed = 3.0f;
	public float Speed = baseSpeed;
	public const float JumpVelocity = 1.5f;
	public float MouseSensitivity = 0.003f;
	
	private Camera3D _camera;
	
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("PlayerCollision/PlayerCamera");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			// Kamera vertikal drehen
			_camera.Rotation = new Vector3(Mathf.Clamp(_camera.Rotation.X - mouseMotion.Relative.Y * MouseSensitivity, -1.5f, 1.5f), _camera.Rotation.Y, _camera.Rotation.Z);

			// Spieler horizontal drehen
			RotateY(-mouseMotion.Relative.X * MouseSensitivity);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		
		if (Input.IsActionPressed("crouch") && IsOnFloor())
		{
			Speed = baseSpeed * 0.4f;
		}
		else if (Input.IsActionPressed("sprint") && IsOnFloor())
		{
			Speed = baseSpeed * 1.8f;
		}
		else
		{
			Speed = baseSpeed;
		}
		
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
