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


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#if PLANAR3_PRO && UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

using System.IO;
using System.Collections.Generic;


namespace PlanarReflections3 {

    public enum ResolutionMode { ScreenBased, ExplicitValue }

    public enum ReflectionClipMode { AccurateClipping, SimpleApproximation }

    public enum PostFXSettingsMode { CopyFromCamera, CustomSettings }


    [System.Serializable]
    public class InternalReflectionRenderer {

        /// <summary> The internal camera that actually renders the reflection </summary>
        public Camera refCamera;

#if PLANAR3_URP
        public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData camData;
#endif

        public RenderTexture assignedTexture;

#if PLANAR3_PRO || PLANAR3_LWRP || PLANAR3_URP || PLANAR3_HDRP

        public Camera depthCamera;

#endif

    }

    [System.Serializable]
    public class ReflectionSettings {

        /// <summary> The way in which the renderer's resolution is handled, either based on the screen resolution or a manual value </summary>
        public ResolutionMode resolutionMode = ResolutionMode.ScreenBased;

        /// <summary> The way the reflection's projection and clipping will be handled, either with an accurate clipping limited to the surface of the plane or with a simplified approximation </summary>
        public ReflectionClipMode reflectionClipMode = ReflectionClipMode.AccurateClipping;

        /// <summary> The explicit resolution to be assigned to this reflection </summary>
        public Vector2 explicitResolution = new Vector2( 1024, 1024 );

        /// <summary> The final scale for the resolution to be multiplied by </summary>
        public float resolutionDownscale = 0.5f;

        /// <summary> The amount to frames to wait before the reflection is re-drawn and updated </summary>
        public int targetFramerate = 0;

        ///<summary> If enabled, additional components attached to this reflection Renderer will be added to the reflection itself </summary>
        public bool useCustomComponents;

        ///<summary> The list of components, as strings, that will be tracked and synchronized </summary>
        public string[] customComponentNames = new string[0];

        /// <summary> If enabled, a specified array of custom components added to this reflective instance will be synched automatically to all the rendered reflections </summary>
        public bool autoSynchComponents = false;

        /// <summary> The near clip distance of the reflection's renderer </summary>
        public float nearClipDistance = 0.05f;

        /// <summary> The far clip distance of the reflection's renderer </summary>
        public float farClipDistance = 100.0f;

        /// <summary> Whether the shadow distance from the Quality Settings should be overriden by this reflection </summary>
        public bool customShadowDistance = false;

        /// <summary> Whether this reflection will be updated only while a Reflection Caster on the scene is using its output texture </summary>
        public bool updateOnCastOnly = false;

        /// <summary> The distance in which shadows are rendered for this reflection </summary>
        public float shadowDistance = 25.0f;

        /// <summary> The rendering path used by the reflection renderer </summary>
        public RenderingPath renderingPath = RenderingPath.Forward;

        ///<summary> Sets the amount of pixel lights that this reflection will render. If set to -1, the number of pixel lights set on the Quality Settings will be used </summary>
        public int pixelLights = -1;

        ///<summary> The layers that this reflection will render </summary>
        public LayerMask reflectLayers = 1;

        /// <summary> If enabled, only cameras with a certain tag will trigger the rendering process of this reflection </summary>
        public bool trackCamerasWithTag = false;

        /// <summary> The tag to look for if the "trackCamerasWithTag" setting is enabled </summary>
        public string CamerasTag = "MainCamera";


#if UNITY_2019_3_OR_NEWER && (PLANAR3_PRO && ( PLANAR3_URP || PLANAR3_HDRP ) )
        //UNIVERSAL RP & HDRP DEFINITIONS GO HERE
#elif UNITY_2019_1_OR_NEWER && (PLANAR3_PRO || PLANAR3_LWRP || PLANAR3_URP )
        //LWRP DEFINITIONS GO HERE
#endif

    }

#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public class PlanarReflectionsRenderer : MonoBehaviour {

#if UNITY_EDITOR

