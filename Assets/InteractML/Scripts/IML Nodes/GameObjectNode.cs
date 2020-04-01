﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    [NodeTint("#3A3B5B")]
    public class GameObjectNode : Node
    {
        /// <summary>
        /// The GameObject from the scene to use
        /// </summary>
        [Output] public GameObject GameObjectDataOut;
        

        [HideInInspector]
        public bool GameObjMissing;
        [HideInInspector]
        public bool state; 

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.GetAllNodes();
               
            }
            name = "GAME OBJECT";

        }

        void Start()
        {
           
        }

        public void OnDestroy()
        {
            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteGameObjectNode(this);
            }
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (GameObjectDataOut != null)
            {
                GameObjMissing = false;
                return GameObjectDataOut;
            }
            else
            {
                if ((graph as IMLController).IsGraphRunning)
                {
                    Debug.LogWarning("GameObject missing from GameObjectNode!!");
                }
                GameObjMissing = true;
                return null;
            }
        }
    }
}