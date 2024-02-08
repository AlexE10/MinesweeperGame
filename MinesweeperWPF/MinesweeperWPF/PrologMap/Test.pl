:- initialization(main).

placeBombsInList([H|T], [_|_], Bombsnumber, R):-
    Bombsnumber =:= 0,
    R = [H|T].

placeBombsInList([H|T], [], _, [H|T]).

placeBombsInList([H|T], [SetH|SetT], Bombsnumber, R):-
    R2 is SetH,
    randomize([H|T], R2, 0, R3),
	placeBombsInList(R3, SetT, Bombsnumber-1, R).

randomize([], _, _, []).

randomize([_|T], RandPos, RandPos, [-1|T]).

randomize([H|T], RandPos, Contor, [H|R]):-
    Contor =\= RandPos,
    Contor1 is Contor + 1,
    randomize(T, RandPos, Contor1, R).

unflatten(List, RowSize, Matrix) :-
    unflatten_helper(List, RowSize, Matrix).

unflatten_helper([], _, []).
unflatten_helper(List, RowSize, [Row|Matrix]) :-
    length(Row, RowSize),
    append(Row, Remaining, List),
    unflatten_helper(Remaining, RowSize, Matrix).

matrix(Nrows, Ncols, BombsNumber, R) :-
    length(Matrix, Nrows),
    length(Row, Ncols),
    maplist(=(0), Row),
    maplist(=(Row), Matrix),
    length(Matrix, ListLength),
    NewLen is ListLength ** 2,
    randset(BombsNumber, NewLen, ResSet),
    placeNumbersForBigMatrix(Matrix, ResSet, Nrows, NumMatrix),
    flatten(NumMatrix, ResMatrix),
    placeBombsInList(ResMatrix, ResSet, BombsNumber, BombsList),
    unflatten(BombsList, Ncols, R).

in_range(Value, Low, High) :-
    Value >= Low,
    Value =< High.

placeNumbersForOneList([], _, _, []).

placeNumbersForOneList([H|T], Y, Contor, R):-
    in_range(Contor, Y-1, Y+1),
    R2 is H+1,
    placeNumbersForOneList(T, Y, Contor+1, R3),
    R = [R2|R3].

placeNumbersForOneList([H|T], Y, Contor, [H|R]):-
    Contor =\= Y,
    Contor1 is Contor + 1,
    placeNumbersForOneList(T, Y, Contor1, R).

placeNumbersForMatrix([], _, _, _, []).

placeNumbersForMatrix([H|T], Y, X, Contor, R):-
    in_range(Contor, X-1, X+1),
    placeNumbersForOneList(H, Y, 0, R2),
    placeNumbersForMatrix(T, Y, X, Contor+1, R4),
    R = [R2|R4].

placeNumbersForMatrix([H|T], Y, X, Contor, R):-
    placeNumbersForMatrix(T, Y, X, Contor+1, R4),
    R = [H|R4].

placeNumbersForBigMatrix([H|T], [], _, [H|T]).

placeNumbersForBigMatrix([H|T], [HSet|TSet], NRows, R):-
	Div is HSet // NRows,
    Mod is HSet mod NRows,
    placeNumbersForMatrix([H|T], Mod, Div, 0, R2),
    placeNumbersForBigMatrix(R2, TSet, NRows, R).


main :-
    open('../../../PrologMap/input.txt', read, InputStream),
    read_line_to_codes(InputStream, NrowsCodes),
    number_codes(Nrows, NrowsCodes),
    read_line_to_codes(InputStream, NcolsCodes),
    number_codes(Ncols, NcolsCodes),
    read_line_to_codes(InputStream, BombsNumberCodes),
    number_codes(BombsNumber, BombsNumberCodes),
    close(InputStream),
    matrix(Nrows, Ncols, BombsNumber, Matrix),

    open('../../../PrologMap/matrix_output.txt', write, OutputStream),
    write_matrix_to_file(Matrix, OutputStream),
    close(OutputStream),

    halt.

write_matrix_to_file([], _).
write_matrix_to_file([Row|Matrix], Stream) :-
    write_row_to_file(Row, Stream),
    write_matrix_to_file(Matrix, Stream).

write_row_to_file([], Stream) :-
    write(Stream, '\n').
write_row_to_file([H|T], Stream) :-
    write(Stream, H),
    write(Stream, ' '),
    write_row_to_file(T, Stream).
