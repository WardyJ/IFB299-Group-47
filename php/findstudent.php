<?php
	include('pdo.inc');
	include('functions.inc');
//code to find student in database with uploaded ID, email or name.
$StudentID;
$Email;
$FullName;
$Password;
$returnID = "None";

if(isset($_POST['StudentID']))
{
	$StudentID = $_POST['StudentID'];
	$stmt =("SELECT StudentID FROM meditationcentre.students WHERE StudentID = $StudentID");

	if(count($pdo->query($stmt)->fetchAll()) < 1)
	{
		$returnID = "None";
	}
	else
	{
		$stmt = $pdo->prepare($stmt);
		$stmt->execute();
		$returnID = $stmt->fetch(PDO::FETCH_COLUMN);
	}
}
if (isset($_POST['Email']))
{   $Email = $_POST['Email'];
	$query = "SELECT StudentID FROM meditationcentre.students WHERE Email = :Email";
	$stmt = $pdo->prepare($query);
	$stmt->bindParam(':Email', $Email);
	
	$stmt->execute();
	$returnID = $stmt->fetch(PDO::FETCH_COLUMN);
	if($returnID == null)
	{
		$returnID = "None";
	}
}
if(isset($_POST['FullName']))
{
	$FullName = $_POST['FullName'];
	$stmt =("SELECT * FROM meditationcentre.students");
	$Student = array();
	foreach ($pdo->query($stmt) as $row)
	{
		if(strpos(strtoLower($row['FullName']),strtoLower($FullName)) !== false)
		{
			$student = array("StudentID"=>$row['StudentID'],
							"FullName"=>$row['FullName']);
			array_push($Student,$student);
		}
	}
	echo utf8_encode(json_encode($Student));
}
else{echo $returnID;}

?>