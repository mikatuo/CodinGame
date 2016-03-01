using System;
using System.Collections.Generic;

class Solution
{
	static void Main(string[] args)
	{
		int N = int.Parse( Console.ReadLine() );
		var horses = new HorseStrengthsList();
		for ( int i = 0; i < N; i++ ) {
			horses.Add( int.Parse( Console.ReadLine() ) );
		}

		Console.WriteLine( horses.GetDifferenceBetweenTwoClosestStrengths() );
	}
}

/// <summary>
/// Simple list of horses strength.
/// </summary>
class HorseStrengthsList
{
	/// <summary>
	/// Horses.
	/// </summary>
	readonly List<int> _horses;

	#region Constructors
	/// <summary>
	/// Initializes the list.
	/// </summary>
	public HorseStrengthsList()
	{
		_horses = new List<int>();
	}
	#endregion

	/// <summary>
	/// Adds a new horse to the list.
	/// </summary>
	/// <param name="horse">Strength of the horse.</param>
	public void Add(int horse)
	{
		_horses.Add( horse );
	}
	/// <summary>
	/// Gets the difference between the two closest strengths.
	/// </summary>
	/// <returns>The two closest strengths.</returns>
	public int GetDifferenceBetweenTwoClosestStrengths()
	{
		_horses.Sort();
		// iterate through sorted strength and find the closest two.
		int j, dif, minDif = int.MaxValue;
		for ( int i = j = 0; i < _horses.Count - 1; i++ ) {
			// compare two values.
			if ( minDif > (dif = Math.Abs( _horses[ i ] - _horses[ ++j ] )) )
				// found closer ones.
				minDif = dif;
		}
		return minDif;
	}
}