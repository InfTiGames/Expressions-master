using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitsControl : MonoBehaviour
{
    [SerializeField] GameObject _unitDeathFxPrefab;
    [SerializeField] GameObject _enemyUnitDeathFxPrefab;

    public static UnitsControl SingletonInstance { get; private set; }

    void Awake()
    {
        SingletonInstance = this;
    }

    bool _inFight;

    Animator _animator;
    int _isRunningHash;
    int _isAttackingHash;

    [SerializeField] LayerMask _enemies;

    NavMeshAgent _navMeshAgent;
    UnitsPointController _parent;

    SpawnManager _spawnManager;

    GameManager _gameManager;  
   
    float _detectionDistance = 10f;
    float _attackDistance = 3f;
    float _rotationSpeed;
      
    Transform _agentTransform; 
    GameObject _curTarget;   

    void Start()
    {
        _spawnManager = SpawnManager.SingletonInstance;
        _gameManager = GameManager.SingletonInstance;
        _parent = GetComponentInParent<UnitsPointController>();              
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rotationSpeed = _navMeshAgent.angularSpeed;
        _agentTransform = _navMeshAgent.transform;
        _animator = GetComponent<Animator>();
        _isRunningHash = Animator.StringToHash("Run");
        _isAttackingHash = Animator.StringToHash("Attack");
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
                if (Vector3.Distance(SortTarget().transform.position, transform.position) <= _attackDistance)
                {
                    _animator.SetBool(_isRunningHash, false);
                    _animator.SetBool(_isAttackingHash, true);
                }
            }
            else
            {
                _parent.InFight = false;
                RotateForward();
                MoveToParent();
                _animator.SetBool(_isRunningHash, true);
                _animator.SetBool(_isAttackingHash, false);
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
        Vector3 lookDirection = Vector3.forward;
        lookDirection.y = 0;
        if (lookDirection == Vector3.zero) return;
        _agentTransform.rotation = Quaternion.RotateTowards(_agentTransform.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), _rotationSpeed * Time.deltaTime);
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

    void DestroyGm(Vector3 _pos, GameObject gm, GameObject _prefab)
    {
        _pos = new Vector3(gm.transform.position.x, 1f, gm.transform.position.z);
        GameObject _fx = Instantiate(_prefab, _pos, Quaternion.identity);
        Destroy(_fx, 1.5f);
        Destroy(gm);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemiesControl enemy))
        {
            _spawnManager.Enemies.Remove(collision.gameObject);
            DestroyGm(collision.gameObject.transform.position, collision.gameObject, _enemyUnitDeathFxPrefab);
            collision.gameObject.GetComponentInParent<EnemiesPoint>().CountEnemyUnits--;
            _parent.Units.Remove(gameObject);
            DestroyGm(transform.position, gameObject, _unitDeathFxPrefab);
        }
    }
}