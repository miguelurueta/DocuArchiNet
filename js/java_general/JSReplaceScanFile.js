class JSReplaceScanFile {
    constructor(options = {}) {
        let defaults = {
            IdImagen: 0,                          //Parametro que representa la identiifcación de la imagen
            Gabinete: "",                         //Paramentro que representa el nombre del gabinete
            name_class_element_icono_aspnet: "",  //Paramentro que representa la clase o el nombre del icono de la tabla asp.net
            DocumentoTilte: "",                   //Paramentro que representa el titulo del modal de diitaliación
            OptionRemlazo: "",                    //Paramentro que representa la opción de remplazo
            ContentError: "",                     //Paramentro que representa el nombre del contenedor de error
            NameModulo: ""  ,                     //Parametro que representa el nombre del modulo de remplazo
            NameControlPadreScan: "",
            UrlSacan: "",
            TipoTable: "asp.net",
            NameTable: "",
            NameCampo: "",
            NameCampoId: ""
        }
        this.settings = $.extend(true, defaults, options);
        this.NameContendorScanFile = "name_contenedor_scan_file_0001";
        this._ModalReplaceScanFile;
        this._BtnoAcptarSave;
        this._ClassVersionParamerterReplace;
    }
    /** Funtion que carga la interfaz de digitalizacion */
    async LoadJSReplaceScanFile() {
        try {    
            let result;
            result = await Service_REST_inicializa_dgitalizacion(this.settings.NameModulo);
            if (result != "YES") {
                return result;
            }
            result = await this._ModalShowJSReplaceScanFile();
            if (result != "YES") {
                return result;
            }
            return "YES";
        } catch (ex) {
            return ex.message;
        }
    }
    /**Show modal sacan file */
    async _ModalShowJSReplaceScanFile() {
        if (document.getElementById(this.NameContendorScanFile)) {
            let element = document.getElementById(this.NameContendorScanFile);
            element.remove();
        }
        const wrapper = document.createElement('div');
        wrapper.id = this.NameContendorScanFile;
        wrapper.innerHTML = [
            '<div class="modal fade modal_opacity " style="z-index:100050" id="modal_digitaliza_remplaza_documento" role="dialog" aria-hidden="false" data-backdrop="false">',
            '<div class="modal-dialog modal-fullscreen-sm-down">',
            '<div class="modal-content-fullscreen">',
            '<div class="modal-header">',
            '<h6 class="modal-title" id="header_modal_digitaliza_remplaza_documento"></h5>',
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">',
            '<span aria-hidden="true">&times;</span>',
            '</button>',
            '</div>',
            '<div class="modal-body">',
            '<iframe id="IframeDitaliza_remplaza" class="border-0" style="width:100%; height:100%"> </iframe>',
            '<div class="d-none">',
            '<input id="Button_save_replace_dig" type="button" value="button" />',
            '</div>',
            '</div>',
            '</div>',
            '</div>',
            '</div>'
        ].join('');
    let content = document.getElementById(this.settings.NameControlPadreScan);
    if (content) {
        content.append(wrapper);
    }
    
    this._BtnoAcptarSave = document.getElementById("Button_save_replace_dig");
    this._BtnoAcptarSave.addEventListener("click", this._ReplaceFileScan, false);
    let HtmlIframe = document.getElementById("IframeDitaliza_remplaza");
    HtmlIframe.setAttribute("src", this.settings.UrlSacan);
    let HmlTitleModal = document.getElementById("header_modal_digitaliza_remplaza_documento");
    if (HmlTitleModal) {
       HmlTitleModal.innerText = "Remplazar (" + this.settings.DocumentoTilte + ")";
    }
    this._ModalReplaceScanFile = $("#modal_digitaliza_remplaza_documento");
    this._ModalReplaceScanFile.modal("show");
    return "YES";

    }
    async _ReplaceFileScan(event) {
        try {
            let ParameterRemplazaDigitalizado = ({
                NameModulo: _JSReplaceScanFile.settings.NameModulo, Gabinete: _JSReplaceScanFile.settings.Gabinete,
                IdImagen: _JSReplaceScanFile.settings.IdImagen
            });
            let result_ = await Service_REST_remplaza_version(ParameterRemplazaDigitalizado);
            if (result_ != "YES") {
                alert_bot(result, 'warning', _JSReplaceScanFile.settings.ContentError);
                return true;
            }
            if (_JSReplaceScanFile.settings.TipoTable == "asp.net") {
                _JSReplaceScanFile._ChangeIconoImageTableAspnet(_JSReplaceScanFile.settings.name_class_element_icono_aspnet,
                    _JSReplaceScanFile._ClassVersionParamerterReplace.IconoAsome);
            } 
            if (_JSReplaceScanFile.settings.TipoTable == "bootstrap") {
                updateCelByUniqueIdReinit(_JSReplaceScanFile.settings.NameTable, _JSReplaceScanFile.settings.NameCampo, _JSReplaceScanFile.settings.IdImagen, 0);
                updateCelByUniqueIdReinit(_JSReplaceScanFile.settings.NameTable, "DBT", _JSReplaceScanFile.settings.IdImagen, _JSReplaceScanFile._ClassVersionParamerterReplace.DBT);
            }
           
        } catch (ex) {
            alert_bot(ex.message, 'warning', _JSReplaceScanFile.settings.ContentError);
        } finally {
            _JSReplaceScanFile._ModalReplaceScanFile.modal("hide");
        }
        
    }
    _ChangeIconoImageTableAspnet(name_class_element_icono_aspnet, class_awsomw_icono) {
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
}

let _JSReplaceScanFile;
const JSReplaceScanFileBoot = async (Option) => {
    _JSReplaceScanFile = new JSReplaceScanFile(Option);
    let result = await _JSReplaceScanFile.LoadJSReplaceScanFile();
    return result;
}
//-------Inicializa el parametro de digitalización
const Service_REST_inicializa_dgitalizacion = async (name_tipo) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_Config_Digitalizacion.asmx/Service_inicializa_dgitalizacion', {
                data: "{" + "'parameter':'" + name_tipo + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
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
const Service_REST_remplaza_version = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_remplaza_version', {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_sistema !== "YES") {
                        resolve(data.d[0].error_sistema);
                    } else {
                        _JSReplaceScanFile._ClassVersionParamerterReplace = data.d[0]; 
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