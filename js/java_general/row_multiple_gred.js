//Se debe agregar esta sentencia en la función donde se caerga el gredview en vb
//scripma.Rows(i).Attributes.Add("onmousedown", "RowClick(this,false,'GridView_val_radicacion','GridviewScrollItem_line_cort_select','GridviewScrollItem_line_cort');")
//paramtros de la función RowClick
//this: envia el row al cual recive el evento
//false: Variable auxialiar
//GridView_val_radicacion: Nombre del gred view
//GridviewScrollItem_line_cort_select : Nombre de la clase que se le asigna al registro cuando se seleciona
//GridviewScrollItem_line_cort: Nombre de la clase que tiene el gredview asignada, este campo se puede estraer del codigo html ejemplo : <RowStyle CssClass="GridviewScrollItem_line_cort" />

var ars_sele = [];//guarda los rows seleccionados
var lastSelectedRow;
var trs; // guarda los trs
var response_sevice_java="";
var result_fuciones;
var contador_registro_gabi;
var id_sel_elemnt_;
var elem = document.getElementById("myBar");
var elment_progres = document.getElementById("myProgress_porcent");
var elment_conta = document.getElementById("myProgress_contador");
var event_name_;
var ROW_ELEMENT_PRENT ;
function fixGridView(tableEl) {
    try {
        var jTbl = $(tableEl);
        if (jTbl.find("tbody>tr>th").length > 0) {
            jTbl.find("tbody").before("<thead><tr></tr></thead>");
            jTbl.find("thead tr").append(jTbl.find("th"));
            jTbl.find("tbody tr:first").remove();

        }
    }
    catch (err) {
        alert(err.message + " Funcion fixGridView");
    }
};
//Función que des selecciona todos los rows con la tecla esc
function table_esc(name_ccs_row, name_css_row_selec,name_selecion) {
    try {
        
        if (window.event.keyCode == 27) {
            des_selectRowsBetweenIndexes(name_ccs_row, name_css_row_selec);
        }
    }
    catch (err) {
        alert(err.message + " Funcion table_esc");
    }
}
//DESACTIVA CHEK
function desactiva_activa_chek(elemen,table) {
    try {
        var value_=true;
        if (elemen.checked == true) {
            value_ = true;
        } else {
            value_ = false;
        }
        $('#' + table + ' .dummychkstyle').each(function () {
            var nod = $(this);
            nod[0].children[0].checked = value_;
        });
    }
    catch (err) {
        alert(err.message + " funcion desactiva_activa_chek " + err.message);
    }
}
//Asigna chebox item selecionado campo hiden     
function asigna_id_seleccionados_cheked_general_hiden(table, hiden) {
    try {
        asigna_id_seleccionados_cheked_general(table);
        if (ars_sele.length > 0) {
            for (var i = 0; i < ars_sele.length; i++) {
                if (i == 0) {
                    document.getElementById(hiden).value = ars_sele[i];
                } else {
                    document.getElementById(hiden).value = document.getElementById(hiden).value + '|' + ars_sele[i];
                }
            }
        }
    } catch (ex) { "Funcion  asigna_id_seleccionados_cheked_general" }
 }
//--Asinga los chek seleccinados a la mtriz general
function asigna_id_seleccionados_cheked_general(table) {
    try {
        var exist;
        ars_sele = [];
        $('#'  + table +  ' .dummychkstyle').each(function () {
            var nod = $(this);
            if (nod[0].children[0].checked == true) {
                var cel = $(this).parent().parent().parent();
                var atri = $(this).parent().parent().parent().attr("id");
                if (atri == undefined) {
                    atri = $(this).parent().parent().attr("id");
                    cel = $(this).parent().parent();
                    exist = verifi_existencia_array(atri);
                    if (exist == "NO") {
                        ars_sele.push(atri);
                    }
                }

                if (atri !== undefined && cel[0].display !== "none") {
                    atri = $(this).parent().parent().parent().attr("id");
                    exist = verifi_existencia_array(atri);
                    if (exist == "NO") {
                        ars_sele.push(atri);
                    }
                }
            }

        });   
    }
    catch (err) {
        alert(err.message + " funcion asigna_usuario_grupos_cheked " + err.message);
    }
}

