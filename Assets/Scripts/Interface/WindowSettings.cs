using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Render
{
    public class WindowSettings
    {
        public char topBorder { get; set; }
        public char bottomBorder { get; set; }
        public char rightBorder { get; set; }
        public char leftBorder { get; set; }
        public char topLeftCorner { get; set; }
        public char topRightCorner { get; set; }
        public char bottomLeftCorner { get; set; }
        public char bottomRightCorner { get; set; }

        public WindowSettings()
        {

        }

        public WindowSettings(char leftBorder, char topBorder, char rightBorder, char bottomBorder, char topLeftCorner, char topRightCorner, char bottomRightCorner, char bottomLeftCorner)
        {
            this.topBorder = topBorder;
            this.bottomBorder = bottomBorder;
            this.rightBorder = rightBorder;
            this.leftBorder = leftBorder;
            this.topLeftCorner = topLeftCorner;
            this.topRightCorner = topRightCorner;
            this.bottomLeftCorner = bottomLeftCorner;
            this.bottomRightCorner = bottomRightCorner;
        }
    }
}