        public Texture2D sceneIcon;

        public bool displaySceneReflector = true;

        public Mesh defaultReflectorMesh;

        public Material defaultReflectorMaterial;
#if UNITY_2019_1_OR_NEWER && (PLANAR3_PRO || PLANAR3_LWRP  || PLANAR3_URP || PLANAR3_HDRP)
        public Material defaultSRPReflectorMaterial;
#endif

        public bool[] folds = new bool[16];

#endif

        private string version = "3.4";

        private float frameTime;

        ///<summary> The current version of the PlanarReflectionsRenderer component </summary>
        public string Version { get { return version; } }

        /// <summary> The static reflection settings shared across all reflection renderers </summary>
        protected static ReflectionSettings globalReflectionSettings = new ReflectionSettings();

        /// <summary> The static reflection settings shared across all reflection renderers </summary>
        public ReflectionSettings GlobalSettings { get { return globalReflectionSettings; } }

        /// <summary> The specific settings of this reflection renderer </summary>
        [SerializeField] protected ReflectionSettings settings = new ReflectionSettings();

        /// <summary> The specific settings of this reflection renderer </summary>
        public ReflectionSettings Settings { get { return settings; } }

        /// <summary> An external RenderTextureAsset that will store the rendered reflection's texture </summary>
        [SerializeField] protected RenderTexture externalReflectionTex;

#if UNITY_EDITOR
        /// <summary> An external RenderTextureAsset that will store the rendered reflection's texture </summary>
        public RenderTexture ExternalReflectionTex { set { externalReflectionTex = value; } get { return externalReflectionTex; } }
#endif

        /// <summary> An external RenderTextureAsset that will store the rendered reflection's depth </summary>
        [SerializeField] protected RenderTexture externalReflectionDepth;

#if UNITY_EDITOR
        /// <summary> An external RenderTextureAsset that will store the rendered reflection's depth </summary>
        public RenderTexture ExternalReflectionDepth { set { externalReflectionDepth = value; } get { return externalReflectionDepth; } }
#endif

        /// <summary> An internal RenderTexture generated to store the rendered reflection's texture </summary>
        protected RenderTexture reflectionTex;

        /// <summary> The output reflection texture that can be used by the Planar Reflection Casters  </summary>
        public RenderTexture ReflectionTex { get { return externalReflectionTex ? externalReflectionTex : reflectionTex; } }

        /// <summary> An internal RenderTexture generated to store the rendered reflection's depth </summary>
        protected RenderTexture reflectionDepth;

        /// <summary> the output reflection depth texture that can be used by the Planar Reflection Casters </summary>
        public RenderTexture ReflectionDepth { get { return externalReflectionDepth ? externalReflectionDepth : reflectionDepth; } }

        /// <summary> Whether this reflection renderer will work in SRP mode or not </summary>
        [SerializeField] protected bool SRPMode;

        public Shader internalDepthShader;

        public bool isSRP { get { return SRPMode; } }

        public bool castersActive;

        private Dictionary<Camera, InternalReflectionRenderer> gameReflectors = new Dictionary<Camera, InternalReflectionRenderer>();

        private InternalReflectionRenderer sceneViewReflector = new InternalReflectionRenderer();

        [SerializeField] protected List<RenderTexture> releasables = new List<RenderTexture>();


#if UNITY_EDITOR
        [MenuItem( "GameObject/Effects/Planar Reflections 3/Create Reflections Renderer", priority = -99 )]
        public static void CreateReflectionsRendererObject() {

            var reflector = new GameObject( "Reflection Renderer", typeof( PlanarReflectionsRenderer ) );
            reflector.transform.position = Vector3.zero;
            reflector.transform.rotation = Quaternion.identity;

        }
#endif


