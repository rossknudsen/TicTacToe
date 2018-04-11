"use strict";

let game;

function getGame() {
    // TODO make API call to get the initial game.
    $.get("http://localhost:8080/api/game/new", function(data, status) {
        if (status === "success") {
            game = data;
            renderBoard();
        } else {
            // TODO handle errors...
        }
    })
}

function renderBoard() {
    if (game === undefined) {
        console.log("The game variable is not available");
        return;
    }
    for (let index = 0; index < 9; index++) {
        let square = game.Game.Board.Squares[index];
        if (square.Token === 1) {
            setToken(index, "X");
        } else if (square.Token === 2) {
            setToken(index, "O");
        } else {
            setToken(index, "");
        }
    }

    hideAllOutcomes();
    var gameState = game.Game.GameResult.GameState;
    if (gameState == 0) {
        // playing
        hideAllOutcomes();
    } else if (gameState == 1) {
        // draw
        showDraw();
    } else if (gameState == 2) {
        // a player won.
        if (game.Game.GameResult.WinningToken == game.Game.HumanPlayer.PlayerToken) {
            showPlayerWon();            
        } else {
            showComputerWon();
        }
        highlightWinningDirection();
    }
}

function hideAllOutcomes() {
    $("#playerWon").hide();
    $("#playerLost").hide();
    $("#draw").hide();
}

function showDraw() {
    $("#draw").show();
}

function showPlayerWon() {
    $("#playerWon").show();
}

function showComputerWon() {
    $("#playerLost").show();
}

function highlightWinningDirection() {
    // TODO currently I am not sending enough information to easily to this.
}

function setToken(squareNumber, token) {
    $("td")[squareNumber].innerText = token;
}

function addClickHandlers() {
    const table = document.getElementsByTagName("table")[0];

    // loop through each row
    const rows = table.getElementsByTagName("tr");

    for (let rowIndex = 0; rowIndex < rows.length; rowIndex++) {
        let row = rows[rowIndex];
        let cells = row.getElementsByTagName("td");
        
        // loop through each cell
        for (let cellIndex = 0; cellIndex < cells.length; cellIndex++) {
            let cell = cells[cellIndex];
            
            cell.addEventListener("click", function(event) {
                handleSquareClick(rowIndex, cellIndex);
            })
        }
    }
}

function handleSquareClick(rowIndex, colIndex) {
    // check if the game is not over.
    if (game.Game.GameResult.GameState !== 0) {
        alert("The game is over now.  Press 'Start New Game'");
        return;
    }

    let action = {
        "row": rowIndex,
        "column": colIndex
    };

    // check if the square is already occupied.
    var squareNumber = rowIndex * 3 + colIndex;
    if (game.Game.Board.Squares[squareNumber].Token !== 0) {
        alert("That square is already occupied.  Choose another.");
        return;
    }

    let actionString = JSON.stringify(action);
    $.ajax
    ({
        type: "POST",
        url: "http://localhost:8080/api/game/" + game.GameId + "/actions",
        data: actionString,
        contentType: "application/json",
        success: function(data, status) {
            game = data;
            renderBoard();
        }
    })
}

function init() {
    getGame();
    addClickHandlers();
}

init();