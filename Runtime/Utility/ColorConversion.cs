using System;
using System.Numerics;
using UnityEngine;

namespace UltraCombos.VFXToolBox
{
    public class ColorConversion
    {
        public static Color HSVToRGB(Color color)
        {
            float[] rgb = new float[3];
            float[] col = new float[] { color.r, color.g, color.b };

            if (col[1] == 0.0f)
            {
                rgb[0] = col[2];
                rgb[1] = col[2];
                rgb[1] = col[2];
            }
            else
            {
                float h = col[0] * 6.0f;

                if (h == 6.0f)
                {
                    h = 0.0f;
                }

                int v_i = (int)h;
                float v_1 = col[2] * (1.0f - col[1]);
                float v_2 = col[2] * (1.0f - (col[1] * (h - v_1)));
                float v_3 = col[2] * (1.0f - (col[1] * (1.0f - (h - v_i))));

                if (v_i == 0)
                {
                    rgb[0] = col[2];
                    rgb[1] = v_3;
                    rgb[2] = v_1;
                }
                else if (v_i == 1)
                {
                    rgb[0] = v_2;
                    rgb[1] = col[2];
                    rgb[2] = v_1;
                }
                else if (v_i == 2)
                {
                    rgb[0] = v_2;
                    rgb[1] = col[2];
                    rgb[2] = v_3;
                }
                else if (v_i == 3)
                {
                    rgb[0] = v_1;
                    rgb[1] = v_2;
                    rgb[2] = col[2];
                }
                else if (v_i == 4)
                {
                    rgb[0] = v_3;
                    rgb[1] = v_1;
                    rgb[2] = col[2];
                }
                else
                {
                    rgb[0] = col[2];
                    rgb[1] = v_1;
                    rgb[2] = v_2;
                }
            }

            return new Color(rgb[0], rgb[1], rgb[2], color.a);
        }

        /// <summary>
        /// Converts HSV to RGB Color Space
        /// </summary>
        /// <param name="colors">Vector4 array of colors in HSV Color Space</param>
        /// <returns>Vector4 array of colors in RGB Color Space</returns>
        public static Color[] HSVToRGB(Color[] colors)
        {
            Color[] rgb = new Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                rgb[i] = HSVToRGB(colors[i]);
            }

            return rgb;
        }

        /// <summary>
        /// Converts Lab to RGB Colorspace
        /// </summary>
        /// <param name="color">Color as Vector4</param>
        /// <returns>Vector 4 in RGB Colorspace</returns>
        public static Color LabToRGB(Color color)
        {
            Color xyz = LabToXYZ(color);
            return XYZToRGB(xyz);
        }

        /// <summary>
        /// Converts Lab to RGB
        /// </summary>
        /// <param name="colors">Colors in Lab</param>
        /// <returns>Vector 4 array in RGB</returns>
        public static Color[] LabToRGB(Color[] colors)
        {
            Color[] rgb = new Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                rgb[i] = LabToRGB(colors[i]);
            }

