using UnityEngine;
using System.Collections.Generic;
using Obstacles;
using Tuples;

public class GameManager : MonoBehaviour {
	
	// Instance
	private static GameManager instance;
	public static GameManager Instance {
		get {
			if (instance == null)
				instance =  GameObject.FindGameObjectWithTag("Global").GetComponent<GameManager>();
			return instance;
		}
	}
	
	// Public attributes
	public int LeftCameraX {
		get {
			return (int) (mainCamera.transform.position.x - mainCamera.GetComponent<Camera>().orthographicSize * mainCamera.GetComponent<Camera>().aspect);
		}
	}
	
	// Public attributes
	public GameObject progressBarBackground;
	public GameObject progressBar;
	
	// Private attributes
	private GameObject mainCamera;
	private float lastUpdateX;
	private bool slowCamera;
	
	
	public float CurrentBaseSpeed;
	
	public float SpeedFactor {
		get {
			return CurrentBaseSpeed / Config.Instance.PlayerDefautSpeed;
		}
	}
	
	public bool IsPaused;
	private bool IsPausedGame;
	private GameObject tutText;
	private GameObject tutTextBackground;
	private GameObject pauseText;
	private GameObject pauseBackground;
	
	public float ScaledEpsilon {
		get {
			return Config.Instance.EpsilonFactor * SpeedFactor;
		}
	}
	
	private System.Random rnd = new System.Random();
	
	private List<int> tutorialSlowdownPositions = new List<int>() {7, 17, 18, 35, 38, 54, 55, 60, 65, 102, 152, 158, 10000 };
	private List<string> tutorialSlowdownKeys = new List<string>() {"ArrowRight", "ShapeShift1", "ArrowUp", "ShapeShift0", "ArrowRight", "ShapeShift2", "ArrowUp", "ShapeShift0", "ArrowRight", "ArrowRight", "ArrowRight", "ArrowRight"};
	private List<int> tutorialSlowdownShapeIDs = new List<int>() {-1, 1, -1, 0, -1, 2, -1, 0, -1, -1, -1, -1};
	private List<int> tutorialSlowdownTextIDs = new List<int>() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
	
	private Vector2 CameraSpeed() {
		return new Vector2(CurrentBaseSpeed, 0.0f);
	}
	
