<?php
include('pdo.inc');


$returnID;

if (isset($_POST['Email']) && isset($_POST['Password']))
{
	//POST variables
	$Email = $_POST['Email'];
	$Password = $_POST['Password'];
	//search for student matching this email and password
	$stmt = $pdo->prepare('SELECT StudentID FROM meditationcentre.students WHERE LOWER(Email) = LOWER(:Email) and PasswordHash = SHA2(:Password,0)');
	$stmt->bindParam(':Email', $Email);
	$stmt->bindParam(':Password', $Password);
	
	$stmt->execute();
	$returnID = $stmt->fetch(PDO::FETCH_COLUMN);
	
	//if a student wasn't found check admin login info
	if($returnID == null)
	{
		$stmt = $pdo->prepare('SELECT Position FROM meditationcentre.administration WHERE LOWER(Email) = LOWER(:Email) and PasswordHash = SHA2(:Password,0)');
		$stmt->bindParam(':Email', $Email);
		$stmt->bindParam(':Password', $Password);
		$stmt->execute();
		$returnID = $stmt->fetch(PDO::FETCH_COLUMN);
		if($returnID == null)
		{
			$returnID = "None";//no admin found either
		}
	}
}
else{$returnID = "error";}//data uploaded incorrectly
echo $returnID;

?>