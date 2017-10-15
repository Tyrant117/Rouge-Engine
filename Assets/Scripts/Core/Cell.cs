using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Render
{
    public class Cell
    {
        public int X;
        public int Y;
        public Console owner;
        public int content = 0;
        public Color backgroundColor;
        public Color color;
        public Color animationColor;

        private int targetContent = 0;
        private Color targetColor;
        private float fadeLeft = 0f;
        private float fadeMax = 0f;
        private Color fadeColor;

        public void Clear()
        {
            SetContent(0, Color.clear, Color.clear);
            animationColor = Color.clear;
        }

        public void Clear(float fadeTime, Color fadeColor)
        {
            SetContent(0, Color.clear, Color.clear, fadeTime, fadeColor);
            animationColor = Color.clear;
        }

        public void SetContent(int content, Color backgroundColor, Color color)
        {
            SetContent(content, backgroundColor, color, 0f, color);
        }

        public void SetContent(int content, Color backgroundColor, Color color, float fadeMax, Color fadeColor)
        {
            // set target content and color
            targetContent = content;
            this.backgroundColor = backgroundColor;
            targetColor = color;

            // fade
            if (fadeMax > 0f)
            {
                this.fadeLeft = this.fadeMax = Random.Range(0f, fadeMax);
                this.color = this.fadeColor = fadeColor;
            }

            // instant
            else
            {
                this.fadeLeft = 0f;
                this.fadeMax = 0f;
            }

            // add cell to top layer
            if (targetContent != 0)
            {
                owner.DiryCell(this);
            }
        }

        public void SetBackgroundColor(Color background)
        {
            backgroundColor = background;
            owner.DiryCell(this);
        }

        public void StartAnimation(int content, Color animColor)
        {
            animationColor = animColor;
        }

        public void Update()
        {
            // fade
            if (fadeLeft > 0f)
            {
                content = targetContent;
                color = Color.Lerp(targetColor, fadeColor, RootConsole.s_Singleton.colorLerpCurve.Evaluate(fadeLeft / fadeMax));
                fadeLeft -= Time.deltaTime;
                owner.DiryCell(this);
            }
            else // fade finished
            {
                fadeLeft = 0f;
                content = targetContent;
                color = targetColor;
            }
        }
    }
}