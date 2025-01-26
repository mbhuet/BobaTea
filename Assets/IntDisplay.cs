using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    public string label = "Score";

    public Action<int> ValueUpdated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void UpdateValue(int value)
    {
        _text.text = label + value;
    }
}
