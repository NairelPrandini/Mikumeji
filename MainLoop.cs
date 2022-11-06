using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using static AuxiliarClass;

public class MainLoop
{


    public Character? MainCharacter;

    public void Init()
    {
        WinApiFunctions.SetScreenToWorkArea();
        WinApiFunctions.SetWindowParcialClickThrough();
        Vector2 SpawnPos = new Vector2(Raylib.GetScreenWidth() / 2, 400);
        MainCharacter = new Character(SpawnPos, Textures.MikuIdle);

    }

    public void Update()
    {

        WinApiFunctions.SetScreenToWorkArea();
        Raylib.DrawFPS(10, 10);
        if (MainCharacter != null)
        {
            MainCharacter.Physics();
            MainCharacter.Drag();
            MainCharacter.Render();
        }
    }



}