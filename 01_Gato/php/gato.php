<?php

    class Gato{
        public $db="game.db";

        public  $p1, // username
                $p2, // username2
                $actual,
                $round,
                $score1,
                $score2,
                $winner,
                $board; // array

        public function init()
        {
            $this->board = array(0,0,0,0,0,0,0,0,0);
            
            $this->p1="id1";
            $this->p2="id2";
            $this->actual=1;
            $this->round=0;
            $this->score1=0;
            $this->score2=0;
            $this->winner=0;
            $this->saveDb();
        }
        
        public function saveDb ()
        {
            $file = fopen($this->db, "w") or die("error");
            $strData = json_encode($this->toJson());
            fwrite($file, $strData);
            fclose($file);
            //Que tal la siguiente forma?
            // file_put_contents($this->db, json_encode($this->toJson()));
        }

        public function toJson ()
        {
            $data = array(
                "p1"=>$this->p1,
                "p2"=>$this->p2,
                "actual"=>$this->actual,
                "round"=>$this->round,
                "score1"=>$this->score1,
                "score2"=>$this->score2,
                "winner"=>$this->winner,
                "board"=>$this->board
            );

            return $data;
        }

        public function loadDb ()
        {
            $file = fopen($this->db, "r") or die ("error");
            $strData = fread($file,filesize($this->db));
            $data = json_decode($strData);
            
            $this->p1 = $data->p1;
            $this->p2 = $data->p2;
            $this->actual = $data->actual;
            $this->round = $data->round;
            $this->score1 = $data->score1;
            $this->score2 = $data->score2;
            $this->winner = $data->winner;
            $this->board = $data->board;

            //Segun chatgpt esto es mejor: 
            // if (!file_exists($this->db)) {
            //     $this->init();
            // }
            
            // $data = json_decode(file_get_contents($this->db));
            // $this->p1 = $data->p1;
            // $this->p2 = $data->p2;
            // $this->actual = $data->actual;
            // $this->round = $data->round;
            // $this->score1 = $data->score1;
            // $this->score2 = $data->score2;
            // $this->board = $data->board;
        }

        public function toString()
        {
            echo "".
            "p1:".$this->p1."<br/>".
            "p2:".$this->p2."<br/>".
            "actual:".$this->actual."<br/>".
            "round:".$this->round."<br/>".
            "score1:".$this->score1."<br/>".
            "score2:".$this->score2."<br/>".
            "winner:".$this->winner."<br/>".
            "board:";
            var_dump($this->board);
            
        }

        public function setPlayerId ($player, $id)
        {
            if ($player == 1)
                $this->p1 = $id;
            elseif ($player == 2)
                $this->p2 = $id;
            else
                return false;
            return true;
        }

        public function getPlayer($id)
        {
            if ($id == $this->p1)
                return 1;
            elseif ($id == $this->p2)
                return 2;
            else
                return 0;
        }

        // public function getScore ($player)
        // {
        //     switch($player)
        //     {
        //         case 1:
        //             return $this->score1;
        //         break;

        //         case 2:
        //             return $this->score2;
        //         break;

        //         default:
        //             return "-1";
        //         break;
        //     }
        // }

        // public function getStatus ($id)
        // {
        //     $player = $this->getPlayer($id);

        //     $data = array(
        //         "actual"=>$this->actual,
        //         "round"=>$this->round,
        //         "score ".$player=>$this->getScore($player),
        //         "board"=>$this->board,
        //     );

        //     return json_encode($data);
        // }

        public function getStatus()
        {
            $data = array(
                "actual"=>$this->actual,
                "round"=>$this->round,
                "score1" => $this->score1,  
                "score2" => $this->score2,
                "winner" => $this->winner,
                //"score ".$player=>$this->getScore(),
                "board"=>$this->board,
            );
        
            return json_encode($data);
            
        }

        // public function turn($id, $pos) // pos en formato de array unidimensional
        // {
          
        //     // validar que sea su turno con el ID
        //     // pos válida
        //     echo $pos;
        //     echo $id;
        //     if ( $this->board[$pos] == 0 ) // pos vacía
        //     {
        //         //guardar pos
        //         $this->board[$pos] = ($this->getPlayer($id) % 2) +1 ;
        //         return "OK";
        //     }else{ // error
        //         return "error";
        //     }
        // }
        public function turn($id, $pos) // pos en formato de array unidimensional
        {
            $this->loadDb();
            $player = $this->getPlayer($id);
            if ($player == 0) {
                return "error: jugador no válido";
            }
            
            // if ($player != ($this->actual % 2) + 1) {
            //     return "error: no es tu turno";
            // }
            // if (($this->actual % 2 == 1 && $player != 1) || ($this->actual % 2 == 0 && $player != 2)) {
            //     return "error: no es tu turno";
            // }
            if ($player != $this->actual) {
                return "error: no es tu turno";
            }
            if ($pos < 0 || $pos >= count($this->board)) {
                return "error: posición inválida";
            }
            
            if ($this->board[$pos] != 0) { 
                return "error: posición ocupada";
            }
        
                //guardar pos
            //$this->board[$pos] = ($this->getPlayer($id) % 2) +1 ;
            $this->board[$pos] = $player;
        
            // $this->actual++;
            
            $this->actual = ($this->actual == 1) ? 2 : 1;
            $winner = $this->isWin();
            if ($winner > 0) {
                $this->winner = $winner;
                if ($winner == 1) $this->score1++;
                if ($winner == 2) $this->score2++;
                $this->round++;
                $this->saveDb();
                return "ok, gajó el winner: ".$winner;//json_encode(array("status" => "win", "winner" => $winner));
            }
        
            
            $this->saveDb();
            return "OK se colocó la ficha y sigue el juego";
        
        }

      
        public function isWin()
        {
            $winPatterns = [
                [0, 1, 2], [3, 4, 5], [6, 7, 8], 
                [0, 3, 6], [1, 4, 7], [2, 5, 8],
                [0, 4, 8], [2, 4, 6]            
            ];
            
            foreach ($winPatterns as $pattern) {
                if ($this->board[$pattern[0]] != 0 && 
                    $this->board[$pattern[0]] == $this->board[$pattern[1]] &&
                    $this->board[$pattern[1]] == $this->board[$pattern[2]]) {
                    return $this->board[$pattern[0]];
                }
            }
            return 0;
        }
    }

    $gato = new Gato();

    if( !empty($_GET["action"]) )
    {
        $action = $_GET["action"];
    }
    else
    {
        $action = 0;
    }

    switch($action)
    {
        case 0: // empty
            echo "empty";
        break;

        case 1:
            $gato->init();
            $gato->loadDb();
            $gato->toString();
        break;

        case 2:
            if( !empty($_GET["id"]) )
            {
                $id = $_GET["id"];
            }
            else
            {
                $id = 0;
            }
            
            $gato->loadDb();
            echo $gato->getStatus($id);
        break;

        case 3: // turn
            if( !empty($_GET["id"]) ) // user
            {
                $id = $_GET["id"];
            }
            else
            {
                $id = 0;
            }
            
            if( !empty($_GET["pos"]) ) // user
            {
                $pos = $_GET["pos"]-1;
            }
            else
            {
                $pos = -1;
                echo "pos -1";
            }

            $gato->loadDb();
            echo $gato->turn($id, $pos);
            $gato->saveDb();
        break;

        default:
            echo "No Control";
        break;
    }

?>