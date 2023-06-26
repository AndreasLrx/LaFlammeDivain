using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispsGroup : MonoBehaviour
{
    public float orbitSpeed = 10;
    public float orbitDistance = 1.5f;
    public GameObject[] wisps;
    public int selectedWispIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        float gap = 360.0f / (float)wisps.Length;

        if (wisps.Length > 0)
            selectedWispIndex = 0;

        for (int i = 0; i < wisps.Length; i++) {
            Transform child = wisps[i].transform;
            child.position = Quaternion.AngleAxis(gap * i, Vector3.forward) * Vector2.down * orbitDistance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, orbitSpeed) * Time.deltaTime);
    }

    public Wisp GetSelectedWisp() {
        if (selectedWispIndex == -1)
            return null;
        return wisps[selectedWispIndex].GetComponent<Wisp>();
    }
}
