using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostEffect : MonoBehaviour {

	public Material monoTone;

	public void Start()
	{
	}
	// カメラによるレンダリングが完了した際に呼ばれる
	// レンダリング結果
	// 第二引数に書き込むことで、レンダリング結果を編集することができる
	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		// 第一引数のテクスチャを、第二引数のテクスチャに、第三引数のマテリアルを用いてコピーします。
		Graphics.Blit(src, dest, monoTone);
	}
}
