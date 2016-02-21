import 'dart:io';
import 'dart:math';

/**
 * Entry point of the program.
 **/
void main() {
    List<int> stockValues = readStockValues();
    
    // if there is nothing to analyze return 0.
    if ( stockValues.length == 0 ) { print( '0' ); return; }
    
    // iterate through the values and find the largest possible loss.
    int curr, loss,
        prev        = stockValues[ 0 ],
        largestLoss = 0;
    for (var i = 1, len = stockValues.length; i < len; ++i) {
        curr = stockValues[i];
        // cache the highest value before fall phase.
        if ( curr > prev ) { prev = curr; continue; }
        // cache the largest loss.
        loss = curr - prev;
        if ( largestLoss > loss ) largestLoss = loss;
    }

    print( largestLoss );
}

/**
 * Reads stock values.
 **/
List<int> readStockValues() {
    var values = new List<int>();
    int n = int.parse( stdin.readLineSync() );
    // split stock values, they separated with a space.
    List raw = stdin.readLineSync().split( ' ' );
    for (int i = 0; i < n; i++) {
        // parse values as integer.
        int v = int.parse( raw[ i ] );
        values.add( v );
    }
    
    return values;
}