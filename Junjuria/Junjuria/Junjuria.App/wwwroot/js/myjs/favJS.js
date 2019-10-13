var token = document.getElementsByName("__RequestVerificationToken")[0].value;
console.log(token);
$(document).ready(function () {
    $("#FavourizeButton").change(function () {
        console.log(choise);
        dto.Choise = dto.Choise?false:true;
        $.ajax({
            type: 'POST',
            url: '/Products/Favourize',
            data: { dto, __RequestVerificationToken: token },
            success: function (data) {
                alert(data);
            }
        });
        dto.Choise = $("#FavourizeButton").prop('checked');
    });
});