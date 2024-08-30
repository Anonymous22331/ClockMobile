using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AlarmController : MonoBehaviour
{
    public DateTime AlarmTime { get; private set; }

    private float _totalTime;
    
    [SerializeField] private InputField _hoursField;
    [SerializeField] private InputField _minutesField;
    [SerializeField] private InputField _secondsField;
    
    [SerializeField] private Transform _hourHand;
    [SerializeField] private Transform _minuteHand;
    [SerializeField] private Transform _secondHand;

    [SerializeField] private Slider _meridemSlider;
    [SerializeField] private Button _AddTimerButton;

    
    [SerializeField] private Text _alarmInfoField;
    [SerializeField] private ClockHandsController _clockHandsController;

    public void UpdateTimeField()
    {
        _hoursField.text = (CalculateTimeValue(_hourHand, 30f) + _meridemSlider.value * 12).ToString();
        _minutesField.text = CalculateTimeValue(_minuteHand, 6f).ToString();
        _secondsField.text = CalculateTimeValue(_secondHand, 6f).ToString();
    }
    
    public void SetAlarmTime()
    {
        
        DateTime alarmDateTime;
        if (DateTime.TryParse(_hoursField.text + ":" + _minutesField.text + ":" + _secondsField.text, out alarmDateTime))
        {
            AlarmTime = alarmDateTime;
            _alarmInfoField.text = "Будильник установлен на: " + alarmDateTime.ToString("HH:mm:ss");
            TimeSpan alarmSpan = new TimeSpan(AlarmTime.Hour,AlarmTime.Minute,AlarmTime.Second);
            DateTime currentTime = _clockHandsController.CurrentTime;
            TimeSpan clockSpan = new TimeSpan(currentTime.Hour,currentTime.Minute,currentTime.Second);
            int secondsToWait = Convert.ToInt32(alarmSpan.Subtract(clockSpan).TotalSeconds);
            _clockHandsController.ToggleAlarm();
            _clockHandsController.ToggleClockHands();
            StartCoroutine(TriggerAlarmTimer(secondsToWait));
        }
        else
        {
            _alarmInfoField.text = "Проверьте введенные данные";
        }
    }
    
    private float CalculateTimeValue(Transform hand, float scale)
    {
        float timeValue = (-hand.localRotation.eulerAngles.z + 360) % 360 / scale;
        return Mathf.Round(timeValue);
    }

    private IEnumerator TriggerAlarmTimer(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        _alarmInfoField.text = "Время вышло";
        _AddTimerButton.gameObject.SetActive(true);
    }

}
