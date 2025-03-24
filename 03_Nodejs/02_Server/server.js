const express = require('express');
const app = express();
const port = 80;
var count = 0;

app.get('/', (req, res) => { 
    res.send('Hello World');
});

app.get('/action/init', (req, res) => { 
    res.send('Initializacion de gato');
});

app.get('/action/count', (req, res) => { 
    count++;
    res.send('Count'+count);
});

app.get('/action/status/:player', (req, res) => { 
    res.send(`Return de status de player ${req.params['player']} `);
});

app.get('/action/turn/:player/:pos', (req, res) => { 
    //let player = req.params['player'];
    let player = "";
    let pos = req.params['pos'];
    
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
    if(pos > 9 || pos < 0){
        pos="error";
    }
    
    res.send(`El player ${player} ha tirado en ${pos}`);
});

app.listen(port, () => { 
    console.log(`Server init: ${port}`);
});