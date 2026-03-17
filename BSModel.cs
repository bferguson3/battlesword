using Godot;

public partial class BSModel : Sprite3D
{
	public CollisionObject3D myCollider;
	public RayCast3D myEyes;
	private CollisionShape3D myShape;
	public Godot.Collections.Array<Node> myLines;

	[Export]
	public bool inCoverDisplay;
	private float colorMult;
	private bool colorDown;
	private Color mySavedColor;

	public void SetCollisionMask(int layer, bool v)
	{
		
		myCollider.SetCollisionMaskValue(layer, v);
	}
	public void SetCollisionLayer(int l, bool v)
	{
		myCollider.SetCollisionLayerValue(l, v);
	}
	
	public override void _Ready()
	{
		colorMult = 1.0f;
		myCollider = (CollisionObject3D)GetNode("battlenun-body");	
		myShape = (CollisionShape3D)GetNode("battlenun-body/battlenun-collider");
		myLines = myShape.GetNode<Node>("LOSNodes").GetChildren();
		myEyes = GetNode<Node>("battlenun-body").GetNode<RayCast3D>("EyeCaster");

		myCollider.SetRayPickable(true);
		myCollider.MouseEntered += MouseFunc;

		mySavedColor = Modulate;
	}

	public void SetColor(Color c)
	{
		Modulate = c;
		mySavedColor = c;
	}

	public void MouseFunc()
	{
		GD.Print(Name);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(inCoverDisplay)
		{	// flash me grey 
			if(!colorDown){
				colorMult -= (float)delta * 0.25f;
				if (colorMult > 0.75f)
				{
					Modulate = new Color(mySavedColor.R * colorMult, mySavedColor.G *colorMult, mySavedColor.B * colorMult);
				}
				else 
					colorDown = true;
			}
			else
			{
				colorMult += (float)delta * 0.25f;
				if (colorMult < 1.0f)
				{
					Modulate = new Color(mySavedColor.R * colorMult, mySavedColor.G *colorMult, mySavedColor.B * colorMult);
				}else 
					colorDown = false;
			}
		}
	}

	public void ColliderOff()
	{
		myShape.SetDeferred("disabled", true);
	}

	public void ColliderOn()
	{
		myShape.SetDeferred("disabled", false);
	}
}
