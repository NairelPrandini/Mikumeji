using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

public class MainLoop
{
    public Character MainCharacter = null;

    public void Init()
    {
        WinApiFunctions.SetScreenToWorkArea();
        WinApiFunctions.SetWindowParcialClickThrough();
        Vector2 SpawnPos = new Vector2(Raylib.GetScreenWidth() / 2, 400);
        MainCharacter = new Character(SpawnPos, Textures.MikuIdle);
    }

    public void Update()
    {



        MainCharacter.Physics();
        MainCharacter.Drag();
        MainCharacter.Render();

    }

}