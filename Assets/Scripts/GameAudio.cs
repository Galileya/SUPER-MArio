using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }
    private AudioSource music, effects;
    private void Awake()
    {
        Instance = this;
        music = gameObject.AddComponent<AudioSource>(); effects = gameObject.AddComponent<AudioSource>();
        music.loop = true; music.volume = .22f; effects.volume = .55f;
    }
    public void SetTheme(string scene)
    {
        music.Stop();
        string theme = scene == "1-2" ? "underground" : scene == "1-3" ? "water" : scene == "1-4" ? "castle" : "overworld";
        if (scene == "GameOver" || scene == "Victory") { Play(scene == "Victory" ? "clear" : "gameover"); return; }
        music.clip = Resources.Load<AudioClip>("Audio/" + theme); music.Play();
    }
    public void Play(string sound)
    {
        if (sound == "death" || sound == "clear" || sound == "gameover") music.Stop();
        var clip = Resources.Load<AudioClip>("Audio/" + sound);
        if (clip != null) effects.PlayOneShot(clip);
    }
}
