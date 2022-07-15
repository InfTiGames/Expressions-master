using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{    
    [SerializeField] Transform _enemiesPointParent;
    [SerializeField] GameObject _pointPrefab;
    [SerializeField] GameObject _groundTile;
    [SerializeField] GameObject _enemyPrefab;

    UnitsPointController _player;
    GameManager _gameManager;
    Transform _tilesParent;
    GameObject _enemiesParent;
    [SerializeField] NavigationBaker _navMeshBaker;

    float _zSpawn = 0f;    
    float _tileLength = 50f;
    float _zSpawnOfPoints = 0f;

    public int _numberOfEnemies = 20;
    int _numberOfPoints = 2;
    int _numberOfTiels = 3;   

    List<GameObject> Tiles = new List<GameObject>();
    public List<GameObject> Points = new List<GameObject>();
    public List<GameObject> Enemies = new List<GameObject>();   

    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _player = FindObjectOfType<UnitsPointController>();
        _tilesParent = gameObject.transform;
        _navMeshBaker = FindObjectOfType<NavigationBaker>();

        for (int i = 0; i < _numberOfTiels; i++)
        {            
            SpawnTile();  
        }
        _navMeshBaker.RebakeNavMesh();
    }

    void Start()
    {
        for (int i = 0; i < _numberOfPoints; i++)
        {
            SpawnPoint();
            SpawnEnemies();
        }
    }

    void Update()
    {
        if (_gameManager.IsGameActive)
        {            
            if (_player.transform.position.z - 53.5 > _zSpawn - (_numberOfTiels * _tileLength))
            {
                SpawnTile();
                Invoke(nameof(SpawnPoint), 0.5f);
                Invoke(nameof(SpawnEnemies), 0.5f);
                Deleting();
                _navMeshBaker.RebakeNavMesh();
            }
        }
    }

    void SpawnTile()
    {
        Vector3 tilePosition = new Vector3(0, 0.1f, _zSpawn);
        GameObject ground = Instantiate(_groundTile, tilePosition, transform.rotation, _tilesParent);
        Tiles.Add(ground);
        _zSpawn += _tileLength;        
    }

    void SpawnPoint()
    {
        _enemiesParent = Instantiate(_pointPrefab, transform.forward * _zSpawnOfPoints, transform.rotation, _enemiesPointParent);
        Points.Add(_enemiesParent);
        _zSpawnOfPoints += _tileLength;
    }

    void SpawnEnemies()
    {
        _numberOfEnemies = _player.Units.Count > 3 ? _player.Units.Count / 2 : 1;
        for (int x = 0; x < _numberOfEnemies; x++)
        {
            float angle = x * Mathf.PI * 2f / _numberOfEnemies;
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * 1f, 0f, (Mathf.Sin(angle) * 1f) + _zSpawnOfPoints);
            GameObject enemy = Instantiate(_enemyPrefab, newPos, Quaternion.LookRotation(Vector3.back), _enemiesParent.transform);
            Enemies.Add(enemy);
        }
    }

    void Deleting()
    {
        Destroy(Tiles[0]);
        Tiles.RemoveAt(0);
        Destroy(Points[0]);
        Points.RemoveAt(0);
    }
}