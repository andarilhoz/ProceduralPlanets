using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    private ShapeSettings settings;
    private INoiseFilter[] noiseFilter;
    public MinMax elevationMinMax;

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilter = new INoiseFilter[settings.noiseLayers.Length];
        elevationMinMax = new MinMax();
        for (var i = 0; i < noiseFilter.Length; i++)
        {
            noiseFilter[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if ( noiseFilter.Length > 0 )
        {
            firstLayerValue = noiseFilter[0].Evaluate(pointOnUnitSphere);
            if ( settings.noiseLayers[0].enable )
            {
                elevation = firstLayerValue;
            }
        }

        for (var i = 1; i < noiseFilter.Length; i++)
        {
            if ( !settings.noiseLayers[i].enable )
            {
                continue;
            }

            var mask = settings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
            elevation += noiseFilter[i].Evaluate(pointOnUnitSphere) * mask;
        }

        elevationMinMax.AddValue(elevation);
        return elevation;
    }

    public float GetScaledElevation(float unscaledElevation)
    {
        var elevation = Mathf.Max(0, unscaledElevation);
        elevation = settings.planetRadious * (1 + elevation);
        return elevation;
    }
}