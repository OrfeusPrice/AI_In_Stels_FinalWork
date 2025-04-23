using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private int _fps = 60;

    void Start()
    {
        Application.targetFrameRate = _fps;
    }
}