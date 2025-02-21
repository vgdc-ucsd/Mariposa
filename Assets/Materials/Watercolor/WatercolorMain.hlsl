void Watercolor_float(float2 uv, out float3 result)
{
    float4 source = Unity_Universal_SampleBuffer_BlitSource_float(uv);
    result = float3(source.r, source.g, source.b);
}