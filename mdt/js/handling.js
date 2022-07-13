$(document).ready(function() {
    $("#search").submit(function(event) {
        $("#errorBox").html("");
        $("#results").html("");
        event.preventDefault(); 
        $("#form_search").prop('disabled', true); 
        // ^ Disables button so only one query can be submitted at a time.
        //console.log("Form submitted");
        // Shitty nested if statement
        var form = {
            'first_name': $("#first_name").val(),
            'last_name': $("#last_name").val(),
            'phone_number': $("#phone_number").val(),
            'plate_number': $("#plate_number").val()
        }

        if(form['first_name'] || form['last_name']) {
            if(!form['first_name'] || !form['last_name']) {
                throwError("Missing first/last name");
                enableButton();
            } else {
                console.log("Name search");
                namesearch(form['first_name'], form['last_name']);
                clearform();
            }
        } else if(form['phone_number']) {
            var pattern = /555-\d{4}/;
            var test = pattern.test(form['phone_number']);
            if(test) {
                numbersearch(form['phone_number']);
                clearform();
            } else {
                throwError('Invalid phone number format');
                enableButton();
            }
            
        } else if(form['plate_number']) {
            vehiclesearch(form['plate_number']);
            clearform();
        } else {
            throwError("Please fill forms");
            enableButton();
        }
    
    });
});


function isJailed(time) {
    if(time != 0) {
        return "<b>Notice</b><br> Person is currently incarcerated for " + time + " minutes";
    } else {
        return "";
    }
}


function namesearch(first_name, last_name) {
    loading("show");
    $.ajax({
        url: "queries.php",
        type: "post",
        dataType: "json",
        data: {"search": "name", "first_name": first_name, "last_name": last_name},
        success:function(result) {
            loading("hide");
            if(result.results == 0) {
                throwError("No results found!");
            } else {
                console.log(result);
                $("#results").html("\
                <h3>Person Search</h3><hr>\
                " + isJailed(result.jailed) + "<br>\
                <b>Personal Details</b>\
                <table><tr><th>Lastname</th><td>"+ result.lastName+ "</td></tr>\
                <tr><th>Firstname</th><td>" + result.firstName + "</td></tr>\
                <tr><th>Phone</th><td>555-"+ pad(result.phone, 4) + "</td></tr>\
                <tr><th>Date of Birth</th><td>"+ result.birthday + "</td></tr>\
                <tr><th>Vehicles</th><td>"+ looprow(result.vehicles) + "</td></tr>\
                <tr><th>Addresses</th><td>"+ looprow(result.addresses) + "</td></tr>\
                <tr><th>Other addresses</th><td>" + looprow(result.tenants, "combined") + "</table>\
                <b>Criminal Record</b><br>\
                <table><tr><th>Charges</th><th>Arresting Ofc.</th><th>Time</th></tr>" + looprow(result.criminal, "table") + "</table><hr>\
                <b>Fines Issued</b><br>\
                <table><tr><th>Charges</th><th>Issuing Ofc.</th><th>Amount</th></tr>" + looprow(result.tickets, "table") + "</table>\
                ");
            }
            //console.log(result);
            enableButton();
        },
        error: function(result) {
            loading("hide");
            throwError("Internal server error");
            $("#results").html(result.responseText);
            console.log(result);
            enableButton();
        },
    });
}


function numbersearch(phone_number) {
    loading("show");
    $.ajax({
        url: "queries.php",
        type: "post",
        dataType: "json",
        data: {"search": "number", "phone_number": phone_number},
        success:function(result) {
            loading("hide");
            if(result.results == 0) {
                throwError("No results found!");
            } else {
                console.log("Found results");
                $("#results").html("\
                <h3>Phone Number Lookup</h3><hr>\
                <table><tr><th>Lastname</th><td>"+ result.lastName+ "</td></tr>\
                <tr><th>Firstname</th><td>" + result.firstName + "</td></tr>\
                <tr><th>Phone</th><td>555-"+ pad(result.phone, 4) + "</td></tr>\
                <tr><th>Date of Birth</th><td>"+ result.birthday + "</td></tr></table>\
                ");
            }
            console.log(result);
            enableButton();
        },
        error: function(result) {
            loading("hide");
            throwError("Internal server error");
            $("#results").html(result.responseText);
            console.log(result);
            enableButton();
        },
    });
}


function vehiclesearch(plate_number) {
    loading("show");
    $.ajax({
        url: "queries.php",
        type: "post",
        dataType: "json",
        data: {"search": "vehicle", "plate_number": plate_number},
        success:function(result) {
            loading("hide");
            if(result.results == 0) {
                throwError("No results found!");
            } else {
                console.log("Found results");
                $("#results").html("\
                <h3>Vehicle Database Lookup</h3><hr>\
                <table><tr><th>Vehicle</th><td>" + result.vehicleName + "</td></tr>\
                <tr><th>Plate</th><td>"+ result.vehiclePlate + "</td></tr>\
                <tr><th>Owner</th><td>" + result.owner.LastName +", " + result.owner.FirstName + "</td></tr>\
                <tr><th>Owner Phone</th><td>555-"+ pad(result.charid, 4) + "</td></tr></table>\
                ");
            }
            console.log(result);
            enableButton();
        },
        error: function(result) {
            loading("hide");
            throwError("Internal server error");
            $("#results").html(result.responseText);
            console.log(result);
            enableButton();
        },
    });
}

/// SUPPORTING FUNCTIONS //


// Throws an error into div.errorbox
function throwError(value) {
    $("#errorBox").html('<p><b>ERROR:</b> ' + value + '</p>');
    console.log("ERROR: " + value);
};


// Enables the submit button after ajax query is processed
function enableButton() {
    $("#form_search").prop('disabled', false);
}

// Padding function to make numbers 4 long
function pad (str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}

// Loops all json array values into one string separated by <br>
// Type = address, residency
function looprow(json, type) {
    string = "";
    $.each(json, function(index, key) {
        if(type == 'combined') {
            string += index + " (" + key + ")<br>";
        } else if(type == 'table') {
            console.log(key);
            string += key;
        } else {
            string += key + "<br>";
        }   
    })
    return string;
}


// Hides the loading div if "hide" and shows it if bool is "show"
function loading(bool) {
    if(bool === "show") {
        $("#loading").show();
    } else if(bool === "hide") {
        console.log("Loading completed");
        $("#loading").hide();
    }
}


// If you don't know what this does, you're an idiot.
function clearform() {
    $("#first_name").val("");
    $("#last_name").val("");
    $("#phone_number").val("");
    $("#plate_number").val("");
}