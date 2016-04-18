public class EnergyItem : BaseItem
{
    public EnergyItem(float startedOn): base(startedOn) {
        this.id = 2;
        this.duration = Config.Instance.EnergyDuration;
        speedFactor = Config.Instance.EnergySpeedupFactor;
    }
    
    public void playSoundEffect() {
        SoundManager.Instance.Play(SoundManager.Instance.energy);
    }
}
