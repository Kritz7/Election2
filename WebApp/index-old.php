
<html>

<?php

error_reporting(E_ALL);
include_once 'gameinput.php';
include_once 'gamewindow.php';

function GetCookie($fCookieName)
{
	return $_COOKIE[$fCookieName];
}

function SaveCookie($fCookieName, $fCookieData)
{
	setcookie($fCookieName, $fCookieData, time()+9999, "/");
}

function DeleteCookie($fCookieName)
{
	setcookie($fCookieName, "", 1, "/");
}

function LoginUser()
{
	if(ctype_alnum($_POST["user"]))
	{
		SaveCookie("USERNAME", strtoupper($_POST["user"]));
		SaveCookie("GAMENAME", strtoupper(substr($_POST["room"], 0, 4)));
		DeleteCookie("GAMESTATE");
		SaveCookie("JS-GAMESTATE", "waiting");
		SaveCookie("JS-LASTVOTE", "wait");
		SaveCookie("LASTVOTEDID", "wait");
		
		AddPlayerToRoom(strtoupper(substr($_POST["room"], 0, 4)),strtoupper($_POST["user"]));
	}
	
	Reload();
}


function LogoutUser()
{
	DeleteCookie("USERNAME");
	DeleteCookie("GAMENAME");
	DeleteCookie("GAMESTATE");
	DeleteCookie("JS-GAMESTATE");
	DeleteCookie("JS-LASTVOTE");
	DeleteCookie("LASTVOTEDID");
	SaveCookie("JS-LASTVOTE", "end");
	SaveCookie("LASTVOTEDID", "end");
	
	PlayerLeaveRoom(GetCookie("GAMENAME"), GetCookie("USERNAME"));
	Reload();
}

function CreateRoomFromUnity($roomName)
{
	CreateRoom($roomName);
}

function Debug($from, $var)
{
	echo "</br>[$from] ".$var."</br>";
}

function GeneratePage()
{
	$username = GetCookie("USERNAME");
	$gamename = GetCookie("GAMENAME");
	$gamestate = GetCookie("JS-GAMESTATE");	
	$jslastvote = GetCookie("JS-LASTVOTE");	
	$lastvote = GetCookie("LASTVOTEDID");

	// SET ALL STATES FROM THE LAST POST
	
	GameInput();

	if(isset($_POST["unity-create"]))
	{
		CreateRoomFromUnity($_POST["unity-create"]);
	}
	
	if($username == NULL && !isset($_POST["login"]))
	{
		// SHOW LOGIN PAGE
		GetLoginForm();
	}
	else if(isset($_POST["login"]))
	{
		LoginUser();
	}
	
	if(isset($_POST["logout"]) || 
	(isset($gamename) && RoomExists($gamename)==false))
	{
		LogoutUser();
	}
	
	if(isset($_POST["unity-gamestate"]) && isset($_POST["unity-gameroom"]))
	{
		AddCommand($_POST["unity-gameroom"], $_POST["unity-gamestate"]);		
	}
	
	// DO STUFF NOW ALL THE VARIBLES ARE SET
	
	if($gamestate == "voting")
	{
		// SHOW GAME BUTTONS
		GameButtons();
	}
	else if($gamestate == "voted")
	{
		echo "Vote submitted!</br>";
		
	}
	
	if(RoomExists(GetCookie("GAMENAME")))
	{
		PrintRoom(GetCookie("GAMENAME"));
		
		GetPlayerInRoom(GetCookie("GAMENAME"), GetCookie("USERNAME"));
	}
	
	echo $jslastvote;

	GenerateLogout();
}

function GameInput()
{	
	if(TestButton("choice1"))
	{
		SaveCookie("LASTVOTEDID", GetCookie("JS-LASTVOTE"));
		SaveCookie("JS-GAMESTATE", "voted");	
		WriteToPlayer(GetCookie("GAMENAME"), GetCookie("USERNAME"), "1");
		Reload();
	}
	
	if(TestButton("choice2"))
	{
		SaveCookie("LASTVOTEDID", GetCookie("JS-LASTVOTE"));
		SaveCookie("JS-GAMESTATE", "voted");
		WriteToPlayer(GetCookie("GAMENAME"), GetCookie("USERNAME"), "2");
		Reload();
	}
}

