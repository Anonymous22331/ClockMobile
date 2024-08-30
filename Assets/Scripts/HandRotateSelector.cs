using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandRotateSelector : MonoBehaviour
{
    private Transform _selectedHand;
    private PointerEventData _PointerEventData;
    
    [SerializeField] private ClockHandsController _clockHandsController;
    [SerializeField] private AlarmController _alarmController;
    [SerializeField] private GraphicRaycaster _Raycaster;
    [SerializeField] private EventSystem _EventSystem;
    [SerializeField] private RectTransform canvasRect;



    void Update()
    {
        if (Input.GetMouseButton(0) && _clockHandsController.IsAlarmActive)
        {
            _PointerEventData = new PointerEventData(_EventSystem);
            _PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            _Raycaster.Raycast(_PointerEventData, results);

            if (results.Count > 0)
            {
                if (results[0].gameObject.CompareTag("Hand"))
                {
                    _selectedHand = results[0].gameObject.transform;
                }
            }
        }
        
        if (!_selectedHand.IsUnityNull() && Input.GetMouseButton(0) && _clockHandsController.IsAlarmActive)
        {
            RotateHandTowardMouse();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _alarmController.UpdateTimeField();
        }
        
    }

    private void RotateHandTowardMouse()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        Vector2 direction = Camera.main.ScreenToWorldPoint(currentMousePosition) - _selectedHand.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _selectedHand.transform.rotation = targetRotation;
    }
}