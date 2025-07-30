using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [SerializeField] private List<GameObject> cameras;
    [SerializeField] private GameObject playerCamera;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        GetCamerasInScene();
        playerCamera = cameras.Find(obj => obj.CompareTag("MainCamera"));
    }

    public void SetActiveCamera(GameObject camera)
    {
        foreach (GameObject cam in cameras)
        {
            cam.SetActive(false);
        }

        cameras.Find(obj => obj.name == camera.name).SetActive(true);
    }

    public void SetActiveCamera(string cameraName)
    {
        foreach (GameObject cam in cameras)
        {
            cam.SetActive(false);
        }

        cameras.Find(obj => obj.name == cameraName).SetActive(true);
    }

    public void EnableCamera(GameObject camera)
    {
        cameras.Find(obj => obj.name == camera.name).SetActive(true);
    }

    public void EnableCamera(string cameraName)
    {
        cameras.Find(obj => obj.name == cameraName).SetActive(true);
    }

    public void DisableCamera(GameObject camera)
    {
        cameras.Find(obj => obj.name == camera.name).SetActive(false);
    }

    public void DisableCamera(string cameraName)
    {
        cameras.Find(obj => obj.name == cameraName).SetActive(false);
    }

    public void GetCamerasInScene()
    {
        CinemachineCamera[] camerasArray = Object.FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        cameras.Clear();
        foreach (CinemachineCamera cam in camerasArray)
        {
            cameras.Add(cam.gameObject);
        }
    }

}
