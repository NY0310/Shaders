using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownSampling : MonoBehaviour
{

	[SerializeField, Range(1, 30)]
	private int _iteration;

	//ダウンサンプリングしたテクスチャを格納
	private RenderTexture[] _renderTextures = new RenderTexture[30];

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{

		var width = source.width;
		var height = source.height;
		var currentSource = source;

		var i = 0;
		RenderTexture currentDest = null;
		// 段階的にダウンサンプリング
		for (; i < _iteration; i++)
		{
			width /= 2;
			height /= 2;
			if (width < 2 || height < 2)
			{
				break;
			}

			// 解像度を落としたテクスチャを生成
			currentDest = _renderTextures[i] = RenderTexture.GetTemporary(width, height, 0, source.format);
			// 出力先へダウンサンプリング
			Graphics.Blit(currentSource, currentDest);
			//ダウンサンプリングしたテクスチャを元テクスチャにコピー
			currentSource = currentDest;
		}

		// アップサンプリング
		for (i -= 2; i >= 0; i--)
		{
			currentDest = _renderTextures[i];
			Graphics.Blit(currentSource, currentDest);
			_renderTextures[i] = null;
			RenderTexture.ReleaseTemporary(currentSource);
			currentSource = currentDest;
		}
		// 最後にdestにBlit
		Graphics.Blit(currentSource, dest);
		RenderTexture.ReleaseTemporary(currentSource);
	}
}
