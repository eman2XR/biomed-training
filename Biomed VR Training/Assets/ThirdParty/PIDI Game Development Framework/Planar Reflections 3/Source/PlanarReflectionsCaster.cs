/*
* PIDI - Planar Reflections™ 3 - Copyright© 2017-2020
* PIDI - Planar Reflections is a trademark and copyrighted property of Jorge Pinal Negrete.

* You cannot sell, redistribute, share nor make public this code, modified or not, in part nor in whole, through any
* means on any platform except with the purpose of contacting the developers to request support and only when taking
* all pertinent measures to avoid its release to the public and / or any unrelated third parties.
* Modifications are allowed only for internal use within the limits of your Unity based projects and cannot be shared,
* published, redistributed nor made available to any third parties unrelated to Irreverent Software by any means.
*
* For more information, contact us at support@irreverent-software.com
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlanarReflections3 {
    [ExecuteInEditMode]
    public class PlanarReflectionsCaster : MonoBehaviour {

        public PlanarReflectionsRenderer castFromRenderer;
        public bool[] castDepth = new bool[0];
        public bool[] castReflection = new bool[0];


        /// <summary> Whether this reflection renderer will work in SRP mode or not </summary>
        [SerializeField] protected bool SRPMode;
        
        public bool isSRP { get { return SRPMode; } }

#if UNITY_EDITOR
        public bool[] folds = new bool[16];
        private string version = "3.4";
        public string Version { get { return version; } }
#endif

        private Renderer rend;
        private Material[] sharedMats = new Material[0];

        private MaterialPropertyBlock mBlock;


        public void OnEnable() {
            rend = GetComponent<Renderer>();
            if (castDepth.Length!= GetComponent<Renderer>().sharedMaterials.Length)
                castDepth = new bool[GetComponent<Renderer>().sharedMaterials.Length];
            if (castReflection.Length!=castDepth.Length)
                castReflection = new bool[castDepth.Length];


#if UNITY_2019_1_OR_NEWER
            if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null) {
                SRPMode = true;
            }
            else {
                SRPMode = false;
            }
#endif
            if (SRPMode) {
#if UNITY_2019_1_OR_NEWER
                UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering -= CheckSRPVisibility;
                UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering += CheckSRPVisibility;
                this.gameObject.layer = 4;
#endif
            }
            else {
                Camera.onPreRender -= CheckVisibility;
                Camera.onPreRender += CheckVisibility;
            }
        }

#if UNITY_2019_1_OR_NEWER
        public void CheckSRPVisibility(UnityEngine.Rendering.ScriptableRenderContext context, Camera srcCamera) {
            CheckVisibility(srcCamera);

            for (int i = 0; i < castReflection.Length; i++) {
                var m = mBlock;

                GetPropertyBlock(ref m, i);


                m.SetTexture("_ReflectionTex", castFromRenderer.ReflectionTex && castReflection[i] && (srcCamera.cameraType != CameraType.Reflection && !srcCamera.name.Contains("PLANAR3_")) ? castFromRenderer.ReflectionTex : (Texture)Texture2D.blackTexture);


                m.SetTexture("_ReflectionDepth", castFromRenderer.ReflectionDepth && castDepth[i] && (srcCamera.cameraType != CameraType.Reflection && !srcCamera.name.Contains("PLANAR3_")) ? castFromRenderer.ReflectionDepth : (Texture)Texture2D.whiteTexture);


                SetPropertyBlock(m, i);

            }
        }
#endif

        public void CheckVisibility(Camera cam) {

            if ( mBlock == null ) {
                mBlock = new MaterialPropertyBlock();
            }

            if (castFromRenderer && rend.isVisible) {
                castFromRenderer.castersActive = true;
                
                for (int i = 0; i < castReflection.Length; i++) {
                    var m = mBlock;

                    GetPropertyBlock(ref m, i);


                    m.SetTexture("_ReflectionTex", castFromRenderer.ReflectionTex && castReflection[i] && (cam.cameraType != CameraType.Reflection) ? castFromRenderer.ReflectionTex : (Texture)Texture2D.blackTexture);


                    m.SetTexture("_ReflectionDepth", castFromRenderer.ReflectionDepth && castDepth[i] && (cam.cameraType != CameraType.Reflection ) ? castFromRenderer.ReflectionDepth : (Texture)Texture2D.whiteTexture);


                    SetPropertyBlock(m, i);

                }
            }
        }


        public void OnDisable() {
            if (SRPMode) {
#if UNITY_2019_1_OR_NEWER
                UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering -= CheckSRPVisibility;
#endif
            }
            else {
                Camera.onPreRender -= CheckVisibility;
            }
        }

        public void Update() {

            if (castFromRenderer && rend.isVisible) {
                

               
            }
        }



        void GetPropertyBlock(ref MaterialPropertyBlock block, int index) {
            if (!rend) {
                rend = GetComponent<Renderer>();
            }

            if (rend && sharedMats.Length < 1) {
                sharedMats = rend.sharedMaterials;
            }

            if (block == null || index < 0 || !rend || sharedMats.Length <= index) {
                return;
            }
            else {
#if UNITY_2018_1_OR_NEWER
                rend.GetPropertyBlock(block, index);
#else
                sharedMats = rend.sharedMaterials;
                var t = sharedMats[0];
                sharedMats[0] = sharedMats[index];
                rend.sharedMaterials = sharedMats;
                rend.GetPropertyBlock(block);
                sharedMats[0] = t;
                rend.sharedMaterials = sharedMats;
#endif
            }
        }


        void SetPropertyBlock(MaterialPropertyBlock block, int index) {

            if (!rend) {
                rend = GetComponent<Renderer>();
            }

            if (rend && sharedMats.Length < 1) {
                sharedMats = rend.sharedMaterials;
            }

            if (block == null || index < 0 || !rend || sharedMats.Length <= index) {
                return;
            }
            else {

#if UNITY_2018_1_OR_NEWER
					rend.SetPropertyBlock( block, index );
#else
                    sharedMats = rend.sharedMaterials;
                    var t = sharedMats[0];
                    sharedMats[0] = sharedMats[index];
                    rend.sharedMaterials = sharedMats;
                    rend.SetPropertyBlock(block);
                    sharedMats[0] = t;
                    rend.sharedMaterials = sharedMats;
#endif
                
            }
        }


    }

}