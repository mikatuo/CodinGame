using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

class Solution
{
	static void Main(string[] args)
	{
		int L = int.Parse( Console.ReadLine() );
		int H = int.Parse( Console.ReadLine() );
		string T = Console.ReadLine();
		var alphabet = new List<string>();
		for ( int i = 0; i < H; i++ ) {
			alphabet.Add( Console.ReadLine() );
		}
		// initialize the converter.
		var converter = new AsciiArtConverter( L, H, alphabet );
		// convert the text T into ASCII art.
		foreach ( string row in converter.Convert( T ) )
			Console.WriteLine( row );
		Console.ReadLine();
	}
}

/// <summary>
/// Converter that can convert text into ASCII art.
/// </summary>
class AsciiArtConverter
{
	#region Local Variables
	/// <summary>
	/// Registered letters in ASCII art.
	/// </summary>
	readonly Dictionary<char, string[]> _letters = new Dictionary<char, string[]>();
	/// <summary>
	/// ASCII art for an unknown letter.
	/// </summary>
	readonly string[] _unknownLetter;
	/// <summary>
	/// Height of a letter.
	/// </summary>
	readonly int _letterHeight;
	#endregion

	#region Constructors
	/// <summary>
	/// Initializes the converter.
	/// </summary>
	/// <param name="letterWidth">Width of a letter.</param>
	/// <param name="letterHeight">Height of a letter.</param>
	/// <param name="alphabet">Alphabet to parse letters from.</param>
	public AsciiArtConverter(int letterWidth, int letterHeight,
							IList<string> alphabet)
    {
		// remember height of a letter.
		_letterHeight = letterHeight;
		// get codes of A and Z characters.
        var a = System.Convert.ToInt32( 'A' );
		var z = System.Convert.ToInt32( 'Z' );
		// determine variable that will store offset of a letter in the alphabet.
		var offset = 0;
		// iterate through all letters from A to Z. Parse them from the alphebet and register.
		for ( int l = a; l <= z; l++ ) {
			// register a new letter.
			_letters.Add(
						// the letter.
				key:	(char)l,
						// slice the letter from the alphabet.
				value:	alphabet.Slice( start: offset, length: letterWidth, height: letterHeight )
				);
			// adjust offset of the next letter.
			offset += letterWidth;
		}
		// after the alphabet there is a "?" character. It should be displayed 
		// if a letter that's being converted is not registered.
		_unknownLetter = alphabet.Slice( start: offset, length: letterWidth, height: letterHeight );
    }
	#endregion

	#region Private Methods
	/// <summary>
	/// Creates a buffer of multiple string builders.
	/// </summary>
	/// <param name="length">Length of the array.</param>
	/// <returns>Buffer.</returns>
	StringBuilder[] CreateBuffer(int length)
	{
		var res = new StringBuilder[ length ];
		for ( int i = 0; i < length; i++ )
			res[ i ] = new StringBuilder();
		return	res;
	}
	#endregion

	/// <summary>
	/// Converts the text into ASCII art.
	/// </summary>
	/// <param name="text">The text to convert.</param>
	/// <returns>Converted text.</returns>
	public string[] Convert(string text)
	{
		if ( String.IsNullOrWhiteSpace( text ) )
			return	new string[ 0 ];
		// determine the variable that will store the result.
		StringBuilder[] result = CreateBuffer( length: _letterHeight );
		// convert each letter of the text.
		foreach ( char letter in text.ToUpper() ) {
			// check if the letter is registered.
			if ( !_letters.ContainsKey( letter ) ) {
				// the letter is unknown.
				result.Append( letter: _unknownLetter );
				continue;
			}
			// the letter is registered.
			result.Append( letter: _letters[ letter ] );
		}
		// return the result.
		return	result.Select( row => row.ToString() ).ToArray();
	}
}

/// <summary>
/// Extensions.
/// </summary>
static class MyExtensions
{
	/// <summary>
	/// Slices a portion of the <paramref name="collection"/>.
	/// </summary>
	/// <param name="collection">The collection to slice.</param>
	/// <param name="start">Start index.</param>
	/// <param name="length">Length of the portion to slice.</param>
	/// <param name="height">Height of the portion to slice.</param>
	/// <returns>Sliced portion of the <paramref name="collection"/>.</returns>
	public static string[] Slice(this IList<string> collection, int start, int length, int height)
	{
		var res = new string[ height ];
		// iterate through all rows and slice each of them.
		for ( int i = 0; i < height; i++ )
			res[ i ] = collection[ i ].Substring( startIndex: start, length: length );
		return	res;
	}
	/// <summary>
	/// Appends a letter to the buffer.
	/// </summary>
	/// <param name="buffer">The buffer to append the letter to.</param>
	/// <param name="letter">The letter to append.</param>
	/// <exception cref="ArgumentException">Unable to append the letter to the buffer. Height of the letter mismatches height of the buffer.</exception>
	public static void Append(this StringBuilder[] buffer, string[] letter)
	{
		if ( buffer.Length != letter.Length )
			throw new ArgumentException(
				message:	"Unable to append the letter to the buffer. " +
							"Height of the letter mismatches height of the buffer.",
				paramName:	"letter"
				);
		// append the letter.
		for ( int i = 0; i < letter.Length; i++ )
			buffer[ i ].Append( letter[ i ] );
	}
}