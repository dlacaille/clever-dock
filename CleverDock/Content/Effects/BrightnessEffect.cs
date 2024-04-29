﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace CleverDock.Effects;

public class BrightnessEffect : ShaderEffect
{
    // Dependencies
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty("Input", typeof(BrightnessEffect), 0);

    public static readonly DependencyProperty BrightnessProperty =
        DependencyProperty.Register("Brightness", typeof(double), typeof(BrightnessEffect),
            new UIPropertyMetadata(0.0d, PixelShaderConstantCallback(0)));

    public static readonly DependencyProperty ContrastProperty =
        DependencyProperty.Register("Contrast", typeof(double), typeof(BrightnessEffect),
            new UIPropertyMetadata(0.0d, PixelShaderConstantCallback(1)));

    // Shader
    private static readonly PixelShader m_pixelshader =
        new()
        {
            UriSource = new Uri("pack://application:,,,/CleverDock;component/Content/Effects/BrightnessEffect.ps")
        };

    public BrightnessEffect()
    {
        PixelShader = m_pixelshader;
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(BrightnessProperty);
        UpdateShaderValue(ContrastProperty);
    }

    // Properties
    public Brush Input
    {
        get => (Brush)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }

    public double Brightness
    {
        get => (double)GetValue(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }

    public double Contrast
    {
        get => (double)GetValue(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }
}