using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TetrominoShape { I, J, L, O, S, T, Z }

[Serializable]
public struct TetrominoData
{
	public TetrominoShape shape;
	public Tile tile;

	[Tooltip("An array of points represent the shape of this tetromino on the grid.")]
	public Vector2Int[] Points { get; private set; }

	public void InitializePoints()
	{
		this.Points = GameData.Points[this.shape];
	}
}