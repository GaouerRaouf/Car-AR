using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script calculate the current fps and show it to a text ui.
/// </summary>

namespace Imagine.WebAR.Samples
{
    public class DemoFPSCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fpsText;

        public float updateRateSeconds = 4.0F;

        int frameCount = 0;
        float dt = 0f;
        float fps = 0f;

        void Update()
        {
            frameCount++;
            dt += Time.unscaledDeltaTime;
            if (dt > 1f / updateRateSeconds)
            {
                fps = frameCount / dt;
                frameCount = 0;
                dt -= 1f / updateRateSeconds;
            }
            fpsText.text = System.Math.Round(fps, 1).ToString("0.0");
        }
    }
}
