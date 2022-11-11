using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static WinApiFunctions;
public class Character
{
    Vector2 Position;
    bool Flipped = false;
    Texture2D Sprite;
    Vector2 Velocity = Vector2.Zero;
    bool Grounded = false;
    Rectangle Hitbox;

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

        Raylib.DrawTexturePro(Sprite, Source, Dest, Origin, 0, Color.WHITE);
    }


    float Gravity = 9.81f;
    float FallTime = 0;
    Rectangle ScreenBounds;
    Vector2 ThrowVector;




    float MinY = 0;

    public void Physics()
    {
        ScreenBounds = WinApiFunctions.GetWorkAreaRect();

        List<Window> WalkableWindows = GetWindows().Where(W => Position.X > W.WRect.Left && Position.X < W.WRect.Right && Position.Y <= W.WRect.Top + 25 && W.WRect.Top >= 0).ToList();
        List<Window> RelevantWindows = GetWindows().Where(W => Position.X > W.WRect.Left && Position.X < W.WRect.Right).ToList();



        bool FullScreenActive = false;

        foreach (var W in GetWindows())
        {
            if (W.Bounds.y == -32000 && W.Bounds.x == -32000)
            {
                FullScreenActive = true;
            }
        }


        if (FullScreenActive)
        {
            WinApiFunctions.SetScreenToMonitorArea();
            MinY = Raylib.GetMonitorHeight(0);
        }

        if (!FullScreenActive)
        {
            WinApiFunctions.SetScreenToWorkArea();

            if (WalkableWindows.Count > 0)
            {
                var BestWindow = WalkableWindows.OrderBy(o => o.Bounds.y).First();

                bool BestWindowWalkable = true;

                foreach (var W in RelevantWindows)
                {
                    if (BestWindow.Layer > W.Layer)
                    {
                        if (BestWindow.WRect.Top > W.WRect.Top && BestWindow.WRect.Top < W.WRect.Bottom)
                        {
                            BestWindowWalkable = false;
                        }
                    }
                }

                if (BestWindowWalkable && Position.Y > 0)
                    MinY = BestWindow.Bounds.y;
                else
                    MinY = ScreenBounds.height - ScreenBounds.y;

            }
            else
            {
                MinY = ScreenBounds.height - ScreenBounds.y;
            }

        }
        else
        {
            MinY = GetMonitorHeight(0);
        }




        if (MinY == Position.Y && Velocity.Y >= 0 && Position.Y >= 0)
        {
            Velocity = Vector2.Zero;
            ThrowVector = Vector2.Zero;
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }


        if (Grounded || OnDrag)
        {
            Velocity = Vector2.Zero;
            FallTime = 0;
        }

        Velocity.Y = (Gravity * MathF.Pow(FallTime, 2) + ThrowVector.Y) * GetFrameTime() * 100;
        Velocity.X = ThrowVector.X * GetFrameTime() * 100;
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




    Vector2 DragStartOffset;
    bool OnDrag;

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
            ThrowVector = GetMouseDelta() * GetFrameTime() * 7.5f;
        }

    }



}