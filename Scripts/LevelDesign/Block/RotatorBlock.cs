using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorBlock : Block {

    public override void Initialize(int origin, int j, int k)
    {
        this.transform.position = new Vector3(0, IndexToNegative(j), 0);
        SetParentOrigin(origin);
        //if (origin == 1)
        //{
        //    this.transform.rotation = Quaternion.Euler(180f, 90f, 0f);
        //}
    }
}
