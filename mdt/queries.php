<?php


/* === DATABASE SETUP AND CONNECTION === */
$debug = 1;
//error_reporting(0);
$DB_USER = '';
$DB_PASS = '';
$DB_HOST = '';
$DB_DBASE = '';
$DB_PORT = 3360;

$mysqli = new mysqli($DB_HOST, $DB_USER, $DB_PASS, $DB_DBASE, $DB_PORT);


if($mysqli->connect_errno) {
    echo "MYSQL Database connection error<br>";
    echo debugmessage($mysqli->connect_error);
}


switch($_POST['search']) {
    case "name":
        namesearch($mysqli);
        break;
    case "number":
        numbersearch($mysqli);
        break;
    case "vehicle":
        vehiclesearch($mysqli);
        break;
    default: 
        break;
}

function debugmessage($errormsg) {
    if($debug = 1) {
        echo $errormsg;
    }
}

// Converts an array into mysql compatible list e.g. ('XX','YY') etc. etc.
function convertToSQL($array) {
    $string = "(";
    for($i = 0; $i < count($array); ++$i) {
        $string .= "'" . $array[$i] . "'"; 
        $y = $i+1;
        if($y == count($array)) {
           $string .= ")";
        } else {
            $string .= ",";
        }
        
    }
    return $string;
}


function namesearch($mysqli) {
    $e_fName = $mysqli->real_escape_string($_POST['first_name']);
    $e_lName = $mysqli->real_escape_string($_POST['last_name']);
    $query = "SELECT * FROM character_data WHERE FirstName='{$e_fName}' AND LastName='{$e_lName}'";
    if($result = $mysqli->query($query)) {
        if($result->num_rows == 0) {
            $return = [
                "search" => "name",
                "results" => "0"
            ];
        } else {
            $row = $result->fetch_assoc();
            $return = [
                "search" => "name",
                "firstName" => $row['FirstName'],
                "lastName" => $row['LastName'],
                "birthday" => $row['DOB'],
                "phone" => $row['CharID'],
                "vehicles" => vehiclesByCharID($mysqli, $row['CharID']),
                "addresses" => propertiesByCharID($mysqli, $row['CharID']),
                "tenants" => tenantbyCharID($mysqli, $row['CharID']),
                "jailed" => $row['JailTime'],
                "criminal" => criminalRecordByCharID($mysqli, $row['CharID']),
                "tickets" => ticketsByCharID($mysqli, $row['CharID'])
            ];
        }
        echo json_encode($return);
        $result->free_result();
    } else {

    }
}



function numbersearch($mysqli) {
    $exp = "/555-(\d{4})/";
    preg_match($exp, $_POST['phone_number'], $charID);
    $e_pNumber = $mysqli->real_escape_string($charID[1]);
    $query = "SELECT * FROM character_data WHERE CharID='{$e_pNumber}'";
    if($result = $mysqli->query($query)) {
        if($result->num_rows == 0) {
            $return = [
                "search" => "number",
                "results" => "0"
            ];
        } else {
            $row = $result->fetch_assoc();
            $return = [
                "search" => "number",
                "firstName" => $row['FirstName'],
                "lastName" => $row['LastName'],
                "birthday" => $row['DOB'],
                "phone" => $row['CharID'],
            ];
        }
        echo json_encode($return);
        $result->free_result();
    } else {

    }
}

function vehiclesearch($mysqli) {
    $e_vPlate = $mysqli->real_escape_string($_POST['plate_number']);
    $query = "SELECT * FROM vehicle_data WHERE Plate='{$e_vPlate}'";
    if($result = $mysqli->query($query)) {
        if($result->num_rows == 0) {
            $return = [
                "search" => "vehicle",
                "results" => "0"
            ];
        } else {
            $row = $result->fetch_assoc();
            $owner = nameByCharID($mysqli, $row['CharID']);
            $return = [
                "search" => "vehicle",
                "vehicleName" => $row['VehicleName'],
                "vehiclePlate" => $row['Plate'],
                "charid" => $row['CharID'],
                "owner" => $owner,
            ];
        }
        echo json_encode($return);
        $result->free_result();
    } else {

    }
}

function propertiesByCharID($mysqli, $charID) {
    $e_charID = $e_charID = $mysqli->real_escape_string($charID);
    $query = "SELECT Address FROM property_data WHERE OwnerID='{$e_charID}'";
    if($result = $mysqli->query($query)) {
        if($result->num_rows == 0){
            $return = [
                "No registered addresses."
            ];
        } else {
            $return = [];
            while($row = $result->fetch_assoc()) {
                array_push($return, $row['Address']);

            }
        }
        return $return;
        $result->free_result();
    }
}

