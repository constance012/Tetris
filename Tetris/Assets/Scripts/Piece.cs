using CSTGames.Utilities;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Piece : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] protected Board gameBoard;

	[Header("Settings"), Space]
	[SerializeField] private float dropDelay;
	[SerializeField] private float lockDelay;
	[SerializeField] private bool dropWhenRotate;

	// Properties.
	public TetrominoData CurrentTetromino { get; protected set; }
	public Vector3Int CurrentPosition { get; protected set; }
	public Vector3Int[] CurrentShapeCells { get; protected set; }

	// Private fields.
	private int _rotationIndex;
	private float _dropDelay;
	private float _lockDelay;

	public bool Initialize(TetrominoData data, Vector3Int position)
	{
		this.CurrentTetromino = data;
		this.CurrentPosition = position;
		this._rotationIndex = 0;
		this._dropDelay = dropDelay;
		this._lockDelay = lockDelay;

		int cells = data.Cells.Length;
		if (this.CurrentShapeCells == null || this.CurrentShapeCells.Length != cells)
		{
			this.CurrentShapeCells = new Vector3Int[cells];
		}

		for (int i = 0; i < cells; i++)
		{
			this.CurrentShapeCells[i] = (Vector3Int)data.Cells[i];
		}

		return gameBoard.IsValidPosition(this, position);
	}

	private void Update()
	{
		gameBoard.ClearPiece(this);

		_lockDelay -= Time.deltaTime;

		_dropDelay -= Time.deltaTime;
		if (_dropDelay < 0f)
		{
			Move(Vector3Int.down);
			_dropDelay = dropDelay;
		}

		if (Input.GetKeyDown(KeyCode.A))
			Move(Vector3Int.left);
		if (Input.GetKeyDown(KeyCode.D))
			Move(Vector3Int.right);
		if (Input.GetKeyDown(KeyCode.S))
			Move(Vector3Int.down);
		if (Input.GetKeyDown(KeyCode.Space))
			HardDrop();

		if (Input.GetKeyDown(KeyCode.Q))
			Rotate(-1);
		if (Input.GetKeyDown(KeyCode.E))
			Rotate(1);

		gameBoard.RenderPiece(this);
	}

	private void HardDrop()
	{
		while (Move(Vector3Int.down))
			continue;

		LockIntoPlace();
	}

	private void LockIntoPlace()
	{
		gameBoard.RenderPiece(this);
		gameBoard.CheckForCompletedRows();
		gameBoard.SpawnNewPiece();
	}

	private bool Move(Vector3Int delta)
	{
		Vector3Int newPos = CurrentPosition;
		newPos += delta;

		if (gameBoard.IsValidPosition(this, newPos))
		{
			CurrentPosition = newPos;
			_lockDelay = lockDelay;

			return true;
		}
		else if (delta == Vector3Int.down && _lockDelay < 0f)
			LockIntoPlace();
		
		return false;
	}

	private void Rotate(int direction)
	{
		int previousRotationIndex = _rotationIndex;
		_rotationIndex = NumberUtils.Wrap(_rotationIndex + direction, 4);

		ApplyRotationMatrix(direction);

		// If can not apply any of those wallKicks, then reverse the rotation.
		if (!CheckWallKicks(_rotationIndex, direction))
		{
			_rotationIndex = previousRotationIndex;
			ApplyRotationMatrix(-direction);
		}
		else if (dropWhenRotate)
		{
			Move(Vector3Int.down);
		}
	}

	private void ApplyRotationMatrix(int direction)
	{
		for (int i = 0; i < CurrentShapeCells.Length; i++)
		{
			Vector3 cell = CurrentShapeCells[i];
			int x, y;

			switch (CurrentTetromino.shape)
			{
				// The I and O rotate differently from the others.
				case TetrominoShape.I:
				case TetrominoShape.O:
					cell.x -= .5f;
					cell.y -= .5f;

					x = Mathf.CeilToInt((cell.x * GameData.RotationMatrix[0] * direction) +
										 (cell.y * GameData.RotationMatrix[1] * direction));
					y = Mathf.CeilToInt((cell.x * GameData.RotationMatrix[2] * direction) +
										 (cell.y * GameData.RotationMatrix[3] * direction));
					break;

				default:
					x = Mathf.RoundToInt((cell.x * GameData.RotationMatrix[0] * direction) +
										 (cell.y * GameData.RotationMatrix[1] * direction));
					y = Mathf.RoundToInt((cell.x * GameData.RotationMatrix[2] * direction) +
										 (cell.y * GameData.RotationMatrix[3] * direction));
					break;
			}

			CurrentShapeCells[i] = new Vector3Int(x, y, 0);
		}
	}

	private bool CheckWallKicks(int rotationIndex, int direction)
	{
		int wallKickIndex = GetWallKickIndex(rotationIndex, direction);

		// Loop through each test.
		for (int i = 0; i < CurrentTetromino.WallKicks.GetLength(1); i++)
		{
			Vector2Int translation = CurrentTetromino.WallKicks[wallKickIndex, i];

			if (Move((Vector3Int)translation))
				return true;
		}

		return false;
	}

	private int GetWallKickIndex(int rotationIndex, int direction)
	{
		int wallKickIndex = (direction < 0) ? rotationIndex * 2 - 1 : rotationIndex * 2;
		return NumberUtils.Wrap(wallKickIndex, CurrentTetromino.WallKicks.GetLength(0));
	}
}