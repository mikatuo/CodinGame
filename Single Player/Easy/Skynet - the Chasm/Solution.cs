using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static void Main(string[] args)
    {
        int road = int.Parse(Console.ReadLine()); // the length of the road before the gap.
        int gap = int.Parse(Console.ReadLine()); // the length of the gap.
        int platform = int.Parse(Console.ReadLine()); // the length of the landing platform.
        
        int gapStartX = road;
        int gapEndX = gapStartX + gap;
        
        // game loop
        while (true)
        {
            int speed = int.Parse(Console.ReadLine()); // the motorbike's speed.
            int coordX = int.Parse(Console.ReadLine()); // the position on the road of the motorbike.
            
            // check if the bike is in front of the gap and is able 
            // to jump over it.
            if ( coordX <= gapStartX && (coordX + speed) >= gapEndX ) {
                Console.WriteLine("JUMP");
                continue;
            }
            
            // wait if the bike have the momentum to 
            // jump over the gap.
            if ( coordX <= gapStartX && (speed == gap + 1) ) {
                Console.WriteLine("WAIT");
                continue;
            }
            
            // the bike has passed the gap. Slow down.
            if ( coordX >= gapEndX || speed > gap ) {
                Console.WriteLine("SLOW");
                continue;
            }
            
            // gain speed.
            Console.WriteLine("SPEED");
        }
    }
}