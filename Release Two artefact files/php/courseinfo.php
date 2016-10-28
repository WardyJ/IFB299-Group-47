<?php
//code to retrieve all course information for all courses
include('pdo.inc');


$stmt = ("SELECT * FROM meditationcentre.courses");

$allCourseInfo = array();

foreach ($pdo->query($stmt) as $row)//for each course put all course info into an array
{
	$courseInfo = array();
	
	$courseID = $row["CourseID"];
	
	$maleStudents = $row["NumMaleStudents"];
	
	$femaleStudents = $row["NumFemaleStudents"];
	
	$maleManagers = $row["NumMaleManagers"];
	
	$femaleManagers = $row["NumFemaleManagers"];
	
	$maleTAs = $row["NumMaleTAs"];
	
	$femaleTAs = $row["NumFemaleTAs"];
	
	$kitchenHelp = $row["NumKitchenHelp"];
	
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
	$courseInfo["KitchenHelp"] = $kitchenHelp;
	
	array_push($allCourseInfo, $courseInfo);
}
echo utf8_encode(json_encode($allCourseInfo));


		?>