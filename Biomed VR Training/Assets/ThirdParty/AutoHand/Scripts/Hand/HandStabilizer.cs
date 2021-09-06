using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Autohand{
    public class HandStabilizer : MonoBehaviour{
        //This is the script that hides unstable joints without compromising joint functionality
        Hand[] hands;
        Vector3[] handsDeltaPos;

        void Start()
        {
#if UNITY_2020_1_OR_NEWER
            hands = FindObjectsOfType<Hand>(true);
#else
            hands = FindObjectsOfType<Hand>();
#endif
            handsDeltaPos = new Vector3[hands.Length];

            if (!GetComponent<Camera>().enabled)
                enabled = false;
        }

        void OnEnable(){
            if(GraphicsSettings.renderPipelineAsset != null){
                RenderPipelineManager.beginFrameRendering += OnPreRender;
                RenderPipelineManager.endCameraRendering += OnPostRender;
            }
        }

        void OnDisable(){
            if(GraphicsSettings.renderPipelineAsset != null){
                RenderPipelineManager.beginFrameRendering -= OnPreRender;
                RenderPipelineManager.endCameraRendering -= OnPostRender;
            }
        }
        
        private void OnPostRender() {
            if (!enabled)
                return;
            foreach(var hand in hands) {
                if(hand.gameObject.activeInHierarchy)
                    hand.OnPostRender();
            }
        }


        private void OnPreRender() {
            if (!enabled)
                return;

            foreach(var hand in hands) {
                if (hand.gameObject.activeInHierarchy)
                    hand.OnPreRender();
            }

        }

        private void OnPreRender(ScriptableRenderContext src, Camera[] cam) {
            foreach(var hand in hands) {
                if (hand.gameObject.activeInHierarchy)
                    hand.OnPreRender();
            }
        }

        private void OnPostRender(ScriptableRenderContext src, Camera cam) {
            foreach(var hand in hands) {
                if (hand.gameObject.activeInHierarchy)
                    hand.OnPostRender();
            }
        }
        
    }
}
