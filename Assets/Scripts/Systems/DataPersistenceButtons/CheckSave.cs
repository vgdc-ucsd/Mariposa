using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CheckSave : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private ButtonProperties buttonProps;
    private Image image;
    void Start()
    {
        buttonProps = GetComponent<ButtonProperties>();
        image = GetComponent<Image>();
        bool fileExists = File.Exists(Path.Combine(Application.persistentDataPath,buttonProps.fileName));
        image.color = fileExists ? buttonProps.enabledColor : buttonProps.disabledColor;
    }
}
