using UnityEngine;
using System.Collections;

public class DataManager : MonoBehaviour {

    static public DataManager instance;

      
    void Awake()
    {
        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Destroy(gameObject);
        }

    }

        public bool prezenter = false;

        public void setPrezenter(bool flag)
        {

            prezenter = flag;
        }
    
}
