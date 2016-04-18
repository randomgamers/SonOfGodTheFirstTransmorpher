using UnityEngine;
using Tuples;
using Obstacles;
using System;

public enum EnvironmentType
{
    GROUND, WATER    
}

public class NextResult
{
    public Tuple<EnvironmentType, bool, float, float> tile;
    public Tuple<BaseObstacle, BaseObstacle, BaseObstacle> obstacles;
    public Tuple<BaseItem, BaseItem, BaseItem> items;
}

public class LevelGenerator : MonoBehaviour {

    private static LevelGenerator instance;
	public static LevelGenerator Instance {
		get {
			if (instance == null)
				instance = (LevelGenerator)GameObject.FindObjectOfType(typeof(LevelGenerator));
			return instance;
		}
	}

    private int N_Calls = 0;
    public int N_Game_Calls {
        get {
            return N_Calls - (int) (Camera.main.orthographicSize * 2.0f * Camera.main.aspect / Config.Instance.TileWidth) - Config.Instance.N_BufferedTiles;
        }
    }
    
    public int N_Game_Calls_Without_Buffer {
        get {
            return N_Calls - (int) (Camera.main.orthographicSize * 2.0f * Camera.main.aspect / Config.Instance.TileWidth);
        }
    }
    
    public bool IsWon {
        get {
            return N_Game_Calls >= Config.Instance.N_TotalTiles; 
        }
    }
    private Tuple<BaseObstacle, BaseObstacle, BaseObstacle> last;

    private System.Random rand = new System.Random();

    private EnvironmentType environment = EnvironmentType.GROUND;

    private int sameEnvironmentDuration = 1;

    public NextResult Next()
    {
        if (Config.Instance.IsTutorial) { 
            NextResult result = new NextResult();
            result.obstacles = NextTutorialObstacles();
            result.tile = Tuple.Create(environment, sameEnvironmentDuration == 0, result.obstacles.Item1.X, result.obstacles.Item2.Y);
            result.items = Tuple.Create((BaseItem) new NoItem(), (BaseItem) new NoItem(), (BaseItem) new NoItem());

            return result;
        } else {
            NextResult result = new NextResult();
            result.obstacles = NextObstacles();
            result.tile = Tuple.Create(environment, sameEnvironmentDuration == 0, result.obstacles.Item1.X, result.obstacles.Item2.Y);
            result.items = GetItems(N_Calls * (Config.Instance.TileWidth-1), result.obstacles.Item1, result.obstacles.Item2, result.obstacles.Item3);

            return result;
        }
    }

    private Tuple<BaseObstacle, BaseObstacle, BaseObstacle> NextTutorialObstacles()
    {
        Config config = Config.Instance;
        
        if (N_Calls >= config.N_TotalTiles) {
            if (environment == EnvironmentType.GROUND && N_Calls == config.N_TotalTiles) {
                sameEnvironmentDuration = 0;
                environment = EnvironmentType.WATER;
            }
            else sameEnvironmentDuration++;
            
            return ObstacleLessNext();
        }
        
        sameEnvironmentDuration++;  
        if (N_Calls == 100){
            sameEnvironmentDuration = 0;
            environment = EnvironmentType.WATER;
        } else if (N_Calls == 145) {
            sameEnvironmentDuration = 0;
            environment = EnvironmentType.GROUND;
        }
        
        float nextX = N_Calls * config.TileWidth;
        
        BaseObstacle bottomObstacle = (Tutorial.BottomObstacles[N_Calls] == '.') ?
                                      (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.BOTTOM):
                                      (environment == EnvironmentType.GROUND) ? 
                                      BaseObstacle.ConstructGroundByVerticalType(nextX, ObstacleVerticalType.BOTTOM):
                                      BaseObstacle.ConstructWaterByVerticalType(nextX, ObstacleVerticalType.BOTTOM);
                            
        BaseObstacle middleObstacle = (Tutorial.MiddleObstacles[N_Calls] == '.') ?
                                      (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.MIDDLE):
                                      (environment == EnvironmentType.GROUND) ? 
                                      BaseObstacle.ConstructGroundByVerticalType(nextX, ObstacleVerticalType.MIDDLE):
                                      BaseObstacle.ConstructWaterByVerticalType(nextX, ObstacleVerticalType.MIDDLE);
                            
        BaseObstacle topObstacle = (Tutorial.TopObstacles[N_Calls] == '.') ?
                                   (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.TOP):
                                      (environment == EnvironmentType.GROUND) ? 
                                      BaseObstacle.ConstructGroundByVerticalType(nextX, ObstacleVerticalType.TOP):
                                      BaseObstacle.ConstructWaterByVerticalType(nextX, ObstacleVerticalType.TOP);
        N_Calls++;    
        return Tuple.Create(bottomObstacle, middleObstacle, topObstacle);
    }

