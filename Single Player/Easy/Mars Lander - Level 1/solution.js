// read surface info from input.
var surface = readSurfaceInfo();
// find a flat area on the surface (a pair of surface points that have the same Y).
var lander = createLander(surface.findFlatArea());

// program lander's behavior by chaining strategies.
lander
    .addProgram(left({ degree: 40, power: 3, turns: 15 }))
    .addProgram(left({ degree: 20, power: 4, turns: 10 }))
    //.addProgram(right({ degree: 40, power: 3, turns: 11 }))
    //.addProgram(right({ degree: 20, power: 4, turns: 7 }))
    // deccelerate until desired spid limits are reached.
    .addProgram(deccelerateForLanding());

// game loop.
while (true) {
    lander.updateState();
    //lander.reportState();
    lander.fly();
}

///// flight strategies - each strategy must return true when it has completed it's job.

function left({ degree, power, turns }) { return turn(degree, power, turns); }

function right({ degree, power, turns }) { return turn(-degree, power, turns); }

function turn(d, p, t) {
    let degree = d, power = p, turns = t;
    // accept object as a parameter as well.
    if (arguments.length === 1) {
        degree = arguments[0].degree;
        power = arguments[0].power;
        turns = arguments[0].turns;
    }
    turns = turns && Math.abs(turns) || 1;
    
    return (landerState) => {
        // turn to the left.
        print(`${degree} ${power}`);
        
        return --turns === 0;
    };
}

function deccelerateForLanding() {
    return deccelerate({ speedLimit: { horizontal: 0, vertical: 39 } });
}

function deccelerate({ speedLimit, turns }) {
    const horizontalSpeedLimit = speedLimit.horizontal,
          verticalSpeedLimit = speedLimit.vertical;
    turns = turns && Math.abs(turns) || 9999; // if no turns provided than this is an infinite program.
          
    return (landerState) => {
        let maneuver;
        
        // deccelerate horizontally first.
        const horizontalSpeed = Math.abs(landerState.speed.horizontal);
        if (horizontalSpeed > horizontalSpeedLimit) {
            log('deccelerating horizontally...');
            const degree = Math.min(horizontalSpeed * 10, 40);
            log(degree);
            const deccelerationParams = { degree: degree, power: 4 };
            // check if the lander moving to the right.
            const movingRight = landerState.speed.horizontal > 0;
            // use decceleration maneuver.
            if (movingRight) maneuver = left(deccelerationParams);
            else maneuver = right(deccelerationParams);
        }
        
        // then deccelerate vertically.
        if (!maneuver && Math.abs(landerState.speed.vertical) > verticalSpeedLimit) {
            log('deccelerating vertically...');
            maneuver = turn({ degree: 0, power: 4 });
        }
        
        // the lander has deccelerated.
        if (!maneuver) maneuver = descend();
        
        // execute decceleration maneuver.
        maneuver();
        return --turns === 0;   /*  */
    };
}

function descend() {
    log('descending...');
    return turn({ degree: 0, power: 3 });
}

///// surface

function readSurfaceInfo() {
    var points = readSurfacePoints();
    
    return {
        findFlatArea: () => findFlatArea(points)
    };
    
    ///// surface functions
    
    // find a two consequent points with the same Y and it 
    // will be the result of this function. There is always 
    // a flat area awailable.
    function findFlatArea(surface) {
        var result = surface.reduce((res, curr) => {
            // if a flat surface is already found just return it.
            if (res.found) return res;
            // find two consequent points with the same Y.
            if (res.y !== curr.y)
                return curr;
            else
                return { found: true, x1: res.x, x2: curr.x, y: res.y };
        }, {});
        
        delete result.found;    // get rid of this extra property.
        return result;
    }
    
    function readSurfacePoints() {
        var surface = [];
        var surfaceN = parseInt(readline()); // the number of points used to draw the surface of Mars.
        for (var i = 0; i < surfaceN; i++) {
            var inputs = readline().split(' ');
            // add a point to the surface.
            var point = {
                x: parseInt(inputs[0]), // X coordinate of a surface point. (0 to 6999)
                y: parseInt(inputs[1])  // Y coordinate of a surface point. By linking all the points together in a sequential fashion, you form the surface of Mars.
            };
            surface.push(point);
        }
        return surface;
    }
}

///// lander

function createLander(dest) {
    let destination = dest;
    let state = {};
    let programs = [];
    
    let self = {
        addProgram: (program) => { programs.push(program); return self; },
        updateState: () => updateState(state),
        reportState: () => log('lander:', state),
        fly: initializeFly()
    };    
    return self;
    
    ///// lander functions
    
    function initializeFly() {
        let currentProgram  = 0,
            hasProgramsToExecute = () => currentProgram !== programs.length;
        
        return () => {
            // check if there are any programs left.
            if (!hasProgramsToExecute())
                return;
            
            // execute a current program.
            const completed = programs[currentProgram](state);
            // if the program has completen then switch to the next one.
            if (completed) ++currentProgram;
        };
    }
    
    function readState() {
        const inputs = readline().split(' ');
        const state = {
            x: parseInt(inputs[0]),
            y: parseInt(inputs[1]),
            speed: {
                horizontal: parseInt(inputs[2]),    // the horizontal speed (in m/s), can be negative.
                vertical: parseInt(inputs[3])       // the vertical speed (in m/s), can be negative.
            },
            fuel: parseInt(inputs[4]),      // the quantity of remaining fuel in liters.
            rotate: parseInt(inputs[5]),    // the rotation angle in degrees (-90 to 90).
            power: parseInt(inputs[6])      // the thrust power (0 to 4).
        };
        
        return  state;
    }
    
    function updateState(state) {
        Object.assign(state, readState());
    }
}

///// utilities

function log() {
    var values = [];
    
    for (var i = 0; i < arguments.length; i++) {
        var value = arguments[i];
        if (typeof value !== 'string')
            value = JSON.stringify(value);
        values.push(value);
    }
    
    printErr(values.join(' '));
}