function TestButton($buttonName)
{
	return isset($_POST[$buttonName]);
}

function GameButtons()
{
	if(RoomExists(GetCookie("GAMENAME")) && GetCookie("JS-GAMESTATE")=="voting")
	{
		if(GetCookie("LASTVOTEDID")!=GetCookie("JS-LASTVOTE") && GetCookie("JS-LASTVOTE")!="end")
		{
			CreateButton("choice1", "Choice 1");
			CreateButton("choice2", "Choice 2");
		}
		else
		{
			echo "Vote submitted!";
		}
	}
}

function CreateButton($buttonName, $buttonText)
{
	echo
		 "<form action='./' method='post'>
		<input value='$buttonText' name='$buttonName' type='submit'>
		</form>";
}

function GenerateLogout()
{
	echo
		 "<form action='./' method='post'>
		<input value='log out' name='logout' type='submit'>
		</form>";
}


function GetLoginForm()
{
	echo
	 "<div id='main'><form action='./' method='post'>
	ROOM: <input type='text' name='room' autocomplete='off' autocorrect='off' style='text-transform: uppercase'><br>
	USERNAME: <input type='text' name='user' autocomplete='off' autocorrect='off' style='text-transform: uppercase'><br>
	<input value='hello' name='login' type='submit'>
	</form>
	</div>";
}
?>

	<meta charset="utf-8">
 	<title>Election Game</title>

    <meta name='viewport' content='width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no' />
	<link rel="stylesheet" href="http://yui.yahooapis.com/pure/0.6.0/pure-min.css">
	<link rel="stylesheet" type="text/css" href="./css/hover-min.css"/>
	<link rel="stylesheet" type="text/css" href="./css/animate.css"/>
	<link rel="stylesheet" type="text/css" href="./css/font-awesome.min.css"/>
	<style type="text/css">
	
		@keyframes fadeOut{
			from{
				display: inline;
				opacity: 1;
			} to {
				display: none;
				opacity: 0;
			}
		}

		@keyframes fadeIn{
			from{
				display: none;
				opacity: 0;
			} to {
				display: inline;
				opacity: 1;
			}
		}
		*, body{
			overflow: hidden;
		}
		body{
			background: #4e5e7a;
			position: relative;
		}

		.screen{
			padding: 5px;
			display: none;
		}
		.hide-screen{
			display: none;
			animation: fadeOut 1s;
		}
		.show-screen{
			display: inline;
			animation: fadeIn 1s;
		}

		h1{
			text-align: center;
			color: #384b6a;
			font-size: 27pt;
			font-family: "Tahoma",  sans-serif;
			letter-spacing: 11px;
			font-weight: 100;
			text-shadow: 3px 2px rgba(0,0,0 ,0.5);
			margin-top: 0;
			margin-bottom: 0;
			
		}
		#heading-container{
			background-image: url("./sayagata-400px/sayagata-400px.png");
			padding-top: 10px;
			padding-bottom: 20px;

		}
		h2{
			color: #fff;
			padding-left: 15px;
			font-family: "Tahoma", sans-serif;
			font-size: 18pt;
			min-height: 24pt;
		}
		.form-container{
			min-width: 100%;
		}
		#loginform{
			background: rgba(255, 255, 255, 0.33);
			border-radius: 10px;
			padding: 33px;
			padding-bottom: 0;
		    margin: 0 auto;
		    min-width: 50%;
		    max-width: 80%;
		    display: block	;
		}
		#loginform legend{
			font-weight: bold;
			color: #fff;
			font-size: 18pt;
		}
		#loginform input{
			margin-top: 15px;
			margin-bottom: 15px;
			width: 100%;
		}
		#loginform #login-btn{
			float: right;
			margin-top: 5px;
			margin-right: 3px;
			border-radius: 10px;
		}
		.left-pad{
			display: inline-block;
			float: left;
			width: 33%;
		}
		.right-pad{
			display: inline-block;
			float: right;
			width: auto;
		}
		.option{
			letter-spacing: 3px;
			padding-top: 20px;
			padding-bottom: 20px;
			margin-top: 5px;
			margin-bottom: 5px;
			margin-left: 0.5%;
			margin-right: 0.5%;
			width: 99%;
			background: #fff;
			background-image: url("sayagata-400px/sayagata-400px.png");
			border-radius: 10px;
			font-size: 17pt;
			color: #384b6a;
			display: none;
		}
		.option:hover{
			cursor: pointer;
		}
		.show-option{
			display: inline-block;
		}
		.option:after{
			height: 20px;
		}
		.options{
			text-align: center;
			font-weight: bold;
			min-height: 156px;
		}
		.secret-goals{
			padding-left: 10px;
			margin: 0 auto;
		}
		.goal{
			padding-top: 5px;
			padding-left: 15px;
			padding-bottom: 5px;
			color: #fff;

			font-weight: 600;
		}
		.goal:before{
			content: "\2022\0020";
		}
		@keyframes hover-text{
			from{
				background-image: url("sayagata-400px/sayagata-400px.png");
			} to {
				background: #fff;
			}
		}
		.hvr-fade:hover,
		.hvr-fade:active{
			background: #fff;
			color: #111b2d;
			/*font-size: 28pt; */
			/*animation: hover-text 1s ease;*/
		}