//-------Inicializa la selección cuando el usuario interactua con el gred
function RowClick(currenttr, lock, name_table, name_css_row_selec, name_ccs_row) {
    try {
        inicializa_tr(name_table);
        if (window.event.ctrlKey) {
            toggleRow(currenttr, name_ccs_row, name_css_row_selec);
            var exist;
            exist = verifi_existencia_array(currenttr.id);
            if (exist == "NO") {
                ars_sele.push(currenttr.id);
            } else {
                var pos = ars_sele.indexOf(currenttr.id);
                ars_sele.splice(pos, 1);
            }
        }
        
        if (window.event.button === 0) {
            if (!window.event.ctrlKey && !window.event.shiftKey) {
                ars_sele = [];
                clearAll(name_ccs_row, name_css_row_selec);
                toggleRow(currenttr, name_ccs_row, name_css_row_selec);
                ars_sele.push(currenttr.id);
            }

            if (window.event.shiftKey) {
                selectRowsBetweenIndexes(name_ccs_row, name_css_row_selec);
            }
        }
    }  catch (err) {
        alert(err.message + " Funcion RowClick");
    }
}
function clearAll(name_ccs_row, name_css_row_selec) {
    try {
    for (var i = 0; i < trs.length; i++) {
        if (trs[i].className == name_ccs_row || trs[i].className == name_css_row_selec) {
            trs[i].className = name_ccs_row;
        }

    }
    } catch (err) {
        alert(err.message + " Funcion clearAll");
    }
}
function inicializa_tr(name_table) {
    try {
        trs = document.getElementById(name_table).tBodies[0].getElementsByTagName('tr');
        if (!trs) {
            alert("Imposible cargar los registros");
        }
    } catch (err) {
        alert(err.message + " Funcion inicializa_tr");
    }
}
function toggleRow(row, name_ccs_row, name_css_row_selec) {
    try {
        row.className = row.className == name_css_row_selec ? '' : name_css_row_selec;
        lastSelectedRow = row;
    }  catch (err) {
        alert(err.message + " Funcion toggleRow");
    }
}
function verifi_existencia_array(id_sleccion) {
    try {
    if (ars_sele.length == 0) {
        return "NO";
    }
    for (var i = 0; i <= ars_sele.length; i++) {
        if (ars_sele[i] == id_sleccion) {
            return "YES";
            break;
        }
    }
    return "NO";
    }  catch (err) {
        alert(err.message + " Funcion verifi_existencia_array");
    }
}
//selecciona todos los indes
function selectRowsBetweenIndexes(name_ccs_row, name_css_row_selec) {
    try {
    ars_sele = [];
    for (var i = 0; i <= trs.length - 1; i++) {
        if (trs[i].className == name_ccs_row || trs[i].className == name_css_row_selec) {
            trs[i].className = name_css_row_selec;
            var exist;
            exist = verifi_existencia_array(trs[i].id);
            if (exist == "NO") {
                ars_sele.push(trs[i].id);
            }
        }

    }
    }  catch (err) {
        alert(err.message + " Funcion selectRowsBetweenIndexes");
    }
}
//quita la seleccion
function des_selectRowsBetweenIndexes(name_ccs_row, name_css_row_selec,name_selector) {
    try {
        ars_sele = [];
        clearAll(name_ccs_row, name_css_row_selec)
        $('#' + name_selector).val(-1);
    } catch (err) {
        alert(err.message + " Funcion des_selectRowsBetweenIndexes");
    }
}
//ZONA DINAMIC WEB TWAIN
function Asigna_matriz_indices_dinamic_timbail_selecte() {
    try {

     } catch (err) {
        alert(err.message + " Funcion Asigna_matriz_indices_dinamic_timbail_selecte ");
    }
}
//TERMINA ZONA DINAMIC WEB TWAIN
//ZONA INSERTA REGISTROS SELLOS SII
//Asigna sellos relacionados


    
//TERMINA ZONA INSERTA REGISTROS SELLOS SII
//inicializa copia archivo
function activa_copia_archivo(name_table) {
    try {
       
        asigna_id_seleccionados_cheked_general(name_table);
        if (ars_sele.length == 0 ) {
            if ($('#hdnEmailID').val() != -1) {
                ars_sele.push($('#hdnEmailID').val());
            }
        }
    } catch (err) {
        alert(err.message + " Funcion activa_copia_archivo");
    }
}
var TIPO_COPIA = 0;
var ID_FLUJO = 0;
var RADICADO = "";
var ROWIDEXPEDIENTE = 0;
var ROWNMBREEXÉDIENTE = "";
var ROWNMBREEXÉDIENTEPREVRELACION = "";
var ROWNOMBREGABINETE = "";
var ROWNAMETABLEBOOT = "";
var ROW_MULTIPLE_ESTATUS_SERVICE = "YES";
var ROW_ESTADO_ARCHIVO_NO_COPY = 0;
var ROW_ESTADO_ARCHIVO_NO_FIRMA = 0
var ROW_ERRORES_ARCHIVO_NO_FIRMA = [];//guarda los rows seleccionados
//------Evento para eliminar un unico registro
function eliminar_file_unico_row_multiple(event,name_table, id_imagen_) {
    try {
       
        var res = confirm("Desea  eliminar el archivo");
        if (res == true) {

        } else {
            return false;
        }
        var index = -1;
        if (ars_sele.length > 0) {
            for (var i = 0; i < ars_sele.length; i++) {
                if (ars_sele[i] === id_imagen_) {
                    index = i;
                    break;
                }
            }
        }
        elimina_regitro_producion_service(name_table, id_imagen_, index);
        event.preventDefault();
    } catch (e) {
        alert("Incosistencia funcion eliminar_file_unico_row_multiple " + e.message);
    }
}
//-------Evento que activa eventos de multiples registros
function event_multiple_row(paramter_additional, name_table, name_service) {
    try {
        var mensaje;
        event_name_ = name_service;
        TIPO_COPIA = 0;
        ROW_ESTADO_ARCHIVO_NO_COPY = 0;
        ROW_ELEMENT_PRENT = $find("ModalPopupExtender_edition_pro_gres_bar");

         if (name_service == "el_service") {          
          asigna_id_seleccionados_cheked_general(name_table);
        }

        //Descarga documentos de un expediente en produción documental
        if (name_service == "dowload_produccion_file_exp") {
            asigna_id_seleccionados_cheked_general(name_table);
        }
        //Asigna documentos para firmar desde workflow documento seleccionado
        if (name_service == "firma_multiple_digital_documento") {
            //asigna_clave_doc_seleccionados_general("GridView_list_documento_relacion_wf", "idd_wf");
            mensaje = "firmar";
        }
        if (name_service == "rotate_dinamic") {
            ars_sele = [];
            //var ars_sele = DWObject.SelectedImagesIndices;
            for (var i = 0; i <= DWObject.SelectedImagesIndices.length - 1; i++) {
                ars_sele.push(DWObject.SelectedImagesIndices[i]);
            }
            mensaje = "rotar";
    }
       
     //Si el usuario nos selecciona el check el sistema asigna el documento
     //seleccioado con la onclklic sobre el registro
        if (ars_sele.length == 0 && name_service == "el_service") {
         if ($('#hdnEmailID').val() != -1) {
             ars_sele.push($('#hdnEmailID').val());
         }
     } 
    
        if (name_service == "el_service") {         
        mensaje = "eliminar";
        }
        if (name_service == "dowload_produccion_file_exp") {
            mensaje = "descargar";
        }
        if (name_service == "cop_file_service_expediente") {
            ars_sele = [];
            if (document.getElementById("Hidden0003").value != "" && document.getElementById("Hidden0006").value == "1") {
                var spl = document.getElementById("Hidden0003").value.split("-");
                for (var i = 0; i <= spl.length - 1; i++) {
                    ars_sele.push(spl[i]);
                }
                ID_FLUJO = document.getElementById("Hidden0005").value;
                TIPO_COPIA = document.getElementById("Hidden0006").value;
                RADICADO = document.getElementById("Hidden0007").value;
                ROWIDEXPEDIENTE = document.getElementById("Hidden0008").value;
                ROW_ESTADO_ARCHIVO_NO_COPY = 0;
                document.getElementById("Hidden0003").value = "";
                mensaje = "copiar";
            } else {
                ESTADO_EVENT_GENERAL = "out";
                alert("Debe seleccionar los archivos a copiar en el flujo de trabajo");
                return false;
                
            }
            
        }
        if (name_service == "cop_file_service_expediente_produccion") {
            ars_sele = [];
            if (document.getElementById("Hidden0003").value != "" && document.getElementById("Hidden0006").value == "1") {
                var spl = document.getElementById("Hidden0003").value.split("-");
                for (var i = 0; i <= spl.length - 1; i++) {
                    ars_sele.push(spl[i]);
                }
                ID_FLUJO = document.getElementById("Hidden0005").value;
                TIPO_COPIA = document.getElementById("Hidden0006").value;
                RADICADO = document.getElementById("Hidden0007").value;
                ROWIDEXPEDIENTE = document.getElementById("Hidden0008").value;
                ROW_ESTADO_ARCHIVO_NO_COPY = 0;
                document.getElementById("Hidden0003").value = "";
                mensaje = "copiar";
            } else {
                ESTADO_EVENT_GENERAL = "out";
                alert("Debe seleccionar los archivos a copiar en el flujo de trabajo");
                return false;

            }

        }
        if (name_service == "cop_file_service") {
            
            mensaje = "copiar";
        }
        if (name_service == "vincula_file_service_gabinete_expediente") {
            ROW_ESTADO_ARCHIVO_NO_COPY = 0;
            mensaje = "vincular";
        }
        if (name_service == "vincula_file_service_expediente" ) {
            if (document.getElementById("Hidden0003").value != "" && document.getElementById("Hidden0006").value =="2") {
                var spl = document.getElementById("Hidden0003").value.split("-");
                for (var i = 0; i <= spl.length - 1; i++) {
                    ars_sele.push(spl[i]);
                }
                ID_FLUJO = document.getElementById("Hidden0005").value;
                TIPO_COPIA = document.getElementById("Hidden0006").value;
                RADICADO = document.getElementById("Hidden0007").value;
                ROWIDEXPEDIENTE = document.getElementById("Hidden0008").value;
                ROW_ESTADO_ARCHIVO_NO_COPY = 0;
                document.getElementById("Hidden0003").value = "";
                mensaje = "vincular";
            } else {
                ESTADO_EVENT_GENERAL = "out";
                alert("Debe seleccionar los archivos a vincular en el flujo de trabajo");
                return false;              
            }      
        }

        if (name_service == "actualiza_indice_batch_wf" || name_service == "actualiza_indice_batch_wf_enlace" || name_service == "actualiza_indice_batch_production") {
            ars_sele = [];
            for (var i = 0; i <= ITEMS_IMAGE_LIST_WF.length - 1; i++) {
                ars_sele.push(ITEMS_IMAGE_LIST_WF[i]);
            }
            if (ars_sele.length == 0) {
                ESTADO_EVENT_GENERAL = "out";
                alert("Debe seleccionar los indices a actualizar");
                return false;
          }
          mensaje = " Actualizar indices de ";
        }
        if (name_service == "actualiza_indice_batch_migracion") {
            mensaje = " Actualizar indices de ";
            ars_sele = [];
            for (let i = 0; i <= paramter_additional.length - 1; i++) {
                ars_sele.push(paramter_additional[i].ID);
            }
        }
        if (name_service == "migra_remplaza_documento_batch_migracion") {
            mensaje = " migrar y remplazar ";
            ars_sele = [];
            for (let i = 0; i <= paramter_additional.length - 1; i++) {
                ars_sele.push(paramter_additional[i].ID);
            }
        }
        //Inicializa la estructura con la identificación de cada imagen para eliminar multiplex imagenes
    if (name_service == "elimina_doc_enlace_wf" || name_service == "elimina_doc_relacionado_wf"
            || name_service == "elimina_doc_relacionado_wf_radicado" || name_service=="elimina_doc_relacionado_consulta_radicado") {
            ars_sele = [];
            for (var i = 0; i <= ITEMS_IMAGE_LIST_WF.length - 1; i++) {
                ars_sele.push(ITEMS_IMAGE_LIST_WF[i]);
            }
        if (ars_sele.length == 0) {
                ESTADO_EVENT_GENERAL = "out";
                alert("Debe seleccionar los documentos a eliminar");
                return false;
            }
            mensaje = " eliminar ";
        }
       
     if (name_service == "vincula_file_service_expediente_auto") {
           
        mensaje = "";
     }
        if (ars_sele.length == 0) {
            ESTADO_EVENT_GENERAL = "out";
            alert("Debe seleccionar los registros");
            return false;
    }  
     if (name_service != "rotate_dinamic") {
            
            Restaura_array();
        }
     if (name_service == "exporta_gabinete_workflow_enlace") {
           
            mensaje = " Exportar a gabinete";
      }
     if (mensaje !== "") {
            var res = confirm("Desea " + mensaje + " (" + ars_sele.length + ")  archivos(s) seleccionado(s)");
     if (res == true) {

     } else {
         ESTADO_EVENT_GENERAL = "out";
           return false;
       }
     }
       
        if (name_service == "firma_multiple_digital_documento") {
            document.getElementById("Label_progres_bar").innerHTML = "Firmando archivos....";
        }
        if (name_service == "rotate_dinamic") {
            document.getElementById("Label_progres_bar").innerHTML = "Rotando archivos....";
        }
        if (name_service == "el_service") {
            document.getElementById("Label_progres_bar").innerHTML = "Eliminado registros....";
        }
        //Descarga documentos desde produción documental
        if (name_service == "dowload_produccion_file_exp") {
            document.getElementById("Label_progres_bar").innerHTML = "Descargando archivos....";
        }
        if (name_service == "cop_file_service" || name_service == "cop_file_service_expediente") {
            document.getElementById("Label_progres_bar").innerHTML = "Copiando archivos....";
        }
        if (name_service == "cop_file_service" || name_service == "cop_file_service_expediente_produccion") {
            document.getElementById("Label_progres_bar").innerHTML = "Copiando archivos....";
        }
        if (name_service == "vincula_file_service_expediente") {
            document.getElementById("Label_progres_bar").innerHTML = "Vinculando archivos....";
        }
        if (name_service == "vincula_file_service_expediente_auto") {
            document.getElementById("Label_progres_bar").innerHTML = "Vinculando automatica de archivos....";
        }
        if (name_service == "vincula_file_service_gabinete_expediente") {
            document.getElementById("Label_progres_bar").innerHTML = "Vinculando archivos....";
        }
       
        if (name_service == "actualiza_indice_batch_wf") {
            document.getElementById("Label_progres_bar").innerHTML = "Actualizando indices....";
        }
        if (name_service == "actualiza_indice_batch_wf_enlace") {
            document.getElementById("Label_progres_bar").innerHTML = "Actualizando indices....";
        }
        if (name_service == "actualiza_indice_batch_production") {
            document.getElementById("Label_progres_bar").innerHTML = "Actualizando indices....";
        }
        if (name_service == "actualiza_indice_batch_migracion") {
            document.getElementById("Label_progres_bar").innerHTML = "Actualizando indices....";
        }
        if (name_service == "migra_remplaza_documento_batch_migracion") {
            document.getElementById("Label_progres_bar").innerHTML = "Migrando y remplazando....";
        }
       
        if (name_service == "exporta_gabinete_workflow_enlace") {
            document.getElementById("Label_progres_bar").innerHTML = "Exportando archivos....";
        }
        if (name_service == "elimina_doc_relacionado_wf") {
            document.getElementById("Label_progres_bar").innerHTML = "Eliminando archivos relacionados....";
        }

        if (name_service == "elimina_doc_enlace_wf") {
            document.getElementById("Label_progres_bar").innerHTML = "Eliminando archivos enlace....";
        }
        if (name_service == "elimina_doc_relacionado_wf_radicado") {
            document.getElementById("Label_progres_bar").innerHTML = "Eliminando archivos enlace radicado....";
        }
        if (name_service == "elimina_doc_relacionado_consulta_radicado") {
            document.getElementById("Label_progres_bar").innerHTML = "Eliminando archivos consulta radicado....";
        }
        document.getElementById("myProgress_porcent").innerHTML = "0%";
        document.getElementById("myProgress_contador").innerHTML = "0";
        document.getElementById("myBar").style.width = 0 + '%';
        document.getElementById("Button_pogres_show").click();
        if (ROW_ELEMENT_PRENT) {
            ROW_ELEMENT_PRENT.show();
        }
        if (name_service !== "rotate_dinamic") {
            ars_sele.sort();
        }
        move(0, ars_sele.length, name_service, name_table, paramter_additional);
   
    } catch (err) {
        alert(err.message + " Funcion event_multiple_row ");
    }
}

