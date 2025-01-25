using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private BobaTeaManager _teaManager;
    private ScoreDisplay _scoreDisplay;
    private int _score = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(Instance);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        // Debug scoring by spawning a pearl of random type
        if (Input.GetKeyDown(KeyCode.D))
        {
            var bobaPearl = new BobaPearl();
            bobaPearl.Type = (BobaPearl.BobaType)Random.Range(0, 2);
            _teaManager.BobaSucked?.Invoke(bobaPearl);
        }
#endif
    }

    public void AssignBobaManager(BobaTeaManager manager)
    {
        if (manager == null)
            return;
        
        _teaManager = manager;
        _teaManager.BobaSucked += OnBobaSucked;
    }

    private void UnassignBobaManager()
    {
        _teaManager.BobaSucked -= OnBobaSucked;
        _teaManager = null;
    }

    public void AssignScoreDisplay(ScoreDisplay display)
    {
        if (display == null)
            return;

        _scoreDisplay = display;
    }

    private void OnBobaSucked(BobaPearl boba)
    {
        switch (boba.Type)
        {
            case BobaPearl.BobaType.GOOD:
                // TODO: hook score up to UI
                _score++;
                _scoreDisplay.ScoreUpdated?.Invoke(_score);
                break;

            case BobaPearl.BobaType.NASTY:
                break;
        }

        Debug.Log($"Score: {_score}", this);
    }
}
