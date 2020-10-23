using System;
using UnityEngine;
using TMPro;

namespace malvra
{
    /// <summary>
    /// This class relates to the timer attached to every activity
    /// an operator can do:
    ///   > "Siembra en cama"
    ///   > "Siembra en banca"
    ///   > "Corte"
    ///   > "Desbotonar"
    /// It's purpose is to measure the operator's performance and
    /// store it in a data base in a remote server.
    /// </summary>
    public class ActivityTimer : MonoBehaviour
    {
        private DateTime startTime;
        float timer;
        float seconds;
        float minutes;
        float hours;

        bool start;

        private TMP_Text activityTimerText;

        public void StartTimer()
        {
            start = true;
            startTime = DateTime.Now;
        }

        void TimerIncrement()
        {
            if (start)
            {
                timer += Time.deltaTime;
                seconds = (int)(timer % 60);
                minutes = (int)((timer / 60) % 60);
                hours = (int)(timer / 3600);

                activityTimerText.text = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
            }
        }

        public void StopTimer()
        {
            start = false;
        }

        public void ResetTimer()
        {
            start = false;
            timer = 0;
            activityTimerText.text = "00:00:00";
        }

        public void Finish()
        {
            // The purpose of this was to send data to a remote server
            start = false;
        }

        void Start()
        {
            activityTimerText = GameObject.Find("/SceneContent/CommandPanel/Time").GetComponent<TMP_Text>();
            Debug.Log(activityTimerText.text);
            timer = 0;
            start = false;
        }

        void Update()
        {
            TimerIncrement();
        }
    }
}
