function prevent_scrol(event, e) {
    try {

        if (e.className == "GridviewScrollItem_line_corte_tr") {
            e.classList.remove("GridviewScrollItem_line_corte_tr");
            e.classList.toggle("GridviewScrollItem_line_corte_tr_scrol");
        } else {
            e.classList.remove("GridviewScrollItem_line_corte_tr_scrol");
            e.classList.toggle("GridviewScrollItem_line_corte_tr");
        }
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_scrol");
    }
}
function prevent_autoriza_xx(event, element) {
    try {

        var fer = $(element).attr("id");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "descarga_xml") {
            $('#Hidden_selec_list').val(fer);
            document.getElementById("Button_dowload_xml").click();
        }

        event.preventDefault();

    }
    catch (err) {
        alert(err.message + " Funcion prevent");
    }
}