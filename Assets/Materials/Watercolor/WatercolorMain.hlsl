float convolve(matrix <float, 3, 3> src, matrix <float, 3, 3> mask) {
    float result = 0;
    result += src[0][0] * mask[0][0];
    result += src[0][1] * mask[0][1];
    result += src[0][2] * mask[0][2];
    result += src[1][0] * mask[1][0];
    result += src[1][1] * mask[1][1];
    result += src[1][2] * mask[1][2];
    result += src[2][0] * mask[2][0];
    result += src[2][1] * mask[2][1];
    result += src[2][2] * mask[2][2];
    return result;
}
void Watercolor_float(float2 uv, out float3 result)
{
    float4 source = Unity_Universal_SampleBuffer_BlitSource_float(uv);
    result = float3(source.r, source.g, source.b);

    // Sobel filter
    float sobel_delta = 0.001;
    float3 sobelResult = float3(0, 0, 0);
    
    matrix <float, 3, 3> sobel_x = {
        1, 0, -1,
        2, 0, -2,
        1, 0, -1
    };
    
    matrix <float, 3, 3> sobel_y = {
        1, 2, 1,
        0, 0, 0,
        -1, -2, -1
    };

    float3 ul = Unity_Universal_SampleBuffer_BlitSource_float(uv + float2(-sobel_delta, sobel_delta)).rgb;
    float3 u = Unity_Universal_SampleBuffer_BlitSource_float(uv + float2(0, sobel_delta)).rgb;
    float3 ur = Unity_Universal_SampleBuffer_BlitSource_float(uv + float2(0, sobel_delta)).rgb;
    float3 l = Unity_Universal_SampleBuffer_BlitSource_float(uv + float2(-sobel_delta, 0)).rgb;
    float3 c = Unity_Universal_SampleBuffer_BlitSource_float(uv).rgb;
    float3 r = Unity_Universal_SampleBuffer_BlitSource_float(uv + float2(sobel_delta, 0)).rgb;
    float3 dl = Unity_Universal_SampleBuffer_BlitSource_float(uv + float2(-sobel_delta, -sobel_delta)).rgb;
    float3 d = Unity_Universal_SampleBuffer_BlitSource_float(uv + float2(0, -sobel_delta)).rgb;
    float3 dr = Unity_Universal_SampleBuffer_BlitSource_float(uv + float2(sobel_delta, -sobel_delta)).rgb;

    matrix <float, 3, 3> sobel_r = {
        ul.r, u.r, ur.r,
        l.r, c.r, r.r,
        dl.r, d.r, dr.r
    };

    matrix <float, 3, 3> sobel_g = {
        ul.g, u.g, ur.g,
        l.g, c.g, r.g,
        dl.g, d.g, dr.g
    };

    matrix <float, 3, 3> sobel_b = {
        ul.b, u.b, ur.b,
        l.b, c.b, r.b,
        dl.b, d.b, dr.b
    };

    sobelResult.r = (convolve(sobel_r, sobel_x) + convolve(sobel_r, sobel_y)) / 2.0;
    sobelResult.g = (convolve(sobel_g, sobel_x) + convolve(sobel_g, sobel_y)) / 2.0;
    sobelResult.b = (convolve(sobel_b, sobel_x) + convolve(sobel_b, sobel_y)) / 2.0;
    //float sobelLuminance = 0.2126 * sobelResult.r + 0.7152 * sobelResult.g + 0.0722 * sobelResult.b;
    float sobelAvg = (sobelResult.r + sobelResult.g + sobelResult.b) / 3.0;
    sobelAvg = clamp(sobelAvg, 0, 1); 

    result = float3(sobelAvg, sobelAvg, sobelAvg);
}