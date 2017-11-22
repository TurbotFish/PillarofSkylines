using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatrickTests
{
    public class CopyTestChild : MonoBehaviour
    {
        [SerializeField]
        int testInt;
        public int TestInt { get { return this.testInt; } set { this.testInt = value; } }

        void Awake()
        {
            
        }

        // Use this for initialization
        void Start()
        {
            Debug.LogErrorFormat("CopyTestChild: awake: name={0}, testInt={1}", this.name, this.testInt);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
} //end of namespace