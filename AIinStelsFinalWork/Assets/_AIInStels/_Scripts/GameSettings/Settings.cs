using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    [SerializeField] private int _fps = 60;

    void Start()
    {
        Application.targetFrameRate = _fps;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R)) SceneManager.LoadScene(0);
    }
}