using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitsControl : MonoBehaviour
{
    [SerializeField] GameObject _unitDeathFxPrefab;
    [SerializeField] GameObject _enemyUnitDeathFxPrefab;

    bool _inFight;

    [SerializeField] LayerMask _enemies;

    NavMeshAgent _navMeshAgent;
    UnitsPointController _parent;
    SpawnManager _spawnManager;
    GameManager _gameManager;  
   
    float _detectionDistance = 10f;   
    float _rotationSpeed;
      
    Transform _agentTransform; 
    GameObject _curTarget;   

    void Start()
    {        
        _spawnManager = FindObjectOfType<SpawnManager>();
        _gameManager = FindObjectOfType<GameManager>();
        _parent = GetComponentInParent<UnitsPointController>();          
        
        _curTarget = _spawnManager.Enemies[0];      
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rotationSpeed = _navMeshAgent.angularSpeed;
        _agentTransform = _navMeshAgent.transform;
    }

    void Update()
    {
        if (_gameManager.IsGameActive)
        {
            SortTarget();
            if (_inFight)
            {
                _parent.InFight = true;
                MoveToTarget();
                RotateToTarget();  
            }
            else
            {
                RotateForward();
                MoveToParent();
                _parent.InFight = false; 
            }           
        }
    }

    GameObject SortTarget()
    {
        GameObject target;
        Collider[] colliders = Physics.OverlapSphere(transform.position - new Vector3(0,0,5f), _detectionDistance, _enemies);

        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                if (col.TryGetComponent(out EnemiesControl enemy))
                {
                    target = col.gameObject;
                    _curTarget = target;
                    _inFight = true;
                }
            }
        }
        else
            _inFight = false;
        return _curTarget;
    }

    void RotateToTarget()
    {
        if (_curTarget != null)
        {
            Vector3 lookDirection = _curTarget.transform.position - _agentTransform.position;
            lookDirection.y = 0;
            _agentTransform.rotation = Quaternion.RotateTowards(_agentTransform.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), _rotationSpeed * Time.deltaTime);
        }
    }

    void RotateForward()
    {
        _agentTransform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.LookRotation(Vector3.forward, Vector3.up), 1f * Time.deltaTime);
    }

    void MoveToTarget()
    {
        if (_curTarget != null)
        {
            _navMeshAgent.SetDestination(_curTarget.transform.position);
        }
    }

    void MoveToParent()
    {
        _navMeshAgent.SetDestination(_parent.transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemiesControl enemy))
        {
            _spawnManager.Enemies.Remove(collision.gameObject);
            Vector3 _fxPositionEnemy = new Vector3(collision.gameObject.transform.position.x, 1, collision.gameObject.transform.position.z);
            GameObject enemPrefabFx = Instantiate(_enemyUnitDeathFxPrefab, _fxPositionEnemy, Quaternion.identity);
            Destroy(enemPrefabFx, 1.5f);
            Destroy(collision.gameObject);

            collision.gameObject.GetComponentInParent<EnemiesPoint>().CountEnemyUnits--;
            
            _parent.Units.Remove(gameObject);
            Vector3 _fxPosition = new Vector3(transform.position.x, 1, transform.position.z);
            GameObject unitPrefabFx = Instantiate(_unitDeathFxPrefab, _fxPosition, Quaternion.identity);
            Destroy(unitPrefabFx, 1.5f);
            Destroy(gameObject);
        }
    }
}