using System;
using UnityEngine;
using UnityEditor;

namespace UnityEditor
{
    public class ExplosionGUI : ShaderGUI 
    {
        protected class Styles
        {
            //Catagories
            public static readonly GUIContent UVFlow = 
                new GUIContent("UV滚动", "通过控制UV的滚动实现贴图的滚动");
            
            public static readonly GUIContent UVWarp = 
                new GUIContent("UV扭曲", "通过控制采样贴图的UV进行对贴图的扭动");
            
            public static readonly GUIContent Dissolve = 
                new GUIContent("溶解", "对物体进行溶解的特效，可以通过遮罩贴图来控制");

            public static readonly GUIContent baseMap = 
                new GUIContent("基础贴图", "最基本的贴图");

            public static readonly GUIContent mainHDRColor = 
                new GUIContent("主颜色", "主颜色");

            public static readonly GUIContent noiseMap = 
                new GUIContent("噪点贴图", "控制噪点的贴图");

            public static readonly GUIContent FlowSpeed = 
                new GUIContent("滚动速度", "贴图UV的滚动速度");

            public static readonly GUIContent WarpIntensity = 
                new GUIContent("扰动强度", "贴图UV的扰动强度");

            public static readonly GUIContent WarpSpeed = 
                new GUIContent("扰动速度", "贴图UV的扰动速度");

            public static readonly GUIContent HDRColor = 
                new GUIContent("边缘颜色", "溶解时候边缘的颜色");

            public static readonly GUIContent DissolveAmount = 
                new GUIContent("溶解量", "溶解的程度");

            public static readonly GUIContent EdgeWidth = 
                new GUIContent("边缘粗细", "溶解时边缘的粗细");

            public static readonly GUIContent EdgeSmoothnessOut = 
                new GUIContent("外边缘平滑度", "溶解时边缘的软硬程度");

            public static readonly GUIContent EdgeSmoothnessIn = 
                new GUIContent("内边缘平滑度", "溶解时边缘的软硬程度");

            public static readonly GUIContent Mask = 
                new GUIContent("溶解方向遮罩", "设定溶解方向的遮罩贴图");

            public static readonly GUIContent doubleSide = 
                new GUIContent("双向溶解", "除了边缘以外都是透明的");
        }

        // MaterialEditor materialEditor;
        // MaterialProperty[] properties;

        // Material targetMat;
        // string[] keyWords;

        static bool UVFlowFoldout = false;
        static bool UVWarpFoldout = false;
        static bool DissolveFoldout = false;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            MaterialProperty culling = FindProperty("_Cull", properties);
            MaterialProperty[] propertiesShow = new MaterialProperty[1];
            propertiesShow[0] = culling;
            base.OnGUI(materialEditor, propertiesShow);
            //base.OnGUI(materialEditor, properties);

            Material targetMat = materialEditor.target as Material;

            //MaterialProperty useUVFlow = FindProperty("_enableUVFlow", properties);
            //MaterialProperty useUVWarp = FindProperty("_enableUVWarp", properties);
            //MaterialProperty useDissolve = FindProperty("_enableDissolve", properties);
            //MaterialProperty useMask = FindProperty("_useMask", properties);

            MaterialProperty baseMap = FindProperty("_BaseMap", properties);
            MaterialProperty mainHDRColor = FindProperty("_MainHDR", properties);
            MaterialProperty noiseMap = FindProperty("_Noise", properties);
            MaterialProperty FlowSpeed = FindProperty("_UVFlowSpeed", properties, true);
            MaterialProperty WarpIntensity = FindProperty("_WarpIntensity", properties);
            MaterialProperty WarpSpeed = FindProperty("_UVWarpSpeed", properties);
            MaterialProperty HDRColor = FindProperty("_HDRColor", properties);
            MaterialProperty DissolveAmount = FindProperty("_DissolveAmount", properties);
            MaterialProperty EdgeWidth = FindProperty("_EdgeWidth", properties);
            MaterialProperty EdgeSmoothnessOut = FindProperty("_EdgeSmoothnessOut", properties);
            MaterialProperty EdgeSmoothnessIn = FindProperty("_EdgeSmoothnessIn", properties);
            MaterialProperty maskMap = FindProperty("_Mask", properties);            

            
            materialEditor.ShaderProperty(baseMap, Styles.baseMap);
            materialEditor.ShaderProperty(mainHDRColor, Styles.mainHDRColor);
            materialEditor.ShaderProperty(noiseMap, Styles.noiseMap);
            
            UVFlowFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(UVFlowFoldout, Styles.UVFlow);
            if (UVFlowFoldout)
            {
                bool enableUVFlow = Array.IndexOf(targetMat.shaderKeywords, "ENABLE_UVFLOW") != -1;
                EditorGUI.BeginChangeCheck();
                enableUVFlow = EditorGUILayout.Toggle(Styles.UVFlow, enableUVFlow);
                if (EditorGUI.EndChangeCheck()){
                    if(enableUVFlow) targetMat.EnableKeyword("ENABLE_UVFLOW");
                    else targetMat.DisableKeyword("ENABLE_UVFLOW");
                }
                    
                EditorGUI.showMixedValue = false;

                if (enableUVFlow)
                    materialEditor.ShaderProperty(FlowSpeed, Styles.FlowSpeed, 1);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            UVWarpFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(UVWarpFoldout, Styles.UVWarp);
            if (UVWarpFoldout)
            {
                bool enableUVWarp = Array.IndexOf(targetMat.shaderKeywords, "ENABLE_UVWARP") != -1;
                EditorGUI.BeginChangeCheck();
                enableUVWarp = EditorGUILayout.Toggle(Styles.UVWarp, enableUVWarp);
                if (EditorGUI.EndChangeCheck())
                {
                    if(enableUVWarp) targetMat.EnableKeyword("ENABLE_UVWARP");
                    else targetMat.DisableKeyword("ENABLE_UVWARP");
                }
                EditorGUI.showMixedValue = false;

                if (enableUVWarp){
                    materialEditor.ShaderProperty(WarpIntensity, Styles.WarpIntensity, 1);
                    materialEditor.ShaderProperty(WarpSpeed, Styles.WarpSpeed, 1);
                }

            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            DissolveFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(DissolveFoldout, Styles.Dissolve);
            if(DissolveFoldout)
            {
                bool enableDissolve = Array.IndexOf(targetMat.shaderKeywords, "ENABLE_DISSOLVE") != -1;
                EditorGUI.BeginChangeCheck();
                enableDissolve = EditorGUILayout.Toggle(Styles.Dissolve, enableDissolve);
                if(EditorGUI.EndChangeCheck())
                {
                    if(enableDissolve) targetMat.EnableKeyword("ENABLE_DISSOLVE");
                    else targetMat.DisableKeyword("ENABLE_DISSOLVE");
                }
                EditorGUI.showMixedValue = false;

                if(enableDissolve)
                {
                    bool doubleSide = Array.IndexOf(targetMat.shaderKeywords, "DOUBLE_SIDE") != -1;
                    EditorGUI.BeginChangeCheck();
                    doubleSide = EditorGUILayout.Toggle(Styles.doubleSide, doubleSide);
                    if(EditorGUI.EndChangeCheck())
                    {
                        if(doubleSide) targetMat.EnableKeyword("DOUBLE_SIDE");
                        else targetMat.DisableKeyword("DOUBLE_SIDE");
                    }

                    materialEditor.ShaderProperty(HDRColor, Styles.HDRColor);
                    materialEditor.ShaderProperty(DissolveAmount, Styles.DissolveAmount);
                    materialEditor.ShaderProperty(EdgeWidth, Styles.EdgeWidth);
                    materialEditor.ShaderProperty(EdgeSmoothnessOut, Styles.EdgeSmoothnessOut);
                    materialEditor.ShaderProperty(EdgeSmoothnessIn, Styles.EdgeSmoothnessIn);
                    
                    bool enableMask = Array.IndexOf(targetMat.shaderKeywords, "USE_MASK") != -1;
                    EditorGUI.BeginChangeCheck();
                    enableMask = EditorGUILayout.Toggle(Styles.Mask, enableMask);
                    if(EditorGUI.EndChangeCheck())
                    {
                        if(enableMask) targetMat.EnableKeyword("USE_MASK");
                        else targetMat.DisableKeyword("USE_MASK");
                    }
                    if(enableMask) materialEditor.ShaderProperty(maskMap, Styles.Mask);
                }
            }
            
            // targetMat.renderQueue = (int)RenderQueue.Transparent;
            // targetMat.renderQueue += targetMat.HasProperty("_QueueOffset") ? (int) targetMat.GetFloat("_QueueOffset") : 0;

        }
    }
}