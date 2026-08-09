/*Clase carga archivo personalizado 
 * Ing Miguel Angel Urueta Miranda
 * Fecha 2025-05-06
 * Requiere JSProgresBar.js   FileUploadHandler.js
 * Implementacion
 * let _OPtionFileLoad = ({
            NameLoadProceso: "PRODUCCION",
            NameContenedorError: "error_content_adjunta_documeto_load_documento_006",
            funcion_name: "insert_row_producion_documental", evento_adjunta: "PRODUCCION",
            IdRespuestaIdExpediente: CDproduccion.CDexpedienteSeleccionado[0].IdExpediente,
            NameContendorLoadDocumento: "Contenedorderecho", ModalWidth: 75, CargaTipologia: 1,
            CargaFecha:1,CargaPreview:1,multi_select: "multiple",
            element_parent:"modal_adjunta_documeto_load_documento_006", TipoFormulario:1
        });
        Result = await IniLoadPerson(_OPtionFileLoad);
        return Result;
    Paramtros 
    NameLoadProceso             : Representa el nombre del proceso que  dibuja el formulario carga de archivos si envia vacio genera el formulario estatico
    NameContenedorError         : Representa el nombre del contendor de errore
    funcion_name                : Representa la función para insertar los datos de la imagen cargada en la interfaz NOTA debe
                                  registrar su evento en la función _RegistraArchivoInterfaz
    evento_adjunta              : Representa el nombre de la funcion que guarda el archivo en el servidor  FileUploadHandler_.ashx.vb
    IdRespuestaIdExpediente     : Representa la identificación del expediente o de id respuesta
    NameContendorLoadDocumento  : Representa el contendor donde se dibuja el formulario
    ModalWidth                  : Representa el porcentaje de ancho del formulario   25% 50% 75% 100% 
    TipoFormulario              : Representa el tipo formulario que dibuja 1-Dinamico 2-Estatico
 */ 
var counter;
var input;
var preview;
var CONTEN_NAME_UPLOAD_FILE = "";                            //Guarda el difereciador de los elementos controles
var CONTEN_NUM_UPLOAD_FILE = 0;                              //Guarda el numero de archivo cargados en el uploas file
var CONTEN_NUM_UPLOAD_INCRE_FILE = 0;                        //Guarda el incremento de archivos guardados 
var CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE = new DataTransfer();
var CONTEN_ID_UPLOAD_DELETE = 0;
var CONTEN_NAME_FILE_DELETE = "";
var CONTENT_RESULT_UPDATE_FILE;
var CONTENT_UPLOAD_ID_TIPO_DOCUMENTAL = 0;                   //Guardar id tipo documental
var CONTENT_UPLOAD_ESTADO_CHEK_ADJUNTO = 0;                 //Guarda el estado de documento adjunto
var CONTENT_UPLOAD_ESTADO_CHEK_RELACIONADO = 0;             //Guarda el estado relacionado
var CONTENT_ESTADO_NUMERO_RELACIONADO = 0;                  //Guarda estado relacionado
var CONTENT_ID_TAREA_WORKFLOW = 0;                          //Guarda el id de la tarea workflow
var CONTENT_ERROR_UPLOAD = "";
var CONTENT_UPLOAD_MULTIPLE_ESTATUS_SERVICE = "";
var CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
var CONTENT_UPLOAD_ARRAY = [];
var CONTENT_ID_UPLOAD = 0;
var CONTENT_NAME_UPLOAD_FUNCION = "";                           //Parametro de nombre de función que ejecuta un evento  el formulario del usuario intercatua con la funcion insert_runtHXR
var CONTENT_ESTENSION_PERMITIDA = ""
var CONTENT_ESTENSION_PERMITIDA_EXCLUSE = ""
var CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD = 0;
var CONTENT_SELECT_FILE_UPLOAD = "";
var CONTENT_ELEMENT_HTML_UPLOAD = "";                            //Represeta el nombre del elemento HTML a actualizar
var CONTENT_ELEMENT_UPDATE_PANEL_UPLOAD = "";
var CONTENT_ELEMENT_PARENT_UPLOAD = "";
var CONTENT_ELEMENT_INSERT_TABLE_UPLOAD = "";                   //Identifica la sigla para la tabla de insert para workflow   rad-wf
var CONTENT_ITEM_ROW_TIPO = new Array();
var CONTENT_PROCESO = "";
var CONTENT_NAME_CONTENDOR_ERROR = "";
var ajax = new XMLHttpRequest();
class LoadFilePERSON {
    constructor(options = {}) {
        let defaults = {
            funcion_name: "adjunta_migra_documento",           //Parametro de nombre de función que ejecuta un evento  el formulario del usuario intercatua con la funcion insert_runtHXR
            funcion_name_strar_file: "",
            evento_adjunta: "WORKFLOW",                       //Parametro que indica el evento que adjunta o modulo que se indentifica en el servicio web de configuración
            element_html_actuliza: "",
            tipo_adjunta: -1,
            name_tipo_documento: "",
            id_respuesta: 0,
            element_update_panel: "",                         //Parametro con el nombre del update panel
            id_tipo_docuental: -1,                            //Parametro representa la tiplogía documental
            element_parent: "modal_adjunta_documeto_migra",
            numero_documento_relacionado: - 1,
            estado_adjunto: -1,
            estado_relacion: -1,
            multi_select: "",
            gabinete: "",
            id_imagen: 0,
            element_parent_html_table: "",
            element_html_table: "",
            element_html_lab_conteo: "",
            apost_html_lab_conteo: "",
            element_drow_list_tipo: "",
            name_class_serivce_list: "",
            name_serivce_list: "",
            name_modulo: "",
            name_element_hml_modal: "",                          ///Parametro que representa modal padre, utlice para operaciones del modal
            option_obliga_tipologia: 0,                          ///Parmetro que obliga a aplicar tipologia
            name_class_element_icono_aspnet: "",
            TipoTable: "asp.net",
            TipoApost: "",                                      ///Parmetro determina si se esta trabajando con la tabla de enlace 'rad' o con la tabla tarea selecconda 'wf'
            NameTable: "",
            NameCampo: "",
            NameCampoId: "",
            estado_relacionado: 0,
            NameLoadProceso: "",                                 ///-----Representa el nombre del proceso que  dibuja el formulario carga de archivos
            NameContendorLoadDocumento: "",                      ///-----Representa el contendor del formulario
            NameContenedorError: "",                             ///-----Representa el nombre del contenedor de error
            ContentNameUploadFile: "",                           ///-----Representa el difereciador de los elementos controles o el post
            ContentElementInsertTableUpload: "",                 ///-----Representa la sigla para la tabla de insert para workflow   rad-wf
            IdRespuestaIdExpediente: 0,                          ///-----Representa la identificación de una respuesta o de un expediente
            ModalWidth: 75,                                      ///-----Representa el porcentaje del modal  25-50-75-100
            CargaTipologia: 1,                                   ///-----Representa si se dibuja la tipogia en el registro del documento
            CargaFecha: 0,                                       ///-----Representa si se dibuja la tipogia en el registro del documento
            CargaPreview: 0,                                     ///-----Representa si se dibuja la tipogia en el registro del documento
            TipoFormulario: 1                                    ///-----Representa el tipo formulario que dibuja 1-Dinamico 2-Estatico

        }
        this.settings = $.extend(true, defaults, options);
        this.FileReturn;                                         ///-----Representa el archivo ubicado en la estructura  CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE
        this.FormData = [];                                      ///-----Representa el archivo ubicado en la estructura de parametros para almacenar una archivo
        this.FilesCache = new DataTransfer();                    ///------Representa el cache de los archivos sleciconados para la CARGA
        this.ContenUploadArray = [];                             ///------Representa el contenido de la estructura de los archivos cargados en la interfaz
        this.NameContendorLoadDocumento = "name_contenedor_load_load_documento_006";       ///-----Guarda el nombre contenedor del documento
        this._NameElementFile = "file_element_adjunta_documeto_load_documento_006";        ///-----Representa el nombre del control file
        this._NameElementTable = "table_file_element_adjunta_documeto_load_documento_006"; ///-----Representa el nombre del control tabla
        this.NameLabelEstado = "count_file_element_adjunta_documeto_load_documento_006";   ///-----Representa el nombre del control label estado
        this._ModalLoadDocumento;                                ///-----Representa el modal de la interfaz de carga
        this._BotonSalvaArchivos;                                ///-----Representa el boton de salvar archivos
        this._BotonDeleteArchivos;                               ///-----Representa el boton de eliminar archivos
        this._BotonAdjuntaArchivos;                              ///-----Representa el boton de adjuntar archivos
        this._LabelArchivos;                                     ///-----Representa el label con los  archivos
        this._InputFIle;                                         ///-----Representa el control iput de archivos
        this._LabelPreview;                                      ///-----Representa el control del label del preview
        this._DragFileConten;                                    ///-----Representa el control del contenido drag
    }
    async LoadPersonFile() {
        try {
            CONTENT_NAME_CONTENDOR_ERROR = this.settings.NameContenedorError;
            let Result = "";
            switch (this.settings.TipoFormulario) {
                case 1:
                    Result = await FilePerson.LoadFormularioDinamico();
                    return Result;
                    break;
                case 2:
                    Result = await FilePerson.LoadFormularioEstatico();
                    return Result;
                    break;
                default:
                    return "No se detecto el tipo de formulario";
                    break;
            }
            return "YES";
        } catch (ex) {
            return "Inconsistencia funcio LoadPersonFile " + ex.mensaje;
        }
    }
    /*Crea formulario dinamico para la carga de archivos*/
    async LoadFormularioDinamico() {
        try {
            let Result = ""
            Result = await FilePerson._CreaModalFile();
            if (Result != "YES") {
                return Result;
            }
            Result = await FilePerson._ServiceRESTparameterUpload();
            if (Result != "YES") {
                return Result;
            }
            switch (FilePerson.settings.NameLoadProceso) {
                case "PRODUCCION":
                    Result = await FilePerson._ServiceRESTListTiposDocumentalesFile("", "WebServiceProducion.asmx", "ServiceSolicitaListaTipologiasExpediente", FilePerson.settings.IdRespuestaIdExpediente);
                    if (Result != "YES") {
                        return Result;
                    }
                    break;
                case "RADICA_WORKFLOW":
                    Result = await FilePerson._ServiceRESTListTiposDocumentalesFile("", FilePerson.settings.name_class_serivce_list, FilePerson.settings.name_serivce_list, FilePerson.settings.IdRespuestaIdExpediente);
                    if (Result != "YES") {
                        return Result;
                    }
                    break;
                case "ADJUNTARADICACION":
                    Result = await FilePerson._ServiceRESTListTiposDocumentalesFile("", FilePerson.settings.name_class_serivce_list, FilePerson.settings.name_serivce_list, FilePerson.settings.IdRespuestaIdExpediente);
                    if (Result != "YES") {
                        return Result;
                    }
                    break;
                case "WORKFLOWSELECCION":
                    Result = await FilePerson._ServiceRESTListTiposDocumentalesFile("", FilePerson.settings.name_class_serivce_list, FilePerson.settings.name_serivce_list, FilePerson.settings.IdRespuestaIdExpediente);
                    if (Result != "YES") {
                        return Result;
                    }
                    break;
                case "WORKFLOWENLACE":
                    Result = await FilePerson._ServiceRESTListTiposDocumentalesFile("", FilePerson.settings.name_class_serivce_list, FilePerson.settings.name_serivce_list, FilePerson.settings.IdRespuestaIdExpediente);
                    if (Result != "YES") {
                        return Result;
                    }
                    break;

                default:
            }
            $(this._ModalLoadDocumento).modal("show");
            this._EventAutoZiseModal();
            return "YES";
        } catch (ex) {
            return "Inconsistencia general funcion LoadFormularioDinamico " + ex.mensaje;
        }
    }
    async LoadFormularioEstatico() {
        try {
            //-------Inicializa los eventos de los botones cuando el formulario de carga no se dibuja por java
            let Result = await FilePerson.InitUploadFileClientAsync(this.settings.funcion_name, this.settings.funcion_name_strar_file);
            if (Result !== "YES") {
                return Result;
            }
            if (this.settings.element_drow_list_tipo != "" && this.settings.name_class_serivce_list != "" && this.settings.name_serivce_list != "") {
                Result = await service_REST_source_list_tipos_documentales(this.settings.element_drow_list_tipo,
                    this.settings.name_class_serivce_list,
                    this.settings.name_serivce_list);
                if (Result !== "YES") {
                    return Result;
                }
            }
            Result = await service_REST_parameter_upload_boot(this.settings.evento_adjunta,
                this.settings.element_parent,
                this.settings.multi_select,
                this.settings.funcion_name);
            if (Result !== "YES") {
                return Result;
            }
            return "YES"
        } catch (ex) {
            return "Inconsistencia general funcion LoadFormularioEstatico " + ex.mensaje
        }
    }
    /**Crea el formulario de carga de archivos */
    async _CreaModalFile() {
        try {
            if (document.getElementById(this.NameContendorLoadDocumento)) {
                let element = document.getElementById(this.NameContendorLoadDocumento);
                element.remove();
            }
            const wrapper = document.createElement('div');
            wrapper.id = this.NameContendorLoadDocumento;
            wrapper.innerHTML = [
                '<div class="modal fade modal_opacity" style="z-index:100061" id="modal_adjunta_documeto_load_documento_006" role="dialog" data-backdrop="false">',
                '<div class="modal-dialog  modal-dialog-centered modal-custom-size-' + this.settings.ModalWidth + ' modal-' + this.settings.ModalWidth + 'w">',
                '<div class="modal-content-fullscreen" id="modal_content_load_documento_006">',
                '<div class="modal-header" id="modal_header_load_documento_006">',
                '<h4 style="color: black" class="modal-title">Adjunta documento</h4>',
                '<button type="button" class="close" data-dismiss="modal">&times;</button>',
                '</div>',
                '<div class="modal-body-fullscreen modal-body">',
                '<div class="row row-body-fullscreen contenedorTabla">',
                '<div class="pl-2 pr-2 w-100">',
                '<div class="row p-2" id="content_boton_adjunta_documeto_load_documento_006">',
                '<div class="col-12 p-0 pl-1 pr-1 d-flex justify-content-end">',
                '<a id="delete_file_element_adjunta_documeto_load_documento_006" title="Elminar todos los archivos cargados" class="btn  btn-danger ml-1" style="opacity: 0; color: white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>',
                '<a id="save_file_element_adjunta_documeto_load_documento_006" title="Guardar todos los archivos" class="btn  btn-success ml-1 mr-1" style="opacity: 0; color: white"><i style="color: white" class="fas fa-save "></i> Guardar </a>',
                '<div class="file-select " id="src-file_">',
                '<input id="file_element_adjunta_documeto_load_documento_006" type="file" multiple=' + this.settings.multi_select + 'accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />',
                '</div>',
                '</div>',
                '</div>',
                '<div id="previewContainer" class="d-flex flex-wrap gap-3 w-100"></div>',
                '<div class="paren_element background_upload tabla-scroll" id="conten_file_element_adjunta_documeto_load_documento_006" style="">',
                '<table id="table_file_element_adjunta_documeto_load_documento_006" class="table table-striped tabla-scroll">',
                '</table>',
                '<div id="content_drop_element_adjunta_documeto_load_documento_006" claas="">',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '<div id="error_content_adjunta_documeto_load_documento_006" style="position: relative; width: 100%" class="pl-4 pr-4"></div>',
                '<div class=" modal-footer_ justify-content-end" id="modal_footer_load_documento_006">',
                '<p id="count_file_element_adjunta_documeto_load_documento_006" class="font-weight-light pr-3" style="float: right">Estado </p>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
            ].join('');
            let content = document.getElementById(this.settings.NameContendorLoadDocumento);
            if (content) {
                content.append(wrapper);
            }
            let Table = document.getElementById(this._NameElementTable);
            this._LabelArchivos = document.getElementById("count_file_element_adjunta_documeto_load_documento_006");
            this._InputFIle = document.getElementById("file_element_adjunta_documeto_load_documento_006");
            this._DragFileConten = document.getElementById("conten_file_element_adjunta_documeto_load_documento_006");
            this._DragFileConten.addEventListener("drop", this._EventDropArchivo, false);
            this._DragFileConten.addEventListener("dragover", this._EventDragOver, false);
            this._DragFileConten.addEventListener("dragleave", this._EventDragLeave, false);
            this._ModalLoadDocumento = $("#modal_adjunta_documeto_load_documento_006");
            this._BotonAdjuntaArchivos = document.getElementById("file_element_adjunta_documeto_load_documento_006");
            this._BotonAdjuntaArchivos.addEventListener("change", this._EventenAdjuntaArchivo, false);
            this._BotonDeleteArchivos = document.getElementById("delete_file_element_adjunta_documeto_load_documento_006");
            this._BotonDeleteArchivos.addEventListener("click", this._EventDeleteAllArchivo, false);
            this._BotonSalvaArchivos = document.getElementById("save_file_element_adjunta_documeto_load_documento_006");
            this._BotonSalvaArchivos.addEventListener("click", this._EventEnviarArchivosServer, false);
            document.getElementById('modal_adjunta_documeto_load_documento_006').addEventListener('shown.bs.modal', this._EventAutoZiseModal);
            Table.addEventListener("click", this._EventTableOnclick);
            window.addEventListener('resize', this._EventAutoZiseModal);
            return "YES";
        } catch (ex) {
            return "Inconsistencia funcion _CreaModalFile " + ex.message;
        }
    };
    /**Zona de eventos del compoente */
    async _EventAutoZiseModal() {
        try {
            let HeigthModal = document.getElementById("modal_content_load_documento_006").clientHeight;
            let wara = document.getElementById("wrapperfile_001");
            let HeigtWrap = 1;
            if (wara) {
                HeigtWrap = wara.clientHeight;
            }
            let Heigth = HeigthModal - (document.getElementById("modal_header_load_documento_006").clientHeight + document.getElementById("modal_footer_load_documento_006").clientHeight + HeigtWrap);
            $('#conten_file_element_adjunta_documeto_load_documento_006').css("height", (Heigth - 100) + "px");
        }
        catch (err) {
            alert(err.message + " funcion _EventAutoZiseModal " + err.message);
        }
    }
    _EventTableOnclick(event) {
        try {
            const fila = event.target.closest("tr");
            // Evita seleccionar si no está dentro del <tbody> (por ejemplo, en el <thead>)
            if (!fila || fila.parentNode.tagName !== "TBODY") return;
            let Table = document.getElementById(FilePerson._NameElementTable);
            // Elimina la clase de todas las filas
            Table.querySelectorAll("tbody tr").forEach(r => r.classList.remove("fila-activa-file"));
            // Agrega la clase a la fila clickeada
            fila.classList.add("fila-activa-file");
        } catch (ex) {
            console.log("Funcion _EventTableOnclick " + ex.mensaje);
        }
    }
    async _EventenAdjuntaArchivo(event) {
        try {
            delete_alert_boot();
            let Result = "";
            Result = await FilePerson._AdjuntaArchivosDesdeDispostivo();
            if (Result != "YES") {
                alert_bot(Result, 'warning', FilePerson.settings.NameContenedorError);
            }
        } catch (ex) {
            alert_bot("Error EventenAdjuntaArchivo " + ex.mensaje, 'warning', FilePerson.settings.NameContenedorError);
        } finally {

        }
    }

