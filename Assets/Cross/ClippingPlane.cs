using UnityEngine;

[ExecuteInEditMode]
public class ClippingPlane : MonoBehaviour
{
    public Material mat;  // <--- Put the clipped object shader's material here !

    Plane m_plane;
    Vector4 m_planeVector;

    private void Start()
    {
        m_plane = new Plane(transform.up, transform.position);
        m_planeVector = new Vector4(m_plane.normal.x, m_plane.normal.y, m_plane.normal.z, m_plane.distance);
    }

    void Update()
    {
        m_plane.SetNormalAndPosition(transform.up, transform.position);
        m_planeVector.x = m_plane.normal.x;
        m_planeVector.y = m_plane.normal.y;
        m_planeVector.z = m_plane.normal.z;
        m_planeVector.w = m_plane.distance;
        mat.SetVector("_Plane", m_planeVector);
    }
}