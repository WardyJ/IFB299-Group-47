<?php

include('pdo.inc');
include('functions.inc');

if(isset($_POST["StudentID"]))
{
	$studentID = $_POST["StudentID"];
	
	$gender = getGender($studentID, $pdo);
	
	$stmt = ("SELECT * FROM meditationcentre.courses");
	
	$allEnrollmentInfo = array();
	
	foreach ($pdo->query($stmt) as $row)
	{
		$courseID = $row["CourseID"];
		$courseEnrollment = array();
		
		$astmt = $pdo->prepare("SELECT Role FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE CourseID = $courseID AND s.StudentID = $studentID LIMIT 0,1");
		$astmt->execute();
		$enrollment = $astmt->rowCount();
		
		if($enrollment > 0)
		{
			$courseEnrollment["CourseID"] = $row["CourseID"];
			$courseEnrollment["Role"] = $astmt->fetch(PDO::FETCH_COLUMN);
			
			array_push($allEnrollmentInfo, $courseEnrollment);
		}		
	}
	echo utf8_encode(json_encode($allEnrollmentInfo));
}
else{ echo "error"; }