        public void OnEnable() {

            var cams = Resources.FindObjectsOfTypeAll<Camera>();

            foreach ( Camera cam in cams ) {
                if ( cam.name.Contains( "PLANAR3_" ) && !SRPMode ) {
                    RenderTexture.ReleaseTemporary( cam.targetTexture );
                    cam.targetTexture = null;
                    DestroyImmediate( cam.gameObject );
                }
            }

#if UNITY_2019_1_OR_NEWER && (PLANAR3_PRO || PLANAR3_LWRP  || PLANAR3_URP || PLANAR3_HDRP)
            if ( UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null ) {
                SRPMode = true;
            }
            else {
                SRPMode = false;
            }
#endif



            if ( SRPMode ) {
#if UNITY_2019_1_OR_NEWER && (PLANAR3_PRO || PLANAR3_LWRP  || PLANAR3_URP || PLANAR3_HDRP)

                foreach ( Camera cam in cams ) {
                    if ( cam.name.Contains( "PLANAR3_" ) ) {
                        cam.targetTexture = null;
                        DestroyImmediate( cam.gameObject );
                    }
                }

                UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering -= SRPRenderReflection;
                UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering += SRPRenderReflection;

                UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering -= CleanupTextures;
                UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering += CleanupTextures;

                this.gameObject.layer = 4;
#endif
            }
            else {
                Camera.onPreCull -= RenderReflection;
                Camera.onPreCull += RenderReflection;
                this.gameObject.layer = 4;
            }
        }


#if UNITY_EDITOR
        private void DrawReflectorMesh( Camera sceneCam ) {

            if ( !displaySceneReflector )
                return;

            var matrix = new Matrix4x4();
            matrix.SetTRS( transform.position, transform.rotation, Vector3.one * 10 );

            if ( SRPMode ) {
#if UNITY_2019_1_OR_NEWER && (PLANAR3_PRO || PLANAR3_LWRP  || PLANAR3_URP || PLANAR3_HDRP)
                if ( !defaultSRPReflectorMaterial ) {
                    defaultSRPReflectorMaterial = AssetDatabase.LoadAssetAtPath<Material>( AssetDatabase.GUIDToAssetPath( AssetDatabase.FindAssets( "Planar3_DefaultSRPReflectorMaterial" )[0] ) );
                }
                var mBlock = new MaterialPropertyBlock();

                mBlock.SetTexture( "_ReflectionTex", ReflectionTex );
                Graphics.DrawMesh( defaultReflectorMesh, matrix, defaultSRPReflectorMaterial, 0, sceneCam, 0, mBlock );
#endif
            }
            else {
                if ( !defaultReflectorMaterial ) {
                    defaultReflectorMaterial = AssetDatabase.LoadAssetAtPath<Material>( AssetDatabase.GUIDToAssetPath( AssetDatabase.FindAssets( "Planar3_DefaultReflectorMaterial" )[0] ) );
                }
                var mBlock = new MaterialPropertyBlock();

                mBlock.SetTexture( "_ReflectionTex", ReflectionTex );
                Graphics.DrawMesh( defaultReflectorMesh, matrix, defaultReflectorMaterial, 0, sceneCam, 0, mBlock );
            }
        }
#endif

#if UNITY_2019_1_OR_NEWER && (PLANAR3_PRO || PLANAR3_LWRP || PLANAR3_URP || PLANAR3_HDRP)
        public void SRPRenderReflection( UnityEngine.Rendering.ScriptableRenderContext context, Camera[] srcCamera ) {
            for ( int i = 0; i < srcCamera.Length; i++ ) {
                if ( !srcCamera[i].name.Contains( "PLANAR3_" ) )
                    RenderReflection( srcCamera[i] );
            }


        }


        public void CleanupTextures( UnityEngine.Rendering.ScriptableRenderContext context, Camera[] srcCamera ) {
            for (int i = 0; i < releasables.Count; i++ ) {
                if ( releasables[i] != null ) {
                    RenderTexture.ReleaseTemporary( releasables[i] );
                    releasables.RemoveAt( i );
                    if (i > 0 ) {
                        i--;
                    }
                }
            }
        }




#endif



