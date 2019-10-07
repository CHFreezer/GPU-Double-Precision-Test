using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateDoublePrecision : MonoBehaviour
{
    public ComputeShader shader;
    public InputField InputA;
    public InputField InputB;
    public Text TextResult;
    public Button ButtonAdd;
    public Button ButtonSubtract;
    public Button ButtonMultiply;
    public Button ButtonDivide;

    struct Data_struct
    {
        public double a;
        public double b;
        public int _operator;
    }

    void Start()
    {
        Screen.SetResolution(640, 480, FullScreenMode.Windowed);

        if (SystemInfo.supportsComputeShaders)
        {
            TextResult.text = "您的显卡支持GPU计算着色器 (ComputeShaders)";
        }
        else
        {
            TextResult.text = "<color=red>您的显卡不支持GPU计算着色器 (ComputeShaders)</color>";
        }

        InputA.onEndEdit.AddListener(text =>
        {
            double.TryParse(InputA.text, out double result);
            InputA.text = System.Convert.ToDecimal(result).ToString();
        });
        InputB.onEndEdit.AddListener(text =>
        {
            double.TryParse(InputB.text, out double result);
            InputB.text = System.Convert.ToDecimal(result).ToString();
        });

        ButtonAdd.onClick.AddListener(() => Calculate(1));
        ButtonSubtract.onClick.AddListener(() => Calculate(2));
        ButtonMultiply.onClick.AddListener(() => Calculate(3));
        ButtonDivide.onClick.AddListener(() => Calculate(4));
    }

    void Calculate(int _operator)
    {
        Data_struct[] data = new Data_struct[1];
        double.TryParse(InputA.text, out data[0].a);
        double.TryParse(InputB.text, out data[0].b);
        data[0]._operator = _operator;
        ComputeBuffer inBuffer = new ComputeBuffer(1, 24);
        ComputeBuffer outBuffer = new ComputeBuffer(1, 8);
        int kernel = shader.FindKernel("CSMain");
        inBuffer.SetData(data);
        shader.SetBuffer(kernel, "inBuffer", inBuffer);
        shader.SetBuffer(kernel, "Result", outBuffer);
        shader.Dispatch(kernel, 1, 1, 1);
        double[] result = new double[1];
        outBuffer.GetData(result);
        inBuffer.Release();
        outBuffer.Release();
        TextResult.text = "GPU运算结果：" + System.Convert.ToDecimal(result[0]);
    }
}
