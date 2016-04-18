using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour {
	
	public float size = 28.8f;
	public float speed;
	public int type;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.transform.position.x + size < GameManager.Instance.LeftCameraX) {
			BackgroundManager.Instance.TileDeleted(type, gameObject.transform.position.x);
			Destroy(gameObject);	
		} else {
			float cameraSpeed = Camera.main.GetComponent<Rigidbody2D>().velocity.x;
			
			float deltaX = cameraSpeed * speed;
			
			Vector3 pos = gameObject.transform.position;
			pos.x += deltaX;
			gameObject.transform.position = pos;
		}
	}
}
