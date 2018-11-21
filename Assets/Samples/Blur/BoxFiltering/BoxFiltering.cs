using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFiltering : MonoBehaviour{
	[SerializeField, Range(1, 30)]
	private int _iteration = 1;

	// 4色をサンプリングして色を作るマテリアル
	[SerializeField]
	private Material _material;


	private RenderTexture[] _renderTextures = new RenderTexture[30];

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{

		var width = source.width;
		var height = source.height;
		var currentSource = source;

		var i = 0;

		RenderTexture currentDest = null;

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

		// 最後にdestにBlit
		Graphics.Blit(currentSource, dest, _material, 1);
		RenderTexture.ReleaseTemporary(currentSource);
	}

}