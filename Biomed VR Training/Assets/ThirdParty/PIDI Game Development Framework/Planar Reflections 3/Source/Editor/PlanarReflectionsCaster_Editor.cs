using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLANAR3_PRO && UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
using UnityEditor;

namespace PlanarReflections3 {

    [CustomEditor(typeof(PlanarReflectionsCaster))]
    public class PlanarReflectionsCaster_Editor : Editor {

        public GUISkin pidiSkin2;

        public PlanarReflectionsCaster refCaster;

        public Texture2D reflectionsLogo;


        public void OnEnable() {
            refCaster = (PlanarReflectionsCaster)target;
        }

        public override void OnInspectorGUI() {

            Undo.RecordObject(refCaster, refCaster.name + refCaster.GetInstanceID());

            GUILayout.BeginVertical(pidiSkin2.box);
            
            AssetLogoAndVersion();

            // GENERAL SETTINGS

            if (BeginCenteredGroup("GENERAL SETTINGS", ref refCaster.folds[0])) {

                GUILayout.Space(16);

                refCaster.castFromRenderer = ObjectField<PlanarReflectionsRenderer>(new GUIContent("CAST REFLECTION FROM", "The Planar Reflections Renderer from which the reflection will be cast"), refCaster.castFromRenderer);

                               
                GUILayout.Space(16);

                for (int i = 0; i < refCaster.castReflection.Length; i++) {
                    GUILayout.BeginHorizontal();GUILayout.Space(12);
                    GUI.color = Color.gray;
                    GUILayout.BeginVertical(pidiSkin2.customStyles[5]);
                    GUILayout.BeginHorizontal(); GUILayout.Space(12);
                    GUILayout.BeginVertical();
                    GUI.color = Color.white;
                    GUILayout.Space(8);
                    CenteredLabel(refCaster.GetComponent<Renderer>().sharedMaterials[i].name);
                    GUILayout.Space(8);

                    refCaster.castReflection[i] = EnableDisableToggle(new GUIContent("REFLECTION TEXTURE", "Enables the reflection texture for this material"), refCaster.castReflection[i]);
                    
                    GUILayout.Space(16);
                    GUILayout.EndVertical();
                    GUILayout.Space(12);GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.Space(12);GUILayout.EndHorizontal();
                }


                GUILayout.Space(16);

            }
            EndCenteredGroup();


            //HELP AND SUPPORT
            if (BeginCenteredGroup("HELP & SUPPORT", ref refCaster.folds[6])) {

                GUILayout.Space(16);
                CenteredLabel("SUPPORT AND ASSISTANCE");
                GUILayout.Space(10);

                HelpBox("Please make sure to include the following information with your request :\n - Invoice number preferably in the PDF format it was provided to you at the time of purchase\n - Screenshots of the PlanarReflectionsRenderer / PlanarReflectionsCaster component and its settings\n - Steps to reproduce the issue.\n - Unity version you are using \n - LWRP/Universal RP version you are using (if any)\n\nOur support service usually takes 1-3 business days to reply, so please be patient. We always reply to all emails.\nPlease remember that our assets do not offer official support to any Experimental feature nor any Beta/Alpha Unity versions.", MessageType.Info);

                GUILayout.Space(8);
                GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
                GUILayout.Label("For support, contact us at : support@irreverent-software.com", pidiSkin2.label);
                GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

                GUILayout.Space(8);

                GUILayout.Space(16);
                CenteredLabel("ONLINE TUTORIALS");
                GUILayout.Space(10);
                if (CenteredButton("INITIAL SETUP", 200)) {
                    Help.BrowseURL("https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_planar_reflections_3#quick_setup_guide");
                }
                if (CenteredButton("BASIC REFLECTIONS", 200)) {
                    Help.BrowseURL("https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_planar_reflections_3#adding_reflections_to_a_scene");
                }
                if (CenteredButton("BASIC SETTINGS", 200)) {
                    Help.BrowseURL("https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_planar_reflections_3#planar_reflections_renderer_basic_settings");
                }
                if (CenteredButton("MOVABLE SURFACES", 200)) {
                    Help.BrowseURL("https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_planar_reflections_3#movable_reflective_surfaces");
                }
                if (CenteredButton("POST FX SUPPORT", 200)) {
                    Help.BrowseURL("https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_planar_reflections_3#post_fx_support");
                }
                if (CenteredButton("CAMERA DEPTH EFFECTS", 200)) {
                    Help.BrowseURL("https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_2d_reflections_2#post_process_fx_standard_only");
                }

                if (CenteredButton("LWRP SETUP & LIMITS", 200)) {
                    Help.BrowseURL("https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_planar_reflections_3#lwrp_vs_standard_pipeline");
                }

                if (CenteredButton("CREATING CUSTOM SHADERS", 200)) {
                    Help.BrowseURL("https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_planar_reflections_3#create_custom_shaders");
                }


                GUILayout.Space(24);
                CenteredLabel("ABOUT PIDI : PLANAR REFLECTIONS™ 3");
                GUILayout.Space(12);

                HelpBox("PIDI : PLANAR REFLECTIONS™ has been integrated in dozens of projects by hundreds of users since 2017.\nYour use and support to this tool is what keeps it growing, evolving and adapting to better suit your needs and keep providing you with the best quality reflections for Unity.\n\nIf this tool has been useful for your project, please consider taking a minute to rate and review it, to help us to continue its development for a long time.", MessageType.Info);

                GUILayout.Space(8);
                if (CenteredButton("REVIEW PLANAR REFLECTIONS 3", 200)) {
                    Help.BrowseURL("https://assetstore.unity.com/packages/tools/particles-effects/pidi-planar-reflections-3-standard-edition-153073");
                }
                GUILayout.Space(8);
                if (CenteredButton("ABOUT THIS VERSION", 200)) {
                    Help.BrowseURL("https://assetstore.unity.com/packages/tools/particles-effects/pidi-planar-reflections-3-standard-edition-153073");
                }
                GUILayout.Space(8);

            }
            EndCenteredGroup();

            GUILayout.Space(16);

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            var lStyle = new GUIStyle();
            lStyle.fontStyle = FontStyle.Italic;
            lStyle.normal.textColor = Color.white;
            lStyle.fontSize = 8;

            GUILayout.Label("Copyright© 2017-2020,   Jorge Pinal N.", lStyle);

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

            GUILayout.Space(24);
            GUILayout.EndVertical();

        }



