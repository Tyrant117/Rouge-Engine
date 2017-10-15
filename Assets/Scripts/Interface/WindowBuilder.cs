using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Render
{
    public class WindowBuilder
    {
        public static void BuildWindow(Console target, char topBorder, char bottomBorder, char rightBorder, char leftBorder, char tlCorner, char trCorner, char blCorner, char brCorner)
        {
            for (int xHor = 1; xHor < target.GetWidth()-1; xHor++)
            {
                target.GetCell(xHor, 0).SetContent(topBorder, Color.clear, Color.blue);
                target.GetCell(xHor, target.GetHeight()-1).SetContent(bottomBorder, Color.clear, Color.blue);
            }

            for (int yVert = 1; yVert < target.GetHeight()-1; yVert++)
            {
                target.GetCell(0, yVert).SetContent(leftBorder, Color.clear, Color.blue);
                target.GetCell(target.GetWidth()-1, yVert).SetContent(rightBorder, Color.clear, Color.blue);
            }

            target.GetCell(0, 0).SetContent(tlCorner, Color.clear, Color.blue);
            target.GetCell(target.GetWidth() - 1, 0).SetContent(trCorner, Color.clear, Color.blue);
            target.GetCell(0, target.GetHeight() - 1).SetContent(blCorner, Color.clear, Color.blue);
            target.GetCell(target.GetWidth() - 1, target.GetHeight() - 1).SetContent(brCorner, Color.clear, Color.blue);

        }

        public static void BuildWindow(Console target, WindowSettings window)
        {
            for (int xHor = 1; xHor < target.GetWidth() - 1; xHor++)
            {
                target.GetCell(xHor, 0).SetContent(window.topBorder, Color.clear, Color.blue);
                target.GetCell(xHor, target.GetHeight() - 1).SetContent(window.bottomBorder, Color.clear, Color.blue);
            }

            for (int yVert = 1; yVert < target.GetHeight() - 1; yVert++)
            {
                target.GetCell(0, yVert).SetContent(window.leftBorder, Color.clear, Color.blue);
                target.GetCell(target.GetWidth() - 1, yVert).SetContent(window.rightBorder, Color.clear, Color.blue);
            }

            target.GetCell(0, 0).SetContent(window.topLeftCorner, Color.clear, Color.blue);
            target.GetCell(target.GetWidth() - 1, 0).SetContent(window.topRightCorner, Color.clear, Color.blue);
            target.GetCell(target.GetWidth() - 1, target.GetHeight() - 1).SetContent(window.bottomRightCorner, Color.clear, Color.blue);
            target.GetCell(0, target.GetHeight() - 1).SetContent(window.bottomLeftCorner, Color.clear, Color.blue);
        }
    }
}