<?php
include('pdo.inc');



//posted data to be added to students table in database
$StudentID = $_POST['StudentID'];
$FullName = $_POST['FullName'];
$DateOfBirth = $_POST['DateOfBirth'];//date_create_from_format('Y-m-d',$_POST['DateOfBirth']);
$PhoneNumber = $_POST['PhoneNumber'];
$Email = $_POST['Email'];
$OldEmail = $_POST['OldEmail'];
$Gender = $_POST['Gender'];
$MedicalConditions = $_POST['MedicalConditions'];
$PrescribedMedication = $_POST['PrescribedMedication'];

//posted emergencycontact table variables
$ContactName = $_POST['ContactName'];
$Relationship = $_POST['Relationship'];
$ContactPhoneNumber = $_POST['ContactPhoneNumber'];

//posted residentialaddress table variables		
$StreetAddress = $_POST['StreetAddress'];
$City = $_POST['City'];
$ZipOrPostcode = $_POST['ZipOrPostcode'];
$State = $_POST['State'];
$Country = $_POST['Country'];

//check if someone else is already using this email address
$stmt = $pdo->prepare("SELECT * FROM meditationcentre.students WHERE Email = :Email");
$stmt->bindParam(':Email', $Email);
$stmt->execute();
$count = $stmt->rowCount();
if($count > 0 && $OldEmail != $Email)
{
	echo "duplicate";
}
else
{
	//update students table with user input
	$stmt = $pdo->prepare("UPDATE meditationcentre.students 
	SET FullName = :FullName, DateOfBirth = :DateOfBirth, PhoneNumber = :PhoneNumber, 
	Email = :Email, Gender = :Gender, MedicalCOnditions = :MedicalConditions, PrescribedMedication
	= :PrescribedMedication
	WHERE StudentID = $StudentID");
	$stmt->bindParam(':FullName', $FullName);
	$stmt->bindParam(':DateOfBirth', $DateOfBirth);
	$stmt->bindParam(':PhoneNumber', $PhoneNumber);
	$stmt->bindParam(':Email', $Email);
	$stmt->bindParam(':Gender', $Gender);
	$stmt->bindParam(':MedicalConditions', $MedicalConditions);
	$stmt->bindParam(':PrescribedMedication', $PrescribedMedication);
	
	$stmt->execute();
	
	
	//update emergency contact table with user input
	$stmt = $pdo->prepare("UPDATE meditationcentre.emergencycontact
	SET ContactName = :ContactName, Relationship = :Relationship, ContactPhoneNumber = :ContactPhoneNumber
	WHERE StudentID = $StudentID");
	$stmt->bindParam(':ContactName', $ContactName);
	$stmt->bindParam(':Relationship', $Relationship);
	$stmt->bindParam(':ContactPhoneNumber', $ContactPhoneNumber);
	
	$stmt->execute();
	
	//update residentialaddress table with user input
	$stmt = $pdo->prepare("UPDATE meditationcentre.residentialaddress
	SET StreetAddress = :StreetAddress, City = :City, ZipOrPostcode = :ZipOrPostcode, State = :State, Country = :Country
	WHERE StudentID = $StudentID");
	$stmt->bindParam(':StreetAddress', $StreetAddress);
	$stmt->bindParam(':City', $City);
	$stmt->bindParam(':ZipOrPostcode', $ZipOrPostcode);
	$stmt->bindParam(':State', $State);
	$stmt->bindParam(':Country', $Country);
	
	$stmt->execute();
	

	//package and return all student info to client app to display updated profile
	
	$Student = array();
	$student = array("StudentID"=>$StudentID,
				"FullName"=>$FullName,
				"DateOfBirth"=>$DateOfBirth,
				"PhoneNumber"=>$PhoneNumber,
				"Email"=>$Email,
				"Gender"=>$Gender,
				"MedicalConditions"=>$MedicalConditions,
				"PrescribedMedication"=>$PrescribedMedication,
				"ContactName"=>$ContactName,
				"Relationship"=>$Relationship,
				"ContactPhoneNumber"=>$ContactPhoneNumber,
				"StreetAddress"=>$StreetAddress,
				"City"=>$City,
				"ZipOrPostcode"=>$ZipOrPostcode,
				"State"=>$State,
				"Country"=>$Country);
	array_push($Student,$student);
	echo utf8_encode(json_encode($Student));
}



?>