</style>

</head>
<body>
	<div id="heading-container">
		<h1 id="main-heading" class="animated fadeInLeft"> Election Game</h1>
	</div>
	
	<!-- Login screen -->
	<div class="screen" id="login">
		<div class="pure-g form-container">
			<div class="pure-u form-pad"></div>
			<form class="pure-u pure-form pure-form-stacked" id="loginform">
				<fieldset>
					<legend>Login to Game</legend>
					
					<input id="room_id" placeholder="Enter four letter room ID"/>
					<!--<label for="username">Username</label>-->
					<input id="username" placeholder="Enter your player name"/>

					<!--<label for="room_id">Room ID</label>-->
					

					<button type="button" id="login-btn" class="pure-button pure-button-primary">PLAY</button>

				</fieldset>
			</form>
			<div class="pure-u form-pad"></div>
		</div>
	</div>

	<!-- game screen -->
	<div class="screen" id="game-1">
	<hr/>
	<h2 id="question"></h2>
	<hr/>
	<div class="options pure-g">

	    <div class="option" id="opt_1"></div>
	    <div class="option" id="opt_2"></div>
	</div>
	<h2 class>Your Goals</h2>
	<hr/>
	<div class="secret-goals pure-menu custom-restricted-width">
		<ul class="pure-menu-list pure-g">
			<li class="goal pure-u-1 pure-menu-item hvr-fade" id="goal_1">Secret goal 1</li>
			<li class="goal pure-u-1 pure-menu-item hvr-fade" id="goal_2">Secret goal 2</li>
			<li class="goal pure-u-1 pure-menu-item hvr-fade" id="goal_3">Secret goal 3</li>
		</ul>
	</div>
	<div id="debug"></div>

