using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionCaller : MonoBehaviour
{
    public System.Action functionNoParams;
    public System.Action<string> function1Param;
    public System.Action<string, string> function2Params;

    public List<string> parameters;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
