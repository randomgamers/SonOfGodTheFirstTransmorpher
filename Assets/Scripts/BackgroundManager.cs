using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour {
	
	private int[] numbersOfTiles;
	private float[] lastXs;
	
	private float tileSize = 28.8f;

	private static BackgroundManager instance;
	public static BackgroundManager Instance {
		get {
			if (instance == null)
				instance =  GameObject.FindGameObjectWithTag("Global").GetComponent<BackgroundManager>();
			return instance;
		}
	}
	
	public void TileDeleted(int layer, float lastX) {
		numbersOfTiles[layer]--;
		lastXs[layer] = lastX;
	}

	void Start () {
		numbersOfTiles = new int[3];
		lastXs = new float[3];
		pregenerateTiles();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < 3; i++) {
			while(numbersOfTiles[i] < 4) {
				generateTile(i);
				numbersOfTiles[i]++;
			}
		}		
	}
	
	private void pregenerateTiles() {
		for(int i = 0; i < 3; i++) {
			for(int j = 0; j < 4; j++) {
				float tilePositionZ = 20.0f;
				switch(i){
					case 0:
						tilePositionZ = 18.0f;
						break;
					case 1:
						tilePositionZ = 19.0f;
						break;
					case 2:
						tilePositionZ = 20.0f;
						break;			
				}
				GameObject bcgTile = Instantiate(Resources.Load(string.Format("Prefabs/Background/bcg{0}", i+1))) as GameObject;
				bcgTile.transform.position = new Vector3(j*tileSize, 4.5f, tilePositionZ);
			}				
		}
		numbersOfTiles[0] = 4;
		numbersOfTiles[1] = 4;
		numbersOfTiles[2] = 4;
	}
	
	private void generateTile(int layer) {
		GameObject bcgTile = Instantiate(Resources.Load(string.Format("Prefabs/Background/bcg{0}", layer+1))) as GameObject;
		
		float tilePositionZ = 20.0f;
		switch(layer){
			case 0:
				tilePositionZ = 18.0f;
				break;
			case 1:
				tilePositionZ = 19.0f;
				break;
			case 2:
				tilePositionZ = 20.0f;
				break;			
		}
		bcgTile.transform.position = new Vector3(lastXs[layer]+4*tileSize, 4.5f, tilePositionZ);		
	}
	
}