    async _EventDropArchivo(e) {
        try {
            let Result = "";
            let DataTransfer = e.dataTransfer;
            if (DataTransfer.files.length == 0) { return "YES"; }
            let ElemenFile = document.getElementById(FilePerson._NameElementFile);
            let ElementTable = document.getElementById(FilePerson._NameElementTable);
            let HtmLabelEstado = document.getElementById(FilePerson.NameLabelEstado);
            ElemenFile.files = DataTransfer.files;
            let Files = $("#" + FilePerson._NameElementFile).get(0).files;
            if (Files.length == 0) {
                return "YES";
            } else {
                if (ElementTable.rows.length > 0) {
                    $("#" + FilePerson._NameElementTable).find("tr").remove();
                }
                FilePerson._BotonSalvaArchivos.style.opacity = "0";
                FilePerson._BotonDeleteArchivos.style.opacity = "0";
                Result = await FilePerson._AdjuntaArchivosDesdeDispostivo();
                if (Result != "YES") {
                    alert_bot(Result, 'warning', FilePerson.settings.NameContenedorError);
                }
                return "YES";
            }

        } catch (ex) {
            alert_bot("Error _EventDropArchivo " + ex.mensaje, 'warning', FilePerson.settings.NameContenedorError);
        }
    }
    async _EventEnviarArchivosServer(event) {
        try {
            delete_alert_boot();
            let Result = "";
            Result = await FilePerson._SolicitaEstructuraTablaUpload(FilePerson._NameElementTable, -1);
            if (Result != "YES") {
                alert_bot(Result, 'warning', FilePerson.settings.NameContenedorError);
                return true;
            }
            Result = await FilePerson._PreEnvioArchivosServidor();
            if (Result != "YES") {
                alert_bot(Result, 'warning', FilePerson.settings.NameContenedorError);
                return true;
            }

        } catch (ex) {
            alert_bot("Error _EventEnviarArchivosServer " + ex.mensaje, 'warning', FilePerson.settings.NameContenedorError);
        }
    }
    async _EventEnviarArchivoServer(e) {
        let timer;
        try {
            delete_alert_boot();
            let Result = "";

            timer = setTimeout(() => {
                FilePerson._MostrarLoading();
            }, 500);
            let id = e.currentTarget.getAttribute("id");
            let Error = e.currentTarget.getAttribute("Error");
            if (Error != "") {
                alert_bot("No se puede guardar el archivo. Se detectó la siguiente advertencia (" + Error + ").", 'warning', FilePerson.settings.NameContenedorError);
                return true;
            }
            Result = await FilePerson._SolicitaEstructuraTablaUpload(FilePerson._NameElementTable, id);
            if (Result != "YES") {
                alert_bot(Result, 'warning', FilePerson.settings.NameContenedorError);
                return true;
            }
            if (FilePerson.settings.setioption_obliga_tipologia == 1 && FilePerson.settings.CargaTipologia == 1) {
                let item = FilePerson.ContenUploadArray[0];
                if (item.DescripcionTipoDocumento === "" || item.IdTipoDocumento === "-1" || item.IdTipoDocumento === "0") {
                    alert_bot("Debe seleccionar una tipología para cada archivo cargado.", 'warning', FilePerson.settings.NameContenedorError);
                    return true;
                }
            }
            Result = await FilePerson._EnviaArchivoServidor(FilePerson.ContenUploadArray[0]);
            if (Result != "YES") {
                alert_bot(Result, 'warning', FilePerson.settings.NameContenedorError);
                return true;
            }

        } catch (ex) {
            alert_bot("Error _EventEnviarArchivoServer " + ex.mensaje, 'warning', FilePerson.settings.NameContenedorError);
        } finally {
            clearTimeout(timer);
            FilePerson._OcultarLoading();
        }
    }
    async _EventDragLeave(e) {
        e.stopPropagation();
        e.preventDefault();
    }
    async _EventDragOver(e) {
        e.stopPropagation();
        e.preventDefault();
    }
    async _EventDeleteAllArchivo(event) {
        try {
            delete_alert_boot();
            let Result = await FilePerson._DeleteFiles("-1");
            if (Result != "YES") {
                alert_bot(Result, 'warning', FilePerson.settings.NameContenedorError);
            }
        } catch (ex) {
            alert_bot("Incosistencia general funcion  _EventDeleteAllArchivo " + ex.mensaje, 'warning', FilePerson.settings.NameContenedorError);
        }
    }
    async _EventDeleteFile(e) {
        try {
            delete_alert_boot();
            let id = e.currentTarget.getAttribute("id");
            if (!id) { alert_bot("Iposible encontrar el identicador " + ex.mensaje, 'warning', FilePerson.settings.NameContenedorError); return true; }
            let Result = await FilePerson._DeleteFiles(id);
            if (Result != "YES") {
                alert_bot(Result, 'warning', FilePerson.settings.NameContenedorError);
            }
        } catch (ex) {
            alert_bot("Incosistencia general funcion  _EventDeleteFile " + ex.mensaje, 'warning', FilePerson.settings.NameContenedorError);
        }
    }
    async _EventVisualizaFile(e) {
        try {
            delete_alert_boot();
            if (FilePerson.FilesCache.items.length == 0) {
                return true;
            }
            let Result = "";
            let IndexFile = e.currentTarget.getAttribute("id");
            let Files = $("#" + FilePerson._NameElementFile).get(0).files;
            File = Files[IndexFile];
            if (File) {
                const url = URL.createObjectURL(File);
                Result = await FilePerson._CrearVistaVisualizaFile(File, url);
                if (Result != "YES") {
                    alert_bot(Result, 'warning', FilePerson.settings.NameContenedorError);
                }
            }
        } catch (ex) {
            alert_bot("Error _EventVisualizaFile " + ex.mensaje, 'warning', FilePerson.settings.NameContenedorError);
        }
    }
    //-----Validaciones de fecha de los campos inputo-----////
    /**
     * Valida los controles input tipo fecha cuando pierden el foco
     * @param {any} e
     */
    _ValidateFechaFocus(e) {
        try {
            let value = e.currentTarget.value;
            if (value == "") {
                return "YES";
            }
            const Actual = new Date().getFullYear();
            let year = value.substring(0, 4);
            let month = value.substring(5, 7);
            let day = value.substring(8, 10);
            let SpanDanger = document.querySelector('span[data-asp-danger="' + e.currentTarget.id + '"]');
            SpanDanger.textContent = "";
            if (Actual < year) {
                e.currentTarget.focus();
                e.currentTarget.setSelectionRange(0, 4);
                e.currentTarget.style.background = "#ffffcc";
                SpanDanger.textContent = "El año ingresado no es valido.";
                return "YES";
            }
            if (month != "" && (month > 12 || month < 1)) {
                e.currentTarget.focus();
                e.currentTarget.setSelectionRange(5, 7);
                e.currentTarget.style.background = "#f7d2d2";
                SpanDanger.textContent = "El mes ingresado no es válido.";
                return "YES";
            }
            if (day != "" && (day > 31 || day < 1)) {
                e.currentTarget.focus();
                e.currentTarget.setSelectionRange(8, 10);
                e.currentTarget.style.background = "#f7d2d2";
                SpanDanger.textContent = "El dia ingresado no es válido.";
                return "YES";
            }
            let Fecha = new Date(year, month - 1, day);
            if (Fecha.getDate() !== Number(day)) {
                e.currentTarget.focus();
                e.currentTarget.setSelectionRange(8, 10);
                e.currentTarget.style.background = "#f7d2d2";
                SpanDanger.textContent = "El dia ingresado no es válido.";
                return "YES";
            }
            let leng = value.length;
            if (leng != 10) {
                e.currentTarget.focus();
                e.currentTarget.setSelectionRange(0, leng);
                e.currentTarget.style.background = "#f7d2d2";
                SpanDanger.textContent = "Tamaño de fecha no valida.";
                return "YES";
            }
            e.currentTarget.style.background = "";
        } catch (ex) {
            alert("function ValidateFechaFocus " + ex.mensaje)
        }
    }
    /**
     * Formatea el valor de la fecha cundo se presiona 
     * la tecla del input fecha yyyy-mm-dd
     * @param {any} e
     */
    _ValidateFechaFormControlFile(e) {
        try {

            let tecla = (document.all) ? e.keyCode : e.which;
            if (tecla == 32) {
                return false;
            }
            let patron = /^[0-9 ]$/;
            var te = String.fromCharCode(tecla);
            var res = patron.test(te);
            if (res) {
                if (e.currentTarget.value.length == 4 || e.currentTarget.value.length == 7) {
                    e.currentTarget.value = e.currentTarget.value + "-";
                    return patron.test(te);
                }
            } else {
                e.preventDefault();
                return false;
            }
        } catch (err) {
            alert(err.message + " funcion _ValidateFechaFormControlFile " + err.message);
        }
    }
    /**
     * /Valida fechas de los controles input fecha
     * @param {any} inputId
     */
    async _EsFechaValida(inputId) {
        try {
            const input = document.getElementById(inputId);
            const fecha = input.value.trim();
            if (fecha == "") {
                return "YES";
            }
            const Actual = new Date().getFullYear();
            let year = fecha.substring(0, 4);
            let month = fecha.substring(5, 7);
            let day = fecha.substring(8, 10);
            if (Actual < year) {
                return "El año ingresado no es valido en un campo de fecha.";
            }
            if (month != "" && (month > 12 || month < 1)) {
                return "El mes ingresado no es válido en un campo de fecha.";
            }
            if (day != "" && (day > 31 || day < 1)) {
                return "El dia ingresado no es válido en un campo de fecha.";

            }
            let Fecha_ = new Date(year, month - 1, day);
            if (Fecha_.getDate() !== Number(day)) {
                return "El dia ingresado no es válido en un campo de fecha.";
            }
            let leng = fecha.length;
            if (leng != 10) {
                return "Tamaño de fecha no valida en un campo de fecha.";
            }
            return "YES";

        } catch (ex) {
            return "Incosnsistencia funcion _EsFechaValida " + ex.mensaje;
        }
    }

