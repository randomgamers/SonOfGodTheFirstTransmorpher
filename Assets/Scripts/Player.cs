using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	// STATIC FIELDS
	private static Player instance;
	
	// STATIC METHODS
	public static Player Instance {
		get {
			if (instance == null)
				instance =  GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
			return instance;
		}
	}
	
	// PRIVATE FIELDS
	private int controllerPosition;
	private int sequenceLength;
	private string[] sequence;
	private float desiredSpeedX;
	private float desiredSpeedY;
	private int shape;
	private float destCoordY;
	private EnvironmentType currentEnvironment;
	
	private List<BaseItem> items;
	
	// PROPERTIES
	public bool shouldIUpdateSpeed;
	
	// public Vector2 Speed { get { return new Vector2(playSpeed, currSpeedY);}}
	public Vector2 Speed { get { return new Vector2(desiredSpeedX, desiredSpeedY); } }
	
	public bool ArrowUpPressed;
	public bool ArrowDownPressed;
	public bool ArrowLeftPressed;
	public bool ArrowRightPressed;
	public bool ShapeShift0Pressed;
	public bool ShapeShift1Pressed;
	public bool ShapeShift2Pressed;
	public bool ShapeShift3Pressed;
	public bool PausePressed;
	public bool AnyKeyPressed;
	private float lastY = -5;
	private float dY = 0;
	
	private float X {
		get {
			return gameObject.transform.position.x;
		}
	}
	
	private float Y {
		get {
			return gameObject.transform.position.y;
		}
	}
	
	// UNITY METHODS
	void Start () {
		shouldIUpdateSpeed = true;
		
		desiredSpeedX = GameManager.Instance.CurrentBaseSpeed;

		changeShape(0);
		items = new List<BaseItem>();
	}
	
	void Update() {
		
		ArrowUpPressed = Input.GetButtonDown("ArrowUp");
		ArrowDownPressed = Input.GetButtonDown("ArrowDown");
		ArrowLeftPressed = Input.GetButtonDown("ArrowLeft");
		ArrowRightPressed = Input.GetButtonDown("ArrowRight");
		ShapeShift0Pressed = Input.GetButtonDown("ShapeShift0");
		ShapeShift1Pressed = Input.GetButtonDown("ShapeShift1");
		ShapeShift2Pressed = Input.GetButtonDown("ShapeShift2");
		ShapeShift3Pressed = Input.GetButtonDown("ShapeShift3");
		AnyKeyPressed = Input.anyKey;
		PausePressed = Input.GetButtonDown("Pause");
		
		if (!GameManager.Instance.IsPaused) {
			MyUpdate();
		}
	}
	
	void MyUpdate () {
		
		dY = Mathf.Abs(Y - lastY);
		lastY = Y;
		
		updateShape();
		updateSpeed();
		
		desiredSpeedY = EaseY(desiredSpeedY);
		
		float speedFactor = ItemEffects();
		GameManager.Instance.CurrentBaseSpeed *= speedFactor;			
	}
	
	public bool IsKeyDown(string keyName) {
		switch (keyName) {
			case "ArrowUp":
				return ArrowUpPressed;
			case "ArrowDown":
				return ArrowDownPressed;
			case "ArrowLeft":
				return ArrowLeftPressed;
			case "ArrowRight":
				return ArrowRightPressed;
			case "ShapeShift0":
				return ShapeShift0Pressed;
			case "ShapeShift1":
				return ShapeShift1Pressed;
			case "ShapeShift2":
				return ShapeShift2Pressed;
			case "ShapeShift3":
				return ShapeShift3Pressed;
			case "Pause":
				return PausePressed;
		}
		return false;
	}
	
	private bool validateControl() {
		if(sequenceLength == 0)
		{
			if (!AnyKeyPressed) { return true; }
		}
		else if (IsKeyDown(sequence[(controllerPosition = controllerPosition % sequenceLength)]))
		{
			controllerPosition++;
			return true;
		}
		return false;
	}
	
	private bool CanIShapeShift(int shapeID) {
		switch (shapeID)
		{
			case 0:	// man
			case 2: // worm
				return currentEnvironment == EnvironmentType.GROUND;
			case 3: // jellyfish
				return currentEnvironment == EnvironmentType.WATER;
		}
		return true;
	}
	
	private void ShapeShiftIfPossible(int shapeID) {
		if (CanIShapeShift(shapeID)) {
			changeShape(shapeID);
		}
	}
	
	private void updateShape() {
		if ((currentEnvironment == EnvironmentType.WATER) && (shape == 0 || shape == 2)) { changeShape(3); } // if water and (man or worm) then jellyfish
		if ((currentEnvironment == EnvironmentType.GROUND) && (shape == 3)) { changeShape(2); } // if ground and jellyfish then worm
		
		if (ShapeShift0Pressed) { ShapeShiftIfPossible(0); }
		if (ShapeShift1Pressed) { ShapeShiftIfPossible(1); }
		if (ShapeShift2Pressed) { ShapeShiftIfPossible(2); }
		if (ShapeShift3Pressed) { ShapeShiftIfPossible(3); }
	}
	
	private void SetColliderData(Vector2 offset, Vector2 size) {
		foreach (BoxCollider2D collider in gameObject.GetComponents<BoxCollider2D>()) {
			collider.offset = offset;
			collider.size = size;
		}
	}
	
	public void changeShape(int shapeID) {
		shape = shapeID;
		controllerPosition = 0;
		switch(shapeID)
		{
			case 0: // man
				gameObject.GetComponent<Animator>().SetTrigger("toHumanTrigger");
				GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toHuman");
				sequenceLength = 1;
				sequence = new string[1] {"ArrowRight"};
				destCoordY = Config.Instance.PlayerMiddleY;
				SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.manSound);
				Player.Instance.SetColliderData(new Vector2(0.12f, -0.6f), new Vector2(1.78f, 1.8f));
				break;
			case 1: // bird
				gameObject.GetComponent<Animator>().SetTrigger("toBirdTrigger");
				GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toBird");
				sequenceLength = 2;
				sequence = new string[2] {"ArrowUp", "ArrowDown"};
				destCoordY = Config.Instance.PlayerTopY;
				SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.birdSound);
				Player.Instance.SetColliderData(new Vector2(0.0f, 0.25f), new Vector2(1.75f, 1.8f));
				break;
			case 2: // worm
				gameObject.GetComponent<Animator>().SetTrigger("toSnakeTrigger");
				GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toSnake");
				sequenceLength = 4;
				sequence = new string[4] {"ArrowLeft", "ArrowUp", "ArrowRight", "ArrowDown"};
				destCoordY = Config.Instance.PlayerBottomY;
				SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.wormSound);
				Player.Instance.SetColliderData(new Vector2(0.0f, 0.0f), new Vector2(2.0f, 1.0f));
				break;
			case 3: // jellyfish
				gameObject.GetComponent<Animator>().SetTrigger("toJellyTrigger");
				GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toJelly");
				sequenceLength = 2;
				sequence = new string[2] {"ArrowLeft", "ArrowRight"};
				destCoordY = Config.Instance.PlayerBottomY;
				SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.jellyfishSound);
				Player.Instance.SetColliderData(new Vector2(0.0f, 0.0f), new Vector2(1.5f, 1.0f));
				break;
			case 4: // GODmode
				gameObject.GetComponent<Animator>().SetTrigger("toJesusTrigger");
				GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toJesus");
				sequenceLength = 0;
				destCoordY = Config.Instance.PlayerMiddleY;
				SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.jesusSound);
				Player.Instance.SetColliderData(new Vector2(0.05f, 0.0f), new Vector2(1.2f, 2.5f));
				break;
		}
	}
	
	private float EaseY(float y) {
		return Mathf.Sign(y) * (Mathf.Abs(y)+0.3f)*(Mathf.Abs(y)+0.3f) * Config.Instance.ShiftEasingFactor;
	}
	
	private void updateSpeed() {
		float windowPosition = gameObject.transform.position.x - GameManager.Instance.LeftCameraX;
		
		if (validateControl() || dY > GameManager.Instance.ScaledEpsilon) {
			if (0 < windowPosition && windowPosition < Config.Instance.DefaultPlayerOffset) {
				desiredSpeedX = (Mathf.Cos(Mathf.PI * windowPosition / Config.Instance.DefaultPlayerOffset) + 1.0f) * 2.0f + 0.8f;
			} else {
				desiredSpeedX = 0;
			}
		} else {
			desiredSpeedX *= ((Mathf.Cos(2 * Mathf.PI * windowPosition / Config.Instance.DefaultPlayerOffset) + 49) / 50) * Config.Instance.SpeedFaultFactor;
		}
		
		if (LevelGenerator.Instance.IsWon) {
			desiredSpeedX = 0;
		}
		
		desiredSpeedY = destCoordY - Y;
	}
	
	public float ItemEffects() { // returns speed modifier, applies jesus
		
		int i = items.Count - 1;
		bool isInEffect;
		float speedFactor = 1;
		bool jesusBool = false;
		BaseItem item;
		
		while (i >= 0)
		{
			item = items[i];
			
			isInEffect = item.IsInEffect(gameObject.transform.position.x);
			if(item.ID == 1) { jesusBool = true;}
			
			if(isInEffect)
			{ 
				speedFactor *= item.SpeedFactor;
			}
			else
			{ items.RemoveAt(i); }
			
			i--;
		}
		
		if (!LevelGenerator.Instance.IsWon) {
			if(jesusBool && shape != 4) { getRidOfAllJesusesPlease(); }
			if (!(jesusBool) && shape == 4) { changeShape(0); }
		}
		
		return speedFactor;
	}
	
	private void getRidOfAllJesusesPlease()
	{ 
		BaseItem item;
		int i = items.Count - 1;
		
		while (i >= 0)
		{
			item = items[i];
			if(item.ID == 1) {items.RemoveAt(i); }
			i--;
		}
	}
	
	// PUBLIC METHODS
	public void AddItem(BaseItem item)
	{
		items.Add(item);
		if (item.ID == 1) {
			if (shape != 4) { 
				changeShape(4); 
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		string otherTag = other.gameObject.tag;
		
		switch (otherTag) {
			case "Tree":
				SoundManager.Instance.Play(SoundManager.Instance.treeHit);
				break;
			case "Stone":
				SoundManager.Instance.Play(SoundManager.Instance.stoneHit);
				break;
			case "Cloud":
				SoundManager.Instance.Play(SoundManager.Instance.cloudHit);
				break;
			case "Mine":
				SoundManager.Instance.Play(SoundManager.Instance.mineHit);
				break;
		}
		
		if (otherTag.Equals("Item")) {
			AddItem(other.GetComponent<ItemHolder>().itemHolder);
			other.GetComponent<ItemHolder>().itemHolder.playSoundEffect();
			Destroy(other.gameObject);
		}
		
		if (otherTag.Equals("GroundToWater")) {
			currentEnvironment = EnvironmentType.WATER;
		}
		
		if (otherTag.Equals("WaterToGround")) {
			currentEnvironment = EnvironmentType.GROUND;
		}
	}
	
	public bool IsJesus() { return shape == 4; }
}
	// public void SetSpeed(float speed)
	// {
	// 	speedX = speed;
	// }
	
	// private void _updateShape()
	// {	
	// 	if((currentEnvironment == EnvironmentType.WATER) && (shape == 0 || shape == 2)) { changeShape(3); } // if water and (man or worm) then jellyfish
	// 	if((currentEnvironment == EnvironmentType.GROUND) && (shape == 3)) { changeShape(2); } // if ground and jellyfish then worm
		
	// 	if(Mathf.Abs(gameObject.GetComponent<Rigidbody2D>().velocity.y) < Config.Instance.EpsilonFactor)
	// 	{
	// 		if(Input.GetButtonDown("ShapeShift0")) { GameManager.Instance.ShapeShift(0); changeShape(0); }
	// 		if(Input.GetButtonDown("ShapeShift1")) { GameManager.Instance.ShapeShift(1); changeShape(1); }
	// 		if(Input.GetButtonDown("ShapeShift2")) { GameManager.Instance.ShapeShift(2); changeShape(2); }
	// 		if(Input.GetButtonDown("ShapeShift3")) { GameManager.Instance.ShapeShift(3); changeShape(3); }
	// 	}
	// }
	
	// two button version shape shifting
	// private void processInput()
	// {
	// 	if(Input.GetButtonDown("ShiftDown")) { shape--; }
	// 	if(Input.GetButtonDown("ShiftUp")) { shape++; }
		
	// 	switch(shape)
	// 	{
	// 		case  4:
	// 		case  0:
	// 		    shape = 0;
	// 			break;
	// 		case  1:
	// 			shape = 1;
	// 			break;
	// 		case  2:
	// 			shape = 2;
	// 			break;
	// 		case  3:
	// 		case -1:
	// 			shape = 3;
	// 			break;
	// 	}
	// 	changeShape(shape);
	// }
	
	// private void applyItems()
	// {
	// 	int i = items.Count - 1;
	// 	bool isInEffect;
	// 	float speedFactor = 1;
	// 	bool jesusBool = false;
	// 	BaseItem item;
		
	// 	while (i >= 0)
	// 	{
	// 		item = items[i];
			
	// 		isInEffect = item.IsInEffect(gameObject.transform.position.x);
	// 		if(item.ID == 1) { jesusBool = true;}
			
	// 		if(isInEffect)
	// 		{ 
	// 			speedFactor *= item.SpeedFactor;
	// 		}
	// 		else
	// 		{ items.RemoveAt(i); }
			
	// 		i--;
	// 	}
		
	// 	if(jesusBool && shape != 4) { getRidOfAllJesusesPlease(); }
	// 	if (!(jesusBool) && shape == 4) { changeShape(0); }
		
	// 	if (shouldIUpdateSpeed) {
	// 		Camera.main.GetComponent<Rigidbody2D>().velocity = new Vector2(Config.Instance.PlayerDefautSpeed * speedFactor ,0);
	// 		playSpeed = currSpeedX * speedFactor;
	// 		Debug.Log("PlaySpeed :" + playSpeed);
	// 	} else {
	// 		playSpeed = 0.0f;
	// 	}
	// }
	
	// private void _updateSpeed()
	// {
	// 	float windowPosition = gameObject.transform.position.x - GameManager.Instance.LeftCameraX;
		
	// 	if(validateControl() || (Mathf.Abs(gameObject.GetComponent<Rigidbody2D>().velocity.y) > Config.Instance.EpsilonFactor))
	// 	{
	// 		if (0 < windowPosition && windowPosition < Config.Instance.DefaultPlayerOffset)			
	// 		{ speedX = (Mathf.Cos(Mathf.PI * windowPosition / Config.Instance.DefaultPlayerOffset) + 2) * Config.Instance.PlayerDefautSpeed * Config.Instance.SpeedupFactor; } //}((sequenceLength + 16) / 20)*
	// 		else
	// 		{ speedX = 0; }
	// 	}
	// 	else
	// 	{ speedX *= ((Mathf.Cos(2 * Mathf.PI * windowPosition / Config.Instance.DefaultPlayerOffset) + 49) / 50) * Config.Instance.SpeedFaultFactor; }
		
	// 	currSpeedX = (speedX + 3 * currSpeedX) / 4; // TODO test
	// 	Debug.Log("Speed updated to :" + currSpeedX);
	// 	currSpeedY = (destCoordY - gameObject.transform.position.y) * Config.Instance.ShiftEasingFactor * currSpeedX; 
	// }
	
	// // PRIVATE METHODS
	// private bool _validateControl()
	// {
	// 	// WHEEEEE
	// 	if (sequenceLength == 0)
	// 	{
	// 		if (!(Input.anyKey)) { return true; }
	// 	}
	// 	else if (Input.GetButtonDown(sequence[(controllerPosition = controllerPosition % sequenceLength)]))
	// 	{
	// 		controllerPosition++;
	// 		return true;
	// 	}
	// 	return false;
	// }
	
	// void _MyUpdate ()
	// {
	// 	if(!(LevelGenerator.Instance.IsWon))
	// 	{
	// 		dY = Mathf.Abs(Y - lastY);
	// 		updateShape();
	// 		updateSpeed();
	// 		applyItems();
	// 	}
	// 	else {
	// 		if (winJesusSet == false)
	// 		{
	// 			changeShape(4);
	// 			winJesusSet = true;
	// 		}
	// 		currSpeedY = 0.0f;
	// 		speedX *= 1.3f;
	// 	}
	// }
	
	// void _Start ()
	// {
	// 	shouldIUpdateSpeed = true;
		
	// 	speedX = Config.Instance.PlayerDefautSpeed;
	// 	currSpeedY = 0;
	// 	changeShape(0);
	// 	items = new List<BaseItem>();
	// }
	
	//private void _changeShape(int shapeID)
	// {
		
		
	// 	shape = shapeID;
	// 	controllerPosition = 0;

	// 	switch(shapeID)
	// 	{
	// 		case 0: // man
	// 			if(currentEnvironment == EnvironmentType.GROUND) {
	// 				gameObject.GetComponent<Animator>().SetTrigger("toHumanTrigger");
	// 				GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toHuman");
	// 				sequenceLength = 1;
	// 				sequence = new string[1] {"ArrowRight"};
	// 				destCoordY = Config.Instance.PlayerMiddleY;
	// 				SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.manSound);
	// 				Player.Instance.SetColliderData(new Vector2(0.12f, -0.6f), new Vector2(1.78f, 1.8f));
	// 			}
	// 			break;
	// 		case 1: // bird
	// 			gameObject.GetComponent<Animator>().SetTrigger("toBirdTrigger");
	// 			GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toBird");
	// 			sequenceLength = 2;
	// 			sequence = new string[2] {"ArrowUp", "ArrowDown"};
	// 			destCoordY = Config.Instance.PlayerTopY;
	// 			SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.birdSound);
	// 			Player.Instance.SetColliderData(new Vector2(0.0f, 0.0f), new Vector2(1.75f, 2.5f));
	// 			break;
	// 		case 2: // worm
	// 			if(currentEnvironment == EnvironmentType.GROUND) {
	// 			gameObject.GetComponent<Animator>().SetTrigger("toSnakeTrigger");
	// 			GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toSnake");
	// 			sequenceLength = 4;
	// 			sequence = new string[4] {"ArrowLeft", "ArrowUp", "ArrowRight", "ArrowDown"};
	// 			destCoordY = Config.Instance.PlayerBottomY;
	// 			SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.wormSound);
	// 			Player.Instance.SetColliderData(new Vector2(0.0f, 0.0f), new Vector2(2.0f, 1.0f));
	// 			}
	// 			break;
	// 		case 3: // jellyfish
	// 		    if(currentEnvironment == EnvironmentType.WATER) {
	// 			Debug.Log("I'm mighty jellyfish!");
	// 			gameObject.GetComponent<Animator>().SetTrigger("toJellyTrigger");
	// 			GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toJelly");
	// 			sequenceLength = 2;
	// 			sequence = new string[2] {"ArrowLeft", "ArrowRight"};
	// 			destCoordY = Config.Instance.PlayerBottomY;
	// 			SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.jellyfishSound);
	// 			Player.Instance.SetColliderData(new Vector2(0.0f, 0.0f), new Vector2(1.5f, 1.0f));
	// 			}
	// 			break;
	// 		case 4: // GODmode
	// 			gameObject.GetComponent<Animator>().SetTrigger("toJesusTrigger");
	// 			GameObject.FindGameObjectWithTag("ArrowButtons").GetComponent<Animator>().SetTrigger("toJesus");
	// 			sequenceLength = 0;
	// 			destCoordY = Config.Instance.PlayerMiddleY;
	// 			SoundManager.Instance.SetNewPlayerSound(SoundManager.Instance.jesusSound);
	// 			Player.Instance.SetColliderData(new Vector2(0.05f, 0.0f), new Vector2(1.2f, 2.5f));
	// 			break;
	// 		// case 5: // CageMode
	// 		// 	// cosi
	// 		// 	break;
	// 	}
	// }