using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingStatusBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform parentTf;
    [SerializeField] Vector3 offset;

    private void Update() {
        transform.parent.rotation = playerCam.transform.rotation;
        transform.position = parentTf.position + offset;
    }

    public void UpdateValue(int value, int maxValue) {
        slider.value = ((float)value) / maxValue;
    }
    public void UpdateValue(float value, float maxValue) {
        slider.value = value / maxValue;
    }
}
