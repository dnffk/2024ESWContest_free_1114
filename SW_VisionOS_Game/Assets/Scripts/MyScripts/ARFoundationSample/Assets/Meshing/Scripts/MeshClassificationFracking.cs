using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.VisionOS;
using ARMeshClassification = UnityEngine.XR.VisionOS.ARMeshClassification;
using UnityEngine.XR.ARSubsystems;
public class MeshClassificationFracking : MonoBehaviour
{
    const int k_NumClassifications = 8;
    public ARMeshManager m_MeshManager;
    public MeshFilter m_NoneMeshPrefab;
    public MeshFilter m_WallMeshPrefab;
    public MeshFilter m_FloorMeshPrefab;
    public MeshFilter m_CeilingMeshPrefab;
    public MeshFilter m_TableMeshPrefab;
    public MeshFilter m_SeatMeshPrefab;
    public MeshFilter m_WindowMeshPrefab;
    public MeshFilter m_DoorMeshPrefab;
    readonly Dictionary<TrackableId, MeshFilter[]> m_MeshFrackingMap = new Dictionary<TrackableId, MeshFilter[]>();
    Action<MeshFilter> m_BreakupMeshAction;
    Action<MeshFilter> m_UpdateMeshAction;
    Action<MeshFilter> m_RemoveMeshAction;
    readonly List<int> m_BaseTriangles = new List<int>();
    readonly List<int> m_ClassifiedTriangles = new List<int>();
    void Awake()
    {
        m_BreakupMeshAction = new Action<MeshFilter>(BreakupMesh);
        m_UpdateMeshAction = new Action<MeshFilter>(UpdateMesh);
        m_RemoveMeshAction = new Action<MeshFilter>(RemoveMesh);
    }
    void OnEnable()
    {
        m_MeshManager.meshesChanged += OnMeshesChanged;
    }
    void OnDisable()
    {
        m_MeshManager.meshesChanged -= OnMeshesChanged;
    }
    void OnMeshesChanged(ARMeshesChangedEventArgs args)
    {
        if (args.added != null)
        {
            args.added.ForEach(m_BreakupMeshAction);
        }
        if (args.updated != null)
        {
            args.updated.ForEach(m_UpdateMeshAction);
        }
        if (args.removed != null)
        {
            args.removed.ForEach(m_RemoveMeshAction);
        }
    }
    TrackableId ExtractTrackableId(string meshFilterName)
    {
        string[] nameSplit = meshFilterName.Split(' ');
        return new TrackableId(nameSplit[1]);
    }
    void ExtractClassifiedMesh(Mesh baseMesh, NativeArray<ARMeshClassification> faceClassifications, ARMeshClassification selectedMeshClassification, Mesh classifiedMesh)
    {
        int classifiedFaceCount = 0;
        for (int i = 0; i < faceClassifications.Length; ++i)
        {
            if (faceClassifications[i] == selectedMeshClassification)
            {
                ++classifiedFaceCount;
            }
        }
        classifiedMesh.Clear();
        if (classifiedFaceCount > 0)
        {
            baseMesh.GetTriangles(m_BaseTriangles, 0);
            Debug.Assert(m_BaseTriangles.Count == (faceClassifications.Length * 3),
                        "unexpected mismatch between triangle count and face classification count");
            m_ClassifiedTriangles.Clear();
            m_ClassifiedTriangles.Capacity = classifiedFaceCount * 3;
            for (int i = 0; i < faceClassifications.Length; ++i)
            {
                if (faceClassifications[i] == selectedMeshClassification)
                {
                    int baseTriangleIndex = i * 3;
                    m_ClassifiedTriangles.Add(m_BaseTriangles[baseTriangleIndex + 0]);
                    m_ClassifiedTriangles.Add(m_BaseTriangles[baseTriangleIndex + 1]);
                    m_ClassifiedTriangles.Add(m_BaseTriangles[baseTriangleIndex + 2]);
                }
            }
            classifiedMesh.vertices = baseMesh.vertices;
            classifiedMesh.normals = baseMesh.normals;
            classifiedMesh.SetTriangles(m_ClassifiedTriangles, 0);
        }
    }
    void BreakupMesh(MeshFilter meshFilter)
    {
        XRMeshSubsystem meshSubsystem = m_MeshManager.subsystem as XRMeshSubsystem;
        if (meshSubsystem == null)
        {
            return;
        }
        var meshId = ExtractTrackableId(meshFilter.name);
        var faceClassifications = meshSubsystem.GetFaceClassifications(meshId, Allocator.Persistent);
        if (!faceClassifications.IsCreated)
        {
            return;
        }
        using (faceClassifications)
        {
            if (faceClassifications.Length <= 0)
            {
                return;
            }
            var parent = meshFilter.transform;
            MeshFilter[] meshFilters = new MeshFilter[k_NumClassifications];
            meshFilters[(int)ARMeshClassification.None] = (m_NoneMeshPrefab == null) ? null : Instantiate(m_NoneMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Wall] = (m_WallMeshPrefab == null) ? null : Instantiate(m_WallMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Floor] = (m_FloorMeshPrefab == null) ? null : Instantiate(m_FloorMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Ceiling] = (m_CeilingMeshPrefab == null) ? null : Instantiate(m_CeilingMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Table] = (m_TableMeshPrefab == null) ? null : Instantiate(m_TableMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Seat] = (m_SeatMeshPrefab == null) ? null : Instantiate(m_SeatMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Window] = (m_WindowMeshPrefab == null) ? null : Instantiate(m_WindowMeshPrefab, parent);
            meshFilters[(int)ARMeshClassification.Door] = (m_DoorMeshPrefab == null) ? null : Instantiate(m_DoorMeshPrefab, parent);
            m_MeshFrackingMap[meshId] = meshFilters;
            var baseMesh = meshFilter.sharedMesh;
            for (int i = 0; i < k_NumClassifications; ++i)
            {
                var classifiedMeshFilter = meshFilters[i];
                if (classifiedMeshFilter != null)
                {
                    var classifiedMesh = classifiedMeshFilter.mesh;
                    ExtractClassifiedMesh(baseMesh, faceClassifications, (ARMeshClassification)i, classifiedMesh);
                    meshFilters[i].mesh = classifiedMesh;
                }
            }
        }
    }
    void UpdateMesh(MeshFilter meshFilter)
    {
        XRMeshSubsystem meshSubsystem = m_MeshManager.subsystem as XRMeshSubsystem;
        if (meshSubsystem == null)
        {
            return;
        }
        var meshId = ExtractTrackableId(meshFilter.name);
        var faceClassifications = meshSubsystem.GetFaceClassifications(meshId, Allocator.Persistent);
        if (!faceClassifications.IsCreated)
        {
            return;
        }
        using (faceClassifications)
        {
            if (faceClassifications.Length <= 0)
            {
                return;
            }
            var meshFilters = m_MeshFrackingMap[meshId];
            var baseMesh = meshFilter.sharedMesh;
            for (int i = 0; i < k_NumClassifications; ++i)
            {
                var classifiedMeshFilter = meshFilters[i];
                if (classifiedMeshFilter != null)
                {
                    var classifiedMesh = classifiedMeshFilter.mesh;
                    ExtractClassifiedMesh(baseMesh, faceClassifications, (ARMeshClassification)i, classifiedMesh);
                    meshFilters[i].mesh = classifiedMesh;
                }
            }
        }
    }
    void RemoveMesh(MeshFilter meshFilter)
    {
        var meshId = ExtractTrackableId(meshFilter.name);
        var meshFilters = m_MeshFrackingMap[meshId];
        for (int i = 0; i < k_NumClassifications; ++i)
        {
            var classifiedMeshFilter = meshFilters[i];
            if (classifiedMeshFilter != null)
            {
                UnityEngine.Object.Destroy(classifiedMeshFilter);
            }
        }
        m_MeshFrackingMap.Remove(meshId);
    }
}