    private Tuple<BaseObstacle, BaseObstacle, BaseObstacle> NextObstacles()
    {
        Config config = Config.Instance;
        
        if (N_Calls >= config.N_TotalTiles) {
            if (environment == EnvironmentType.GROUND && N_Calls == config.N_TotalTiles) {
                sameEnvironmentDuration = 0;
                environment = EnvironmentType.WATER;
            }
            else sameEnvironmentDuration++;
            
            return ObstacleLessNext();
        }

        if (N_Calls < config.N_BlankTiles) {
            return ObstacleLessNext();
        }
        
        if (environment == EnvironmentType.GROUND) {
            if (sameEnvironmentDuration > config.GroundThreshold) {
                if (rand.NextDouble() < config.P_ground2water*(sameEnvironmentDuration-config.GroundThreshold)) {
                    sameEnvironmentDuration = 0;
                    environment = EnvironmentType.WATER;
                    return ObstacleLessNext();
                }
            }
            sameEnvironmentDuration++;
            return NextGround();
        }
        else {
            if (sameEnvironmentDuration > config.WaterThreshold) {
                if (rand.NextDouble() < config.P_water2ground*(sameEnvironmentDuration-config.WaterThreshold)) {
                    sameEnvironmentDuration = 0;
                    environment = EnvironmentType.GROUND;
                    return ObstacleLessNext();
                }
            }
            sameEnvironmentDuration++;
            return NextWater();
        }
    }
    
