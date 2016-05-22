using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static Dictionary<int, Func<int, int, Room>> _roomsByTypeFactories = CreateRoomFactories();
    static Room[,] _map;
    static Indy _indy = new Indy();
    
    static void Main(string[] args)
    {
        ReadMapInfo();

        // game loop
        while ( true ) {
            ReadIndyInfo();
            
            Room currentRoom    = GetRoomWhereIndyNow();
            Room nextRoom       = GetNextRoomIfEntersAt( currentRoom, _indy.Pos );

            // One line containing the X Y coordinates of the room in which you believe Indy will be on the next turn.
            Console.WriteLine( "{0} {1}", nextRoom.X, nextRoom.Y );
        }
    }
    
    static Room GetRoomWhereIndyNow()
    {
        return _map[ _indy.X, _indy.Y ];
    }
    static Room GetNextRoomIfEntersAt(Room room, Position entrance)
    {
        Position? exitPosition = room.GetExitPositionOf( entrance );
        if ( null == exitPosition )
            throw new Exception( string.Format( "Failed to find exit for the {0} entrance.", entrance ) );
        
        int x = room.X,
            y = room.Y;
        switch ( exitPosition.Value ) {
            case Position.Left:
                x -= 1;
                break;
            case Position.Right:
                x += 1;
                break;
            case Position.Bottom:
                y += 1;
                break;
            default:
                throw new Exception( "Indy can't exit via top because of the gravity." );
        }
        
        return _map[ x, y ];
    }
    static Dictionary<int, Func<int, int, Room>> CreateRoomFactories()
    {
        return new Dictionary<int, Func<int, int, Room>> {
            { 0, (x,y) => new Room( x, y ) },
            // type 1 paths: top -> bottom, left -> bottom, right -> bottom.
            { 1, (x,y) => new Room( 
                    x, y,
                    new Path( entrance: Position.Top, exit: Position.Bottom ),
                    new Path( entrance: Position.Left, exit: Position.Bottom ),
                    new Path( entrance: Position.Right, exit: Position.Bottom ) ) },
            { 2, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Left, exit: Position.Right ),
                    new Path( entrance: Position.Right, exit: Position.Left ) ) },
            { 3, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Top, exit: Position.Bottom ) ) },
            { 4, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Top, exit: Position.Left ),
                    new Path( entrance: Position.Right, exit: Position.Bottom ) ) },
            { 5, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Top, exit: Position.Right ),
                    new Path( entrance: Position.Left, exit: Position.Bottom ) ) },
            { 6, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Left, exit: Position.Right ),
                    new Path( entrance: Position.Right, exit: Position.Left ) ) },
            { 7, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Top, exit: Position.Bottom ),
                    new Path( entrance: Position.Right, exit: Position.Bottom ) ) },
            { 8, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Left, exit: Position.Bottom ),
                    new Path( entrance: Position.Right, exit: Position.Bottom ) ) },
            { 9, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Top, exit: Position.Bottom ),
                    new Path( entrance: Position.Left, exit: Position.Bottom ) ) },
            { 10, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Top, exit: Position.Left ) ) },
            { 11, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Top, exit: Position.Right ) ) },
            { 12, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Right, exit: Position.Bottom ) ) },
            { 13, (x,y) => new Room(
                    x, y,
                    new Path( entrance: Position.Left, exit: Position.Bottom ) ) },
        };
    }
    static void ReadMapInfo()
    {
        // read map info.
        string[] inputs; inputs = Console.ReadLine().Split( ' ' );
        int w = int.Parse( inputs[ 0 ] ); // number of columns.
        int h = int.Parse( inputs[ 1 ] ); // number of rows.
        // initialize the map.
        _map = new Room[ w, h ];
        // fill the map.
        int roomType;
        string[] rooms;
        for ( int y = 0; y < h; y++ ) {
            // represents a line in the grid and contains W integers. Each integer represents one room of a given type.
            string line = Console.ReadLine();
            rooms = line.Split( new [] { ' ' } );
            for ( int x = 0; x < w; x++ ) {
                // parse an integer that indicates root type.
                roomType = int.Parse( rooms[ x ] );
                // create room by it's type.
                _map[ x, y ] = _roomsByTypeFactories[ roomType ]( x, y );
            }
            Console.Error.WriteLine( line );
        }
        int exit = int.Parse( Console.ReadLine() ); // the coordinate along the X axis of the exit (not useful for this first mission, but must be read).
    }
    static void ReadIndyInfo()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int x = int.Parse( inputs[ 0 ] );
        int y = int.Parse( inputs[ 1 ] );
        Position pos;
        switch ( inputs[ 2 ] ) {
            case "TOP":
                pos = Position.Top; break;
            case "LEFT":
                pos = Position.Left; break;
            case "RIGHT":
                pos = Position.Right; break;
            case "BOTTOM":
                pos = Position.Bottom; break;
            default:
                throw new NotSupportedException( string.Format( "Unknown position: \"{0}\"", inputs[ 2 ] ) );
        }
        
        _indy.X = x;
        _indy.Y = y;
        _indy.Pos = pos;
    }
}

class Room
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Path[] Pathways { get; private set; }
    
    public Room(int x, int y)
        : this( x, y, pathways: null )
    {
    }
    public Room(int x, int y, params Path[] pathways)
    {
        this.X          = x;
        this.Y          = y;
        this.Pathways   = pathways ?? new Path[ 0 ];
    }
    
    public Position? GetExitPositionOf(Position entrance)
    {
        if ( 0 == this.Pathways.Length )
            return null;
            
        for ( int i = 0; i < this.Pathways.Length; i++ ) {
            if ( this.Pathways[ i ].Entrance == entrance )
                return this.Pathways[ i ].Exit;
        }
        // no exits found.
        return null;
    }
}

class Path
{
    public Position Entrance { get; private set; }
    public Position Exit { get; private set; }
    
    public Path(Position entrance, Position exit)
    {
        this.Entrance   = entrance;
        this.Exit       = exit;
    }
}

class Indy
{
    public int X { get; set; }
    public int Y { get; set; }
    public Position Pos { get; set; }
}

enum Position
{
    Top,
    Left,
    Right,
    Bottom,
}