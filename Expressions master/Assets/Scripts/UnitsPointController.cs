using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitsPointController : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    TextMeshProUGUI _countUnitsTxt;
    Camera _mainCamera;
    [SerializeField] GameObject _unitPrefab;
    const float _speedX = 3f;

    public static UnitsPointController SingletonInstance { get; private set; }

    public bool InFight = false;
    [HideInInspector] public List<GameObject> Units = new();

    const float _speed = 8f;

    private void Awake()
    {
        SingletonInstance = this;
    }

    void Start()
    {
        _mainCamera = Camera.main;
        GameObject firstUnit = Instantiate(_unitPrefab, transform.position, Quaternion.identity, gameObject.transform);
        AddUnitsToList(firstUnit);
        _countUnitsTxt = GetComponentInChildren<TextMeshProUGUI>();        
    }

    public void AddUnitsToList(GameObject unit)
    {
        Units.Add(unit);       
    }

    void Update()
    {
        if (Units.Count <= 0)
        {
            _gameManager.IsGameActive = false;
            Cursor.visible = true;
            _gameManager.GameOver();
        }        
        if (_gameManager.IsGameActive)
        {
            _countUnitsTxt.text = Units.Count.ToString();
            _countUnitsTxt.enabled = true;
            if (!InFight)
            {
                MoveToCursor();
            }
        }       
        else
        {
            _countUnitsTxt.enabled = false;
        }
    }

    void MoveToCursor()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime, Space.World);
        Ray rayFromCamera = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayFromCamera, out RaycastHit hit))
        {
            Vector3 hitPointWithCharY = new Vector3(hit.point.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, hitPointWithCharY, _speedX * Time.deltaTime);
        }
    }
}