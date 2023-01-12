using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _playerSpeedModifer;

    [Header("Components")]
    [SerializeField] private Rigidbody _rigidbody;
    private float _playerRotationAngle;

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _playerRotationAngle = -90f;
            MovePlayer();
        }

        else if (Input.GetKey(KeyCode.D))
        {
            _playerRotationAngle = 90f;
            MovePlayer();
        }

        else if (Input.GetKey(KeyCode.W))
        {
            _playerRotationAngle = 0f;
            MovePlayer();
        }

        else if (Input.GetKey(KeyCode.S))
        {
            _playerRotationAngle = 180f;
            MovePlayer();
        }

        HandleRotation();
    }

    private void MovePlayer()
    {
        _rigidbody.AddForce((transform.forward) * _playerSpeedModifer * Time.deltaTime);
    }

    private void HandleRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _playerRotationAngle, 0), Time.deltaTime * 20);
    }
}
