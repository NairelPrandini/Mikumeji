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
        MainLoop _MainLoop = new MainLoop();
        // Raylib.SetTargetFPS(200);
        Raylib.InitWindow(1280, 720, "Mikumeji");
        Raylib.InitAudioDevice();
        _MainLoop.Init();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLANK);
            _MainLoop.Update();
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

}
