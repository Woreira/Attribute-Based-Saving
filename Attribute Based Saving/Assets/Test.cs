using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour{
    private void Update(){
        if(Input.GetKeyDown(KeyCode.S)){
            SaveSystem.Save();
        }
        if(Input.GetKeyDown(KeyCode.L)){
            SaveSystem.Load();
        }
        if(Input.GetKeyDown(KeyCode.C)){
            SaveSystem.Setup();
        }
    }
}