using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Linq;

/*
    usage:
    1. add [Track] to your class
    2. add [Save] to your field
    3. save (if the tracking cache is not innitialized, it will be initialized automatically)
    4. load (if the tracking cache is not innitialized, it will be initialized automatically)

    be aware:
    - unity classes are not serializable, so [Save] will not work on them.
    - you may need to use SetupSettersAndGetters() manually to properly save recently instantiated objects.
*/

public class SaveSystem : MonoBehaviour{

    public static string fileName = "save.data";
    public static string path = Application.persistentDataPath + "/" + fileName;
    public static List<System.Type> trackingCache;
    public static List<System.Func<object>> getters;
    public static List<System.Action<object>> setters;

    public static bool configured = false;

    public static void SetPath(string path, string fileName){
        SaveSystem.fileName = fileName;
        SaveSystem.path = path + "/" + fileName;
    }

    public static void Setup(){
        if(!configured) Debug.Log("<color=#00ffff>[SaveSystem::Cache]</color> Setting up (auto)");
        trackingCache = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(Track), false).Length > 0).ToList();
        Debug.Log("<color=#00ffff>[SaveSystem::Cache]</color> Tracking " + trackingCache.Count + " types");
        SetupSettersAndGetters();
        configured = true;
    }

    public static void Save(){
        Debug.Log("<color=#00ff00>[SaveSystem::Save]</color> Saving...");
        if(!configured) Setup();
        List<object> data = new List<object>();

        foreach(var getter in getters){
            data.Add(getter());
        }
        
        BinaryFormatter f = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        f.Serialize(stream, data);
       
        stream.Close();    
    }

    //writes a empty file to the path (no system file acess problems this way)
    public static void ClearSavedData(){
        Debug.Log("<color=#00ff00>[SaveSystem::ClearSavedData]</color> Clearing saved data...");
        List<object> data = new List<object>();
        
        BinaryFormatter f = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        f.Serialize(stream, data);
       
        stream.Close();    
    }

    public static void Load(){
        Debug.Log("<color=#00ff00>[SaveSystem::Load]</color> Loading...");
        if(!configured) Setup();
        if(File.Exists(path)){
            Debug.Log("<color=#00ff00>[SaveSystem::Load]</color> File found! Starting deserialization...");
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryFormatter f = new BinaryFormatter();

            List<object> data = (List<object>)f.Deserialize(stream);
            foreach(var setter in setters){
                setter(data[setters.IndexOf(setter)]);
            }

            stream.Close();

        }else{
            Debug.LogError("<color=#ff0000>[SaveSystem::LoadGame]</color> No save file found!");
        }
    }

    public static void SetupSettersAndGetters(){
        
        getters = new List<System.Func<object>>();
        setters = new List<System.Action<object>>();

        foreach(System.Type type in trackingCache){
            var objs = FindObjectsOfType(type);
            var fields = type.GetFields();
            foreach(var inst in objs){    
                foreach(FieldInfo field in fields){
                    if(field.GetCustomAttributes(typeof(Save), false).Length > 0){
                        getters.Add(() => field.GetValue(inst));
                        setters.Add(value => field.SetValue(inst, value));
                    }
                }
            }
        }
       
    }

}

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class Save : System.Attribute{}

[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class Track : System.Attribute{}