        #region PIDI 2020 EDITOR


        public void HelpBox(string message, MessageType messageType) {
            GUILayout.Space(8);
            GUILayout.BeginHorizontal(); GUILayout.Space(8);
            GUILayout.BeginVertical(pidiSkin2.customStyles[5]);

            GUILayout.Space(4);
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            var mType = "INFO";

            switch (messageType) {
                case MessageType.Error:
                    mType = "ERROR";
                    break;

                case MessageType.Warning:
                    mType = "WARNING";
                    break;
            }

            var tStyle = new GUIStyle();
            tStyle.fontSize = 11;
            tStyle.fontStyle = FontStyle.Bold;
            tStyle.normal.textColor = Color.black;

            GUILayout.Label(mType, tStyle);

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
            GUILayout.Space(4);

            GUILayout.BeginHorizontal(); GUILayout.Space(8); GUILayout.BeginVertical();
            tStyle.fontSize = 9;
            tStyle.fontStyle = FontStyle.Normal;
            tStyle.wordWrap = true;
            GUILayout.TextArea(message, tStyle);

            GUILayout.Space(8);
            GUILayout.EndVertical(); GUILayout.Space(8); GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(8); GUILayout.EndHorizontal();
            GUILayout.Space(8);
        }


        public Color ColorField(GUIContent label, Color currentValue) {

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            currentValue = EditorGUILayout.ColorField(currentValue);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            return currentValue;

        }



