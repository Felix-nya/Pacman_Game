using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] float xcoord = 0f;
    private Vector3 _positionShifting;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _positionShifting.x = xcoord;
        _positionShifting.y = 1f;
        _positionShifting.z = 0f;
        collision.transform.position = _positionShifting;
    }
}
