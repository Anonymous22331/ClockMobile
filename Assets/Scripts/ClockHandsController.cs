using System;
using UnityEngine;
using UnityEngine.UI;

public class ClockHandsController : MonoBehaviour
{
    public bool IsAlarmActive { get; private set; } = false;
    public bool IsClockSetToZero { get; private set; } = false;

    public DateTime CurrentTime { get; private set; }
    private float _timeCheckoutCooldown = 0f;
    
    [SerializeField] private Transform _hourHand;
    [SerializeField] private Transform _minuteHand;
    [SerializeField] private Transform _secondHand;

    [SerializeField] private Slider _meridemSlider;
    
    [SerializeField] private Text _numericalTimeText;
    [SerializeField] private TimeSynchronizer _timeSynchronizer;
        
    private void Update()
    {
        if (_timeCheckoutCooldown <= 0)
        {
            _timeSynchronizer.SynchronizeTime();
            if (_timeSynchronizer.IsTimeSynchronized)
            {
                CurrentTime = _timeSynchronizer.GetSynchronizedTime();
                _timeCheckoutCooldown = 3600;
            }
            
        }

        if (!IsAlarmActive)
        {
            SetRotation(_hourHand, (CurrentTime.Hour % 12 + CurrentTime.Minute / 60f) * 30f);
            SetRotation(_minuteHand, (CurrentTime.Minute + CurrentTime.Second / 60f) * 6f);
            SetRotation(_secondHand, CurrentTime.Second * 6f);
            if (CurrentTime.Hour > 12)
            {
                _meridemSlider.value = 1;
            }
            else
            {
                _meridemSlider.value = 0;
            }
        }
        else
        {
            if (!IsClockSetToZero)
            {
                SetRotation(_hourHand, 0);
                SetRotation(_minuteHand, 0);
                SetRotation(_secondHand, 0);
                IsClockSetToZero = true;
            }
        }

        
        _numericalTimeText.text = CurrentTime.Hour + ":" + CurrentTime.Minute + ":" + CurrentTime.Second;
        CurrentTime += TimeSpan.FromSeconds(Time.deltaTime);
        _timeCheckoutCooldown -= Time.deltaTime;
    }
    private void SetRotation(Transform hand, float angle)
    {
        hand.localRotation = Quaternion.Euler(0, 0, -angle);  // Negative angle for clockwise rotation
    }

    public void ToggleAlarm()
    {
        IsAlarmActive = !IsAlarmActive;
    }

    public void ToggleClockHands()
    {
        IsClockSetToZero = !IsClockSetToZero;
    }
}
