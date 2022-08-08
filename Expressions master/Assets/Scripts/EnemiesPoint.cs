using UnityEngine;
using TMPro;

public class EnemiesPoint : MonoBehaviour
{
    TextMeshProUGUI _countUnitsTxt;
    GameManager _gameManager;
    SpawnManager _spawnManager;
    public int CountEnemyUnits;

    void Start()
    {
        _countUnitsTxt = GetComponentInChildren<TextMeshProUGUI>();
        _gameManager = GameManager.SingletonInstance;
        _spawnManager = SpawnManager.SingletonInstance;
        CountEnemyUnits = _spawnManager._numberOfEnemies;        
    }

    void Update()
    {
        if (_gameManager.IsGameActive)
        {   
            if (CountEnemyUnits > 0)
                _countUnitsTxt.gameObject.SetActive(true); 
            else
                _countUnitsTxt.gameObject.SetActive(false);        

            _countUnitsTxt.text = CountEnemyUnits.ToString();

            Vector3 position = new Vector3(transform.position.x, 4f, transform.position.z + 50f);
            _countUnitsTxt.transform.position = position;          
        }
        else
            _countUnitsTxt.gameObject.SetActive(false);
    }
}