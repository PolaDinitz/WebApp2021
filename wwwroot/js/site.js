// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function deleteEntity(controller, id) {
    $.post('/' + controller + '/Delete',
        { id },
        function callback(url) {
            window.location.href = url;
        }
    );
}