using UnityEngine;
using System.Threading;
using System.Collections;
public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;
    int MaxRate = 9999;
    public float TargetFrameRate = 60.0f;
    float currentFrameTime;
    void Awake(){
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = MaxRate;
        StartCoroutine("WaitForNextFrame");
    }

    IEnumerator WaitForNextFrame(){
        while(true){
            yield return new WaitForEndOfFrame();
            currentFrameTime += 1.0f / TargetFrameRate;
            var t = Time.realtimeSinceStartup;
            var sleepTime = currentFrameTime - t - 0.01f;
            if(sleepTime>0)
            Thread.Sleep((int)(sleepTime * 1000));
            while(t< currentFrameTime)
            t= Time.realtimeSinceStartup;

        }
    }
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
