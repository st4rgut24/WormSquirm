using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Stats : MonoBehaviour
{
    public TextMeshProUGUI StatsText;

    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps;



    void Start()
    {
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        for (; ; )
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it

            StatsText.text = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
            //fps = 
        }
    }


    //void OnGUI()
    //{
    //    GUI.Label(new Rect(Screen.width - 100, 10, 150, 20), fps);
    //}
}
