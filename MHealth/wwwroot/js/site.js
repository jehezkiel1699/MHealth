// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Handle star hover effect
    $(".star").hover(function () {
        var value = $(this).data("value");
        $("#rating").attr("data-value", value);
        $(this).css("color", "yellow");
        $(this).prevAll().css("color", "yellow");
        $(this).nextAll().css("color", "black");
    });

    // Handle rating submission
    $(".star").click(function () {
        var value = $("#rating").data("value");
        $.post("/Rating/Rate", { value: value }, function (result) {
            alert(result);
        });
    });

    $('#selectedDate').datepicker({
        changeMonth: true,
        changeYear: true,
        format: "mm/dd/yyyy",
        startDate: new Date(),
        todayHighlight: true,
    });
});