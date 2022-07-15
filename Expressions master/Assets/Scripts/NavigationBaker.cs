using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour
{
    [SerializeField] NavMeshSurface _navMeshSurface;

    void Awake()
    {
        _navMeshSurface = GetComponent<NavMeshSurface>();       
    }

    public void RebakeNavMesh()
    {
        _navMeshSurface.BuildNavMesh();
    }
}