        /// <summary>
        /// Draws a standard object field in the PIDI 2020 style
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <param name="inputObject"></param>
        /// <param name="allowSceneObjects"></param>
        /// <returns></returns>
        public T ObjectField<T>(GUIContent label, T inputObject, bool allowSceneObjects = true) where T : UnityEngine.Object {

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            GUI.color = Color.gray;
            inputObject = (T)EditorGUILayout.ObjectField(inputObject, typeof(T), allowSceneObjects);
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            return inputObject;
        }


        /// <summary>
        /// Draws a centered button in the standard PIDI 2020 editor style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool CenteredButton(string label, float width) {
            GUILayout.Space(2);
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            var tempBool = GUILayout.Button(label, pidiSkin2.customStyles[0], GUILayout.MaxWidth(width));
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
            GUILayout.Space(2);
            return tempBool;
        }


        /// <summary>
        /// Draws the asset's logo and its current version
        /// </summary>
        public void AssetLogoAndVersion() {

            GUILayout.BeginVertical(reflectionsLogo, pidiSkin2 ? pidiSkin2.customStyles[1] : null);
            GUILayout.Space(45);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(refCaster.Version, pidiSkin2.customStyles[2]);
            GUILayout.Space(6);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draws a label centered in the Editor window
        /// </summary>
        /// <param name="label"></param>
        public void CenteredLabel(string label) {

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            GUILayout.Label(label, pidiSkin2.label);
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

        }

        /// <summary>
        /// Begins a custom centered group similar to a foldout that can be expanded with a button
        /// </summary>
        /// <param name="label"></param>
        /// <param name="groupFoldState"></param>
        /// <returns></returns>
        public bool BeginCenteredGroup(string label, ref bool groupFoldState) {

            if (GUILayout.Button(label, pidiSkin2.customStyles[0])) {
                groupFoldState = !groupFoldState;
            }
            GUILayout.BeginHorizontal(); GUILayout.Space(12);
            GUILayout.BeginVertical();
            return groupFoldState;
        }


        /// <summary>
        /// Finishes a centered group
        /// </summary>
        public void EndCenteredGroup() {
            GUILayout.EndVertical();
            GUILayout.Space(12);
            GUILayout.EndHorizontal();
        }



        /// <summary>
        /// Custom integer field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public int IntField(GUIContent label, int currentValue) {

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            currentValue = EditorGUILayout.IntField(currentValue, pidiSkin2.customStyles[4]);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            return currentValue;
        }

