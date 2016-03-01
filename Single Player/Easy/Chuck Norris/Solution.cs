using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

class Solution
{
	static void Main(string[] args)
	{
		string message = Console.ReadLine();

		// initialize encoder.
		var encoder = new ChuckNorrisEncoder( inputEncoding: Encoding.ASCII );

		Console.WriteLine( encoder.Encode( message ) );
	}
}

/// <summary>
/// Encoder that converts text into unary message.
/// </summary>
class ChuckNorrisEncoder
{
	#region Local Variables
	/// <summary>
	/// Format for each type of encoded block.
	/// </summary>
	readonly Dictionary<char, string> _encodeBlockFormat = new Dictionary<char, string> {
		{ '0', "00 {0}" },
		{ '1', "0 {0}" }
	};
	/// <summary>
	/// A character that separates encoded blocks.
	/// </summary>
	readonly char _separator = ' ';
	/// <summary>
	/// Encoding of the input text.
	/// </summary>
	readonly Encoding _inputEncoding;
	#endregion

	/// <summary>
	/// Initializes the encoder.
	/// </summary>
	/// <param name="inputEncoding">Encoding of the input text.</param>
	public ChuckNorrisEncoder(Encoding inputEncoding)
	{
		_inputEncoding = inputEncoding;
	}

	#region Private Methods
	/// <summary>
	/// Encodes a character.
	/// </summary>
	/// <remarks>
	/// Here is the encoding principle:
	///		- The input message consists of ASCII characters (7-bit)
	///		- The encoded output message consists of blocks of 0
	///		- A block is separated from another block by a space
	///		- Two consecutive blocks are used to produce a series of same value bits (only 1s or 0s):
	///		- First block: it is always 0 or 00. If it is 0, then the series contains 1s, if not, it contains 0s
	///		- Second block: the number of 0s in this block is the number of bits in the series
	/// </remarks>
	/// <param name="source">Source to read the text to encode from.</param>
	/// <param name="destination">Write the encoded character into.</param>
	void Encode(StringBuilder source, StringBuilder destination)
	{
		if ( null == source || 0 == source.Length )
			return;
		// determine variables that will store current block and current index.
		char currBlock = source[ 0 ]; int i = 0;
		// iterate through the string.
		while ( source.Length != 0 && i < source.Length - 1 ) {
			// check if the next character is the same.
			if ( source[ ++i ] == currBlock )
				// search end of the block.
				continue;
			// found the end of the block. Encode it.
			EncodeBlock(
				block:			currBlock,
				length:			i,
				destination:	destination
				);
			// separate blocks with space.
			destination.Append( _separator );
			// remove processed block. Reset the counter.
			source.Remove( 0, length: i ); i = 0;
			// update current block.
			currBlock = source[ i ];
		}
		// process the last block.
		EncodeBlock(
				block:			currBlock,
				length:			i + 1,
				destination:	destination
				);
	}
	/// <summary>
	/// Encodes a block.
	/// </summary>
	/// <param name="block">The block to encode.</param>
	/// <param name="length">Length of the block.</param>
	/// <param name="destination">Write the encoded block into.</param>
	void EncodeBlock(char block, int length, StringBuilder destination)
	{
		destination.Append(
			string.Format(
				_encodeBlockFormat[ block ],		// format
				new String( '0', count: length )	// {0}
				)
			);
	}
	/// <summary>
	/// Converts a code into a seven bit binary string.
	/// </summary>
	/// <param name="code">The code to convert.</param>
	/// <returns>Binary.</returns>
	string ConvertToSevenBitBinaryString(byte code)
	{
		return	Convert.ToString( code, toBase: 2 )
					.PadLeft( totalWidth: 7, paddingChar: '0' );
	}
	#endregion

	/// <summary>
	/// Converts a text into unary message.
	/// </summary>
	/// <param name="text">The text to convert.</param>
	/// <returns>Encoded text.</returns>
	public string Encode(string text)
	{
		var result = new StringBuilder();
		// convert all characters of the text into their char codes.
		byte[] charCodes = _inputEncoding.GetBytes( text );
		// convert char codes in binary format and join them together.
		var binaryText = new StringBuilder(
								String.Join(
									separator:	"",
									values:		charCodes.Select( ConvertToSevenBitBinaryString ) ) );
		// iterate through each char code and encode it.
		Encode(
			source:			binaryText, 
			destination:	result
			);
		// done.
		return	result.ToString();
	}
}