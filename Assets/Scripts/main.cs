using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class main : MonoBehaviour
{

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    private SphereCollider sphereCollider;

    //Managers
    private WorldManager worldManager;//environment and world parent
    private ObjectManager objectManager;//will handle object sent to it by the world
    private PlanManager planManager;
    private ScoreManager scoreManager;//when one is required...

    public OBase[] ObjBase;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
    }   

    // Start is called before the first frame update
    void Start()
    {        

        planManager = new PlanManager();
        planManager.groundProblem();
        planManager.callPlanner();
        
        worldManager = new WorldManager();
        worldManager.startWorld();

        objectManager = new ObjectManager();
        objectManager.setWorld(worldManager.worldObjects);

    }

    // Update is called once per frame
    void Update()
    {
        worldManager.frameTick();
        objectManager.frameTick();
    }

    T[] InitializeArray<T>(int length) where T : new()
    {
        T[] array = new T[length];
        for (int i = 0; i < length; ++i)
        {
            array[i] = new T();
        }
        return array;
    }

}
