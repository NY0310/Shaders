using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class ScreenSpaceLocalReflecti : MonoBehaviour {

	[SerializeField]
	private Shader _shader;

	Material _material;

	[SerializeField]
	private Vector4 _params1;

	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (_material == null)
		{
			_material = new Material(_shader);
		}

		var camera = GetComponent<Camera>();
		var view = camera.worldToCameraMatrix;
		var proj = GL.GetGPUProjectionMatrix(camera.projectionMatrix, false);
		var viewProj = proj * view;
		_material.SetMatrix("_ViewProj", viewProj);
		_material.SetMatrix("_InvViewProj", viewProj.inverse);
		_material.SetVector("_Params1", _params1);

		Graphics.Blit(src, dst, _material, 0);
	}
}
