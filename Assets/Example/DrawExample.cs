using Rougelikeberry.Render;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawExample : MonoBehaviour
{
    public Console console;

    private IEnumerator Start()
    {
        // wait until display has initialized
        while (!console.Initialized)
        {
            yield return null;
        }

        WindowBuilder.BuildWindow(console, (char)205, (char)205, (char)186, (char)186, (char)201, (char)187, (char)200, (char)188);

        StartCoroutine(RandomGrid());
    }

    public IEnumerator RandomGrid()
    {

        // fill random cells every frame on layer 1
        while (Application.isPlaying)
        {
            for (int i = 0; i < 40; i++)
            {
                var cell = console.GetCell(Random.Range(1, console.GetWidth()-1), Random.Range(1, console.GetHeight()-1));
                // random color and alpha
                Color color = Color.Lerp(Color.yellow, Color.green, Random.Range(0f, 1f));
                color = new Color(color.r, color.g, color.b, Random.Range(0f, 1f));

                cell.SetContent(Random.Range(48, 57) , Color.clear, color, Color.clear);
            }

            yield return null;
        }
    }
}
