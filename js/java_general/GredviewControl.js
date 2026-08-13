//Agrega item de documento relacionado en workflow
function insert_row_documento_relacionado(date_campo, selecion, activa_registro_versionado) {
    try {
        var element_atrit;
        var element_sel;
        if (selecion == "wf") {
            element_sel = "GridView_list_documento_relacion_wf";
            element_atrit = "_wf";
        } else {
            selecion = "rad";
            element_sel = "GridView_list_documento_relacion";
            element_atrit = "_rad";
        }
        var element_table = document.getElementById(element_sel);
        if (element_table) {
        } else {
            return true;
        }
        if (date_campo == "") {
            return true;
        }
        let split = date_campo.split("|");
        let IconoFle = "";
        if (split.length >= 8) {
            IconoFle = "fal " + split[7];
        } else {
            IconoFle = "fal fa-file"
        }
        let iconFirma = "";
        switch (split[6]) {
            case 1:
                iconFirma = "fal fa-lock-alt";
                break;
            case 2:
                iconFirma = "fal fa-lock-alt";
                break;
            default:
                iconFirma = "fal fa-file-signature";
        }
        //--------Representa la clase del icono identificador de la imagen
        let ClaseIcono = "font-weight-light  f_d_v_d_a_" + split[1];
        let ClaseIconoEnlace = "font-weight-light  r_d_v_d_" + split[1];
        //--------Representa el tributo de workflow para el evento firma digital  r_d_v_d_
        let AtribIddWf = "|f_d_v_d_a_" + split[1];
        let AtribIddRA = "|r_d_v_d_" + split[1];
        let IdEliminar = element_sel + "d_e_d_a_" + split[1];
        let IdCambiarTipologia = element_sel + "d_ch_t_d_a_" + split[1];
        let IdFirmaDigital = element_sel + "d_s_d_f_a_" + split[1];
        let IdVersiones = element_sel + "d_l_v_d_a_" + split[1];
        let IdRemplazo = element_sel + "d_r_v_d_a_" + split[1];
        let IdIconoTable = element_sel + "_" + selecion + "d_v_i_d_a_" + split[1];
        let html_menu;
        if (selecion == "wf") {
            html_menu = [
                '<div class="row pl-1 w-100" style="display:inline-flex; margin-right:0px">',
                '<div class="w-100 col-10 pl-2 row" style="margin-right:0px;" onclick="prevent(event,this);" title="Ver documento" id_wf="' + split[1] + ' " idd_wf="' + date_campo + ' " tip_event=vis_doc_selecion_' + selecion + '>',
                '<div class="col-2 pt-2 ">',
                '<a id="' + IdIconoTable + '" class="' + ClaseIcono + '" style="color: #0062cc;" aria-hidden="true" focusable="false"> ',
                '<i class="' + IconoFle  + '" style="color:#0062cc;"></i>',
                '</a>',
                '</div>',
                '<div class="col-10 pl-1 pt-1">',
                '<spam class="pl-0 GridviewSpanOverFlow" style="color:black;">' + split[4] + '',
                '</spam>',
                '</div>',
                '</div>',
                '<div class="col-2 p-0 nav-item dropdown active">',
                '<a class="nav-link dropdown-toggle justify-content-start btn-lg mt-1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" href="#"></a>',
                '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Eliminar documento" id="' + IdEliminar + '"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="elim_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false"  > <i style="color: #0062cc;" class="fal fa-trash-alt"></i> <spam class="pl-1 font-weight-light"> Eliminar documento</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Cambiar tipología documental" id="' + IdCambiarTipologia + '"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="cambia_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false"  > <i style="color: #0062cc;" class="fal fa-file-edit"></i> <spam class="pl-1 font-weight-light"> Cambiar tipología</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Firma digital" id="' + IdFirmaDigital + '"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + AtribIddWf  + '" tip_event="firma_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="' + iconFirma + '"></i> <spam class="pl-1 font-weight-light"> Firma digital</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Versiones del documento" id="' + IdVersiones + '"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="lista_ver_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="far fa-folder-open"></i> <spam class="pl-1 font-weight-light"> Versiones del documento</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Remplazar documento" id="' + IdRemplazo + '"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + AtribIddWf + '" tip_event="remplaza_ver_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="far fa-clone"></i> <spam class="pl-1 font-weight-light"> Remplazar documento</spam></a>',
                '</div>',
                '</div>',
                '</div>'
            ].join('')
        } else {
            let option_activa_registro_versionado = "";
            if (activa_registro_versionado == 1) {
                option_activa_registro_versionado = '<a class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Versiones del documento" id="' + IdVersiones + '"' +
                    'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="lista_ver_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="far fa-folder-open"></i> <spam class="pl-1 font-weight-light"> Versiones del documento</spam></a>' +
                    '<a class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Remplazar documento" id="' + IdRemplazo + '"' +
                    'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + AtribIddRA + '" tip_event="remplaza_ver_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="far fa-clone"></i> <spam class="pl-1 font-weight-light"> Remplazar documento</spam></a>';           
            }   
            html_menu = [
                '<div class="row pl-1 w-100" style="display:inline-flex; margin-right:0px">',
                '<div class="w-100 col-10 pl-2 row" style="margin-right:0px;" onclick="prevent(event,this);" title="Ver documento" id_rad="' + split[1] + ' " idd_rad="' + date_campo + ' " tip_event=vis_doc_selecion_' + selecion + '>',
                '<div class="col-2 pt-2 ">',
                '<a id="' + IdIconoTable + '" class="' + ClaseIconoEnlace + '" style="color: #0062cc;" aria-hidden="true" focusable="false"> ',
                '<i class="' + IconoFle + '" style="color:#0062cc;"></i>',
                '</a>',
                '</div>',
                '<div class="col-10 pl-1 pt-1">',
                '<spam class="pl-0 GridviewSpanOverFlow" style="color:black;">' + split[4] + '',
                '</spam>',
                '</div>',
                '</div>',
                '<div class="col-2 p-0 nav-item dropdown active">',
                '<a class="nav-link dropdown-toggle justify-content-start btn-lg mt-1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" href="#"></a>',
                '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink">',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Eliminar documento"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="elim_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" data-prefix="fal" data-icon="trash-alt" role="img" > <i style="color: #0062cc;" class="fal fa-trash-alt"></i> <spam class="pl-1 font-weight-light"> Eliminar documento</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Cambiar tipología documental"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + '" tip_event="cambia_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" data-prefix="fal" data-icon="file-edit" role="img" > <i style="color: #0062cc;" class="fal fa-file-edit"></i> <spam class="pl-1 font-weight-light"> Cambiar tipología</spam></a>',
                '<a  class="dropdown-item pl-3 font-weight-light" onclick="prevent(event,this);" title="Firmar digital"',
                'id' + element_atrit + '="' + split[1] + '" idd' + element_atrit + '="' + date_campo + AtribIddRA + '" tip_event="firma_doc_selecion_' + selecion + '" class="dropdown-item font-weight-light" style="margin-left:1px; aria-hidden="true" focusable="false" > <i style="color: #0062cc;" class="' + iconFirma + '"></i> <spam class="pl-1 font-weight-light"> Firma digital</spam></a>',
                option_activa_registro_versionado,
                '</div>',
                '</div>',
                '</div>'
            ].join('')
        }
        var conta_td = 0;
        //Agrega el row a la tabla
        var element_row = element_table.insertRow(element_table.rows.length);
        element_row.setAttribute("class" , "GridviewRow");
        element_row.setAttribute("id" + element_atrit, split[1]);
        element_row.setAttribute("idd" + element_atrit, date_campo);
        element_row.style.cursor = "pointer";
        element_row.style.background = "white";
        element_row.style.color = "black";
        //Agregar el check en la celda 0
        let element_td = element_row.insertCell(0);
        let htmlCheck;
        if (selecion == "wf") {
            htmlCheck = ['<div class="ctw-document-row-selector">',
                '<input type="checkbox" class="chek_selecion_list_wf" chek_id="' + split[1] + '" aria-label="Seleccionar documento">',
                '</div>'
            ].join('')
        } else {
            htmlCheck = ['<div class="pl-0 pt-2">',
                '<input type="checkbox" class="ml-0 chek_selecion_list_rad" chek_id="' + split[1] + '">',
                '</div>'
            ].join('')
        }
        element_td.insertAdjacentHTML("beforeend", htmlCheck);
        //Agrega la celda del menú
        element_td = element_row.insertCell(1);
        //Agrega el menu a la celda
        element_td.insertAdjacentHTML("beforeend", html_menu);
        var numero_fila = element_table.rows.length - 1;
        if (selecion == "wf") {
            document.getElementById("Hidden_numero_doc_rel_wf").value = numero_fila;
            document.getElementById("Label_docu_relacionado_wf").innerHTML = "Documentos " + numero_fila;
        } else {
            document.getElementById("Hidden_numero_doc_rel").value = numero_fila;
            document.getElementById("Label_documentos").innerHTML = "Documentos " + numero_fila;
        }

    } catch (err) {
        alert(err.message + " Funcion insert_row_producion_documental");
    }
}
//Functión que actualiza el campo del muevo modelo de tabla aspNET con menu
const update_Cell_AspNetGred = (nombre_grid, id, valor_campo, nombre_campo, atr) => {
    try {
        $("#" + nombre_grid + " tr[" + atr + "=" + id + "]").each(function () {
            let idex = -1;
            idex = colum_index_(nombre_campo, nombre_grid);
            if (idex != -1) {
                let htmlCel = $(this)[0].cells[idex];
                let htmlSpan = htmlCel.firstChild.firstChild.lastChild.firstChild;
                if (valor_campo !== "") {
                    if (htmlSpan !== null) {
                        htmlSpan.innerText = valor_campo;
                    }
                } else {
                    if (htmlSpan !== null) {
                        htmlSpan.innerText = "\u00a0";
                    }
                }
            }
        })
    }
    catch (err) {
        alert(err.message + " update_Cell_AspNetGred");
    }
}
//Cambia el icono de una tabla asp.net  ---Pirceso de rempalzo de documento y retauración de documento
const cahange_icono_image_table_asp_net_general = (name_class_element_icono_aspnet, class_awsomw_icono) => {
    try {
        let ListElement = document.getElementsByClassName(name_class_element_icono_aspnet);
        if (ListElement.length > 0) {
            let Element = ListElement[0];
            var color_i = "";
            while (Element.hasChildNodes()) {
                if (color_i == "") {
                    if (Element.firstChild.style != null) {
                        color_i = Element.firstChild.style.color;
                    }
                }
                Element.removeChild(Element.firstChild);
            }
            var ihtml = document.createElement("i");
            ihtml.style.color = color_i;
            ihtml.classList.add("fal");
            ihtml.classList.add(class_awsomw_icono);
            Element.appendChild(ihtml);
        } else {
            let Element = document.getElementById(name_class_element_icono_aspnet);
            var color_i = "";
            while (Element.hasChildNodes()) {
                if (color_i == "") {
                    if (Element.firstChild.style != null) {
                        color_i = Element.firstChild.style.color;
                   }
                }
                Element.removeChild(Element.firstChild);
            }
            var ihtml = document.createElement("i");
            ihtml.style.color = color_i;
            ihtml.classList.add("fal");
            ihtml.classList.add(class_awsomw_icono);
            Element.appendChild(ihtml);
        }
        return "YES";
    }
    catch (ex) {
        return ex.message;
    }

}
