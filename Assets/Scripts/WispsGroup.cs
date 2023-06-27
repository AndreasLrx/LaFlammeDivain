using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispsGroup : MonoBehaviour
{
    public float orbitSpeed = 10;
    public float orbitDistance = 1.5f;
    public List<GameObject> wisps;
    public int selectedWispIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        EqualizeWisps();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, orbitSpeed) * Time.deltaTime);
    }

    private void EqualizeWisps()
    {
        float gap = 360.0f / wisps.Count;

        if (wisps.Count > 0)
            selectedWispIndex = 0;

        for (int i = 0; i < wisps.Count; i++)
        {
            Transform child = wisps[i].transform;
            child.position = Quaternion.AngleAxis(gap * i, Vector3.forward) * Vector2.down * orbitDistance;
        }
    }

    public void AddWisp(GameObject wisp)
    {
        wisps.Add(wisp);
        wisp.transform.SetParent(transform);
        EqualizeWisps();
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
    }

    public Wisp GetSelectedWisp()
    {
        // No wisp left
        if (selectedWispIndex == -1)
            return null;
        return wisps[selectedWispIndex].GetComponent<Wisp>();
    }
}
