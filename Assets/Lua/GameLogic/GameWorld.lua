--GameWorld<LuaFacade> Lua -> C#
--CSFacade  C# -> Lua
GameWorld = {}
local this = GameWorld
local LuaFacade = GameMain.LuaFacade

function GameWorld.SayHi()
    LuaFacade.SayHi()
end
