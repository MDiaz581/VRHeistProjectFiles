using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stopwatch : MonoBehaviour
{
    public bool timerStop;
    Text timerText;
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        timerStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!timerStop)
        {
            timer += Time.deltaTime;
        }
        if (timer <= 0)
        {
        }

        timerText.text = timer.ToString("00");
    }
}