//echo criminalRecordByCharID($mysqli, '35');

function ticketsByCharID($mysqli, $charID) {
    $e_charID = $e_charID = $mysqli->real_escape_string($charID);
    $query = "SELECT reason, amount, issuing_officer FROM player_tickets WHERE game_character_id='{$e_charID}'"; 
    if($result = $mysqli->query($query)){
        if($result->num_rows == 0) {
            $return = [
                "<tr><td>No tickets found.</td><td></td><td></td></tr>"
            ];
        } else {
            $return = [];
            while($row = $result->fetch_assoc()) {
                $string = "<tr><td>" . $row['reason'] . "</td><td>" . $row['issuing_officer'] . "</td><td>" . $row['amount'] . "</td></tr>";
                array_push($return, $string);
            }
        }
        return $return;
        $result->free_result();
    } else {
        echo $mysqli->error;
    }
}

function criminalRecordByCharID($mysqli, $charID) {
    $e_charID = $e_charID = $mysqli->real_escape_string($charID);
    $query = "SELECT reason, time, arresting_officer FROM player_arrests WHERE game_character_id='{$e_charID}'"; 
    if($result = $mysqli->query($query)){
        if($result->num_rows == 0) {
            $return = [
                "<tr><td>No arrest record found.</td><td></td><td></td></tr>"
            ];
        } else {
            $return = [];
            while($row = $result->fetch_assoc()) {
                $string = "<tr><td>" . $row['reason'] . "</td><td>" . $row['arresting_officer'] . "</td><td>" . $row['time'] . "</td></tr>";
                array_push($return, $string);
            }
        }
        return $return;
        $result->free_result();
    } else {
        echo $mysqli->error;
    }
}

/*
    Fetch TenantCharacterID, PropertyID
    => Then find address by PropertyID
        => Return address

        SELECT * FROM `property_tenants` WHERE `TenantCharacterID` in ('49','35')

*/

function tenantbyCharID($mysqli, $charID) {
    $e_charID = $mysqli->real_escape_string($charID);
    $query = "SELECT PropertyID, AccessType FROM property_tenants WHERE TenantCharacterID='{$e_charID}'";
    if($result = $mysqli->query($query)) {
        if($result->num_rows == 0) {
            $return = [
                'No tenancies found.'
            ];
        } else {
            $return = [];
            $address = [];
            while($row = $result->fetch_assoc()) {
                array_push($return, $row['PropertyID']);
                array_push($address, $row['AccessType']); // For later use, shows owner relationship
            }
            convertToSQL($return);
            $result->free_result();
            $list = convertToSQL($return);
            $query = "SELECT Address FROM property_data WHERE PropertyID in {$list}";
            if($result = $mysqli->query($query)) {
                if($result->num_rows == 0) {
                    $return = [''];
                } else {
                    $return = [];
                    while($row = $result->fetch_assoc()) {
                        array_push($return, $row['Address']);
                    }
                    $result = array_combine($return, $address);
                    return $result;
                }
            }
        }
    } else {
        echo $mysqli->error;
    }
}




function vehiclesByCharID($mysqli, $cid) {
    $e_cid = $e_charID = $mysqli->real_escape_string($cid);
    $query = "SELECT model, license_plate FROM characters_cars WHERE ID='{$e_cid}'";
    if($result = $mysqli->query($query)) {
        if($result->num_rows == 0){
            $return = [
                "No vehicles"
            ];
        } else {
            $return = [];
            while($row = $result->fetch_assoc()) {
                $full = $row['model'] . " (" . $row['license_plate'] . ")";
                array_push($return, $full);

            }
        }
        return $return;
        $result->free_result();
    }
}




function namebyCharID($mysqli, $ID) {
    $e_ID = $mysqli->real_escape_string($ID);
    $query = "SELECT first_name, last_name FROM characters WHERE ID='{$e_ID}'";
    if($result = $mysqli->query($query)) {
        if($result->num_rows == 0) {
            $return = [
                "search" => "vehicle",
                "results" => "0"
            ];
        } else {
            $row = $result->fetch_assoc();
            $return = [
                "first_name" => $row['first_mame'],
                "last_name" => $row['last_name']
            ];
        }
        return $return;
        $result->free_result();
    } else {
        echo $mysqli->error;
    }   
}