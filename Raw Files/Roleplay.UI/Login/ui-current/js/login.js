function Delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function OnLoginRequest(elem) {
    elem.classList.add("scale-out");
    elem.style.display = "none";
    var loginBar = document.getElementById("login-progess");
    loginBar.classList.remove("s10");
    loginBar.classList.add("s12");
    showLoadingBar();
    await Delay(1000);
    console.log("[LOGIN] Sending login request to LUA")
    $.post("http://roleplay/Login.LoadCharacters", JSON.stringify({}));
}

async function OnDoLogin(elem) {
    var selectButtons = document.querySelectorAll("#btn-select-char, #btn-delete-char");
    selectButtons.forEach(o => o.classList.add("disabled"));
    showLoadingBar();

    await Delay(600);

    $.post("http://roleplay/Login.SelectChar", JSON.stringify({
        charId: elem.getAttribute("data-charid")
    }));  
}

function showLoadingBar() {
    var loginBar = document.getElementById("login-progess");
    loginBar.classList.remove("scale-out");
}

function hideLoadingBar() {
    var loginBar = document.getElementById("login-progess");
    loginBar.classList.add("scale-out");
}

function getCorrectDate(date) {
    var dateList = date.toString().split("/");
    return `${dateList[2]}/${dateList[1]}/${dateList[0]}`;
}

async function OnCreateCharacter() {
    var firstName = document.getElementById("form_firstname").value;
    var lastName = document.getElementById("form_lastname").value;
    var dob = getCorrectDate(document.getElementById("form_date").value.replace(new RegExp("-", 'g'), "/"));

    var modalElem = M.Modal.getInstance(document.getElementById("create-char-modal"));
    modalElem.close();
    showLoadingBar();

    await Delay(600);

    $.post("http://roleplay/Login.CreateCharacter", JSON.stringify({
        firstName: firstName,
        lastName: lastName,
        DOB: dob
    }));
}

async function OnCharacterDelete(charId) {
    showLoadingBar();

    await Delay(600);

    $.post("http://roleplay/Login.DeleteChar", JSON.stringify({
        charId: charId
    }));
}

window.addEventListener("message", event => {
    if (event.data.type === "showCharacterSelect") {
        console.log("[LOGIN] Recieved characters for 'showCharacterSelect' loading UI elements")
        hideLoadingBar();

        if (JSON.parse(event.data.chars) === null) return;

        var myNode = document.getElementById("main-row");
        while (myNode.firstChild) {
            myNode.removeChild(myNode.firstChild);
        }

        var charDisplay = document.getElementById("character-display");
        charDisplay.classList.remove("scale-out");

        var chars = JSON.parse(event.data.chars);
        for (var i in chars) {
            var char = new Character(chars[i]);
            var card = char.CreateCharacterCard();
            document.getElementById("main-row").append(card);
        }

        document.getElementById("char-create-button").style.opacity = 1.0;
        var characterCount = document.getElementById("main-row").childElementCount;
        if (characterCount === 1) {
            var deletes = document.querySelectorAll("#btn-delete-char");
            for (var del = 0; del < deletes.length; del++) {
                deletes[del].classList.remove("disabled");
            }

            document.getElementById("char-create-button").classList.add("disabled");
        }
        else {
            document.getElementById("char-create-button").classList.remove("disabled");

            if (characterCount === 0) {
                document.getElementById("char-create-button").classList.add("pulse");
            }
        }
    } else if (event.data.type === "close") {
        document.getElementById("login-screen").style.display = "none";

    } else if (event.data.type === "toast") {
        M.toast({ html: event.data.message, displayLength: event.data.length });
    }

});

window.addEventListener("DOMContentLoaded", function() {
    $.get("https://ca-rp.net/fivem/patchnotes.php", function(data) {
        var dataArray = JSON.parse(data);

        document.getElementById("patchnotes-title").innerHTML = dataArray["Title"];
        document.getElementById("patchnotes-div").innerHTML = dataArray["Content"];
	
    });
});