using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] UnitsPointController _player;    
    Vector3 offset = new Vector3(0, 0, -8f);
    [SerializeField] GameManager _gameManager;

    void LateUpdate()
    {
        if (!_player.InFight && _gameManager.IsGameActive)
        {
            MoveCamera();
        }                 
    }

    void MoveCamera()
    {
        Vector3 cameraPosition = new Vector3(transform.position.x, transform.position.y, _player.transform.position.z + offset.z);
        transform.position = Vector3.Slerp(transform.position, cameraPosition, 8f);
    }
}