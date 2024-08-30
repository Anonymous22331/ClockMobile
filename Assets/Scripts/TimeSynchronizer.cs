using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class TimeSynchronizer : MonoBehaviour
{
    private string _firstTimeApiUrl = "http://worldtimeapi.org/api/timezone/Etc/UTC";
    private string _secondTimeApiUrl = "https://timeapi.io/api/time/current/zone?timeZone=Europe%2FMoscow";
    private DateTime _synchronizedTime;
    public bool IsTimeSynchronized { get; private set; } = false;
    
    public void SynchronizeTime()
    {
        if (!IsTimeSynchronized)
        {
            StartCoroutine(SynchronizeTimeCoroutine());
        }
        else
        {
            Debug.Log("Синхронизация уже проходит");
        }
    }

    public DateTime GetSynchronizedTime()
    {
        IsTimeSynchronized = false;
        return _synchronizedTime;
    }
    
    private IEnumerator SynchronizeTimeCoroutine()
    {
        IsTimeSynchronized = false;
        UnityWebRequest firstRequest = UnityWebRequest.Get(_firstTimeApiUrl);
        yield return firstRequest.SendWebRequest();
       
        if (firstRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Не удалось подключиться к первому сервису");
            UnityWebRequest secondRequest = UnityWebRequest.Get(_secondTimeApiUrl);
            yield return secondRequest.SendWebRequest();

            if (secondRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Ошибка подключения к сервисам, проверьте соединение с интернетом!");
                _synchronizedTime = DateTime.Now;
                Debug.Log("Выставлено время с компьютера");

            }
            else
            {
                string jsonResult = secondRequest.downloadHandler.text;
                jsonResult = jsonResult.Replace("dateTime", "datetime");
                TimeData timeData = JsonUtility.FromJson<TimeData>(jsonResult);
                _synchronizedTime = DateTime.Parse(timeData.datetime);
            }
        }
        else
        {
            string jsonResult = firstRequest.downloadHandler.text;
            TimeData timeData = JsonUtility.FromJson<TimeData>(jsonResult);
            _synchronizedTime = DateTime.Parse(timeData.datetime);
        }
        
        IsTimeSynchronized = true;
    }
}

[Serializable]
public class TimeData
{
    public string datetime;
}