    async _AdjuntaArchivosDesdeDispostivo() {
        try {

            let ElementTable = document.getElementById(FilePerson._NameElementTable);
            let HtmLabelEstado = document.getElementById(FilePerson.NameLabelEstado);
            let Files = $("#" + FilePerson._NameElementFile).get(0).files;
            if (Files.length == 0) {
                return "YES";
            } else {
                if (ElementTable.rows.length > 0) {
                    $("#" + FilePerson._NameElementTable).find("tr").remove();
                }
                FilePerson._BotonSalvaArchivos.style.opacity = "0";
                FilePerson._BotonDeleteArchivos.style.opacity = "0";
            }
            let ContaRow = 0;
            for (let File of Files) {
                /*let valida_ext = ValidateFileType(File.name);
                if (valida_ext == "none") {
                    /*if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                        alert_bot("El archivo (" + file.name + ") pertence a una extensión no permitida, el archivo no se cargará.", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                    } else {
                        alert("El archivo (" + file.name + ") pertence a una extensión no permitida, el archivo no se cargará.");
                    }
                }*/
                let ValidaTamano = "yes";
                let FileSize = 0;
                FileSize = returnFileSize(File.size);
                let FileSizPermitido = returnFileSize(CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD);
                let ErrorPopuver = "";
                if (File.size > CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD) {
                    ValidaTamano = "NO"
                    ErrorPopuver = "Supera el tamaño permitido " + FileSizPermitido;
                }
                if (ValidaTamano == "yes") {
                    FilePerson.FilesCache.items.add(File);
                }
                let ContadorCelda = 0;
                let ElementRow = ElementTable.insertRow(ContaRow);
                let ElementTd = ElementRow.insertCell(ContadorCelda);
                ElementTd.classList.add("w-25");
                ElementRow.setAttribute("id", ContaRow);
                ElementRow.setAttribute("NameFile", File.name);
                ElementRow.setAttribute("Error", "0");
                ElementRow.style.cursor = "pointer";
                ElementRow.style.background = "white";
                ElementRow.style.color = "black";
                //Agrega celda nombre
                let DivhtmlTitle = document.createElement("div");
                let HtmlSpan = document.createElement('span');
                HtmlSpan.classList.add("font-weight-light");
                HtmlSpan.textContent = File.name;
                HtmlSpan.setAttribute("id_element", File.name);
                DivhtmlTitle.appendChild(HtmlSpan);
                ElementTd.appendChild(DivhtmlTitle);

                //Agrega celda tamano archivo
                ContadorCelda = ContadorCelda + 1;
                ElementTd = ElementRow.insertCell(ContadorCelda);
                let DivhtmlTamano = document.createElement("div");
                HtmlSpan = document.createElement('span');
                HtmlSpan.classList.add("font-weight-light");
                HtmlSpan.textContent = FileSize;
                DivhtmlTamano.appendChild(HtmlSpan);

                //Agrega popu ver archivos con errores
                if (ErrorPopuver != "") {
                    HtmlSpan = document.createElement('span');
                    HtmlSpan.classList.add("d-inline-block");
                    HtmlSpan.classList.add("pl-2");
                    HtmlSpan.setAttribute("data-bs-toggle", "popover");
                    HtmlSpan.setAttribute("data-bs-trigger", "hover focus");
                    HtmlSpan.setAttribute("data-bs-content", ErrorPopuver);
                    let ihtmls = document.createElement("i");
                    ihtmls.style.color = "white";
                    ihtmls.classList.add("fal");
                    ihtmls.classList.add("fa-exclamation-triangle");
                    let HtmlA = document.createElement("a");
                    HtmlA.title = ErrorPopuver;
                    HtmlA.classList.add("btn");
                    HtmlA.classList.add("btn-warning");
                    HtmlA.classList.add("btn-sm");
                    HtmlA.appendChild(ihtmls);
                    HtmlSpan.appendChild(HtmlA);
                    ElementRow.setAttribute("Error", "1");
                    DivhtmlTamano.appendChild(HtmlSpan);
                }
                ElementTd.appendChild(DivhtmlTamano);
                //--Agrega celda boton eliminar------/// 
                ContadorCelda = ContadorCelda + 1;
                ElementTd = ElementRow.insertCell(ContadorCelda);
                ElementTd.classList.add("w-25");
                let DivhtmlBoton = document.createElement("div");
                let ihtml = document.createElement("i");
                ihtml.style.color = "white";
                ihtml.classList.add("fal");
                ihtml.classList.add("fa-trash-alt");
                let ahtml = document.createElement("a");
                ahtml.setAttribute("id", ContaRow);
                ahtml.classList.add("btn");
                ahtml.classList.add("btn-danger");
                ahtml.classList.add("btn-sm");
                ahtml.addEventListener("click", this._EventDeleteFile, false);
                ahtml.setAttribute("title", "Eliminar archivo");
                ahtml.appendChild(ihtml);
                DivhtmlBoton.appendChild(ahtml);
                ElementTd.appendChild(DivhtmlBoton);

                //-----Agrega el boton de vizualización---------///
                if (FilePerson.settings.CargaPreview == 1) {
                    ihtml = document.createElement("i");
                    ihtml.style.color = "white";
                    ihtml.classList.add("fal");
                    ihtml.classList.add("fa-file");
                    ahtml = document.createElement("a");
                    ahtml.setAttribute("id", ContaRow);
                    ahtml.classList.add("btn");
                    ahtml.classList.add("btn-primary");
                    ahtml.classList.add("btn-sm");
                    ahtml.classList.add("ml-2");
                    //ahtml.setAttribute("onclick", FilePerson._EventVisualizaFile());
                    ahtml.addEventListener("click", this._EventVisualizaFile, false);
                    ahtml.setAttribute("title", "Ver archivo");
                    ahtml.appendChild(ihtml);
                    DivhtmlBoton.appendChild(ahtml);
                    ElementTd.appendChild(DivhtmlBoton);
                }
                //-----Agrega el boton guardar el archivo---------/// 
                ihtml = document.createElement("i");
                ihtml.style.color = "white";
                ihtml.classList.add("fal");
                ihtml.classList.add("fa-save");
                ahtml = document.createElement("a");
                ahtml.setAttribute("id", ContaRow);
                ahtml.setAttribute("Error", ErrorPopuver);
                ahtml.classList.add("btn");
                ahtml.classList.add("btn-success");
                ahtml.classList.add("btn-sm");
                ahtml.classList.add("ml-2");
                ahtml.addEventListener("click", this._EventEnviarArchivoServer, false);
                ahtml.setAttribute("title", "Guardar archivo");
                ahtml.appendChild(ihtml);
                DivhtmlBoton.appendChild(ahtml);
                ElementTd.appendChild(DivhtmlBoton);
                //------------Agrega el option para las tipologías-----------////
                if (FilePerson.settings.CargaTipologia == 1) {
                    ContadorCelda = ContadorCelda + 1;
                    ElementTd = ElementRow.insertCell(ContadorCelda);
                    ElementTd.classList.add("w-25");
                    let DivhtmlBoton = document.createElement("div");
                    var optionHtml = document.createElement("SELECT");
                    optionHtml.classList.add("form-select");
                    optionHtml.classList.add("w-100");
                    optionHtml.id = "element_input_" + ContaRow;
                    let m_compare = CONTENT_ITEM_ROW_TIPO;
                    let CoinsidePalabra = 0;
                    let indexOtion = 0;
                    if (m_compare != null) {
                        for (let z = 0; z < CONTENT_ITEM_ROW_TIPO.length; z++) {
                            let opt = document.createElement("OPTION");
                            opt.text = CONTENT_ITEM_ROW_TIPO[z].text;
                            opt.value = CONTENT_ITEM_ROW_TIPO[z].value;
                            //Asigna el valor default del campo seleccion
                            let TipoDocument = opt.text.toUpperCase();
                            let NameFile = File.name.toUpperCase();
                            NameFile = NameFile.split('.').slice(0, -1).join('.');
                            let StruName = NameFile.trim().split(/\s+/);
                            let IndexOF = await FilePerson._BuscaCoinsidenciaEstructura(StruName, TipoDocument);
                            if (IndexOF > 0) {
                                if (CoinsidePalabra < IndexOF) {
                                    CoinsidePalabra = IndexOF;
                                    indexOtion = z;
                                }
                            }
                            optionHtml.add(opt);
                        }
                        if (indexOtion > 0) {
                            optionHtml.options[indexOtion].selected = true;
                        }
                    }
                    DivhtmlBoton.appendChild(optionHtml);
                    ElementTd.appendChild(DivhtmlBoton);
                }
                //---------Aagrega el campo fecha--------------------------///
                if (FilePerson.settings.CargaFecha == 1) {
                    ContadorCelda = ContadorCelda + 1;
                    ElementTd = ElementRow.insertCell(ContadorCelda);
                    ElementTd.classList.add("w-25");
                    let imputhml = document.createElement("input");
                    imputhml.id = "element_date_" + ContaRow;
                    LoadDateFormControlFile(imputhml.id);
                    imputhml.addEventListener("keypress", FilePerson._ValidateFechaFormControlFile);
                    imputhml.addEventListener("blur", FilePerson._ValidateFechaFocus);
                    //FilePerson._LoadDateFormControlFile(imputhml);
                    imputhml.placeholder = "yyyy mm dd";
                    imputhml.classList.add("w-100");
                    imputhml.classList.add("form-control-person");
                    imputhml.setAttribute("max-length", "10");
                    ElementTd.appendChild(imputhml);
                    let SpanHtml = document.createElement("SPAN");
                    SpanHtml.classList.add("w-auto");
                    SpanHtml.classList.add("text-danger");
                    SpanHtml.setAttribute("data-asp-danger", imputhml.id);
                    ElementTd.appendChild(SpanHtml);
                }
                HtmLabelEstado.innerText = Files.length + " Archivo(s) Cargado(s)"
                ContaRow = ContaRow + 1;
            }
            FilePerson._BotonSalvaArchivos.style.opacity = "1";
            FilePerson._BotonDeleteArchivos.style.opacity = "1";
            return "YES";
        } catch (ex) {
            return "Inconsistencia general funcion _AdjuntaArchivosDesdeDispostivo " + ex.mensaje
        }
    }
    async _CrearVistaVisualizaFile(file, url) {
        try {
            let wrape = document.getElementById("wrapperfile_001");
            if (wrape) {
                wrape.remove();
            }
            const wrapper = document.createElement("div");
            wrapper.className = "zoomable-wrapper";
            wrapper.id = "wrapperfile_001";
            const controls = document.createElement("div");
            controls.className = "zoom-controls";
            controls.innerHTML = `
   <a class="zoom-out btn btn-warning" type="button" id="bton_hiden_file_preview" title="Cerrar"><i style="color: white" class="far fa-times"></i></a>
  `;
            const content = document.createElement("div");
            content.className = "zoomable-content";
            let preview;
            const ext = file.name.split(".").pop().toLowerCase();
            if (file.type.startsWith("image/")) {
                preview = document.createElement("img");
                preview.src = url;
            } else if (file.type === "application/pdf") {
                preview = document.createElement("iframe");
                preview.src = url;
            } else if (["doc", "docx", "xls", "xlsx", "ppt", "pptx"].includes(ext)) {
                const gview = `https://docs.google.com/gview?url=${encodeURIComponent(url)}&embedded=true`;
                preview = document.createElement("iframe");
                preview.src = gview;
            } else {
                preview = document.createElement("img");
                preview.src = await FilePerson.obtenerIconoPorTipo(file);
            }
            preview.setAttribute("id", "element_preview_file");
            preview.className = "zoom-target-element-file";
            content.appendChild(preview);
            const label = document.createElement("div");
            label.textContent = file.name;
            label.className = "mt-2 small";
            label.setAttribute("AtrNameFile", file.name);
            this._LabelPreview = label;
            label.appendChild(controls);
            content.appendChild(label);
            wrapper.appendChild(content);
            document.getElementById("previewContainer").appendChild(wrapper);
            let htmlBotonHidenPreciew = document.getElementById("bton_hiden_file_preview");
            htmlBotonHidenPreciew.addEventListener("click", this._HidenFilePreview, false);
            this._EventAutoZiseModal();
            return "YES";
        } catch (ex) {
            return "Inconsistencia general funcion  _CrearVistaVisualizaFile " + ex.mensaje;
        }
    }
    obtenerIconoPorTipo(file) {
        const ext = file.name.split(".").pop().toLowerCase();
        const iconos = {
            pdf: "icons/pdf.png",
            doc: "icons/word.png",
            docx: "icons/word.png",
            xls: "icons/excel.png",
            xlsx: "icons/excel.png",
            ppt: "icons/ppt.png",
            pptx: "icons/ppt.png",
            zip: "icons/zip.png",
            txt: "icons/txt.png",
        };
        return iconos[ext] || "icons/file.png";
    }
    async _HidenFilePreview(e) {

        let wrape = document.getElementById("wrapperfile_001");
        if (wrape) {
            wrape.remove();
        }
        FilePerson._EventAutoZiseModal();
    }
    async _PreEnvioArchivosServidor() {
        try {
            let Result = "";
            if (FilePerson.settings.setioption_obliga_tipologia == 1 && FilePerson.settings.CargaTipologia == 1) {
                for (let item of FilePerson.ContenUploadArray) {
                    if (item.DescripcionTipoDocumento === "" || item.IdTipoDocumento === "-1" || item.IdTipoDocumento === "0") {
                        return "Debe seleccionar una tipología para cada archivo cargado.";
                        break;
                    }
                }
            }
            /*** Dependencia del archivo JSPressBar */
            let _OPtionProgresBar = ({
                name_service: "EnviaArchivoServidor",
                OptionItemSelect: FilePerson.ContenUploadArray,
                NameControlPadreProgres: FilePerson.settings.element_parent, NameProceso: "Cargando archivos", ObjectComponente: FilePerson
            });
            /*Envia los archivos al servidor a travez del componente progrees bar y utiliza la funcion _EnviaArchivoServidor*/
            Result = await JSProgresBarBoot(_OPtionProgresBar);
            return Result;
        } catch (ex) {
            return "Inconsistecia general funcion  _PreEnvioArchivosServidor " + ex.mensaje;
        }
    }
    /**
     * Envia un archivo al servidor
     * @param {any} CDarchivoLoad
     */
    async _EnviaArchivoServidor(CDarchivoLoad) {
        try {
            const respuesta = await fetch("../generic_control/fileuploadhandler_.ashx", {
                method: "POST",
                body: CDarchivoLoad.FormData
            });
            if (!respuesta.ok) {
                throw new Error(`Error HTTP: ${respuesta.status}`);
            }
            // Parseo correcto del JSON
            const UploadFilesResult = await respuesta.json();
            if (UploadFilesResult[0].error_sistema !== "YES") {
                return UploadFilesResult[0].error_sistema;
            }
            let Result = await FilePerson._RegistraArchivoInterfaz(UploadFilesResult);
            if (Result != "YES") {
                return Result;
            }
            Result = await FilePerson._DeleteFiles(CDarchivoLoad.id);
            if (Result != "YES") {
                return Result;
            }
            Result = await FilePerson._ValidaCierreModal();
            return Result;
        } catch (ex) {
            return "Inconsistecia general funcion  _EnviaArchivoServidor " + ex.mensaje;
        }
    }
    /**
     * Registra el archivo en la interfaz del usuario
     * @param {any} UploadFilesResult
     */
    async _RegistraArchivoInterfaz(UploadFilesResult) {
        try {
            if (this.settings.funcion_name == "insert_row_producion_documental") {
                var FileIconSome = "fa-file";
                if (UploadFilesResult[0].icono_icono_awe_some != "") {
                    var espacio = " ";
                    var spli_some = UploadFilesResult[0].icono_icono_awe_some.split(espacio);
                    FileIconSome = spli_some[1];
                }
                let DateCampo = "|" + UploadFilesResult[0].id_registro + "|" + UploadFilesResult[0].nombre_archivo + "|" + UploadFilesResult[0].fecha + "|" +
                    UploadFilesResult[0].tipodocumental + "|" + UploadFilesResult[0].name_gabinete + "|" + UploadFilesResult[0].aleas + "|" +
                    UploadFilesResult[0].estado_firma_digital + "|" + FileIconSome + "|" + UploadFilesResult[0].id_imageinsert_row_documento_relacionado
                insert_row_producion_documental(DateCampo); insert_row_documento_relacionado
            }
            if (this.settings.funcion_name == "adjunta_documeto_version_document") {
                let IconoAsome = UploadFilesResult[0].Class_list_detalle_version_document[0].IconoAsome;
                if (FilePerson.settings.TipoTable == "asp.net") {
                    cahange_icono_image_table_asp_net_general(FilePerson.settings.name_class_element_icono_aspnet,
                        UploadFilesResult[0].Class_list_detalle_version_document[0].IconoAsome
                    )
                }
                if (FilePerson.settings.TipoTable == "bootstrap") {
                    let NameCampo = FilePerson.settings.NameCampo;
                    let ValueIcono = 0;
                    //Desactiva la opción de firma digital
                    updateCelByUniqueIdReinit(FilePerson.settings.NameTable, FilePerson.settings.NameCampo, FilePerson.settings.id_imagen, 0);
                    ValueIcono = UploadFilesResult[0].Class_list_detalle_version_document[0].DBT;
                    NameCampo = "DBT";
                    updateCelByUniqueIdReinit(FilePerson.settings.NameTable, NameCampo, FilePerson.settings.id_imagen, ValueIcono);
                }
            }
            if (this.settings.funcion_name == "adjunta_nueva_version_document") {
                let row = {};
                row = new Object();
                row["ID"] = UploadFilesResult[0].Class_list_detalle_version_document[0].ID;
                row["PAG"] = 0;
                row["TIPODOCUMENTO"] = UploadFilesResult[0].Class_list_detalle_version_document[0].TIPO_ARCHIVO;
                row["ESTADO_FIRMA_DIGITAL"] = UploadFilesResult[0].Class_list_detalle_version_document[0].ESTADO_FIRMA_DIGITAL;
                row["IconoAsome"] = UploadFilesResult[0].Class_list_detalle_version_document[0].IconoAsome;
                row["DBT"] = UploadFilesResult[0].Class_list_detalle_version_document[0].DBT;
                row["fecha_registro_version"] = UploadFilesResult[0].Class_list_detalle_version_document[0].fecha_registro_version;
                row["id_version_doc"] = UploadFilesResult[0].Class_list_detalle_version_document[0].id_version_doc;
                row["id_registro_version"] = UploadFilesResult[0].Class_list_detalle_version_document[0].id_registro_version;
                insert_row_table(FilePerson.settings.element_html_table, row);
                let numrowTables = 0;
                numrowTables = total_row_table(FilePerson.settings.element_html_table);
                let html_lable = document.getElementById(FilePerson.settings.element_html_lab_conteo);
                if (html_lable) {
                    html_lable.innerText = FilePerson.settings.apost_html_lab_conteo + " " + numrowTables;
                }
            }
            if (this.settings.funcion_name == "insert_row_documento_relacionado") {
                var file_icon_some = "fa-file"
                if (UploadFilesResult[0].icono_icono_awe_some != "") {
                    file_icon_some = FilePerson._removeFirstClass(UploadFilesResult[0].icono_icono_awe_some);
                }
                var date_campo = UploadFilesResult[0].name_gabinete + "|" + UploadFilesResult[0].id_image + "|" + UploadFilesResult[0].radicado + "|" +
                    UploadFilesResult[0].tipodocumental + "|" + UploadFilesResult[0].notitipodocumental + "|" + UploadFilesResult[0].id_tarea_workflow + "|" +
                    UploadFilesResult[0].estado_firma_digital + "|" + file_icon_some;
                insert_row_documento_relacionado(date_campo, UploadFilesResult, 1);
            }
            //--------Inserta registro interfaz workflow tarea asiganda y enlace------////
            if (this.settings.funcion_name == "InsertRowWorkflowSeleccion") {
                var file_icon_some = "fa-file"
                if (UploadFilesResult[0].icono_icono_awe_some != "") {
                    file_icon_some = FilePerson._removeFirstClass(UploadFilesResult[0].icono_icono_awe_some);
                }
                var date_campo = UploadFilesResult[0].name_gabinete + "|" + UploadFilesResult[0].id_image + "|" + UploadFilesResult[0].radicado + "|" +
                    UploadFilesResult[0].tipodocumental + "|" + UploadFilesResult[0].notitipodocumental + "|" + UploadFilesResult[0].id_tarea_workflow + "|" +
                    UploadFilesResult[0].estado_firma_digital + "|" + file_icon_some;
                insert_row_documento_relacionado(date_campo, this.settings.TipoApost, 1);
            }
            return "YES";
        } catch (ex) {
            return "Inconsistencia funcion _RegistraArchivoInterfaz " + ex.mensaje;
        }
    }


_removeFirstClass(classString) {
    if (!classString) return "";

    let partes = classString.trim().split(/\s+/); // separa en array
    partes.shift(); // elimina la primera clase
    return partes.join(" ");
    }
    /**
     * Solicita la estructura de los archivos cargados en la interfaz
     * y retorna los parametros id--> Identificación del registro
     * name--> Nombre del archivo  IdTipoDocumento--> Indentificación del 
     * la tipologia documental 
     * DescripcionTipoDocumento-> Descripción de la tipoogia documental
     * FormData-> Estructura del forn data
     * CONTENT_UPLOAD_ARRAY
     * @param {any} NameTable
     */
    async _SolicitaEstructuraTablaUpload(NameTable,index){
        try {
            FilePerson.ContenUploadArray = [];
            let Result = "";
            const Tabla = document.getElementById(NameTable);
            const Filas = Tabla.getElementsByTagName("tbody")[0].rows;
            if (Filas.length == 0) {
                return "YES";
            }
            let FileLeng = Filas.length;
            if (index == -1) {
                for (let i = 0; i < FileLeng; i++) {
                    Result = await FilePerson._SolicitaItemTabla(Filas, i);
                    if (Result != "YES") {
                        return Result;
                    }
                }
            } else {
                const filas = Array.from(Filas);
                let IndexFila = filas.findIndex(fila => fila.getAttribute("id") === index);
                if (IndexFila == -1) {
                    return "Imposible emcontar la fila con atributo id : " + index;
                }
                Result = await FilePerson._SolicitaItemTabla(Filas, IndexFila);
                if (Result != "YES") {
                    return Result;
                }
            }
            return "YES";
        } catch (ex) {
            return "Inconsistencia general funcion _SolicitaEstructuraTablaUpload " & ex.mensaje
        }
    }
    async _SolicitaItemTabla(Filas,i) {
        try {
            let Id = Filas[i].getAttribute("id");
            let Error = Filas[i].getAttribute("Error");
            let Result = "";
            if (Error == "1") {
                return "YES";
            }
            let NombreArchivo = "";
            let TextCampoFecha = "";
            let IdTipoDocumento = "0";
            let DescripcionTipoDocumento = "";
            const celdas = Filas[i].cells;
            NombreArchivo = celdas[0].firstChild.textContent;
            let NameControl = "element_date_" + Id;
            let InputHtml;
            if (FilePerson.settings.CargaFecha == 1) {
                InputHtml = document.getElementById(NameControl);
                if (!InputHtml) {
                    return "Imposible econtar el control (" + NameControl + ") en la tabla de arcvhivos pendientes por cargar.";
                }
                TextCampoFecha = InputHtml.value;
                Result = await FilePerson._EsFechaValida(InputHtml.id)
                if (Result !== "YES") {
                    return Result;
                }
            }
            NameControl = "element_input_" + Id;
            let OptionHtml;
            if (FilePerson.settings.CargaTipologia == 1) {
                OptionHtml = document.getElementById(NameControl);
                if (!OptionHtml) {
                    return "Imposible econtar el control (" + NameControl + ") en la tabla de arcvhivos pendientes por cargar.";
                }
                if (OptionHtml.options.length > 0) {
                    IdTipoDocumento = OptionHtml.options[OptionHtml.selectedIndex].value;
                    DescripcionTipoDocumento = OptionHtml.options[OptionHtml.selectedIndex].text;
                }  
            }
            let NombreBuscado = Filas[i].getAttribute("NameFile");
            let InputFile = document.getElementById(FilePerson._NameElementFile);
            const Archivos = Array.from(InputFile.files);
            let File = Archivos.find(file => file.name === NombreBuscado) || null;
            if (!File) {
                return "Imposible encontar el archivo " + NombreBuscado;
            }
            let formdata = new FormData();
            formdata.append("file1", File);
            formdata.append("funcion", this.settings.funcion_name);
            formdata.append("tipo_adjunta", this.settings.tipo_adjunta);
            formdata.append("id_respuesta", this.settings.IdRespuestaIdExpediente);
            formdata.append("evento_adjunta", this.settings.evento_adjunta);
            formdata.append("chek_adjunta_relacionado", this.settings.estado_relacionado);
            formdata.append("chek_adjunta_anexo", this.settings.estado_adjunto);
            formdata.append("num_docu_relacion", this.settings.numero_documento_relacionado);
            formdata.append("id_tipo_documento", IdTipoDocumento);
            formdata.append("gabinete", this.settings.gabinete);
            formdata.append("id_image", this.settings.id_imagen);
            formdata.append("id_expediente", this.settings.IdRespuestaIdExpediente);
            formdata.append("nombre_tipo_documento", DescripcionTipoDocumento);
            formdata.append("name_modulo", this.settings.name_modulo);
            formdata.append("FechaCarga", TextCampoFecha);
            FilePerson.ContenUploadArray.push({
                id: Id, name: NombreArchivo, IdTipoDocumento: IdTipoDocumento,
                DescripcionTipoDocumento: DescripcionTipoDocumento, FormData: formdata
            });
            return "YES";
        } catch (ex) {
            return "Inconsistencia funcion _SolicitaItemTabla " + ex.mensaje;
        }
    }
    /**
     * Valida la exitencia del archivo en la estructura de archvos
     * cargados
     * @param {any} NombreArchivo
     */
    async _SolicitaExistenciaArchivoEstructura(NombreArchivo) {
        try {
            this.FileReturn = null;
            for (let i = 0; i < CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.length; i++) {
                if (NombreArchivo === CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items[i].getAsFile().name) {
                    FilePerson.FileReturn = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items[i].getAsFile();
                    return "YES";
                }
            }
            return "Imposible encontrar el archivo (" + NameArchivo + ") en la estructura cache";
            
        } catch (ex) {
            return "Inconsistencia general funcion _SolicitaExistenciaArchivoEstructura " + ex.mensaje;
        }
    }
    //Agrega los parametros d de multiple seleccion, etensiones permitidas async
    async _ServiceRESTparameterUpload() {
        let myPromise = new Promise(function (resolve) {
            try {
                $.ajax('../webservice/WebServiceProducion.asmx/Service_parameter_upload', {
                    data: "{" + "'parameter':'" + FilePerson.settings.evento_adjunta + "'}",
                    dataType: 'json',
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.d[0].error_result !== "YES") {
                            resolve(data.d[0].error_result);
                        } else {
                            CONTENT_ESTENSION_PERMITIDA = data.d[0].ExtensionPermitida;
                            CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD = data.d[0].Maximo_tamano_archivo_byte;
                            CONTENT_SELECT_FILE_UPLOAD = FilePerson.settings.multi_select;
                            var NameElementFile = FilePerson._NameElementFile;
                            var ElementFile = document.getElementById(NameElementFile);
                            if (ElementFile) {
                                if (FilePerson.settings.multi_select == "") {
                                    ElementFile.removeAttribute("multiple");
                                } else {
                                    ElementFile.setAttribute("multiple", FilePerson.settings.multi_select);
                                }
                                ElementFile.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA);
                                resolve("YES");
                            } else {
                                resolve("Imposible encontrar el control (" + NameElementFile + ")");
                            }
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
    /**
     * Función que solicita la estructura de las tipologías documentales
     * 2025-08-02 Miguel Urueta
     * @param {any} NameControl
     * @param {any} NombreclaseWebService
     * @param {any} NombreWebService
     * @param {any} Parameter
     */
    async _ServiceRESTListTiposDocumentalesFile(NameControl, NombreclaseWebService, NombreWebService, Parameter)  {
        let myPromise = new Promise(function (resolve) {
            try {
                $.ajax('../webservice/' + NombreclaseWebService + '/' + NombreWebService, {
                    data: "{'id':" + "'" + Parameter + "'}",
                    dataType: 'json',
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.d[0].error_sistema !== "YES") {
                            resolve(data.d[0].error_sistema);
                        } else {
                            CONTENT_ITEM_ROW_TIPO = new Array();
                            $.each(data.d[0].item_sistema, function (k, v) {
                                CONTENT_ITEM_ROW_TIPO.push(v);
                            });
                            if (document.getElementById(NameControl)) {
                                var element_drow = document.getElementById(NameControl);
                                $("#" + NameControl).empty();
                                for (var i = 0; i < CONTENT_ITEM_ROW_TIPO.length; i++) {
                                    element_drow[i] = new Option(items_drow[i].text, CONTENT_ITEM_ROW_TIPO[i].value);
                                }
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
    async _BuscaCoinsidenciaEstructura(StruName, StrinCadena) {
        try {
            for (let z = 0; z < StruName.length; z++) {
                let coincidencias = await FilePerson._BuscarCoincidenciaFlexible(StrinCadena, StruName[z], 4);
                if (coincidencias > 0) {
                    return coincidencias;
                }
            }
            return 0;
        } catch (ex) {
            return 0;
        }
    }
    /**
     * /Busca las coinsidencia de una cadena texto desde el minimo numero de
     * carascteres 4 incrementado o decrementado hasta que encuentra culquier
     * coinsidencia   2025-08-02 Miguel urueta
     * @param {any} texto
     * @param {any} palabraBuscada
     * @param {any} min
     */
    async _BuscarCoincidenciaFlexible(texto, palabraBuscada, min) {
        try {
            texto = texto.toLowerCase();
            //Búsqueda exacta...
            palabraBuscada = palabraBuscada.toLowerCase();
            if (texto.includes(palabraBuscada)) {
                return palabraBuscada.length;
            }
            //Búsqueda por aumento...
            for (let i = min; i <= palabraBuscada.length; i++) {
                let subcadena = palabraBuscada.substring(0, i);
                if (texto.includes(subcadena)) {
                    return subcadena.length;
                }
            }
            //Búsqueda por disminución...
            for (let i = palabraBuscada.length; i >= min; i--) {
                let subcadena = palabraBuscada.substring(0, i);
                if (texto.includes(subcadena)) {
                    return subcadena.length;
                }
            }
            //Búsqueda por partes
            for (let i = 0; i <= palabraBuscada.length - min; i++) {
                let parte = palabraBuscada.substring(i, i + min);
                if (texto.includes(parte)) {
                    return parte.length;
                }
            }
            return 0;
        }
        catch (ex) {
            return 0;
            console.log(ex.mensaje);
        }
    }
    async _ValidaCierreModal() {
        try {
            let Table = document.getElementById(this._NameElementTable);
            if (Table.rows.length==0) {
                $(this._ModalLoadDocumento).modal("hide");
            }
            return "YES"
        } catch (ex) {
            return "Inconsistencia funcion _ValidaCierreModal " + ex.mensaje
        }
    }
    /**
     * Elimina un archivo o todos los archivos cargados en la interfaz 
     * @param {any} Index
     */
    async _DeleteFiles(Index) {
    try {
        if (Index == -1) {
            if (FilePerson.FilesCache.files.length > 0) {
                FilePerson.FilesCache.items.clear();
            }
            $("#" + this._NameElementTable + " tr ").remove();
            if (this._LabelArchivos) {
                this._LabelArchivos.files = FilePerson.FilesCache.files;
            }
            this._BotonAdjuntaArchivos.style.opacity = "0";
            this._BotonDeleteArchivos.style.opacity = "0";
            this._BotonSalvaArchivos.style.opacity = "0";
            this._LabelArchivos.innerText = FilePerson.FilesCache.files.length + " Archivo(s) Cargado(s)";
            FilePerson._HidenFilePreview();
        } else {
            let Files = $("#" + FilePerson._NameElementFile).get(0).files;
            File = Files[Index];
            let NameFile = File.name;
            FilePerson.FilesCache.items.remove(Index);
            $("#" + this._NameElementTable + " tr[id=" + Index + "]").remove();
            this._LabelArchivos.innerText = FilePerson.FilesCache.files.length + " Archivo(s) Cargado(s)";
            if (FilePerson.FilesCache.files.length == 0) {
                this._BotonAdjuntaArchivos.style.opacity = "0";
                this._BotonDeleteArchivos.style.opacity = "0";
                this._BotonSalvaArchivos.style.opacity = "0";
                FilePerson._HidenFilePreview();
            } else {
                if (this._LabelPreview) {
                    if (this._LabelPreview.getAttribute("AtrNameFile") == NameFile) {
                        FilePerson._HidenFilePreview();
                    }
                }  
            }
        }
        return 'YES';
    } catch (ex) {
        return "Incosistencia general funcion _DeleteFiles " + ex.mensaje;
    }
    }
     _MostrarLoading() {
        try {
            const loading = document.createElement("div");
            loading.id = "loadingSpinner";
            loading.style.position = "fixed";
            loading.style.top = "0";
            loading.style.left = "0";
            loading.style.width = "100%";
            loading.style.height = "100%";
            loading.style.backgroundColor = "rgba(255, 255, 255, 0.8)";
            loading.style.display = "flex";
            loading.style.justifyContent = "center";
            loading.style.alignItems = "center";
            loading.style.zIndex = "110061";
            const spinner = document.createElement("div");
            spinner.className = "spinner-file";
            loading.appendChild(spinner);
            document.body.appendChild(loading);
            return "YES";
        } catch (ex) {
            return "Inconsistencia general funcion _MostrarLoading " + ex.mensaje;
        }
}

 _OcultarLoading() {
    const loading = document.getElementById("loadingSpinner");
    if (loading) loading.remove();
}
}

let FilePerson;
const IniLoadPerson = async (Option) => {
    FilePerson = new LoadFilePERSON(Option);
    let result = await FilePerson.LoadPersonFile();
    return result;
}

function start_file_save_UploadFile_default() {
    let name_element = "DropDownList_" + FilePerson.settings.funcion_name;
    let drow_tipo = document.getElementById(name_element);
    if (drow_tipo) {
        if (drow_tipo.value != "" && drow_tipo.value != "-1") {
            FilePerson.settings.name_tipo_documento = drow_tipo.options[drow_tipo.selectedIndex].text;
            FilePerson.settings.id_tipo_docuental = drow_tipo.options[drow_tipo.selectedIndex].value;
        }
    }
    if (FilePerson.settings.option_obliga_tipologia != 0) {
        if (FilePerson.settings.id_tipo_docuental == -1 || FilePerson.settings.id_tipo_docuental == 0) {
            alert("Por favor, seleccione el tipo de documento aplicable para continuar con el proceso.");
            return true;
        }
    }
    star_copy_interval_file_Upload(FilePerson.settings.estado_adjunto, FilePerson.settings.estado_relacion, FilePerson.settings.id_tipo_docuental, FilePerson.settings.funcion_name, FilePerson.settings.element_parent, FilePerson.settings.evento_adjunta,
        FilePerson.settings.numero_documento_relacionado, FilePerson.settings.element_html_actuliza, FilePerson.settings.element_update_panel, FilePerson.settings.id_respuesta, FilePerson.settings.tipo_adjunta,
        FilePerson.settings.element_isert_table, FilePerson.settings.name_tipo_documento, FilePerson.settings.gabinete, FilePerson.settings.id_imagen, FilePerson.settings.name_modulo);
}
function hide_upload_content(element_parent_close) {
    try {
        var element_parent = document.getElementById(element_parent_close);
        var element_parent = $find(element_parent_close);
        if (element_parent) {
            if (CONTENT_UPLOAD_ELEMENT_INTERVAL !== 0) {
                alert("Se esta cargando un archivo, imposible cerrar ventana");
                return true;
            } else {
                clearInterval(CONTENT_UPLOAD_ELEMENT_INTERVAL);
                CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
                element_parent.hide();
            }
            
        }
    } catch (err) {
        alert(" funcion hide_upload_content " + err.mensaje);
    }
}
//----------------------------------------------------------
//Funcion inicializa botones de cargar, eliminar y cancelar
//----------------------------------------------------------
const InitUploadFileClientAsync = async (element, name_fucion_star_file) => {
    let myPromise = new Promise((resolve) => {
        try {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                delete_alert_boot();
            }
            CONTEN_NAME_UPLOAD_FILE = element;
            CONTENT_ESTENSION_PERMITIDA = "";
            CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
            var name_conten_file_element = "conten_file_element_" + element;  
            document.getElementById(name_conten_file_element).addEventListener('drop', drop_Upload_file);
            document.getElementById(name_conten_file_element).addEventListener("dragenter", dragenter);
            document.getElementById(name_conten_file_element).addEventListener("dragover", dragover);
            var name_delete_file_element = "delete_file_element_" + element;
            document.getElementById(name_delete_file_element).addEventListener('click', delete_file_all_UploadFile);
            var element_satar = "file_element_" + element;
            document.getElementById(element_satar).addEventListener('change', InsertaArchivosFront);
            var name_cancel_file_element = "cancel_file_element_" + element;
            var element_satar = "file_element_" + element;
            document.getElementById(name_cancel_file_element).addEventListener('click', cancel_file_all_upload);
            var name_save_file_element = "save_file_element_" + element;
            switch (name_fucion_star_file) {
                case "start_file_save_UploadFile_document_verion":
                    document.getElementById(name_save_file_element).addEventListener('click', start_file_save_UploadFile_document_verion);
                    break;
                case "start_file_save_UploadFile":
                    document.getElementById(name_save_file_element).addEventListener('click', start_file_save_UploadFile);
                    break;
                default:
                    document.getElementById(name_save_file_element).addEventListener('click', start_file_save_UploadFile_default);
                    break;
            } 
            delete_file_all_UploadFile();
            resolve("YES");

        } catch (err) {
            return resolve(err.mensaje + " function InitUploadFileClientAsync")
           
        }
    });
    let result = await myPromise;
    return result;
    
}

//-------------------------------------------------------------
//Inserta los archivos cargados de dispositivo a la interface
//-------------------------------------------------------------
/**
 * Valida el formato fecha yyyy-mm-dd con la perdida del foco del elemento
 * @param {any} e
 */
function ValidateFechaFocus(e) {
    try {
        let value = e.currentTarget.value;
        let SpanDanger = document.querySelector('span[data-asp-danger="' + e.currentTarget.id + '"]');
        if (value == "") {
            e.currentTarget.style.background = "";
            SpanDanger.textContent = "";
            return "YES";
        }
        const Actual = new Date().getFullYear();
        let year = value.substring(0, 4);
        let month = value.substring(5, 7);
        let day = value.substring(8, 10);
        SpanDanger.textContent = "";
        if (Actual < year) {
            e.currentTarget.focus();
            e.currentTarget.setSelectionRange(0, 4);
            e.currentTarget.style.background = "#ffffcc";
            SpanDanger.textContent = "El año ingresado no es valido.";
            return "YES";
        }
        if (month != "" && (month > 12 || month < 1)) {
            e.currentTarget.focus();
            e.currentTarget.setSelectionRange(5, 7);
            e.currentTarget.style.background = "#f7d2d2";
            SpanDanger.textContent = "El mes ingresado no es válido.";
            return "YES";
        }
        if (day != "" && (day > 31 || day < 1)) {
            e.currentTarget.focus();
            e.currentTarget.setSelectionRange(8, 10);
            e.currentTarget.style.background = "#f7d2d2";
            SpanDanger.textContent = "El dia ingresado no es válido.";
            return "YES";
        }
        let leng = value.length;
        if (leng != 10) {
            e.currentTarget.focus();
            e.currentTarget.setSelectionRange(0, leng);
            e.currentTarget.style.background = "#f7d2d2";
            SpanDanger.textContent = "Tamaño de fecha no valida.";
            return "YES";
        }
        e.currentTarget.style.background = "";
    } catch (ex) {
        alert("function ValidateFechaFocus " + ex.mensaje)
    }
}
/**
 * Valida y transforma la fecha en el formato yyyy-mm-dd al momento de digitar
 * @param {any} e
 */
function ValidateFechaFormControlFile(e) {
    try {

        let tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 32) {
            return false;
        }
        let patron = /^[0-9 ]$/;
        var te = String.fromCharCode(tecla);
        var res = patron.test(te);
        if (res) {
            if (e.currentTarget.value.length == 4 || e.currentTarget.value.length == 7) {
                e.currentTarget.value = e.currentTarget.value + "-";
                return patron.test(te);
            }
        } else {
            e.preventDefault();
            return false;
        }
    } catch (err) {
        alert(err.message + " funcion ValidateFechaFormControlFile " + err.message);
    }
}
function LoadDateFormControlFile(name) {
    try {
        let control = document.getElementById(name);
        let value_cache = "";
        if (control) {
            value_cache = control.value;
        }
        $('#' + name).datepicker({
            autoclose: false,
            dateFormat: "yy-mm-dd"
        });
        $('#' + name).css('zIndex', 99999999);
    } catch (ex) { alert(ex.mensaje + " funcion  LoadDateFormControlFile") }

}
async function InsertaArchivosFront()  {
    try {
        var name_element_file = "file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var element_file = document.getElementById(name_element_file);
        var name_a_save_file_element = "save_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var a_save_file_element = document.getElementById(name_a_save_file_element);
        if (!a_save_file_element) {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_a_save_file_element + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_a_save_file_element + ")");
                return true;
            }
        }
        var name_a_cancel_file_element = "cancel_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var a_cancel_file_element = document.getElementById(name_a_save_file_element);
        if (!a_cancel_file_element) {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_a_cancel_file_element + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_a_cancel_file_element + ")");
                return true;
            }
        }
        var name_a_delete_file_element = "delete_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var a_delete_file_element = document.getElementById(name_a_delete_file_element);
        if (!a_delete_file_element) {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_a_delete_file_element + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_a_delete_file_element + ")");
                return true;
            }
        }
        var name_Label_progres_bar_file_element = "Label_progres_bar_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var Label_progres_bar_file_element = document.getElementById(name_Label_progres_bar_file_element);
        if (!Label_progres_bar_file_element) {
        
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_Label_progres_bar_file_element + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_Label_progres_bar_file_element + ")");
                return true;
            }
        }
        var name_pogres_file_element_contador_demo = "pogres_file_element_contador_" + CONTEN_NAME_UPLOAD_FILE;
        var pogres_file_element_contador_demo = document.getElementById(name_pogres_file_element_contador_demo);
        if (!pogres_file_element_contador_demo) {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_pogres_file_element_contador_demo + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_pogres_file_element_contador_demo + ")");
                return true;
            }
        }
        var name_pogres_file_element_porcent_demo = "pogres_file_element_porcent_" + CONTEN_NAME_UPLOAD_FILE;
        var pogres_file_element_porcent_demo = document.getElementById(name_pogres_file_element_porcent_demo);
        if (!pogres_file_element_porcent_demo) {
       
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_pogres_file_element_porcent_demo + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_pogres_file_element_porcent_demo + ")");
                return true;
            }
        }
        var name_myProgress_file_element_demo = "myProgress_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var myProgress_file_element_demo = document.getElementById(name_myProgress_file_element_demo);
        if (!myProgress_file_element_demo) {
      
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_myProgress_file_element_demo + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_myProgress_file_element_demo + ")");
                return true;
            }
        }
        var name_element_p_count_UploadFile = "count_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var element_p_count_UploadFile = document.getElementById(name_element_p_count_UploadFile);
        if (!element_p_count_UploadFile) {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_element_p_count_UploadFile + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_element_p_count_UploadFile + ")");
                return true;
            }
        }
        var name_count_byte_file_element_UploadFile = "count_byte_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var element_count_byte_file_UploadFile = document.getElementById(name_count_byte_file_element_UploadFile);
        if (!element_count_byte_file_UploadFile) {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_count_byte_file_element_UploadFile + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_count_byte_file_element_UploadFile + ")");
                return true;
            }
        }
        var input = document.getElementById(name_element_file);
        if (!input) {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_element_file + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_element_file + ")");
                return true;
            }
        }
        var name_elment_table = "table_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var table_content = document.getElementById(name_elment_table);
        if (!table_content) {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar el control (" + name_elment_table + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar el control (" + name_elment_table + ")");
                return true;
            }
           
        }
        //---Adiciona los archivos a load tranfer general
        var files = $("#" + name_element_file).get(0).files;
        for (let file of files) {
            let valida_ext = ValidateFileType(file.name);
            if (valida_ext == "none") {
                if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                    alert_bot("El archivo (" + file.name + ") pertence a una extensión no permitida, el archivo no se cargará.", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                } else {
                    alert("El archivo (" + file.name + ") pertence a una extensión no permitida, el archivo no se cargará.");
                }
            }
            let valida_tam = "yes";
            let file_size = 0;
            file_size = returnFileSize(file.size);
            let file_size_per = returnFileSize(CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD);
            if (file.size > CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD) {
                valida_tam = "no"
                if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                    alert_bot("El archivo (" + file.name + ") de (" + file_size + ") supera el tamaño permitido (" + file_size_per + ") , el archivo no se cargará.", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                } else {
                    alert("El archivo (" + file.name + ") de (" + file_size + ") supera el tamaño permitido (" + file_size_per + ") , el archivo no se cargará.");
                }
            }
            if (valida_ext == "yes" && valida_tam == "yes") {
                CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.add(file);
            }
        }
        CONTEN_NUM_UPLOAD_FILE = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files.length;
        files.files = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files;
        input.files = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files;
        if (table_content) {
        } else {
            if (CONTENT_NAME_CONTENDOR_ERROR != "") {
                alert_bot("Imposible encontrar la tabla (" + CONTEN_NAME_UPLOAD_FILE + ")", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
                return true;
            } else {
                alert("Imposible encontrar la tabla (" + CONTEN_NAME_UPLOAD_FILE + ")");
                return true;
            }
        }
        var name_content_drop_element = "content_drop_element_" + CONTEN_NAME_UPLOAD_FILE;
        var content_drop_element = document.getElementById(name_content_drop_element);
        var curFiles = input.files;
        if (curFiles.length === 0) {
            content_drop_element.style.opacity = "1";

        } else {
            if (table_content.rows.length >= 0) {
                $("#" + name_elment_table).find("tr").remove();
            }
            content_drop_element.style.opacity = "0";
            var conta_row = 0;
            for (const file of curFiles) {
                var file_size = 0;
                file_size = returnFileSize(file.size);
                let file_size_per = returnFileSize(CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD);
                if (file.size > CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD) {
                    //alert("El archivo (" + file.name + ") supera el tamaño permitido (" + file_size_per + ")");
                    //delete_file_UploadFileName(file.name);      
                } else {
                    var conta_td = 0;
                    var element_row = table_content.insertRow(table_content.rows.length);
                    var element_td = element_row.insertCell(conta_td);
                    element_td.classList.add("w-25");
                    element_row.setAttribute("id", conta_row);
                    element_row.style.cursor = "pointer";
                    element_row.style.background = "white";
                    element_row.style.color = "black";
                    //Agrga celda nombre
                    var divhtml_title = document.createElement("div");
                    var p = document.createElement('divp');
                    p.classList.add("font-weight-light");
                    p.textContent = file.name;
                    p.setAttribute("id_element", file.name);
                    divhtml_title.appendChild(p);
                    element_td.appendChild(divhtml_title);
                    //Agrega celda tamano
                    conta_td = conta_td + 1;
                    element_td = element_row.insertCell(conta_td);
                    var divhtml_tamano = document.createElement("div");
                    p = document.createElement('div');
                    p.classList.add("font-weight-light");
                    p.textContent = file_size;
                    divhtml_tamano.appendChild(p);
                    element_td.appendChild(divhtml_tamano);
                    
                    //--Agrega celda boton eliminar------///
                    conta_td = conta_td + 1;
                    element_td = element_row.insertCell(conta_td);
                    var divhtml_boton = document.createElement("div");
                    var ihtml = document.createElement("i");
                    ihtml.style.color = "white";
                    ihtml.classList.add("fal");
                    ihtml.classList.add("fa-trash-alt");
                    var ahtml = document.createElement("a");
                    ahtml.setAttribute("id", conta_row);
                    ahtml.classList.add("btn");
                    ahtml.classList.add("btn-danger");
                    ahtml.classList.add("btn-sm");
                    ahtml.setAttribute("onclick", " prevent_UploadFile(event,this);");
                    ahtml.setAttribute("title", "Eliminar archivo");
                    ahtml.appendChild(ihtml);
                    divhtml_boton.appendChild(ahtml);
                    element_td.appendChild(divhtml_boton);
                    //-----Agrega el boton de vizualización---------///
                    if (CONTENT_PROCESO == "PRODUCCION") {
                        ihtml = document.createElement("i");
                        ihtml.style.color = "white";
                        ihtml.classList.add("fal");
                        ihtml.classList.add("fa-file");
                        ahtml = document.createElement("a");
                        ahtml.setAttribute("id", conta_row);
                        ahtml.classList.add("btn");
                        ahtml.classList.add("btn-primary");
                        ahtml.classList.add("btn-sm");
                        ahtml.classList.add("ml-2");
                        ahtml.setAttribute("onclick", "PrevenFileVisualizar(event,this);");
                        ahtml.setAttribute("title", "Ver archivo");
                        ahtml.appendChild(ihtml);
                        divhtml_boton.appendChild(ahtml);
                        element_td.appendChild(divhtml_boton);
                    }
                    //------------Agrega el option para las tipologías-----------////
                    if (CONTENT_PROCESO == "PRODUCCION") {
                        conta_td = conta_td + 1;
                        element_td = element_row.insertCell(conta_td);
                        element_td.classList.add("w-25");
                        var divhtml_boton = document.createElement("div");
                        var optionHtml = document.createElement("SELECT");
                        optionHtml.classList.add("form-select");
                        optionHtml.classList.add("w-100");
                        optionHtml.id = "element_input_" + conta_row;
                        let m_compare = CONTENT_ITEM_ROW_TIPO;
                        if (m_compare != null) {
                            for (let z = 0; z < CONTENT_ITEM_ROW_TIPO.length; z++) {
                                let opt = document.createElement("OPTION");
                                opt.text = CONTENT_ITEM_ROW_TIPO[z].text;
                                opt.value = CONTENT_ITEM_ROW_TIPO[z].value;
                                //Asigna el valor default del campo seleccion
                                if (opt.text.indexOf(file.name) !== -1) {
                                    opt.selected = true;
                                }
                                optionHtml.add(opt);
                            }
                        }
                        divhtml_boton.appendChild(optionHtml);
                        element_td.appendChild(divhtml_boton);
                    }
                    //---------Aagrega el campo fecha--------------------------///
                    if (CONTENT_PROCESO == "PRODUCCION") {
                        conta_td = conta_td + 1;
                        element_td = element_row.insertCell(conta_td);
                        element_td.classList.add("w-25");
                        var imputhml = document.createElement("INPUT");
                        imputhml.id = "element_date_" + conta_row;
                        LoadDateFormControlFile(imputhml.id);
                        imputhml.addEventListener("keypress", ValidateFechaFormControlFile);
                        imputhml.addEventListener("blur", ValidateFechaFocus);
                        imputhml.placeholder = "yyyy mm dd";
                        imputhml.classList.add("w-100");
                        imputhml.classList.add("form-control-person");
                        imputhml.setAttribute("max-length", "10");
                        element_td.appendChild(imputhml);
                        let SpanHtml = document.createElement("SPAN");
                        SpanHtml.classList.add("w-auto");
                        SpanHtml.classList.add("text-danger");
                        SpanHtml.setAttribute("data-asp-danger", imputhml.id);
                        element_td.appendChild(SpanHtml);
                    }
                    element_p_count_UploadFile.innerText = curFiles.length + " Archivo(s) Cargado(s)"
                    conta_row = conta_row + 1;
                }
            }
            a_save_file_element.style.opacity = "1";
            a_delete_file_element.style.opacity = "1";
        }
    } catch (err) {
        if (CONTENT_NAME_CONTENDOR_ERROR != "") {
            alert_bot(err.message + " Funcion InsertaArchivosFront", 'warning', CONTENT_NAME_CONTENDOR_ERROR);
        } else {
            alert(err.message + " Funcion InsertaArchivosFront");
        }
       
    }
}
function inicializa_upload_file_client(element) {
    try {
        if (CONTENT_NAME_CONTENDOR_ERROR != "") {
            delete_alert_boot();
        }
        CONTENT_NAME_CONTENDOR_ERROR = "";
        CONTEN_NAME_UPLOAD_FILE = element;
        CONTENT_ESTENSION_PERMITIDA = "";
        CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
        var name_conten_file_element = "conten_file_element_" + element;
        document.getElementById(name_conten_file_element).addEventListener('drop', drop_Upload_file);
        document.getElementById(name_conten_file_element).addEventListener("dragenter", dragenter);
        document.getElementById(name_conten_file_element).addEventListener("dragover", dragover);
        var name_delete_file_element = "delete_file_element_" + element;
        document.getElementById(name_delete_file_element).addEventListener('click', delete_file_all_UploadFile);
        var element_satar = "file_element_" + element;
        document.getElementById(element_satar).addEventListener('change', InsertaArchivosFront);    
        var name_cancel_file_element = "cancel_file_element_" + element;
        var element_satar = "file_element_" + element;
        document.getElementById(name_cancel_file_element).addEventListener('click', cancel_file_all_upload);
        var name_save_file_element = "save_file_element_" + element;
        document.getElementById(name_save_file_element).addEventListener('click', start_file_save_UploadFile);
        delete_file_all_UploadFile();
        
    } catch (err) {
        alert(err.mensaje + " function inicializa_upload_file_client");
    }
}
//-------------------------------------------
//Funcion inicializa envio del archivo
//-------------------------------------------
function UploadFile() {
    var files = $("#file1").get(0).files;
    counter = 1;
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var formdata = new FormData();
        formdata.append("file1", file);
        var ajax = new XMLHttpRequest();
        ajax.upload.addEventListener("progress", progressHandler, false);
        ajax.addEventListener("load", completeHandler, false);
        ajax.addEventListener("error", errorHandler, false);
        ajax.addEventListener("abort", abortHandler, false);
        ajax.open("POST", "../generic_control/fileuploadhandler_.ashx");
        ajax.send(formdata);
    }
}
//-----------------------------------------------------------------------
//Funcion envia los archivos async al servidor depedencia JSProgress.js
//-----------------------------------------------------------------------

