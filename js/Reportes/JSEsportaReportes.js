class JsExportaReporte {
    constructor(options = {}) {
        let defaults = {
            NameTable: "",
            NameReporte: "",
            NameService: "",
            IndexHiden: "-1"
        }
        this.settings = $.extend(true, defaults, options);
        this.CdfExportaReportes = Object;
        this.UrlFileReporte = "";
        this.NameFile = "";
        this.RutaFile = "";
    }
    async LoadJServiceExportReporte() {
        try {
            let Result = "";
            switch (this.settings.NameService) {
                case "ExportAspNet":
                    Result = await _JSExport.ExporReporteAspNet(this.settings.NameTable, this.settings.IndexHiden, this.settings.NameReporte);
                    return Result;
                    break;
            }
        } catch (ex) {
            return "Inconsistencia funcion LoadJServiceExportReporte " + ex.message;
        }
    }
    /**
     * 
     * @param {any} NameTable
     * @param {any} IndexHiden
     * @param {any} NameReporte
     */
    async ExporReporteAspNet(NameTable, IndexHiden, NameReporte) {
        try {
            let Result = "";
            Result = await _JSExport.SolicitaDatosTablAspNet(NameTable, IndexHiden, NameReporte);
            if (Result != "YES") {
                return Result;
            }
            Result = await ServiceRESTExportaReporteExcel(_JSExport.CdfExportaReportes);
            if (Result != "YES") {
                return Result;
            }
            Result = await _JSExport.DowloadFileReport(_JSExport.UrlFileReporte, _JSExport.NameFile);
            if (Result != "YES") {
                return Result;
            }
            Result = await ServiceRESTEliminaArchivoReport(_JSExport.RutaFile);
            return Result;
        } catch (ex) {
            return "Inconsistencia funcion ExporReporteAspNet " + ex.message;
        }
    }
    /*Extrae los datos de la tabla aspNET*
     * 
     * @param {any} NameTable
     * @param {any} IndexHiden
     * @param {any} NameReporte
     */
    async SolicitaDatosTablAspNet(NameTable, IndexHiden, NameReporte) {
        try {
            let HtmlTable = document.getElementById(NameTable);
            if (HtmlTable == null) {
                return "No se pudo encontrar la tabla (" + NameTable + ") para exportar los resultados.";
            }
            if (HtmlTable.rows.length == 1) {
                return "La tabla no contiene registros para exportar los resultados.";
            }
            let CdColumReportes = [];
            let HeaderTable = HtmlTable.querySelectorAll("th");
            HeaderTable.forEach((th, index) => {
                if (IndexHiden != index) {
                    CdColumReportes.push({ NameColum: th.textContent.toUpperCase(), AleasColum: th.textContent.toUpperCase() });
                }    
            });
            let CdRowReportes = [];
            let CdCellReportes = [];
            for (let i = 1; i < HtmlTable.rows.length; i++) {
                for (let j = 0; j < HtmlTable.rows[i].cells.length; j++) {
                    if (IndexHiden != j) { 
                        const celda = HtmlTable.rows[i].cells[j];
                        CdCellReportes.push({ CellValue: celda.innerText });
                    }
                }
                CdRowReportes.push({ Cell: CdCellReportes });
                CdCellReportes = [];
            }
            _JSExport.CdfExportaReportes = [];
            _JSExport.CdfExportaReportes.push({ NombreReporte: NameReporte, UsuarioReporte: "", ColumReportes: CdColumReportes, Row: CdRowReportes });
            //console.log(CdfExportaReportes);
            return "YES";
        } catch (ex) {
            return "Inconsistencia funcion SolicitaDatosTablAspNet " + ex.message;
        }
    }
    async DowloadFileReport(filename, name_file) {
    try {
        let element = document.createElement('a');
        element.setAttribute('href', filename);
        element.setAttribute('download', name_file);
        element.style.display = 'none';
        document.body.appendChild(element);
        element.click();
        document.body.removeChild(element);
        return "YES";
    } catch (err) {
        return " funcion  DowloadFileReport" + err.mensaje  ;
    }
}
}
let _JSExport;
const JsExport = async (Option) => {
    _JSExport = new JsExportaReporte(Option);
    let Rest = await _JSExport.LoadJServiceExportReporte();
    return Rest;
}

//------------Servicio web que solcita archivo de reporte excel-------//////
const ServiceRESTExportaReporteExcel = async (CdfExportaReportes) => {
    let serialice = JSON.stringify(CdfExportaReportes);
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceReportes.asmx/ServiceExportaReporteExcel",
                data: "{'CdfExportaReportes':'" + serialice + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d[0].AppError !== "YES") {
                        resolve(data.d[0].AppError);
                    } else {
                        _JSExport.UrlFileReporte = data.d[0].UrlFileReporte;
                        _JSExport.NameFile = data.d[0].NameFile;
                        _JSExport.RutaFile = data.d[0].RutaFile;
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
//------------Servicio web elimina archivo reporte en el servidor-------//////
const ServiceRESTEliminaArchivoReport = async (RutaFile) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceReportes.asmx/ServiceEliminaArchivoReport",
                data: "{'RutaFile':'" + RutaFile + "'}",
                dataType: "json",
                success: function (data) {
                    if (data.d[0].AppError !== "YES") {
                        resolve(data.d[0].AppError);
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