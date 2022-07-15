using UnityEngine;
using TMPro;

public class Expressions : MonoBehaviour
{
    [SerializeField] GameObject _leftExpression;
    [SerializeField] GameObject _rightExpression;
    [SerializeField] GameObject _unitPrefab;
    UnitsPointController _player;
    TextMeshProUGUI _expressionText;
    int _expressionsValue;

    enum Expression
    {
        Multiply = 1, Divide = 2, Sum = 3, Minus = 4, 
    }

    Expression _expressionType; 

    void Awake()
    {        
        _player = FindObjectOfType<UnitsPointController>();
        _expressionText = GetComponentInChildren<TextMeshProUGUI>();       
    }

    void Start()
    {        
        _expressionsValue = Random.Range(1, 10);
        _expressionType = (Expression)Random.Range(1, 4);

        if (_expressionsValue > 4 && _expressionType == Expression.Multiply || _expressionsValue > 4 && _expressionType == Expression.Divide)
        {
            _expressionType = (Expression)Random.Range(4, 3);
        } 

        ExpressionText(_expressionText);
    }

    void ExpressionText(TextMeshProUGUI _expressionText)
    {
        if (_expressionType == Expression.Sum)
        {
            _expressionText.text = "x+" + _expressionsValue;         
        }
        
        if (_expressionType == Expression.Minus)
        {
            _expressionText.text = "x-" + _expressionsValue;   
        }
        
        if(_expressionType == Expression.Multiply)
        {
            _expressionText.text = "x*" + _expressionsValue;
        }
        
        if (_expressionType == Expression.Divide)
        {
            _expressionText.text = "x/" + _expressionsValue;
        } 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (this.gameObject == _leftExpression)
            {
                _rightExpression.SetActive(false);
            }
            else
            {
                _leftExpression.SetActive(false);
            }
            gameObject.SetActive(false);

            switch (_expressionType)
            {
                case Expression.Sum:
                    for (int i = 0; i < _expressionsValue; i++)
                    {
                        float angle = i * Mathf.PI * 2f / _expressionsValue;
                        Vector3 newPos = new Vector3(Mathf.Cos(angle) * 1f, -0.1f, Mathf.Sin(angle) * 1f);
                        GameObject gm = Instantiate(_unitPrefab, transform.position + newPos, Quaternion.identity, _player.transform);
                        _player.AddUnitsToList(gm);
                    }
                    break;
                case Expression.Minus:
                    for (int i = 0; i < _expressionsValue; i++)
                    {
                        if (_player.Units.Count > 0)
                        {
                            Destroy(_player.Units[0].gameObject);
                            _player.Units.Remove(_player.Units[0].gameObject);
                        }                    
                    }
                    break;
                case Expression.Multiply:
                    int g = (_player.Units.Count * _expressionsValue) - _player.Units.Count;
                    for (int i = 0; i < g; i++)
                    {
                        float angle = i * Mathf.PI * 2f / g;
                        Vector3 newPos = new Vector3(Mathf.Cos(angle) * 1f, -0.1f, Mathf.Sin(angle) * 1f);

                        GameObject units = Instantiate(_unitPrefab, transform.position + newPos, Quaternion.identity, _player.transform);
                        _player.AddUnitsToList(units);
                    }
                    break;
                case Expression.Divide:
                    int y = _player.Units.Count / _expressionsValue;
                    int z = _player.Units.Count - y;
                    for (int i = 0; i < z; i++)
                    {
                        if (_player.Units.Count != 1)
                        {
                            Destroy(_player.Units[0].gameObject);
                            _player.Units.Remove(_player.Units[0].gameObject);
                        }
                    }
                    break;
            }            
        }
    }
}