using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic; 

public class NavMeshSwitcher : Singleton<NavMeshSwitcher>
{
    [SerializeField] private List<GameObject> _navmeshPrefabs;
    private GameObject currentNavMeshObject;

    public void SwitchTo(int index)
    {
        if (index < 0 || index >= _navmeshPrefabs.Count)
        {
            return;
        }

        if (currentNavMeshObject != null)
        {
            Destroy(currentNavMeshObject);
        }

        GameObject navMeshPrefab = _navmeshPrefabs[index];
        if (navMeshPrefab == null)
        {
            return;
        }

        currentNavMeshObject = Instantiate(navMeshPrefab);
        NavMeshSurface surface = currentNavMeshObject.GetComponent<NavMeshSurface>();

        if (surface == null)
        {
            return;
        }

        surface.BuildNavMesh();
    }
}
