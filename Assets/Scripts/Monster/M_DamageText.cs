using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_DamageText : PoolAble
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