//-------------------------------
//Envia los archivos al servidor
//--------------------------------
function star_copy_interval_file_Upload(estado_adjunto,
    estado_relacionado, id_tipo_documental, name_funcion,
    element_parent_close, evento_adjunta, num_docu_relacion,
    element_html_actuliza, element_update_panel,
    id_respuesta_id_expediente, tipo_adjunta, insert_table,
    nombre_tipo_documental, gabinete, id_imagen, name_modulo) {
    try {
        CONTENT_ELEMENT_UPDATE_PANEL_UPLOAD = element_update_panel;
        CONTENT_ELEMENT_HTML_UPLOAD = element_html_actuliza;
        CONTENT_UPLOAD_ID_TIPO_DOCUMENTAL = id_tipo_documental;
        CONTENT_UPLOAD_ESTADO_CHEK_ADJUNTO = estado_adjunto;
        CONTENT_ESTADO_NUMERO_RELACIONADO = estado_relacionado; 
        CONTENT_ERROR_UPLOAD = "";
        CONTEN_NUM_UPLOAD_INCRE_FILE = 1;
        CONTENT_UPLOAD_MULTIPLE_ESTATUS_SERVICE = "YES";
        CONTENT_ELEMENT_PARENT_UPLOAD = element_parent_close;
        CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
        CONTENT_ELEMENT_INSERT_TABLE_UPLOAD = insert_table;
        var name_elment_table = "table_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var elment_table = document.getElementById(name_elment_table);
        var parent_close = document.getElementById(element_parent_close);
        var total_rows = elment_table.rows.length;
        CONTEN_NUM_UPLOAD_FILE = elment_table.rows.length;
        CONTENT_NAME_UPLOAD_FUNCION = name_funcion;
        var ini_rows = 0;
        solicita_stru_table_Upload(name_elment_table);
        CONTENT_UPLOAD_ELEMENT_INTERVAL = setInterval(frame, 50);
        function frame() {
            if (CONTENT_UPLOAD_MULTIPLE_ESTATUS_SERVICE == "YES") {
                if (ini_rows >= total_rows) {
                    set_progres_Upload();
                    CONTENT_UPLOAD_MULTIPLE_ESTATUS_SERVICE = "";
                    var element_parent = $find(element_parent_close);
                    if (element_parent) {
                        element_parent.hide();
                    }
                    clearInterval(CONTENT_UPLOAD_ELEMENT_INTERVAL);
                    CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
                } else {
                    var file = null;
                    var nombre_archivo = CONTENT_UPLOAD_ARRAY[ini_rows].name;
                    CONTENT_ID_UPLOAD = CONTENT_UPLOAD_ARRAY[ini_rows].id;
                    file = solicita_file_archivo_UploadFile(nombre_archivo);
                    if (file === null) {
                        set_progres_Upload();
                        alert("Imposible encontrar el nombre  (" + nombre_archivo + ") en el file data  funcion star_copy");
                        CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
                        return false;
                    }
                    var formdata = new FormData();
                    formdata.append("file1", file);
                    formdata.append("funcion", name_funcion);
                    formdata.append("tipo_adjunta", tipo_adjunta);
                    formdata.append("id_respuesta", id_respuesta_id_expediente);
                    formdata.append("evento_adjunta", evento_adjunta);
                    formdata.append("chek_adjunta_relacionado", estado_relacionado);
                    formdata.append("chek_adjunta_anexo", estado_adjunto);
                    formdata.append("num_docu_relacion", num_docu_relacion);
                    formdata.append("id_tipo_documento", id_tipo_documental);
                    formdata.append("gabinete", gabinete);
                    formdata.append("id_image", id_imagen);
                    formdata.append("id_expediente", id_respuesta_id_expediente);
                    formdata.append("nombre_tipo_documento", nombre_tipo_documental);
                    formdata.append("name_modulo", name_modulo);
                    CONTENT_UPLOAD_MULTIPLE_ESTATUS_SERVICE = "";
                    CONTENT_ERROR_UPLOAD = "";
                    runXHR_Upload("../generic_control/fileuploadhandler_.ashx", formdata);
                    ini_rows++;
                }
            }
        }
    } catch (err) {
        clearInterval(CONTENT_UPLOAD_ELEMENT_INTERVAL);
        CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
        alert(err.message + " funcion star_copy_interval_file_Upload");
    }
}