            return rgb;
        }

        /// <summary>
        /// Converts Lab to XYZ Color Space
        /// </summary>
        /// <returns>Vector4 in XYZ Color Space</returns>
        /// <param name="color">Color as Vector4</param>
        public static Color LabToXYZ(Color color)
        {
            float[] xyz = new float[3];
            float[] col = new float[] { color.r, color.g, color.b };
            xyz[1] = (col[0] + 16.0f) / 116.0f;
            xyz[0] = (col[1] / 500.0f) + xyz[0];
            xyz[2] = xyz[0] - (col[2] / 200.0f);

            for (int i = 0; i < 3; i++)
            {
                float pow = xyz[i] * xyz[i] * xyz[i];
                if (pow > .008856f)
                {
                    xyz[i] = pow;
                }
                else
                {
                    xyz[i] = (16.0f / 116.0f) / 7.787f;
                }
            }

            xyz[0] = xyz[0] * 95.047f;
            xyz[1] = xyz[1] * 100.0f;
            xyz[2] = xyz[2] * 108.883f;

            return new Color(xyz[0], xyz[1], xyz[2], color.a);
        }

        /// <summary>
        /// Converts Lab to XYZ Color Space
        /// </summary>
        /// <param name="colors">Vector 4 array or XYZ colors</param>
        /// <returns>Vector4 array of colors in XYZ Color Spce</returns>
        public static Color[] LabToXYZ(Color[] colors)
        {
            Color[] xyz = new Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                xyz[i] = LabToXYZ(colors[i]);
            }

            return xyz;
        }

        /// <summary>
        /// Converts RGB to HSV Colorspace
        /// </summary>
        /// <param name="color">Color as Vector4</param>
        /// <returns>Vector 4 in HSV Colorspace</returns>
        public static Color RGBToHSV(Color color)
        {
            float[] hsv = new float[3];
            float[] rgb = new float[3];
            float[] col = new float[] { color.r, color.g, color.b };

            float min = Math.Min(col[0], col[1]);
            min = Math.Min(min, col[2]);
            float max = Math.Max(col[0], col[1]);
            max = Math.Max(max, col[2]);
            float maxDelta = max - min;

            hsv[2] = max;

            if (max == 0.0f)
            {
                hsv[0] = 0.0f;
                hsv[1] = 0.0f;
            }
            else
            {
                hsv[1] = maxDelta / max;

                for (int i = 0; i < 3; i++)
                {
                    rgb[i] = (((max - col[i]) / 6.0f) + (maxDelta / 2.0f)) / maxDelta;
                }

                if (col[0] == max)
                {
                    hsv[0] = rgb[2] - rgb[1];
                }
                else if (col[1] == max)
                {
                    hsv[0] = (1.0f / 3.0f) + rgb[0] - rgb[2];
                }
                else if (col[2] == max)
                {
                    hsv[0] = (2.0f / 3.0f) + rgb[2] - rgb[0];
                }

                if (hsv[0] < 0)
                {
                    hsv[0] += 1.0f;
                }
                else if (hsv[0] > 1.0f)
                {
                    hsv[0] -= 1.0f;
                }
            }

            return new Color(hsv[0], hsv[1], hsv[2], color.a);
        }

        /// <summary>
        /// Converts RGB to HSV Color Space
        /// </summary>
        /// <param name="colors">Vector4 array of colors in RGB Color Space</param>
        /// <returns>Vector4 or colors in HSV Color Space</returns>
        public static Color[] RGBToHSV(Color[] colors)
        {
            Color[] hsv = new Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                hsv[i] = RGBToHSV(colors[i]);
            }

            return hsv;
        }

        /// <summary>
        /// Converts RGB to LAB Colorspace
        /// </summary>
        /// <param name="color">Color as Vector4</param>
        /// <returns>Vector4 in Lab Colorspace</returns>
        public static Color RGBToLab(Color color)
        {
            float[] xyz = new float[3];
            float[] lab = new float[3];
            float[] rgb = new float[] { color.r, color.g, color.b, color.a };

            if (rgb[0] > .04045f)
            {
                rgb[0] = (float)Math.Pow((rgb[0] + .0055) / 1.055, 2.4);
            }
            else
            {
                rgb[0] = rgb[0] / 12.92f;
            }

            if (rgb[1] > .04045f)
            {
                rgb[1] = (float)Math.Pow((rgb[1] + .0055) / 1.055, 2.4);
            }
            else
            {
                rgb[1] = rgb[1] / 12.92f;
            }

            if (rgb[2] > .04045f)
            {
                rgb[2] = (float)Math.Pow((rgb[2] + .0055) / 1.055, 2.4);
            }
            else
            {
                rgb[2] = rgb[2] / 12.92f;
            }

            xyz[0] = ((rgb[0] * .412453f) + (rgb[1] * .357580f) + (rgb[2] * .180423f)) * 95.047f;
            xyz[1] = ((rgb[0] * .212671f) + (rgb[1] * .715160f) + (rgb[2] * .072169f)) * 100.0f;
            xyz[2] = ((rgb[0] * .019334f) + (rgb[1] * .119193f) + (rgb[2] * .950227f)) * 108.883f;

            if (xyz[0] > .008856f)
            {
                xyz[0] = (float)Math.Pow(xyz[0], 1.0 / 3.0);
            }
            else
            {
                xyz[0] = (xyz[0] * 7.787f) + (16.0f / 116.0f);
            }

            if (xyz[1] > .008856f)
            {
                xyz[1] = (float)Math.Pow(xyz[1], 1.0 / 3.0);
            }
            else
            {
                xyz[1] = (xyz[1] * 7.787f) + (16.0f / 116.0f);
            }

            if (xyz[2] > .008856f)
            {
                xyz[2] = (float)Math.Pow(xyz[2], 1.0 / 3.0);
            }
            else
            {
                xyz[2] = (xyz[2] * 7.787f) + (16.0f / 116.0f);
            }

            lab[0] = (116.0f * xyz[1]) - 16.0f;
            lab[1] = 500.0f * (xyz[0] - xyz[1]);
            lab[2] = 200.0f * (xyz[1] - xyz[2]);

            return new Color(lab[0], lab[1], lab[2], color.a);
        }

        /// <summary>
        /// Converts Array of RGB Colors to Lab
        /// </summary>
        /// <param name="colors">Colors</param>
        /// <returns>Vector4 array of colors</returns>
        public static Color[] RGBToLab(Color[] colors)
        {
            Color[] lab = new Color[colors.Length];

            for (int i = 0; i < lab.Length; i++)
            {
                lab[i] = RGBToLab(colors[i]);
            }

            return lab;
        }

        /// <summary>
        /// Converts RGB to XYZ color space
        /// </summary>
        /// <returns>Vector4 in XYZ color space </returns>
        /// <param name="color">Color as Vector4</param>
        public static Color RGBToXYZ(Color color)
        {
            float[] xyz = new float[4];
            float[] rgb = new float[] { color.r, color.g, color.b, color.a };

            for (int i = 0; i < 3; i++)
            {
                if (rgb[i] > .04045f)
                {
                    rgb[i] = (float)Math.Pow((rgb[i] + .0055) / 1.055, 2.4);
                }
                else
                {
                    rgb[i] = rgb[i] / 12.92f;
                }
            }

            xyz[0] = (rgb[0] * .412453f) + (rgb[1] * .357580f) + (rgb[2] * .180423f);
            xyz[1] = (rgb[0] * .212671f) + (rgb[1] * .715160f) + (rgb[2] * .072169f);
            xyz[2] = (rgb[0] * .019334f) + (rgb[1] * .119193f) + (rgb[2] * .950227f);

            return new Color(xyz[0], xyz[1], xyz[2], color.a);
        }

        /// <summary>
        /// Converts RGB to XYZ Color Space
        /// </summary>
        /// <param name="colors">Colors as Vector4</param>
        /// <returns>Vector 4 array in XYZ Color Space</returns>
        public static Color[] RGBToXYZ(Color[] colors)
        {
            Color[] xyz = new Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                xyz[i] = RGBToXYZ(colors[i]);
            }

            return xyz;
        }

        /// <summary>
        /// Converts RGB to YUV Color Space
        /// </summary>
        /// <returns>Vector4 in TUY Color Space</returns>
        /// <param name="color">Color as Vector4</param>
        public static Color RGBToYUV(Color color)
        {
            float[] yuv = new float[3];
            float[] col = new float[] { color.r, color.g, color.b };

            yuv[0] = (.299f * col[0]) + (.05978f * col[1]) + (.114f * col[2]);
            yuv[1] = (col[2] - yuv[0]) * .565f;
            yuv[2] = (col[0] - yuv[0]) * .713f;

            return new Color(yuv[0], yuv[1], yuv[2], color.a);
        }

        /// <summary>
        /// Converts RGB to YUV Color Space
        /// </summary>
        /// <param name="colors">Vector4 array of colors in RGB Color Space</param>
        /// <returns>Vector4 array of colors in YUV Color Space</returns>
        public static Color[] RGBToYUV(Color[] colors)
        {
            Color[] yuv = new Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                yuv[i] = RGBToYUV(colors[i]);
            }

            return yuv;
        }

        /// <summary>
        /// Converts XYZ to Lab Color Space
        /// </summary>
        /// <returns>Vector4 in Lab Color Space</returns>
        /// <param name="color">Color as Vector4</param>
        public static Color XYZToLab(Color color)
        {
            float[] lab = new float[3];
            float[] xyz = new float[3];
            float[] col = new float[] { color.r, color.g, color.b };

            xyz[0] = col[0] * 95.047f;
            xyz[1] = col[1] * 100.0f;
            xyz[2] = col[2] * 108.883f;

            for (int i = 0; i < 3; i++)
            {
                if (xyz[i] > .008856f)
                {
                    xyz[i] = (float)Math.Pow(xyz[i], 1.0 / 3.0);
                }
                else
                {
                    xyz[i] = (xyz[i] * 7.787f) + (16.0f / 116.0f);
                }
            }

            lab[0] = (116.0f * xyz[1]) - 16.0f;
            lab[1] = 500.0f * (xyz[0] - xyz[1]);
            lab[2] = 200.0f * (xyz[1] - xyz[2]);

            return new Color(lab[0], lab[1], lab[2], color.a);
        }

        /// <summary>
        /// Converts XYZ to Lab
        /// </summary>
        /// <param name="colors">Colors as Vector4</param>
        /// <returns>Vector4 array in Lab Color Space</returns>
        public static Color[] XYZToLab(Color[] colors)
        {
            Color[] lab = new Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                lab[i] = XYZToLab(colors[i]);
            }

            return lab;
        }

        /// <summary>
        /// Converts XYZ to RGB Color Space
        /// </summary>
        /// <returns>Vector4 in RGB Color Space</returns>
        /// <param name="color">Color as Vector4</param>
        public static Color XYZToRGB(Color color)
        {
            float[] rgb = new float[3];
            float[] xyz = new float[3];
            float[] col = new float[] { color.r, color.g, color.b };

            for (int i = 0; i < 3; i++)
            {
                xyz[i] = col[i] / 100.0f;
            }

            rgb[0] = (xyz[0] * 3.240479f) + (xyz[1] * -1.537150f) + (xyz[2] * -.498535f);
            rgb[1] = (xyz[0] * -.969256f) + (xyz[1] * 1.875992f) + (xyz[2] * .041556f);
            rgb[2] = (xyz[0] * .055648f) + (xyz[1] * -.204043f) + (xyz[2] * 1.057311f);

            for (int i = 0; i < 3; i++)
            {
                if (rgb[i] > .0031308f)
                {
                    rgb[i] = (1.055f * (float)Math.Pow(rgb[i], 1.0 / 2.4)) - .055f;
                }
                else
                {
                    rgb[i] = rgb[i] * 12.92f;
                }
            }

            return new Color(rgb[0], rgb[1], rgb[2], color.a);
        }

        /// <summary>
        /// Converts XYZ to EGB  Color Space
        /// </summary>
        /// <param name="colors">Colors as Vector4</param>
        /// <returns>Vector4 array in RGB Color Space</returns>
        public static Color[] XYZToRGB(Color[] colors)
        {
            Color[] rgb = new Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                rgb[i] = XYZToRGB(colors[i]);
            }

            return rgb;
        }

        /// <summary>
        /// Converts YUV To RGB Colorspace
        /// </summary>
        /// <param name="color">Color as Vector4</param>
        /// <returns>Vector 4 in RGB colorspace</returns>
        public static Color YUVToRGB(Color color)
        {
            float[] rgb = new float[3];
            float[] col = new float[] { color.r, color.g, color.b};

            rgb[0] = col[0] + (1.403f * col[2]);
            rgb[1] = col[0] - (0.344f * col[1]) - (.714f * col[2]);
            rgb[2] = col[0] + (1.770f * col[1]);

            return new Color(rgb[0], rgb[1], rgb[2], color.a);
        }

        /// <summary>
        /// Converts YUV to RGB Color Space
        /// </summary>
        /// <param name="colors">Vector4 array of colors in YUV Color Space</param>
        /// <returns>Vector 4 array or colors in RGB Color Space</returns>
        public static Color[] YUVToRGB(Color[] colors)
        {
            Color[] rgb = new Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                rgb[i] = YUVToRGB(colors[i]);
            }

            return rgb;
        }
    }
}