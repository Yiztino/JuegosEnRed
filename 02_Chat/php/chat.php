<?php

    class Chat{
        public $db_folder="rooms";

        public  $allMessages
                ; // array

        public function init()
        {
            
        }
        
        public function saveDb ($room, $newMessage)
        {
            $path = $this->db_folder."/".$room.".db";
            $file = fopen($path, "a") or die("error");
            fwrite($file, "\n".$newMessage);
            fclose($file);

            return true;
        }

        public function loadDb ($room)
        {
            $path = $this->db_folder."/".$room.".db";

            $file = fopen($path, "r") or die("error");
            $this->allMessages = fread($file,filesize($path));

            return true;
        }

        public function getRooms ()
        {
            $rooms = "";
            $files = array_diff(scandir($this->db_folder), array('.', '..'));
            
            foreach($files as $file)
            {
                $rooms = $rooms . "\n" . $file;
            }
            return $rooms;
        }

        public function toString()
        {
            echo "";
        }

        
    }

    $chat = new Chat();

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

        case 1: // getRooms
            $chat->init();
            echo $chat->getRooms();
        break;

        case 2: // getRoomMessages
            if( !empty($_GET["room"]) )
            {
                $room = $_GET["room"];
            }
            else
            {
                $room = 0;
            }
            
            $chat->loadDb($room);
            echo $chat->allMessages;
        break;

        case 3: // sendMessage
            if( !empty($_GET["room"]) ) // user
            {
                $room = $_GET["room"];
            }
            else
            {
                $room = 0;
            }
            
            if( !empty($_GET["message"]) ) // user
            {
                $message = $_GET["message"];
            }
            else
            {
                $message = "noMessage";
            }

            if( !empty($_GET["username"]) ) // user
            {
                $username = $_GET["username"];
            }
            else
            {
                $username = "userX";
            }

            $chat->saveDb($room, $username ." : ".$message);
            #echo $gato->turn($id, $pos);
            #$gato->saveDb();
        break;

        default:
            echo "No Control";
        break;
    }

?>