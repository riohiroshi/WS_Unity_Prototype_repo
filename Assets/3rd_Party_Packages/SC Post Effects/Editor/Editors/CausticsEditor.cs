﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using SCPE;
#if HDRP_7_2_0_OR_NEWER || URP_7_2_0_OR_NEWER
using UnityEditor.Rendering;
#endif
#if PPS && !HDRP_7_2_0_OR_NEWER && !URP_7_2_0_OR_NEWER
using UnityEngine.Rendering.PostProcessing;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Caustics))]
    public class CausticsEditor : PostProcessEffectEditor<Caustics>
    {
        SerializedParameterOverride causticsTexture;
        SerializedParameterOverride intensity;
        SerializedParameterOverride luminanceThreshold;
        
        SerializedParameterOverride minHeight;
        SerializedParameterOverride minHeightFalloff;
        SerializedParameterOverride maxHeight;
        SerializedParameterOverride maxHeightFalloff;
        
        SerializedParameterOverride size;
        SerializedParameterOverride speed;
        
        SerializedParameterOverride distanceFade;
        SerializedParameterOverride startFadeDistance;
        SerializedParameterOverride endFadeDistance;

        public override void OnEnable()
        {
            causticsTexture = FindParameterOverride(x => x.causticsTexture);
            intensity = FindParameterOverride(x => x.intensity);
            luminanceThreshold = FindParameterOverride(x => x.luminanceThreshold);

            minHeight = FindParameterOverride(x => x.minHeight);
            minHeightFalloff = FindParameterOverride(x => x.minHeightFalloff);
            maxHeight = FindParameterOverride(x => x.maxHeight);
            maxHeightFalloff = FindParameterOverride(x => x.maxHeightFalloff);
            
            size = FindParameterOverride(x => x.size);
            speed = FindParameterOverride(x => x.speed);
            
            distanceFade = FindParameterOverride(x => x.distanceFade);
            startFadeDistance = FindParameterOverride(x => x.startFadeDistance);
            endFadeDistance = FindParameterOverride(x => x.endFadeDistance);
        }
        
        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("caustics");

            SCPE_GUI.DisplaySetupWarning<CausticsRenderer>();

            PropertyField(causticsTexture);
            PropertyField(intensity);
            PropertyField(luminanceThreshold);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Height filter", EditorStyles.boldLabel);

            PropertyField(minHeight);
            PropertyField(minHeightFalloff);
            PropertyField(maxHeight);
            PropertyField(maxHeightFalloff);
            
            EditorGUILayout.Space();

            PropertyField(size);
            PropertyField(speed);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Distance Fading", EditorStyles.boldLabel);
            PropertyField(distanceFade);
            if (distanceFade.value.boolValue)
            {
                PropertyField(startFadeDistance);
                PropertyField(endFadeDistance);
            }
        }
    }
}
#endif