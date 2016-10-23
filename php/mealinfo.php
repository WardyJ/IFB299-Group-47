<?php

include('pdo.inc');


$stmt = ("SELECT * FROM meditationcentre.meals");

$allMealInfo = array();

foreach ($pdo->query($stmt) as $row)
{
	$mealInfo = array();
	
	$dateAndTime = $row["DateandTime"];
	
	$details = $row["Details"];
		
	$mealInfo["DateandTime"] = $dateAndTime;
	$mealInfo["Details"] = $details;
	
	array_push($allMealInfo, $mealInfo);
}
echo utf8_encode(json_encode($allMealInfo));


		?>