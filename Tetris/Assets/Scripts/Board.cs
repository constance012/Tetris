using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityRandom = UnityEngine.Random;

public class Board : MonoBehaviour
{
	[Header("Tetrominoes Data")]
	public TetrominoData[] tetrominoes;

	// Properties.
	public Tilemap Tilemap { get; private set; }

	private void Awake()
	{
		this.Tilemap = this.GetComponentInChildren<Tilemap>("Tetrominoes");
		Array.ForEach(tetrominoes, tetromino => tetromino.InitializePoints());
	}

	private void Start()
	{
		SpawnPiece()
	}

	public void SpawnPiece()
	{
		int randomIndex = UnityRandom.Range(0, tetrominoes.Length);
	}
}