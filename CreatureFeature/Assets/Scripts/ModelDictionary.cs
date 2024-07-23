using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class ModelDictionary : MonoBehaviour
{
   public GameObject modelBeingCarried;
   public GameObject modelOnStove;
   public GameObject modelOnCuttingBoard;
   public GameObject modelOnWorkSpace;
   public GameObject modelOnServingWindow;

   /// <summary>
   ///  0 = carryingslot, 1 = Stove, 2 = CuttingBoard, 3 = WorkSpace, 4 = ServingWindow
   /// </summary>
    public GameObject[] spawnPoints;
    
    
    // Singleton instance
    public static ModelDictionary Instance { get; private set; }

    private Dictionary<string, Queue<GameObject>> modelPoolDictionary;
    private Vector3 poolLocation = new Vector3(100, 100, 100);

    void Awake()
    {
        // Ensure that there's only one instance
        if (Instance == null)
        {
            Instance = this;
            modelPoolDictionary = new Dictionary<string, Queue<GameObject>>();
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePool()
    {
        // Load all GameObjects in the "Models" folder within the Resources folder
        GameObject[] models = Resources.LoadAll<GameObject>("Models");

        // Iterate through each model and add it to the pool dictionary
        foreach (GameObject model in models)
        {
            Queue<GameObject> modelQueue = new Queue<GameObject>();
            // Instantiate a pool of 1 objects for each model type (adjust the pool size as needed)
            for (int i = 0; i < 1; i++)
            {
                GameObject obj = Instantiate(model, poolLocation, Quaternion.identity);
                obj.SetActive(false);
                modelQueue.Enqueue(obj);
            }
            modelPoolDictionary.Add(model.name, modelQueue);
        }
    }

    public GameObject GetModel(string modelName)
    {
        if (modelPoolDictionary.TryGetValue(modelName, out Queue<GameObject> modelQueue))
        {
            if (modelQueue.Count > 0)
            {
                GameObject model = modelQueue.Dequeue();
                model.SetActive(true);
                return model;
            }
            else
            {
                Debug.LogWarning("No available objects in the pool for model: " + modelName);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Model not found in pool: " + modelName);
            return null;
        }
    }

    public void ReturnModel(string modelName, GameObject model)
    {
        if (modelPoolDictionary.ContainsKey(modelName))
        {
            model.SetActive(false);
            model.transform.position = poolLocation;
            modelPoolDictionary[modelName].Enqueue(model);
        }
        else
        {
            Debug.LogWarning("Model not found in pool: " + modelName);
        }
    }

    public void SpawnModel(string modelName, int spawnPointIndex)
    {
        // Get the GameObject from the pool
        GameObject model = GetModel(modelName);

        // Check if the model is found
        if (model != null)
        {
            if (spawnPoints != null)
            {
                GameObject spawnPoint = spawnPoints[spawnPointIndex];
                
                // Move the model to the desired location
                model.transform.position = spawnPoint.transform.position;
                model.transform.rotation = transform.rotation;
                model.transform.parent = spawnPoint.transform;
                Debug.Log(modelName + " object instantiated from pool.");
                
                switch (spawnPointIndex)
                {
                    case 0:
                        modelBeingCarried = model;
                        break;
                    case 1:
                         modelOnStove = model;
                         break;
                    case 2:
                         modelOnCuttingBoard = model;
                         break;
                    case 3:
                         modelOnWorkSpace = model;
                         break;
                    case 4:
                         modelOnServingWindow = model;
                         break;
                    default:
                        Debug.LogWarning("Spawn point index not found");
                        return;
                }
            }
        }
        else
        {
            Debug.LogWarning(modelName + " object not found in pool.");
        }

    }

    public void RemoveModel(int spawnPointIndex)
    {
            GameObject model = null; 
            
            switch (spawnPointIndex)
            {
                case 0:
                    model = modelBeingCarried;
                    break;
                case 1:
                    model = modelOnStove;
                    break;
                case 2:
                    model = modelOnCuttingBoard;
                    break;
                case 3:
                    model = modelOnWorkSpace;
                    break;
                case 4:
                    model = modelOnServingWindow;
                    break;
            }

            string modelName = model.name.Replace("(Clone)", "").Trim();
            ReturnModel(modelName, model);
    }
}