//------------------------------------------------------
//Solicita la lista de la estructura cargada al front
//------------------------------------------------------
function solicita_stru_table_Upload(name_elment_table) {
    try {
        CONTENT_UPLOAD_ARRAY = [];   
        $("#" + name_elment_table + " tr ").each(function () {     
            var id = -1;
            var nombre_archivo = "";
            var file = null;
            id = $(this).attr("id");
            //CONTEN_ID_UPLOAD_DELETE = id;
            if (id == -1) {
                alert("Imposible encontrar el id del record (" + CONTEN_NUM_UPLOAD_INCRE_FILE + ")");
                return false;
            }
            nombre_archivo = $(this)[0].cells[0].firstChild.textContent;
            //CONTEN_NAME_FILE_DELETE = nombre_archivo;
            if (nombre_archivo == "") {
                alert("Imposible encontrar el nombre del id (" + id + ") ");
                return false;
            }
            file = solicita_file_archivo_UploadFile(nombre_archivo);
            if (file === null) {
                alert("Imposible encontrar el nombre  (" + nombre_archivo + ") en el file data ");
                CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
                return false;
            }
           
            CONTENT_UPLOAD_ARRAY.push({ id: id, name: nombre_archivo});
            
        });
    } catch (err) {
        alert(err.message + " funcion solicita_stru_table_Upload")
    }
}
//----------------------------------------------------
//Inicializa controles de progreso y bonton cancelar
//----------------------------------------------------
function set_progres_Upload() {
    try {
        var elem = document.getElementById("myBar_file_element_" + CONTEN_NAME_UPLOAD_FILE);
        var elment_progres = document.getElementById("pogres_file_element_porcent_" + CONTEN_NAME_UPLOAD_FILE);
        var elment_conta = document.getElementById("pogres_file_element_contador_" + CONTEN_NAME_UPLOAD_FILE);
        if (elem && elment_progres && elment_conta) {
            elem.style.width = 0 + '%';
            elment_progres.innerHTML = "";
            elment_conta.innerHTML = "";
        }
        var name_cancel_file_element = "cancel_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var cancel_file_element = document.getElementById(name_cancel_file_element);
        if (cancel_file_element) {
            cancel_file_element.style.opacity = "0";
        }
        var name_count_byte_file_element_UploadFile = "count_byte_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var element_count_byte_file_UploadFile = document.getElementById(name_count_byte_file_element_UploadFile);
       
        if (element_count_byte_file_UploadFile) {
            $("#" + name_count_byte_file_element_UploadFile).html("");
        }
    } catch (err) {

        alert(err.message + " funcion set_progres_Upload")
    }
}
//----------------------------------------------------
//Agrega los eventos del ajax 
//----------------------------------------------------
function addListeners(ajax) {
    ajax.upload.addEventListener("progress", progressHandler, true);
    ajax.addEventListener("load", completeHandler, false);
    ajax.addEventListener("error", errorHandler, false);
    ajax.addEventListener("abort", abortHandler, false);
    ajax.addEventListener('loadend', loadenEvent, false);
    ajax.addEventListener('loadstart', loadstartEvent, false);
    ajax.addEventListener('timeout', timeoutEvent, false);
}
//----------------------------------------------------
//Envia el archivo
//----------------------------------------------------
function runXHR_Upload(url, formdata) {
    try {
        ajax = new XMLHttpRequest();
        addListeners(ajax);
        ajax.open("POST", url, true);
        ajax.send(formdata);
    } catch (err) {
        alert(err.message + " funcion runXHR_Upload");
    }
}

