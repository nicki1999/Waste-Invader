<?php
header("Access-Control-Allow-Origin: *");

// Allow specific methods (GET, POST, etc.)
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");

// Allow headers (if you need them)
header("Access-Control-Allow-Headers: Content-Type");

error_reporting(E_ALL);
ini_set('display_errors', 1);

$missingParams = [];  // Initialize missing params array

// Corrected typo in $_POST["playerName"]
$playerName = isset($_POST["playerName"]) ? $_POST["playerName"] : null;

if (!$playerName) {
    $missingParams[] = "playerName";
}

if (!empty($missingParams)) {
    echo json_encode(["error" => "Missing required POST parameters: " . implode(", ", $missingParams)]);
    exit();
}

// Database connection
$servername = "172.30.8.134";
$username = "unity_user";
$password = "yourpassword";
$dbname = "SpaceInvaders";

$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    echo json_encode(["error" => "Connection failed: " . $conn->connect_error]);
    exit();
}

// Check if the player name exists
$sql = "SELECT COUNT(*) AS count FROM Leaderboard WHERE name = ?";
$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $playerName);
$stmt->execute();
$stmt->bind_result($count);
$stmt->fetch();
$stmt->close();

if ($count > 0) {
    echo json_encode(["error" => "This name is already taken."]);
    exit();
}

echo json_encode(["success" => "Username is available."]);
$conn->close();
?>
