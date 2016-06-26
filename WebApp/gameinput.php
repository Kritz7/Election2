<?php

include_once "index.php";

function Reload($delay)
{
	if(isset($delay))
	{
		header( 'Refresh:'.$delay.'; url=http://kritz.net/election/' );
	}
	else
	{
		header( 'Location: http://kritz.net/election/' );
	}
}

function CreateRoom($roomName)
{
	if(isset($roomName))
	{
		if(!RoomExists($roomName))
		{
			$myfile = fopen("./rooms/GAMEDATA_".$roomName, "w") or die("Unable to load game!");
			chmod("./rooms/GAMEDATA_".$roomName, 0777);
			fclose($myfile);
			
			$myfile = fopen("./rooms/GAMEDATA_".$roomName."_COMMANDS", "w") or die("Unable to load game!");
			chmod("./rooms/GAMEDATA_".$roomName, 0777);
			fclose($myfile);
		}
		
		$myfile = fopen("./rooms/GAMEDATA_".$roomName, "a") or die("Unable to append to game!");
		fwrite($myfile, '['.$roomName.']:');
		fclose($myfile);
	}
	else
	{
		Reload();
	}		
}

function RoomExists($roomName)
{
	return file_exists("./rooms/GAMEDATA_".$roomName);
}

function DestroyRoom($roomName)
{
	if(RoomExists($roomName))
	{
		unlink("./rooms/GAMEDATA_".$roomName);
	}
	
	if(RoomExists($roomName."_COMMANDS"))
	{
		unlink("./rooms/GAMEDATA_".$roomName."_COMMANDS");
	}
}

function PrintRoom($roomName)
{
	$myfile =  file_get_contents("./rooms/GAMEDATA_".$roomName) or die("Unable to read $roomName!");
	// echo $myfile;
	fclose($myfile);
}

function AddPlayerToRoom($roomName, $playerName)
{
	if(!RoomExists($roomName))
	{
		CreateRoom($roomName);
	}
	
	$myfile = fopen("./rooms/GAMEDATA_".$roomName, "a") or die("Unable to APTR!");
	fwrite($myfile, $playerName.'{}:');
	fclose($myfile);
}

function WriteToPlayer($roomName, $playerName, $data)
{	
	if(RoomExists($roomName) && GetPlayerInRoom($roomName, $playerName)!=null)
	{		
		$fileRead = file_get_contents("./rooms/GAMEDATA_".$roomName);

		$playerStartPos = strpos($fileRead, $playerName."{") + strlen($playerName."{");
		$playerEndPos = strpos($fileRead, "}", $playerStartPos);
		
		$newFileEdited = substr($fileRead,0,$playerEndPos)."$data,".substr($fileRead,$playerEndPos);
		
		//echo "</br></br>~ ".$newFileEdited." ~</br></br>";
		
		/*
		echo "Append: ".substr($fileRead,0,$playerStartPos)."$data".substr($fileRead,$playerEndPos)."$playerStartPos , $playerEndPos</br></br>$fileRead";
		*/
		
		$myfile = fopen("./rooms/GAMEDATA_".$roomName, "wa+");
		fwrite($myfile, $newFileEdited);	
		fclose($myfile);
	}
}

function PlayerLeaveRoom($roomName, $playerName)
{
	if(RoomExists($roomName) && GetPlayerInRoom($roomName, $playerName)!=null)
	{
		$fileRead = file_get_contents("./rooms/GAMEDATA_".$roomName) or die("Unable to RPLR $roomName!");
		
		$fileBeforePlayerPos = strpos($fileRead,$playerName);
		$fileAfterPlayerPos = strPos($fileRead, ":", $fileBeforePlayerPos)+1;
		
		$filePre = substr($fileRead, 0, $fileBeforePlayerPos);
		$filePos = substr($fileRead, $fileAfterPlayerPos, $fileAfterPlayerPos);
		
		$myfile = fopen("./rooms/GAMEDATA_".$roomName, "w") or die("Unable to load game!");
		fwrite($myfile, $filePre.$filePos);
		fclose($myfile);
		
		if(GetPlayersInRoom($roomName)==0)
		{
			DestroyRoom($roomName);
		}
	}
}

function GetPlayersInRoom($roomName)
{	
	$count = 0;
	$fileRead = file_get_contents("./rooms/GAMEDATA_".$roomName) or die("Unable to RPLR $roomName!");
	$count = substr_count($fileRead, "{");
	fclose($fileRead);
	
	return $count;
}

function GetPlayerInRoom($roomName, $playerName)
{
	$myfile = file_get_contents("./rooms/GAMEDATA_".$roomName) or die("Unable to PGD $roomName!");
	$strPos = strpos($myfile,$playerName);
	
	if(!isset($strPos))
	{
		echo "$playerName not found  ".($strPos===false);
		return null;
	}
	else
	{
		$line = substr($myfile, $strPos, $strPos-strPos($myfile, ":", $strPos));
		//echo "</br></br>Line: ".$line." ".strPos($myfile, ":", $strPos);
		return $line;
	}
}


function AddCommand($roomName, $commandName)
{
	if(RoomExists($roomName))
	{
		$myfile = fopen("./rooms/GAMEDATA_".$roomName."_COMMANDS", "w+") or die("Unable to APTR!");
		fwrite($myfile, $commandName.",");
		fclose($myfile);
	}
}

function ExecuteCommand($command)
{
	if($command == "reload-all")
	{
		Reload();
	}
}

?>