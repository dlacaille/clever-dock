sampler2D input : register(S0);
float brightness : register(C0);
float contrast : register(C1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(input, uv);
	float4 result = float4(color.rgb + brightness, color.a);
	result = result * (1.0+contrast)/1.0;
    result.rgb *= color.a; // Premultiplied alpha magic
	return result;
}