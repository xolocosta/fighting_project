using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _speed;

    [SerializeField] private Vector3 _offset;
    void Start()
    {
        
    }

    void Update()
    {
    }
    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(_player.transform.position.x - transform.position.x, transform.position.y, transform.position.z);
        direction = Vector3.Lerp(this.transform.position, _player.transform.position + _offset, _speed * Time.deltaTime);
        transform.position = new Vector3(direction.x, this.transform.position.y, this.transform.position.z);
    }
}
