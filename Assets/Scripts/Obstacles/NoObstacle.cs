namespace Obstacles {
    public class NoObstacle : BaseObstacle
    {
        // CONSTRUCTOR
        public NoObstacle(float x, ObstacleVerticalType vertType) : base(x, vertType, 0)
        {
             this.isEmpty = true;
        }
    }
}