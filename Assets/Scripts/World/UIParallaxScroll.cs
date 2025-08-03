using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIParallaxScroll : MonoBehaviour
{
    public float parallaxEffect;
    [SerializeField] private float yOffset = 0f;
    private float startPos;
    private float length;
    [SerializeField] private RectTransform rectTransform;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = 0f;
        length = rectTransform.rect.width / 3.0f;
    }

    void FixedUpdate()
    {
        float distance = Camera.main.transform.position.x * parallaxEffect;
        float movement = Camera.main.transform.position.x * (1 - parallaxEffect);

        transform.localPosition = new Vector3(startPos + distance, yOffset, transform.position.z);
        
        if (movement > (startPos + length))
        {
            startPos += length;
        }
        else if (movement < (startPos - length))
        {
            startPos -= length;
        }
    }
}
