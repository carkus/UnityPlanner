using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldManager
{
    public static int score;        // The player's score.

    public OBase[] worldObjects {
        get;
        set;        
    }

    public bool awake;

    //Text text;                      // Reference to the Text component.

    public void startWorld()
    {
        // Set up the reference.
        //text = GetComponent <Text> ();

        // Reset the score.
        //score = 0;

        //PrimitiveType type = PrimitiveType.Cube;

        /*for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                OBase obj = new OBase();
                OType randomType = (OType)(UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(OType)).Length));
                obj.setPosition(new Vector3(i*2, 0, j*2));
                obj.setType(randomType);
                obj.setColor(randomType);

            }
        }*/


        //_domain = new JSONParser();

        awake = true;

    }


    public void frameTick()
    {

        if (awake) {

            //Debug.Log("I AM AWAKE!!!");

        }
        // Set the displayed text to be the word "Score" followed by the score value.
        //text.text = "Score: " + score;
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