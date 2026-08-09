$(document).ready(function () {

    $.fn.cligred = function () {          
        auto_zise_anotacion(0);
        auto_size_gredview();
        var RowID = $('#hdnEmailID').val();
        var datos = RowID.split("|");
        var arc1 = datos[0];
        if (RowID != "0") {
            $('#GridViewlista tr[id=' + RowID + ']').css({ "background-color": "#E7EDF5", "color": "Black" });
        }
        $('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('#GridViewlista tr[id]').click(function () {
            $('#GridViewlista tr[id]').css({ "background-color": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer.toString());
            
        });
        $('#GridViewlista tr[id]').dblclick(function () {
            $('#GridViewlista tr[id]').css({ "background-color": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer.toString());
            var boton = $('#Buttonclidatos');
            boton.click();
        });
       
    }
    
    $('#ButtonActualizar').click(function () {
        var RowID = $('#hdnEmailID').val();
        var datos = RowID.split("|");
        var arc1 = datos[0];
        var arc2 = datos[2];
        if (RowID != "0") {
            $('#GridViewlista tr[id]').each(function () {
                var valores = $(this).attr("id");
                var nuevodato = arc1 + "|" + $('#TextBoxdatos').val() + "|" + arc2;
                if (valores == RowID) {
                    $(this).attr("id", nuevodato);
                }
            });
        }
    });

})
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
function rezize_event() {
    try {
        auto_zise_anotacion(0);
        auto_size_gredview();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
function event_click(e) {
    try {
        var k = e.currentTarget.value;
        var d = document.getElementById(k);
        d.click();
        e.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion event_click");
    }
}
function prevent_event(event, element) {
    try {
        var fer = $(element).attr("idd");
        var tip_event = $(element).attr("tip_event");
        if (tip_event == "eli_nota") {
            var r = confirm("Desea eliminar la nota");
            if (r == false) {
                return false;
            }
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("ButtonEliminar").click();
        }
        if (tip_event == "ver_nota") {
            $('#hdnEmailID_VAL').val(fer);
            document.getElementById("Buttonclidatos").click();
        }
        event.preventDefault();
        element.focus();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_event");
    }
}
function ShowModalPopup(modalPopupId_, name_panel, zIndex) {
    try {
        var modalPopupId = document.getElementById(modalPopupId_);
        var name_panel_ = document.getElementById(name_panel);
        var modalPopupBehavior = modalPopupId;
        zIndex = typeof (zIndex) != 'undefined' ? zIndex : null;
        if (zIndex != null) {
            modalPopupBehavior.style.zIndex = zIndex;
            name_panel_.style.zIndex = zIndex + 1;
        }
    }
    catch (ex) {
        alert('Exception in ShowModalPopup: ' + ex.message);
    }
}
//MUEVE EL SCCROL AL ID SELECCIONADO
function eliminar_fila_data_gred(gred) {
    try {
        $("#" + gred + " tr[id=" + $("#hdnEmailID").val() + "]").remove();
        $('#hdnEmailID').val("-1");

    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred");
    }

}
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    try {
        if ($("#" + data_grid + " td").children.length == 0 && $("#" + data_grid + " tr:visible").length == 0) {
            return true;
        }
        if ($("#" + HiddenSeleccion).val() != "-1" && $("#" + HiddenSeleccion).val() != "0") {
            var scrollableDiv = $("#" + data_grid).parent();
            var index = $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]");
            //limpia todos los seleccionados
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Red" });
            $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
                if (index[0].rowIndex > 1) {
                    //$(scrollableDiv).scrollTop(70);
                    $(scrollableDiv).scrollTop(($(this).offset().top));
                    return true;
                }

            });
        }
    }
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_data_gred");
    }
}
function ConfirmMensaje(mensaje) {
    var x;
    var r = confirm(mensaje);
    if (r == true) {
        x = "0";
    }
    else {
        x = "1";
    }
    document.getElementById("HiddenPROMP").value = x;


}

function posicion_update_pogres(progres) {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

            }
        }
        var prog = document.getElementById(progres);
        var widtop = (espacio_iframe / 2);
        var heitop = (with_frame / 2);
        prog.style.top = widtop + "px";
        prog.style.left = heitop + "px";
        prog.style.zIndex = "1000009";
        $("#progres_bar").css("display", "block");
        prog.style.position = "fixed";
    }
    catch (err) {
        alert(err.message + " funcion posicion_update_pogres " + err.message);
    }

}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function labe_texto_modal_nota(nota) {
    document.getElementById("Label_nota_respuesta").innerText = nota;
}
function prevent_scrol(event, e) {
    try {

        if (e.className == "GridviewScrollItem_line_cort_tr_flex") {
            e.classList.remove("GridviewScrollItem_line_cort_tr_flex");
            e.classList.toggle("GridviewScrollItem_line_corte_tr_flex_scrol");
        } else {
            e.classList.remove("GridviewScrollItem_line_corte_tr_flex_scrol");
            e.classList.toggle("GridviewScrollItem_line_cort_tr_flex");
        }
        event.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion prevent_scrol");
    }
}

