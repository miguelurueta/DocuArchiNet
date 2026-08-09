var ITEMS_DATOS_SIST_META_ARCHIVO = [];
var ID_IMAGEN_META_DATO = 0;
var GABINETE_META_DATO = "";
var RADICADO_META_DATO = "";
var ID_TAREA_META_DATO = 0;
var ID_BOTON_META_DATO = "";
var ID_IMAGEN_VIS_WF = 0;
var GABIENTE_VIS_WF = "";
var DAT_ROW_META_DATO;
//AREA META DATOS
function service_crea_interface_registro_meta_dato(id_imagen_, gabinete_, documento_) {
    try {
        $.ajax('../webservice/WebService_Meta_Dato.asmx/Service_parameter_interface_meta_dato', {
            data: "{'id_image':" + "'" + id_imagen_ + "'" + "," + "'gabinete':'" + gabinete_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].ERROR_SERVICE !== "YES") {
                    alert(data.d[0].ERROR_SERVICE);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    ITEMS_DATOS_SIST_META_ARCHIVO = new Array();
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_SIST_META_ARCHIVO.push(v);
                    });
                    ESTADO_EVENT_GENERAL = "out";
                    create_interface_meta_data(documento_);
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert('service_crea_interface_registro_meta_dato  ' + ex.message);
    }
}
function agrega_meta_dato_documento(id_imagen_, gabinete_, radicado_, id_tarea_, modulo_funcion_, firma_digital_, valida_meta_dato_obligatorio_,boton_id_change_) {
    try {
        for (var i = 0; i < ITEMS_DATOS_SIST_META_ARCHIVO.length; i++) {
            if (ITEMS_DATOS_SIST_META_ARCHIVO[i].VALOR_AUTO_POBLADO == "INFO USUARIO" && ITEMS_DATOS_SIST_META_ARCHIVO[i].ESTADO_VISIBLE_METADATO == "1") {
                var ref_element = document.getElementById(ITEMS_DATOS_SIST_META_ARCHIVO[i].nombre_meta_dato);
                if (ref_element) {
                    if (ref_element.value == "" && ITEMS_DATOS_SIST_META_ARCHIVO[i].estado_obliga_torio == "O") {
                        alert("Debe informar el campo (" + ITEMS_DATOS_SIST_META_ARCHIVO[i].nombre_meta_dato + ")");
                        ref_element.focus();
                        ESTADO_EVENT_GENERAL = "out";
                        return true;
                    } else {
                        ITEMS_DATOS_SIST_META_ARCHIVO[i].value = ref_element.value;
                    }

                }
            }
        }
        service_agrega_meta_dato_documento(id_imagen_, gabinete_, radicado_, id_tarea_, modulo_funcion_, firma_digital_, valida_meta_dato_obligatorio_, boton_id_change_);

    }
    catch (ex) {
        alert('agrega_meta_dato_documento  ' + ex.message);
    }
}
function service_agrega_meta_dato_documento(id_imagen_, gabinete_, radicado_, id_tarea_, modulo_funcion_, firma_digital_, valida_meta_dato_obligatorio_, boton_id_change_) {
    try {
        var serialice = JSON.stringify(ITEMS_DATOS_SIST_META_ARCHIVO);
        $.ajax('../webservice/WebService_Meta_Dato.asmx/Service_agrea_meta_dato_documento', {
            data: "{'id_image':" + "'" + id_imagen_ + "'" + "," + "'gabinete':'" + gabinete_ + "','parameter':'" + serialice + "','radicado':'" + radicado_ + "','id_tarea':'" + id_tarea_ +
                "','modulo_funcion':'" + modulo_funcion_ + "','valida_firma_digital':'" + firma_digital_ + "','valida_meta_dato_obligatorio':'" + valida_meta_dato_obligatorio_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].ERROR_SERVICE !== "YES") {
                    alert(data.d[0].ERROR_SERVICE);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    if (boton_id_change_ != "" && data.d[0].ESTADO_FIRMA_DIGITAL == "YES") {
                        cahange_icono_lista(boton_id_change_, 1);
                    } else {
                        cahange_icono_lista(boton_id_change_, 2);
                    }
                    $find("ModalPopupExtender_edition_interface_regitra_meta_dato").hide();
                    ESTADO_EVENT_GENERAL = "out";

                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert('service_agrega_meta_dato_documento  ' + ex.message);
    }
}
function service_agrega_multiple_meta_dato_documento(id_imagen_, gabinete_, radicado_, id_tarea_, modulo_funcion_, firma_digital_, valida_meta_dato_obligatorio_, boton_id_change_) {
    try {
        var serialice = JSON.stringify(ITEMS_DATOS_SIST_META_ARCHIVO);
        $.ajax('../webservice/WebService_Meta_Dato.asmx/Service_agrea_meta_dato_documento', {
            data: "{'id_image':" + "'" + id_imagen_ + "'" + "," + "'gabinete':'" + gabinete_ + "','parameter':'" + serialice + "','radicado':'" + radicado_ + "','id_tarea':'" + id_tarea_ +
                "','modulo_funcion':'" + modulo_funcion_ + "','valida_firma_digital':'" + firma_digital_ + "','valida_meta_dato_obligatorio':'" + valida_meta_dato_obligatorio_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].ERROR_SERVICE !== "YES") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                    ROW_ESTADO_ARCHIVO_NO_FIRMA++;
                    ROW_ERRORES_ARCHIVO_NO_FIRMA.push(data.d[0].ERROR_SERVICE);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    if (boton_id_change_ != "" && data.d[0].ESTADO_FIRMA_DIGITAL == "YES") {
                        cahange_icono_lista(boton_id_change_, 1);
                    } else {
                        cahange_icono_lista(boton_id_change_, 2);
                    }
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                    ESTADO_EVENT_GENERAL = "out";       
                }
            }, error: function (xception, textStatus, errorThrown) {
                
                if (xception.status === 0) {
                    myStopFunction_Event('Not connect: Verify Network.');

                } else if (xception.status == 404) {
                    myStopFunction_Event('Requested page not found [404]');

                } else if (xception.status == 500) {
                    myStopFunction_Event('Internal Server Error [500].' + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    myStopFunction_Event('Requested JSON parse failed.');


                } else if (textStatus === 'timeout') {
                    myStopFunction_Event('Time out error.');


                } else if (textStatus === 'abort') {
                    myStopFunction_Event('Ajax request aborted.');


                } else {
                    myStopFunction_Event('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert('service_agrega_multiple_meta_dato_documento  ' + ex.message);
    }
}
function cahange_icono_lista(boton_id_change_, valor) {
    try {     
        if (document.getElementById(boton_id_change_)) {
            const list = document.getElementById(boton_id_change_);
            var color_i = "";
            while (list.hasChildNodes()) {
                if (color_i == "") {
                    color_i = list.firstChild.style.color;
                }
                list.removeChild(list.firstChild);
            }
            var ihtml = document.createElement("i");
            ihtml.style.color = color_i;
            ihtml.classList.add("fal");
            if (valor == 1) {
                ihtml.classList.add("fa-lock-alt");
                document.getElementById(boton_id_change_).title = "Documento con firma digital y meta datos";
            }
            if (valor == 2) {
                ihtml.classList.add("fa-file-invoice");
                document.getElementById(boton_id_change_).title = "Documento con meta datos";
            }
            document.getElementById(boton_id_change_).appendChild(ihtml);
           
        } else {
            
        }
    }
    catch (ex) {
        
        alert('cahange_icono_lista  ' + ex.message);
    }
}

function Service_Solicita_listar_meta_datos_Archivo(id_imagen_, gabinete_) {
    try {
        var $table = $('#table_meta_row')
        $.ajax('../webservice/WebService_Meta_Dato.asmx/Service_Solicita_listar_meta_datos_Archivo', {
            data: "{'id_image':" + "'" + id_imagen_ + "'" + "," + "'gabinete':'" + gabinete_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].ERROR_SERVICE !== "YES") {
                    $table.bootstrapTable('removeAll');
                    alert(data.d[0].ERROR_SERVICE);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    $table.bootstrapTable('destroy').bootstrapTable({ data: data.d });
                    $find("ModalPopupExtender_edition_interface_consulta_meta_dato").show();
                    auto_zise_popup_consulta_meta_dato();
                    ESTADO_EVENT_GENERAL = "out";

                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });

    }
    catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert('Service_Solicita_listar_meta_datos_Archivo  ' + ex.message);
    }
}
function create_interface_meta_data(documento) {
    try {

        $find("ModalPopupExtender_edition_interface_regitra_meta_dato").show();
        document.getElementById("Panel_interface_regitra_meta_dato").display = true;
        if (documento == "multiplex") {
            document.getElementById("label_interface_regitra_meta_dato").innerHTML = "Firmando multiplex documentos ";
        } else {
            document.getElementById("label_interface_regitra_meta_dato").innerHTML = "Firmando documento (" + documento + ")";
        }
        
        var content_control_meta = document.getElementById("conte_regitra_meta_dato_control");
        if (content_control_meta) {
            while (content_control_meta.hasChildNodes()) {
                content_control_meta.removeChild(content_control_meta.firstChild);
            }
            for (var i = 0; i < ITEMS_DATOS_SIST_META_ARCHIVO.length; i++) {
                if (ITEMS_DATOS_SIST_META_ARCHIVO[i].VALOR_AUTO_POBLADO == "INFO USUARIO" && ITEMS_DATOS_SIST_META_ARCHIVO[i].ESTADO_VISIBLE_METADATO == "1") {
                    var divtml = document.createElement("div");
                    divtml.classList.add("row");
                    divtml.classList.add("p-1");
                    content_control_meta.appendChild(divtml);
                    var divtml_ = document.createElement("div");
                    divtml_.classList.add("col-6");
                    var spntml = document.createElement("span");
                    spntml.classList.add("h6");
                    spntml.classList.add("font-weight-light");
                    //estado_obliga_torio
                    if (ITEMS_DATOS_SIST_META_ARCHIVO[i].nombre_meta_dato != "") {
                        let string_ = ITEMS_DATOS_SIST_META_ARCHIVO[i].nombre_meta_dato;
                        var estado_obligatorio;
                        if (ITEMS_DATOS_SIST_META_ARCHIVO[i].estado_obliga_torio == "O") {
                            estado_obligatorio = " *";
                            spntml.classList.add("text-danger");
                        } else {
                            spntml.classList.add("text-dark");
                            estado_obligatorio = " ";
                        }
                        spntml.innerHTML = string_[0].toUpperCase() + string_.slice(1) + estado_obligatorio;
                    }
                    divtml_.appendChild(spntml);
                    divtml.appendChild(divtml_)
                    divtml_ = document.createElement("div");
                    divtml_.classList.add("col-6");
                    var imputhml = document.createElement("INPUT");
                    imputhml.setAttribute("type", "text");
                    imputhml.id = ITEMS_DATOS_SIST_META_ARCHIVO[i].nombre_meta_dato;
                    var value_text="";
                    if (ITEMS_DATOS_SIST_META_ARCHIVO[i].value !== "NA") {
                        value_text = ITEMS_DATOS_SIST_META_ARCHIVO[i].value;
                    }
                    imputhml.value = value_text;
                    imputhml.classList.add("form-control");
                    divtml_.appendChild(imputhml);
                    divtml.appendChild(divtml_);
                }
            }
        }

    } catch (ex) {
        alert('create_interface_meta_data  ' + ex.message);
    }

}
//----Asigna documentos relacionados a las lista
function asigna_doc_seleccionados_multi_firma(table, clave_sel) {
    try {
        ars_sele = [];
        $('#' + table + ' tr[' + clave_sel + ']').each(function () {
            var nod_value = $(this).attr("idd_wf");
            var node_paren = $(this);
            var id_boton = node_paren[0].cells[0].childNodes[0].childNodes[3].id;
            if (id_boton) {
                ars_sele.push(nod_value + "|" + id_boton);
            } else {
                alert("Imposible encontrar el icono idicativo de firma");
                return true;
            }
        });
    }
    catch (err) {
        alert(err.message + " funcion asigna_doc_seleccionados_multi_firma " + err.message);
    }
}