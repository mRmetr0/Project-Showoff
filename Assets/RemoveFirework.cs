using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveFirework : MonoBehaviour
{
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
