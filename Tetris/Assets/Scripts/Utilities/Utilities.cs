namespace CSTGames.Utilities
{
	public static class NumberUtils
	{
		public static int Wrap(int x, int m)
		{
			return (x % m + m) % m;
		}
	}
}
