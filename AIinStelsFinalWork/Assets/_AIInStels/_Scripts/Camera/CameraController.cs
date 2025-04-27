using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Image _img;

    [SerializeField] private Sprite _focus;
    [SerializeField] private Sprite _focusTarget;

    private RaycastHit _hit;

    public bool TryGetEnemy(out Enemy enemy)
    {
        enemy = null;
        if (_hit.collider != null)
        {
            enemy = _hit.collider.gameObject.GetComponent<Enemy>();
            return true;
        }

        return false;
    }

    private void Start()
    {
        _hit = new RaycastHit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _img.gameObject.SetActive(true);
            transform.localPosition = new Vector3(0.6f, 0.4f, -0.6f);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Focus();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            _img.gameObject.SetActive(false);
            transform.localPosition = new Vector3(0, 2, -2);
            transform.localRotation = Quaternion.Euler(30, 0, 0);
        }
    }

    private void Focus()
    {
        if (Physics.Raycast(this.transform.position, this.transform.forward, out _hit, 50, LayerMask.GetMask("Enemy")))
        {
            _img.sprite = _focusTarget;
        }
        else _img.sprite = _focus;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.forward * 50);
        //Gizmos.DrawSphere(_hit.point, 1);
    }
}