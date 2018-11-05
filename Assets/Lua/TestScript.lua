require "GameLogic/GameWorld"
local GameWorld = GameWorld

TestScript = {}
local this = TestScript
-- local Debugger = LuaInterface.Debugger

function this.Start()
    Debugger.LogError("TestScript.Start")
    GameWorld.SayHi()
end

function this.Test()
    Debugger.LogError("This is a test method")
end

function this.Update()
    Debugger.LogError("TestScript.Update")
end

