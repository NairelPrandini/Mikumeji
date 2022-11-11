using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

static class Program
{
    public static void Main()
    {

        Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_UNDECORATED);
        Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_ALWAYS_RUN);
        Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_TOPMOST);
        Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_TRANSPARENT);
        //  Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_HIGHDPI);

        MainLoop _MainLoop = new MainLoop();




        Raylib.SetTargetFPS(120);
        Raylib.InitWindow(1280, 720, "Mikumeji");
        Raylib.InitAudioDevice();

        _MainLoop.Init();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(0, 0, 0, 0));
            _MainLoop.Update();


            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

}