</body>
<script src="./js/mustache.min.js"></script>
<script type="text/javascript">

	var screenIDs = ["login", "game-1"];
	var gameState = "loginScreen";

	var outAnim = "zoomOutUp";

	function enableClass(el, className){
		if(el != null && el.className.indexOf(className) == -1){
			el.className += (" " + className);
		} 
	}

	function disableClass(el, className){
		if(el != null &&el.className.indexOf(className) != -1){
			var tmpCls = el.className;
			el.className = tmpCls.replace(" " + className, "");
		} 
	}

	function showScreen(screenID){
		for(var i = 0; i < screenIDs.length; i++){
			if(screenIDs[i] == screenID){
				var screenEl = document.getElementById(screenID);
				console.log("Showing " + screenID);
				disableClass(screenEl, "hide-screen");
				enableClass(screenEl, "show-screen");
				return;
			}
		}
	}

	function hideScreen(screenID){
		for(var i = 0; i < screenIDs.length; i++){
			if(screenIDs[i] == screenID){
				console.log("Hiding " + screenID);
				var screenEl = document.getElementById(screenID);
				disableClass(screenEl, "show-screen");
				enableClass(screenEl, "hide-screen");
				return;
			}
		}
	}

	function headingModeLogin(){

	}

	function headingModeInGame(){

	}

	var goal1 = document.getElementById("goal_1");

	goal1.innerHTML ="World domination";

	var opt1 = document.getElementById("opt_1");
	var opt2 = document.getElementById("opt_2");
	var question = document.getElementById("question");

	function selectOption(optEl, altOptEl, outAnim){
		var debug = document.getElementById("debug");
		debug.innerHTML += "click ";

		// remove hover and animate to select
		enableClass(optEl, "animated rubberBand");

		// fill in star
		var opt1Star = optEl.firstElementChild;
		disableClass(opt1Star, "fa-star-o");
		enableClass(opt1Star, "fa-star");
		
		// animate out of screen (up) to indicate it's being sent to server
		setTimeout(function(){
			disableClass(optEl, "rubberBand")
			enableClass(optEl, "animated " + outAnim);

			// swipe away unselected option
			setTimeout(function(){
				enableClass(altOptEl, "animated fadeOutLeft");
			}, 1000);
			
		}, 1000); 

		enableClass(question, "animated fadeOutLeft");
	}

	var selectOpt1 = function(){
		selectOption(opt1, opt2, "zoomOutUp");
		setTimeout(function(){
			newQuestion("Construct additional pylons", "insufficient vespene gas", "nuclear threat detected");	
		}, 5000);
		
	}
	var selectOpt2 = function(){
		selectOption(opt2, opt1, "zoomOutUp");
	}

	opt1.onclick = selectOpt1;
	opt2.onclick = selectOpt2;

	console.log("Game start..");

	function newQuestion(questionText, answer1, answer2){

		// clear animationclasses
		opt1.innerHTML = "";
		opt2.innerHTML = "";
		question.innerHTML = "";

		disableClass(opt1, "animated " + outAnim);
		disableClass(opt2, "animated " + outAnim);
		disableClass(opt1, "pure-u-1 pure-u-md-1-2");
		disableClass(opt2, "pure-u-1 pure-u-md-1-2");
		var opt1Star = opt1.firstElementChild;
		var opt2Star = opt2.firstElementChild;
		disableClass(opt1, "show-option");
		disableClass(opt2, "show-option");

		disableClass(opt1Star, "fa-star");
		disableClass(opt2Star, "fa-star");
		disableClass(opt1, "animated fadeOutLeft");
		disableClass(opt2, "animated fadeOutLeft");

		// show question
		setTimeout(function(){
			disableClass(question, "animated");
			disableClass(question, "fadeOutLeft");
			enableClass(question, "animated fadeInLeft");
			question.innerHTML = "Question: " + questionText;
		}, 1000);

		// show options
		setTimeout(function(){
			enableClass(opt1, "pure-u-1 pure-u-md-1-2 animated fadeInLeft show-option");
			enableClass(opt2, "pure-u-1 pure-u-md-1-2 animated fadeInLeft show-option");
			opt1.innerHTML = "<i id=\"star-1\" class=\" fa fa-star-o\"></i> " + answer1;
			opt2.innerHTML = "<i class=\" fa fa-star-o\"></i> " + answer2;
			setTimeout(function(){
				disableClass(opt1, "animated fadeInLeft");
				disableClass(opt2, "animated fadeInLeft");
			}, 1000);
		}, 2000);
	}

	function startGame(){
		newQuestion("stop the hoverboats", "yarr", "nope");
	}

	if(gameState == "loginScreen"){
		showScreen("login");	
		//showScreen("game-1");
		//startGame();
	}

	var submitBtn = document.getElementById("login-btn");

	submitBtn.addEventListener("click", function(e){
		e.preventDefault();
		if(gameState == "loginScreen"){
			console.log("Hello");
			hideScreen("login");
			showScreen("game-1");

			startGame();


			// TODO: move to a place where data is received from server

		gameState = "inGame";
		}
	});
	
	//updateScreen();
</script>
</html> 