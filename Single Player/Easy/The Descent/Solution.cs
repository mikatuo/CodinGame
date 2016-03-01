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

        // game loop
        while (true)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int spaceX = int.Parse(inputs[0]);
            int spaceY = int.Parse(inputs[1]);
            // find the highest mountain.
            int highestX = -1, highestH = -1;
            for (int i = 0; i < 8; i++)
            {
                int mountainH = int.Parse(Console.ReadLine()); // represents the height of one mountain, from 9 to 0. Mountain heights are provided from left to right.
                if ( mountainH > highestH ) {
                    highestH = mountainH;
                    highestX = i;
                }
            }
            // wait until the ship is above the mountain.
            if ( spaceX != highestX )
                Console.WriteLine("HOLD");
            else
                Console.WriteLine("FIRE");
        }
    }
}