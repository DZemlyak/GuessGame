$(document).ready(function() {
 
    var $logInButton = $("#logInButton");
    var $logOutButton = $("#logOutButton");
    var $guessButton = $("#guessButton");
    var $newGameButton = $("#newGameButton");
    var $showHistory = $("#showHistory");

    enableHistory();
    checkCookie();
    getStatus();

    $logInButton.click(function () {
        setCookie();
        checkCookie();
    });

    $logOutButton.click(function () {
        $.removeCookie("UserName");
        checkCookie();
    });

    $guessButton.click(function() {
        guessNumber();
    });

    $newGameButton.click(function () {
        startGame();
    });

    $showHistory.click(function () {
        showHistory();
    });

});

function enableHistory() {
    $.ajax({
        url: "/Home/SetAllHistory",
        method: "POST"
    });
}

function checkCookie() {
    var cookieValue = $.cookie("UserName");
    if (typeof cookieValue === "undefined") {
        showElements(false);
    } else {
        var $greetings = $("#userGreeting");
        showElements(true);
        var text = "Hello, " + cookieValue + "!";
        $greetings[0].innerHTML = text;

        $.ajax({
            url: "/Home/SetHistory?userName=" + cookieValue,
            method: "POST"
        });
    }
}

function setCookie() {
    var $name = $("#userName").val().trim();
    $.cookie("UserName", $name,{
        expires: 1,
        path: '/'
    });
}

function showElements(logged) {
    var $logOut = $("#logOut");
    var $logIn = $("#logIn");
    var $field = $("#gameField");
    if (logged) {
        $logOut.show();
        $logIn.hide();
        $field.show();
    } else {
        $logOut.hide();
        $logIn.show();
        $field.hide();
    }
}

function getStatus() {
    var $gameStatus = $("#gameStatus");
    $.ajax({
        url: "/Home/GetStatus",
        method: "GET",
        success: function(data) {
            if (data.Host != null) {
                $gameStatus.text("User \"" + data.Host + "\" has started the Game. Guess the Number!");
            } else if(data.Winner != null) {
                $gameStatus.text("User \"" + data.Winner + "\" is a winner! The Number was " + data.SecretNumber + ".");
            } else {
                $gameStatus.text(data.Status);
            }
            setTimeout(getStatus, 1000);
        },
        error: function() {
            $gameStatus.text("Error occurred!");
        }
    });
}

function guessNumber() {
    var $number = parseInt($("#guessNumberInput").val().trim());
    var $result = $("#userResult");
    var $userName = $.cookie("UserName");
    $.ajax({
        url: "/Home/GuessNumber?number=" + $number + "&userName=" + $userName,
        method: "GET",
        success: function (data) {
            $result.text(data.Result);
        }
    });
}

function startGame() {
    var $number = parseInt($("#startGameInput").val().trim());
    var $userName = $.cookie("UserName");
    $.ajax({
        url: "/Home/StartGame?number=" + $number + "&userName=" + $userName,
        method: "GET",
        success: function () {
            $("#startGameInput").text("");
        }
    });
}

function showHistory() {
    var $history = $("#history");
    var $userName = $.cookie("UserName");
    $.ajax({
        url: "/Home/ShowHistory?userName=" + $userName,
        method: "GET",
        success: function (data) {
            $history.text(data.History);
        }
    });

}