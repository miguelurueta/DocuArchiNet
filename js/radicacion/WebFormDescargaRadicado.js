$(document).ready(function () {
    $.fn.inicio = function () {
        asigna_ruta();
    }
});
function asigna_ruta() {
    try {
        var Hidden_ruta_archivo = $("#Hidden_ruta_archivo", window.parent.document);
        if (Hidden_ruta_archivo == undefined) {
            alert("Imposible encontrar el control Hidden_ruta_archivo");

        } else {
            document.getElementById("Hidden_ruta_archivo").value = Hidden_ruta_archivo[0].value;
        }
        if (Hidden_ruta_archivo[0].value !== "") {
            document.getElementById("Button_descarga").click();
        }

    }
    catch (err) {
        alert(err.message + " funcion asigna_ruta " + err.message);
    }
}