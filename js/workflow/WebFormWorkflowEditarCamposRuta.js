$(document).ready(function () {
    $.fn.inicio = function () {
        $('#data_grid tr[id]').click(function () {
            $('#data_grid tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            fer = $(this).attr("id_nombre");
            $('#HiddenType').val(fer);
            
        });
        $('#data_grid_dos tr[id]').click(function () {
            $('#data_grid_dos tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID_dos').val(fer);

        });
        $('#data_grid tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        $('#data_grid_dos tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        /*$('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });*/
        if (document.getElementById("Hidden_result_detalle").value == "Segunda") {
            document.getElementById("Hidden_result_detalle").value = "inhabilitado";
            //auto_zise_popup_lista_campos_disponibles("0", "1");

        }
        if (document.getElementById("Hidden_resultado_gred").value == "YES") {
            document.getElementById("Hidden_resultado_gred").value = "";
            document.getElementById("Hidden_result_detalle").value = "Segunda";
            //auto_zise_popup_lista_campos_lista();
            //auto_zise_popup_lista_campos_disponibles("1", "1");
        }
        
       
            auto_zise_popup_lista_campos_disponibles("1", "1");
        
    }


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



    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
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
function rezize_event() {
    try {
        auto_zise_popup_lista_campos_disponibles("1", "1");
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
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


function auto_zise_popup_lista_campos_disponibles(value_lista_general, value_lista_usuario) {
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


       
        $('#div_contenedor_drecho').css("height", (espacio_iframe - 5) + "px");
        $('#div_unidades_title').css("height", (document.getElementById("DropDownList_rutas_workflow").clientHeight)  + "px");
        $('#contenedor_opciones_data_gred_uno_general').css("height", (document.getElementById("Button_agregar_campo_disponible").clientHeight + 10) + "px");  
        $('#contenedor_opciones_data_gred_dos_general').css("height", (document.getElementById("Button_eliminar").clientHeight + 10) + "px");
        var total = document.getElementById("div_unidades_title").clientHeight + document.getElementById("contenedor_opciones_data_gred_uno_general").clientHeight  + document.getElementById("contenedor_opciones_data_gred_dos_general").clientHeight + document.getElementById("contenido_titulo_data_grid_title").clientHeight + document.getElementById("contenido_titulo_data_grid_dos_title").clientHeight;   
        var gridheihg = ((espacio_iframe - 5) - total) / 2;
        $('#Contenedorgrid_dos').css("height", (gridheihg - 5) + "px");
        $('#Contenedorgrid').css("height", (gridheihg - 5) + "px");
        $('#Panel_principal').css("height", (gridheihg - (document.getElementById("contenido_titulo_data_grid_title").clientHeight + 10))  + "px");
        $('#Panelactividad_documentos').css("height", (gridheihg - (document.getElementById("contenido_titulo_data_grid_dos_title").clientHeight + 10)) + "px");
       


    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_campos_disponibles " + err.message);
    }
}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function ConfirmMensajeGeneral(mensaje, name_hiden) {
    try {
        var element_hiden = document.getElementById(name_hiden)
        if (element_hiden === null) {
            alert("Imposible encontrar el control " + name_hiden);
            return false;
        }
        var x = "";
        var r = confirm(mensaje);
        if (r == true) {
            x = "1";
        }
        else {
            x = "0";
        }
        document.getElementById(name_hiden).value = x;
    }
    catch (err) {
        alert(err.message + " ConfirmMensajeGeneral");
    }
}
function ConfirmPrompMensajeGeneral(mensaje, name_hiden) {
    try {
        var element_hiden = document.getElementById(name_hiden)
        if (element_hiden === null) {
            alert("Imposible encontrar el control " + name_hiden);
            return false;
        }
        var x = "";
        var r = prompt(mensaje,"");
        if (r == "7894561230.7894561230.") {
            x = "1";
        }
        else {
            x = "0";
        }
        document.getElementById(name_hiden).value = x;
    }
    catch (err) {
        alert(err.message + " ConfirmPrompMensajeGeneral");
    }
}
function posicion_update_pogres(progres) {
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
function eliminar_fila_data_gred(gred, nombre_hiden) {
    try {
        var idex = 0;
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").each(function () {
            idex = $(this)[0].rowIndex;
        })
        $("#" + gred + " tr[id=" + $("#" + nombre_hiden).val() + "]").remove();
        $('#' + nombre_hiden).val("-1");
        //recorre el titulo de la tabla fija

        if (idex == 1) {
            auto_zise_popup_lista_campos_disponibles("0", "1");
        } else {
            if (document.getElementById(gred).clientHeight < document.getElementById(gred + "PanelItemContent").clientHeight) {
                //document.getElementById(gred + "VerticalRail").style.display = "none"; .style.visibility = "hidden";VerticalBar

                if (document.getElementById(gred + "VerticalRail") !== undefined) {
                    document.getElementById(gred + "VerticalRail").style.display = "none";
                    document.getElementById(gred + "VerticalRail").style.visibility = "hidden";
                    document.getElementById(gred + "VerticalBar").style.display = "none";
                    document.getElementById(gred + "VerticalBar").style.visibility = "hidden";
                }

            }
        }
    }
    catch (err) {
        alert(err.message + " Funcion eliminar_fila_data_gred");
    }

}
function pre_actualiza_campos_lista(nombre_grid, id) {
    try {
    var nombre_campo = new Array(3);
    nombre_campo[0] = "Ordena_Tarea"
    nombre_campo[1] = "Prioridad"
    nombre_campo[2] = "Lista_Tarea"
    var valor_campo = new Array(3);
    if (document.getElementById("CheckBox_Ordena_La_lista").checked) {
        valor_campo[0] = "1"
    } else { valor_campo[0] = "0" }
    if (document.getElementById("CheckBox_Campo_Prioridad_Lista").checked) {
        valor_campo[1] = "1"
    } else { valor_campo[1] = "0" }
    if (document.getElementById("CheckBox_Lista_Campo_ruta").checked) {
        valor_campo[2] = "1"
    } else {
        valor_campo[2] = "0"
    }
    actualiza_gre_campo(nombre_grid, id, valor_campo, nombre_campo)

    }
    catch (err) {
        alert(err.message + " Funcion pre_actualiza_campos_lista");
    }
}
function cambia_registro_gred(nombre_grid, id_index_campo_seleccion, index_campo_seleccion, id_index_campo_siguiente, index_campo_siguiente) {
    try {

        
        //Llena la matriz con las celdas de los campos seleccionados
        var iconta = 0;
        valor_campo_selecion = new Array();
        $("#" + nombre_grid + " tr[id=" + id_index_campo_seleccion + "]").each(function () {
           
            for (i = 0; i < this.cells.length; i++) {
                valor_campo_selecion.push(this.cells[i].innerText);
            }         
        })
        //LLena la matriz con las celdas del siguiente campo a cambiar de orden
        valor_campo_siguiente = new Array();
        $("#" + nombre_grid + " tr[id=" + id_index_campo_siguiente + "]").each(function () {

            for (i = 0; i < this.cells.length; i++) {
                valor_campo_siguiente.push(this.cells[i].innerText);
            }

        })
        //Remplaza los datos en registro seleccionado con los datos del siguiente registro
        $("#" + nombre_grid + " tr[id=" + id_index_campo_seleccion + "]").each(function () {          
            for (i = 0; i < this.cells.length; i++) {
                this.cells[i].innerText = valor_campo_siguiente[i];
            }
        })
        //Remplaza los datos del siguiente registro con los datos del registro seleccionado
        $("#" + nombre_grid + " tr[id=" + id_index_campo_siguiente + "]").each(function () {          
            for (i = 0; i < this.cells.length; i++) {
                this.cells[i].innerText = valor_campo_selecion[i];
            }
        })
        //Remplaza el id del registro seleccionado temporalmente
        $("#" + nombre_grid + " tr[id=" + id_index_campo_seleccion + "]").each(function () {           
            this.id = "-1";
        })
        //Remplaza el siguiente registro con el id del registro seleccionado
        $("#" + nombre_grid + " tr[id=" + id_index_campo_siguiente + "]").each(function () {
            this.id = id_index_campo_seleccion;           
        })
        //Remplaza el id del registro seleccionado con el id del campo siguiente
        $("#" + nombre_grid + " tr[id=" + "-1" + "]").each(function () {
            this.id = id_index_campo_siguiente;
        })
        //Verifica si es el primer registro de la tabla
        var idex_registro;
        $("#" + nombre_grid + " tr[id=" + id_index_campo_seleccion + "]").each(function () {
            idex_registro = $(this)[0].rowIndex;
        })
        if (idex_registro == 1) {
            auto_zise_popup_lista_campos_disponibles("0", "1");          
        }
        if (idex_registro == 2) {
            auto_zise_popup_lista_campos_disponibles("0", "1");
        }
        $("#" + nombre_grid + ' tr[id]').css({ "background": "White", "color": "Black" });
        $("#" + nombre_grid + " tr[id=" + id_index_campo_seleccion + "]").css({ "background-color": "#E7EDF5", "color": "Black" });
        $('#hdnEmailID_dos').val(id_index_campo_seleccion);
        }
    catch (err) {
        alert(err.message + " Funcion cambia_registro_gred ");
    }
}
function actualiza_gre_campo(nombre_grid, id, valor_campo, nombre_campo) {
    try {
        $("#" + nombre_grid + " tr[id=" + id + "]").each(function () {
            for (var i = 0; i <= nombre_campo.length - 1 ; i++) {
                var idex = -1;
                var name = nombre_campo[i];
                idex = colum_index(name, nombre_grid);
                if (idex != -1) {
                    if (valor_campo == "") {
                        var sas = $(this)[0].cells[idex];
                        //var nodetext = document.getElementById("ocultop");
                        var trfirst = $('#' + nombre_grid + ' tr:first').next();
                        if (sas.childElementCount == 0) {
                            $(this)[0].cells[idex].innerText = "\u00a0";
                            //nodetext.innerText = "\u00a0";
                        }
                        if (sas.childElementCount >= 1) {
                            sas.firstChild.innerHTML = "&nbsp;";
                            //nodetext.innerText = "\u00a0";
                        }
                    }
                    if (valor_campo !== "") {
                        var trfirst = $('#' + nombre_grid + ' tr:first').next();
                        var sas = $(this)[0].cells[idex];
                        if (sas.childElementCount <= 0) {
                            var clinet_widt_old = sas.firstChild.clientWidth;
                            var div_element = document.createElement("div");
                            var p_element = document.createElement("p");
                            p_element.innerHTML = valor_campo;
                            div_element.appendChild(p_element);
                            $(this)[0].appendChild(div_element);
                            $(this)[0].cells[idex].innerText = valor_campo[i];
                            var clinet_widt_new = p_element.clientWidth;
                            //verifcar que la fila uno tenga childs
                            if (($(this)[0].cells[idex].clientWidth - 10) > trfirst[0].cells[idex].firstChild.clientWidth) {
                                trfirst[0].cells[idex].firstChild.style.width = $(this)[0].cells[idex].clientWidth + "px";
                                var x2 = $('#' + nombre_grid + 'Copy th');
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
                            p_element.innerHTML = valor_campo;
                            div_element.appendChild(p_element);
                            sas.firstChild.innerHTML = valor_campo[i];
                            sas.appendChild(div_element);
                            var clinet_widt_new = p_element.clientWidth;
                            if (clinet_widt_new > trfirst[0].cells[idex].firstChild.clientWidth) {
                                if (trfirst[0].cells[idex].firstChild.childElementCount > 0) {
                                    trfirst[0].cells[idex].firstChild[0].style.width = clinet_widt_new + "px";
                                }
                                else {
                                    trfirst[0].cells[idex].firstChild.style.width = clinet_widt_new + "px";
                                }
                                var x2 = $('#' + +nombre_grid + 'Copy th');
                                x2[idex].firstChild.style.width = clinet_widt_new + "px";
                                x2[idex].clientWidth = clinet_widt_new;
                            }
                            sas.removeChild(div_element);
                        }

                    }

                }
            }

        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_gre_campo");
    }
}
function seter_gre_campo(nombre_grid, id, valor_campo, nombre_campo) {
    try {
        var k=0;
        $("#" + nombre_grid + " tr").each(function () {
           
            for (var i = 0; i <= nombre_campo.length - 1 ; i++) {
                var idex = -1;
                var name = nombre_campo[i];
                idex = colum_index(name, nombre_grid);
                k = k + 1;
                if (idex !== -1 && k > 1) {
                    if (valor_campo == "") {
                        var sas = $(this)[0].cells[idex];
                        //var nodetext = document.getElementById("ocultop");
                        //var trfirst = $('#' + nombre_grid + ' tr:first').next();
                        if (sas.childElementCount == 0) {
                            $(this)[0].cells[idex].innerText = "\u00a0";
                            //k = k + 1;
                            //nodetext.innerText = "\u00a0";
                        }
                        if (sas.childElementCount >= 1) {
                            sas.firstChild.innerHTML = "&nbsp;";
                            //k = k + 1;
                            //nodetext.innerText = "\u00a0";
                        }
                        
                    }
                    if (valor_campo !== "") {
                        var trfirst = $('#' + nombre_grid + ' tr:first').next();
                        var sas = $(this)[0].cells[idex];
                        if (sas.childElementCount <= 0) {
                            var clinet_widt_old = sas.firstChild.clientWidth;
                            var div_element = document.createElement("div");
                            var p_element = document.createElement("p");
                            p_element.innerHTML = valor_campo;
                            div_element.appendChild(p_element);
                            $(this)[0].appendChild(div_element);
                            $(this)[0].cells[idex].innerText = valor_campo[i];
                            var clinet_widt_new = p_element.clientWidth;
                            //verifcar que la fila uno tenga childs
                            if (($(this)[0].cells[idex].clientWidth - 10) > trfirst[0].cells[idex].firstChild.clientWidth) {
                                trfirst[0].cells[idex].firstChild.style.width = $(this)[0].cells[idex].clientWidth + "px";
                                var x2 = $('#' + nombre_grid + 'Copy th');
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
                            p_element.innerHTML = valor_campo;
                            div_element.appendChild(p_element);
                            sas.firstChild.innerHTML = valor_campo[i];
                            sas.appendChild(div_element);
                            var clinet_widt_new = p_element.clientWidth;
                            if (clinet_widt_new > trfirst[0].cells[idex].firstChild.clientWidth) {
                                if (trfirst[0].cells[idex].firstChild.childElementCount > 0) {
                                    trfirst[0].cells[idex].firstChild[0].style.width = clinet_widt_new + "px";
                                }
                                else {
                                    trfirst[0].cells[idex].firstChild.style.width = clinet_widt_new + "px";
                                }
                                var x2 = $('#' + +nombre_grid + 'Copy th');
                                x2[idex].firstChild.style.width = clinet_widt_new + "px";
                                x2[idex].clientWidth = clinet_widt_new;
                            }
                            sas.removeChild(div_element);
                        }

                    }

                }
            }

        })
        return true;
    }
    catch (err) {
        alert(err.message + " Funcion seter_gre_campo");
    }
}
function colum_index(colum_name, nombre_grid) {
    try {
        var x = $('#' + nombre_grid + ' th');
        var txt = "";
        var i;
        for (i = 0; i < x.length; i++) {
            if (x[i].innerText.toUpperCase() == colum_name.toUpperCase()) {

                return i;
            }

        }
        return -1;
    }
    catch (err) {
        alert(err.message + " funcion colum_index " + err.message);
    }
}