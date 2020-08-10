//trigger upload-image link to open browsing window then submit the uploaded image
$(function () {
    $("#upload-img").on('click', function (e) {
        e.preventDefault();
        $("#upload:hidden").trigger('click');
    });
    $("#upload").change(function () {
        $("#img-form").submit();
    });
});