//----Función que recorre la matriz de elemenntos ejecuta las acciones
let width_ = 0;
let leng_= 0;
function move(numero_ini, numero_fin, fuction_name, table, paramter_additional) {
    try {
    width_ = numero_ini;
    leng_ = numero_fin;
    elem = document.getElementById("myBar");
    elment_progres = document.getElementById("myProgress_porcent");
    elment_conta = document.getElementById("myProgress_contador");
    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
    id_sel_elemnt_ = setInterval(frame_row_multiple, 10);
        function frame_row_multiple() {
        if (ROW_MULTIPLE_ESTATUS_SERVICE == "yes") {
            if (width_ >= numero_fin) {
                ESTADO_EVENT_GENERAL = "out";
                clearInterval(id_sel_elemnt_);
                ars_sele = [];           
                if (fuction_name == "firma_multiple_digital_documento") {
                    if (ROW_ESTADO_ARCHIVO_NO_FIRMA !== 0) {   
                        var error = "";
                        for (var i = 0; i <= ROW_ERRORES_ARCHIVO_NO_FIRMA.length - 1; i++) {
                            error = error + " (" + i + "." + ROW_ERRORES_ARCHIVO_NO_FIRMA[i] + ")";
                            
                        }
                        alert("No se firmaron (" + ROW_ESTADO_ARCHIVO_NO_FIRMA + ") archivos, por los siguientes errores " + error);
                        ROW_ERRORES_ARCHIVO_NO_FIRMA = [];
                        ROW_ESTADO_ARCHIVO_NO_FIRMA = 0;
                    }
                    document.getElementById('Button_cerrar_pro_gres_bar').click();
                    //ESTADO_EVENT_GENERAL = "out";
                    return true;
                }
                if (ROW_ESTADO_ARCHIVO_NO_COPY !== 0 && TIPO_COPIA == 1) {
                    alert(ROW_ESTADO_ARCHIVO_NO_COPY + " archivo (s) no se copiaron al expediente, debido que ya antes se habria hecho una copia");
                    document.getElementById('Button_cerrar_pro_gres_bar').click();
                    //ESTADO_EVENT_GENERAL = "out";
                    return true;
                }
                if (ROW_ESTADO_ARCHIVO_NO_COPY !== 0 && TIPO_COPIA == 2) {
                    alert(ROW_ESTADO_ARCHIVO_NO_COPY + " archivo (s) no se vincularon al expediente(" + ROWNMBREEXÉDIENTE + ")  debido a que pertenecen al expediente (" + ROWNMBREEXÉDIENTEPREVRELACION + ")");
                    document.getElementById('Button_cerrar_pro_gres_bar').click();
                    return true;
                }
                if (fuction_name == "actualiza_indice_batch_wf" && paramter_additional !="") {
                    $find(paramter_additional).hide();
                }
                if (fuction_name == "actualiza_indice_batch_wf_enlace" && paramter_additional != "") {
                    $find(paramter_additional).hide();
                }
                if (fuction_name == "actualiza_indice_batch_production" && paramter_additional != "") {
                    $find(paramter_additional).hide();
                }
                if (fuction_name == "actualiza_indice_batch_migracion") {
                    $("#modal_actualiza_indice_batch_mig").modal("hide");
                }
                document.getElementById('Button_cerrar_pro_gres_bar').click();
            } else {
                
                //Firma digital y meta datos de documentos
                if (fuction_name == "firma_multiple_digital_documento") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    var spliter = ars_sele[width_].split("|");
                    ID_IMAGEN_META_DATO = spliter[1];
                    GABINETE_META_DATO = spliter[0];
                    RADICADO_META_DATO = spliter[2];
                    ID_TAREA_META_DATO = spliter[5];
                    ID_BOTON_META_DATO = spliter[8];
                    service_agrega_multiple_meta_dato_documento(ID_IMAGEN_META_DATO, GABINETE_META_DATO, RADICADO_META_DATO, ID_TAREA_META_DATO, 1, 1, 1, ID_BOTON_META_DATO);
                }
                //Elimina documentos produción documental
                if (fuction_name == "el_service") {
                    if (ars_sele[width_] !== "") {
                        ROW_MULTIPLE_ESTATUS_SERVICE = "";
                        elimina_regitro_producion_service(table, ars_sele[width_], width_);
                    }
                }
                //Descarga multiplex documentos desde produción documental
                if (fuction_name == "dowload_produccion_file_exp") {
                    if (ars_sele[width_] !== "") {        
                        ROW_MULTIPLE_ESTATUS_SERVICE = "";
                        Service_descarga_documentos_expediente_produccion(ars_sele[width_], ars_sele.length, (width_ + 1));
                    }
                    
                }
                //Copia archivos expediente produccion
                if (fuction_name == "cop_file_service") {
                    if (ars_sele[width_] !== "") {
                        ROW_MULTIPLE_ESTATUS_SERVICE = "";
                        TIPO_COPIA = 0;
                        copia_archivo_producion_service(ars_sele[width_], width_);
                    }              
                }
                //Copia archivos a expedientes de produccion desde workflow
                if (fuction_name == "cop_file_service_expediente_produccion") {
                    if (ars_sele[width_] !== "") {
                        ROW_MULTIPLE_ESTATUS_SERVICE = "";
                        TIPO_COPIA = 1;
                        copia_archivo_producion_workflow_service(ars_sele[width_], width_);
                    }
                }
                //Copia archivos a expedientes en la gestión de expedientes
                if (fuction_name == "cop_file_service_expediente") {
                    if (ars_sele[width_] !== "") {
                        ROW_MULTIPLE_ESTATUS_SERVICE = "";
                        TIPO_COPIA = 1;
                        copia_archivo_expediente_service(ars_sele[width_], width_);
                    }
                }
                //Vinculando archivos a expedientes en la gestión de expedientes
                if (fuction_name == "vincula_file_service_expediente") {
                    if (ars_sele[width_] !== "") {
                        ROW_MULTIPLE_ESTATUS_SERVICE = "";
                        var spliter = ars_sele[width_].split("|");
                        TIPO_COPIA = 2;
                        vincula_archivo_expediente_service(spliter[0], width_, spliter[1]);
                    }
                }
                //Vinculando archivos a expedientes desde el gabinete   
                if (fuction_name == "vincula_file_service_gabinete_expediente") {
                    if (ars_sele[width_] !== "") {
                        ROW_MULTIPLE_ESTATUS_SERVICE = "";
                        TIPO_COPIA = 2;
                        Service_REST_auto_vincula_documento_gabinete_expediente(ars_sele[width_], width_);
                    }
                }
                if (fuction_name == "vincula_file_service_expediente_auto") {
                    if (ars_sele[width_] !== "") {
                        ROW_MULTIPLE_ESTATUS_SERVICE = "";
                        var spliter = ars_sele[width_].split("|");
                        TIPO_COPIA = 2;
                        vincula_archivo_expediente_service(spliter[0], width_, spliter[1]);
                    }
                }
                //Rotando archivos del interfe vista miniatura de dinamic web 
                if (fuction_name == "rotate_dinamic") {
                    if (ars_sele[width_] !== -1) {
                        ROW_MULTIPLE_ESTATUS_SERVICE = "";          
                        rotate_paginas_miniaturas(table, ars_sele[width_],1);
                    }
                }
               
                //Exporta archivos gabinete workflow   
                if (fuction_name == "exporta_gabinete_workflow_enlace") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    var spliter = ars_sele[width_].split("|");
                    Service_exporta_documento_gabinete_workflow_java(spliter[0], table, spliter[1]);
                }
                //Actualiza indice batch wf seleccion
                if (fuction_name == "actualiza_indice_batch_wf") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    Service_actualiza_indice_batch_wf(ITEM_GENERAL_CONTROL_ARRAY_DIFERENT, ars_sele[width_].id_item,1);
                }
                //Actualiza indice batch production
                if (fuction_name == "actualiza_indice_batch_production") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    Service_actualiza_indice_batch_production(ITEM_GENERAL_CONTROL_ARRAY_DIFERENT, ars_sele[width_].id_item, 1);
                }
                //Actualiza indice batch wf seleccion
                if (fuction_name == "actualiza_indice_batch_wf_enlace") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    Service_actualiza_indice_batch_wf_enlace(ITEM_GENERAL_CONTROL_ARRAY_DIFERENT, ars_sele[width_].id_item, 1);
                }
                //Actualiza indice batch migración
                if (fuction_name == "actualiza_indice_batch_migracion") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    Service_REST_actualiza_indice_batch_migracion_gabinete(ITEM_GENERAL_CONTROL_ARRAY_DIFERENT, ars_sele[width_], table);
                }
                //Migra y remplaza documento en batch
                if (fuction_name == "migra_remplaza_documento_batch_migracion") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    Service_REST_migra_formato_remplaza_documento(ars_sele[width_], table);
                }
                //Elimina documentos relacionados wf
                if (fuction_name == "elimina_doc_relacionado_wf") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    Service_elimina_documento_relacionado_workflow(ars_sele[width_].id_item, table);
                }
                //Elimina documentos enlace workflow
                if (fuction_name == "elimina_doc_enlace_wf") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    Service_elimina_documento_enlace_workflow(ars_sele[width_].id_item, table);
                }
                //Elimina documentos relacionados enlace radicado workflow
                if (fuction_name == "elimina_doc_relacionado_wf_radicado") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    Service_elimina_documento_enlace_radicado_workflow(ars_sele[width_].id_item, table);
                }
                //Delete document relation  search  setlled
                if (fuction_name == "elimina_doc_relacionado_consulta_radicado") {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "";
                    Service_elimina_documento_relacionado_consulta_radicado(ars_sele[width_].id_item, table);
                }         
                width_++;
                var porcent = (100 * width_) / (numero_fin + 1);
                if (ars_sele.length == 1) {
                    porcent = 50;
                } else {
                    porcent = Math.round(porcent);
                }
                elem.style.width = porcent + '%';
                elment_progres.innerHTML = porcent + '% ';
                elment_conta.innerHTML = "(" + width_ + ' de ' + numero_fin + ")";
                //Proceso que descarga los documentos y actualiza el label para la producción documental
                if (fuction_name == "dowload_produccion_file_exp") {
                    if (porcent == 100) {
                        document.getElementById("Label_progres_bar").innerHTML = "Espere por favor estamos consolidando la descarga, el proceso puede tardar....";
                    }
                }
            }
        }
        
    }
} catch (err) {
    alert(err.message + " Funcion move");
}
}
function myStopFunction_cancel(mesaje) {
    try {
        alert(mesaje);
        elem.style.width = 0 + '%';
        elment_progres.innerHTML = 0 + '%';
        Restaura_array();
        clearInterval(id_sel_elemnt_);   
        if (ROW_ELEMENT_PRENT) {
            ROW_ELEMENT_PRENT.hide();
        }
        //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
        ESTADO_EVENT_GENERAL = "out";
    } catch (err) {
        if (ROW_ELEMENT_PRENT) {
            ROW_ELEMENT_PRENT.hide();
        }
        ESTADO_EVENT_GENERAL = "out";
    }
}
function myStopFunction(event) {
    try {
        var con = confirm("Desea cancelar el proceso?");
        if (con == true) {
            elem.style.width = 0 + '%';
            elment_progres.innerHTML = 0 + '%';
            Restaura_array();
            clearInterval(id_sel_elemnt_);
            if (ROW_ELEMENT_PRENT) {
                ROW_ELEMENT_PRENT.hide();
            }
            ESTADO_EVENT_GENERAL = "out";
        } else {
            ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
        }
        event.preventDefault();
    } catch (err) {
        if (ROW_ELEMENT_PRENT) {
            ROW_ELEMENT_PRENT.hide();
        }
        alert(err.message + " Funcion myStopFunction");
        event.preventDefault();
        ESTADO_EVENT_GENERAL = "out";
    }
}
function myStopFunction_Event(error) {
    try {
        let cont;
        if (width_ < leng_) {
            con = confirm(error + ", presione aceptar para continuar  el proceso");
            if (con == false) {
                elem.style.width = 0 + '%';
                elment_progres.innerHTML = 0 + '%';
                Restaura_array();
                clearInterval(id_sel_elemnt_);
                if (ROW_ELEMENT_PRENT) {
                    ROW_ELEMENT_PRENT.hide();
                }
                width_ = leng_;
                ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                ESTADO_EVENT_GENERAL = "out";
            } else {
                ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
            }
        } else {
            alert(error);
            ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
            ESTADO_EVENT_GENERAL = "out";
        }   
    } catch (err) {
        if (ROW_ELEMENT_PRENT) {
            ROW_ELEMENT_PRENT.hide();
        }
        alert(err.message + " Funcion myStopFunction_Event");
        ROW_MULTIPLE_ESTATUS_SERVICE = "YES";
        ESTADO_EVENT_GENERAL = "out";
       
    }
}

