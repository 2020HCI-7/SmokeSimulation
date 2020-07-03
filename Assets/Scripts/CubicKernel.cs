using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicKernel : MonoBehaviour
{
    public static CubicKernel instance;

    private void Awake() {
        if (instance != null)
            Destroy(instance);
        instance = this;
    }

	public static float m_radius { get; protected set;}
	public static float m_k { get; protected set;}
	public static float m_l { get; protected set;}
	public static float m_W_zero { get; protected set;}

    // struct SetRadiusInputStruct
    // {
    //     public float pi;
    //     public float radius;
    // }
    // struct SetRadiusOutputStruct
    // {
    //     public float k;
    //     public float l;
    // }
    // struct WInputStruct
    // {
    //     public float rl;
    //     public float radius;
    //     public float k;
    // }
    // struct WOutputStruct
    // {
    //     public float res;
    // }
    // struct GradWInputStruct
    // {
    //     public Vector3 r;
    //     public float radius;
    //     public float l;
    // }
    // struct GradWOutputStruct
    // {
    //     public Vector3 res;
    // }
    // public ComputeShader m_compute;

    // //public ComputeShader m_setRadius_computeShader;
    // private static ComputeBuffer m_setRadius_input_buffer;
    // private static ComputeBuffer m_setRadius_output_buffer;
    // private static int SetRadiusKernel;
    // //public ComputeShader m_W_computeShader;
    // private static ComputeBuffer m_W_input_buffer;
    // private static ComputeBuffer m_W_output_buffer;
    // private static int WKernel;
    // //public ComputeShader m_gradW_computeShader;
    // private static ComputeBuffer m_gradW_input_buffer;
    // private static ComputeBuffer m_gradW_output_buffer;
    // private static int gradWKernel;
    // private static void CreateBuffer()
    // {
    //     m_setRadius_input_buffer = new ComputeBuffer(1, sizeof(float) * 2);
    //     m_setRadius_output_buffer = new ComputeBuffer(1, sizeof(float) * 2);
    //     m_W_input_buffer = new ComputeBuffer(1, sizeof(float) * 3);
    //     m_W_output_buffer = new ComputeBuffer(1, sizeof(float) * 1);
    //     m_gradW_input_buffer = new ComputeBuffer(1, sizeof(float) * 5);
    //     m_gradW_output_buffer = new ComputeBuffer(1, sizeof(float) * 3);
    // }
    // private void Start() {
    //     SetRadiusKernel = instance.m_compute.FindKernel("SetRadius");
    //     // WKernel = instance.m_compute.FindKernel("W");
    //     // gradWKernel = instance.m_compute.FindKernel("GradW");
    //     CreateBuffer();
    // }

    public static float getRadius() { return m_radius; }
    public static void setRadius(float val) 
    {
        m_radius = val;
        float pi = Mathf.PI;
        float h3 = m_radius * m_radius * m_radius;

        m_k = 8.0f / (pi * h3);
        m_l = 48.0f / (pi * h3);
        // SetRadiusInputStruct[] inputs = new SetRadiusInputStruct[1];
        // inputs[0].radius = m_radius; inputs[0].pi = Mathf.PI;
        // m_setRadius_input_buffer.SetData(inputs);
        // instance.m_compute.SetBuffer(SetRadiusKernel, "SetRadiusInput", m_setRadius_input_buffer);
        // instance.m_compute.SetBuffer(SetRadiusKernel, "SetRadiusOutput", m_setRadius_output_buffer);
        // instance.m_compute.Dispatch(SetRadiusKernel, 8, 8, 1);

        // SetRadiusOutputStruct[] output = new SetRadiusOutputStruct[1];
        // m_setRadius_output_buffer.GetData(output);

        // m_k = output[0].k;
        // m_l = output[0].l;

        m_W_zero = W(Vector3.zero);
    }

    public static float W(Vector3 r)
    {
        float res = 0.0f;
        float rl = r.magnitude;
        float q = rl/m_radius;

        if (q <= 0.5f)
        {
            float q2 = q * q;
            float q3 = q2 * q;
            res = m_k * (6.0f * q3 - 6.0f * q2 + 1.0f);
        }
        else
        {
            res = m_k * (2.0f * Mathf.Pow(1.0f - q, 3));
        }

        // WInputStruct[] inputs = new WInputStruct[1];
        // inputs[0].rl = rl; inputs[0].radius = m_radius; inputs[0].k = m_k;
        // m_W_input_buffer.SetData(inputs);
        // instance.m_compute.SetBuffer(WKernel, "WInput", m_W_input_buffer);
        // instance.m_compute.SetBuffer(WKernel, "WOutput", m_W_output_buffer);
        // instance.m_compute.Dispatch(WKernel, 8, 8, 1);

        // WOutputStruct[] output = new WOutputStruct[1];
        // m_W_output_buffer.GetData(output);

        // res = output[0].res;
        return res;
    }
    public static Vector3 gradW(Vector3 r)
    {
        Vector3 res = new Vector3();
        float rl = r.magnitude;
        float q = rl/m_radius;

        if (rl > 0)
        {
            Vector3 gradq = r * (1.0f / (rl*m_radius));
            if (q <= 0.5f)
            {
                res = m_l*q*(3.0f * q - 2.0f)*gradq;
            }
            else
            {
                float factor = 1.0f - q;
                res = m_l*(-factor*factor)*gradq;
            }
        }
        return res;
    }
    public static float W_zero()
    {
        return m_W_zero;
    }

}
