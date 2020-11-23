using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioSpectrum))]
public class AudioSpectrumInspector : Editor
{
    AnimationCurve curve;

    void UpdateCurve()
    {
        var spectrum = target as AudioSpectrum;

        // Create a new curve to update the UI.
        curve = new AnimationCurve();

        // Add keys for the each band.
        var bands = spectrum.OctaveBands;
        //Debug.Log(bands.Length);
        for (var i = 0; i < bands.Length; i++)
        {
            curve.AddKey(1.0f / bands.Length * i, bands[i]);
        }
    }

    public override void OnInspectorGUI()
    {
        // Update the curve only when it's playing.
        if (EditorApplication.isPlaying)
        {
            UpdateCurve();
        }
        else if (curve == null)
        {
            curve = new AnimationCurve();
        }

        // Shows the spectrum curve.
        EditorGUILayout.CurveField(curve, Color.white, new Rect(0, 0, 1.0f, 0.1f), GUILayout.Height(64));

        // Update frequently while it's playing.
        if (EditorApplication.isPlaying)
        {
            EditorUtility.SetDirty(target);
        }
    }
}