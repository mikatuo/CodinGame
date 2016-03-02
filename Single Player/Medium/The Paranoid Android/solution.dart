import 'dart:io';
import 'dart:math';

// entry point for the program.
void main() {
    // parse inputs from the input string.
    final inputs = new PuzzleInputs.fromString(stdin.readLineSync());
    
    // game loop
    Clone leadingClone = new Clone();
    while (true) {
        // read position and direction of the leading clone.
        leadingClone.updatePositionAndDirection(stdin.readLineSync());

        // Write an action using print()
        // To debug: stderr.writeln('Debug messages...');

        print('WAIT'); // action: WAIT or BLOCK
    }
}

void log(msg) {
    stderr.writeln(msg);
}

class PuzzleInputs {
    int floorsCount;            // number of floors
    int width;                  // width of the area
    int roundsCount;            // maximum number of rounds
    int exitFloor;              // floor on which the exit is found
    int exitPosition;           // position of the exit on its floor
    int totalClonesCount;          // number of generated clones
    int additionalElevatorsCount;  // ignore (always zero), not used for this first question, value is always 0.

    PuzzleInputs.fromString(String raw) {
        List inputs = raw.split(' ');
        this
            ..floorsCount           = int.parse(inputs[0])
            ..width                 = int.parse(inputs[1])
            ..roundsCount           = int.parse(inputs[2])
            ..exitFloor             = int.parse(inputs[3])
            ..exitPosition          = int.parse(inputs[4])
            ..totalClonesCount          = int.parse(inputs[5])
            ..additionalElevatorsCount  = int.parse(inputs[6]);
        
        // read elevators position.
        int nbElevators = int.parse(inputs[7]);         // number of elevators
        for (int i = 0; i < nbElevators; i++) {
            inputs = stdin.readLineSync().split(' ');
            int elevatorFloor = int.parse(inputs[0]);   // floor on which this elevator is found
            int elevatorPos = int.parse(inputs[1]);     // position of the elevator on its floor
        }
    }
}

class Clone {
    int floor;              // floor of the clone
    int position;           // position of the clone on its floor
    Direction direction;

    void updatePositionAndDirection(String fromString) {
        List inputs = fromString.split(' ');
        this
            ..floor     = int.parse(inputs[0])
            ..position  = int.parse(inputs[1])
            ..direction  = new Direction.fromString(inputs[2]);
    }
}

// this should've been an enum rather than a class but 
// for some reason CodinGame throws an error
// Standard Error Stream: unexpected token 'enum'.
class Direction {
    String _value;
    bool get left   => _value == 'LEFT';
    bool get right  => !left;
    
    Direction.fromString(String value) {
        _value = value;
    }
}