using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public GameObject targetLocationIcon;
    public RawImage compassImage;
    public Transform cam;

    float compassUnit;

    public GameLoopHandler loopHandler;
    public CarPassengerPickUpHandler pickUpHandler;

    GameObject newMarker;
    public TargetLocationMarker currentMarker;
    void Awake() {
        cam ??= Camera.main.transform;
        compassUnit = compassImage.rectTransform.rect.width / 360f;
        loopHandler ??= GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLoopHandler>();
        loopHandler.OnNewLocationMarker.AddListener(AddLocationMarker);

        pickUpHandler ??= GameObject.FindGameObjectWithTag("Player").GetComponent<CarPassengerPickUpHandler>();

    }

    private void RemoveLocationMarker() {
        if (newMarker)
        {
            Destroy(newMarker);
        }
    }

    // Update is called once per frame
    void Update()
    {
        compassImage.uvRect = new Rect (cam.localEulerAngles.y / 360f, 0f, 1f, 1f);


        if (currentMarker != null) {
            currentMarker.image.rectTransform.anchoredPosition = GetPosOnCompass(currentMarker);
        }
    }

    public void AddLocationMarker(TargetLocationMarker marker) {
        RemoveLocationMarker();
        newMarker = Instantiate(targetLocationIcon, compassImage.transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;
        currentMarker = marker;
    }


    Vector2 GetPosOnCompass(TargetLocationMarker marker) {
        Vector2 camPos = new Vector2(cam.transform.position.x, cam.transform.position.z);
        Vector2 camFwd = new Vector2(cam.transform.forward.x, cam.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - camPos, camFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