	void Start () {
		UnityEngine.Cursor.visible = false;
		
		CurrentBaseSpeed = Config.Instance.PlayerDefautSpeed;
		IsPaused = false;
		
		slowCamera = false;
		
		if (Config.Instance.IsTutorial) {
			Config.Instance.N_TotalTiles = Tutorial.TopObstacles.Length;
		}
			
		mainCamera = Camera.main.gameObject;
		mainCamera.GetComponent<Rigidbody2D>().velocity = CameraSpeed();
		// mainCamera.transform.position = new Vector3(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize, -20);
		
		float cameraWidth = 2 * Camera.main.orthographicSize * Camera.main.aspect;
		
		
		GameObject.FindGameObjectWithTag("ArrowButtons").transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.07f, 0.9f, 10));
		progressBarBackground.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 1.0f, 10));
		progressBar.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 1.0f, 8));
		
		
		progressBarBackground.transform.localScale = new Vector3(cameraWidth, progressBarBackground.transform.localScale.y, progressBarBackground.transform.localScale.z);
		progressBar.transform.localScale = new Vector3(0, progressBarBackground.transform.localScale.y, progressBar.transform.localScale.z); 
		
		Player.Instance.gameObject.transform.position = new Vector2(Config.Instance.DefaultPlayerOffset, Config.Instance.PlayerMiddleY);
		
		for (int i = 0; i < Camera.main.orthographicSize * 2 * Camera.main.aspect / Config.Instance.TileWidth + Config.Instance.N_BufferedTiles; i++) {
			Visualize(LevelGenerator.Instance.Next());
		}
		
		lastUpdateX = 0.0f;
		
	}
	
	public void ShowTutorialTextWithId(int id) {
		tutTextBackground = Instantiate(Resources.Load("Prefabs/TutTexts/textBackground")) as GameObject;
		tutTextBackground.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1.0f, 1.1f));
		tutTextBackground.transform.parent = Camera.main.transform;
		
		float cameraHeight = 2 * Camera.main.orthographicSize;
		float cameraWidth = cameraHeight * Camera.main.aspect;
		
		tutTextBackground.transform.localScale = new Vector3(cameraWidth, cameraHeight, tutTextBackground.transform.localScale.z);
		
		tutText = Instantiate(Resources.Load(string.Format("Prefabs/TutTexts/text{0}", id))) as GameObject;
		tutText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1));
		
	}
	
	public void HideTutorialText() {
		Destroy(tutText);
		Destroy(tutTextBackground);
	}
	
	private void Visualize(NextResult nextResult) {
		VisualizeEnvironment(nextResult.tile);
		VisualizeObstacles(nextResult.obstacles);
		VisualizeItems(nextResult.items, nextResult.obstacles.Item1.Position.x);
	}

	private void VisualizeItems(Tuple<BaseItem, BaseItem, BaseItem> tuple, float x) {
		int index = 0;
		float[] positions = {Config.Instance.PlayerBottomY, Config.Instance.PlayerMiddleY, Config.Instance.PlayerTopY};
		foreach (BaseItem item in tuple.ToArray()) {
			string spriteHandle = null;
			switch (item.ID) {
				case 1:
					spriteHandle = "Jesus";
					break;
				case 2:
					spriteHandle = string.Format("EnergyDrink{0}", rnd.Next(2));
					break;
				case 3:
					spriteHandle = string.Format("Mushroom{0}", rnd.Next(2));
					break;
			}
			
			if (spriteHandle != null) {
				GameObject newItem = Instantiate(Resources.Load(string.Format("Prefabs/Items/{0}", spriteHandle))) as GameObject; 
				newItem.transform.position = new Vector2(x, positions[index]);
				newItem.GetComponent<ItemHolder>().itemHolder = item;
			}
			
			index++;
		}
	}
	
	private void VisualizeObstacles(Tuple<BaseObstacle, BaseObstacle, BaseObstacle> tuple) {
		foreach (BaseObstacle obstacle in tuple.ToArray()) {
			if (obstacle.ID == 0) {
				(Instantiate(Resources.Load("Prefabs/Obstacles/Obstacle0")) as GameObject).transform.position = obstacle.Position;
			} else if (obstacle.ID == 1) {
				(Instantiate(Resources.Load(string.Format("Prefabs/Obstacles/Obstacle{0}_{1}", obstacle.ID, Mathf.Ceil((rnd.Next(100) + 1) / 45.0f)))) as GameObject).transform.position = obstacle.Position;
			} else {
				(Instantiate(Resources.Load(string.Format("Prefabs/Obstacles/Obstacle{0}_{1}", obstacle.ID, rnd.Next(3) + 1))) as GameObject).transform.position = obstacle.Position;
			}
		}
	}
	
	private void VisualizeEnvironment(Tuple<EnvironmentType, bool, float, float> environment) {
		EnvironmentType e = environment.Item1;
		bool eTransfer = environment.Item2;
		float tilePositionX = environment.Item3;
		float tilePositionY = environment.Item4;

		string tileHandle = "";
		if (e == EnvironmentType.GROUND) {
			if (eTransfer) {
				tileHandle = "WaterToGround";
			} else {
				tileHandle = "GroundTile0";  // possibility for random shit
			}
		} else {
			if (eTransfer) {
				tileHandle = "GroundToWater";
			} else {
				tileHandle = "WaterTile0";
			}
		}
		GameObject env = Instantiate(Resources.Load(string.Format("Prefabs/Tiles/{0}", tileHandle))) as GameObject;
		env.transform.position = new Vector3(tilePositionX, tilePositionY, 1);
		
	}
	
	public void SlowCamera(int textID) {
		ShowTutorialTextWithId(textID);
		slowCamera = true;
	} 
	
	public void ShowPauseText() {
		pauseBackground = Instantiate(Resources.Load("Prefabs/TutTexts/textBackground")) as GameObject;
		pauseBackground.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1.0f, 1.1f));
		pauseBackground.transform.parent = Camera.main.transform;
		
		float cameraHeight = 2 * Camera.main.orthographicSize;
		float cameraWidth = cameraHeight * Camera.main.aspect;
		
		pauseBackground.transform.localScale = new Vector3(cameraWidth, cameraHeight, pauseBackground.transform.localScale.z);
		
		pauseText = Instantiate(Resources.Load("Prefabs/pauseText")) as GameObject;
		pauseText.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1));
	}
	
	public void HidePauseText() {
		Destroy(pauseText);
		Destroy(pauseBackground);
	}
	
	public void PauseGame(){
		ShowPauseText(); 
		slowCamera = true;
	}
	
	void Update () {

		if (Config.Instance.IsTutorial && LevelGenerator.Instance.N_Game_Calls_Without_Buffer >= tutorialSlowdownPositions[0] && tutText == null) {
			SlowCamera(tutorialSlowdownTextIDs[0]);
			tutorialSlowdownTextIDs.RemoveAt(0);
			tutorialSlowdownPositions.RemoveAt(0);
		}
		
		if (!Config.Instance.IsTutorial) {
			Config.Instance.PlayerDefautSpeed += 0.00001f;
		}
				
		if (!IsPaused) {
			
			if (Player.Instance.PausePressed && !IsPausedGame) {
				
				IsPaused = true;
				IsPausedGame = true;
				slowCamera = true;
				
				ShowPauseText();
			}
			
			if (LevelGenerator.Instance.IsWon) {
				Camera.main.GetComponent<Rigidbody2D>().velocity = CameraSpeed();
				
				if (Player.Instance.Speed.x + Player.Instance.Speed.y < 0.01f) {
					Player.Instance.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(CurrentBaseSpeed * 2.0f, 0.0f);
				} else {
					Player.Instance.gameObject.GetComponent<Rigidbody2D>().velocity = Player.Instance.Speed * CurrentBaseSpeed;
				}
				
				if (!Player.Instance.IsJesus()) {
					Player.Instance.changeShape(4);
				}

				if (Player.Instance.gameObject.transform.position.x > LeftCameraX + 2.5 * Camera.main.orthographicSize * Camera.main.aspect) {
					if (Config.Instance.IsTutorial) {
						Application.LoadLevel("Scenes/WinTutorialScene");
					} else {
						Application.LoadLevel("Scenes/WinScene");
					}
					
				}
				
			} else {
				Camera.main.GetComponent<Rigidbody2D>().velocity = CameraSpeed();
				Player.Instance.gameObject.GetComponent<Rigidbody2D>().velocity = Player.Instance.Speed * CurrentBaseSpeed;
			}	
		} else {
			Player.Instance.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
			Camera.main.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
			
			if (IsPausedGame && Player.Instance.PausePressed) {
				IsPaused = false;
				slowCamera = false;
				IsPausedGame = false;
				HidePauseText();
				
				CurrentBaseSpeed = Config.Instance.PlayerDefautSpeed;
			}
			
			if (Config.Instance.IsTutorial && Player.Instance.IsKeyDown(tutorialSlowdownKeys[0])) {
				IsPaused = false;
				slowCamera = false;
				HideTutorialText();
				
				CurrentBaseSpeed = Config.Instance.PlayerDefautSpeed;
				
				if (tutorialSlowdownShapeIDs[0] != -1) {
					Player.Instance.changeShape(tutorialSlowdownShapeIDs[0]);
				}
				
				tutorialSlowdownKeys.RemoveAt(0);
				tutorialSlowdownShapeIDs.RemoveAt(0);
			}
		}
		
		float cameraWidth = 2 * Camera.main.orthographicSize * Camera.main.aspect;
		progressBar.transform.localScale = new Vector3(cameraWidth * LevelGenerator.Instance.N_Game_Calls / Config.Instance.N_TotalTiles, progressBarBackground.transform.localScale.y, progressBar.transform.localScale.z);
		
		foreach (BoxCollider2D collider in Player.Instance.gameObject.GetComponents<BoxCollider2D>()) {
			if (!collider.isTrigger) {
				collider.enabled = !Player.Instance.IsJesus();
			}
		}
		
		if (LeftCameraX > lastUpdateX) {
			Visualize(LevelGenerator.Instance.Next());
			lastUpdateX += Config.Instance.TileWidth;
		}
		
		if (Player.Instance.gameObject.transform.position.x + 0.5f * Config.Instance.TileWidth < LeftCameraX) {
			if (Config.Instance.IsTutorial) {
				Application.LoadLevel("Scenes/DeathTutorialScene");
			} else {
				Application.LoadLevel("Scenes/DeathScene");
			}
			
		}
		
		if (slowCamera) {
			if (CurrentBaseSpeed > 0.01f) {
				CurrentBaseSpeed *= 0.95f;
			} else {
				IsPaused = true;
				CurrentBaseSpeed = 0.0f;
			}
		}
		else {
			CurrentBaseSpeed = Config.Instance.PlayerDefautSpeed;
		}
	}
}