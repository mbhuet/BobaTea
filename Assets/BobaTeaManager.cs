using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class BobaTeaManager : MonoBehaviour
{
    public BobaMix[] _bobaMixes;
    public BobaTeaCup _bobaTeaCup;
    public BobaStraw _straw;

    public Action<BobaPearl> BobaSucked;
    public Transform _offScreenTarget;
    public Transform _onScreenTarget;

    public Action OnTeaEmpty;
    public Action TeaRefilled;

    private bool _isServingTea;
    private bool _isInteractable;

    private void Start()
    {
        Debug.Log("BobaTeaManager Start");
        _bobaTeaCup.TeaDepleted += OnTeaCompleted;
        _bobaTeaCup.gameObject.SetActive(false);
    }

    public void OnTeaCompleted()
    {
        RemoveEmptyTea();
    }

    public void BeginTeaService()
    {
        _isServingTea = true;
        ServeFreshTea();
    }

    public void EndTeaService()
    {
        _isServingTea = false;
        _straw.SetInteractable(false);
    }

    private BobaMix GetRandomMix()
    {
        return _bobaMixes[UnityEngine.Random.Range(0, _bobaMixes.Length)];
    }

    public void ServeFreshTea()
    {
        _bobaTeaCup.gameObject.SetActive(true);
        _bobaTeaCup.transform.position = _offScreenTarget.position;
        _bobaTeaCup.Clean();
        _bobaTeaCup.Refill();
        _bobaTeaCup.SpawnMix(GetRandomMix());
        StartCoroutine(ServeFreshTeaRoutine());
        if (TeaRefilled != null) TeaRefilled.Invoke();
        if (AudioManager.Instance._freshTeaPourSound)
        {
            AudioSource.PlayClipAtPoint(AudioManager.Instance._freshTeaPourSound, Camera.main.transform.position);
        }
    }

    private IEnumerator ServeFreshTeaRoutine()
    {
        _bobaTeaCup.transform.DOMove(_onScreenTarget.position, 1).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1);
        _straw.SetInteractable(true);
    }

    public void RemoveEmptyTea()
    {
        _straw.SetInteractable(false);
        if (AudioManager.Instance._emptyCupSound != null)
        {
            AudioSource.PlayClipAtPoint(AudioManager.Instance._emptyCupSound, Camera.main.transform.position);
        }

        StartCoroutine(RemoveEmptyTeaRoutine());
    }

    private IEnumerator RemoveEmptyTeaRoutine()
    {
        _bobaTeaCup.transform.DOShakeRotation(1, 30);
        yield return new WaitForSeconds(1);
        _bobaTeaCup.transform.DOMove(_offScreenTarget.position, 1).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(1);
        _bobaTeaCup.gameObject.SetActive(false);

        if (_isServingTea)
        {
            ServeFreshTea();
        }
    }

    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;
        _straw.SetInteractable(interactable);
    }
}
