using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    const string WaitAction = "WAIT";
    const string BlockAction = "BLOCK";
    
    static Level s_level;
    static Clone s_activeClone;
    static Func<bool>[] s_blockConditions;
    
    static void Main(string[] args)
    {
        ReadLevelInfo();
        s_blockConditions = new Func<bool>[] {
            WhenExitIsBehindOfClone,
            WhenElevatorIsBehindOfClone,
        };

        // game loop
        while (true) {
            ReadActiveCloneInfo();

            // check if any of block conditions returns true.
            if ( s_blockConditions.Any( condition => condition() == true ) ) {
                Console.WriteLine( BlockAction );
                continue;
            }

            Console.WriteLine( WaitAction );
        }
    }
    
    static bool WhenExitIsBehindOfClone()
    {
        if ( !s_activeClone.IsOnSameFloor( s_level.ExitAt.Floor ) )
            // exit is on different floor.
            return false;
        if ( s_activeClone.IsDirectedTo( s_level.ExitAt.Pos ) )
            // clone is heading to the exit.
            return false;
        // clone is heading to an opposite direction of exit.
        return true;
    }
    static bool WhenElevatorIsBehindOfClone()
    {
        Position? elevator = s_level.GetElevator( s_activeClone.Position.Floor );
        if ( null == elevator )
            // there is no elevator on this floor.
            return false;
        if ( s_activeClone.IsDirectedTo( elevator.Value.Pos ) )
            // clone is heading to the elevator.
            return false;
        // clone is heading to an opposite direction of elevator.
        return true;
    }
    
    static void ReadLevelInfo()
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int nbFloors = int.Parse(inputs[0]); // number of floors
        int width = int.Parse(inputs[1]); // width of the area
        int nbRounds = int.Parse(inputs[2]); // maximum number of rounds
        int exitFloor = int.Parse(inputs[3]); // floor on which the exit is found
        int exitPos = int.Parse(inputs[4]); // position of the exit on its floor
        int nbTotalClones = int.Parse(inputs[5]); // number of generated clones
        int nbAdditionalElevators = int.Parse(inputs[6]); // ignore (always zero)
        int nbElevators = int.Parse(inputs[7]); // number of elevators
        
        // read position of all elevators.
        var elevatorsAt = new List<Position>();
        for (int i = 0; i < nbElevators; i++) {
            inputs = Console.ReadLine().Split(' ');
            int elevatorFloor = int.Parse(inputs[0]); // floor on which this elevator is found
            int elevatorPos = int.Parse(inputs[1]); // position of the elevator on its floor
            elevatorsAt.Add( new Position( elevatorFloor, elevatorPos ) );
        }
        
        // initialize level.
        s_level = new Level {
            Width               = width,
            Floors              = nbFloors,
            Rounds              = nbRounds,
            TotalClones         = nbTotalClones,
            ExitAt              = new Position( exitFloor, exitPos ),
        };
        s_level.SetElevators( elevatorsAt );
        // initialize clone.
        s_activeClone = new Clone();
    }
    static void ReadActiveCloneInfo()
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int cloneFloor = int.Parse(inputs[0]); // floor of the leading clone
        int clonePos = int.Parse(inputs[1]); // position of the leading clone on its floor
        string direction = inputs[2]; // direction of the leading clone: LEFT or RIGHT
        
        s_activeClone.SetPosition( cloneFloor, clonePos );
        s_activeClone.SetDirection( direction );
    }
}

class Level
{
    readonly Dictionary<int, Position> _floorElevators = new Dictionary<int, Position>();
    
    public int Width { get; set; }
    public int Floors { get; set; }
    public int Rounds { get; set; }
    public int TotalClones { get; set; }
    public int Elevators { get { return this.ElevatorsAt.Count; } }
    public Position ExitAt { get; set; }
    public HashSet<Position> ElevatorsAt { get; private set; }
    
    public void SetElevators(IList<Position> elevators)
    {
        ElevatorsAt = new HashSet<Position>( elevators );
        // index on which floor there are elevators.
        _floorElevators.Clear();
        foreach (var el in elevators)
            _floorElevators.Add( el.Floor, el );
    }
    public Position? GetElevator(int floor)
    {
        if ( !_floorElevators.ContainsKey( floor ) )
            return null;
            
        return _floorElevators[ floor ];
    }
}

class Clone
{
    public Position Position { get; private set; }
    public Direction Direction { get; private set; }
    
    public Clone()
    {
        this.Position = new Position();
    }
    
    public void SetPosition(int floor, int position)
    {
        this.Position = new Position( floor, position );
    }
    public void SetDirection(string value)
    {
        this.Direction = value == "RIGHT"
            ? Direction.Right
            : Direction.Left;
    }
    public bool IsDirectedTo(int position)
    {
        if ( position <= this.Position.Pos && this.Direction == Direction.Left )
            return true;
        if ( position >= this.Position.Pos && this.Direction == Direction.Right )
            return true;
        // directed to other direction.
        return false;
    }
    public bool IsOnSameFloor(int floor)
    {
        return floor == this.Position.Floor;
    }
}

struct Position
    : IEquatable<Position>
{
    public int Floor { get; set; }
    public int Pos { get; set; }
    
    public Position(int floor, int position)
    {
        this.Floor  = floor;
        this.Pos    = position;
    }
    
    public bool Equals(Position other)
    {
        return  this.Floor == other.Floor &&
                this.Pos == other.Pos;
    }
    public override bool Equals(object obj)
    {
        if ( obj == null || GetType() != obj.GetType() ) {
            return false;
        }
        return Equals( (Position)obj );
    }
    public override int GetHashCode()
    {
        unchecked
        {
            var hashcode = this.Floor;
            hashcode = (hashcode * 397) ^ this.Pos;
            return  hashcode;
        }
    }
}

enum Direction
{
    Left,
    Right,
}