//---------------------------------------------------------
//Registra en las interfaces front los archivos guardados
//---------------------------------------------------------
function insert_runtHXR() {
    try {
        var elem = document.getElementById("myBar_file_element_" + CONTEN_NAME_UPLOAD_FILE);
        var elment_progres = document.getElementById("pogres_file_element_porcent_" + CONTEN_NAME_UPLOAD_FILE);
        var elment_conta = document.getElementById("pogres_file_element_contador_" + CONTEN_NAME_UPLOAD_FILE);
        var name_count_byte_file_element_UploadFile = "count_byte_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var element_count_byte_file_UploadFile = document.getElementById(name_count_byte_file_element_UploadFile);    
        CONTENT_RESULT_UPDATE_FILE = JSON.parse(ajax.responseText);          
        delete_file_UploadFile(CONTENT_ID_UPLOAD);
        if (CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files.length == 0) {
                        elem.style.width = 0 + '%';
                        elment_progres.innerHTML = "";
            elment_conta.innerHTML = "";
            if (element_count_byte_file_UploadFile) {
                $("#" + name_count_byte_file_element_UploadFile).html("");
            }
        } else {
                      
        }
        
        if (CONTENT_NAME_UPLOAD_FUNCION == "insert_row_producion_documental") {
            var file_icon_some = "fa-file";
            if (CONTENT_RESULT_UPDATE_FILE[0].icono_icono_awe_some != "") {
                var espacio = " ";
                var spli_some = CONTENT_RESULT_UPDATE_FILE[0].icono_icono_awe_some.split(espacio);
                file_icon_some = spli_some[1];
            }
            var date_campo = "|" + CONTENT_RESULT_UPDATE_FILE[0].id_registro + "|" + CONTENT_RESULT_UPDATE_FILE[0].nombre_archivo + "|" + CONTENT_RESULT_UPDATE_FILE[0].fecha + "|" +
                CONTENT_RESULT_UPDATE_FILE[0].tipodocumental + "|" + CONTENT_RESULT_UPDATE_FILE[0].name_gabinete + "|" + CONTENT_RESULT_UPDATE_FILE[0].aleas + "|" +
                CONTENT_RESULT_UPDATE_FILE[0].estado_firma_digital + "|" + file_icon_some + "|" + CONTENT_RESULT_UPDATE_FILE[0].id_image;
                insert_row_producion_documental(date_campo);
        }
        if (CONTENT_NAME_UPLOAD_FUNCION == "insert_row_documento_relacionado") {
            var file_icon_some = "fa-file"
            if (CONTENT_RESULT_UPDATE_FILE[0].icono_icono_awe_some != "") {
                var espacio = " ";
                var spli_some = CONTENT_RESULT_UPDATE_FILE[0].icono_icono_awe_some.split(espacio);
                file_icon_some = spli_some[1];
            }
            var date_campo = CONTENT_RESULT_UPDATE_FILE[0].name_gabinete + "|" + CONTENT_RESULT_UPDATE_FILE[0].id_image + "|" + CONTENT_RESULT_UPDATE_FILE[0].radicado + "|" +
                CONTENT_RESULT_UPDATE_FILE[0].tipodocumental + "|" + CONTENT_RESULT_UPDATE_FILE[0].notitipodocumental + "|" + CONTENT_RESULT_UPDATE_FILE[0].id_tarea_workflow + "|" +
                CONTENT_RESULT_UPDATE_FILE[0].estado_firma_digital + "|" + file_icon_some;
            insert_row_documento_relacionado(date_campo, CONTENT_ELEMENT_INSERT_TABLE_UPLOAD,1);
        }
        //Visualiza imagen de remplazo para migración 
        if (CONTENT_NAME_UPLOAD_FUNCION == "adjunta_migra_documento") {
            document.getElementById("Iframe_visor_pdf").src = CONTENT_RESULT_UPDATE_FILE[0].url;
            ID_REGISTRO_MIGRA = CONTENT_RESULT_UPDATE_FILE[0].id_registro;
            $("#modal_adjunta_documeto_migra").modal("hide");
        }       
        if (CONTENT_NAME_UPLOAD_FUNCION == "actualiza_contador_imagen") {
            var element_html_update = document.getElementById(CONTENT_ELEMENT_HTML_UPLOAD);
            if (element_html_update) {
                element_html_update.value = CONTENT_RESULT_UPDATE_FILE[0].contador_paginas;
                element_html_update.innerHTML = CONTENT_RESULT_UPDATE_FILE[0].contador_paginas;
                element_html_update.innerText = CONTENT_RESULT_UPDATE_FILE[0].contador_paginas;
            }
            var UpdatePanel = document.getElementById(CONTENT_ELEMENT_UPDATE_PANEL_UPLOAD);
            if (UpdatePanel) {
                UpdatePanel.click();
            }
           
        }
        if (CONTENT_NAME_UPLOAD_FUNCION == "actualiza_semaforo_respuesta") {
            var element_html_update = document.getElementById(CONTENT_ELEMENT_HTML_UPLOAD);
            if (element_html_update) {
                document.getElementById(CONTENT_ELEMENT_HTML_UPLOAD).src = CONTENT_RESULT_UPDATE_FILE[0].url_image_semaforo;                    
            }
        }
        if (CONTENT_NAME_UPLOAD_FUNCION == "actualiza_drowp_respuesta") {
            var element_html_update = document.getElementById(CONTENT_ELEMENT_HTML_UPLOAD);
            if (element_html_update) {
                var option = document.createElement("option");
                option.text = CONTENT_RESULT_UPDATE_FILE[0].nombre_anexo;
                option.value = CONTENT_RESULT_UPDATE_FILE[0].id_anexo;
                document.getElementById(CONTENT_ELEMENT_HTML_UPLOAD).add(option);
                var dorop_list_simple = document.getElementById("DropDownList_anexos_respuesta_simple");
                if (dorop_list_simple) {
                    var option = document.createElement("option");
                    option.text = CONTENT_RESULT_UPDATE_FILE[0].nombre_anexo;
                    option.value = CONTENT_RESULT_UPDATE_FILE[0].id_anexo;
                    dorop_list_simple.add(option);
                }
            }
        }
        if (CONTENT_NAME_UPLOAD_FUNCION == "actualiza_drowp_pqrs") {
            var element_html_update = document.getElementById(CONTENT_ELEMENT_HTML_UPLOAD);
            if (element_html_update) {
                $("#" + CONTENT_ELEMENT_HTML_UPLOAD).empty();
                var option = document.createElement("option");
                option.text = CONTENT_RESULT_UPDATE_FILE[0].name;
                option.value = CONTENT_RESULT_UPDATE_FILE[0].ruta_archivo;
                document.getElementById(CONTENT_ELEMENT_HTML_UPLOAD).add(option);
                
            }
        }
        //----Evento que inserta una nueva versión del documento, en la interfaz de gestión de versiones
        if (CONTENT_NAME_UPLOAD_FUNCION == "MIGRACION") {
            insert_new_versio_document(CONTENT_RESULT_UPDATE_FILE);
        }
        if (CONTENT_NAME_UPLOAD_FUNCION == "WORKFLOW") {
            insert_new_versio_document(CONTENT_RESULT_UPDATE_FILE);
        }
        if (CONTENT_NAME_UPLOAD_FUNCION == "RADICACION") {
            insert_new_versio_document(CONTENT_RESULT_UPDATE_FILE);
        }
        if (CONTENT_NAME_UPLOAD_FUNCION == "DOCUARCHI") {
            insert_new_versio_document(CONTENT_RESULT_UPDATE_FILE);
        }
        if (CONTENT_NAME_UPLOAD_FUNCION == "CORRESPO") {
            insert_new_versio_document(CONTENT_RESULT_UPDATE_FILE);
        }
        //Caso que remplaza el icono cuando se remplaza la versión de un documento en lo modulos del gestor documental
        if (CONTENT_NAME_UPLOAD_FUNCION == "adjunta_remplazo_version_document_001") {
            let IconoAsome = CONTENT_RESULT_UPDATE_FILE[0].Class_list_detalle_version_document[0].IconoAsome;
            if (FilePerson.settings.TipoTable == "asp.net") {
                cahange_icono_image_table_asp_net_general(FilePerson.settings.name_class_element_icono_aspnet,
                    CONTENT_RESULT_UPDATE_FILE[0].Class_list_detalle_version_document[0].IconoAsome
                )
            }
           
            if (FilePerson.settings.TipoTable == "bootstrap") {
                let NameCampo = FilePerson.settings.NameCampo;
                let ValueIcono = 0;
                //Desactiva la opción de firma digital
                updateCelByUniqueIdReinit(FilePerson.settings.NameTable, FilePerson.settings.NameCampo, FilePerson.settings.id_imagen, 0);
                ValueIcono = CONTENT_RESULT_UPDATE_FILE[0].Class_list_detalle_version_document[0].DBT;
                NameCampo = "DBT";
                updateCelByUniqueIdReinit(FilePerson.settings.NameTable, NameCampo, FilePerson.settings.id_imagen, ValueIcono);
            }
            let name_modal_ = FilePerson.settings.name_element_hml_modal;
            $("#" + name_modal_).modal("hide");
        }
        if (CONTENT_NAME_UPLOAD_FUNCION == "adjunta_documeto_version_document") {
            let row = {};
            row = new Object();
            row["ID"] = CONTENT_RESULT_UPDATE_FILE[0].id_image;
            row["PAG"] = 0;
            row["TIPODOCUMENTO"] = CONTENT_RESULT_UPDATE_FILE[0].notitipodocumental;
            row["ESTADO_FIRMA_DIGITAL"] = CONTENT_RESULT_UPDATE_FILE[0].estado_firma_digital;
            row["IconoAsome"] = CONTENT_RESULT_UPDATE_FILE[0].IconoAsome;
            row["DBT"] = CONTENT_RESULT_UPDATE_FILE[0].Class_list_detalle_version_document[0].DBT;
            insert_row_table(FilePerson.settings.element_html_table, row);
            let html_parent = document.getElementById(FilePerson.settings.element_parent);
            if (html_parent) {
                $("#" + FilePerson.settings.element_parent).modal("hide");
            }
            let numrowTables = 0;
            numrowTables = total_row_table(FilePerson.settings.element_html_table);
            let html_lable = document.getElementById(FilePerson.settings.element_html_lab_conteo);
            if (html_lable) {
                html_lable.innerText = FilePerson.settings.apost_html_lab_conteo + " " + numrowTables;
            }
        }
        //Caso load file excel RUE SII
        if (CONTENT_NAME_UPLOAD_FUNCION == "adjunta_archivo_rue_sii") {
            Show_row_table_boot_rue(CONTENT_RESULT_UPDATE_FILE[0].row_table_boot, CONTENT_RESULT_UPDATE_FILE[0].obj_field_boot_table, FilePerson.settings.element_html_table, FilePerson.settings.element_parent_html_table);
        }
        //Caso load fie excel virtual SII
        if (CONTENT_NAME_UPLOAD_FUNCION == "adjunta_archivo_virtual_sii") {
            Show_row_table_boot_vitual(CONTENT_RESULT_UPDATE_FILE[0].row_table_boot, CONTENT_RESULT_UPDATE_FILE[0].obj_field_boot_table, FilePerson.settings.element_html_table, FilePerson.settings.element_parent_html_table);
        }
        return "YES";
              
     }

    catch (err) {
        return err.message + " funcion insert_runtHXR";
        
    }
}
//-----------------------------------------------------
//Evento inicial de tranferencia e archivo al servidor
//-----------------------------------------------------
function loadstartEvent(event) {
    try {
    var elem = document.getElementById("myBar_file_element_" + CONTEN_NAME_UPLOAD_FILE);
    var elment_progres = document.getElementById("pogres_file_element_porcent_" + CONTEN_NAME_UPLOAD_FILE);
    var elment_conta = document.getElementById("pogres_file_element_contador_" + CONTEN_NAME_UPLOAD_FILE);
    var progres = document.getElementById("progres_bar");
    if (progres) {
        posicion_update_pogres('progres_bar');
    }
    var name_cancel_file_element = "cancel_file_element_" + CONTEN_NAME_UPLOAD_FILE;
    var cancel_file_element = document.getElementById(name_cancel_file_element);
    if (cancel_file_element) {
        cancel_file_element.style.opacity = "1";
    }
    var porcent = (100 * CONTEN_NUM_UPLOAD_INCRE_FILE) / CONTEN_NUM_UPLOAD_FILE;
    porcent = Math.round(porcent);
    elem.style.width = porcent + '%';
    elment_progres.innerHTML = porcent + '% ';
    elment_conta.innerHTML = "(" + CONTEN_NUM_UPLOAD_INCRE_FILE + ' de ' + CONTEN_NUM_UPLOAD_FILE + ")";
     
    }
     catch (err) {
        alert(err.message + " funcion loadstartEvent");
     }
}
function timeoutEvent(event) {
    try {
        set_progres_Upload();
        alert("Error : " + ajax.status + " Mensaje : " + ajax.statusText);
        clearInterval(CONTENT_UPLOAD_ELEMENT_INTERVAL);
        CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
        
    } catch (err) {
        alert(err.message + " funcion timeoutEvent");
    }
}
//-----------------------------------------------------
//Evento final de tranferencia al archivo al servidor
//-----------------------------------------------------
function loadenEvent(event) {
    try {
        if (ajax.status !== 200 ) {
            set_progres_Upload();
            if (ajax.status !== 0) {
                alert("Error : " + ajax.status + " Mensaje : " + ajax.statusText);
            }    
            clearInterval(CONTENT_UPLOAD_ELEMENT_INTERVAL);
            CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
            return false;
        } else {
            var result = "";   
            var json = JSON.parse(ajax.responseText); 
            if (json[0].error_sistema !== "YES") {
                set_progres_Upload();
                CONTENT_ERROR_UPLOAD = json[0].error_sistema;
                alert(CONTENT_ERROR_UPLOAD);
                clearInterval(CONTENT_UPLOAD_ELEMENT_INTERVAL);
                CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
            } else {
                result = insert_runtHXR();
                if (result !== "YES") {
                    alert(result);
                }
                CONTEN_NUM_UPLOAD_INCRE_FILE++;
                CONTENT_UPLOAD_MULTIPLE_ESTATUS_SERVICE = "YES";
            }
        }
        
    } catch (err) {
        alert(err.message + " funcion  loadenEvent");
    } finally {
       
    }
}
function progressHandler(event) {
    try {
    var name_count_byte_file_element_UploadFile = "count_byte_file_element_" + CONTEN_NAME_UPLOAD_FILE;
    var element_count_byte_file_UploadFile = document.getElementById(name_count_byte_file_element_UploadFile);
    /*$("#loaded_n_total").html("Uploaded " + event.loaded + " bytes of " + event.total);*/
    var percent = (event.loaded / event.total) * 100;
   // $("#progressBar").val(Math.round(percent));
    if (element_count_byte_file_UploadFile) {
        if (percent < 100) {
            $("#" + name_count_byte_file_element_UploadFile).html(Math.round(percent) + "% Cargando...");
        } else {
            $("#" + name_count_byte_file_element_UploadFile).html("Guardando...");
        }
        
    }
    } catch (err) {
        alert(err.message + " funcion progressHandler");
    }
}

