using UnityEngine;

public class CrossSectionController : MonoBehaviour
{
    public Material material; // Tham chiếu đến material sử dụng shader cross-section
    public float cutoffSpeed = 0.5f; // Tốc độ điều chỉnh mặt cắt

    private void Update()
    {
        if (material != null)
        {
            // Tăng giá trị mặt cắt theo thời gian (hoặc điều chỉnh theo cách bạn muốn)
            float newCutoff = Mathf.PingPong(Time.time * cutoffSpeed, 1.0f);
            material.SetFloat("_Cutoff", newCutoff);
        }
    }
}
