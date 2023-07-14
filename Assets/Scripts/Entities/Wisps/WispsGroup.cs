using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispsGroup : MonoBehaviour
{
    public float orbitSpeed = 10;
    public float orbitDistance = 1.5f;
    public float selectedOrbitDistance = 0.8f;
    private List<Wisp> wisps;
    private Wisp _selectedWisp;
    private Wisp selectedWisp
    {
        get { return _selectedWisp; }
        set
        {
            _selectedWisp = value;
            if (value != null)
                _selectedWisp.transform.SetParent(GetComponentInParent<Player>().transform);
        }
    }

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

        if (selectedWisp)
        {
            Player player = GetComponentInParent<Player>();
            selectedWisp.SetTarget((Vector2)player.transform.position + player.AimedDirection() * selectedOrbitDistance);
        }
    }

    private void EqualizeWisps(int wispIndex = 0)
    {
        // Avoid division by zero
        if (wisps.Count == 0)
            return;

        float gap = 360.0f / wisps.Count;

        float startingAngle = Vector2.SignedAngle(Vector2.down, wisps[wispIndex].transform.localPosition);
        for (int i = wispIndex; i >= 0; i--)
            wisps[i].SetTarget(Quaternion.AngleAxis(startingAngle - gap * (wispIndex - i), Vector3.forward) * Vector2.down * orbitDistance, true);
        for (int i = wispIndex + 1; i < wisps.Count; i++)
            wisps[i].SetTarget(Quaternion.AngleAxis(startingAngle + gap * (i - wispIndex), Vector3.forward) * Vector2.down * orbitDistance, true);
    }

    private float GetWispAngle(Wisp wisp)
    {
        float res = Vector2.SignedAngle(Vector2.down, wisp.transform.localPosition);
        if (res < 0)
            return res + 360;
        return res;
    }

    private void InsertWispNaturally(Wisp wisp)
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
        // Insert wisp between others
        else
            wisps.Insert(wispIndex, wisp);
        EqualizeWisps(wispIndex);
    }

    public void AddWisp(Wisp wisp)
    {
        wisp.transform.SetParent(transform);
        if (selectedWisp == null)
            selectedWisp = wisp;
        else
            InsertWispNaturally(wisp);
    }

    private int GetNextActivableWispIndex(int defaultIndex = -1)
    {
        for (int i = 0; i < wisps.Count; i++)
            if (wisps[i].IsActivable())
                return i;
        return defaultIndex;
    }

    private int GetPreviousActivableWispIndex(int defaultIndex = -1)
    {
        for (int i = wisps.Count - 1; i >= 0; i--)
            if (wisps[i].IsActivable())
                return i;
        return defaultIndex;
    }

    public void DetachSelectedWisp()
    {
        if (wisps.Count == 0)
            selectedWisp = null;
        else
        {
            // Get the next activable wisp if any, or the next one if they are all in cooldown
            int wispIndex = GetNextActivableWispIndex(0);
            selectedWisp = wisps[wispIndex];
            wisps.RemoveAt(wispIndex);
        }
    }

    public void DetachWisp(Wisp wisp)
    {
        if (wisp == selectedWisp)
            DetachSelectedWisp();
        else
            wisps.Remove(wisp);
        wisp.transform.SetParent(null);
        EqualizeWisps();
    }

    public Wisp GetSelectedWisp()
    {
        return selectedWisp;
    }

    public void SelectNextWisp()
    {
        if (selectedWisp == null || wisps.Count == 0)
            return;
        int wispIndex = GetNextActivableWispIndex(0);

        for (int i = 0; i < wispIndex + 1; i++)
        {
            // Get the selected wisp back into the group
            selectedWisp.transform.SetParent(transform);
            wisps.Add(selectedWisp);
            // Select the next wisp
            selectedWisp = wisps[0];
            wisps.RemoveAt(0);

            // Equalize the orbiting wisps positions
            EqualizeWisps(wisps.Count - 1);
        }
    }

    public void SelectPreviousWisp()
    {
        if (selectedWisp == null || wisps.Count == 0)
            return;
        int wispIndex = GetPreviousActivableWispIndex(wisps.Count - 1);

        for (int i = wisps.Count - 1; i >= wispIndex; i--)
        {
            // Get the selected wisp back into the group
            selectedWisp.transform.SetParent(transform);
            wisps.Insert(0, selectedWisp);
            // Select the previous wisp
            selectedWisp = wisps[^1];
            wisps.RemoveAt(wisps.Count - 1);
            // Equalize the orbiting wisps positions
            EqualizeWisps(0);
        }
    }
}
