using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Building))]
public class Edit_Building : Editor
{
    private Building self;
    
    
    
    private void OnSceneGUI()
    {
        self = (Building)target;

        for (int i = 0; i < self.regions.Length; ++i)
        {
            Handles.color = Color.Lerp(Color.red, Color.green, ((float)(i+1))/(float)self.regions.Length);
            Handles.DrawWireDisc(self.transform.position, Vector3.up, self.regions[i].minRadius);
            Handles.DrawWireDisc(self.transform.position, Vector3.up, self.regions[i].maxRadius);
        }
    }
}
