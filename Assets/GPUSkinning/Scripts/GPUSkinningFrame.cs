﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class GPUSkinningFrame
{
    public Matrix4x4[] matrices = null;

    public Vector3 rootPosition = Vector3.zero;

    public Quaternion rootRotation = Quaternion.identity;

    [System.NonSerialized]
    private bool rootMotionInvInit = false;
    [System.NonSerialized]
    private Matrix4x4 rootMotionInv;
    public Matrix4x4 RootMotionInv(int rootBoneIndex)
    {
        if (!rootMotionInvInit)
        {
            rootMotionInv = matrices[rootBoneIndex].inverse;
            rootMotionInvInit = true;
        }
        return rootMotionInv;
    }
}
