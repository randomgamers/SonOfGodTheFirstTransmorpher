using UnityEngine;

public class Destroyer : MonoBehaviour {

	void Start () {	}
	
	void Update () {
		if (gameObject.transform.position.x + Config.Instance.TileWidth < GameManager.Instance.LeftCameraX) {
			Destroy(gameObject);
		}
	}
}
