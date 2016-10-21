<?php
	include('pdo.inc');
	include('functions.inc');
	
$returnID;//value to be returned to client app

	
if (isset($_POST['CourseID']) && isset($_POST['StudentID']) && isset($_POST['Role']))
{
	//POST enrollment table variables
	$courseID = $_POST['CourseID'];
	$studentID = $_POST['StudentID'];
	$role = $_POST['Role'];
	
	$gender = getGender($studentID, $pdo);
	//insert user input into enrollment table
	$stmt = $pdo->prepare("DELETE FROM meditationcentre.enrollment WHERE StudentID = $studentID AND CourseID = $courseID");
	$stmt->execute();
	
	//update course table
	if($role == "Student")
	{
		if($gender == "Male")
		{
			$insertion = "NumMaleStudents = NumMaleStudents - 1";
			$returnID = substr($insertion,3,12);
		}
		else
		{
			$insertion = "NumFemaleStudents = NumFemaleStudents - 1";
			$returnID = substr($insertion,3,14);
		}
	}
	else if($role == "Manager")
	{
		if($gender == "Male")
		{
			$insertion = "NumMaleManagers = NumMaleManagers - 1";
			$returnID = substr($insertion,3,12);
		}
		else
		{
			$insertion = "NumFemaleManagers = NumFemaleManagers - 1";
			$returnID = substr($insertion,3,14);
		}
	}
	else if($role == "AssistantTeacher")
	{
		if($gender == "Male")
		{
			$insertion = "NumMaleTAs = NumMaleTAs - 1";
			$returnID = substr($insertion, 3, 7);
		}
		else
		{
			$insertion = "NumFemaleTAs = NumFemaleTAs - 1";
			$returnID = substr($insertion, 3, 9);
		}
	}
	else
	{
		$insertion = "NumKitchenHelp = NumKitchenHelp - 1";
		$returnID = substr($insertion, 3, 11);
	}
	$stmt = $pdo->prepare("UPDATE meditationcentre.courses SET $insertion WHERE CourseID = $courseID");
	$stmt->execute();


}
else{$returnID = "error";}//data uploaded incorrectly
echo $returnID;

?>