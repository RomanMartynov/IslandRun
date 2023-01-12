using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _target;

    private void Update()
    {
        _camera.position = new Vector3(_target.position.x, 5f, _target.position.z - 5f);
    }
}
