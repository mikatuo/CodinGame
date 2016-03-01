/* global readline */
/* global printErr */

// initialize the surface.
var surface = new Surface();
var landingSpot = surface.findLandingArea();

// initialize the lander.
var lander = new Lander(surface);
lander.setDestination(landingSpot);

// game loop
while (true) {
	lander.readState();
	lander.makeTurn();
}

///// Surface /////

function Surface() {
	this.data = readSurface();
    
    this.findLandingArea = findLandingArea;
}

///// Lander /////

function Lander() {
    this.destination = {};
    this.program = {rotate: 0, power: 0, turns: 0};
    
    this.makeTurn = makeTurn;
    this.endTurn = endTurn;
    this.readState = readState;
	this.setDestination = setDestination;
}

///// Methods /////

/**
 * Accelerates if the lander is not moving in horizontal plane.
 */
function accelerateIfNoHorizontalSpeed(lander) {
    // check if horizontal acceleration is needed.
    if (lander.speed.x !== 0)
        return false;
        
    log('initial acceleration');
    // need horizontal acceleration.
    // identify in which direction.
    lander.program.rotate = 60;   // by default to the left
    lander.program.power = 4;
    lander.program.turns = 8;
    if (lander.destination.distance.x > 0)
        // to the right.
        lander.program.rotate *= -1;
    return true;
}

/**
 * Adjusts a course of a lander.
 */
function makeTurn() {
    var lander = this;
    // follow the course if a program was set.
    if (lander.program.turns) {
        log('keep the course');
        --lander.program.turns;
        return lander.endTurn();
    }
    if (approachingDestination(lander)) {
        log('approaching destination');
        // horizontalDeceleration.
        if (horizontalDeceleration(lander))
            return lander.endTurn();
        if (verticalDeceleration(lander))
            return lander.endTurn();
    } else {
        if (keepHeight(lander))
            return lander.endTurn();
    }
    if (accelerateIfNoHorizontalSpeed(lander))
        return lander.endTurn();
    
    log('default');
    lander.program.rotate = 0;
    lander.program.power = 3;
    return lander.endTurn();
}

/**
 * Checks if lander has approached it's destination.
 */
function approachingDestination(lander) {
    // calculate horizontal position in 2 turns;
    var inAdvance = Math.ceil(lander.speed.x / 6);
    var movingLeft = Math.sign(lander.speed.x) === -1;
    var futureX;
    if (movingLeft)
        futureX = lander.position.x - lander.speed.x * inAdvance;
    else
        futureX = lander.position.x + lander.speed.x * inAdvance;
    var leftX = lander.destination.x1,
        rightX = lander.destination.x2;
    log(leftX + ' < ' + futureX + ' <= ' + rightX)
    return leftX < futureX &&
        futureX <= rightX;
}

/**
 * Calculates distance to the destinaton.
 */
function calculateDestinationDistance(lander) {
    var dest = lander.destination;
    var pos = lander.position;
    
    var x1 = dest.x1 - pos.x;
    var x2 = dest.x2 - pos.x;
    lander.destination.distance = {
        x1: x1,
        x2: x2,
        x: x1 + (x2 - x1)/2,
        y: dest.y - pos.y
    }
}

/**
 * Ends turn.
 */
function endTurn() {
    print(this.program.rotate + ' ' + this.program.power); // rotate power. rotate is the desired rotation angle. power is the desired thrust power.
}

/**
 * Finds an area that can serve as a landing spot.
 */
function findLandingArea() {
    var area = [];
    // search for a flat area.
    for (var i = 0; i < this.data.length - 2; i++) {
        var curr = this.data[i],
            next = this.data[i + 1];
        if (curr.y === next.y) {
            area.push(curr);
            area.push(next);
            break;
        }
    }
    
	return area;
}

/**
 * Decelerate horizontal speed until the speed will be safe for the landing.
 */
function horizontalDeceleration(lander) {
    // at this point the lander should be above the destination.
    // decelerate.
    var speed = lander.speed.x;
    var futureX = lander.position.x + speed * 10;
    var inLandingRange = lander.destination.x1 < futureX 
        && futureX < lander.destination.x2;
    if ((Math.abs(speed) <= 10) && inLandingRange)
        return false;
    
    if (!inLandingRange)
        lander.program.turns = 6;
    speed %= 40;
    log('horizontal deceleration');
    lander.program.rotate = Math.sign(speed) * 45;
    lander.program.power = 4;
    return true;
}

/**
 * Logs a value.
 */
function log(value) {
    if (typeof value === 'object')
        printErr(JSON.stringify(value));
    else
        printErr(value);
}

/**
 * Reads new state of the lander.
 */
function readState() {
    var lander = this;
	var inputs = readline().split(' ');
    lander.position = {
        x: parseInt(inputs[0]),
        y: parseInt(inputs[1])
    }
    lander.speed = {
        x: parseInt(inputs[2]), // the horizontal speed (in m/s), can be negative.
        y: parseInt(inputs[3]) // the vertical speed (in m/s), can be negative.
    }
    lander.fuel = parseInt(inputs[4]); // the quantity of remaining fuel in liters.
    lander.rotate = parseInt(inputs[5]); // the rotation angle in degrees (-90 to 90).
    lander.power = parseInt(inputs[6]); // the thrust power (0 to 4).
    
    calculateDestinationDistance(lander);
}

/**
 * Reads surface information.
 */
function readSurface() {
	var data = [];	
	var surfaceN = parseInt(readline()); // the number of points used to draw the surface of Mars.
	for (var i = 0; i < surfaceN; i++) {
		var inputs = readline().split(' ');
		var landX = parseInt(inputs[0]); // X coordinate of a surface point. (0 to 6999)
		var landY = parseInt(inputs[1]); // Y coordinate of a surface point. By linking all the points together in a sequential fashion, you form the surface of Mars.
		data.push({x: landX, y: landY});
	}
    return data;
}

/**
 * Sets destination of a lander.
 */
function setDestination(area) {
    this.destination.x1 = area[0].x;
    this.destination.x2 = area[1].x;
    this.destination.y = area[0].y;
    this.destination.length = this.destination.x2 - this.destination.x1;
}

/**
 * Stick to the top to avoid any obstacles.
 */
function keepHeight(lander) {
    if ((lander.position.y - lander.destination.y) < 500) {
            log('keep height 1');
            lander.program.rotate = 0;
            lander.program.power = 4;
            return true;
        }
    if (Math.abs(lander.speed.y) > 20) {
        log('keep height 2');
        lander.program.rotate = 0;
        lander.program.power = 4;
        return true;
    }
    
    return false;
}

/**
 * Decelerate vertical speed until the speed will be safe for the landing.
 */
function verticalDeceleration(lander) {
    if (lander.speed.y > -40) {
        return false;
    }
    
    log('vertical deceleration');
    lander.program.rotate = 0;
    lander.program.power = 4;
    return true;
}