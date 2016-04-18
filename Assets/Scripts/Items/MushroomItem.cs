public class MushroomItem : BaseItem
{
    public MushroomItem(float startedOn): base(startedOn) {
        this.id = 3;
        this.duration = Config.Instance.MushroomDuration;
        speedFactor = Config.Instance.MushroomSpeedupFactor;
    }
    
    public void playSoundEffect() {
        SoundManager.Instance.Play(SoundManager.Instance.mushroom);
    }
}
