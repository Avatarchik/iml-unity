﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace InteractML.DataTypeNodes
{
    [NodeTint("#3A3B5B")]
    public class IntegerNode : Node
    {

        [SerializeField]
        public int Value;

        [Output]
        public int IntToOutput;

        public string ValueName;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            name = "INTEGER";

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            IntToOutput = Value;
            return IntToOutput;
        }

    }
}