        public void RenderReflection( Camera srcCamera ) {

            if ( Settings.updateOnCastOnly && !castersActive && Application.isPlaying ) {
                return;
            }

            if ( Settings.trackCamerasWithTag ) {
                if ( srcCamera.tag != Settings.CamerasTag && srcCamera.cameraType != CameraType.SceneView ) {
                    return;
                }
            }

            if ( srcCamera.cameraType == CameraType.Reflection || srcCamera.cameraType == CameraType.Preview ) {
                return;
            }

            InternalReflectionRenderer currentReflector = null;


            if ( srcCamera.depthTextureMode == DepthTextureMode.None ) {
                srcCamera.depthTextureMode = DepthTextureMode.Depth;
            }


            if ( srcCamera.cameraType == CameraType.SceneView ) {

                if ( !sceneViewReflector.refCamera ) {
                    sceneViewReflector.refCamera = new GameObject( "PLANAR3_SCENEVIEW", typeof( Camera ) ).GetComponent<Camera>();
                    sceneViewReflector.refCamera.gameObject.hideFlags = sceneViewReflector.refCamera.hideFlags = HideFlags.HideAndDontSave;
                }

                currentReflector = sceneViewReflector;


            }
            else if ( srcCamera.cameraType == CameraType.Game ) {

                if ( !gameReflectors.ContainsKey( srcCamera ) || gameReflectors[srcCamera] == null ) {
                    gameReflectors.Add( srcCamera, new InternalReflectionRenderer() );
                }


                if ( !gameReflectors[srcCamera].refCamera ) {
                    gameReflectors[srcCamera].refCamera = new GameObject( "PLANAR3_GAMECAM_" + srcCamera.GetInstanceID(), typeof( Camera ) ).GetComponent<Camera>();
                    gameReflectors[srcCamera].refCamera.gameObject.hideFlags = gameReflectors[srcCamera].refCamera.hideFlags = HideFlags.HideAndDontSave;
                }
                else {
                    gameReflectors[srcCamera].refCamera.gameObject.hideFlags = gameReflectors[srcCamera].refCamera.hideFlags = HideFlags.HideAndDontSave;
                }

                currentReflector = gameReflectors[srcCamera];



            }

            if ( currentReflector == null ) {
                return;
            }

            currentReflector.refCamera.CopyFrom( srcCamera );


            if ( !SRPMode )
                currentReflector.refCamera.cameraType = CameraType.Reflection;

            if ( currentReflector.refCamera.clearFlags == CameraClearFlags.Nothing || currentReflector.refCamera.clearFlags == CameraClearFlags.Depth ) {
                currentReflector.refCamera.clearFlags = CameraClearFlags.Color;
            }

            currentReflector.refCamera.enabled = SRPMode && Settings.targetFramerate == 0;


            if ( !externalReflectionTex ) {

                if ( !SRPMode ) {
                    if ( reflectionTex ) {
                        RenderTexture.ReleaseTemporary( reflectionTex );
                    }

                    int width = (int)((Settings.resolutionMode == ResolutionMode.ExplicitValue ? Settings.explicitResolution.x : srcCamera.pixelWidth) * Settings.resolutionDownscale);
                    int height = (int)((Settings.resolutionMode == ResolutionMode.ExplicitValue ? Settings.explicitResolution.y : srcCamera.pixelHeight) * Settings.resolutionDownscale);

                    reflectionTex = RenderTexture.GetTemporary( width, height, 24 );

                }
                else {
                    if ( !currentReflector.assignedTexture ) {
                        int width = (int)((Settings.resolutionMode == ResolutionMode.ExplicitValue ? Settings.explicitResolution.x : srcCamera.pixelWidth) * Settings.resolutionDownscale);
                        int height = (int)((Settings.resolutionMode == ResolutionMode.ExplicitValue ? Settings.explicitResolution.y : srcCamera.pixelHeight) * Settings.resolutionDownscale);


                        currentReflector.assignedTexture = RenderTexture.GetTemporary( width, height, 24 );
                        reflectionTex = currentReflector.assignedTexture;
                    }
                    else {



                        int width = (int)((Settings.resolutionMode == ResolutionMode.ExplicitValue ? Settings.explicitResolution.x : srcCamera.pixelWidth) * Settings.resolutionDownscale);
                        int height = (int)((Settings.resolutionMode == ResolutionMode.ExplicitValue ? Settings.explicitResolution.y : srcCamera.pixelHeight) * Settings.resolutionDownscale);

                        if ( !currentReflector.assignedTexture || currentReflector.assignedTexture.width != width || currentReflector.assignedTexture.height != height ) {

                            if ( currentReflector.assignedTexture != null && !releasables.Contains( currentReflector.assignedTexture ) ) {
                                releasables.Add( currentReflector.assignedTexture );
                            }

                            currentReflector.refCamera.targetTexture = null;

                            currentReflector.assignedTexture = RenderTexture.GetTemporary( width, height, 24 );
                        }

                        reflectionTex = currentReflector.assignedTexture;
                    }
                }
            }


            var tempShadowDistance = QualitySettings.shadowDistance;

            if ( !SRPMode && Settings.customShadowDistance )
                QualitySettings.shadowDistance = Settings.shadowDistance;



            currentReflector.refCamera.targetTexture = ReflectionTex;

            currentReflector.refCamera.aspect = srcCamera.aspect;

            currentReflector.refCamera.rect = new Rect( 0, 0, 1, 1 );

            Vector3 worldSpaceViewDir = srcCamera.transform.forward;
            Vector3 worldSpaceViewUp = srcCamera.transform.up;
            Vector3 worldSpaceCamPos = srcCamera.transform.position;

            Vector3 planeSpaceViewDir = transform.InverseTransformDirection( worldSpaceViewDir );
            Vector3 planeSpaceViewUp = transform.InverseTransformDirection( worldSpaceViewUp );
            Vector3 planeSpaceCamPos = transform.InverseTransformPoint( worldSpaceCamPos );

            planeSpaceViewDir.y *= -1.0f;
            planeSpaceViewUp.y *= -1.0f;
            planeSpaceCamPos.y *= -1.0f;

            worldSpaceViewDir = transform.TransformDirection( planeSpaceViewDir );
            worldSpaceViewUp = transform.TransformDirection( planeSpaceViewUp );
            worldSpaceCamPos = transform.TransformPoint( planeSpaceCamPos );

            currentReflector.refCamera.transform.position = worldSpaceCamPos;
            currentReflector.refCamera.transform.LookAt( worldSpaceCamPos + worldSpaceViewDir, worldSpaceViewUp );

            currentReflector.refCamera.nearClipPlane = Settings.nearClipDistance;
            currentReflector.refCamera.farClipPlane = Settings.farClipDistance;




            currentReflector.refCamera.renderingPath = Settings.renderingPath;

            currentReflector.refCamera.cullingMask = Settings.reflectLayers;



            //Global fix for Screen position out of frustum, Camera assertion failed and similar errors.
            currentReflector.refCamera.transform.Rotate( 0.01f, 0.01f, 0.01f );
            currentReflector.refCamera.transform.Translate( 0.002f, 0.002f, 0.002f );

            if ( settings.reflectionClipMode == ReflectionClipMode.AccurateClipping ) {
                currentReflector.refCamera.projectionMatrix = currentReflector.refCamera.CalculateObliqueMatrix( CameraSpacePlane( currentReflector.refCamera, transform.position, transform.up ) );
            }


            if ( !SRPMode ) {
                if ( !Application.isPlaying || Settings.targetFramerate == 0 ) {

                    currentReflector.refCamera.Render();
                }
                else if ( srcCamera.cameraType != CameraType.SceneView && Application.isPlaying ) {

                    if ( Time.realtimeSinceStartup > frameTime ) {
                        frameTime = Time.realtimeSinceStartup + (1.0f / Settings.targetFramerate);
                        currentReflector.refCamera.Render();
                    }
                }
            }
            else {
                if ( !Application.isPlaying || Settings.targetFramerate == 0 ) {
                    currentReflector.refCamera.enabled = true;
#if PLANAR3_URP
                    if ( !currentReflector.camData ) {
                        if ( currentReflector.refCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>() ) {
                            currentReflector.camData = currentReflector.refCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                        }
                        else {
                            currentReflector.camData = currentReflector.refCamera.gameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                        }
                    }
#endif
                }
                else {

                    if ( Time.realtimeSinceStartup > frameTime ) {
                        frameTime = Time.realtimeSinceStartup + (1.0f / Settings.targetFramerate);
                        currentReflector.refCamera.enabled = true;
#if PLANAR3_URP
                        if ( !currentReflector.camData ) {
                            currentReflector.camData = currentReflector.refCamera.gameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                        }

                        currentReflector.camData.requiresDepthOption = CameraOverrideOption.Off;
                        currentReflector.camData.requiresColorOption = CameraOverrideOption.Off;
                        currentReflector.camData.renderShadows = Settings.shadowDistance > 0;
                    }
                    else {
                        currentReflector.refCamera.enabled = false;
                        currentReflector.refCamera.allowMSAA = false;
                        if ( !currentReflector.camData ) {
                            currentReflector.camData = currentReflector.refCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
                        }

                        currentReflector.camData.requiresDepthOption = CameraOverrideOption.Off;
                        currentReflector.camData.requiresColorOption = CameraOverrideOption.Off;
                        currentReflector.camData.renderShadows = Settings.shadowDistance > 0;
#endif
                    }

                }

                currentReflector.refCamera.depth = -999;
            }



            QualitySettings.shadowDistance = tempShadowDistance;

#if UNITY_EDITOR
            if ( srcCamera.cameraType == CameraType.SceneView )
                DrawReflectorMesh( srcCamera );
#endif


        }



