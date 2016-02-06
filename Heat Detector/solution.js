/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

var inputs      = readInputs();

var building    = inputs.building,
    jumps       = inputs.jumps,
    batman      = inputs.batman;

// game loop
while (true) {
    var BOMBDIR = readline(); // the direction of the bombs from batman's current location (U, UR, R, DR, D, DL, L or UL)
    
    // get windows that can contain the bomb based on the direction.
    var zoneWithBomb = getZoneWithBomb(BOMBDIR);
    // TODO get center of the zone with the bomb.
    var window = getCentralWindow(zoneWithBomb);
    // move batman in the middle of the zone with the bomb.
    moveBatman(window.x, window.y); // the location of the next window Batman should jump to.
}

//////////

/** Gets window in the center of the specified zone. */
function getCentralWindow(zone) {
    var lengthX = zone.x2 - zone.x1,
        lengthY = zone.y2 - zone.y1;
    // calculate X an Y in the middle of the zone.
    var middleX = Math.ceil(lengthX / 2),
        middleY = Math.ceil(lengthY / 2);
    // return the window in the center.
    return {
        x: zone.x1 + middleX,
        y: zone.y1 + middleY
    };
}

/** Gets zone in the specified direction from batman. */
function getZoneWithBomb(direction) {
    var x1 = 0, x2 = building.width - 1,
        y1 = 0, y2 = building.height - 1;
    
    switch (direction) {
        case 'U':   // Up
            x1 = batman.x; x2 = batman.x;
            y2 = batman.y - 1;
            break;
        case 'UR':  // Up-Right
            x1 = batman.x + 1;
            y2 = batman.y - 1;
            break;
        case 'R':   // Right
            x1 = batman.x + 1;
            y1 = batman.y; y2 = batman.y;
            break;
        case 'DR':  // Down-Right
            x1 = batman.x + 1;
            y1 = batman.y + 1;
            break;
        case 'D':   // Down
            x1 = batman.x; x2 = batman.x;
            y1 = batman.y + 1;
            break;
        case 'DL':  // Down-Left
            x2 = batman.x - 1;
            y1 = batman.y - 1;
            break;
        case 'L':   // Left
            x2 = batman.x - 1;
            y1 = batman.y; y2 = batman.y;
            break;
        case 'UL':  // Up-Left
            x2 = batman.x - 1;
            y2 = batman.y - 1;
            break;
        default:
            throw 'The direction "' + direction + '" is not supported.';
    }
    // consider previous knowledge.
    if (building.canHaveBomb.x1 > x1) x1 = building.canHaveBomb.x1;
    else building.canHaveBomb.x1 = x1;
    if (building.canHaveBomb.x2 < x2) x2 = building.canHaveBomb.x2;
    else building.canHaveBomb.x2 = x2;
    if (building.canHaveBomb.y1 > y1) y1 = building.canHaveBomb.y1;
    else building.canHaveBomb.y1 = y1;
    if (building.canHaveBomb.y2 < y2) y2 = building.canHaveBomb.y2;
    else building.canHaveBomb.y2 = y2;
    // the zone has been identified.
    return {
        x1: x1,
        x2: x2,
        y1: y1,
        y2: y2
    };
}

/** Moves the batman. */
function moveBatman(x, y) {
    batman.x = x;
    batman.y = y;
    print(x + ' ' + y)
}

/** Reads inputs and returns built building and available BATMAN jumps. */
function readInputs() {
    var inputs = readline().split(' ');
    var W = parseInt(inputs[0]), // width of the building.
        H = parseInt(inputs[1]), // height of the building.
        N = parseInt(readline()); // maximum number of turns before game over.
    var inputs = readline().split(' ');
    var X0 = parseInt(inputs[0]),
        Y0 = parseInt(inputs[1]);
    // build the building.
    var building = {
        width: W,
        height: H,
        // at the beginning we don't know which part of the 
        // building contains the bomb.
        canHaveBomb: {
            x1: 0,
            x2: W - 1,
            y1: 0,
            y2: H - 1 
        }
    };
    // done.
    return {
        batman: { x: X0, y: Y0 },
        building: building,
        jumps: N
    }
}

function log(value) {
    var stringValue = JSON.stringify(value);
    
    var cursor = 0, length = 63;
    while (cursor < stringValue.length) {
        printErr(stringValue.substr(cursor, length));
        cursor += length;
    }
}