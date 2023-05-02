using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public float baseSize;
    public float scaleFactor;

    private GameObject button;

    public bool useLT;

    private void Start() {
        button = gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (useLT) {
            LeanTween.scale(button, new Vector3(scaleFactor, scaleFactor, 0f), 0.1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (useLT) {
            LeanTween.scale(button, new Vector3(baseSize, baseSize, 0f), 0.1f);
        }
    }
}
