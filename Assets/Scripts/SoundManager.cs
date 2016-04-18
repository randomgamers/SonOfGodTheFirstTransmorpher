using UnityEngine;

public class SoundManager : MonoBehaviour {
	public AudioSource efxSource;
	public AudioSource playerSource;
	public AudioSource musicSource;
	public AudioClip menuMusic;
	public AudioClip winMusic;
	public AudioClip deathMusic;
	public AudioClip gameMusic;
	public AudioClip jesusBackgroundMusic;
	public AudioClip energy;
	public AudioClip mushroom;
	public AudioClip jesus;
	public AudioClip mineHit;
	public AudioClip treeHit;
	public AudioClip cloudHit;
	public AudioClip stoneHit;
	public AudioClip birdSound;
	public AudioClip manSound;
	public AudioClip jesusSound;
	public AudioClip jellyfishSound;
	public AudioClip wormSound;

	private static SoundManager instance;
	public static SoundManager Instance {
		get {
			if (instance == null)
				instance = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
			return instance;
		}
	}

	public void Play(AudioClip clip)
	{
        if (Player.Instance.IsJesus()) return;
		efxSource.clip = clip;
		efxSource.pitch = Random.Range(0.95f, 1.05f);
		efxSource.Play();
	}
    
    public void SetNewPlayerSound(AudioClip newSound)
    {
        playerSource.Stop();
        playerSource.clip = newSound;
        playerSource.Play();
    }
}
