<?php
header("Access-Control-Allow-Origin: *");

// Allow specific methods (GET, POST, etc.)
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");

// Allow headers (if you need them)
header("Access-Control-Allow-Headers: Content-Type");
error_reporting(E_ALL);
ini_set('display_errors', 1);

header("Content-Type: application/json");

// Extract values from POST data
$playerName = isset($_POST["playerName"]) ? $_POST["playerName"] : null;
$playerWave = isset($_POST["playerWave"]) && $_POST["playerWave"] !== "" ? intval($_POST["playerWave"]) : null;
$playerScore = isset($_POST["playerScore"]) && $_POST["playerScore"] !== "" ? intval($_POST["playerScore"]) : null;


// Stop if any required field is missing
$missingParams = [];

if (!$playerName) $missingParams[] = "playerName";
if ($playerWave === null) $missingParams[] = "playerWave";
if ($playerScore === null) $missingParams[] = "playerScore";

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

echo "Connected successfully<br>";

// Retrieve leaderboard in descending score order
$sql = "SELECT id, name, wave, score, rank FROM Leaderboard ORDER BY score DESC";
$result = $conn->query($sql);

$newRank = 0;

// Determine the new rank based on existing scores
if ($result->num_rows > 0) {
    $lastRank = 0;
    while ($row = $result->fetch_assoc()) {
        if ($playerScore < $row['score']) {
            continue;
        }
        if (($playerScore > $row['score']) ){
                $newRank = $row["rank"]; // Insert player right after this row
                // Now update all players who have a lower score than the new score
                $sql = "UPDATE Leaderboard SET rank = rank + 1 WHERE rank >= ?";
                $stmt = $conn->prepare($sql);
                $stmt->bind_param("i", $newRank); // bind the new rank to update ranks
                $stmt->execute();
                break;
        }
        else if(($playerScore == $row['score']) ) {
            $newRank = $row["rank"];
            break;
        }

    }
    if ($newRank == 0) {
        $newRank = $result->num_rows + 2; // // Place at the bottom if no lower score is found
    }
} else {
    $newRank = 1; // If the leaderboard is empty, assign rank 1
}



// Check if player already exists
$sql = "SELECT name FROM Leaderboard WHERE name = ?";
$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $playerName);
$stmt->execute();
$result = $stmt->get_result();

if ($result->num_rows > 0) {
    echo json_encode(["error" => "This name is already taken."]);
} else {
    echo "Creating user ...";

    // Insert the new player into the leaderboard
    $sql = "INSERT INTO Leaderboard (name, wave, score, rank) VALUES (?, ?, ?, ?)";
    $stmt = $conn->prepare($sql);
    $stmt->bind_param("siii", $playerName, $playerWave, $playerScore, $newRank);
    
    if ($stmt->execute()) {
        echo "Record added with rank: $newRank <br>";
    } else {
        echo json_encode(["error" => "Error inserting record: " . $stmt->error]);
    }
}

// Close statements and database connection
if ($stmt !== null) {
    $stmt->close();
}
$conn->close();
?>