function completeHandler(event) {
    try {


    } catch (err) {
        alert(err.message + " funcion completeHandler ")
    } finally {
        var progres = document.getElementById("progres_bar");
        if (progres) {
            progres_hiden('progres_bar');
        }
    }
}
function errorHandler(event) {
    set_progres_Upload();
    clearInterval(CONTENT_UPLOAD_ELEMENT_INTERVAL);
    CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
    var progres = document.getElementById("progres_bar");
    if (progres) {
        progres_hiden('progres_bar');
    }
   
}

function abortHandler(event) {
    set_progres_Upload();
    var progres = document.getElementById("progres_bar");
    if (progres) {
        progres_hiden('progres_bar');
    }

}
function cancel_file_all_upload() {
    try {
        ajax.abort();
        clearInterval(CONTENT_UPLOAD_ELEMENT_INTERVAL);
        CONTENT_UPLOAD_ELEMENT_INTERVAL = 0;
        set_progres_Upload();
        CONTENT_UPLOAD_MULTIPLE_ESTATUS_SERVICE = "";
        
    } catch (err) {
        alert(err.message + " funcion cancel_file_all_upload")
    }
}
function delete_file_all_UploadFile() {
    try {
        var name_a_save_file_element = "save_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var a_save_file_element = document.getElementById(name_a_save_file_element);
        if (a_save_file_element) {
        } else {
            alert("Imposible encontrar el control (" + name_a_save_file_element + ")");
            return true;
        }
        var name_a_cancel_file_element = "cancel_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var a_cancel_file_element = document.getElementById(name_a_cancel_file_element);
        if (a_cancel_file_element) {
        } else {
            alert("Imposible encontrar el control (" + name_a_cancel_file_element + ")");
            return true;
        }
        var name_a_delete_file_element = "delete_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var a_delete_file_element = document.getElementById(name_a_delete_file_element);
        if (a_delete_file_element) {
        } else {
            alert("Imposible encontrar el control (" + name_a_delete_file_element + ")");
            return true;
        }
        var name_content_drop_element = "content_drop_element_" + CONTEN_NAME_UPLOAD_FILE;
        var content_drop_element = document.getElementById(name_content_drop_element);
        var name_element_p_count_UploadFile = "count_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var element_p_count_UploadFile = document.getElementById(name_element_p_count_UploadFile);
        var name_elment_table = "table_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var name_element_file = "file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var input = document.getElementById(name_element_file);
        if (CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files.length > 0) {
            CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.clear();
        }
        $("#" + name_elment_table + " tr ").remove();
        if (input) {
            input.files = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files;
        }
        a_save_file_element.style.opacity = "0";
        a_cancel_file_element.style.opacity = "0";
        a_delete_file_element.style.opacity = "0";
        a_delete_file_element.style.opacity = "0";
        content_drop_element.style.opacity = "1";
        element_p_count_UploadFile.innerText = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files.length + " Archivo(s) Cargado(s)";
    } catch (err) {
        alert(err.message + " funcion delete_file_all_UploadFile ");
    }
}
const delete_file_UploadFileName = (name) => {
    var name_element_p_count_UploadFile = "count_file_element_" + CONTEN_NAME_UPLOAD_FILE;
    var element_p_count_UploadFile = document.getElementById(name_element_p_count_UploadFile);
    var name_element_file = "file_element_" + CONTEN_NAME_UPLOAD_FILE;
    var input = document.getElementById(name_element_file);
    for (var i = 0; i < CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.length; i++) {
        if (name == CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items[i].getAsFile().name) {
            CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.remove(i);     
        }
        element_p_count_UploadFile.innerText = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files.length + " Archivo(s) Cargado(s)";
        input.files = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files;
        return "YES";
    }
}

