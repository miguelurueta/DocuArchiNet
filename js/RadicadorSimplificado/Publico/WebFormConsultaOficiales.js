function activa_retroceso_principal() {
    try {
        if (window.parent.document.getElementById("element_a_inicio")) {
            window.parent.document.getElementById("element_a_inicio").click();
        }

    }
    catch (err) {
        alert(err.message + " Funcion activa_retroceso_pagina");
    }
}