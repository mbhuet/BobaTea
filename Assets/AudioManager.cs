using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip _bobaSuckSound;
    public AudioClip _emptyCupSound;
    public AudioClip _freshTeaPourSound;
    public AudioClip _disgustedSound;
    public AudioClip _celebrationSound;
    public AudioClip _glassFilled;
    
    private void Awake()
    {
        Instance = this;   
    }
}
