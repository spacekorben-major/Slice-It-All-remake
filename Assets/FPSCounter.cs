using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _ui;

    private float _duration = 0f;

    private int _count = 0;
    void Update()
    {
        if (_count < 10)
        {
            _count++;
            _duration += Time.deltaTime;
        }
        else
        {
            _ui.text = $"{Mathf.Floor(10 / _duration).ToString()}";
            _count = 0;
            _duration = 0;
        }
    }
}