        /// <summary>
        /// Custom float field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public float FloatField(GUIContent label, float currentValue) {

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            currentValue = EditorGUILayout.FloatField(currentValue, pidiSkin2.customStyles[4]);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            return currentValue;
        }


        /// <summary>
        /// Custom text field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public string TextField(GUIContent label, string currentValue) {

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            currentValue = EditorGUILayout.TextField(currentValue, pidiSkin2.customStyles[4]);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            return currentValue;
        }


        public Vector2 Vector2Field(GUIContent label, Vector2 currentValue) {

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            currentValue.x = EditorGUILayout.FloatField(currentValue.x, pidiSkin2.customStyles[4]);
            GUILayout.Space(8);
            currentValue.y = EditorGUILayout.FloatField(currentValue.y, pidiSkin2.customStyles[4]);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            return currentValue;

        }


        /// <summary>
        /// Custom slider using the PIDI 2020 editor skin and adding a custom suffix to the float display
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <param name="minSlider"></param>
        /// <param name="maxSlider"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public float SliderField(GUIContent label, float currentValue, float minSlider = 0.0f, float maxSlider = 1.0f, string suffix = "") {

            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            GUI.color = Color.gray;
            currentValue = GUILayout.HorizontalSlider(currentValue, minSlider, maxSlider, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb);
            GUI.color = Color.white;
            GUILayout.Space(12);
            currentValue = Mathf.Clamp(EditorGUILayout.FloatField(float.Parse(currentValue.ToString("n2")), pidiSkin2.customStyles[4], GUILayout.MaxWidth(40)), minSlider, maxSlider);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            return currentValue;
        }


        /// <summary>
        /// Custom slider using the PIDI 2020 editor skin and adding a custom suffix to the float display
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <param name="minSlider"></param>
        /// <param name="maxSlider"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public int IntSliderField(GUIContent label, int currentValue, int minSlider = 0, int maxSlider = 1) {

            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            GUI.color = Color.gray;
            currentValue = (int)GUILayout.HorizontalSlider(currentValue, minSlider, maxSlider, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb);
            GUI.color = Color.white;
            GUILayout.Space(12);
            currentValue = (int)Mathf.Clamp(EditorGUILayout.FloatField(float.Parse(currentValue.ToString("n2")), pidiSkin2.customStyles[4], GUILayout.MaxWidth(40)), minSlider, maxSlider);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            return currentValue;
        }


        /// <summary>
        /// Draw a custom popup field in the PIDI 2020 style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggleValue"></param>
        /// <returns></returns>
        public int PopupField(GUIContent label, int selected, string[] options) {


            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            selected = EditorGUILayout.Popup(selected, options, pidiSkin2.customStyles[0]);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            return selected;
        }



        /// <summary>
        /// Draw a custom toggle that instead of using a check box uses an Enable/Disable drop down menu
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggleValue"></param>
        /// <returns></returns>
        public bool EnableDisableToggle(GUIContent label, bool toggleValue, bool trueFalseToggle = false) {

            int option = toggleValue ? 1 : 0;

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            option = EditorGUILayout.Popup(option, new string[] { "DISABLED", "ENABLED" }, pidiSkin2.customStyles[0]);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            return option == 1;
        }


        /// <summary>
        /// Draw an enum field but changing the labels and names of the enum to Upper Case fields
        /// </summary>
        /// <param name="label"></param>
        /// <param name="userEnum"></param>
        /// <returns></returns>
        public int UpperCaseEnumField(GUIContent label, System.Enum userEnum) {

            var names = System.Enum.GetNames(userEnum.GetType());

            for (int i = 0; i < names.Length; i++) {
                names[i] = System.Text.RegularExpressions.Regex.Replace(names[i], "(\\B[A-Z])", " $1").ToUpper();
            }

            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));
            var result = EditorGUILayout.Popup(System.Convert.ToInt32(userEnum), names, pidiSkin2.customStyles[0]);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            return result;
        }


        /// <summary>
        /// Draw a layer mask field in the PIDI 2020 style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="selected"></param>
        public LayerMask LayerMaskField(GUIContent label, LayerMask selected) {

            List<string> layers = null;
            string[] layerNames = null;

            if (layers == null) {
                layers = new List<string>();
                layerNames = new string[4];
            }
            else {
                layers.Clear();
            }

            int emptyLayers = 0;
            for (int i = 0; i < 32; i++) {
                string layerName = LayerMask.LayerToName(i);

                if (layerName != "") {

                    for (; emptyLayers > 0; emptyLayers--) layers.Add("Layer " + (i - emptyLayers));
                    layers.Add(layerName);
                }
                else {
                    emptyLayers++;
                }
            }

            if (layerNames.Length != layers.Count) {
                layerNames = new string[layers.Count];
            }
            for (int i = 0; i < layerNames.Length; i++) layerNames[i] = layers[i];


            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, pidiSkin2.label, GUILayout.Width(EditorGUIUtility.labelWidth));

            selected.value = EditorGUILayout.MaskField(selected.value, layerNames, pidiSkin2.customStyles[0]);

            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            return selected;
        }



        #endregion


    }

}