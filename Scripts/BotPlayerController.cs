using Godot;
using System;

public partial class BotPlayerController : CharacterBody3D
{
	[Export] int vie = 3;
	[Export] float speed = 20f; // Vitesse de marche
	[Export] float acceleration = 15f; // Vitesse pour passer de 0 à la vitesse de croisière 
	[Export] float airAcceleration = 5f; // Pour se déplacer dans les airs
	[Export] float gravity = 1f;
	[Export] float maxTerminalVelocity = 55f; // Vitesse Max 
	[Export] float jumpForce = 20f; 
	
	[Export (PropertyHint.Range, "0.1, 1.0")] float mouseSensivity = 0.3f;
	[Export (PropertyHint.Range, "-90, 0, 1")] float minPitch = -90f;
	[Export (PropertyHint.Range, "0, 90, 1")] float maxPitch = 90f;
	bool bounce = false;
	
	Vector3 velocity;
	float yVelocity;
	[Export]Node3D cameraPivot;
	[Export]Camera3D camera;
	[Export]AnimationPlayer animationPlayer; 
	
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		animationPlayer.Play("Idle");
	}
	
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
	
	// Detecte les input automatiquement
	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseMotion motionEvent)
		{
			// On modifie la rotation sur l'axe Y du noeud principal
			// Le personnage et la camera vont pivoter de gauche à droite
			Vector3 rotDeg = RotationDegrees;
			rotDeg.Y -= motionEvent.Relative.X * mouseSensivity;
			RotationDegrees = rotDeg;
			
			// On pivote le noeud CameraPivot sur son Axe X
			// Donc de haut en bas
			rotDeg = cameraPivot.RotationDegrees;
			rotDeg.X -= motionEvent.Relative.Y * -mouseSensivity;
			rotDeg.X = Mathf.Clamp(rotDeg.X, minPitch, maxPitch);
			cameraPivot.RotationDegrees = rotDeg;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		HandleMovement(delta);
	}
	
	public async void HandleMovement(double delta)
	{
		Vector3 direction = new Vector3(0, 0, 0);
		if(Input.IsActionPressed("move_up"))
		{
			direction += Transform.Basis.Z;
		}
		if(Input.IsActionPressed("move_down"))
		{
			direction -= Transform.Basis.Z;
		}
		if(Input.IsActionPressed("move_left"))
		{
			direction += Transform.Basis.X;
		}
		if(Input.IsActionPressed("move_right"))
		{
			direction -= Transform.Basis.X;
		}
		
		// Permet d'avoir la même vitesse même si on appuie sur deux directions
		direction = direction.Normalized(); 
		
		float accel = IsOnFloor() ? acceleration : airAcceleration;
		velocity = direction * speed * accel;
		
		if(bounce)
		{
			yVelocity = jumpForce;
			bounce = false;
		}
		else
		{
			if(IsOnFloor())
			{
				yVelocity = -0.01f; // Pas besoin d'être attiré au sol car le personnage est déjà sur le sol
			}
			else
			{
				yVelocity = Mathf.Clamp(yVelocity - gravity, -maxTerminalVelocity, maxTerminalVelocity);
				animationPlayer.Play("fall");
			}
		}
		
		if(Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			yVelocity = jumpForce;
		}
		
		velocity.Y = yVelocity;
		Velocity = velocity;
		MoveAndSlide();
		
		// Permet de savoir avec quoi le personnage entre en collision
		/*
		for(int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			//GD.Print("Collision avec ", ((Node)collision.GetCollider()).Name);
		}
		*/
		
		if(direction != new Vector3(0,0,0) && IsOnFloor())
		{
			animationPlayer.Play("walk");
		}
		if(direction == new Vector3(0,0,0) && IsOnFloor())
		{
			animationPlayer.Play("Idle");
		}
	}
}
