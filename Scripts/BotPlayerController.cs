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
}
