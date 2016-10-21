<?php
	include('pdo.inc');
	include('functions.inc');
	
$returnID;//value to be returned to client app

	
if (isset($_POST['CourseID']) && isset($_POST['StudentID']) && isset($_POST['CourseHistoryOne']) && isset($_POST['CourseHistoryTwo']) 
	&& isset($_POST['CourseHistoryThree']) && isset($_POST['CourseHistoryFour']) && isset($_POST['CourseHistoryFive']) && isset($_POST['Role']))
{
	//POST enrollment table variables
	$courseID = $_POST['CourseID'];
	$studentID = $_POST['StudentID'];
	$courseHistoryOne = $_POST['CourseHistoryOne'];
	$courseHistoryTwo = $_POST['CourseHistoryTwo'];
	$courseHistoryThree = $_POST['CourseHistoryThree'];
	$courseHistoryFour = $_POST['CourseHistoryFour'];
	$courseHistoryFive = $_POST['CourseHistoryFive'];
	$role = $_POST['Role'];
	
	$gender = getGender($studentID, $pdo);
	
	$stmt = $pdo->prepare("SELECT * FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE CourseID = $courseID AND Gender = '$gender' AND Role = '$role'");
	$stmt->execute();
	$count = $stmt->rowCount();
	if($count > 25)
	{
		$returnID = "error";
	}
	else
	{
		//insert user input into enrollment table
		$stmt = $pdo->prepare('INSERT INTO meditationcentre.enrollment(StudentID, CourseID, CourseHistoryOne, CourseHistoryTwo, CourseHistoryThree, CourseHistoryFour, CourseHistoryFive, Role)
		VALUES(:StudentID, :CourseID, :CourseHistoryOne, :CourseHistoryTwo, :CourseHistoryThree, :CourseHistoryFour, :CourseHistoryFive, :role)');
		$stmt->bindParam(':StudentID', $studentID);
		$stmt->bindParam(':CourseID', $courseID);
		$stmt->bindParam(':CourseHistoryOne', $courseHistoryOne);
		$stmt->bindParam(':CourseHistoryTwo', $courseHistoryTwo);
		$stmt->bindParam(':CourseHistoryThree', $courseHistoryThree);
		$stmt->bindParam(':CourseHistoryFour', $courseHistoryFour);
		$stmt->bindParam(':CourseHistoryFive', $courseHistoryFive);
		$stmt->bindParam(':role', $role);
		$stmt->execute();
		
		//update course table
		if($role == "Student")
		{
			if($gender == "Male")
			{
				$insertion = "NumMaleStudents = NumMaleStudents + 1";
				$returnID = substr($insertion,3,13);
			}
			else
			{
				$insertion = "NumFemaleStudents = NumFemaleStudents + 1";
				$returnID = substr($insertion,3,15);
			}
		}
		else if($role == "Manager")
		{
			if($gender == "Male")
			{
				$insertion = "NumMaleManagers = NumMaleManagers + 1";
				$returnID = substr($insertion,3,13);
			}
			else
			{
				$insertion = "NumFemaleManagers = NumFemaleManagers + 1";
				$returnID = substr($insertion,3,15);
			}
		}
		else if($role == "AssistantTeacher")
		{
			if($gender == "Male")
			{
				$insertion = "NumMaleTAs = NumMaleTAs + 1";
				$returnID = substr($insertion, 3, 9);
			}
			else
			{
				$insertion = "NumFemaleTAs = NumFemaleTAs + 1";
				$returnID = substr($insertion, 3, 11);
			}
		}
		else
		{
			$insertion = "NumKitchenHelp = NumKitchenHelp + 1";
			$returnID = substr($insertion, 3);
		}
		$stmt = $pdo->prepare("UPDATE meditationcentre.courses SET $insertion WHERE CourseID = $courseID");
		$stmt->execute();
	}
	
}
else{$returnID = "error";}//data uploaded incorrectly
echo $returnID;

?>