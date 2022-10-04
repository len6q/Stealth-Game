using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private MeshFilter _viewMeshFilter;
    [SerializeField] private MeshRenderer _viewMeshRender;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _detectionColor;

    private float _viewRadius;
    private float _viewAngle;

    private readonly float _meshResolution = 1f;
    private readonly int _edgeResolveIterations = 5;
    private readonly float _edgeDestinationThreshold = 0.5f;

    private Mesh _viewMesh;

    private List<Collider> _visibleTargets = new List<Collider>();

    private const int OBSTACLE_LAYER_MASK = 1 << 3;
    private const int TARGET_LAYER_MASK = 1 << 6;
    
    private void LateUpdate()
    {
        ShowFieldOfView();
    }

    private void OnDrawGizmos()
    {        
        Gizmos.color = Color.white;
        Vector3 position = transform.position;
        Gizmos.DrawWireSphere(position, _viewRadius);

        Vector3 viewAngleA = DirectionFromAngle(-_viewAngle / 2);
        Vector3 viewAngleB = DirectionFromAngle(_viewAngle / 2);        
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * _viewRadius);

        Gizmos.color = Color.red;
        foreach(var target in _visibleTargets)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }

    public void Init(float viewRadius, float viewAngle)
    {
        _viewRadius = viewRadius;
        _viewAngle = viewAngle;

        _viewMesh = new Mesh();
        _viewMeshFilter.mesh = _viewMesh;
    }

    public void SetBaseColor()
    {
        _viewMeshRender.material.color = _baseColor;
    }

    public void SetDetectionColor()
    {
        _viewMeshRender.material.color = _detectionColor;
    }

    private void ShowFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
        float stepAngleSize = _viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        var oldViewCast = new ViewCastConfig();
        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - _viewAngle / 2 + stepAngleSize * i;
            ViewCastConfig newViewCast = GetViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.Destination - newViewCast.Destination) > _edgeDestinationThreshold;
                if (oldViewCast.Hit != newViewCast.Hit || (oldViewCast.Hit && newViewCast.Hit && edgeDstThresholdExceeded))
                {
                    EdgeConfig edge = GetEdge(oldViewCast, newViewCast);
                    if (edge.PointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointA);
                    }
                    if (edge.PointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }
            viewPoints.Add(newViewCast.Point);
            oldViewCast = newViewCast;
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();

        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    private Vector3 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private bool FillBuffer()
    {
        _visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, TARGET_LAYER_MASK);

        foreach (var target in targetsInViewRadius)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < _viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, OBSTACLE_LAYER_MASK) == false)
                {
                    _visibleTargets.Add(target);
                }
            }
        }

        return _visibleTargets.Count > 0;
    }

    public Target AcquireTarget()
    {        
        if (FillBuffer())
        {
            return _visibleTargets[0].GetComponent<Target>();            
        }        
        return null;
    }

    public bool IsTargetTracked(ref Target target)
    {
        if(target == null)
        {            
            return false;
        }

        return true;
    }

    private EdgeConfig GetEdge(ViewCastConfig minViewCast, ViewCastConfig maxViewCast)
    {
        float minAngle = minViewCast.Angle;
        float maxAngle = maxViewCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastConfig newViewCast = GetViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.Destination - newViewCast.Destination) > _edgeDestinationThreshold;
            if (newViewCast.Hit == minViewCast.Hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.Point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.Point;
            }
        }

        return new EdgeConfig(minPoint, maxPoint);
    }

    private ViewCastConfig GetViewCast(float angle)
    {
        Vector3 direction = DirectionFromAngle(angle);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, direction, out hit, _viewRadius, OBSTACLE_LAYER_MASK))
        {
            return new ViewCastConfig(true, hit.point, hit.distance, angle);
        }
        else
        {
            return new ViewCastConfig(false, transform.position + direction * _viewRadius, _viewRadius, angle);
        }
    }

    public struct ViewCastConfig
    {
        public readonly bool Hit;
        public readonly Vector3 Point;
        public readonly float Destination;
        public readonly float Angle;

        public ViewCastConfig(bool hit, Vector3 point, float destination, float angle)
        {
            Hit = hit;
            Point = point;
            Destination = destination;
            Angle = angle;
        }
    }

    public struct EdgeConfig
    {
        public readonly Vector3 PointA;
        public readonly Vector3 PointB;

        public EdgeConfig(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }
}
