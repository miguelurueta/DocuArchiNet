let STRU_ANDES_FIRMA;
let FilesTamp;
class LoadFileStamp {
    constructor(options = {}) {
        let defaults = {
            id_imagen: 0,                       //Parametro que representa la identiifcación de la imgen
            name_gabinete: "",                  //Parametro que representa el nombre del gabinete
            module: 7,                          //Parametro que representa el modulo desde donde se firma 1-modulo worflow 2-Gestor documental 3- Docuarchi
            valida_firma: 1,                    //Parametro que representa si se valida la exitencia de una firma 1- Valida   0- No valida
            content_error: "",                  //Parametro que representa el nombre del cotrol contedeor del error
            NameControlParent: "",              //Parametro que representa el nombre del contenedor principal
            name_table: "",                     //Parametro que representa el nombre de la tabla  bootstrap o aspnet
            name_campo_estado_firma: "",        //Parametro que representa el nombre del campo donde se atualiza el icono en la taba bootstrap
            name_tipo_table: "",                //Parametro que representa el tipo de tabla para actulizar   bootstrap - tabla tipo bootstrap   aspnettable  - tabla tipo asp.net
            name_element_table_aspnet: "",      //Parametro que representa el nombre del icono en el menu tabla asp.net    
            name_class_element_icono_aspnet: "", //Parametro que representa el nombre  del icono de visualizar tabla asp.net 
            AtributeSingAspNet: "",              //Parametro que representa atributo de selección para tablas asp.net workflow idd_wf  idd_rad
            name_class_icono:"fa-file-certificate"
        }

        this.settings = $.extend(true, defaults, options);
    }
    ///Inicializa componente de firma de andes
    async LoadFilesStampAndes() {
        let result = "";
        STRU_ANDES_FIRMA = {};
        result = await Service_REST_solicita_estrutura_andes_firma(this.settings);
        if (result !== "YES") {
            return result;
        }
        /*Consume servicio web firma*/
        result = await Service_REST_andes_firma(STRU_ANDES_FIRMA.class_parameter_firma_sistema.url_service, STRU_ANDES_FIRMA.Class_paramter_andes_firma);
        if (result !== "YES") {
            return result;
        }
        /*Registra firma en el docuarchi*/
        result = await Service_REST_agrega_certificado_digital_a_documento(STRU_ANDES_FIRMA.class_parameter_firma_sistema, this.settings);
        if (result !== "YES") {
            return result;
        }
        return "YES";
    }
    async LoadStampMultipleSing() {
        try {
            let result = "";
            let PrameterTable = ({
                module: this.settings.module, valida_firma: this.settings.valida_firma, name_table: this.settings.name_table,
                name_campo_estado_firma: this.settings.name_campo_estado_firma, name_tipo_table: this.settings.name_tipo_table,
                name_class_element_icono_aspnet: this.settings.name_class_icono, name_class_icono: this.settings.name_class_icono
            })
            //----Asigna los item selecionados de la firma
            if (this.settings.name_tipo_table == "aspnettable") {
                result = await OtionItemDocumentosFirma(this.settings.name_table, this.settings.AtributeSingAspNet, PrameterTable);
                if (result != "YES") {
                    return result;
                }
            }
            let JSPResultPopup = await JSPopupConfirmInit(({
                TitlePopup: "", MensajepPoput: "¿Desea firmar digitalmente los documentos seleccionados?",
                NameContenPopup: this.settings.NameControlParent
            }));
            if (JSPResultPopup.EstadoErrorPopup != "") {
                return JSPResultPopup.EstadoErrorPopup;
            } else {
                if (JSPResultPopup.ReSulTPopup == "NOT") { return "YES"; }
            }
            let OPtionProgresBar = ({
                name_service: "firma_digital_andes_001",
                OptionItemSelect: OptionItem, NameControlPadreProgres: this.settings.NameControlParent, NameProceso: "Firma digital"
            });
            result = await JSProgresBarBoot(OPtionProgresBar);
            if (result != "YES") {
                return result;
            }
            return "YES"
        } catch (ex) {
            return ex.message;
        }
    }
    cahange_icono_image_table_asp_net = (name_class_element_icono_aspnet, class_awsomw_icono) => {
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
                ihtml.classList.add("far");
                ihtml.classList.add(class_awsomw_icono);
                Element.appendChild(ihtml);
            } else {
                ListElement = document.getElementById(name_class_element_icono_aspnet);
                let Element = ListElement;
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
                ihtml.classList.add("far");
                ihtml.classList.add(class_awsomw_icono);
                Element.appendChild(ihtml);
            }
        }
        catch (ex) {
            console.log(ex.message);
        }
           
   } 
    
    ///------Funcion cambia icono menu tabla asp.net
    cahange_icono_menu_table_asp_net = (boton_id_change_, valor)=> {
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
            let spamhtml = document.createElement("spam");
            spamhtml.classList.add("pl-1");
            spamhtml.classList.add("font-weight-light");
            spamhtml.innerText = "Firma digital";
            if (valor == 1) {
                ihtml.classList.add("fa-lock-alt");
                document.getElementById(boton_id_change_).title = "Documento con firma digital";  
            }
            if (valor == 2) {
                ihtml.classList.add("fa-file-invoice");
                document.getElementById(boton_id_change_).title = "Documento con meta datos";
            }
            ihtml.appendChild(spamhtml);
            document.getElementById(boton_id_change_).appendChild(ihtml);

        }
        return "YES";
    }
    catch (ex) {

        return "Error funcion cahange_icono_table_asp_net " + ex.message;
    }
    }
   

}

