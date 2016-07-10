(* Auto-generated code below aims at helping you parse *)
(* the standard input according to the problem statement. *)
open System

let countSequenceNumbers (list : int list) =
    let mutable result = []
    let mutable num = list.[0]
    let mutable count = 1
    for i in 1..list.Length-1 do
        if num = list.[i] then
            count <- count + 1
        else
            result <- (num, count)::result
            num <- list.[i]
            count <- 1
    result <- (num, count)::result
    List.rev result

let R = int(Console.In.ReadLine())
let L = int(Console.In.ReadLine())

// initialize up to first two lines of the sequence.
let mutable conwaySequence = [[R]; [1; R]]

// build rest of the sequence up to line number L.
// the algorithm expects initial conway sequence list to
// be in reverse order - for better performance because
// adding new lines to the list is faster if they are 
// prepend to the list.
conwaySequence <- List.rev conwaySequence
for i in 3..L do
    // previous line.
    let prevLine = conwaySequence.[0]
    // count numbers of the previous line.
    let newLine = 
        prevLine
        |> countSequenceNumbers
        |> Seq.map (fun (num, count) -> [count; num])
        |> Seq.toList
        |> List.concat
    conwaySequence <- newLine::conwaySequence
    
// reverse order of the list elements.
conwaySequence <- List.rev conwaySequence

// contacenate numbers of line L into a string.
let result = 
    conwaySequence.[L-1]
    |> List.map (fun v -> (string v))
    |> String.concat " "

printfn "%s" result