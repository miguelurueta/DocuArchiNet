class JSexpdiente {
    constructor(options = {}) {
        let defaults = {
            OptionExpediente: Object,
            OptionProgres:Object,
            NameService: "",
            IdTramite: 0,
           
        }
        
        this.settings = $.extend(true, defaults, options);
        this.CStruSiiCahcheExpediente = Object;
        this.CStruSiiCahcheVinculacion = Object;
    }
    async LoadJServiceExpediente() {
        try {
            let Result = "";
            switch (this.settings.NameService) {
                case "ServiceVinculaDocumentoSII":
                    Result = await _JSExpdiente.VinculaDocumentosExpedienteSII(this.settings.IdTramite, this.settings.OptionProgres, this.settings.OptionExpediente);
                    _JSEexpedienteResult.ErrorResuladoExpediente = Result;
                    return _JSEexpedienteResult;
                    break;
                case "ServiceRegistraExpeidenteSIIVincula":
                    Result = await _JSExpdiente.CreaExpedienteVinculaDocumentoSII(this.settings.IdTramite, this.settings.OptionProgres, this.settings.OptionExpediente);
                    _JSEexpedienteResult.ErrorResuladoExpediente = Result;
                    return _JSEexpedienteResult;
                    break;
            }
        } catch (ex) {
            _JSEexpedienteResult.ErrorResuladoExpediente = "Inconsistencia general funcion LoadJServiceExpediente " + ex.message;
            return _JSEexpedienteResult;
        }
    }
    /*Vincula un documentos a un único expediente*/
    /**
     * 
     * @param {any} IdTramite      : Reprenta la identifición del tramite documental
     * @param {any} OptionProgres  : Guarda las opciones para la función de progresos lista de documentos a vincular
     * @param {any} OptionExpediente : Gurda la estructura de las inscripciones del SII
     */
    async VinculaDocumentosExpedienteSII(IdTramite, OptionProgres, OptionExpediente) {
        try {
            let Result = "";
            /*Solicita el regitro cache de creación de expediente por matricula*/
            Result = await ServiceRESTSolicitaRegistroExpedienteMatricula(OptionExpediente, IdTramite);
            if (Result != "YES") {
                return result;
            }
            /*Valida exitencia cache expediente creado para vincular los documentos al expediente*/
            if (_JSEexpedienteResult.CStruSiiCahcheExpediente[0].Matricula == null) {
                return "YES";
            }
            Result = await ServiceRESTsolicitaCahcheVinculacionSII(OptionExpediente);
            if (Result != "YES") {
                return Result;
            }
            if (_JSEexpedienteResult.CStruSiiCahcheVinculacion[0].RadicadoSII == null) {
                Result = await ServiceRESTSolicitaDocumentosVinculacionUnicoExpedienteSII(OptionExpediente[0].RADICADO_SII, IdTramite,
                    OptionExpediente[0].COD_BARRA_SII, _JSEexpedienteResult.CStruSiiCahcheExpediente.id_expediente);
                if (Result != "YES") {
                    return Result;
                }
                /*** Dependencia del archivo JSPressBar */
                let _OPtionProgresBar = ({
                    name_service: OptionProgres.name_service,
                    OptionItemSelect: _JSEexpedienteResult.OptionResultExpediente.ClsssStructureVinculaDocumento,
                    NameControlPadreProgres: OptionProgres.NameControlPadreProgres, NameProceso: OptionProgres.NameProceso
                });
                /*Auto vincula los documentos*/
                Result = await JSProgresBarBoot(_OPtionProgresBar);
                if (Result != "YES") {
                    return Result;
                }
                /**Registro cache vinculaccion SII*/
                let _CStruSiiCahcheVinculacion = ({
                    RadicadoSII: OptionExpediente[0].RADICADO_SII,
                    CodigoBarras: OptionExpediente[0].COD_BARRA_SII,
                    Matricula: _JSEexpedienteResult.OptionResultExpediente.Matricula,
                    IdExpediente: _JSEexpedienteResult.OptionResultExpediente.id_expediente,
                    NombreGabinete: _JSEexpedienteResult.OptionResultExpediente.gabinete,
                    FechaRegistroCache: ""
                });
                Result = await ServiceRESTregistraCahcheVinculacionSII(_CStruSiiCahcheVinculacion);
                return Result;
            }
            return "YES";
        } catch (ex) {
            return ("Inconsistencia general funcion return VinculaDocumentosExpedienteSII " + ex.message);
        }
    }
    async CreaExpedienteVinculaDocumentoSII(IdTramite, OptionProgres, OptionExpediente) {
        try {
            let Result = "";
            /*Solicita el regitro cache de creación de expediente*/
            Result = await ServiceRESTSolicitaRegistroExpedienteMatricula(OptionExpediente, IdTramite);
            if (Result != "YES") {
                return Result;
            }
            /*Registra expediente y vincula documentos a expediente*/
            if (_JSEexpedienteResult.CStruSiiCahcheExpediente[0].Matricula == null) {
                /*Auto registra el expediente*/
                Result = await ServiceRESTcreaExpedienteIntegracionSII(IdTramite, OptionExpediente);
                if (Result != "YES") {
                    return Result;
                }
                /*** Dependendencia del archivo JSPressBar */
                let _OPtionProgresBar = ({
                    name_service: OptionProgres.name_service,
                    OptionItemSelect: _JSEexpedienteResult.OptionResultExpediente.ClsssStructureVinculaDocumento, NameControlPadreProgres: OptionProgres.NameControlPadreProgres, NameProceso: OptionProgres.NameProceso
                });
                /*Auto vincula los documentos*/
                Result = await JSProgresBarBoot(_OPtionProgresBar);
                if (Result != "YES") {
                    return Result;
                }

                /**Registro cache vinculaccion SII*/
                let _CStruSiiCahcheVinculacion = ({
                    RadicadoSII: OptionExpediente[0].RADICADO_SII,
                    CodigoBarras: OptionExpediente[0].COD_BARRA_SII,
                    Matricula: _JSEexpedienteResult.OptionResultExpediente.Matricula,
                    IdExpediente: _JSEexpedienteResult.OptionResultExpediente.id_expediente,
                    NombreGabinete: _JSEexpedienteResult.OptionResultExpediente.gabinete,
                    FechaRegistroCache :""
                });
                Result = await ServiceRESTregistraCahcheVinculacionSII(_CStruSiiCahcheVinculacion);
                return Result;

            } else {
                /*Solo vincula documentos a expedientes en caso de no tener expendientes relacionados*/
                Result = await ServiceRESTsolicitaCahcheVinculacionSII(OptionExpediente);
                if (Result != "YES") {
                    return Result;
                }
                if (_JSEexpedienteResult.CStruSiiCahcheVinculacion[0].RadicadoSII == null) {
                    Result = await ServiceRESTSolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII(OptionExpediente[0].RADICADO_SII, IdTramite,OptionExpediente);
                    if (Result != "YES") {
                        return Result;
                    }
                    /*** Dependencia del archivo JSPressBar */
                    let _OPtionProgresBar = ({
                        name_service: OptionProgres.name_service,
                        OptionItemSelect: _JSEexpedienteResult.OptionResultExpediente.ClsssStructureVinculaDocumento, NameControlPadreProgres: OptionProgres.NameControlPadreProgres, NameProceso: OptionProgres.NameProceso
                    });
                    /*Auto vincula los documentos*/
                    Result = await JSProgresBarBoot(_OPtionProgresBar);
                    if (Result != "YES") {
                        return Result;
                    }
                    /**Registro cache vinculaccion SII*/
                    let _CStruSiiCahcheVinculacion = ({
                        RadicadoSII: OptionExpediente[0].RADICADO_SII,
                        CodigoBarras: OptionExpediente[0].COD_BARRA_SII,
                        Matricula: _JSEexpedienteResult.OptionResultExpediente.Matricula,
                        IdExpediente: _JSEexpedienteResult.OptionResultExpediente.id_expediente,
                        NombreGabinete: _JSEexpedienteResult.OptionResultExpediente.gabinete,
                        FechaRegistroCache: ""
                    });
                    Result = await ServiceRESTregistraCahcheVinculacionSII(_CStruSiiCahcheVinculacion);
                    return Result;
                }
            }
            return "YES";
        } catch (ex) {
            return (ex.message);
        }
    }
}
let _JSExpdiente;
let _JSEexpedienteResult = {
    OptionResultExpediente : Object,
    ErrorResuladoExpediente  : ""
};
const JSExpdiente = async (Option) => {
    _JSExpdiente = new JSexpdiente(Option);
    _JSEexpedienteResult = await _JSExpdiente.LoadJServiceExpediente();
    return _JSEexpedienteResult;
}
//------------Servicio web que solicita lista de  documentos para relacionar para un único  expediente-------//////
const ServiceRESTSolicitaDocumentosVinculacionUnicoExpedienteSII = async (ReciboSII, IdTramite, CodBarras, IdExpediente) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceGaExpediente.asmx/ServiceSolicitaDocumentosVinculacionUnicoExpedienteSII",
                data: "{'ReciboSII':'" + ReciboSII + "','IdTramite':'" + IdTramite + "','CodBarras':'" + CodBarras + "','IdExpediente':'" + IdExpediente + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        _JSEexpedienteResult.OptionResultExpediente = new Array();
                        _JSEexpedienteResult.OptionResultExpediente = data.d[0];
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
//------------Servicio web que solicita lista de  documento para relacionar a multiplex expedientes-------//////
const ServiceRESTSolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII = async (ReciboSII, IdTramite, CIncripcionSII) => {
    var serialice = JSON.stringify(CIncripcionSII);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceGaExpediente.asmx/ServiceSolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII",
                data: "{'ReciboSII':'" + ReciboSII + "','IdTramite':'" + IdTramite + "','CIncripcionSII':'" + serialice + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        _JSEexpedienteResult.OptionResultExpediente = new Array();
                        _JSEexpedienteResult.OptionResultExpediente = data.d[0];
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
const ServiceRESTviculaDocumentoExpediente = async (ClsssStructureVinculaDocumento) => {
    let serialice = JSON.stringify(ClsssStructureVinculaDocumento);
    let myPromise = new Promise(function (resolve) {
        try {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceGaExpediente.asmx/ServiceVinculaDocumentoExpediente",
        data: "{'ClsssStructureVinculaDocumento':'[" + serialice +  "]'}",
        dataType: "json",
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
//------------Servicio web que crea  expediente integracion SII-------//////
const ServiceRESTcreaExpedienteIntegracionSII = async (IdTramite, OptionExpediente) => {
    let serialice = JSON.stringify(OptionExpediente);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceGaExpediente.asmx/ServiceCreaExpedienteIntegracionSII",
                data: "{'IdTramite':'" + IdTramite + "','CIncripcionSII':'" + serialice + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                    } else {
                        _JSEexpedienteResult.OptionResultExpediente = new Array();
                        _JSEexpedienteResult.OptionResultExpediente = data.d[0];
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

//-------Servicio que solicita la estructura del cache de creación expediente SII-----///             
const ServiceRESTSolicitaRegistroExpedienteMatricula = async (CIncripcionSII, IdTramite) => {
    var serialice = JSON.stringify(CIncripcionSII);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceSolicitaRegistroExpedienteMatricula', {
                data: "{" + "'CIncripcionSII':'" + serialice + "','IdTramite':'" + IdTramite + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].ErrorService !== "YES") {
                        resolve(data.d[0].ErrorService);
                    } else {
                        _JSEexpedienteResult.CStruSiiCahcheExpediente = new Array();
                        $.each(data.d, function (k, v) {
                            _JSEexpedienteResult.CStruSiiCahcheExpediente.push(v);
                        });
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

//-------Servicio que registra cache expediente sii SII-----///             
const ServiceRESTregistraCacheCreacionExpedienteSII = async (CStruSiiCahcheExpediente) => {
    var serialice ="[" + JSON.stringify(CStruSiiCahcheExpediente) + "]";
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceRegistraCacheCreacionExpedienteSII', {
                data: "{" + "'CStruSiiCahcheExpediente':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].ErrorService !== "YES") {
                        resolve(data.d[0].ErrorService);
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
//-------Servicio que actualiza estado vinculacion documentos cache expediente sii SII-----///             
const ServiceRESTactualizaEstadoVinculacionDocumentoSII = async (IdExpediente) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceActualizaEstadoVinculacionDocumentoSII', {
                data: "{" + "'IdExpediente':'" + IdExpediente + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].ErrorService !== "YES") {
                        resolve(data.d[0].ErrorService);
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
//-------Servicio que registra cache vinculacion documentos expediente SII-----///             
const ServiceRESTregistraCahcheVinculacionSII = async (CStruSiiCahcheVinculacion) => {
    var serialice = "[" + JSON.stringify(CStruSiiCahcheVinculacion) + "]";
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceRegistraCahcheVinculacionSII', {
                data: "{" + "'CStruSiiCahcheVinculacion':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].ErrorService !== "YES") {
                        resolve(data.d[0].ErrorService);
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
//-------Servicio que solicita la estructura del cache de vinculación documentos expediente SII-----///   
/**
 * 
 * @param {any} CIncripcionSII : Solicita la estructura de cache de inscripción
 */
const ServiceRESTsolicitaCahcheVinculacionSII = async (CIncripcionSII) => {
    var serialice = JSON.stringify(CIncripcionSII);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebService_integracion_sii.asmx/ServiceSolicitaCahcheVinculacionSII', {
                data: "{" + "'CIncripcionSII':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].ErrorService !== "YES") {
                        resolve(data.d[0].ErrorService);
                    } else {
                        _JSEexpedienteResult.CStruSiiCahcheVinculacion = new Array();
                        $.each(data.d, function (k, v) {
                            _JSEexpedienteResult.CStruSiiCahcheVinculacion.push(v);
                        });
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