        public void LateUpdate() {
            castersActive = false;

            var cams = gameReflectors.Keys;

            foreach ( Camera cam in cams ) {
                if ( cam != null ) {

                    Vector3 worldSpaceViewDir = cam.transform.forward;
                    Vector3 worldSpaceViewUp = cam.transform.up;
                    Vector3 worldSpaceCamPos = cam.transform.position;

                    Vector3 planeSpaceViewDir = transform.InverseTransformDirection( worldSpaceViewDir );
                    Vector3 planeSpaceViewUp = transform.InverseTransformDirection( worldSpaceViewUp );
                    Vector3 planeSpaceCamPos = transform.InverseTransformPoint( worldSpaceCamPos );

                    planeSpaceViewDir.y *= -1.0f;
                    planeSpaceViewUp.y *= -1.0f;
                    planeSpaceCamPos.y *= -1.0f;

                    worldSpaceViewDir = transform.TransformDirection( planeSpaceViewDir );
                    worldSpaceViewUp = transform.TransformDirection( planeSpaceViewUp );
                    worldSpaceCamPos = transform.TransformPoint( planeSpaceCamPos );

                    if ( gameReflectors[cam].refCamera ) {
                        gameReflectors[cam].refCamera.transform.position = worldSpaceCamPos;
                        gameReflectors[cam].refCamera.transform.LookAt( worldSpaceCamPos + worldSpaceViewDir, worldSpaceViewUp );


                        if ( Settings.useCustomComponents ) {
                            var comps = new List<string>( Settings.customComponentNames );
                            foreach ( Component c in gameObject.GetComponents( typeof( Component ) ) ) {
                                if ( comps.Contains( c.GetType().Name ) ) {
                                    if ( !gameReflectors[cam].refCamera.GetComponent( c.GetType() ) ) {
                                        var copy = gameReflectors[cam].refCamera.gameObject.AddComponent( c.GetType() );
                                        System.Reflection.FieldInfo[] fields = c.GetType().GetFields();
                                        foreach ( System.Reflection.FieldInfo field in fields ) {
                                            field.SetValue( copy, field.GetValue( c ) );
                                        }
                                    }
                                    else if ( Settings.autoSynchComponents ) {
                                        var copy = gameReflectors[cam].refCamera.gameObject.AddComponent( c.GetType() );
                                        System.Reflection.FieldInfo[] fields = c.GetType().GetFields();
                                        foreach ( System.Reflection.FieldInfo field in fields ) {
                                            field.SetValue( copy, field.GetValue( c ) );
                                        }
                                    }
                                }
                            }
                        }


                    }

                }
                else {
                    gameReflectors.Remove( cam );
                }
            }


        }


