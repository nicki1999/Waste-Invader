<?php
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: GET, POST, OPTIONS"); // Allow specific methods (GET, POST)
header("Access-Control-Allow-Headers: Content-Type"); // Allow specific headers
header('Content-Type: application/json'); // Explicitly set the response type to JSON
header("Access-Control-Allow-Credentials: true");

error_reporting(E_ALL);
ini_set('display_errors', 1);

if ($_SERVER['REQUEST_METHOD'] == 'OPTIONS') {
    header("HTTP/1.1 200 OK");
    exit(0);
}

$servername = "172.30.8.134";
$username = "unity_user";
$password = "yourpassword";
$dbname = "SpaceInvaders";  // Specify your actual database name

$conn = new mysqli($servername, $username, $password, $dbname);  // Connect with the database
if ($conn->connect_error) {
    echo json_encode(["error" => "Connection failed: " . $conn->connect_error]);
    exit;
}

$sql = "SELECT name, rank, score, wave FROM Leaderboard ORDER BY rank ASC";  // Query to get data
$result = $conn->query($sql);  // Execute query

$leaderboard = [];

if ($result === false) {
    echo json_encode(["error" => "Query error: " . $conn->error]);
} else if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $leaderboard[] = [
            "name" => $row["name"],
            "rank" => (int) $row["rank"],
            "score" => (int) $row["score"],
            "wave" => (int) $row["wave"]
        ];
    }
}else {
    echo json_encode(["message" => "No results found "]);
}
echo json_encode(["leaderboard" => $leaderboard], JSON_PRETTY_PRINT);


$conn->close();
?>
