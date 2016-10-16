<?php 

include('pdo.inc');

$stmt = ("SELECT * FROM meditationcentre.courses");
$dates = array();
foreach ($pdo->query($stmt) as $row)
	{
		$CourseID = $row["CourseID"];
		$date = $row["CommencementDate"];
		
		
		$length = $row["Length"];		
		for($i = 0; $i < $length; $i++)
		{
			$stmt = ("SELECT * FROM meditationcentre.classes WHERE CAST(DateandTime AS DATE) = '$date'");
			$count = 0;
			
			foreach ($pdo->query($stmt) as $classrow)
			{				
				$count += 1;
				$class = array();
				$class["CourseID"] = $CourseID;
				$class["CourseName"] = $row["CourseName"];
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
	

/*
	$superstudent = array();
	$student = array("Date"=>"21-06-2016",
					"Event"=>"Tuesday");
	$Student = array("21-06-2016"=>$student);
	array_push($superstudent, $Student);
	//array_push($Student,$student);
	
	$student= array("Date"=>"22-06-2016",
					"Event"=>"Wednesday");
	$Student = array("22-06-2016"=>$student);
	array_push($superstudent, $Student);
	echo utf8_encode(json_encode($superstudent));


/*$student = array("Date"=>"21-06-2016",
					"Event"=>"Tuesday");
	$Student = array("21-06-2016"=>$student);
	//array_push($Student,$student);
	echo utf8_encode(json_encode($Student));*/

?>

