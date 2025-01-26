using DG.Tweening;
using NUnit.Framework.Constraints;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DelilahStateMachine;

public class GameManager : MonoBehaviour
{
    private enum State
    {
        COMIC_INTRO, //comic panels
        GAME,
        RESULTS,
    }

    private enum Emote
    {
        STUN,
        CELEBRATE,
        DRINK,
    }

    private State currentState;

    public static GameManager Instance;

    public BobaTeaManager _teaManager;
    public IntDisplay _scoreDisplay;
    public IntDisplay _timeDisplay;

    private int _score = 0;
    private float _time = 0;
    private int _refills = 0;
    public float _countdownTime = 60f;

    public BobaTeaManager TeaManager => _teaManager;

    public Transform _uiPanelRoot;
    public Canvas _resultsCanvas;
    public Image[] _comicPanels;
    public Image _resultsBlackout;
    public Image _outroComic;
    public GameObject _outroButtonPanel;
    public TMPro.TextMeshProUGUI _resultsText;
    public float _comicPanelDelayTime = 2f;
    public float _comicPanelFadeTime = .2f;
    public SpriteRenderer _cowgirlSprite;
    public DelilahStateMachine _cowgirlStateMachine;

    private Coroutine _cowgirlEmoteRoutine;

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

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }


    private void ResetUI()
    {
        _uiPanelRoot.transform.localPosition = Vector3.left * 500;

        for (int comicIndex = 0; comicIndex < _comicPanels.Length; comicIndex++)
        {
            _comicPanels[comicIndex].gameObject.SetActive(false);
        }
        _cowgirlSprite.gameObject.SetActive(false);
        _resultsCanvas.gameObject.SetActive(false);
        _outroButtonPanel.gameObject.SetActive(false);
        _resultsText.color = Color.clear;

        _resultsBlackout.color = Color.clear;
        _outroComic.color = Color.clear;
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
        if (Input.GetKeyDown(KeyCode.T))
        {
            _time = 0;
        }
#endif
        UpdateState();

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }

    #region STATE MACHINE
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
        currentState = newState;

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
                _refills = 0;
                break;
            case State.RESULTS:
                StartCoroutine(ResultsRoutine());
                break;
        }
    }
    #endregion

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

    private IEnumerator ResultsRoutine()
    {
        _resultsText.text = string.Format("RESULTS\n\n{0} Bobas \n{1} Refills\n\nclick to continue", _score, _refills);
        DoEmote(Emote.CELEBRATE);

        //_resultsPanel.DOMoveX(0, 1).SetEase(Ease.InOutBack);
        _resultsCanvas.gameObject.SetActive(true);
        _resultsBlackout.color = Color.clear;
        _resultsBlackout.DOColor(Color.black, _comicPanelFadeTime);

        yield return DelayOrClickToAdvance();

        _resultsText.DOColor(Color.white, _comicPanelFadeTime);
        _resultsText.transform.localScale = Vector3.one * 2f;
        _resultsText.transform.DOScale(1, .2f);

        yield return DelayOrClickToAdvance();
        _resultsBlackout.color = Color.black;
        _resultsText.color = Color.white;

        while (!ClickToAdvance())
        {
            yield return null;
        }


        _cowgirlSprite.DOColor(Color.clear, _comicPanelFadeTime);
        _outroComic.color = Color.clear;
        _outroComic.DOColor(Color.white, _comicPanelFadeTime);

        yield return DelayOrClickToAdvance();

        _outroComic.color = Color.white;
        _outroButtonPanel.gameObject.SetActive(true);
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
        DoEmote(Emote.CELEBRATE, 2);
    }

    private void StopEmote()
    {
        if (_cowgirlEmoteRoutine != null)
        {
            StopCoroutine(_cowgirlEmoteRoutine);
            _teaManager.SetInteractable(currentState == State.GAME);
            DOTween.Kill(_cowgirlSprite);
        }
    }

    private void DoEmote(Emote emote, float seconds = -1)
    {
        StopEmote();
        switch (emote)
        {
            case Emote.STUN:
                _cowgirlEmoteRoutine = StartCoroutine(StunRoutine(seconds));
                break;
            case Emote.CELEBRATE:
                _cowgirlEmoteRoutine = StartCoroutine(CelebrationRoutine(seconds));
                break;
            case Emote.DRINK:
                _cowgirlEmoteRoutine = StartCoroutine(DrinkingRoutine(seconds));
                break;
        }
    }

    private IEnumerator StunRoutine(float seconds)
    {
        if (AudioManager.Instance._disgustedSound)
        {
            AudioSource.PlayClipAtPoint(AudioManager.Instance._disgustedSound, Camera.main.transform.position);
        }
        _cowgirlStateMachine.SetDelilahState(DelilahStateMachine.DelilahState.Stunned);
        _teaManager.SetInteractable(false);
        _cowgirlSprite.transform.DOShakePosition(seconds, 2);
        yield return new WaitForSeconds(seconds);
        _teaManager.SetInteractable(true);
        _cowgirlStateMachine.SetDelilahState(DelilahStateMachine.DelilahState.Default);
    }

    private IEnumerator CelebrationRoutine(float seconds)
    {
        _cowgirlStateMachine.SetDelilahState(DelilahStateMachine.DelilahState.Celebratory);
        if(seconds > 0)
        {
            yield return new WaitForSeconds(seconds);
            _cowgirlStateMachine.SetDelilahState(DelilahStateMachine.DelilahState.Default);
        }
        yield return null;
    }

    private IEnumerator DrinkingRoutine(float seconds)
    {
        _cowgirlStateMachine.SetDelilahState(DelilahStateMachine.DelilahState.Drinking);
        if (seconds > 0)
        {
            yield return new WaitForSeconds(seconds);
            _cowgirlStateMachine.SetDelilahState(DelilahStateMachine.DelilahState.Default);
        }
        yield return null;
    }

    private void OnBobaSucked(BobaPearl boba)
    {
        switch (boba.Type)
        {
            case BobaPearl.BobaType.GOOD:
                _score++;
                _scoreDisplay.UpdateValue(_score);
                DoEmote(Emote.DRINK);
                break;

            case BobaPearl.BobaType.NASTY:
                DoEmote(Emote.STUN, 2);
                break;
        }
    }
}
