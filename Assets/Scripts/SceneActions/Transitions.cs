using UnityEngine;

public class Transitions : MonoBehaviour {
	public void Awake() {
		UnityEngine.Cursor.visible = true;
	}
	public void Go2Exit()
	{
		Application.Quit();
	}
	
	public void Go2Game()
	{
		Application.LoadLevel("Scenes/GameScene");
	}
	
	public void Go2Menu()
	{
		Application.LoadLevel("Scenes/MenuScene");
	}
	
	public void Go2Tutorial()
	{
		Application.LoadLevel("Scenes/TutorialScene");
	}
}