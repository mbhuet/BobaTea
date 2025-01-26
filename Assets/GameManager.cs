using DG.Tweening;
using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private enum State
    {
        COMIC_INTRO, //comic panels
        GAME,
        RESULTS,
    }

    private State currentState;

    public static GameManager Instance;

    public BobaTeaManager _teaManager;
    public IntDisplay _scoreDisplay;
    public IntDisplay _timeDisplay;

    private int _score = 0;
    private float _time = 0;

    public float _countdownTime = 60f;

    public BobaTeaManager TeaManager => _teaManager;

    public Transform _uiPanelRoot;
    public Image[] _comicPanels;
    public float _comicPanelDelayTime = 2f;
    public float _comicPanelFadeTime = .2f;
    public SpriteRenderer _cowgirlSprite;
    public DelilahStateMachine _cowgirlStateMachine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        //DontDestroyOnLoad(Instance);

        _teaManager.BobaSucked += OnBobaSucked;

        ResetUI();
    }


    private void ResetUI()
    {
        _uiPanelRoot.transform.localPosition = Vector3.left * 500;
        for (int comicIndex = 0; comicIndex < _comicPanels.Length; comicIndex++)
        {
            _comicPanels[comicIndex].gameObject.SetActive(false);
        }
        _cowgirlSprite.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetState(State.COMIC_INTRO);
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
        UpdateState();

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case State.COMIC_INTRO:
                break;
            case State.GAME:
                _time -= Time.deltaTime;
                _timeDisplay.UpdateValue(Mathf.RoundToInt(_time));
                if(_time <= 0)
                {
                    SetState(State.RESULTS);
                }
                break;
            case State.RESULTS:
                break;
        }
    }

    private void OnBobaSucked(BobaPearl boba)
    {
        switch (boba.Type)
        {
            case BobaPearl.BobaType.GOOD:
                _score++;
                _scoreDisplay.UpdateValue(_score);
                break;

            case BobaPearl.BobaType.NASTY:
                break;
        }

        //Debug.Log($"Score: {_score}", this);
    }

    private void SetState(State newState)
    {
        //Exit old state
        switch (currentState)
        {
            case State.COMIC_INTRO:
                break;
            case State.GAME:
                _teaManager.TeaRefilled -= OnTeaRefilled;
                _teaManager.EndTeaService();
                break;
            case State.RESULTS:
                break;
        }

        //Begin new state
        switch (newState)
        {
            case State.COMIC_INTRO:
                StartCoroutine(IntroRoutine());
                break;
            case State.GAME:
                _teaManager.TeaRefilled += OnTeaRefilled;
                _teaManager.BeginTeaService();
                _time = _countdownTime;
                break;
            case State.RESULTS:
                break;
        }
        currentState = newState;
    }

    private IEnumerator IntroRoutine()
    {
        yield return DelayOrClickToAdvance();

        //fade in comic panels one at a time
        for (int comicIndex = 0; comicIndex < _comicPanels.Length; comicIndex++)
        {
            Image comic = _comicPanels[comicIndex];
            comic.gameObject.SetActive(true);
            comic.color = Color.clear;
            comic.DOColor(Color.white, _comicPanelFadeTime);
            yield return DelayOrClickToAdvance();
        }

        //fade in cowgirl
        _cowgirlSprite.gameObject.SetActive(true);
        _cowgirlStateMachine.SetDelilahState(DelilahStateMachine.DelilahState.Default);
        _cowgirlSprite.color = Color.clear;
        _cowgirlSprite.DOColor(Color.white, _comicPanelFadeTime);

        yield return DelayOrClickToAdvance();


        for (int comicIndex = 0; comicIndex < _comicPanels.Length; comicIndex++)
        {
            Image comic = _comicPanels[comicIndex];
            DOTween.Kill(comic);
            comic.DOColor(Color.clear, _comicPanelFadeTime);
        }

        _uiPanelRoot.DOMoveX(0, 1).SetEase(Ease.InOutBack);

        yield return DelayOrClickToAdvance();


        SetState(State.GAME);
        yield return null;
    }


    private IEnumerator DelayOrClickToAdvance()
    {
        float t = 0;
        while (t < _comicPanelDelayTime && !ClickToAdvance())
        {
            t += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    private bool ClickToAdvance()
    {
        return Input.GetMouseButtonDown(0);
    }

    private void OnTeaRefilled()
    {
        _cowgirlStateMachine.SetDelilahState(DelilahStateMachine.DelilahState.Celebratory);
    }
}
