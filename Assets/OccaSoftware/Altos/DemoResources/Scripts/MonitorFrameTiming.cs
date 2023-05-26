using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OccaSoftware.Altos.Demo
{

    /// <summary>
    /// Monitors and displays your application's frame timing (in milliseconds).
    /// </summary>
    public class MonitorFrameTiming : MonoBehaviour
    {
        private float tFirst;
        private float tLast;
        private float trailingT;


        private void Start()
        {
            Setup();
        }

        private void Update()
        {
            if(!IsTooEarly())
                UpdateTrailingTime();
        }

        private void OnGUI()
        {
            DrawFrameTimingOnScreen();
        }

        /// <summary>
        /// Initializes variables on startup.
        /// </summary>
        private void Setup()
        {
            tFirst = Time.time;
            tLast = Time.time;
            trailingT = 0.0166f;
        }

        /// <summary>
        /// Checks if the application started recently. We don't want to corrupt our trailing performance data with application startup data.
        /// </summary>
        private bool IsTooEarly()
		{
            if (Time.time - tFirst < 2f)
                return true;

            return false;
        }

        /// <summary>
        /// Updates the trailing time variable.
        /// </summary>
        private void UpdateTrailingTime()
		{
            float tDelta = Time.time - tLast;
            trailingT = Mathf.Lerp(trailingT, tDelta, 0.002f);
            tLast = Time.time;
        }

        /// <summary>
        /// Draws the calculated trailing Frame Timing on the screen.
        /// </summary>
        private void DrawFrameTimingOnScreen()
		{
            GUIStyle s = GUI.skin.GetStyle("label");
            s.fontSize = 22;
            float msTiming = trailingT * 1000f;
            int w = Screen.width / 10;
            int h = Screen.height / 10;
            GUI.Label(new Rect(w, h, Screen.width, Screen.height), $"{msTiming:0.00}ms");
        }
    }
}