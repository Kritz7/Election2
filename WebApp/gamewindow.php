<?php

?>

<script language="javascript">

var myVar=setInterval(function(){checkUpdate()},2*1000); // at 5 second intervals

function getCookie(name) {
  var value = "; " + document.cookie;
  var parts = value.split("; " + name + "=");
  if (parts.length == 2) return parts.pop().split(";").shift();
}

function checkUpdate()
{
	var xhttp = new XMLHttpRequest();
	  xhttp.onreadystatechange = function() {
		if (xhttp.readyState == 4 && xhttp.status == 200) {
		  document.getElementById("stat").innerHTML = xhttp.responseText;
		  
		if(xhttp.responseText.indexOf("reload-all")>-1
		  && xhttp.responseText.substring(10,14) != getCookie("last-reload-id"))
		  {
			document.cookie = "last-reload-id="+xhttp.responseText.substring(10,14)+";path=/";
			location.reload();
		  }
		  
		if(xhttp.responseText.indexOf("endvote-all")>-1
		&& xhttp.responseText.substring(10,14) != getCookie("last-endvote-id"))
		  {
			document.cookie = "last-endvote-id="+xhttp.responseText.substring(10,14)+";path=/";
			document.cookie = "JS-GAMESTATE=waiting;path=/";
			document.cookie = "JS-LASTVOTE=end;path=/";
			
			location.reload();
			
		  }
		  
		if(xhttp.responseText.indexOf("voting-all")>-1
		&& xhttp.responseText.substring(10,14) != getCookie("last-startvote-id"))
		  {
			document.cookie = "last-startvote-id="+xhttp.responseText.substring(10,14)+";path=/";
			document.cookie = "JS-GAMESTATE=voting;path=/";
			document.cookie = "JS-LASTVOTE="+xhttp.responseText.substring(10,14)+";path=/";
			
			location.reload();
			
		  }
		}
	  };
	xhttp.open('GET', './rooms/GAMEDATA_'+getCookie('GAMENAME')+'_COMMANDS');
	xhttp.send();
}

</script>