using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTracker : MonoBehaviour
{
    public Shot startingShot;
    [SerializeField] private Shot prevShot;

    void Start()
    {
        startingShot.CutToShot();
        prevShot = startingShot;
    }

    public Shot GetPrevShot()
    {
        return prevShot;
    }

    public void SetPrevShot(Shot newShot)
    {
        prevShot = newShot;
    }
}