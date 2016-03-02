import 'dart:io';
import 'dart:math';

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
void main() {
    List inputs;
    inputs = stdin.readLineSync().split(' ');
    int nbFloors = int.parse(inputs[0]); // number of floors
    int width = int.parse(inputs[1]); // width of the area
    int nbRounds = int.parse(inputs[2]); // maximum number of rounds
    int exitFloor = int.parse(inputs[3]); // floor on which the exit is found
    int exitPos = int.parse(inputs[4]); // position of the exit on its floor
    int nbTotalClones = int.parse(inputs[5]); // number of generated clones
    int nbAdditionalElevators = int.parse(inputs[6]); // ignore (always zero)
    int nbElevators = int.parse(inputs[7]); // number of elevators
    for (int i = 0; i < nbElevators; i++) {
        inputs = stdin.readLineSync().split(' ');
        int elevatorFloor = int.parse(inputs[0]); // floor on which this elevator is found
        int elevatorPos = int.parse(inputs[1]); // position of the elevator on its floor
    }

    // game loop
    while (true) {
        inputs = stdin.readLineSync().split(' ');
        int cloneFloor = int.parse(inputs[0]); // floor of the leading clone
        int clonePos = int.parse(inputs[1]); // position of the leading clone on its floor
        String direction = inputs[2]; // direction of the leading clone: LEFT or RIGHT

        // Write an action using print()
        // To debug: stderr.writeln('Debug messages...');

        print('WAIT'); // action: WAIT or BLOCK
    }
}