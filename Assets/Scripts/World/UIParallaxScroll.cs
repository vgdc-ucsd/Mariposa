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
    [SerializeField] private PlayerMovement playerMovementInfo;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        // rectTransform.position = new(Camera.main.transform.position.x, rectTransform.position.y, rectTransform.position.z);
        startPos = rectTransform.position.x + (50.0f * length / 2.0f);
        length = rectTransform.rect.width;
        StartCoroutine(GetPlayerMovement());
    }

    IEnumerator GetPlayerMovement()
    {
        for (int i = 0; i < 10; i++)
        {
            playerMovementInfo = Player.ActivePlayer.gameObject.GetComponent<PlayerMovement>();
            if (playerMovementInfo == null)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        Debug.Log("Player not found");
    }

    void FixedUpdate()
    {
        if (playerMovementInfo == null) return;

        // new implementation; horizontally displace item from canvas, which moves with camera
        float distance = Camera.main.transform.position.x * parallaxEffect;
        float movement = Camera.main.transform.position.x * (1 - parallaxEffect);

        rectTransform.position = new Vector3(startPos + distance, Camera.main.transform.position.y + yOffset, transform.position.z);
        
        if (movement > startPos + length / 2.0f)
        {
            startPos += length;
        }
        else if (movement <  startPos - length / 2.0f)
        {
            startPos -= length;
        }
    }
}
