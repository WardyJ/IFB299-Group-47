<?php

function randomPassword()
{
	$result = " ";
	$chars = "abcdefghijklmnopqrstuvwxyz0123456789";
	$charArray = str_split($chars);
	for($i = 0; $i < 9; $i++)
	{
		$randItem = array_rand($charArray);
		$result = $result.$charArray[$randItem];
	}
	return $result;
}

?>