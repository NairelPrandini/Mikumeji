using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;
using static Raylib_cs.Raylib;
public class Character
{
    Vector2 Position;
    bool Flipped = false;
    Texture2D Sprite;
    float Rotation = 0;
    Vector2 Velocity = Vector2.Zero;
    bool Grounded = false;

    public Rectangle Hitbox;

    public Character(Vector2 P, Texture2D T)
    {
        Position = P;
        Sprite = T;


    }

    public void Render()
    {


        Rectangle Source = new Rectangle(0, 0, Sprite.width, Sprite.height);

        if (Flipped)
            Source.width = Sprite.width * -1;
        else
            Source.width = Sprite.width * 1;



        Rectangle Dest = new Rectangle(Position.X, Position.Y, Sprite.width, Sprite.height);
        Vector2 Origin = new Vector2(Sprite.width / 2, Sprite.height);

        Hitbox = new Rectangle(Position.X - Origin.X, Position.Y - Origin.Y, Sprite.width, Sprite.height);



        Raylib.DrawTexturePro(Sprite, Source, Dest, Origin, Rotation, Color.WHITE);
    }


    float Gravity = 9.81f;

    float FallTime = 0;

    Rectangle ScreenBounds;

    public Vector2 ThrowVector = Vector2.Zero;






    public void Physics()
    {
        ScreenBounds = WinApiFunctions.GetWorkAreaRect();

        List<WinApiFunctions.DesktopWindow> WalkableWindows = new List<WinApiFunctions.DesktopWindow>();
        foreach (var W in WinApiFunctions.GetWindows())
        {
            if (W.Bounds.x == 0 && W.Bounds.y == 0 & W.Bounds.height >= GetMonitorHeight(0) && W.Bounds.width >= GetMonitorWidth(0)) continue;

            if (W.Title.Trim() == "Alternância de Tarefas") continue;
            if (Position.X > W.Bounds.x && Position.X < W.Bounds.width + W.Bounds.x)
            {
                WalkableWindows.Add(W);
            }
        }

        float MinY;

        if (WalkableWindows.Count > 0)
        {
            var BestWindow = WalkableWindows.OrderBy(o => o.Layer).First().Bounds;

            if (Position.Y - BestWindow.y >= -10 && Position.Y - BestWindow.y <= 10)
            {
                MinY = BestWindow.y;
            }
            else
            {
                MinY = ScreenBounds.height - ScreenBounds.y;
            }
        }
        else
        {
            MinY = ScreenBounds.height - ScreenBounds.y;
        }







        if (MinY == Position.Y)
        {
            if (Velocity.Y >= 0)
            {
                Velocity = Vector2.Zero;
                //Comment Below For Cool Behaviour (i have no idea why it just works)
                ThrowVector = Vector2.Zero;
                Grounded = true;
            }
        }
        else
        {
            Grounded = false;
        }

        if (Position.Y < 0)
            Grounded = false;
        if (Position.Y == ScreenBounds.height - ScreenBounds.y)
            Grounded = true;


        if (!OnDrag && !Grounded && 1 == 2)
        {
            ThrowVector *= 0.01f;
            Velocity = ThrowVector;
        }

        if (Grounded || OnDrag)
        {

            Velocity = Vector2.Zero;
            FallTime = 0;
        }

        Velocity.Y = (Gravity * MathF.Pow(FallTime, 2)) + ThrowVector.Y;
        Velocity.X = 0 + ThrowVector.X;
        Velocity.X = Math.Clamp(Velocity.X, -10, 10);
        Velocity.Y = Math.Clamp(Velocity.Y, -10, 10);

        if (!Grounded)
        {
            FallTime += GetFrameTime();
            Position += Velocity * 5;
        }

        Position.Y = Math.Clamp(Position.Y, -ScreenBounds.height, MinY);
        Position.X = Math.Clamp(Position.X, ScreenBounds.x, ScreenBounds.width);


    }


    Vector2 DragStartOffset = Vector2.Zero;
    bool OnDrag = false;



    public void Drag()
    {


        if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
        {
            if (CheckCollisionPointRec(GetMousePosition(), Hitbox))
            {
                DragStartOffset = Position - GetMousePosition();
                OnDrag = true;
            }
        }



        if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_RIGHT))
        {
            OnDrag = false;
        }

        if (OnDrag)
        {
            Position = DragStartOffset + GetMousePosition();
            ThrowVector = GetMouseDelta() * GetFrameTime() * 5;
        }


    }



}