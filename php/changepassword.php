<?php
	include('pdo.inc');

$Email = $_POST['Email'];
$Password = $_POST['Password'];
$OldPassword = $_POST['OldPassword'];
$returnID;

$stmt =$pdo->prepare("SELECT * FROM meditationcentre.students WHERE Email = '$Email' && PasswordHash = SHA2(:OldPassword,0)");
$stmt->bindParam(':OldPassword', $OldPassword);
$stmt->execute();

$count = $stmt->rowCount();
if($count < 1)
{
	$returnID = "false";
}
else
{
	$stmt =$pdo->prepare("SELECT * FROM meditationcentre.students WHERE Email = '$Email' && PasswordHash = SHA2(:Password,0)");
	$stmt->bindParam(':Password', $Password);
	$stmt->execute();

	$count = $stmt->rowCount();
	if($count > 0)
	{
		$returnID = "old";
	}
	else
	{
		$stmt = $pdo->prepare("UPDATE meditationcentre.students SET PasswordHash = SHA2(:Password,0) WHERE Email = :Email");
		$stmt->bindParam(':Email', $Email);
		$stmt->bindParam(':Password', $Password);
		$stmt->execute();
		$returnID = "success";
	}

}

echo $returnID;
?>