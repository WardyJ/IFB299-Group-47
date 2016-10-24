<?php 
//code used to retrieve all data about all classes
include('pdo.inc');

$stmt = ("SELECT * FROM meditationcentre.courses");
$dates = array();
foreach ($pdo->query($stmt) as $row)
	{
		$CourseID = $row["CourseID"];
		$date = $row["CommencementDate"];
		
		
		$length = $row["Length"];		
		for($i = 0; $i < $length; $i++)//for each day of the course
		{
			$stmt = ("SELECT * FROM meditationcentre.classes WHERE CAST(DateandTime AS DATE) = '$date'");
			$count = 0;
			
			foreach ($pdo->query($stmt) as $classrow)//get all class info for each class of the day
			{				
				$class = array();
				$class["CourseID"] = $CourseID;
				$class["ClassID"] = $classrow["ClassID"];
				$class["ClassType"] = $classrow["ClassType"];
				$class["Length"] = $classrow["Length"];
				$class["Description"] = $classrow["Description"];
				$class["DateandTime"] = $classrow["DateandTime"];
				$class["RecordingID"] = $classrow["RecordingID"];
					
				array_push($dates, $class);
			}			
			$date = date('Y-m-d', strtotime($date. " + 1 days"));
			
		}
	}
	
	echo utf8_encode(json_encode($dates));
	


?>

