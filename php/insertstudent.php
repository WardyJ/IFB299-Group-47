<?php
	include('pdo.inc');

$returnID;//value to be returned to client app

	
if (isset($_POST['FullName']) && isset($_POST['DateOfBirth']) && isset($_POST['PhoneNumber']) && isset($_POST['Email']) && isset($_POST['Gender']) && isset($_POST['Password']))
{
	//POST students table variables
	$FullName = $_POST['FullName'];
	$DateOfBirth = $_POST['DateOfBirth'];//date_create_from_format('Y-m-d',$_POST['DateOfBirth']);
	$PhoneNumber = $_POST['PhoneNumber'];
	$Email = $_POST['Email'];
	$Gender = $_POST['Gender'];
	$Password = $_POST['Password'];
	$MedicalConditions = $_POST['MedicalConditions'];
	$PrescribedMedication = $_POST['PrescribedMedication'];
	
	//POST emergencycontact table variables
	$ContactName = $_POST['ContactName'];
	$Relationship = $_POST['Relationship'];
	$ContactPhoneNumber = $_POST['ContactPhoneNumber'];
	
	//POST residentialaddress table variables		
	$StreetAddress = $_POST['StreetAddress'];
	$City = $_POST['City'];
	$ZipOrPostcode = $_POST['ZipOrPostcode'];
	$State = $_POST['State'];
	$Country = $_POST['Country'];
	
	$stmt = $pdo->prepare("SELECT * FROM meditationcentre.students WHERE Email = :Email");
	$stmt->bindParam(':Email', $Email);
	$stmt->execute();
	$count = $stmt->rowCount();
	if($count > 0)
	{
		$returnID = "duplicate";
	}
	else
	{
		//insert user input into students table
		$stmt = $pdo->prepare('INSERT INTO meditationcentre.students(FullName, DateOfBirth, PhoneNumber, Email, Gender, PasswordHash, MedicalCOnditions, PrescribedMedication)
		VALUES(:FullName, :DateOfBirth, :PhoneNumber, :Email, :Gender, SHA2(:Password,0), :MedicalConditions, :PrescribedMedication)');
		$stmt->bindParam(':FullName', $FullName);
		$stmt->bindParam(':DateOfBirth', $DateOfBirth);
		$stmt->bindParam(':PhoneNumber', $PhoneNumber);
		$stmt->bindParam(':Email', $Email);
		$stmt->bindParam(':Gender', $Gender);
		$stmt->bindParam(':Password', $Password);
		$stmt->bindParam(':MedicalConditions', $MedicalConditions);
		$stmt->bindParam(':PrescribedMedication', $PrescribedMedication);
		
		$stmt->execute();
		
		
		//retrieve new student ID to return to app
		$stmt = $pdo->prepare("SELECT * FROM meditationcentre.students WHERE Email = :Email");
		$stmt->bindParam(':Email', $Email);
		$stmt->execute();
		$returnID = $stmt->fetch(PDO::FETCH_COLUMN);
		
		//insert user input into emergency contact table
		$stmt = $pdo->prepare("INSERT INTO meditationcentre.emergencycontact
		VALUES($returnID, :ContactName, :Relationship, :ContactPhoneNumber)");
		$stmt->bindParam(':ContactName', $ContactName);
		$stmt->bindParam(':Relationship', $Relationship);
		$stmt->bindParam(':ContactPhoneNumber', $ContactPhoneNumber);
		
		$stmt->execute();
		
		//insert user input into residentialaddress table
		$stmt = $pdo->prepare("INSERT INTO meditationcentre.residentialaddress
		VALUES($returnID, :StreetAddress, :City, :ZipOrPostcode, :State, :Country)");
		$stmt->bindParam(':StreetAddress', $StreetAddress);
		$stmt->bindParam(':City', $City);
		$stmt->bindParam(':ZipOrPostcode', $ZipOrPostcode);
		$stmt->bindParam(':State', $State);
		$stmt->bindParam(':Country', $Country);
		
		$stmt->execute();
	}
	
}
else{$returnID = "None";}//data uploaded incorrectly
echo $returnID;

?>