using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoF : MonoBehaviour {
	[SerializeField, Range(1, 30)]
	private int _iteration = 1;

	// 4色をサンプリングして色を作るマテリアル
	[SerializeField]
	private Material _material;
	[SerializeField]
	private float _foucus;

	private RenderTexture[] _renderTextures = new RenderTexture[30];


	void Start()
	{
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{


		var width = source.width;
		var height = source.height;
		var currentSource = source;

		var i = 0;

		RenderTexture currentDest = null;
		RenderTexture blurTex = RenderTexture.GetTemporary(width, height, 0, source.format); ;

		// ダウンサンプリング
		for (; i < _iteration; i++)
		{
			//width /= 2;
			//height /= 2;
			if (width < 2 || height < 2)
			{
				break;
			}
			currentDest = _renderTextures[i] = RenderTexture.GetTemporary(width, height, 0, source.format);

			// Blit時にマテリアルとパスを指定する
			Graphics.Blit(currentSource, currentDest, _material, 0);

			currentSource = currentDest;
		}

		// アップサンプリング
		for (i -= 2; i >= 0; i--)
		{
			currentDest = _renderTextures[i];

			// Blit時にマテリアルとパスを指定する
			Graphics.Blit(currentSource, currentDest, _material, 1);

			_renderTextures[i] = null;
			RenderTexture.ReleaseTemporary(currentSource);
			currentSource = currentDest;
		}

		// ぼかしテクスチャにBlit
		Graphics.Blit(currentSource, blurTex, _material, 1);
		RenderTexture.ReleaseTemporary(currentSource);

		// ブラーテクスチャ
		_material.SetTexture("_BlurTex", blurTex);

		// カメラからのピントを合わせる距離
		_material.SetFloat("_Foucus", _foucus);

		// 被写界深度
		Graphics.Blit(source, dest, _material, 2);
		RenderTexture.ReleaseTemporary(blurTex);

	}
}
