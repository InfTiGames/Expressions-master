using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemiesControl : MonoBehaviour
{
    NavMeshAgent _navMeshAgent;  
    float _detectionDistance = 10f;
    bool _inFight;
    float _rotationSpeed;    
    GameObject _target;    
    Transform _agentTransform;  
    GameManager _gameManager;
    [SerializeField] LayerMask _unitsMask;

    Animator _animator;
    int _isWalkingHash;
    int _isAttackingHash;
    float _attackDistance = 5f;

    void Start()
    {
        _gameManager = GameManager.SingletonInstance;
        _navMeshAgent = GetComponent<NavMeshAgent>();  
        _rotationSpeed = _navMeshAgent.angularSpeed;
        _agentTransform = _navMeshAgent.transform;
        _animator = GetComponent<Animator>();
        _isWalkingHash = Animator.StringToHash("Walk");
        _isAttackingHash = Animator.StringToHash("Attack");
    }

    void Update()
    {
        if (_gameManager.IsGameActive)
        {
            SortTarget();
            if (_inFight)
            {
                if (Vector3.Distance(_target.transform.position, transform.position) <= _attackDistance)
                {
                    _animator.SetBool(_isWalkingHash, false);
                    _animator.SetBool(_isAttackingHash, true);
                }
                else
                {
                    _animator.SetBool(_isWalkingHash, true);
                }
                RotateToTarget();
                MoveToTarget();  
            }
            else
            {
                RotateBack(); 
            }
        }
    }

    GameObject SortTarget()
    {
        GameObject target;
        Collider[] colliders = Physics.OverlapSphere(transform.position, _detectionDistance, _unitsMask);
        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                if (col.TryGetComponent(out UnitsControl unit))
                {
                    target = col.gameObject;
                    _target = target;
                    _inFight = true;
                }
            }
        }
        else
            _inFight = false;
        return _target;
    }            

    void MoveToTarget()
    {
        if (_target != null)
        {
            _navMeshAgent.SetDestination(_target.transform.position);
        }        
    }

    void RotateToTarget()
    {        
        if (_target != null)
        {
            Vector3 lookDirection = _target.transform.position - _agentTransform.position;
            lookDirection.y = 0;
            if (lookDirection == Vector3.zero) return;
            _agentTransform.rotation = Quaternion.RotateTowards(_agentTransform.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), _rotationSpeed * Time.deltaTime);
        }
    }

    void RotateBack()
    {
        Vector3 lookDirection = Vector3.back;
        lookDirection.y = 0;
        if (lookDirection == Vector3.zero) return;
        _agentTransform.rotation = Quaternion.RotateTowards(_agentTransform.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), _rotationSpeed * Time.deltaTime);
    }
}