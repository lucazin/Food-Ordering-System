$('#signupForm').submit(function (e) {
    if ($('#inputPassword').val() != $('#inputConfirmPassword').val()) {
        $('#label').text("Password not matched")
        $('#alert2').show();
        event.preventDefault();
    }
    else {
        return;
    }
});

document.getElementById("btnClose2").addEventListener("click", function (event) {
    $('#alert2').hide();
});
document.getElementById("btnClose").addEventListener("click", function (event) {
    $('#alert1').hide();
});