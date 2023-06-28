using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispsGroup : MonoBehaviour
{
    public float orbitSpeed = 10;
    public float orbitDistance = 1.5f;
    public int selectedWispIndex = -1;
    private List<GameObject> wisps;

    // Start is called before the first frame update
    void Start()
    {
        wisps = new();
        EqualizeWisps();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, orbitSpeed) * Time.deltaTime);
    }

    private void EqualizeWisps(int wispIndex = 0)
    {
        float gap = 360.0f / wisps.Count;

        if (wisps.Count > 0)
            selectedWispIndex = 0;
        else
            return;

        float startingAngle = Vector2.SignedAngle(Vector2.down, wisps[wispIndex].transform.localPosition);
        for (int i = wispIndex; i >= 0; i--)
            wisps[i].GetComponent<Wisp>().SetTarget(Quaternion.AngleAxis(startingAngle - gap * (wispIndex - i), Vector3.forward) * Vector2.down * orbitDistance, true);
        for (int i = wispIndex + 1; i < wisps.Count; i++)
            wisps[i].GetComponent<Wisp>().SetTarget(Quaternion.AngleAxis(startingAngle + gap * (i - wispIndex), Vector3.forward) * Vector2.down * orbitDistance, true);
    }

    private float GetWispAngle(GameObject wisp)
    {
        float res = Vector2.SignedAngle(Vector2.down, wisp.transform.localPosition);
        if (res < 0)
            return res + 360;
        return res;
    }

    private void InsertWispNaturally(GameObject wisp)
    {
        int wispIndex;
        // Avoid division by zero
        if (wisps.Count == 0)
            wispIndex = wisps.Count;
        else
        {
            float angle = GetWispAngle(wisp) - GetWispAngle(wisps[0]);
            if (angle < 0)
                angle += 360;
            wispIndex = (int)(angle / (360f / wisps.Count)) + 1;
        }

        // Append wisp at the end
        if (wispIndex == wisps.Count)
            wisps.Add(wisp);
        // Insert wisp between others and update selectedWispIndex if required
        else
        {
            wisps.Insert(wispIndex, wisp);
            if (wispIndex <= selectedWispIndex)
                selectedWispIndex++;
        }
        EqualizeWisps(wispIndex);
    }

    public void AddWisp(GameObject wisp)
    {
        wisp.transform.SetParent(transform);
        InsertWispNaturally(wisp);
    }

    public void DetachWisp(GameObject wisp)
    {
        if (selectedWispIndex == -1)
            return;
        wisps[selectedWispIndex].transform.SetParent(null);
        wisps.Remove(wisp);
        if (wisps.Count == 0)
            selectedWispIndex = -1;
        else if (selectedWispIndex >= wisps.Count)
            selectedWispIndex--;
        EqualizeWisps();
    }

    public Wisp GetSelectedWisp()
    {
        // No wisp left
        if (selectedWispIndex == -1)
            return null;
        return wisps[selectedWispIndex].GetComponent<Wisp>();
    }
}
