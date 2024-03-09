using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TetrominoShape { I, J, L, O, S, T, Z }

[Serializable]
public class TetrominoData
{
	public TetrominoShape shape;
	public Tile tile;

	[Tooltip("An array of relative cell positions represent the shape of this tetromino on the grid.")]
	public Vector2Int[] Cells { get; private set; }
	public Vector2Int[,] WallKicks { get; private set; }

	/// <summary>
	/// Instantiate a blank Tetris Piece, useful for ghost pieces.
	/// </summary>
	/// <param name="shape"></param>
	/// <param name="tile"></param>
	public TetrominoData(TetrominoShape shape, Tile tile)
	{
		this.shape = shape;
		this.tile = tile;
		Cells = null;
		WallKicks = null;
	}

	public void InitializeCells()
	{
		this.Cells = GameData.ShapeCells[this.shape];
		this.WallKicks = GameData.WallKicks[this.shape];
	}
}