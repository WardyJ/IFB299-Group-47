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

function getGender($studentID, $pdo)
{
	$stmt = ("SELECT Gender FROM meditationcentre.students WHERE StudentID = $studentID");
	$gender = $pdo->query($stmt)->fetch(PDO::FETCH_COLUMN);
	return $gender;
}

?>