const LoadStampFile = async (Option) => {
    FilesTamp = new LoadFileStamp(Option);
    let result = await FilesTamp.LoadFilesStampAndes();
    return result;
}
const LoadStampMultipleSing = async (Option) => {
    FilesTamp = new LoadFileStamp(Option);
    let result = await FilesTamp.LoadStampMultipleSing();
    return result;
}
//------Zona firma digital---------// 
//--------Selecciona los items de firma de una tabla asp.net
const OtionItemDocumentosFirma = async (NameTable, IdentiItem, PrameterTable) => {
    try {
        OptionItem = new Array();
        $('#' + NameTable + ' tr.GridviewRow').each(function () {
            let ItemAtribute = $(this).attr(IdentiItem);
            let node_paren = $(this);
            //------Asigna la identificación del control padre del icono de visualización---//// 
            let id_boton = node_paren[0].cells[1].childNodes[0].childNodes[0].childNodes[0].childNodes[0].id;
            //-----Asiga el check de Item----------///////
            let HmlChek = node_paren[0].cells[0].childNodes[0].childNodes[0];
            if (HmlChek.checked) {
                ItemAtribute = ItemAtribute + "||" + id_boton;
                let nod_value = ItemAtribute.split("|");
                if (id_boton) {
                    OptionItem.push({
                        name_gabinete: nod_value[0], id_imagen: nod_value[1], Radicado: nod_value[2], TipoDocuarchi: nod_value[3],
                        TipoDocumentoTrd: nod_value[4], IdFlujo: nod_value[5], NameClase: nod_value[8], IdImageBoton: nod_value[9],
                        module: PrameterTable.module, valida_firma: PrameterTable.valida_firma, name_table: PrameterTable.name_table,
                        name_campo_estado_firma: PrameterTable.name_campo_estado_firma, name_tipo_table: PrameterTable.name_tipo_table,
                        name_element_table_aspnet: nod_value[9],
                        name_class_element_icono_aspnet: nod_value[9], name_class_icono: PrameterTable.name_class_icono
                    })
                   
                } else {
                    return "¡Atención! imposible constinuar con el firmado digital, falta el icono de firma (" + id_boton + ")";
                }
            }
        });
        if (OptionItem.length == 0) {
            return "¡Atención! No has seleccionado todos los ítems.Por favor, asegúrate de completar tu selección antes de continuar con el firmado digital";
        } else {
            return "YES";
        }
    }
    catch (err) {
        return " funcion OtionItemDocumentosFirma " + err.message;
    }
}
//---------activa firmado digital andes----//////// 
const stamp_file_doument_genral = async (id_imgen, name_tipo_table, gabinete, name_element_table_aspnet, name_class_element_icono_aspnet, name_container_error, module, name_class_icono) => {
    try {
        let result = "";
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        let Option = {
            id_imagen: id_imgen, name_gabinete: gabinete, module: module,
            valida_firma: "1", name_table: "", name_campo_estado_firma: "",
            name_tipo_table: name_tipo_table, name_element_table_aspnet: name_element_table_aspnet,
            name_class_element_icono_aspnet: name_class_element_icono_aspnet, name_class_icono: name_class_icono
        };
        result = await LoadStampFile(Option);
        if (result != "YES") {
            alert_bot(result, 'warning', name_container_error);

        }
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', name_container_error);
    } finally {
        progres_hiden('progres_bar');
    }
}
//-------Solicita estructuctura firma digital---////
const Service_REST_solicita_estrutura_andes_firma = async (element_this) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceFirmaDigital.asmx/Service_Solicita_estrutura_andes_firma', {
                data: "{'id_image':" + "'" + element_this.id_imagen + "'" + "," + "'gabinete':'" + element_this.name_gabinete +
                    "','modulo_funcion':'" + element_this.module + "','valida_exitencia_firma':'" + element_this.valida_firma + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        resolve(data.d[0].Error_gestion);
                    } else {
                        STRU_ANDES_FIRMA = data.d[0];
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
//-------Consube web service de firma digital---////
const Service_REST_andes_firma = async (url_service, data_parameter_andes) => {
    var serialice = JSON.stringify(data_parameter_andes);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax(url_service,{
                data: serialice,
                dataType: 'json',   
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data !== "YES") {
                        resolve(data);    
                    } else {
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
                        resolve("Time out error.");


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
//-------Agrega el registro de firma digital al documento ----////
const Service_REST_agrega_certificado_digital_a_documento = async (data_service_parameter_firma, element_this) => {
    var serialice = JSON.stringify(data_service_parameter_firma);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceFirmaDigital.asmx/Service_Agrega_certificado_digital_a_documento', {
                data: "{'parameter':" + "'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_gestion !== "YES") {
                        resolve(data.d[0].Error_gestion);
                    } else {
                        if (element_this.name_tipo_table == "bootstrap") {
                            updateCelByUniqueIdReinit(element_this.name_table, element_this.name_campo_estado_firma, element_this.id_imagen, 1);
                        }
                        if (element_this.name_tipo_table == "aspnettable") {
                            FilesTamp.cahange_icono_image_table_asp_net(element_this.name_class_element_icono_aspnet, element_this.name_class_icono);

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
                        resolve("Time out error.");

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
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                                
                             