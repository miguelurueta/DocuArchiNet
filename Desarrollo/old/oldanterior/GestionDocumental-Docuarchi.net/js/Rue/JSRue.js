class JsRues {
    constructor(options = {}) {
        let defaults = {
            ParamRue: "",
            CodigoCamara: "",
            NameService: "",
            NameContendorVisor: "",
            NameControlPadreVisor: "",
            NameControlError:""
        }
        this.settings = $.extend(true, defaults, options);
        this._CdefRues = Object;
        this._NameGabinete = "";
        this.DocumentoTilte = "";
        this._ModalVisor = "";
        this._UrlVisorPdf = "";
        this._Matricula = "";
    }
    async LoadJServiceRues() {
        let Result = "";
        switch (this.settings.NameService) {
            case "ServiceConsultaExpedienteRue":
                Result = await _JSERues.ConsultaExpedienteRue();
                return Result;
                break;

        }
    }
    async ConsultaExpedienteRue() {
        try {
            let Result = "";
            Result = await ServiceRESTIniciaConsultaRue(this.settings.ParamRue, this.settings.CodigoCamara);
            if (Result != "YES") {
                return Result;
            }
            console.log(_JSERues._CdefRues);
            Result = await ServiceRESTconsultaGabineteRue(_JSERues._CdefRues);
            if (Result != "YES") {
                return Result;
            }
            Result = await ServiceRESTsolicitaCatractrizacionExpedienteRue(_JSERues._CdefRues[0].CdRues.expediente,_JSERues._NameGabinete);
            return Result;
           
        } catch (ex) {
            return (ex.message);
        }
    }
    /**Show modal sacan file */
    async _ModalShowJSVisorRue() {
        if (document.getElementById(this.NameContendorVisor)) {
            let element = document.getElementById(this.NameContendorVisor);
            element.remove();
        }
        const wrapper = document.createElement('div');
        wrapper.id = this.NameContendorVisor;
        wrapper.innerHTML = [
            '<div class="modal fade modal_opacity " style="z-index:100050" id="modal_visor_rue" role="dialog" aria-hidden="false" data-backdrop="false">',
            '<div class="modal-dialog modal-fullscreen-sm-down">',
            '<div class="modal-content-fullscreen">',
            '<div class="modal-header">',
            '<h6 class="modal-title" id="header_modal_visor_rue"></h5>',
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">',
            '<span aria-hidden="true">&times;</span>',
            '</button>',
            '</div>',
            '<div class="modal-body">',
            '<iframe id="Iframe_doc_maticulado" class="border-0" style="width:100%; height:100%"> </iframe>',
            '</div>',
            '</div>',
            '</div>',
            '</div>'
        ].join('');
        let content = document.getElementById(this.settings.NameControlPadreVisor);
        if (content) {
            content.append(wrapper);
        }
        let HtmlIframe = document.getElementById("Iframe_doc_maticulado");
        HtmlIframe.setAttribute("src", _JSERues._UrlVisorPdf);
        let HmlTitleModal = document.getElementById("header_modal_visor_rue");
        if (HmlTitleModal) {
            HmlTitleModal.innerText = "Documento (" + _JSERues.DocumentoTilte + ")";
        }
        _JSERues._ModalVisor = $("#modal_visor_rue");
        _JSERues._ModalVisor.modal("show");
        return "YES";

    }
    async ShowVisorDocumentoConsultaRues (row)  {
        try {
            let result = "";
            delete_alert_boot();
            posicion_update_pogres('progres_bar');
            let ident = table_boot_return_objet_jonson(row);
            let Title_visor = "*DCUMENTO : " + ident.ID + "  *MATRICULA : " + _JSERues._Matricula;
            for (const [key, value] of Object.entries(row)) {
                let value_ = "";
                if (value == null || value == "") {
                    value_ = "Na";
                } else {
                    value_ = value;
                }
                Title_visor = Title_visor + "  *" + key.toUpperCase() + " : " + value_;
            }
            _JSERues.DocumentoTilte = Title_visor;
            result = await ServiceRESTSolicitaDocumentoConsultaRue(_JSERues._Matricula, _JSERues._NameGabinete, ident.ID);
            if (result !== "YES") {
                alert_bot(result, 'warning', this.settings.NameControlError);
                return true;
            }
            result = await _JSERues._ModalShowJSVisorRue();
            if (result !== "YES") {
                alert_bot(result, 'warning', this.settings.NameControlError);
                return true;
            }
        }
        catch (ex) {
            alert_bot(ex.message, 'warning', this.settings.NameControlError);
        } finally {
            progres_hiden('progres_bar');

        }
    }
}
let _JSERues;
const JSRue = async (Option) => {
    _JSERues = new JsRues(Option);
    let Rest = await _JSERues.LoadJServiceRues();
    return Rest;
}
//Interface para evento de visualizar documento matriculado
function operateFormattertablebootRueDocumentos(value, row, index) {
    return [
        '<a style="color: white" class="font-weight-light active_show_visualiza_documento btn btn-primary active" href="#" title="Visualiza documento soporte"  ><i class="fas fa-file-image"></i> Ver detalle </a>',
    ].join('')
}
//Evento de lista documentos del matriculado
window.operateEventsrUEDocumentos = {
    'click .active_show_visualiza_documento': (e, value, row, index) => {
        delete_alert_boot();
        _JSERues.ShowVisorDocumentoConsultaRues(row);

    }
}
//------------Servicio web que inicia la consulta web de expedientes RUES-------//////
const ServiceRESTIniciaConsultaRue = async (ParramRue, CodigoCamara) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceRue.asmx/ServiceIniciaConsultaRue",
                data: "{'ParramRue':'" + ParramRue + "','CodigoCamara':'" + CodigoCamara + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d[0].ErrorAppp !== "YES") {
                        resolve(data.d[0].ErrorAppp);
                    } else {
                        //this._CdefRues = data.d[0];
                        _JSERues._CdefRues = new Array();
                        $.each(data.d, function (k, v) {
                            _JSERues._CdefRues.push(v);
                        });
                        document.getElementById("r_3_us_value").innerText = _JSERues._CdefRues[0].CdRues.nombreUsuario;
                        document.getElementById("r_1_e_value").innerText = _JSERues._CdefRues[0].CdRues.nitEntidad;
                        document.getElementById("r_2_n_value").innerText = _JSERues._CdefRues[0].CdRues.nombreEntidad;
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
//------------Servicio web que vincula un documento a un expediente-------//////
const ServiceRESTconsultaGabineteRue = async (_CdefRues) => {
    let serialice = JSON.stringify(_CdefRues);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceRue.asmx/ServiceConsultaGabineteRue",
                data: "{'_CdefRues':'" + serialice + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        _JSERues._NameGabinete = data.d[0].NameTabla;
                        let class_stru_row_Gabinete_Generic = JSON.parse(data.d[0].Obj_ilist_row_generic);
                        if (document.getElementById("Label_resultado_title") !== null) {
                            document.getElementById("Label_resultado_title").innerText = class_stru_row_Gabinete_Generic.length + " Registro (s) " ;
                        }
                        init_row_feld_table_boostrap_table("table_documentos_rue", data.d[0].Obj_ilist_fileds_generic, class_stru_row_Gabinete_Generic, "contenido_datagrid_val_radicacion", "", "table-borderless");
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
//------------Servicio web que solicita al SII los datos de caracteriación del matriculado-------//////
const ServiceRESTsolicitaCatractrizacionExpedienteRue = async (Matricula, Gabinete) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceRue.asmx/ServiceSolicitaCatractrizacionExpedienteRue",
                data: "{'Matricula':'" + Matricula + "','Gabinete':'" + Gabinete + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d[0].ErrorAppp !== "YES") {
                        resolve(data.d[0].ErrorAppp);
                    } else {
                        document.getElementById("r_1_tr_value").innerText = data.d[0].CdRuesCaracterizacion.TipoRegistro;
                        document.getElementById("r_2_mt_value").innerText = data.d[0].CdRuesCaracterizacion.Matricula;
                        document.getElementById("r_3_rs_value").innerText = data.d[0].CdRuesCaracterizacion.Rsocial;
                        _JSERues._Matricula = data.d[0].CdRuesCaracterizacion.Matricula;
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
//-------Solicita la url del documento a visualizar del matriculado
const ServiceRESTSolicitaDocumentoConsultaRue = async (Matricula, Gabinete, IdImagen) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceDocuarchi.asmx/ServiceSolicitaDocumentoConsultaRue', {
                data: "{'Matricula':'" + Matricula + "','Gabinete':'" + Gabinete + "','IdImagen':'" + IdImagen + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        _JSERues._UrlVisorPdf = data.d[0].url_iframe;
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
                        resolve ("Time out error.");


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