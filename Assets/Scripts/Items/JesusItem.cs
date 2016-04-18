public class JesusItem : BaseItem
{
    public JesusItem(float startedOn): base(startedOn) {
        this.id = 1;
        this.duration = Config.Instance.JesusDuration;
        speedFactor = Config.Instance.JesusSpeedupFactor;
    }
    
    public void playSoundEffect() {
        SoundManager.Instance.Play(SoundManager.Instance.jesus);
    }
}
