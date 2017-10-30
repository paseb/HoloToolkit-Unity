﻿//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MRTK.UX
{
    public class LineMeshes : LineRenderer
    {
        readonly string InvisibleShaderName = "HUX/InvisibleShader";

        public Mesh LineMesh;

        public Material LineMaterial;
        
        public string ColorProp = "_Color";

        protected override void OnEnable()
        {
            base.OnEnable();

            if (linePropertyBlock == null)
            {
                LineMaterial.enableInstancing = true;
                linePropertyBlock = new MaterialPropertyBlock();
                colorID = Shader.PropertyToID(ColorProp);
            }

            if (onWillRenderHelper == null)
            {   // OnWillRenderObject won't be called unless there's a renderer attached
                // and if the renderer's bounds are visbile.
                // So we create a simple 1-triangle mesh to ensure it's always called.
                // Hackey, but it works.
                onWillRenderHelper = gameObject.AddComponent<MeshRenderer>();
                onWillRenderHelper.receiveShadows = false;
                onWillRenderHelper.shadowCastingMode = ShadowCastingMode.Off;
                onWillRenderHelper.lightProbeUsage = LightProbeUsage.Off;
                onWillRenderHelper.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

                onWillRenderMesh = new Mesh();
                onWillRenderMesh.vertices = meshVertices;
                onWillRenderMesh.triangles = new int[] { 0, 1, 2 };

                MeshFilter helperMeshFilter = gameObject.AddComponent<MeshFilter>();
                helperMeshFilter.sharedMesh = onWillRenderMesh;

                // Create an 'invisible' material so the mesh doesn't show up pink
                onWillRenderMat = new Material(Shader.Find(InvisibleShaderName));
                onWillRenderHelper.sharedMaterial = onWillRenderMat;
            }
        }

        private void Update()
        {
            executeCommandBuffer = false;

            if (source.enabled)
            {
                if (meshTransforms == null || meshTransforms.Length != NumLineSteps)
                {
                    meshTransforms = new Matrix4x4[NumLineSteps];
                }

                if (colorValues == null || colorValues.Length != NumLineSteps)
                {
                    colorValues = new Vector4[NumLineSteps];
                    linePropertyBlock.Clear();
                }

                for (int i = 0; i < NumLineSteps; i++)
                {
                    float normalizedDistance = (1f / (NumLineSteps - 1)) * i;
                    colorValues[i] = GetColor(normalizedDistance);
                    meshTransforms[i] = Matrix4x4.TRS(source.GetPoint(normalizedDistance), source.GetRotation(normalizedDistance), Vector3.one * GetWidth(normalizedDistance));
                }

                linePropertyBlock.SetVectorArray(colorID, colorValues);

                executeCommandBuffer = true;
            }
        }

        private void OnDisable()
        {
            foreach (KeyValuePair<Camera, CommandBuffer> cam in cameras)
            {
                if (cam.Key != null)
                {
                    cam.Key.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, cam.Value);
                }
            }
            cameras.Clear();
        }

        private void OnWillRenderObject()
        {
            Camera cam = Camera.current;
            CommandBuffer buffer = null;
            if (!cameras.TryGetValue(cam, out buffer))
            {
                buffer = new CommandBuffer();
                buffer.name = "Line Mesh Renderer " + cam.name;
                cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, buffer);
                cameras.Add(cam, buffer);
            }

            buffer.Clear();
            if (executeCommandBuffer)
            {
                buffer.DrawMeshInstanced(LineMesh, 0, LineMaterial, 0, meshTransforms, meshTransforms.Length, linePropertyBlock);
            }
        }

        private void LateUpdate()
        {
            // Update our helper mesh so OnWillRenderObject will be called
            meshVertices[0] = transform.InverseTransformPoint (source.GetPoint(0.0f));// - transform.position;
            meshVertices[1] = transform.InverseTransformPoint (source.GetPoint(0.5f));// - transform.position;
            meshVertices[2] = transform.InverseTransformPoint (source.GetPoint(1.0f));// - transform.position;
            onWillRenderMesh.vertices = meshVertices;
            onWillRenderMesh.RecalculateBounds();
        }

        // Command buffer properties
        private MaterialPropertyBlock linePropertyBlock;
        private int colorID;
        private Matrix4x4[] meshTransforms;
        private Vector4[] colorValues;
        private bool executeCommandBuffer = false;
        private Dictionary<Camera, CommandBuffer> cameras = new Dictionary<Camera, CommandBuffer>();
        // OnWillRenderObject helpers
        private MeshRenderer onWillRenderHelper;
        private Mesh onWillRenderMesh;
        private Material onWillRenderMat;
        private Vector3[] meshVertices = new Vector3[3];
    }
}