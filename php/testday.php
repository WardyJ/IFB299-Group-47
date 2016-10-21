<?php 

$servername = "127.0.0.1";
$username = "team47";
$password = "rolav1";
$dbname = "meditationcentre";
$port = "3306";

$pdo = new PDO("mysql: dbname=$dbname; host=$servername; $port", $username, $password);
$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

$stmt = $pdo->prepare("SELECT ClassID FROM meditationcentre.classes WHERE CAST(DateandTime AS DATE) = '2016-03-26'");
$stmt->execute();
$returnID = $stmt->fetch(PDO::FETCH_COLUMN);
echo $returnID;
?>

