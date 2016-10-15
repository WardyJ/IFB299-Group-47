<?php
	include('pdo.inc');
	include('functions.inc');

$Email;
$Password;
if (isset($_POST['Email']))//data uploaded correctly
{
	$Password = randomPassword();
	$Email = $_POST['Email'];
	$stmt = $pdo->prepare("UPDATE meditationcentre.students SET PasswordHash = SHA2(:Password,0) WHERE Email = :Email");
	$stmt->bindParam(':Email', $Email);
	$stmt->bindParam(':Password', $Password);
	$stmt->execute();


	$stmt =("SELECT * FROM meditationcentre.students WHERE Email = '$Email'");


	if(count($pdo->query($stmt)->fetchAll()) < 1) //no students exist with posted email address
	{
		echo "None";
	}
	foreach ($pdo->query($stmt) as $row)//student's phonenumber and new password are packaged and sent back to client app
	{
		$Student = array();
		$student = array("PhoneNumber"=>$row['PhoneNumber'],
						"PasswordHash"=>$Password); //plaintext is sent back but is called hash
		array_push($Student,$student);
		echo utf8_encode(json_encode($Student));
	}
}
else //data uploaded incorrectly
{
	echo "Error";
}

?>