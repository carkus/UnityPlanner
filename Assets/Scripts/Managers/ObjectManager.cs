using UnityEngine;
using UnityEngine.AI;


public class ObjectManager : MonoBehaviour
{
    private GameObject[] citizens;

    //public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
    //public float spawnTime = 3f;            // How long between each spawn.

    public NavMeshSurface surface;
    public NavMeshBaker navMeshBaker;

    void Awake()
    {
        navMeshBaker = GameObject.Find("main").AddComponent(typeof(NavMeshBaker)) as NavMeshBaker;
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        //InvokeRepeating ("Spawn", spawnTime, spawnTime);
    }

    public void frameTick()
    {
        //if (awake) {

        //Debug.Log("I AM AWAKE!!!");

        //}
        // Set the displayed text to be the word "Score" followed by the score value.
        //text.text = "Score: " + score;
    }

    public void setWorld(OBase[] worldObjects)
    {



        OBase plane = new OBase(PrimitiveType.Plane);
        plane.setScale(10f, 1f, 10f);
        plane.setPosition(new Vector3(5f, 0, 5f));

        NavMeshSurface surface = plane.getBody().AddComponent(typeof(NavMeshSurface)) as NavMeshSurface;

        navMeshBaker.addMeshSurface(surface);
        navMeshBaker.bakeNavMeshSurfaces();




        //PrimitiveType type = PrimitiveType.Cube;
        OType randomType;

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                OBase obj = new OBase(PrimitiveType.Cube);
                randomType = (OType)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(OType)).Length));
                obj.setPosition(new Vector3(i * 4, 0, j * 4));
                obj.setType(randomType);
                obj.setColor(randomType);
                obj.setScale(2f, 2f, 2f);
                obj.makeObstacle();
            }
        }

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {


                OBase agent = new OBase(PrimitiveType.Sphere);
                randomType = (OType)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(OType)).Length));
                agent.setPosition(new Vector3(i * 4, 0, j * 4));
                agent.setType(randomType);
                agent.setColor(randomType);
                agent.makeAgent();

                Vector3 targetVector = new Vector3(50f, 0, 20f);
                agent.setDestination(targetVector);

            }

        }



        //surface.BuildNavMesh();




    }

    void Spawn()
    {
        // If the player has no health left...
        /* /if(playerHealth.currentHealth <= 0f)
        {
            // ... exit the function.
            return;
        }

        // Find a random index between zero and one less than the number of spawn points.
        int spawnPointIndex = Random.Range (0, spawnPoints.Length);

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);*/
    }


}