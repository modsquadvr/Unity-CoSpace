using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System;

public class TaskOptions : MonoBehaviour
{
    private readonly string gameDataFileName = "tasks.json";
    // Start is called before the first frame update
    Dropdown m_Dropdown;

    void Start()
    {
        //Fetch the Dropdown GameObject
        m_Dropdown = GetComponent<Dropdown>();
        m_Dropdown.options.Clear();
        //Add listener for when the value of the Dropdown changes, to take action
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });
        //Initialise the options
        LoadAllTasks();

        m_Dropdown.captionText.text = "Task Options";


    }

    //Ouput the new value of the Dropdown into Text
    async void DropdownValueChanged(Dropdown change)
    {
#if UNITY_WSA && !UNITY_EDITOR
        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile file = await storageFolder.CreateFileAsync(gameDataFileName,
                        Windows.Storage.CreationCollisionOption.OpenIfExists);
        string filePath = file.Path;
#else
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
#endif

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
           
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(dataAsJson);

            jsonObj["loadedmap"] = m_Dropdown.options[change.value].text;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            //File.WriteAllText(filePath, output);

#if UNITY_WSA && !UNITY_EDITOR

            await Windows.Storage.FileIO.WriteTextAsync(file, output);
#else
            File.WriteAllText(filePath, output);
#endif

        }
        else
        {
            Debug.LogError("Cannot load task data!");
        }

    }

    async void LoadAllTasks()
    {
#if UNITY_WSA && !UNITY_EDITOR
        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile file = await storageFolder.CreateFileAsync(gameDataFileName,
                        Windows.Storage.CreationCollisionOption.OpenIfExists);
        string filePath = file.Path;
#else
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
#endif


        if (File.Exists(filePath))
            {
                string dataAsJson;
                while (true)
                {
                    dataAsJson = File.ReadAllText(filePath);
                    if (dataAsJson.Length>0)
                    {
                        break;
                    }
                }
                // Read the json from the file into a string
                
                // Pass the json to JsonUtility, and tell it to create a GameData object from it
                

                JObject obj = JObject.Parse(dataAsJson);

                JArray tasks = (JArray)obj["Tasks"];

                foreach (var task in tasks)
                {
                    m_Dropdown.options.Add(new Dropdown.OptionData() { text = task["Name"].ToString() });
                }

            }
            else
            {
                Debug.LogError("Cannot load task data!");
            }

       
    }
}
