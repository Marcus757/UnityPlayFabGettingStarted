using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionCaller
{
    public System.Action functionNoParams;
    public System.Action<string> function1Param;
    public System.Action<string, string> function2Params;
    public System.Action<string, string, string> function3Params;
    public System.Action<string, string, string, string> function4Params;
    public List<string> parameters = new List<string>();
}
