using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPiece : Piece
{
	[Header("References"), Space]
	[SerializeField] private Tile ghostTile;
	[SerializeField] private Piece trackingPiece;

	public Tilemap GhostTilemap { get; private set; }

	private void Awake()
	{
		GhostTilemap = this.GetComponentInChildren<Tilemap>("Ghost Overlay");
		CurrentTetromino = new TetrominoData(TetrominoShape.O, ghostTile);
		CurrentShapeCells = new Vector3Int[4];
	}

	private void Update() { }

	private void LateUpdate()
	{
		gameBoard.ClearPiece(this, GhostTilemap);
		Copy();
		CreateOverlay();
		gameBoard.RenderPiece(this, GhostTilemap);
	}

	private void Copy()
	{
		Array.Copy(trackingPiece.CurrentShapeCells, this.CurrentShapeCells, this.CurrentShapeCells.Length);
	}

	private void CreateOverlay()
	{
		Vector3Int position = trackingPiece.CurrentPosition;

		int trackingRow = position.y;
		int bottomRow = gameBoard.Bounds.yMin;

		// Clear the tracking piece first, so the its position won't be occupied by itself.
		gameBoard.ClearPiece(trackingPiece);

		for (int y = trackingRow; y >= bottomRow; y--)
		{
			position.y = y;

			if (gameBoard.IsValidPosition(trackingPiece, position))
			{
				CurrentPosition = position;
				continue;
			}

			break;
		}

		gameBoard.RenderPiece(trackingPiece);
	}
}