function delete_file_UploadFile(idex) {
    try {
        var name_a_save_file_element = "save_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var a_save_file_element = document.getElementById(name_a_save_file_element);
        if (a_save_file_element) {
        } else {
            alert("Imposible encontrar el control (" + name_a_save_file_element + ")");
            return true;
        }
        var name_a_cancel_file_element = "cancel_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var a_cancel_file_element = document.getElementById(name_a_cancel_file_element);
        if (a_cancel_file_element) {
        } else {
            alert("Imposible encontrar el control (" + name_a_cancel_file_element + ")");
            return true;
        }
        var name_a_delete_file_element = "delete_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var a_delete_file_element = document.getElementById(name_a_delete_file_element);
        if (a_delete_file_element) {
        } else {
            alert("Imposible encontrar el control (" + name_a_delete_file_element + ")");
            return true;
        }
        var name_content_drop_element = "content_drop_element_" + CONTEN_NAME_UPLOAD_FILE;
        var content_drop_element = document.getElementById(name_content_drop_element);
        var name_element_p_count_UploadFile = "count_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var element_p_count_UploadFile = document.getElementById(name_element_p_count_UploadFile);
        var name_elment_table = "table_file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var name_element_file = "file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var input = document.getElementById(name_element_file);
        var name = "";
        name = search_nombre_file_UploadFile(name_elment_table, idex)
        if (name == "") {
            return "NO";
        }
        for (var i = 0; i < CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.length; i++) {
            if (name == CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items[i].getAsFile().name) {
                CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.remove(i);
                $("#" + name_elment_table + " tr[id=" + idex + "]").remove();
                input.files = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files;
                element_p_count_UploadFile.innerText = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files.length + " Archivo(s) Cargado(s)";
                if (CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files.length == 0) {
                    a_save_file_element.style.opacity = "0";
                    a_cancel_file_element.style.opacity = "0";
                    a_delete_file_element.style.opacity = "0";
                    content_drop_element.style.opacity = "1";
                }
                return "YES";
            }

        }    
        input.files = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files;
        return 'YES';

    } catch (err) {
        alert(err.message + " delete_file_UploadFile");
    }
}
function solicita_file_archivo_UploadFile(nombre_archivo) {
    try {
        var resul_dunction = null;
        for (let i = 0; i < CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.length; i++) {
            if (nombre_archivo === CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items[i].getAsFile().name) {
                resul_dunction = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items[i].getAsFile();
                return resul_dunction;
            }

        }
        return resul_dunction;
    } catch (err) {
        alert(err.message + " Function solicita_file_archivo_UploadFile" );
    }
}
function updateImageDisplay_(element) {    
    input = document.getElementById(element);
    preview = document.querySelector('.preview');
    //input.style.opacity = 0;
    while (preview.firstChild) {
        preview.removeChild(preview.firstChild);
    }

    const curFiles = input.files;
    if (curFiles.length === 0) {
        const para = document.createElement('p');
        para.textContent = 'No files currently selected for upload';
        preview.appendChild(para);
    } else {
        const list = document.createElement('ol');
        preview.appendChild(list);

        for (const file of curFiles) {
            const listItem = document.createElement('li');
            const para = document.createElement('p');
            if (validFileType(file)) {
                para.textContent = `File name ${file.name}, file size ${returnFileSize(file.size)}.`;
                const image = document.createElement('img');
                image.src = URL.createObjectURL(file);

                listItem.appendChild(image);
                listItem.appendChild(para);
            } else {
                para.textContent = `File name ${file.name}: Not a valid file type. Update your selection.`;
                listItem.appendChild(para);
            }

            list.appendChild(listItem);
        }
    }
}
function dragenter(e) {
    e.stopPropagation();
    e.preventDefault();
}

function dragover(e) {
    e.stopPropagation();
    e.preventDefault();
}
async function  drop_Upload_file(e) {
    try {
        e.stopPropagation();
        e.preventDefault();
        var name_element_file = "file_element_" + CONTEN_NAME_UPLOAD_FILE;
        var element_file = document.getElementById(name_element_file);
        var dt = e.dataTransfer;
        if (CONTENT_SELECT_FILE_UPLOAD != "") {
            element_file.files = dt.files;
        } else {
            CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.clear();
            CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.add(dt.files[0]);
            element_file.files = CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.files;
            CONTEN_NUM_UPLOAD_DAT_TRANFER_FILE.items.clear();
        }
       
        await  InsertaArchivosFront();
    } catch (err) {
        alert(err.mensaje);
    }
}


function prevent_UploadFile(event, element) {
    try {
        var id = $(element).attr("id");
        delete_file_UploadFile(id)
    } catch (err) {
        alert(err.message + " prevent_UploadFile");
    }
}
//-----------------------------------------------------
//Solicita nombre archivo tabla gred
//-----------------------------------------------------
function search_nombre_file_UploadFile(nombre_grid, id_) {
    try {
        var valor_retorn = "";
        $("#" + nombre_grid + " tr[id=" + id_ + "]").each(function () {
            var idex = 0;
            if (idex != -1) {
                var elemnt_p = $(this)[0].cells[idex].firstChild;
                var valor = elemnt_p.innerText;
                valor_retorn = valor;
            } else {
                valor_retorn = "";
            }
        });
        return valor_retorn;
    }
    catch (err) {
        alert(err.message + " Funcion search_nombre_file_UploadFile");
    }
}
function search_id_file_UploadFile(nombre_grid, name_file) {
    try {
        var valor_retorn = "-1";
        var conta = 0;
        $("#" + nombre_grid + " tr ").each(function () {
            var elemnt_p = $(this)[0].cells[0].firstChild;
            if (elemnt_p) {        
                if (elemnt_p.innerText == name_file) {              
                    valor_retorn = conta;
                    return valor_retorn;
                }
            }
            conta++;
        })
        return valor_retorn;
    }
    catch (err) {
        alert(err.message + " Funcion search_nombre_file_UploadFile");
    }
}
//----------------------------------------------------
//Valida el tipo de archivo
//----------------------------------------------------
const ValidateFileType = (namefile) => {
    let split_name_file = namefile.split(".");
    if (split_name_file.length == 0) {
        return "none";
    }
    let index_exte = split_name_file.length -1;
    let exten_file = "." + split_name_file[index_exte];
    let split_content_ext_file = CONTENT_ESTENSION_PERMITIDA.split(",");
    if (split_content_ext_file.length == 0) {
        return "none" ;
    }
    for (i = 0; i < split_content_ext_file.length; i++) {
        if (exten_file.toUpperCase() == split_content_ext_file[i].toUpperCase()) {
            return "yes";
        }
    }
    return "none";
}
const fileTypes = [
    "image/apng",
    "image/bmp",
    "image/gif",
    "image/jpeg",
    "image/pjpeg",
    "image/png",
    "image/svg+xml",
    "image/tiff",
    "image/webp",
    "image/x-icon"
];

function validFileType(file) {
    return fileTypes.includes(file.type);
}
function returnFileSize(number) {
    if (number < 1024) {
        return number + 'bytes';
    } else if (number >= 1024 && number < 1048576) {
        return (number / 1024).toFixed(1) + 'KB';
    } else if (number >= 1048576) {
        return (number / 1048576).toFixed(1) + 'MB';
    }
}
//------------------------------------------------------------------------------------
//Agrega los parametros de multiple seleccion, extensiones permitidas
//------------------------------------------------------------------------------------
function parameter_upload(Estado_event_general, parameter, element_clik, multi_select,pref_element) {
    try {    
        $.ajax('../webservice/WebServiceProducion.asmx/Service_parameter_upload', {
            data: "{" + "'parameter':'" + parameter + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {         
                    CONTENT_ESTENSION_PERMITIDA = data.d[0].ExtensionPermitida;
                    CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD = data.d[0].Maximo_tamano_archivo_byte;
                    CONTENT_SELECT_FILE_UPLOAD = multi_select;
                    CONTENT_PROCESO = parameter;
                    var name_element_file = "file_element_" + pref_element;
                    var element_file = document.getElementById(name_element_file);
                    if (element_file) {
                        if (multi_select == "") {
                            element_file.removeAttribute("multiple");
                        } else {
                            element_file.setAttribute("multiple", multi_select);
                        }             
                        element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA);
                        if (element_clik !== "") {
                            var element_Clik_ = document.getElementById(element_clik);
                            if (element_Clik_) {
                                element_Clik_.click();
                            } else {
                                alert("Imposible encontrar el control " + element_Clik_);
                            }
                        }

                    } 
                    upload_file_config_aceptar(pref_element);
                    ESTADO_EVENT_GENERAL = "out";
                }
            },
            error: function (result) {
                alert("Estatus : " + result.status + " Error ");
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion parameter_upload");
    }
}
const ServiceRESTSolicitaListaTramiteAutoVinculacionGabinete = async (parameter) => {
    try {
        // Si parameter es objeto -> se serializa
        // Si ya es string -> se manda directo
        const response = await fetch("../webservice/WebService_Config_Digitalizacion.asmx/ServiceSolicitaEstructuraConfiguracion", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: "{" + "'IdTipoTramite':'" + parameter + "'}", // tu data JSON se convierte en TEXTO
        });
        if (!response.ok) {
            return { error: true, status: response.status, message: response.statusText };
        }
        let responsejson = await response.json(); // 👈 respuesta en JSON
        if (responsejson.d[0].error_gestion != "YES") {
            return { error: true, message: responsejson.d[0].error_gestion, interface_config_digitaliza: responsejson.d[0] };
        } else {
            return { error: false, message: responsejson.d[0].error_gestion, interface_config_digitaliza: responsejson.d[0] };
        }
    } catch (err) {
        return { error: true, message: err.message };
    }
}
//------------------------------------------------------------------------------------
//Agrega los parametros de multiple seleccion, etensiones permitidas
//------------------------------------------------------------------------------------
function parameter_upload_modal(Estado_event_general, parameter, element_clik, multi_select, pref_element) {
    try {

        $.ajax('../webservice/WebServiceProducion.asmx/Service_parameter_upload', {
            data: "{" + "'parameter':'" + parameter + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_result !== "YES") {
                    alert(data.d[0].error_result);
                    ESTADO_EVENT_GENERAL = "out";
                } else {
                    CONTENT_ESTENSION_PERMITIDA = data.d[0].ExtensionPermitida;
                    CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD = data.d[0].Maximo_tamano_archivo_byte;
                    CONTENT_SELECT_FILE_UPLOAD = multi_select;
                    var name_element_file = "file_element_" + pref_element;
                    var element_file = document.getElementById(name_element_file);
                    if (element_file) {
                        if (multi_select == "") {
                            element_file.removeAttribute("multiple");
                        } else {
                            element_file.setAttribute("multiple", multi_select);
                        }
                        element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA);
                        if (element_clik !== "") {
                            $find(element_clik).show();
                            auto_zise_popup_adjunta_anexo_respuesta();
                           
                        }

                    }
                    upload_file_config_aceptar(pref_element);
                    ESTADO_EVENT_GENERAL = "out";
                }
            },
            error: function (result) {
                alert("Estatus : " + result.status + " Error ");
                ESTADO_EVENT_GENERAL = "out";
            }, compelete: function () {

            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        alert(ex.message + " funcion parameter_upload_modal");
    }
}
const service_REST_source_list_tipos_documentales = async (name_control,name_class_service, name_service,parameter) => {
    let myPromise = new Promise(function (resolve) {
    try {
        $.ajax('../webservice/' + name_class_service + '/' + name_service, {
            data: "{'id':" + "'" + parameter + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_sistema !== "YES") {
                    resolve(data.d[0].error_sistema);
                } else {
                    CONTENT_ITEM_ROW_TIPO = new Array();
                    $.each(data.d[0].item_sistema, function (k, v) {
                        CONTENT_ITEM_ROW_TIPO.push(v);
                    });
                    if (document.getElementById(name_control)) {
                        var element_drow = document.getElementById(name_control);
                        $("#" + name_control).empty();
                        for (var i = 0; i < CONTENT_ITEM_ROW_TIPO.length; i++) {
                            element_drow[i] = new Option(items_drow[i].text, CONTENT_ITEM_ROW_TIPO[i].value);
                        }
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
//Agrega los parametros d de multiple seleccion, etensiones permitidas async y modal bot strap
const service_REST_parameter_upload_boot = async (modulo_configuracion, modal_content, multi_select, pref_element) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceProducion.asmx/Service_parameter_upload', {
                data: "{" + "'parameter':'" + modulo_configuracion + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_result !== "YES") {
                        resolve(data.d[0].error_result);
                    } else {
                        CONTENT_ESTENSION_PERMITIDA = data.d[0].ExtensionPermitida;
                        CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD = data.d[0].Maximo_tamano_archivo_byte;
                        CONTENT_SELECT_FILE_UPLOAD = multi_select;
                        var name_element_file = "file_element_" + pref_element;
                        var element_file = document.getElementById(name_element_file);
                        if (element_file) {
                            if (multi_select == "") {
                                element_file.removeAttribute("multiple");
                            } else {
                                element_file.setAttribute("multiple", multi_select);
                            }
                            element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA);
                            if (modal_content !== "") {
                                $("#" + modal_content).modal("show");
                            }
                            resolve("YES");
                        } else {
                            resolve("Imposible encontrar el control (" + name_element_file + ")");
                        }
                       
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
//Configura el flitro de extensiones de archivos permitidos
function upload_file_config_aceptar(pref_element) {
    var name_element_file = "file_element_" + pref_element;
    var element_file = document.getElementById(name_element_file);
    if (pref_element == "adjunto_doc_visor") {
        var chek_adjunto = document.getElementById("Check_anexo_radicado_adj");
        if (chek_adjunto) {
            CONTENT_ESTENSION_PERMITIDA_EXCLUSE = ".TIF";
            if (chek_adjunto.checked == true) {
                if (element_file) {
                    element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA_EXCLUSE);
                }
            }
        }
        var chek_adjunto = document.getElementById("CheckBox_relacionado_radicado_adj");
        if (chek_adjunto) {
            if (chek_adjunto.checked == true) {
                if (element_file) {
                    element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA);
                }
            }
        }
    }
    if (pref_element == "adjunto_doc_respuesta") {
        var chek_adjunto = document.getElementById("Check_adjunta_formato");
        if (chek_adjunto) {
            CONTENT_ESTENSION_PERMITIDA_EXCLUSE = ".DOCX";
            if (chek_adjunto.checked == true) {
                if (element_file) {
                    element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA_EXCLUSE);
                }
            }
        }
        var chek_adjunto = document.getElementById("CheckBox_adjunta_documento_libre");
        if (chek_adjunto) {
            if (chek_adjunto.checked == true) {
                if (element_file) {
                    element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA);
                }
            }
        }
    }
}
//Captura onclic adjunta documento visor
function upload_adjunto_doc_visor_event_cheked_relacion(event) {
    try { 
            if (event.target.checked == true) {
                var name_element_file = "file_element_" + CONTEN_NAME_UPLOAD_FILE;
                var element_file = document.getElementById(name_element_file);
                if (element_file) {
                    element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA);
                }
            }       
    } catch (err) {
        alert(err.mensaje + " funcio upload_adjunto_doc_visor_event_cheked_relacion")
    }
}
function upload_adjunto_doc_visor_event_cheked_adjunto(event) {
    try {
        CONTENT_ESTENSION_PERMITIDA_EXCLUSE = ".TIF";
        if (event.target.checked == true) {
            var name_element_file = "file_element_" + CONTEN_NAME_UPLOAD_FILE;
            var element_file = document.getElementById(name_element_file);
            if (element_file) {
                element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA_EXCLUSE);
            }
        }
    } catch (err) {
        alert(err.mensaje + " funcio upload_adjunto_doc_visor_event_cheked_adjunto")
    }
}

//Captura onclic adjunta documento respuesta
function upload_adjunto_doc_respuesta_event_cheked_adjunto(event) {
    try {
        CONTENT_ESTENSION_PERMITIDA_EXCLUSE = ".DOCX";
        if (event.target.checked == true) {
            var name_element_file = "file_element_" + CONTEN_NAME_UPLOAD_FILE;
            var element_file = document.getElementById(name_element_file);
            if (element_file) {
                element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA_EXCLUSE);
            }
        }
    } catch (err) {
        alert(err.mensaje + " funcio upload_adjunto_doc_respuesta_event_cheked_adjunto")
    }
}
function upload_adjunto_doc_respuesta_libre_event_cheked_adjunto(event) {
    try {
        
        if (event.target.checked == true) {
            var name_element_file = "file_element_" + CONTEN_NAME_UPLOAD_FILE;
            var element_file = document.getElementById(name_element_file);
            if (element_file) {
                element_file.setAttribute("accept", CONTENT_ESTENSION_PERMITIDA);
            }
        }
    } catch (err) {
        alert(err.mensaje + " funcio upload_adjunto_doc_respuesta_libre_event_cheked_adjunto")
    }
}
