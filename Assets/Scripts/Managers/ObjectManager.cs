using UnityEngine;

public class ObjectManager
{
    private GameObject[] citizens;

    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
    public float spawnTime = 3f;            // How long between each spawn.

    void Start ()
    {
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

    }

    void Spawn ()
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