using UnityEngine;
using UnityEngine.AI;

using System.Collections;
using System.Collections.Generic;


public class NavMeshBaker : MonoBehaviour
{

    [SerializeField]
    public List<NavMeshSurface> navMeshSurface = new List<NavMeshSurface>();



    public void bakeNavMeshSurfaces()
    {

        for (int i=0; i<navMeshSurface.Count; i++){    
            navMeshSurface[i].BuildNavMesh();
        }

    }

    public void addMeshSurface(NavMeshSurface surface) {
        navMeshSurface.Add(surface);
    }



}


