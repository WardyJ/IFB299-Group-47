<?php
//code to retrieve the gender and student type (if they are considered old or new) of a particular student
include('pdo.inc');

if(isset($_POST["StudentID"]))
{
	$StudentID = $_POST["StudentID"];
	
	$stmt = ("SELECT Gender,StudentType FROM meditationcentre.students WHERE StudentID = $StudentID");
	
	foreach ($pdo->query($stmt) as $row)
			{				
				$info = array();
				$gender = $row["Gender"];
				$studentType = $row["StudentType"];
				
				$info["Gender"] = $gender;
				$info["StudentType"] = $studentType;				
			}
			$return = array();
			array_push($return, $info);
			echo utf8_encode(json_encode($return));
}
else{echo "error";}

?>