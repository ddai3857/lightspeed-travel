using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    protected Dictionary<Button, Coroutine> buttonCoroutines = new();
    protected Dictionary<VisualElement, Coroutine> visualCoroutines = new();
    protected Dictionary<Button, float> buttonBaseSize = new();

    protected float hoverSize = 15f;
    protected void MouseEnter(PointerEnterEvent evt)
    {
        Button curr = (Button)evt.currentTarget;

        if (!buttonBaseSize.ContainsKey(curr))
        {
            buttonBaseSize[curr] = curr.resolvedStyle.fontSize;
        }

        if (buttonCoroutines.ContainsKey(curr) && buttonCoroutines[curr] != null)
        {
            StopCoroutine(buttonCoroutines[curr]);
            buttonCoroutines[curr] = null;
        }

        buttonCoroutines[curr] = StartCoroutine(HoverButton(curr, true));
        curr.style.color = Color.lightBlue;
    }
    protected void MouseExit(PointerLeaveEvent evt)
    {
        Button curr = (Button)evt.currentTarget;
        if (buttonCoroutines.ContainsKey(curr) && buttonCoroutines[curr] != null)
        {
            StopCoroutine(buttonCoroutines[curr]);
            buttonCoroutines[curr] = null;
        }
        buttonCoroutines[curr] = StartCoroutine(HoverButton(curr, false));
        curr.style.color = Color.HSVToRGB(0,0,11/255f);
    }

    protected IEnumerator HoverButton (Button b, bool isGetsBigger)
    {
        // Change size
        float start = b.resolvedStyle.fontSize;
        float target = isGetsBigger ? buttonBaseSize[b] + hoverSize : buttonBaseSize[b];
        float maxTotalTime = 0.1f;
        float currTime = 0f;
        while (b.resolvedStyle.fontSize != target)
        {
            currTime += Time.unscaledDeltaTime;
            yield return null; // Next update
            b.style.fontSize = Mathf.Lerp(start, target, currTime / maxTotalTime);
        }
        yield break;
    }

    protected IEnumerator UIToggle(VisualElement v, bool toggleOn)
    {
        if (toggleOn)
        {
            v.style.display = DisplayStyle.Flex;
        }

        // Change size
        float start = toggleOn ? 0 : 1;
        float target = 1 - start;
        float maxTotalTime = 0.2f;
        float currTime = 0f;
        while (v.style.scale != new Vector2(1, target))
        {
            currTime += Time.unscaledDeltaTime;
            yield return null; // Next update
            v.style.scale =  new Vector2(1, Mathf.Lerp(start, target, currTime / maxTotalTime));
            v.style.opacity =  Mathf.Lerp(start, target, currTime / maxTotalTime);
        }

        if (!toggleOn)
        {
            v.style.display = DisplayStyle.None;
        }
        visualCoroutines[v] = null;
        yield break;
    }
}
