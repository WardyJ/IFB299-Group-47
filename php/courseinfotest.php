<?php

include('pdo.inc');

if(isset($_POST["StudentID"]))
{
		$StudentID = 516;//$_POST["StudentID"];


	$stmt = ("SELECT Gender FROM meditationcentre.students WHERE StudentID = $StudentID");
	$gender = $pdo->query($stmt)->fetch(PDO::FETCH_COLUMN);

	$stmt = ("SELECT * FROM meditationcentre.courses");
	$courseInfo = array();
	$allCourseInfo = array();

	$status;
	foreach ($pdo->query($stmt) as $row)
	{
		$courseID = $row["CourseID"];
		
		$astmt = $pdo->prepare("SELECT * FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE Gender = 'Male' AND courseID = $courseID AND Role = 'Student'");
		$astmt->execute();
		$maleStudents = $astmt->rowCount();
		
		$astmt = $pdo->prepare("SELECT * FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE Gender = 'Female' AND courseID = $courseID AND Role = 'Student'");
		$astmt->execute();
		$femaleStudents = $astmt->rowCount();
		
		$astmt = $pdo->prepare("SELECT * FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE Gender = 'Male' AND courseID = $courseID AND Role = 'Manager'");
		$astmt->execute();
		$maleManagers = $astmt->rowCount();
		
		$astmt = $pdo->prepare("SELECT * FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE Gender = 'Female' AND courseID = $courseID AND Role = 'Manager'");
		$astmt->execute();
		$femaleManagers = $astmt->rowCount();
		
		$astmt = $pdo->prepare("SELECT * FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE Gender = 'Male' AND courseID = $courseID AND Role = 'AssistantTeacher'");
		$astmt->execute();
		$maleTAs = $astmt->rowCount();
		
		$astmt = $pdo->prepare("SELECT * FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE Gender = 'Female' AND courseID = $courseID AND Role = 'AssistantTeacher'");
		$astmt->execute();
		$femaleTAs = $astmt->rowCount();
		
		$astmt = $pdo->prepare("SELECT * FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE courseID = $courseID AND Role = 'KitchenHelp'");
		$astmt->execute();
		$kitchenHelp = $astmt->rowCount();
		
		$astmt = $pdo->prepare("SELECT * FROM meditationcentre.enrollment e INNER JOIN meditationcentre.students s ON e.StudentID = s.StudentID WHERE courseID = $courseID AND s.StudentID = $StudentID");
		$astmt->execute();
		$enrollment = $astmt->rowCount();
		
		$CommencementDate = $row["CommencementDate"];
		
		if($enrollment > 0)
		{
			$status = "Registered";
		}
		elseif(time() > strtotime($CommencementDate))
		{
			$status = "Closed";
		}
		elseif($gender == "Male" && $maleStudents > 25 || $gender == "Female" && $femaleStudents > 25)
		{
			$status = "Full";
		}
		else
		{
			$status = "Open";
		}
		$courseInfo["CourseID"] = $courseID;
		$courseInfo["CourseName"] = $row["CourseName"];
		$courseInfo["Length"] = $row["Length"];
		$courseInfo["CommencementDate"] = $row["CommencementDate"];
		$courseInfo["Description"] = $row["Description"];
		$courseInfo["MaleStudents"] = $maleStudents;
		$courseInfo["FemaleStudents"] = $femaleStudents;
		$courseInfo["MaleManagers"] = $maleManagers;
		$courseInfo["FemaleManagers"] = $femaleManagers;
		$courseInfo["MaleTAs"] = $maleTAs;
		$courseInfo["FemaleTAs"] = $femaleTAs;
		$courseInfo["Status"] = $status;
		
		array_push($allCourseInfo, $courseInfo);
	}
	echo utf8_encode(json_encode($allCourseInfo));

}
else{echo"error";}


		
		

		?>