using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private float angle;

    [SerializeField] private bool rotateAroundCenter;
    [SerializeField] private float rotationSpeed;

    private Vector3 focusPosition;

    internal void Initialize(Vector3 focusPosition)
    {
        this.focusPosition = focusPosition;

        Quaternion lookRotation = Quaternion.Euler(new Vector2(angle, 0));
        Vector3 lookDirection = lookRotation * Vector3.forward;
        transform.localPosition = focusPosition - lookDirection * distance;
        transform.localRotation = lookRotation;
    }

    private void Update()
    {
        if (!rotateAroundCenter || !GameManager.instance.playing)
            return;

        transform.LookAt(focusPosition);
        transform.Translate(Vector3.right * Time.deltaTime * rotationSpeed);
    }
}
