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
    UnitsPointController _targetUnits;
    GameManager _gameManager;
    [SerializeField] LayerMask _units;


    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _targetUnits = FindObjectOfType<UnitsPointController>();
        _navMeshAgent = GetComponent<NavMeshAgent>();  
        _rotationSpeed = _navMeshAgent.angularSpeed;
        _agentTransform = _navMeshAgent.transform;        
        _target = _targetUnits.Units[0];       
    }

    void Update()
    {
        if (_gameManager.IsGameActive)
        {
            SortTarget();
            if (_inFight)
            {
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, _detectionDistance, _units);
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