        public void OnDisable() {


            if ( SRPMode ) {
#if UNITY_2019_1_OR_NEWER && (PLANAR3_PRO || PLANAR3_LWRP || PLANAR3_URP || PLANAR3_HDRP)
                UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering -= SRPRenderReflection;
                UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering -= CleanupTextures;
#endif
            }
            else {
                Camera.onPreCull -= RenderReflection;
            }
#if UNITY_2019_1_OR_NEWER && (PLANAR3_PRO || PLANAR3_LWRP || PLANAR3_URP || PLANAR3_HDRP)
            if ( SRPMode ) {
                var cams = Resources.FindObjectsOfTypeAll<Camera>();

                foreach ( Camera cam in cams ) {
                    if ( cam.name.Contains( "PLANAR3_" ) )
                        DestroyImmediate( cam.gameObject );
                }
            }
#endif
        }


        private Vector4 CameraSpacePlane( Camera forCamera, Vector3 planeCenter, Vector3 planeNormal ) {
            Vector3 offsetPos = planeCenter;
            Matrix4x4 mtx = forCamera.worldToCameraMatrix;
            Vector3 cPos = mtx.MultiplyPoint( offsetPos );
            Vector3 cNormal = mtx.MultiplyVector( planeNormal ).normalized * 1;
            return new Vector4( cNormal.x, cNormal.y, cNormal.z, -Vector3.Dot( cPos, cNormal ) );
        }


#if UNITY_EDITOR

