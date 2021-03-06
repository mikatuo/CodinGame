/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

const manager = contactManager();

var N = parseInt(readline());
for (var i = 0; i < N; i++) {
  var telephone = readline();
  manager.addTelephone(telephone);
}

// Write an action using print()
// To debug: printErr('Debug messages...');

// The number of elements (referencing a number) stored in the structure.
print(manager.elementsCount());

///// contact-manager.js //////

function contactManager() {
  let contacts = {},
      elements = 0;

  return {
    addTelephone: function(telephone) { mapIndividualNumbers(telephone, contacts); },
    elementsCount: elementsCount,
    contacts: contacts
  };

  /////

  // adds a telephone number to the map.
  function mapIndividualNumbers(numbers, map) {
    // no more numbers
    if (!numbers && !numbers.length)
      return;

    // extract first number.
    var num = numbers[0];
    var rest = numbers.slice(1);
    
    // add the number to the map if it's not there.
    if (!map.hasOwnProperty(num)) {
      ++elements; // a new element is added.
      map[num] = {};
    }

    // map rest of the numbers.
    mapIndividualNumbers(rest, map[num]);
  }

  // the number of elements (referencing a number) stored in the structure
  function elementsCount() {
    return elements;
  }
}