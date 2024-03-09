using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityRandom = UnityEngine.Random;

public class Board : MonoBehaviour
{
	[Header("Tetrominoes Data"), Space]
	public TetrominoData[] tetrominoes;
	[SerializeField] private Vector3Int spawnPosition;
	[SerializeField] private Vector2Int boardSize = new Vector2Int(10, 20);

	[Header("Currently Active Piece"), Space]
	[SerializeField] private Piece activePiece;

	// Properties.
	public Tilemap Tilemap { get; private set; }
	public RectInt Bounds
	{
		get
		{
			Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
			return new RectInt(position, boardSize);
		}
	}

	private void Awake()
	{
		Tilemap = this.GetComponentInChildren<Tilemap>("Tetrominoes");
		Array.ForEach(tetrominoes, tetromino => tetromino.InitializeCells());
	}

	private void Start()
	{
		SpawnNewPiece();
	}

	#region Tetris Piece Management.
	public void SpawnNewPiece()
	{
		int randomIndex = UnityRandom.Range(0, tetrominoes.Length);
		TetrominoData data = this.tetrominoes[randomIndex];

		if (activePiece.Initialize(data, spawnPosition))
			RenderPiece(activePiece);
		else
			GameOver();
	}

	public void RenderPiece(Piece piece, Tilemap tilemap = null)
	{
		tilemap ??= this.Tilemap;

		for (int i = 0; i < piece.CurrentShapeCells.Length; i++)
		{
			Vector3Int tilePos = piece.CurrentShapeCells[i] + piece.CurrentPosition;
			tilemap.SetTile(tilePos, piece.CurrentTetromino.tile);
		}
	}

	public void ClearPiece(Piece piece, Tilemap tilemap = null)
	{
		tilemap ??= this.Tilemap;

		for (int i = 0; i < piece.CurrentShapeCells.Length; i++)
		{
			Vector3Int tilePos = piece.CurrentShapeCells[i] + piece.CurrentPosition;
			tilemap.SetTile(tilePos, null);
		}
	}

	public bool IsValidPosition(Piece piece, Vector3Int position)
	{
		RectInt bounds = this.Bounds;

		for (int i = 0; i < piece.CurrentShapeCells.Length; i++)
		{
			Vector3Int tilePos = piece.CurrentShapeCells[i] + position;

			// If this position is already occupied by other tiles or it's out of bounds.
			if (Tilemap.HasTile(tilePos) || !bounds.Contains((Vector2Int)tilePos))
				return false;
		}

		return true;
	}
	#endregion

	#region Completed Rows Management.
	public void CheckForCompletedRows()
	{
		RectInt bounds = this.Bounds;
		int row = bounds.yMin;

		while (row < bounds.yMax)
		{
			if (IsRowCompleted(row))
				ClearRow(row);
			else
				row++;
		}
	}

	private bool IsRowCompleted(int row)
	{
		RectInt bounds = this.Bounds;

		for (int x = bounds.xMin; x < bounds.xMax; x++)
		{
			Vector3Int tilePos = new Vector3Int(x, row, 0);
			if (!Tilemap.HasTile(tilePos))
				return false;
		}

		return true;
	}

	private void ClearRow(int row)
	{
		RectInt bounds = this.Bounds;

		// Clear the row.
		for (int x = bounds.xMin; x < bounds.xMax; x++)
		{
			Vector3Int tilePos = new Vector3Int(x, row, 0);
			Tilemap.SetTile(tilePos, null);
		}

		// Shift all above rows down.
		while (row < bounds.yMax)
		{
			for (int x = bounds.xMin; x < bounds.xMax; x++)
			{
				Vector3Int tilePos = new Vector3Int(x, row + 1, 0);
				TileBase aboveTile = Tilemap.GetTile(tilePos);

				tilePos.y--;
				Tilemap.SetTile(tilePos, aboveTile);
			}

			row++;
		}
	}
	#endregion

	private void GameOver()
	{
		Tilemap.ClearAllTiles();

		Debug.LogWarning("Game Over!!");
		// Extends further here...
	}
}