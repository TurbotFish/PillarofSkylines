using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatrickTests
{
    public class CopyTestParent : MonoBehaviour
    {
        public CopyTestChild original;

        // Use this for initialization
        void Start()
        {
            original.TestInt = 5;

            var copy = Instantiate(original);
            copy.name = "Copy";
            //copy.TestInt = original.TestInt;

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
} //end of namespace