function Restaura_array() {
    var copi_ars_sele = [];
    for (var i = 0; i <= ars_sele.length - 1; i++) {
        if (ars_sele[i] !== "") {
            copi_ars_sele.push(ars_sele[i])

        }
    }
    ars_sele = copi_ars_sele.slice(0, copi_ars_sele.length);
}

function elimina_regitro_producion_service(name_table, id_imagen_, index) {
    try {
        $.ajax({
            async: true,
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            url: "../webservice/WebServiceDocuarchi.asmx/Get_elimina_registro_producion_service",
            data: "{'id_imagen':'" + id_imagen_ + "'}",
            dataType: "json",
            success: function (data) {
                if (data.d == "YES") {
                    succes_elimina_registro_gred(name_table, id_imagen_, index);
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                } else {
                   
                    myStopFunction_Event(data.d);
                }
            },
            error: function (result) {
                myStopFunction_Event("Error...... " + result);

            }, compelete: function () {
                ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
            }
        })
       
    } catch (e) {
        myStopFunction_Event("Inconsistencia general funcíon elimina_regitro_producion_service " + e.mensaje);
       
    }
}
function resul_elimina_registro_gred(date) {
    response_sevice_java = date;
}
function succes_elimina_registro_gred(name_table, id_imagen_, index) {
    try {
        result_fuciones = eliminar_fila_data_gred_service(name_table, id_imagen_);
        if (result_fuciones !== "YES") {
            alert(result_fuciones);
            return true;
        } else {      
            document.getElementById("Hidden_nureg").value = (document.getElementById("Hidden_nureg").value - 1);
            document.getElementById("titulo_label_grid").innerHTML = document.getElementById("Hidden_nureg").value + " archivo(s) encontrado(s)";
            var exis;
            exis = verifi_existencia_array($('#hdnEmailID').val());
            if (exis == "NO") {
                $('#hdnEmailID').val(-1);
            }
            if (index = -1) {
                ars_sele[index] = "";
            }
           
        }
    } catch (e) {
        alert("Inconsistencia general funcion succes_elimina_registro_gred " + w.mensaje);
    }
}
function eliminar_fila_data_gred_service(gred, id) {
    try {

      $("#" + gred + " tr[id=" + id + "]").remove();
        return 'YES';
    }
    catch (err) {
        return "Inconsistencia general función eliminar_fila_data_gred_service " + err.message;
    }

}
function copia_archivo_producion_service(id_imagen_, width_) {
    $.ajax({    
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceGaExpediente.asmx/Get_copia_archivo_producion_service",
        data: "{'id_imagen':'" + id_imagen_ + "','tipo_copia':'" + TIPO_COPIA + "','id_flujo_wf':'" + ID_FLUJO + "','radicado':'" + RADICADO + "'}",
        dataType: "json",
        success: function (data) {
            if (data.d) {
                var split = data.d.split("|");
                if (split[0] !== "YES") {
                    //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                    myStopFunction_Event(split[0]);
                } else {
                    if (split[1] !== "") {
                        insert_row_producion_documental(data.d);
                    } else {
                        ROW_ESTADO_ARCHIVO_NO_COPY ++;
                    }
                    var exis;
                    exis = verifi_existencia_array($('#hdnEmailID').val());
                    if (exis == "NO") {
                        $('#hdnEmailID').val(-1);
                    }
                    ars_sele[width_] = "";
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
                
            } 
        },
        error: function (result) {
            //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
            myStopFunction_Event("Error...... " + result);         
        }, compelete: function () {
            //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
        }
    });
}
function insert_row_producion_documental(date_campo) {
    try {
        var element_table = document.getElementById("data_grid");
        if (element_table) {
        } else {
            document.getElementById("Button_actualiza").click();
            return true;
        }
        if (date_campo == "") {
            return true;
        }
        var split = date_campo.split("|");
        var conta_td = 0;       
        var element_row = element_table.insertRow(1);
        var element_td = element_row.insertCell(conta_td);
        element_row.id = split[1];
        element_row.style.cursor = "pointer";
        element_row.style.background = "white";
        element_row.style.color = "black";
        element_row.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        element_row.setAttribute("onmouseover", "preven_scrol_onmouseover(event,this)");
        var elment_spa = document.createElement("SPAN");
        elment_spa.classList.add("dummychkstyle");
        var divhtml = document.createElement("div");

        var ihtml = document.createElement("i");
        var element_imput = document.createElement("INPUT");
        element_imput.id = "data_grid_chkSelection_" + split[1];
        element_imput.name = "data_grid$ctl04$chkSelection";
        element_imput.type = "checkbox";
        elment_spa.appendChild(element_imput);
        divhtml.appendChild(elment_spa);

        ihtml.style.color = "white";
        ihtml.classList.add("fal");
       
        if (split.length >= 8) {
            ihtml.classList.add(split[8]);
        } else {
            ihtml.classList.add("fa-file");
        }
        ihtml.classList.add("fa-lg");
        
        var ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-primary");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_scrol(event,this,'vis')");
        ahtml.setAttribute("title", "Visualiza archivo");
        ahtml.setAttribute("idd", split[1]);
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);
        
        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        //ihtml.classList.add("fal", "fa-file-download", "fa-lg");
        ihtml.classList.add("fal");
        ihtml.classList.add("fa-file-download");
        ihtml.classList.add("fa-lg");
        ahtml = document.createElement("a");
        //ahtml.classList.add("btn", "btn-info", "btn-sm");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-info");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_scrol(event,this,'dow')");
        ahtml.setAttribute("title", "Descarga archivo");
        ahtml.setAttribute("idd", split[1]);
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal");
        ihtml.classList.add("fa-file-excel");
        ihtml.classList.add("fa-lg");
        ahtml = document.createElement("a");
      
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-danger");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_scrol(event,this,'del')");
        ahtml.setAttribute("title", "Elimina archivo");
        ahtml.setAttribute("idd", split[1]);
        ahtml.style.marginLeft = "3px";
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        ihtml = document.createElement("i");
        ihtml.style.color = "white";
        ihtml.classList.add("fal");
        if (split.length >= 8) {
            if (split[7] == 1) {
                ihtml.classList.add("fa-lock-alt");
            }
            if (split[7] == 2) {
                ihtml.classList.add("fa-file-invoice");
            }
            if (split[7] !== 2 || split[7] !== 3) {
                ihtml.classList.add("fa-file-signature");
            }
        } else {
            ihtml.classList.add("fa-file-signature");
        }
        ahtml = document.createElement("a");
        ahtml.classList.add("btn");
        ahtml.classList.add("btn-success");
        ahtml.classList.add("btn-sm");
        ahtml.setAttribute("onclick", "prevent_scrol(event,this,'fir');");
        ahtml.setAttribute("title", "Firmar y agregar meta dato");
        if (split[6] == 1) {
            ahtml.setAttribute("title", "Documento con firma digital y meta datos");
        }
        if (split[6] == 2) {
            ahtml.setAttribute("title", "Documento con meta datos");
        }
        var datos_firma = split[5] + "|" + split[9] + "||" + split[9] + "|" + split[2] + "|0";
        ahtml.setAttribute("idd" , split[1]);
        ahtml.setAttribute("id_rad", split[9]);
        ahtml.setAttribute("idd_rad", datos_firma);
        ahtml.setAttribute("tip_event", "firma_doc_selecion_rad");
        ahtml.style.marginLeft = "3px";
        ahtml.id = "d_f_d_" + split[1];
        ahtml.appendChild(ihtml);
        divhtml.appendChild(ahtml);

        divhtml.style.display = "inline-flex";
        element_td.appendChild(divhtml);
        var elemet_length = split.length - 3;
        for (var i = 2; i < elemet_length; i++) {
            conta_td++;
            element_td = element_row.insertCell(conta_td);
            element_td.innerHTML = split[i];
            element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
            element_td.setAttribute("onclick", "prevent_scrol(event,this,'')");
            element_td.setAttribute("ondblclick", "prevent_scrol_ondblclick(event,this)");
        }
        document.getElementById("Hidden_nureg").value = element_table.rows.length - 1;
        document.getElementById("titulo_label_grid").innerHTML = document.getElementById("Hidden_nureg").value + " archivo(s) encontrado(s)";
        
        }    catch (err) {
            alert(err.message + " Funcion insert_row_producion_documental");
       }
}
//WEB SERVICE COPIA VINCULA EXPEDIENTE 
function copia_archivo_expediente_service(id_imagen_, width_) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceGaExpediente.asmx/Get_copia_archivo_expediente_service",
        data: "{'id_imagen':'" + id_imagen_ + "','tipo_copia':'" + TIPO_COPIA + "','id_flujo_wf':'" + ID_FLUJO + "','radicado':'" + RADICADO + "','id_expediente_web':'" + ROWIDEXPEDIENTE + "'}",
        dataType: "json",
        success: function (data) {
            if (data.d) {
                var split = data.d.split("|");
                if (split[0] !== "YES") {
                    myStopFunction_Event(split[0]);
                } else {
                    if (split[1] !== "") {
                        
                    } else {
                        ROW_ESTADO_ARCHIVO_NO_COPY++;
                    } 
                    ars_sele[width_] = "";
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }

            }
        },
        error: function (result) {
            
            myStopFunction_Event("Error...... " + result);
        }, compelete: function () {
            ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
        }
    });
}
function vincula_archivo_expediente_service(id_imagen_, width_, gabinete) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceGaExpediente.asmx/Get_vincula_archivo_expediente_service",
        data: "{'id_imagen':'" + id_imagen_ + "','gabinete':'" + gabinete + "','id_flujo_wf':'" + ID_FLUJO + "','radicado':'" + RADICADO + "','id_expediente_web':'" + ROWIDEXPEDIENTE + "'}",
        dataType: "json",
        success: function (data) {
            if (data.d[0].error_gestion !== "YES") {
                myStopFunction_Event(data.d[0].error_gestion);
            } else {
                if (data.d[0].valor_campos !== "") {
                    ROWNMBREEXÉDIENTEPREVRELACION = data.d[0].nombre_expediente_rlacionado;
                    ROW_ESTADO_ARCHIVO_NO_COPY++;
                }    
                ars_sele[width_] = "";
                ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
            }
        },
        error: function (result) {
            ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
            myStopFunction_Event("Error...... " + result);
        }, compelete: function () {
            ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
        }
    });
}
function Service_Auto_registra_expediente_tramite(id_tipo_doc_entrante_, radicado_, id_tarea_workflow_) {
    try {
        $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_Auto_registra_expediente_tramite', {
            data: "{'id_tipo_doc_entrante':" + "'" + id_tipo_doc_entrante_ + "'" + "," + "'radicado':'" + radicado_ + "','id_tarea_workflow':'" + id_tarea_workflow_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);
                } else {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";             
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event('Service_Auto_registra_expediente_tramite  ' + ex.message);
       
    }
}
//-------Realiza la creación de un expediente
const Service_REST_auto_vincula_documentos_a_expediente_estructura = async (id_tarea_seleccionada) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_auto_vincula_documentos_a_expediente_estructura', {
                data: "{'id_tarea_seleccionada':" + "'" + id_tarea_seleccionada +  "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        ars_sele = [];
                        if (data.d[0].list_image.length > 0) {
                            for (i = 0; i < data.d[0].list_image.length; i++) {
                                ars_sele.push(data.d[0].list_image[i].id_imagen + "|" + data.d[0].list_image[i].gabinete);
                            }
                            ID_FLUJO = data.d[0].id_flujo;
                            TIPO_COPIA = data.d[0].tipo_copia;
                            RADICADO = data.d[0].radicado;
                            ROWIDEXPEDIENTE = data.d[0].id_expediente;
                            ROWNMBREEXÉDIENTE = data.d[0].nombre_expediente;
                        }
                        ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                        if (data.d[0].list_image.length > 0) {
                            event_multiple_row("", '', 'vincula_file_service_expediente_auto');
                        }
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;

}
function Service_auto_vincula_documentos_a_expedientea_auto_link_document_proceedings() {
    ars_sele = [];
    try {
        $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_auto_vincula_documentos_a_expediente', {
            data: "{'dna':" + "'" + 0 + "'"  + "}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);
                    
                } else {
                   
                    if (data.d[0].list_image.length > 0) {
                        for (i = 0; i < data.d[0].list_image.length; i++) {
                            ars_sele.push(data.d[0].list_image[i].id_imagen + "|" + data.d[0].list_image[i].gabinete);
                        }
                        ID_FLUJO = data.d[0].id_flujo;
                        TIPO_COPIA = data.d[0].tipo_copia;
                        RADICADO = data.d[0].radicado;
                        ROWIDEXPEDIENTE = data.d[0].id_expediente;
                        ROWNMBREEXÉDIENTE = data.d[0].nombre_expediente;
                    }
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                    if (data.d[0].list_image.length > 0) {
                        event_multiple_row("", '', 'vincula_file_service_expediente_auto');
                    }
               
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event('Service_auto_vincula_documentos_a_expediente  ' + ex.message);
      
    }
}
function Service_auto_vincula_documentos_seleccionados_a_expediente(para_meter_ca) {
    ars_sele = [];
    var serialice = JSON.stringify(para_meter_ca);
    try {
        $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_activa_auto_vincula_documentos_seleccionados_a_expediente', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);
                } else {

                    if (data.d[0].list_image.length > 0) {
                        for (i = 0; i < data.d[0].list_image.length; i++) {
                            ars_sele.push(data.d[0].list_image[i].id_imagen + "|" + data.d[0].list_image[i].gabinete);
                        }
                        ID_FLUJO = data.d[0].id_flujo;
                        TIPO_COPIA = data.d[0].tipo_copia;
                        RADICADO = data.d[0].radicado;
                        ROWIDEXPEDIENTE = data.d[0].id_expediente;
                        ROWNMBREEXÉDIENTE = data.d[0].nombre_expediente;
                    }
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                    if (data.d[0].list_image.length > 0) {
                        event_multiple_row("", '', 'vincula_file_service_expediente_auto');
                    }
                    
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event('Service_auto_vincula_documentos_seleccionados_a_expediente  ' + ex.message);
        
    }
}
////-------Registra expediente con datos de auto registro gabinete
const Service_REST_auto_registra_gabinete_expediente = async (id_gabinete, gabinete, id_auto_registro, id_imagen, parameter,name_table) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceGaExpediente.asmx/Service_auto_registra_gabinete_expediente', {
                data: "{'id_gabinete':" + "'" + id_gabinete + "','id_imagen':'" + id_imagen + "','id_auto_registro':'" + id_auto_registro + "','gabinete':'" + gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        ars_sele = [];
                        for (i = 0; i < parameter.length; i++) {
                            ars_sele.push(parameter[i].ID);
                        }
                        ROWIDEXPEDIENTE = data.d[0].id_expediente;
                        ROWNAMETABLEBOOT = name_table;
                        ROWNMBREEXÉDIENTE = data.d[0].nombre_expediente_rlacionado;
                        ROWNOMBREGABINETE = gabinete;
                        $("#modal_tipo_tramite_vinculacion").modal("hide");
                        event_multiple_row("", '', 'vincula_file_service_gabinete_expediente');    
                        resolve("YES");
                    }
                }, error: function (xception, textStatus, errorThrown) {

                    if (xception.status === 0) {
                        resolve("Not connect: Verify Network.");


                    } else if (xception.status == 404) {
                        resolve("Requested page not found [404]");


                    } else if (xception.status == 500) {
                        resolve("Internal Server Error [500]." + xception.responseText);


                    } else if (textStatus === 'parsererror') {
                        resolve("Requested JSON parse failed.");


                    } else if (textStatus === 'timeout') {
                        return "Time out error.";


                    } else if (textStatus === 'abort') {
                        resolve("Ajax request aborted.");


                    } else {
                        resolve("Ajax request aborted." + xception.responseText);


                    }
                }
            });
        }
        catch (ex) {
            resolve(ex.message);
        }
    })
    let result = myPromise;
    return result;
}
///////Vincula documento gabinete a expediente
function Service_REST_auto_vincula_documento_gabinete_expediente(id_imagen_, width_) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceGaExpediente.asmx/Service_auto_vincula_documento_gabinete_expediente",
        data: "{'id_imagen':'" + id_imagen_ + "','id_expediente':'" + ROWIDEXPEDIENTE + "','gabinete':'" + ROWNOMBREGABINETE + "'}",
        dataType: "json",
        success: function (data) {
            if (data.d) {  
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);
                } else {
                    if (data.d[0].valor_campos !== "") {
                        ROW_ESTADO_ARCHIVO_NO_COPY++;
                    }
                    ROWNMBREEXÉDIENTEPREVRELACION = data.d[0].nombre_expediente_rlacionado;
                    updateCelByUniqueId(ROWNAMETABLEBOOT, 'EXPEDIENTE', id_imagen_, ROWNMBREEXÉDIENTEPREVRELACION);
                    ars_sele[width_] = "";
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }
        },
        error: function (result) {
            myStopFunction_Event("Error...... " + result);
        }, compelete: function () {
            ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
        }
    });
}
function copia_archivo_producion_workflow_service(id_imagen_, width_) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceGaExpediente.asmx/Get_copia_archivo_expediente_produccion_service",
        data: "{'id_imagen':'" + id_imagen_ + "','tipo_copia':'" + TIPO_COPIA + "','id_flujo_wf':'" + ID_FLUJO + "','radicado':'" + RADICADO + "'}",
        dataType: "json",
        success: function (data) {
            if (data.d) {
                var split = data.d.split("|");
                if (split[0] !== "YES") {
                    
                    myStopFunction_Event(split[0]);
                } else {
                    if (split[1] !== "") {
                        insert_row_producion_documental(data.d);
                    } else {
                        ROW_ESTADO_ARCHIVO_NO_COPY++;
                    }
                    var exis;
                    exis = verifi_existencia_array($('#hdnEmailID').val());
                    if (exis == "NO") {
                        $('#hdnEmailID').val(-1);
                    }
                    ars_sele[width_] = "";
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }

            }
        },
        error: function (result) {
            //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
            myStopFunction_Event("Error...... " + result);
        }, compelete: function () {
            //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
        }
    });
}
//WEB SERVICE SII



