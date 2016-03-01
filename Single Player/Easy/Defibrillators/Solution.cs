using System;
using System.Globalization;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Solution
{
	/// <summary>
	/// Entry point of the program.
	/// </summary>
	/// <param name="args">Command line arguments.</param>
	static void Main(string[] args)
	{
		string lon = Console.ReadLine();
		string lat = Console.ReadLine();
		// create defibrillator seaker.
		var seeker = new DefibrillatorSeeker();
		// read info about defibrillators.
		int n = int.Parse( Console.ReadLine() );
		for ( int i = 0; i < n; i++ ) {
			string defib = Console.ReadLine();
			// add available defibrillators.
			seeker.Add( defib );
		}

		Console.WriteLine( seeker.FindClosestTo( longitude: lon, latitude: lat ).Name );
	}
}

/// <summary>
/// Seeks for defibrillators.
/// </summary>
class DefibrillatorSeeker
{
	/// <summary>
	/// Available defibrillators.
	/// </summary>
	readonly List<Defibrillator> _defibrillators = new List<Defibrillator>();

	#region Private Methods
	/// <summary>
	/// Calculate the distance between the <paramref name="defib"/> and 
	/// <paramref name="longitude"/> with <paramref name="latitude"/>.
	/// </summary>
	/// <param name="defib">The defibrillator.</param>
	/// <param name="longitude">The longitude.</param>
	/// <param name="latitude">The latitude.</param>
	static double CalculateDistance(Defibrillator defib, double longitude, double latitude)
	{
		// determine variables to calculate the distance.
		double x, y;
		x = (longitude - defib.Longitude)
		    * Math.Cos( (latitude + defib.Latitude) / 2 );
		y = (latitude - defib.Latitude);
		return	Math.Sqrt( x * x + y * y ) * 6371;
	}
	#endregion

	/// <summary>
	/// Adds a new defibrillator to the base.
	/// </summary>
	/// <remarks>
	/// This data (<paramref name="defibrillator"/>) is comprised of lines, each of which 
	/// represents a defibrillator. Each defibrillator is represented by the following fields:
	///		- A number identifying the defibrillator
	///		- Name
	///		- Adress
	///		- Contact Phone number
	///		- Longitude (degrees)
	///		- Latitude (degrees)
	///	These fields are separated by a semicolon ;
	/// </remarks>
	/// <param name="defibrillator">Defibrillator data.</param>
	public void Add(string defibrillator)
	{
		_defibrillators.Add( new Defibrillator( defibrillator ) );
	}
	/// <summary>
	/// Seeks for the closest defibrillator.
	/// </summary>
	/// <param name="longitude">Latitude of the place to find the closest defibrillator to.</param>
	/// <param name="latitude">Longitute of the place to find the closest defibrillator to.</param>
	/// <returns>Defibrillator.</returns>
	public Defibrillator FindClosestTo(string longitude, string latitude)
	{
		double longitudeNum	= longitude.ParseCoordinate();
		double latitudeNum	= latitude.ParseCoordinate();
		Defibrillator curr;
		// determine variables that will store temp data.
		double distance;
		// determine variables that will store the closest defibrilator 
		// and the distance to it.
		double closestDistance		= double.MaxValue;
		Defibrillator closestDefib	= null;
		// iterate through all defibrillators and seek for the closest one.
		for ( int i = 0; i < _defibrillators.Count; i++ ) {
			curr = _defibrillators[ i ];
			// calculate the distance between two points.
			distance = CalculateDistance(
				defib:		curr,
				longitude:	longitudeNum,
				latitude:	latitudeNum
				);
			// check if the current defibrillator is closer.
			if ( distance < closestDistance ) {
				// update the closest defibrillator variable and distance.
				closestDistance = distance;
				closestDefib = curr;
			}
		}
		// done.
		return	closestDefib;
	}
}

/// <summary>
/// A defibrillator.
/// </summary>
class Defibrillator
{
	/// <summary>
	/// A number identifying the defibrillator.
	/// </summary>
	public int ID
	{
		get;
		private set;
	}
	/// <summary>
	/// Name.
	/// </summary>
	public string Name
	{
		get;
		private set;
	}
	/// <summary>
	/// Address.
	/// </summary>
	public string Address
	{
		get;
		private set;
	}
	/// <summary>
	/// Contact Phone number.
	/// </summary>
	public string ContactPhone
	{
		get;
		private set;
	}
	/// <summary>
	/// Longitude (degrees).
	/// </summary>
	public double Longitude
	{
		get;
		private set;
	}
	/// <summary>
	/// Latitude (degrees).
	/// </summary>
	public double Latitude
	{
		get;
		private set;
	}

	/// <summary>
	/// Initializes the defibrillator.
	/// </summary>
	/// <param name="defibrillator"></param>
	public Defibrillator(string defibrillator)
	{
		string[] tokens = defibrillator.Split( new [] { ';' } );
		// parse tokens.
		this.ID				= int.Parse( tokens[ 0 ], CultureInfo.InvariantCulture );
		this.Name			= tokens[ 1 ];
		this.Address		= tokens[ 2 ];
		this.ContactPhone	= tokens[ 3 ];
		this.Longitude		= tokens[ 4 ].ParseCoordinate();
		this.Latitude		= tokens[ 5 ].ParseCoordinate();
	}
}

/// <summary>
/// Extensions.
/// </summary>
static class MyExtensions
{
	/// <summary>
	/// Parses longitude or latitude.
	/// </summary>
	/// <param name="value">The value to parse.</param>
	/// <returns>Parsed.</returns>
	public static double ParseCoordinate(this string value)
	{
		return	double.Parse( value.Replace( ",", "." ), CultureInfo.InvariantCulture );
	}
}