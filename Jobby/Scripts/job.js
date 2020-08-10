// show/hide details button
function Details(element) {
    var icon = element.firstElementChild;
    if (icon.classList.contains("fa-eye")) {
        element.title = "Hide Details"
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
    }
    else {
        element.title = "Show Details"
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
    }
}