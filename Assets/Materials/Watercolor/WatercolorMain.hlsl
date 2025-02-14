void Watercolor_float(float2 uv, float4 source, out float3 result)
{
    result = float3(source.r, source.g, source.b);
    result = lerp(result, float3(uv.x, uv.y, 0.0), 0.5);
}