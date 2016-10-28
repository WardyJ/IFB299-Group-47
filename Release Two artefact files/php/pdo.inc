<?php 
//set up pdo to be used to for all database queries

$servername = "ec2-52-62-115-138.ap-southeast-2.compute.amazonaws.com";
$username = "Team47";
$password = "rolav1";
$dbname = "meditationcentre";
$port = "3306";

$pdo = new PDO("mysql: dbname=$dbname; host=$servername; $port", $username, $password);
$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
try
{
$result = $pdo->query('SELECT * FROM meditationcentre.courses');
}
catch (PDOException $e)
{
echo $e->getMessage();
};
?>

