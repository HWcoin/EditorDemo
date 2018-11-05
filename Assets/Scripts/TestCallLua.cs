using UnityEngine;
using LuaInterface;

public class TestCallLua : MonoBehaviour {
    LuaState lua = null;
    LuaFunction luaFunc = null;
  
	// Use this for initialization
	void Start () {
        new LuaResLoader();
        lua = new LuaState();
        lua.Start();

        string luaPath = Application.dataPath + "/Lua";//注意这里的文件位置
        lua.AddSearchPath(luaPath);

        LuaBinder.Bind(lua);

        
        /*
        lua.DoString(script);
        luaFunc = lua.GetFunction("test.luaFunc");

        if (luaFunc != null)
        {
            int num = luaFunc.Invoke<int, int>(123456);
            Debugger.Log("generic call return: {0}", num);

            num = CallFunc();
            Debugger.Log("expansion call return: {0}", num);

            num = lua.Invoke<int, int>("test.luaFunc", 123456, true);
            Debugger.Log("luastate call return: {0}", num);
        }
        */
        lua.DoFile("TestScript.lua");
        CallFunc("TestScript.Start", gameObject);

        lua.DoFile("GameLogic/CSFacade");
        CallFunc("CS_SAY_HELLO", gameObject);
	}

    int CallFunc()
    {
        luaFunc.BeginPCall();
        luaFunc.Push(123456);
        luaFunc.PCall();
        int num = (int)luaFunc.CheckNumber();
        luaFunc.EndPCall();
        return num;
    }
	
	// Update is called once per frame
	void Update () {
        //CallFunc("TestScript.Update", gameObject);
	}

    void CallFunc(string func, GameObject obj)
    {
        luaFunc = lua.GetFunction(func);
        if (luaFunc != null) {
            luaFunc.Call(obj);
            luaFunc.Dispose();
            luaFunc = null;
        }
        
    }

    private void OnApplicationQuit()
    {
        lua.Dispose();
        lua = null;
    }
}
