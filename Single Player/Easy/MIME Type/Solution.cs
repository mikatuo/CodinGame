using System;
using System.IO;
using System.Collections.Generic;

class Solution
{
	/// <summary>
	/// Entry point of the program.
	/// </summary>
	/// <param name="args"></param>
	static void Main(string[] args)
	{
		int typesCount = int.Parse( Console.ReadLine() ); // Number of elements which make up the association table.
		int filesCount = int.Parse( Console.ReadLine() ); // Number of file names to be analyzed.
		// create the mapper.
		MimeMapper mimeMapper = CreateMapper( typesCount );
		// process all file names and output their mime types.
		for ( int i = 0; i < filesCount; i++ ) {
			string filename = Console.ReadLine(); // One file name per line.
			Console.WriteLine( mimeMapper.GetMimeType( filename ) );
		}
	}
	/// <summary>
	/// Creates the mime mapper.
	/// </summary>
	/// <param name="typesCount">Number of elements which make up the association table.</param>
	/// <returns>Mime types mapper.</returns>
	static MimeMapper CreateMapper(int typesCount)
	{
		var mapper = new MimeMapper();
		// read available maps.
		for ( int i = 0; i < typesCount; i++ ) {
			string[] inputs = Console.ReadLine().Split( ' ' );
			string ext	= inputs[ 0 ]; // file extension
			string mime	= inputs[ 1 ]; // MIME type.
			// register a map type.
			mapper.Register( ext, mime );
		}
		return	mapper;
	}
}

/// <summary>
/// Maps file extensions with their corresponding mime types.
/// </summary>
class MimeMapper
{
	/// <summary>
	/// The mime type is unknown.
	/// </summary>
	const string UnknownMimeType = "UNKNOWN";

	/// <summary>
	/// Currently registered maps.
	/// </summary>
	Dictionary<string, string> _map = new Dictionary<string, string>( StringComparer.InvariantCultureIgnoreCase );

	/// <summary>
	/// Registers a new mime type.
	/// </summary>
	/// <param name="extension">Extension without dot.</param>
	/// <param name="mimeType">Mime type associated with the extension.</param>
	public void Register(string extension, string mimeType)
	{
		if ( String.IsNullOrWhiteSpace( extension ) )
			throw new ArgumentNullException( paramName: "extension" );
		if ( String.IsNullOrWhiteSpace( mimeType ) )
			throw new ArgumentNullException( paramName: "mimeType" );
		// add "." to the extension if it's not yet there.
		if ( !extension.StartsWith( "." ) )
			extension = string.Format( ".{0}", extension );
		// register or update the mapping.
		if ( !_map.ContainsKey( extension ) )
			// register a new map.
			_map.Add( extension, mimeType );
		else
			// update the map.
			_map[ extension ] = mimeType;
	}
	/// <summary>
	/// Gets mime type of the file.
	/// </summary>
	/// <param name="filename"></param>
	/// <returns></returns>
	public string GetMimeType(string filename)
	{
		string mime;
		// check if there is a mime associated with such extension. Get it if there is one.
		if ( !_map.TryGetValue( key: Path.GetExtension( filename ) ?? String.Empty, value: out mime ) )
			// the mime type is unknown.
			return	UnknownMimeType;
		// found one.
		return	mime;
	}
}