        public void OnDrawGizmos() {

#if UNITY_EDITOR && UNITY_2018_1_OR_NEWER
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
#endif

            if ( AssetDatabase.FindAssets( "Planar3Logo_Gizmos" ).Length < 1 ) {
                if ( !AssetDatabase.IsValidFolder( "Assets/Gizmos" ) )
                    AssetDatabase.CreateFolder( "Assets", "Gizmos" );
                var t = new Texture2D( sceneIcon.width, sceneIcon.height );
                t.SetPixels( sceneIcon.GetPixels() );
                File.WriteAllBytes( Application.dataPath + "/Gizmos/Planar3Logo_Gizmos.png", t.EncodeToPNG() );
                AssetDatabase.Refresh();
                var importer = (TextureImporter)AssetImporter.GetAtPath( "Assets/Gizmos/Planar3Logo_Gizmos.png" );
                importer.isReadable = true;
                importer.textureType = TextureImporterType.GUI;
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
            }


            Gizmos.matrix = Matrix4x4.TRS( transform.position, transform.rotation, Vector3.one );
            Gizmos.DrawIcon( transform.position + transform.rotation * Vector3.up, "Planar3Logo_Gizmos.png" );
            Gizmos.color = Color.clear;
            Gizmos.DrawCube( Vector3.zero, new Vector3( 1, 0.01f, 1 ) * 10 );
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube( Vector3.zero, new Vector3( 1, 0, 1 ) * 10 );
            Gizmos.matrix = Matrix4x4.TRS( Vector3.zero, Quaternion.identity, Vector3.one );

        }


        public void OnDrawGizmosSelected() {

        }

#endif

    }


}