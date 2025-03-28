const express = require('express');
const fs = require('fs');
const app = express();
const port = 80;
var count = 0;

// PARA EL JUEGO DE GATO EN SÍ

class Gato{
    constructor(){
        this.db = "game.db";
        this.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        this.p1 = "id1";
        this.p2 = "id2";
        this.actual = 1;
        this.round = 0;
        this.score1 = 0;
        this.score2 = 0;
        this.winner = 0;
    }
    init(){
        this.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
        this.p1 = "id1";
        this.p2 = "id2";
        this.actual = 1;
        this.round = 0;
        this.score1 = 0;
        this.score2 = 0;
        this.winner = 0;
        this.saveDb();
    }
    saveDb(){
        fs.writeFileSync(this.db, JSON.stringify(this));
    }

    loadDb() {
        if (fs.existsSync(this.db)) {
            const data = JSON.parse(fs.readFileSync(this.db));
            Object.assign(this, data);
        } else {
            this.init();
        }
    }

    getPlayer(id) {
        return id === this.p1 ? 1 : id === this.p2 ? 2 : 0;
    }

    getStatus(){
        return{
            actual: this.actual,
            round: this.round,
            score1: this.score1,
            score2: this.score2,
            winner: this.winner,
            board: this.board
        }
    }
    turn(id, pos) {
        this.loadDb();
        let player = this.getPlayer(id);
        if (player === 0) return "error: jugador no válido";
        if (player !== this.actual) return "error: no es tu turno";
        if (pos < 0 || pos >= 9) return "error: posición inválida";
        if (this.board[pos] !== 0) return "error: posición ocupada";

        this.board[pos] = player;
        this.actual = this.actual === 1 ? 2 : 1;

        let winner = this.isWin();
        if (winner > 0) {
            this.winner = winner;
            winner === 1 ? this.score1++ : this.score2++;
            this.round++;
            this.saveDb();
            return `Ganó el jugador ${winner}`;
        }

        this.saveDb();
        return "OK, sigue el juego";
    }

    isWin() {
        const winPatterns = [
            [0, 1, 2], [3, 4, 5], [6, 7, 8],
            [0, 3, 6], [1, 4, 7], [2, 5, 8],
            [0, 4, 8], [2, 4, 6]
        ];
        for (let pattern of winPatterns) {
            if (this.board[pattern[0]] !== 0 &&
                this.board[pattern[0]] === this.board[pattern[1]] &&
                this.board[pattern[1]] === this.board[pattern[2]]) {
                return this.board[pattern[0]];
            }
        }
        return 0;
    }
}

const game = new Gato();

// Para las calls de los jugadores
app.get('/', (req, res) => { 
    res.send('Hello World');
});

app.get('/action/init', (req, res) => { 
    game.init();
    res.send('Initializacion de gato');
});

// app.get('/action/count', (req, res) => { 
//     count++;
//     res.send('Count'+count);
// });

app.get('/action/status', (req, res) => { 
    res.json(game.getStatus());
    //res.send(`Return de status} `);
});

app.get('/action/turn/:player/:pos', (req, res) => { 
    //let player = req.params['player'];
   
    let pos = req.params['pos'];
    let player = "";
    switch(req.params['player']){
        case "1":
            player = "player01";
            break;
        case "2":
            player = "player02";
            break;
        default:
            player = "error";
            break;
    }

    if (!/^\d+$/.test(pos)) {
        return res.status(400).send("Error: La posición debe ser un número.");
    }
    pos = parseInt(pos);
    if(pos > 9 || pos < 1){
        pos="error, la posición no es válida";
    }
    res.send(game.turn(player, pos));
    //res.send(`El player ${player} ha tirado en ${pos}`);
});

app.listen(port, () => { 
    console.log(`Server init: ${port}`);
});
