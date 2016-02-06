/* global printErr */
/* global readline */
/**
 * Don't let the machines win. You are humanity's last hope...
 **/

var grid = readGrid();

// iterate through grid and link power nodes.
for (var i = 0; i < grid.length; i++)
for (var j = 0; j < grid[i].length; j++) {
    var node = grid[i][j];
    // skip empty nodes.
    if (!node.powerNode) continue;
    var current = node.coordinates,
        right = node
            .nodeToRight ? node.nodeToRight.coordinates : '-1 -1',
        bottom = node
            .nodeToBottom ? node.nodeToBottom.coordinates : '-1 -1';
    print(current + ' ' + right + ' ' + bottom);
}

//////////

function readGrid() {
    var grid = [],
        width = parseInt(readline()), // the number of cells on the X axis
        height = parseInt(readline()); // the number of cells on the Y axis
    // initialize the grid of power nodes.
    for (var i = 0; i < height; i++) {
        var line = readline(); // width characters, each either 0 or .
        grid.push([]);
        // initialize a line of the grid.
        for (var j = 0; j < width; j++) {
            // it's an empty node (by default).
            var node = {
                powerNode:      false,
                coordinates:    j + ' ' + i,
                nodeToRight:    null,   // will be set later.
                nodeToBottom:   null    // will be set later.
            };
            grid[i].push(node);
            // check if it's a power node.
            if (line[j] === '0') { node.powerNode = true; }
            // set node to right for the node to left.
            if (j !== 0 && node.powerNode) {
                var row = grid[i];
                var prev = findPowerNodePreceedingIndex(row, j);
                if (prev) prev.nodeToRight = node;
            }
            // set node to bottom for the node to top.
            if (i !== 0 && node.powerNode) {
                var column = grid.map(function(row) { return row[j]; });
                var prev = findPowerNodePreceedingIndex(column, i);
                if (prev) prev.nodeToBottom = node;
            }
        }
    }
    // the grid has been initialized.
    return grid;
}

function findPowerNodePreceedingIndex(row, endIndex) {
    for (var i = endIndex - 1; i >= 0; i--) {
        if (!row[i].powerNode) continue;
        return row[i];
    }
    return null;
}