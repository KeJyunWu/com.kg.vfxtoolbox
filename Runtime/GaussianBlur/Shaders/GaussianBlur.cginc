#define PI 3.14159265

#ifdef MEDIUM_KERNEL
	#define KERNEL_SIZE 35
#elif BIG_KERNEL
	#define KERNEL_SIZE 127
#else //LITTLE_KERNEL
	#define KERNEL_SIZE 7
#endif

float gauss(float x, float sigma)
{
	return  1.0f / (2.0f * PI * sigma * sigma) * exp(-(x * x) / (2.0f * sigma * sigma));
}

float gauss(float x, float y, float sigma)
{
    return  1.0f / (2.0f * PI * sigma * sigma) * exp(-(x * x + y * y) / (2.0f * sigma * sigma));
}

struct pixel_info
{
	sampler2D tex;
	float2 uv;
	float4 texelSize;
};

float4 GaussianBlur(float2 texelSize, float2 uv, sampler2D tex, float sigma, float2 dir)
{
	float4 o = 0;
	float sum = 0;
	float2 uvOffset;
	float weight;
	
	for(int kernelStep = - KERNEL_SIZE / 2; kernelStep <= KERNEL_SIZE / 2; ++kernelStep)
	{
		uvOffset = uv;
		uvOffset.x += ((kernelStep) *texelSize.x) * dir.x;
		uvOffset.y += ((kernelStep) *texelSize.y) * dir.y;
		weight = gauss(kernelStep, sigma) + gauss(kernelStep+1, sigma);
		o += tex2D(tex, uvOffset) * weight;
		sum += weight;
	}
	o *= (1.0f / sum);
	return o;
}

float4 GaussianBlurLinearSampling(float2 texelSize, float2 uv, sampler2D tex, float sigma, float2 dir)
{
	float4 o = 0;
	float sum = 0;
	float2 uvOffset;
	float weight;
	
	for(int kernelStep = - KERNEL_SIZE / 2; kernelStep <= KERNEL_SIZE / 2; kernelStep += 2)
	{
		uvOffset = uv;
		uvOffset.x += ((kernelStep+0.5f) * texelSize.x) * dir.x;
		uvOffset.y += ((kernelStep+0.5f) * texelSize.y) * dir.y;
		weight = gauss(kernelStep, sigma) + gauss(kernelStep+1, sigma);
		o += tex2D(tex, uvOffset) * weight;
		sum += weight;
	}
	o *= (1.0f / sum);
	return o;
}

float4 KawaseBlur(pixel_info pinfo, int pixelOffset)
{
	float4 o = 0;
	o += tex2D(pinfo.tex, pinfo.uv + (float2(pixelOffset + 0.5,pixelOffset + 0.5) * pinfo.texelSize)) * 0.25;
	o += tex2D(pinfo.tex, pinfo.uv + (float2(-pixelOffset - 0.5,pixelOffset + 0.5) * pinfo.texelSize))* 0.25;
	o += tex2D(pinfo.tex, pinfo.uv + (float2(-pixelOffset - 0.5,-pixelOffset - 0.5) * pinfo.texelSize)) * 0.25;
	o += tex2D(pinfo.tex, pinfo.uv + (float2(pixelOffset + 0.5,-pixelOffset - 0.5) * pinfo.texelSize)) * 0.25;
	return o;
}