function auto_zise_nota_tarea(porcentaje) {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

            }
        }
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 20) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#Panel_nota_respuesta').css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
        $('#modal_content_nota_respuesta').css("height", (heig_porcent - 10) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_nota_respuesta').css("height", (document.getElementById("modal_content_nota_respuesta").clientHeight - (document.getElementById("divcabecer2_radica_documento").clientHeight + document.getElementById("content_boton_nota").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#TextBoxdatos').css("height", (document.getElementById("contenido_procesa_nota_respuesta").clientHeight - 5) + "px");

    }
    catch (err) {
        alert(err.message + " funcion auto_zise_nota_tarea " + err.message);
    }
}

function auto_size_gredview() {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

            }
        }   
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#modal_content_anotacion').css("height", (heig_porcent - 1) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        $('#contenido_procesa_content_anotacion').css("height", (document.getElementById("modal_content_anotacion").clientHeight - (document.getElementById("content_boton").clientHeight)) + "px");
        //Para los modal que contiene gred
        $('#contenido_gred_anotacion').css("height", (document.getElementById("contenido_procesa_content_anotacion").clientHeight - document.getElementById("contenido_titulo").clientHeight) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_size_gredview " + err.message);
    }
}

function actualiza_gre_campos_dinamicos() {
    try {
        
        var hidencamposaleas = document.getElementById("hidden_campos_dinamicos_aleas").value;
        var hidenvalores = document.getElementById("hidden_valore_campos").value;
        var spli_campos = hidencamposaleas.split("|");
        var valores = hidenvalores.split("|||||");
        $("#GridViewlista tr[id=" + $("#hdnEmailID").val() + "]").each(function () {
            var idex = -1;
            for (i = 0; i <= (spli_campos.length - 1) ; i++) {
               
                var name = spli_campos[i];
                if (valores[i] != undefined) {
                    idex = colum_index(name);
                    if (idex != -1) {
                        if (valores[i] == "") {
                            var sas = $(this)[0].cells[idex];
                            //var nodetext = document.getElementById("ocultop");
                            var trfirst = $('#GridViewlista tr:first').next();
                            if (sas.childElementCount == 0) {
                                $(this)[0].cells[idex].innerText = "\u00a0";
                                //nodetext.innerText = "\u00a0";
                            }
                            if (sas.childElementCount >= 1) {
                                sas.firstChild.innerHTML = "&nbsp;";
                                //nodetext.innerText = "\u00a0";
                            }
                        }
                        if (valores[i] !== "") {
                            var trfirst = $('#GridViewlista tr:first').next();
                            var sas = $(this)[0].cells[idex];
                            if (sas.childElementCount <= 0) {
                                var clinet_widt_old = sas.firstChild.clientWidth;
                                var div_element = document.createElement("div");
                                var p_element = document.createElement("p");
                                p_element.innerHTML = valores[i];
                                div_element.appendChild(p_element);
                                $(this)[0].appendChild(div_element);
                                $(this)[0].cells[idex].innerText = valores[i];
                                var clinet_widt_new = p_element.clientWidth;
                                //verifcar que la fila uno tenga childs
                                if (($(this)[0].cells[idex].clientWidth - 10) > trfirst[0].cells[idex].firstChild.clientWidth) {
                                    trfirst[0].cells[idex].firstChild.style.width = $(this)[0].cells[idex].clientWidth + "px";
                                    var x2 = $('#GridViewlistaCopy th');
                                    x2[idex].firstChild.style.width = ($(this)[0].cells[idex].clientWidth - 10) + "px";
                                    x2[idex].clientWidth = ($(this)[0].cells[idex].clientWidth - 10);
                                }
                                $(this)[0].removeChild(div_element);
                            }
                            //Opcion para actualizar la primera fila de la tabla que se le agrega un div, cuado trae mas de un elemento
                            if (sas.childElementCount >= 1) {
                                var clinet_widt_old = sas.firstChild.clientWidth;
                                var div_element = document.createElement("div");
                                var p_element = document.createElement("p");
                                p_element.innerHTML = valores[i];
                                div_element.appendChild(p_element);
                                sas.firstChild.innerHTML = valores[i];
                                sas.appendChild(div_element);
                                var clinet_widt_new = p_element.clientWidth;
                                if (clinet_widt_new > trfirst[0].cells[idex].firstChild.clientWidth) {
                                    if (trfirst[0].cells[idex].firstChild.childElementCount > 0) {
                                        trfirst[0].cells[idex].firstChild[0].style.width = clinet_widt_new + "px";
                                    }
                                    else {
                                        trfirst[0].cells[idex].firstChild.style.width = clinet_widt_new + "px";
                                    }
                                    var x2 = $('#GridViewlistaCopy th');
                                    x2[idex].firstChild.style.width = clinet_widt_new + "px";
                                    x2[idex].clientWidth = clinet_widt_new;
                                }
                                sas.removeChild(div_element);
                            }

                        }

                    }
                }
            }
        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campos_dinamicos");
    }
}

//Retorna el idex de una columna en una tabla
function colum_index(colum_name) {

    var x = $('#GridViewlista th');
    var txt = "";
    var i;
    for (i = 0; i < x.length; i++) {
        if (x[i].innerText.toUpperCase() == colum_name.toUpperCase()) {

            return i;
        }

    }
    return -1;
}