//WEB SERVICE ACTUALIZA INDICE
function Service_actualiza_indice_batch_wf(dat_indice, id_parameter_, tipo_indice_actualiza) {
    var serialice = JSON.stringify(dat_indice);
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_actualiza_indice_batch_wf', {
            data: "{" + "'parameter':'" + serialice + "','id_parameter':'" + id_parameter_ + "','tipo_indice_actualiza':'" + tipo_indice_actualiza + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    myStopFunction_Event(data.d);
                   
                } else {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event( ex.message);
        
    }
}
function Service_actualiza_indice_batch_wf_enlace(dat_indice, id_parameter_, tipo_indice_actualiza) {
    var serialice = JSON.stringify(dat_indice);
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_actualiza_indice_batch_wf_enlace', {
            data: "{" + "'parameter':'" + serialice + "','id_parameter':'" + id_parameter_ + "','tipo_indice_actualiza':'" + tipo_indice_actualiza + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    myStopFunction_Event(data.d);
                    //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                } else {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event(ex.message);
    }
}
function Service_actualiza_indice_batch_production(dat_indice, id_parameter_, tipo_indice_actualiza) {
    var serialice = JSON.stringify(dat_indice);
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_actualiza_indice_batch_production', {
            data: "{" + "'parameter':'" + serialice + "','id_parameter':'" + id_parameter_ + "','tipo_indice_actualiza':'" + tipo_indice_actualiza + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    myStopFunction_Event(data.d);
                    //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                } else {
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event(ex.message);

    }
}
function Service_REST_actualiza_indice_batch_migracion_gabinete(dat_indice, id_imagen, name_table) {
    var serialice = JSON.stringify(dat_indice);
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_actualiza_indice_batch_migracion_gabinete', {
            data: "{" + "'parameter':'" + serialice + "','id_imagen':'" + id_imagen + "','name_table':'" + name_table + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);
                } else {
                    UpdaTeRows("table_consulta_migracion", id_imagen, dat_indice);
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event(ex.message);
    }
}
//WEB SERVICE MIGRA REMPLAZA MULTIPLES DOCUMENTOS
function Service_REST_migra_formato_remplaza_documento(id_imagen, gabinete) {   
    try {
        $.ajax('../webservice/WebServiceMigracion.asmx/Service_migra_formato_remplaza_documento', {
            data: "{" + "'id_imagen':'" + id_imagen + "','gabinete':'" + gabinete + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].Error_result !== "YES") {
                    myStopFunction_Event(data.d[0].Error_result);
                } else {      
                    updateCelByUniqueId('table_consulta_migracion', 'ESTENSION', id_imagen, data.d[0].Extension_doc_migrado);
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event(ex.message);
    }
}
//WEB SERVICE DESCARGA IMAGENES EXPEDIENTES
function Service_descarga_documentos_expediente_produccion(id_producion, totalCount, id_Cont) {
    try {
        $.ajax('../webservice/WebServiceProducion.asmx/Service_descarga_documentos_expediente_produccion', {
            data: "{" + "'id_producion':'" + id_producion + "','totalCount':'" + totalCount + "','id_Cont':'" + id_Cont + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].result !== "YES") {
                    myStopFunction_Event(data.d);
                    //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                } else {
                    if (data.d[0].state_propietary !== 1) {
                        myStopFunction_Event('El usuario no tiene permiso para descargar archivo del expediente, debido a que el expediente pertenece a un nivel de otro usuario');
                    } else {
                        if (data.d[0].url_file_zip !== "") {
                            dowload_file(data.d[0].url_file_zip, data.d[0].name_document);
                        }
                    }            
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event(ex.message);
    }
}
//WEB SERVICE ELIMINA DOCUMENTOS RELACIONADOS
function Service_elimina_documento_relacionado_workflow(id_image, table) {
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_elimina_documento_relacionado_workflow', {
            data: "{" + "'parameter':'" + id_image + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);
                    //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                } else {
                    elimina_row_gred_relacionado(table, "id_wf", data.d[0].id_imagen);
                    if (data.d[0].limpia_visor == 1) {
                        let res = Clear_view_wf_select();
                        if (res !== "YES") {
                            myStopFunction_Event(res);
                        }
                    }  
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event(ex.message);  
    }
}
const Clear_view_wf_select = () => {
    try {
        let ref_Panel_tolbar_pdf = document.getElementById("Panel_tolbar_pdf");
        if (ref_Panel_tolbar_pdf) {
            ref_Panel_tolbar_pdf.style.display = "none";
        }
        let ref_panel_content_iframe = document.getElementById("panel_content_iframe");
        if (ref_panel_content_iframe) {
            ref_panel_content_iframe.style.display = "none";
        }
        let ref_panel_content_image_draw = document.getElementById("panel_content_image_draw");
        if (ref_panel_content_image_draw) {
            ref_panel_content_image_draw.style = "none";
        }
        let ref_div_buton = document.getElementById("div_buton");
        if (ref_div_buton) {
            ref_div_buton.style.display = "none";
        }
        let ref_ifrm_visor_ = document.getElementById("ifrm_visor_");
        if (ref_ifrm_visor_) {
            ref_ifrm_visor_.setAttribute("SRC", "../workflow/WebFormiframe.aspx");
        }
        let ref_raw_some_table = document.getElementById("raw_some_table");
        if (ref_raw_some_table) {
            ref_raw_some_table.style.display = "none";
        }
        return "YES";
    } catch (ex) {
        return "error funcion Clear_view_wf_select " + ex.mensaje;
       
    }
}
////WEB SERVICE ELIMINA DOCUMENTOS EN LA VENTANA ENLACE
function Service_elimina_documento_enlace_workflow(id_image, table) {
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_elimina_documento_enlace_workflow', {
            data: "{" + "'parameter':'" + id_image + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);
                    //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                } else {
                    elimina_row_gred_enlace(table, "id_rad", data.d[0].id_imagen);
                    if (data.d[0].limpia_visor == 1) {      
                        let res = clrear_view_wf_enlace();
                        if (res !== "YES") {
                            myStopFunction_Event(res);
                        }
                    }
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event(ex.message);
    }
}
const clrear_view_wf_enlace = () => {
    try {
        let ref_titel_visor = document.getElementById("titel_visor");
        if (ref_titel_visor) {
            ref_titel_visor.innerHTML = "";
        }
        let ref_IframeVisor_ = document.getElementById("IframeVisor_");
        if (ref_IframeVisor_) {
            ref_IframeVisor_.setAttribute("SRC", "");
        }
        let ref_Area_Visor = document.getElementById("Area_Visor");
        if (ref_Area_Visor) {
            ref_Area_Visor.style.display = "none";
        }
        let ref_Are_Digitalizacion = document.getElementById("Are_Digitalizacion");
        if (ref_Are_Digitalizacion) {
            ref_Are_Digitalizacion.style.display = "block";
        }
        return "YES";
    } catch (ex) {
        return "funcion name clrear_view_wf_enlace error : " + ex.mensaje;     
    }
}
////WEB SERVICE ELIMINA DOCUMENTOS EN LA VENTANA ENLACE RADICADOR
function Service_elimina_documento_enlace_radicado_workflow(id_image, table) {
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_elimina_documento_enlace_radicado_workflow', {
            data: "{" + "'parameter':'" + id_image + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);
                    //ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                } else {
                    elimina_row_gred_enlace(table, "id_rad", data.d[0].id_imagen);
                    if (data.d[0].limpia_visor == 1) {
                        let res = clrear_view_wf_enlace();
                        if (res !== "YES") {
                            myStopFunction_Event(res);
                        }
                    }
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event(ex.message);
    }
}
////WEB SERVICE ELIMINA DOCUMENTOS EN LA VENTANA CONSULTA RADICADO
function Service_elimina_documento_relacionado_consulta_radicado(id_image, table) {
    try {
        $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_elimina_documento_relacionado_consulta_radicado', {
            data: "{" + "'parameter':'" + id_image + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    myStopFunction_Event(data.d[0].error_gestion);
                } else {
                    elimina_row_gred_enlace(table, "id_rad", data.d[0].id_imagen);
                    if (data.d[0].limpia_visor == 1) {
                        let res = clrear_view_wf_enlace();
                        if (res !== "YES") {
                            myStopFunction_Event(res);
                        }
                    }
                    ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
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
        myStopFunction_Event(ex.message);
    }
}
