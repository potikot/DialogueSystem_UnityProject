using UnityEngine;
using System;
using PotikotTools.UniTalks.Demo;

public class ClockTrigger : MonoBehaviour, IActionTrigger
{
    [Header("Clock Hands")]
    [SerializeField] private Transform hourHand;
    [SerializeField] private  Transform minuteHand;
    [SerializeField] private  Transform secondHand;

    [Header("Time Control")]
    [SerializeField] private float timeScale = 1f;

    private DateTime initialSystemTime;
    private float elapsedTime;

    public float TimeScale
    {
        get => timeScale;
        set => timeScale = value;
    }

    private void Awake()
    {
        initialSystemTime = DateTime.Now;
        elapsedTime = 0f;
        G.ClockActionTrigger = this;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime * timeScale;
        DateTime currentTime = initialSystemTime.AddSeconds(elapsedTime);

        float hour = currentTime.Hour % 12;
        float minute = currentTime.Minute;
        float second = currentTime.Second + currentTime.Millisecond / 1000f;

        float secondAngle = 6f * second;
        float minuteAngle = 6f * (minute + second / 60f);
        float hourAngle = 30f * (hour + minute / 60f);

        secondHand.localRotation = Quaternion.Euler(0f, 90f, secondAngle);
        minuteHand.localRotation = Quaternion.Euler(0f, 90f, minuteAngle);
        hourHand.localRotation = Quaternion.Euler(0f, 90f, hourAngle);
    }

    public void Trigger()
    {
        TimeScale *= -1f;
    }
}