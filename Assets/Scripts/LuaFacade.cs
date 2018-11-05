using UnityEngine;
using System.Collections;

namespace GameMain {
    //Lua层通过这里的接口调用C#方法
    public class LuaFacade
    {
        public static void SayHi() {
            Debug.LogError("hello C# -> lua:");
        }
    }
}

