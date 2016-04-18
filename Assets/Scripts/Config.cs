using UnityEngine;
using System.Collections;

public class Config : MonoBehaviour {

	[Range(0, 1000)] public float TileBottomHeight;
	[Range(0, 1000)] public float TileMiddleHeight;
	[Range(0, 1000)] public float TileTopHeight;
	[Range(0, 1000)] public float TileWidth;
	[Range(1, 20)] public int N_BlankTiles;
	[Range(1, 20)] public int N_BufferedTiles;
	[Range(1, 100000)] public int N_TotalTiles;
	public bool IsTutorial;
	public float PlayerDefautSpeed;
	[Range(0, 1)] public float EpsilonFactor;
	[Range(1, 500)] public int DefaultPlayerOffset;
	[Range(0, 1)] public float SpeedFaultFactor;
	[Range(0, 1)] public float P_1;
	[Range(0, 1)] public float P_2;
	[Range(0, 1)] public float P_3;
	[Range(0, 1)] public float P_ground2water;
	[Range(0, 1)] public float P_water2ground;
	[Range(0, 1)] public float P_waterObstacle;
	[Range(10, 500)] public float GroundThreshold;
	[Range(10, 500)] public float WaterThreshold;
	public float PlayerBottomY = 1.5f;
	public float PlayerMiddleY = 4.5f;
	public float PlayerTopY = 7.5f;
	[Range(0, 1)] public float P_can;
	[Range(0, 1)] public float P_mushroom;
	[Range(0, 1)] public float P_jesus;
	[Range(0, 10)] public float ShiftEasingFactor;
	[Range(0, 1)] public float WaterEnergyDiscountFactor;
	[Range(0, 1000)] public float JesusDuration;
	[Range(0, 5)] public float JesusSpeedupFactor;
	[Range(0, 1000)] public float EnergyDuration;
	[Range(0, 5)] public float EnergySpeedupFactor;
	[Range(0, 1000)] public float MushroomDuration;
	[Range(0, 5)] public float MushroomSpeedupFactor;

    private static Config instance;
	public static Config Instance {
		get {
			if (instance == null)
				instance = GameObject.FindGameObjectWithTag("Global").GetComponent<Config>();
			return instance;
		}
	}
	
	void Start () {}
	
	void Update () {}
}