    private Tuple<BaseObstacle, BaseObstacle, BaseObstacle> NextGround()
    {
        Config config = Config.Instance;
        float nextX = N_Calls * config.TileWidth;
        
        BaseObstacle[] obstacles = new BaseObstacle[3];
        obstacles[0] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.BOTTOM);
        obstacles[1] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.MIDDLE);
        obstacles[2] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.TOP);

        if (N_Calls > config.N_BlankTiles) {
            int n_empty = 0;
            int[] empties = {-1,-1,-1};
            int i = 0;
            
            foreach (BaseObstacle obstacle in last.ToArray()) {
                if (obstacle.IsEmpty) {
                    empties[n_empty] = i;
                    n_empty++;
                }
                i++;
            }
            
            if (n_empty == 0) {
                obstacles[0] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.BOTTOM);
                obstacles[1] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.MIDDLE);
                obstacles[2]  = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.TOP);
            } else if (n_empty == 1) {
                obstacles[empties[0]] = (BaseObstacle) new NoObstacle(nextX, (ObstacleVerticalType) empties[0]);
                for (int j=0; j<3; j++) {
                    if (j != empties[0]) {
                        if (rand.NextDouble() < config.P_1) {
                            obstacles[j] = BaseObstacle.ConstructGroundByVerticalType(nextX, (ObstacleVerticalType) j);
                        } else {
                            obstacles[j] = (BaseObstacle) new NoObstacle(nextX, (ObstacleVerticalType) j);
                        }
                       
                    }
                }
            } else if (n_empty == 2) {
                obstacles[empties[0]] = (BaseObstacle) new NoObstacle(nextX, (ObstacleVerticalType) empties[0]);
                obstacles[empties[1]] = (BaseObstacle) new NoObstacle(nextX, (ObstacleVerticalType) empties[1]);

               for (int j=0; j<3; j++) {
                    if (j != empties[0] && j != empties[1]) {
                        if (rand.NextDouble() < config.P_2) {
                            obstacles[j] = BaseObstacle.ConstructGroundByVerticalType(nextX, (ObstacleVerticalType) j);
                        } else {
                            obstacles[j] = (BaseObstacle) new NoObstacle(nextX, (ObstacleVerticalType) j);
                        }
                       
                    }
                }                
            } else if (n_empty == 3) {
                int k = rand.Next(3);
                obstacles[k] = (BaseObstacle) new NoObstacle(nextX, (ObstacleVerticalType) k);
                for (int j=0; j<3; j++) {
                    if (j != k) {
                       if (rand.NextDouble() < config.P_3) {
                            obstacles[j] = BaseObstacle.ConstructGroundByVerticalType(nextX, (ObstacleVerticalType) j);
                        } else {
                            obstacles[j] = (BaseObstacle) new NoObstacle(nextX, (ObstacleVerticalType) j);
                        }
                    }
                }
                
            }
        }
        
        N_Calls++;
        Tuple<BaseObstacle, BaseObstacle, BaseObstacle> next_round = Tuple.Create(obstacles[0], obstacles[1], obstacles[2]);
        last = next_round;
        return next_round;
    }
    private int lastWaterFree = 0;
    private Tuple<BaseObstacle, BaseObstacle, BaseObstacle> NextWater()
    {
        Config config = Config.Instance;
        float nextX = N_Calls * config.TileWidth;
        
        BaseObstacle[] obstacles = new BaseObstacle[3];
        obstacles[0] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.BOTTOM);
        obstacles[1] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.MIDDLE);
        obstacles[2] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.TOP);

        int free = lastWaterFree;
        int other = 1 - free;
        free += Math.Sign(free);
        other += Math.Sign(other);

        if (rand.NextDouble() < config.P_waterObstacle) {
            obstacles[other] = BaseObstacle.ConstructWaterByVerticalType(nextX, (ObstacleVerticalType) other);
        } else {
            if (rand.Next(2) == 1) {
                lastWaterFree = 1 - lastWaterFree;
            }
        }

        N_Calls++;
        Tuple<BaseObstacle, BaseObstacle, BaseObstacle> next_round = Tuple.Create(obstacles[0], obstacles[1], obstacles[2]);
        last = next_round;
        return next_round;
    }
    
    private Tuple<BaseObstacle, BaseObstacle, BaseObstacle> ObstacleLessNext()
    {
        Config config = Config.Instance;
        float nextX = N_Calls * config.TileWidth;
        
        BaseObstacle[] obstacles = new BaseObstacle[3];
        obstacles[0] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.BOTTOM);
        obstacles[1] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.MIDDLE);
        obstacles[2] = (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.TOP);

        N_Calls++;
        Tuple<BaseObstacle, BaseObstacle, BaseObstacle> next_round = Tuple.Create((BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.BOTTOM),
                                                                                  (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.MIDDLE),
                                                                                  (BaseObstacle) new NoObstacle(nextX, ObstacleVerticalType.TOP));
        last = next_round;
        return next_round;
    }    
    
    private Tuple<BaseItem, BaseItem, BaseItem> GetItems(float startedOn, BaseObstacle bottomObstacle, BaseObstacle middleObstacle, BaseObstacle topObstacle)
    {
        Config conf = Config.Instance;
        BaseItem bottom = (BaseItem) new NoItem();
        BaseItem middle = (BaseItem) new NoItem();
        BaseItem top = (BaseItem) new NoItem();
        
        if (N_Calls > Config.Instance.N_TotalTiles) {
            return Tuple.Create(bottom, middle, top);
        }
        
        if (bottomObstacle.IsEmpty) {
            float factor = (environment == EnvironmentType.WATER) ? conf.WaterEnergyDiscountFactor : 1.0f;
            if (rand.NextDouble() < factor*conf.P_can) {
                bottom = (BaseItem) new EnergyItem(startedOn);
            }
        }
        if (middleObstacle.IsEmpty && environment != EnvironmentType.WATER) {
            if (rand.NextDouble() < conf.P_mushroom) {
                middle = (BaseItem) new MushroomItem(startedOn);
            }
        }
        if (topObstacle.IsEmpty) {
            if (rand.NextDouble() < conf.P_jesus) {
                top = (BaseItem) new JesusItem(startedOn);
            }
        }
        
        return Tuple.Create(bottom, middle, top);
    }
    
	void Start () {}
	void Update () {}
}
