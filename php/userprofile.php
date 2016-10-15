<?php
	include('pdo.inc');
//returns data from database to populate a student's profile
	

if(isset($_POST['StudentID'])){
	$StudentID = (int)$_POST['StudentID'];

	$stmt =("SELECT * FROM meditationcentre.students 
			INNER JOIN meditationcentre.emergencycontact ON meditationcentre.students.StudentID = meditationcentre.emergencycontact.StudentID 
			INNER JOIN meditationcentre.residentialaddress ON meditationcentre.emergencycontact.StudentID = meditationcentre.residentialaddress.StudentID 
			WHERE meditationcentre.students.StudentID = $StudentID");

	//package and return all student profile related data
	foreach ($pdo->query($stmt) as $row)
	{
		$Student = array();
		$student = array("StudentID"=>$row['StudentID'],
						"FullName"=>$row['FullName'],
						"DateOfBirth"=>$row['DateOfBirth'],
						"PhoneNumber"=>$row['PhoneNumber'],
						"Email"=>$row['Email'],
						"Gender"=>$row['Gender'],
						"MedicalConditions"=>$row['MedicalConditions'],
						"PrescribedMedication"=>$row["PrescribedMedication"],
						"ContactName"=>$row["ContactName"],
						"Relationship"=>$row["Relationship"],
						"ContactPhoneNumber"=>$row["ContactPhoneNumber"],
						"StreetAddress"=>$row["StreetAddress"],
						"City"=>$row["City"],
						"ZipOrPostcode"=>$row["ZipOrPostcode"],
						"State"=>$row["State"],
						"Country"=>$row["Country"]);
		array_push($Student,$student);
		echo utf8_encode(json_encode($Student));
	}
}
else{echo "None";}//no student with that ID exists
	

?>