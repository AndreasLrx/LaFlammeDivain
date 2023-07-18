using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispsGroup : MonoBehaviour
{
    public float orbitSpeed = 10;
    public float orbitDistance = 1.5f;
    public float selectedOrbitDistance = 0.8f;
    // private List<Wisp> wisps;
    private List<WispStack> stacks;
    private WispStack _selectedStack;
    private WispStack selectedStack
    {
        get { return _selectedStack; }
        set
        {
            _selectedStack = value;
            if (value != null)
            {
                _selectedStack.transform.SetParent(GetComponentInParent<Player>().transform);
                stacks.Remove(value);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        stacks = new();
        EqualizeStacks();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, orbitSpeed) * Time.deltaTime);

        if (selectedStack)
        {
            Player player = GetComponentInParent<Player>();
            selectedStack.SetTarget((Vector2)player.transform.position + player.aimedDirection * selectedOrbitDistance);
        }
    }

    private void EqualizeStacks(int wispIndex = 0)
    {
        // Avoid division by zero
        if (stacks.Count == 0)
            return;

        float gap = 360.0f / stacks.Count;

        float startingAngle = Vector2.SignedAngle(Vector2.down, stacks[wispIndex].transform.localPosition);
        for (int i = wispIndex; i >= 0; i--)
            stacks[i].SetTarget(Quaternion.AngleAxis(startingAngle - gap * (wispIndex - i), Vector3.forward) * Vector2.down * orbitDistance, true);
        for (int i = wispIndex + 1; i < stacks.Count; i++)
            stacks[i].SetTarget(Quaternion.AngleAxis(startingAngle + gap * (i - wispIndex), Vector3.forward) * Vector2.down * orbitDistance, true);
    }

    private float GetObjectAngle(Transform transform)
    {
        float res = Vector2.SignedAngle(Vector2.down, transform.localPosition);
        if (res < 0)
            return res + 360;
        return res;
    }

    private void InsertStackNaturally(WispStack stack)
    {

        int stackIndex;
        // Avoid division by zero
        if (stacks.Count == 0)
            stackIndex = stacks.Count;
        else
        {
            float angle = GetObjectAngle(stack.transform) - GetObjectAngle(stacks[0].transform);
            if (angle < 0)
                angle += 360;
            stackIndex = (int)(angle / (360f / stacks.Count)) + 1;
        }

        // Append stack at the end
        if (stackIndex == stacks.Count)
            stacks.Add(stack);
        // Insert stack between others
        else
            stacks.Insert(stackIndex, stack);
        EqualizeStacks(stackIndex);
    }

    private WispStack GetAssociatedStack(Wisp wisp, bool createIfNotExisting = true)
    {
        if (selectedStack != null && selectedStack.IsWispTypeCompatible(wisp))
            return selectedStack;
        foreach (WispStack stack in stacks)
            if (stack.IsWispTypeCompatible(wisp))
                return stack;
        if (createIfNotExisting)
        {
            WispStack newStack = new GameObject("WispStack", typeof(WispStack)).GetComponent<WispStack>().Setup(wisp);
            newStack.transform.SetParent(transform);
            InsertStackNaturally(newStack);
            return newStack;
        }
        return null;
    }

    public void AddWisp(Wisp wisp)
    {
        WispStack stack = GetAssociatedStack(wisp);
        stack.AddWisp(wisp);
        if (selectedStack == null)
            selectedStack = stack;
    }

    private int GetNextActivableStackIndex(int defaultIndex = -1)
    {
        for (int i = 0; i < stacks.Count; i++)
            if (stacks[i].IsActivable())
                return i;
        return defaultIndex;
    }

    private int GetPreviousActivableStackIndex(int defaultIndex = -1)
    {
        for (int i = stacks.Count - 1; i >= 0; i--)
            if (stacks[i].IsActivable())
                return i;
        return defaultIndex;
    }

    private void DeleteStack(WispStack stack)
    {
        if (stack == selectedStack)
        {
            if (stacks.Count == 0)
                selectedStack = null;
            else
                SelectNextStack();
        }
        stacks.Remove(stack);
        Destroy(stack.gameObject);
        EqualizeStacks();
    }

    public void DetachWisp(Wisp wisp)
    {
        WispStack stack = GetAssociatedStack(wisp, false);

        if (stack == null)
            return;
        stack.DetachWisp(wisp);
        if (stack.WispsCount() == 0)
            DeleteStack(stack);
    }

    public bool ActivateSelectedWisp()
    {
        return selectedStack != null && selectedStack.ActivateSingle();
    }

    public bool ActivateSelectedStack()
    {
        return selectedStack != null && selectedStack.ActivateStack();
    }

    public void SelectNextStack()
    {
        if (selectedStack == null || stacks.Count == 0)
            return;
        int wispIndex = GetNextActivableStackIndex(0);

        for (int i = 0; i < wispIndex + 1; i++)
        {
            // Get the selected wisp back into the group
            selectedStack.transform.SetParent(transform);
            stacks.Add(selectedStack);
            // Select the next wisp
            selectedStack = stacks[0];

            // Equalize the orbiting wisps positions
            EqualizeStacks(stacks.Count - 1);
        }
    }

    public void SelectPreviousStack()
    {
        if (selectedStack == null || stacks.Count == 0)
            return;
        int wispIndex = GetPreviousActivableStackIndex(stacks.Count - 1);

        for (int i = stacks.Count - 1; i >= wispIndex; i--)
        {
            // Get the selected wisp back into the group
            selectedStack.transform.SetParent(transform);
            stacks.Insert(0, selectedStack);
            // Select the previous wisp
            selectedStack = stacks[^1];
            // Equalize the orbiting wisps positions
            EqualizeStacks(0);
        }
    }

    private bool CallWispOnDamage()
    {
        if (selectedStack == null)
            return false;
        bool absorbed = selectedStack.AbsorbDamage(false);

        foreach (WispStack stack in stacks)
            absorbed = stack.AbsorbDamage(absorbed) | absorbed;
        return absorbed;
    }

    public bool AbsorbDamage()
    {
        if (selectedStack == null)
            return false;
        // Trigger wisps OnDamage (they may absorb the damage)
        if (CallWispOnDamage())
            return true;

        // Absorb the damage at a cost of 1 wisp
        Destroy(selectedStack.PopWisp().gameObject);
        if (selectedStack.WispsCount() == 0)
            DeleteStack(selectedStack);
        return true;
    }
}
