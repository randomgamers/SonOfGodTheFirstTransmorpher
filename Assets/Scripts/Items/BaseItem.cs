public class BaseItem
{
    public float StartedOn { get {return startedOn; } }
    public float Duration { get {return duration; } }
    public int ID { get {return id; } }
    public float SpeedFactor { get {return speedFactor; } }

    protected float startedOn;
    protected float duration;
    protected int id;
    protected float speedFactor = 1.0f;
    
    public BaseItem(float startedOn) {
        this.startedOn = StartedOn;
    }
    
    public bool IsInEffect(float currectPos) {
        return currectPos < startedOn + duration;
    }
    
    public void playSoundEffect() { }
}
