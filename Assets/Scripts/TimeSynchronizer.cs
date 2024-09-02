using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class TimeSynchronizer : MonoBehaviour
{
    private string [] _allTimeApiUrls = new []
    {
        "http://worldtimeapi.org/api/timezone/Etc/UTC",
        "https://timeapi.io/api/time/current/zone?timeZone=Europe%2FMoscow"
    } ;
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
        bool success = false;
        
        foreach (string url in _allTimeApiUrls)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;
                jsonResult = jsonResult.Replace("DateTime", "datetime", StringComparison.OrdinalIgnoreCase);
                TimeData timeData = JsonUtility.FromJson<TimeData>(jsonResult);
                _synchronizedTime = DateTime.Parse(timeData.datetime);
                success = true;
                break;
            }
            else
            {
                Debug.Log($"Не удалось подключиться к сервису: {url}");
            }
        }
        if (!success)
        {
            Debug.Log("Ошибка подключения к сервисам, проверьте соединение с интернетом!");
            _synchronizedTime = DateTime.Now;
            Debug.Log("Выставлено время с компьютера");
        }
        
        IsTimeSynchronized = true;
    }
}

[Serializable]
public class TimeData
{
    public string datetime;
}