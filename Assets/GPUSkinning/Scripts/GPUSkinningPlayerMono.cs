﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GPUSkinningPlayerMono : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    public GPUSkinningAnimation anim = null;

    [HideInInspector]
    [SerializeField]
    public Mesh mesh = null;

    [HideInInspector]
    [SerializeField]
    public Material mtrl = null;

    [HideInInspector]
    [SerializeField]
    public TextAsset textureRawData = null;

    [HideInInspector]
    [SerializeField]
    private int defaultPlayingClipIndex = 0;

    [HideInInspector]
    [SerializeField]
    public bool rootMotion = false;

    private static GPUSkinningPlayerMonoManager playerManager = new GPUSkinningPlayerMonoManager();

    private GPUSkinningPlayer player = null;
    public GPUSkinningPlayer Player
    {
        get
        {
            return player;
        }
    }

    public void Init()
    {
        if(player != null)
        {
            return;
        }

        if (anim != null && mesh != null && mtrl != null && textureRawData != null)
        {
            GPUSkinningPlayerResources res = null;

            if (Application.isPlaying)
            {
                playerManager.Register(anim, mesh, mtrl, textureRawData, this, out res);
            }
            else
            {
                res = new GPUSkinningPlayerResources();
                res.anim = anim;
                res.mesh = mesh;
                res.mtrl = new GPUSkinningPlayerMaterial(new Material(mtrl));
                res.texture = GPUSkinningUtil.CreateTexture2D(textureRawData, anim);
                res.mtrl.Material.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                res.texture.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            }

            player = new GPUSkinningPlayer(gameObject, res);
            player.RootMotionEnabled = rootMotion;

            if (anim != null && anim.clips != null && anim.clips.Length > 0)
            {
                player.Play(anim.clips[Mathf.Clamp(defaultPlayingClipIndex, 0, anim.clips.Length)].name);
            }
        }
    }

#if UNITY_EDITOR
    public void Update_Editor(float deltaTime)
    {
        if(player != null && !Application.isPlaying)
        {
            player.Update_Editor(deltaTime);
        }
    }

    private void OnValidate()
    {
        Init();
        Update_Editor(0);
    }
#endif

    private void Start()
    {
        Init();
#if UNITY_EDITOR
        Update_Editor(0); 
#endif
    }

    private void Update()
    {
        if (player != null)
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                player.Update(Time.deltaTime);
            }
            else
            {
                player.Update_Editor(0);
            }
#else
            player.Update(Time.deltaTime);
#endif
        }
    }

    private void OnDestroy()
    {
        player = null;

        if (Application.isPlaying)
        {
            playerManager.Unregister(this);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Resources.UnloadUnusedAssets();
            UnityEditor.EditorUtility.UnloadUnusedAssetsImmediate();
        }
#endif
    }
}
