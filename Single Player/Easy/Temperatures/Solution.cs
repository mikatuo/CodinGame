using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static void Main(string[] args)
    {
        int N = int.Parse( Console.ReadLine() ); // the number of temperatures to analyse
        string TEMPS = Console.ReadLine(); // the N temperatures expressed as integers ranging from -273 to 5526
        
        // if no temperature is provided.
        if ( N == 0 ) {
            // display 0 (zero) and exit.
            Console.WriteLine( "0" );
            return;
        }
        
        // parse temperatures.
        int[] temps = TEMPS.Split( new [] { " " }, StringSplitOptions.RemoveEmptyEntries ).Select( value => int.Parse( value ) ).ToArray();
        
        // get the temperature closest to zero.
        Console.WriteLine( GetTemperatureClosestToZero( temps ) );
    }
    static int GetTemperatureClosestToZero(IList<int> temperatures)
    {
        // if there is only one temperature. Return it.
        if ( temperatures.Count == 1 )
            return temperatures[ 0 ];
        // search for the temperature closest to zero.
        int maxNegative = int.MinValue,
            minPositive = int.MaxValue;
        for( var i = 0; i < temperatures.Count; i++ ) {
            if ( temperatures[ i ] == 0 )
                return 0;
            // search for the maximum negative number.
            if ( temperatures[ i ] < 0 && maxNegative < temperatures[ i ] )
                maxNegative = temperatures[ i ];
            // search for the minimum positive number.
            if ( temperatures[ i ] > 0 && minPositive > temperatures[ i ] )
                minPositive = temperatures[ i ];
        }
        // check if there is only one closest value is found.
        if ( maxNegative == int.MinValue )
            return minPositive;
        if ( minPositive == int.MaxValue )
            return maxNegative;
        // check if the positive number is equal to the negative 
        // number in absolute magnitude. Prefer the positive number then.
        int negativeAbs = Math.Abs( maxNegative );
        if ( negativeAbs == minPositive )
            return minPositive;
        // check which number is is closer to zero.
        if ( negativeAbs < minPositive )
            // the negative number is closer.
            return maxNegative;
        // the positive number is closer.
        return minPositive;
    }
}