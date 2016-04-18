using UnityEngine;

namespace Obstacles {
    public class BaseObstacle
    {
        // PRIVATE FIELDS
        public float X { get {return x; } }
        public float Y { get {return y; } }
        public int ID { get {return id; } }
        public ObstacleVerticalType VertType { get {return vertType; } }
        public bool IsEmpty { get {return isEmpty; } }
        protected float x;
        protected float y;
        protected int id;
        protected ObstacleVerticalType vertType;
        protected bool isEmpty;
        
        public Vector2 Position {
            get {
                return new Vector2(x, y);
            }
        }
        
        // CONSTRUCTOR
        public BaseObstacle(float x, ObstacleVerticalType vertType, int id)
        {
            this.x = x;
            // this.y = y;
            
            // this.x = x * GameManager.Instance.ScaleFactor;
            if (vertType == ObstacleVerticalType.BOTTOM) {
                this.y = Config.Instance.TileBottomHeight / 2.0f;
            } else if (vertType == ObstacleVerticalType.MIDDLE) {
                this.y = Config.Instance.TileBottomHeight + Config.Instance.TileMiddleHeight / 2.0f;
            } else {
                this.y = Config.Instance.TileBottomHeight + Config.Instance.TileMiddleHeight + Config.Instance.TileTopHeight / 2.0f;
            }
            // this.y *= GameManager.Instance.ScaleFactor;
            
            this.id = id;
            this.isEmpty = false;
        }
        
        public static BaseObstacle ConstructGroundByVerticalType(float x, ObstacleVerticalType vt) {
            if (vt == ObstacleVerticalType.BOTTOM) return new StoneObstacle(x);
            else if (vt == ObstacleVerticalType.MIDDLE) return new TreeObstacle(x);
            else return new CloudObstacle(x);
        }

        public static BaseObstacle ConstructWaterByVerticalType(float x, ObstacleVerticalType vt) {
            if (vt == ObstacleVerticalType.BOTTOM) return new MineObstacle(x);
            else if (vt == ObstacleVerticalType.MIDDLE) return new NoObstacle(x, ObstacleVerticalType.MIDDLE);
            else return new CloudObstacle(x);
        }
    }
}