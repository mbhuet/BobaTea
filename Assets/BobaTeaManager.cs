using UnityEngine;

public class BobaTeaManager : MonoBehaviour
{
    public BobaMix[] _bobaMixes;
    public BobaTeaCup _bobaTeaCup;

    private void Start()
    {
        Debug.Log("BobaTeaManager Start");
        _bobaTeaCup.TeaDepleted += OnTeaCompleted;
        _bobaTeaCup.SpawnMix(_bobaMixes[0]);
    }

    public void OnTeaCompleted()
    {

    }
}
