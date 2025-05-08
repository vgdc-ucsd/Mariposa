using FMOD.Studio;
using UnityEngine;
public class AudioControls : MonoBehaviour
{
    public void method1()
    {
        Debug.Log("hello");
    }
    public void method11()
    {
        Debug.Log("hello1");
    }
    public void method111()
    {
        Debug.Log("hello11");
    }
    public void method1111()
    {
        Debug.Log("hello111");
    }
    private void method2() { }
    public void method3(int apple, string banana, float dog) { Debug.Log(apple); }
    public void method33(int apple, Vector2 cow, bool watermelon) { Debug.Log(apple); }
    public void method333(int apple, AudioManager.Audio_Ambience eventName) { Debug.Log(apple); }
    public int method4() { return 1; }
    internal void method5() { }
}