using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Raycast : MonoBehaviour
{
    public float MaxDistance = 15;
    public int Span = 360;
    public float ConnectThreshold = 50;
    public float PropSpeed = 60;
    public float BasePower = 500;
    public float CapTheta = 18;
    public float LineWidth = 0.08f;
    public float Timeout = 0;

    public Transform playerPos;
    public GameObject RenderPrefab;

    // Start is called before the first frame update
    void Start()
    {
        /*
        var self = this;
        Camera.onPreRender += c => {
            if (self != null) {
            }
        };
        */
        //GetComponent<MeshRenderer>().sortingLayerName = "Raycast";
    }


    record MeshIx(int verts, int tris);

    public void RunRaycast(float center, float span) {
        RunRaycast(center - span, center + span, false);
    }
    public void RunRaycast() {
        RunRaycast(0, Mathf.PI * 2, true);
    }
    public CastResult.Type GetType(GameObject obj) {
        if (obj.tag == "Death") {
            return CastResult.Type.Danger;
        } else if (obj.tag == "Enemy") {
            obj.GetComponent<EnemyBehavior>().StartChasing(playerPos.position);
            return CastResult.Type.Danger;
        }
        return CastResult.Type.Safe;
    }
    void RunRaycast(float start, float end, bool doWrap) {
            if (Time.time < Timeout) return;
            Timeout = Time.time + MaxDistance / PropSpeed;
            Vector2 pos = playerPos.position;
            var chunks = (int) Mathf.Ceil((end - start) / (Mathf.PI * 2) * Span);
            var castResults = new CastResult[chunks];
            var numResults = 0;
            for (int i = 0; i < chunks; i++) {
                var angle = Mathf.Lerp(start, end, (float) i / (doWrap ? chunks : chunks - 1));
                var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                var hit = Physics2D.Raycast(pos, dir, MaxDistance);
                if (hit.collider != null) {
                    var obj = hit.collider.gameObject;
                    castResults[i] = new CastResult.Hit(new CastResult.Info(obj.GetHashCode(), GetType(obj)), hit.point);
                    numResults++;
                } else {
                    castResults[i] = new CastResult.Miss();
                }
            }
            var lineGroups = new List<CastResult.Collection>();
            CastResult lastVal = null;
            float thresh = ConnectThreshold * Mathf.PI * 2 / Span;
            foreach (var cast in castResults) {
                var info = cast.GetInfo();
                if (cast is CastResult.Hit hit) {
                    var threshMul = (hit.point - pos).magnitude;
                    if (lineGroups.Count == 0 || !(lastVal is CastResult res) || !res.IsNear(cast, thresh * threshMul)) {
                        lineGroups.Add(new CastResult.Collection(hit.info, new List<Vector2>()));
                    }
                    lineGroups.Last().points.Add(hit.point);
                }
                lastVal = cast;
            }
            if (doWrap) {
                var threshMul = (lineGroups.Last().points.Last() - pos).magnitude;
                if (lineGroups.Count > 1 && castResults[0].IsNear(castResults.Last(), thresh * threshMul)) {
                    var last = lineGroups.Last().points;
                    lineGroups.RemoveAt(lineGroups.Count - 1);
                    lineGroups[0] = new CastResult.Collection(lineGroups[0].info, new List<Vector2>(last.Concat(lineGroups[0].points)));
                }
            }
            Debug.Log("[Ray] " + numResults + " vertices, " + lineGroups.Count + " sectors");

            // initial buffers for vtx/tri
            var vertices = new Vector3[20 * numResults];
            // u: raw light, v: angle magnitude
            var vtxData = new Vector3[20 * numResults];
            var colorData = new Vector2[20 * numResults];
            var triangles = new int[60 * (numResults - lineGroups.Count)];
            var ix = new MeshIx(0, 0);
            foreach (var coll in lineGroups) {
                var points = coll.points;
                var vtxDataIn = new Vector3[points.Count];
                var colorFlag = (float) (int) coll.info.type;
                for (int i = 0; i < points.Count; i++) {
                    var tangent = (i == 0 ? points[i] : points[i - 1]) - (i == points.Count - 1 ? points[i] : points[i + 1]);
                    var offset = pos - points[i];
                    var illumination = Vector2.Dot(tangent.normalized, Vector2.Perpendicular(offset).normalized);
                    //if ((i == 0 || i == points.Count - 1) && points.Count >= 3) illumination /= 2;
                    vtxDataIn[i] = new Vector3(illumination * BasePower / offset.sqrMagnitude, offset.magnitude / PropSpeed, colorFlag);
                }
                var newIx = buildLine(ix, points, vertices, vtxDataIn, vtxData, triangles);
                ix = newIx;
            }
            var mesh = new Mesh();
            mesh.SetVertices(vertices, 0, ix.verts);
            mesh.SetTriangles(triangles, 0, ix.tris, 0, true, 0);
            mesh.SetUVs(0, vtxData, 0, ix.verts);
            var meshObj = Instantiate(RenderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            meshObj.GetComponent<MeshFilter>().mesh = mesh;
            meshObj.GetComponent<Detonate>().playerPos = playerPos;
            meshObj.GetComponent<Detonate>().MasterCopy = false;
    }
    MeshIx buildLine<T>(MeshIx ix, List<Vector2> points, Vector3[] vertices, T[] vtxDataIn, T[] vtxData, int[] triangles) {
        int vix = ix.verts, tix = ix.tris;
        var joinAngles = new float[points.Count];
        var joinOffsets = new float[points.Count];
        if (points.Count < 2) return ix;
        for (var i = 2; i < points.Count; i++) {
            var prev = points[i - 2];
            var curr = points[i - 1];
            var next = points[i];
            var left = prev - curr;
            var right = curr - next;
            var angle = joinAngles[i - 1] = Vector2.SignedAngle(left, right) * Mathf.PI / 180;
            joinOffsets[i - 1] = Mathf.Sin(angle) / (1 + Mathf.Cos(angle));
        }
        if (points.Count == 2) {
            var p1 = points[0];
            var p2 = points[1];
            var normal = (p2 - p1).normalized * LineWidth;
            var edge = new Vector2(-normal.y, normal.x);
            vertices[vix + 0] = p1 + edge;
            vertices[vix + 1] = p1 - edge;
            vertices[vix + 2] = p2 - edge;
            vertices[vix + 3] = p2 + edge;
            vtxData[vix + 0] = vtxDataIn[0];
            vtxData[vix + 1] = vtxDataIn[0];
            vtxData[vix + 2] = vtxDataIn[1];
            vtxData[vix + 3] = vtxDataIn[1];
            triangles[tix + 0] = vix + 0;
            triangles[tix + 1] = vix + 1;
            triangles[tix + 2] = vix + 2;
            triangles[tix + 3] = vix + 0;
            triangles[tix + 4] = vix + 2;
            triangles[tix + 5] = vix + 3;
            vix += 4;
            tix += 6;
        } else {
            for (var i = 1; i < points.Count; i++) {
                var p1 = points[i - 1];
                var p2 = points[i];
                // vector along p1 to p2
                var normal = (p2 - p1).normalized * LineWidth;
                // rotate by 90 to create the hexagon
                var edge = new Vector2(-normal.y, normal.x);
                var jlow = joinOffsets[i - 1];
                var jhigh = joinOffsets[i];
                // hexagon vtxs
                vertices[vix + 0] = (p1 + Mathf.Max(0, jlow) * normal) + edge;
                vertices[vix + 1] = p1;
                vertices[vix + 2] = (p1 + Mathf.Max(0, -jlow) * normal) - edge;
                vertices[vix + 3] = (p2 - Mathf.Max(0, -jhigh) * normal) - edge;
                vertices[vix + 4] = p2;
                vertices[vix + 5] = (p2 - Mathf.Max(0, jhigh) * normal) + edge;
                vtxData[vix + 0] = vtxDataIn[i - 1];
                vtxData[vix + 1] = vtxDataIn[i - 1];
                vtxData[vix + 2] = vtxDataIn[i - 1];
                vtxData[vix + 3] = vtxDataIn[i];
                vtxData[vix + 4] = vtxDataIn[i];
                vtxData[vix + 5] = vtxDataIn[i];
                // hexagon fan
                for (var j = 1; j <= 4; j++) {
                    triangles[tix + 0] = vix + 0;
                    triangles[tix + 1] = vix + j;
                    triangles[tix + 2] = vix + j + 1;
                    tix += 3;
                }
                vix += 6;
                if (i >= 2) {
                    var p1Ix = vix - 5;
                    var sangle = joinAngles[i - 1];
                    var angle = Mathf.Abs(sangle);
                    var activeEdge = Mathf.Sign(sangle) * edge;
                    var joinStepsF = Mathf.Ceil(angle * 180 / (CapTheta * Mathf.PI));
                    var joinSteps = (int) joinStepsF;
                    if (joinSteps < 1) continue;
                    for (int j = 0; j <= joinSteps; j++) {
                        vertices[vix + j] = p1 - Rotate(activeEdge, -sangle * (j / joinStepsF));
                        vtxData[vix + j] = vtxDataIn[i - 1];
                    }
                    for (int j = 0; j < joinSteps; j++) {
                        triangles[tix + 0] = p1Ix;
                        triangles[tix + 1] = vix + j;
                        triangles[tix + 2] = vix + j + 1;
                        tix += 3;
                    }
                    vix += joinSteps + 1;
                }
            }
        }
        System.Action<int, Vector2> buildCap = (ix, prev) => {
            var pos = points[ix];
            var joinStepsF = Mathf.Ceil(180 / CapTheta);
            var joinSteps = (int) joinStepsF;
            var edge = Vector2.Perpendicular((pos - prev).normalized) * LineWidth;
            vertices[vix] = pos;
            vtxData[vix] = vtxDataIn[ix];
            vix++;
            for (int i = 0; i <= joinSteps; i++) {
                vertices[vix + i] = pos - Rotate(edge, Mathf.PI * (i / joinStepsF));
                vtxData[vix + i] = vtxDataIn[ix];
            }
            for (int j = 0; j < joinSteps; j++) {
                triangles[tix + 0] = vix - 1;
                triangles[tix + 1] = vix + j;
                triangles[tix + 2] = vix + j + 1;
                tix += 3;
            }
            vix += joinSteps + 1;
        };
        buildCap(0, points[1]);
        buildCap(points.Count - 1, points[points.Count - 2]);
        return new MeshIx(vix, tix);
    }
    static Vector2 Rotate(Vector2 val, float angle) {
        float cos = Mathf.Cos(angle), sin = Mathf.Sin(angle);
        return new Vector2(val.x * cos - val.y * sin, val.y * cos + val.x * sin);
    }
}
