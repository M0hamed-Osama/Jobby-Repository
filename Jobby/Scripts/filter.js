//trigger filter form to be submitted when clicking on filter option
$(function () {
    $(".custom-control-input").click(function () {
        $("#filter-form").submit();
    });
});

// toggle + or- icon when expand filter options
function Expand(element) {
    var icon = element.firstElementChild;
    if (icon.classList.contains("fa-plus")) {
        icon.classList.remove("fa-plus");
        icon.classList.add("fa-minus");
    }
    else {
        icon.classList.remove("fa-minus");
        icon.classList.add("fa-plus");
    }
}