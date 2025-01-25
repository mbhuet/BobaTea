using System;
using UnityEngine;

public class BobaTeaManager : MonoBehaviour
{
    public BobaMix[] _bobaMixes;
    public BobaTeaCup _bobaTeaCup;

    public Action<BobaPearl> BobaSucked;

    private void Start()
    {
        Debug.Log("BobaTeaManager Start");
        GameManager.Instance.AssignBobaManager(this);
        _bobaTeaCup.TeaDepleted += OnTeaCompleted;
        _bobaTeaCup.SpawnMix(_bobaMixes[0]);
    }

    public void OnTeaCompleted()
    {

    }
}
