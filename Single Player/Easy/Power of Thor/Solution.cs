using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static void Main(string[] args)
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int lightX = int.Parse(inputs[0]); // the X position of the light of power
        int lightY = int.Parse(inputs[1]); // the Y position of the light of power
        
        var thor = new Thor( x: inputs[2], y: inputs[3] );

        // game loop
        while (true)
        {
            int remainingTurns = int.Parse(Console.ReadLine());

            thor.Move( x: lightX, y: lightY );
        }
    }
}

class Thor
{
    #region Local Variables & Properties
    public int X
    {
        get;
        private set;
    }
    public int Y
    {
        get;
        private set;
    }
    #endregion
    
    #region Constructors
    public Thor(string x, string y)
    {
        this.X = int.Parse( x );
        this.Y = int.Parse( y );
    }
    #endregion
    
    #region Private Methods
    string MoveX(int x)
    {
        if ( x == this.X )
            return null;
        if ( x > this.X ) {
            ++this.X;
            return "E";
        }
        --this.X;
        return "W";
    }
    string MoveY(int y)
    {
        if ( y == this.Y )
            return null;
        if ( y > this.Y ) {
            ++this.Y;
            return "S";
        }
        --this.Y;
        return "N";
    }
    #endregion
    
    public void Move(int x, int y)
    {
        Console.WriteLine( string.Format( "{0}{1}", MoveY( y ), MoveX( x ) ) );
    }
}