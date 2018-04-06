"use strict";

let gameId;
let game;

function getGame() {
    // TODO make API call to get the initial game.
    game = $.get("http://localhost:8080/api/game/new", function(data, status) {
        if (status === "success") {
            gameId = JSON.parse(data).gameId;
        } else {
            // TODO handle errors...
        }
    })
}

function renderBoard() {
    if (game === undefined) {
        return;
    }
    

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
    let action = {
        "row": rowIndex,
        "column": colIndex
    };
    let actionString = JSON.stringify(action);
    $.ajax
    ({
        type: "POST",
        url: "http://localhost:8080/api/game/" + gameId + "/actions",
        data: actionString,
        contentType: "application/json",
        success: function(data, status) {
            game = JSON.parse(data);
        }
    })
}

function init() {
    getGame();
    renderBoard();
    addClickHandlers();
}

init();