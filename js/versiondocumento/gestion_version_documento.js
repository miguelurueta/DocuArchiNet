/**
 * Implementación gestión de versionamiento
 * let ref_idd = $(element).attr("idd_rad");
            let spliter = ref_idd.split("|");
            let id_imagen = spliter[1];
            let gabinete = spliter[0];
            let name_class_element_icono_aspnet = spliter[8];
            let DocumentoTilte = spliter[4];
            let option =
                ({
                    IdImagen: id_imagen, Gabinete: gabinete, name_class_element_icono_aspnet: name_class_element_icono_aspnet,
                    DocumentoTilte: DocumentoTilte, OptionRemPlazo: "RAD", ContentError: "error_div_selecion_tarea_rad",
                    NameControlParent: "div_content_general_wf"
                })
             ShowActivaOpcionRemplazo(option);
             archivos dependencia  
             <script src="../js/java_general/GredviewControl.js" type="text/javascript"></script>
             <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
             <script src="../js/java_general/ASMXClient.js" type="text/javascript"></script>
             <script src="../js/java_general/JSProgresBar.js" type="text/javascript"></script>
             <link href="../generic_control/UploadFile.css" rel="stylesheet" />
             Ing: Miguel Angel Urueta Miranda  2025-05-25

 * */
class JSVesrionDocumento {
    constructor(options = {}) {
        let defaults = {
            IdImagen: 0,                          //Parametro que representa la identiifcación de la imagen
            Gabinete: "",                         //Paramentro que representa el nombre del gabinete
            name_class_element_icono_aspnet: "",  //Paramentro que representa la clase o el nombre del icono de la tabla asp.net
            DocumentoTilte: "",                   //Paramentro que representa el titulo del modal de diitaliación
            OptionRemlazo: "",                    //Paramentro que representa la opción de remplazo
            ContentError: "",                     //Paramentro que representa el nombre del contenedor de error
            NameModulo: "",                       //Parametro que representa el nombre del modulo de remplazo 
            TipoModulo: "",                       //Parametro que representa el tipo de modulo  - 1 Mod migracion  (MIGRACION)   2- Modulo workflow (WORFKLOW)    -3 modulo producion (PRODUCCION)  4- modulo docuarchi (DOCUARCHI)  5- Radicacion  (RADICACION)  6- Modulo gestion correspondencia  (CORRESPO)
            NameControlParent: "",
            TipoTable: "asp.net",
            NameTable: "",
            NameCampo: "",
            NameCampoId: ""
        }
        this.settings = $.extend(true, defaults, options);
        this.NameContendorVersionDocumento = "name_contenedor_version_documento_0001";
        this._ModalVersionDocumento = "";
        this._BtnoActiveLoad;
        this.NameContendorLoadDocumento = "name_contenedor_load_documento_0001";
        this._ModalLoadDocumento = "";
        this.NameContendorDetailDocumento = "name_contenedor_detail_documento_0001";
        this._ModalDetailDocumento = "";
        this.NameContendorSelectionReplace = "name_contenedor_selection_replace_0001";
        this._ModalSelectionReplace = "";
        this._OptionChekSelectionAdjunta;
        this._OptionChekSelectionScan;
        this._BtnoActiveSelectionReplace;
        this.NameContendorLoadReplaceVersion = "name_contenedor_load_replace_version_0001";
        this._ModalShowLoadVReplaceersion_ = "";
    }
    /** Funtion que carga la interfaz de digitalizacion */
    async LoadJSVesrionDocumento() {
        try {
            MODULO_LISTA_VERSION = this.settings.TipoModulo;
            let result;
            result = await this._ModalShowJSVesrionDocumento();
            if (result != "YES") {
                return result;
            }
            //----Crea interfaz de adjuntar doumento nueva versión
            result = await this._ModalShowLoadVersion();
            if (result != "YES") {
                return result;
            }
            //----Crea interfaz detalle de versión
            result = await this._ModalShowDetailVersion();
            if (result != "YES") {
                return result;
            }
           
            result = await Service_REST_solicita_estructura_configuracion_gabinete(this.settings.Gabinete);
            if (result !== "YES") {
                return result;
            }
            result = await Service_REST_listar_versiones_documentos(this.settings.IdImagen, ID_GABINETE_VERSION_DOCUMENTO);
            if (result !== "YES") {
                return result;
            }
            return "YES";
        } catch (ex) {
            return ex.message;
        }
    }
    /**Show modal versiones del documento */
    async _ModalShowJSVesrionDocumento() {
        try {
            if (document.getElementById(this.NameContendorVersionDocumento)) {
                let element = document.getElementById(this.NameContendorVersionDocumento);
                element.remove();
            }
            const wrapper = document.createElement('div');
            wrapper.id = this.NameContendorVersionDocumento;
            wrapper.innerHTML = [
                '<div class="modal fade fade_person modal_opacity" style="z-index:100059" id="modal_version_document" role="dialog" data-backdrop="false">',
                '<div class="modal-dialog modal-fullscreen-sm-down modal-person-compment-fullscreen">',
                '<div id="modal_verson_dcoument_full_screen" class="modal-content-fullscreen modal-person-compment-fullscreen">',
                '<div id="header_modal_version_document" style="width: 100%">',
                '<div class="modal-header" style="max-height:73px">',
                '<h6 class="modal-title" id="title_version_documento" style="color: black" >Versiones del documento</h6>',
                '<button type="button" class="close" data-dismiss="modal">&times;</button>',
                '</div>',
                '</div>',
                '<div class="modal-body-fullscreen" >',
                '<div id="tool_bar_version_document" class="navbar navbar-expand-sm  modal_content_no_back_inferior row">',
                '<div class="nav col-md-6">',
                '<div class="nav-item active_">',
                '<a class="nav-link active ml-1 " title="Adjunta nueva versión" id="Button_activa_adjunta_document_version" style="color:black" href="#">  <i style="color: black" class="fal fa-arrow-from-bottom"></i>',
                ' Adjunta nueva versión',
                '</a>',
                '</div>',
                '</div>',
                '</div>',
                '<div class="row row-body-fullscreen">',
                '<div class="col-4  modal-body-fullscreen_ pr-0">',
                '<div class="" id="content_tabl_lista_version_documento" style="height:100%" >',
                '<table class="border-boton-person-none_ border-right-person_ table-no-borders" style="background-color: white"',
                'id="tabl_lista_version_documento"',
                'data-unique-id="id_registro_version"',
                'data-locale="es-SP">',
                '<thead class="GridviewScrollHeader_line_boot">',
                '<tr>',
                '<th data-field="ESTADO_ACTIVO_GABINETE" title="Versión activa en el gabinete" data-formatter="version_operateFormatter_asing" data-events="operateEventsVesrion" ></th>',
                '<th data-field="operate" data-formatter="operate_list_version_document" data-events="operateEventsVesrion">OPCIONES</th>',
                '<th data-field="id_registro_version" data-visible="false">ID</th>',
                '<th data-field="id_version_doc" title="Versión del documento" data-visible="false">VERSION</th>',
                '<th data-field="fecha_registro_version" title="fecha versión del documemto">FECHA VERSION</th>',
                '<th data-field="IconoAsome" data-visible="false">IconoAsome</th>',
                '</tr>',
                '</thead>',
                '</table>',
                '</div>',
                '</div>',
                '<div class="col-8 modal-body-fullscreen pl-0">',
                '<div class="conten_gred_border_" id="content_view_version_documento" style="height: 100%">',
                '<iframe id="Iframe_document_visor_version" runat="server" loading="lazy" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '<div id="error_content_version_documento" style="position: relative; width:100%"></div>',
                '<div id="footer_modal_version_document" >',
                '<div class=" modal-footer" style="max-height:73px">',
                '<div class="row row-body-fullscreen">',
                '<div class="col-6 justify-content-start">',
                '<h6 style="color: black" id="h_title_gabinete_image"></h6>',
                '</div>',
                '<div class="col-6 justify-content-end">',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '</div>'
            ].join('');
            let content = document.getElementById(this.settings.NameControlParent);
            if (content) {
                content.append(wrapper);
            } else {
                return "Imposible encontrar el contenedor principal (" + this.settings.NameControlParent + ") . Agregue un contenedor válido para alojar el contenedor de versiones.";
            }
            this._BtnoActiveLoad = document.getElementById("Button_activa_adjunta_document_version");
            this._BtnoActiveLoad.addEventListener("click", this._ShowLoadVersionDocumento, false);
            let HmlTitleModal = document.getElementById("title_version_documento");
            if (HmlTitleModal) {
                HmlTitleModal.innerText = "Versiones del documento (" + this.settings.DocumentoTilte + ")";
            }
            this.AutoZiseVersionDocument();
            window.addEventListener("resize", this.AutoZiseVersionDocument, false);
            return "YES";
        } catch (ex) {
            return ex.mensaje;
        }
    };
    /**Deprecate se utiliza la versión del FileUploadHandler.js */
    async _ModalShowLoadVersion() {
        try {
            if (document.getElementById(this.NameContendorLoadDocumento)) {
                let element = document.getElementById(this.NameContendorLoadDocumento);
                element.remove();
            }
            const wrapper = document.createElement('div');
            wrapper.id = this.NameContendorLoadDocumento;
            wrapper.innerHTML = [
                '<div class="modal fade modal_opacity" style="z-index:100061" id="modal_adjunta_documeto_version_document_005" role="dialog" data-backdrop="false">',
                '<div class="modal-dialog  modal-mediunscreen-sm-down ">',
                '<div class="modal-content-fullscreen">',
                '<div class="modal-header">',
                '<h4 style="color: black" class="modal-title">Adjunta documento version</h4>',
                '<button type="button" class="close" data-dismiss="modal">&times;</button>',
                '</div>',
                '<div class="modal-body-fullscreen modal-body">',
                '<div class="row row-body-fullscreen">',
                '<div class="p-4 w-100">',
                '<div class="row p-2" id="content_boton_adjunta_documeto_version_document_005">',
                '<div class="col-12 p-0 pl-1">',
                '<div class="file-select " id="src-file_">',
                '<input id="file_element_adjunta_documeto_version_document_005" type="file" multiple="multiple" accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />',
                '</div>',
                '<a id="save_file_element_adjunta_documeto_version_document_005" title="Guardar todos los archivos" class="btn  btn-success ml-1" style="opacity: 0; color: white"><i style="color: white" class="fas fa-save "></i> Guardar </a>',
                '<a id="delete_file_element_adjunta_documeto_version_document_005" title="Elminar todos los archivos cargados" class="btn  btn-danger ml-1" style="opacity: 0; color: white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>',
                '<a id="cancel_file_element_adjunta_documeto_version_document_005" title="Cancelar guardar archivos" class="btn  btn-warning ml-1" style="opacity: 0; color: white"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>',
                '</div>',
                '</div>',
                '<div class="paren_element background_upload" id="conten_file_element_adjunta_documeto_version_document_005" style="overflow: auto; height: 80%">',
                '<div id="content_drop_element_adjunta_documeto_version_document_005" claas="">',
                '</div>',
                '<table id="table_file_element_adjunta_documeto_version_document_005" class="table table-striped">',
                '</table>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '<div id="error_content_adjunta_documeto_version_document_005" style="position: relative; width: 100%" class="pl-4 pr-4"></div>',
                '<div class=" modal-footer_">',
                '<div class="row border_ pt-2 w-100" id="content_pie_title_adjunta_documeto_version_document_005">',
                '<div class="col-8 justify-content-start">',
                '<div class="row p-2">',
                '<div class="col-4 p-0">',
                '<div>',
                '<asp:Label ID="Label_progres_bar_file_element_adjunta_documeto_version_document_005" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>',
                '</div>',
                '<div id="pogres_file_element_contador_adjunta_documeto_version_document_005" style="text-align: center; font-family: Arial; font-size: 14px">',
                '</div>',
                '<div id="pogres_file_element_porcent_adjunta_documeto_version_document_005" style="text-align: center; font-family: Arial; font-size: 14px">',
                '</div>',
                '</div>',
                '<div class="col-5 p-0">',
                '<div>',
                '<div id="myProgress_file_element_adjunta_documeto_version_document_005">',
                '<div id="myBar_file_element_adjunta_documeto_version_document_005" class="file-select-bar"></div>',
                '</div>',
                '</div>',
                '</div>',
                '<div class="col-3 p-0 pl-3">',
                '<p id="count_byte_file_element_adjunta_documeto_version_document_005"></p>',
                '</div>',
                '</div>',
                '</div>',
                '<div class="col-4 justify-content-end pt-2">',
                '<p id="count_file_element_adjunta_documeto_version_document_005" class="font-weight-light" style="float: right">Estado </p>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
            ].join('');
            let content = document.getElementById(this.settings.NameControlParent);
            if (content) {
                content.append(wrapper);
            }
            this._ModalLoadDocumento = $("#modal_adjunta_documeto_version_document_005");
            return "YES";
        } catch (ex) {
            return ex.message;
        }
    };
    async _ModalShowDetailVersion() {
        try {
            if (document.getElementById(this.NameContendorDetailDocumento)) {
                let element = document.getElementById(this.NameContendorDetailDocumento);
                element.remove();
            }
            const wrapper = document.createElement('div');
            wrapper.id = this.NameContendorDetailDocumento;
            wrapper.innerHTML = [
                '<div class="modal fade modal_opacity" style="z-index:100060" id="modal_detalle_version_documento" role="dialog" data-backdrop="false">',
                    '<div class="modal-dialog modal-dialog-scrollable">',
                        '<div class="modal-content" >',
                            '<div class="modal-header">',
                                '<h4 class="modal-title">Detalle versión documento</h4>',
                                '<button type="button" class="close" data-dismiss="modal">&times;</button>',
                            '</div>',
                            '<div class="modal-body">',
                                '<div class="row ">',
                                    '<div class="col-6">',
                                        '<span>Identificador de versión</span>',
                                    '</div>',
                                    '<div class="col-6">',
                                        '<span id="spn_id_registro_version"></span>',
                                    '</div>',
                                '</div>',
                                '<div class="row ">',
                                    '<div class="col-6">',
                                        '<span>Versión del documento</span>',
                                    '</div>',
                                    '<div class="col-6">',
                                        '<span id="spn_id_version_doc"></span>',
                                    '</div>',
                                '</div>',
                                '<div class="row ">',
                                    '<div class="col-6">',
                                        '<span>Fecha versión</span>',
                                    '</div>',
                                    '<div class="col-6">',
                                        '<span id="spn_fecha_registro_version"></span>',
                                    '</div>',
                                '</div>',
                                '<div class="row ">',
                                    '<div class="col-6">',
                                        '<span>Tipo archivo</span>',
                                    '</div>',
                                    '<div class="col-6">',
                                        '<span id="spn_tipo_archivo"></span>',
                                    '</div>',
                                '</div>',
                                '<div class="row ">',
                                    '<div class="col-6">',
                                        '<span>Tamaño archivo</span>',
                                    '</div>',
                                    '<div class="col-6">',
                                        '<span id="spn_peso_documento"></span>',
                                    '</div>',
                                '</div>',
                                '<div class="row ">',
                                    '<div class="col-6">',
                                        '<span>Paginas</span>',
                                    '</div>',
                                    '<div class="col-6">',
                                        '<span id="spn_paginas_document"></span>',
                                    '</div>',
                                '</div>',
                                '<div class="row ">',
                                    '<div class="col-6">',
                                        '<span>Productor</span>',
                                    '</div>',
                                    '<div class="col-6">',
                                        '<span id="spn_productor_document"></span>',
                                    '</div>',
                                '</div>',
                            '</div>',
                            '<div id="error_detalle_version_documento" style="position: relative; width:100%"></div>',
                            '<div class=" modal-footer">',
                            '</div>',
                        '</div>',
                    '</div>',
                '</div>'
            ].join('');
            let content = document.getElementById(this.settings.NameControlParent);
            if (content) {
                content.append(wrapper);
            }
            this._ModalDetailDocumento = $("#modal_detalle_version_documento");
            return "YES";
        } catch(ex) {
          return ex.message;
        }
    };
    async _ModalShowSelctionReplace() {
        try {
            if (document.getElementById(this.NameContendorSelectionReplace)) {
                let element = document.getElementById(this.NameContendorSelectionReplace);
                element.remove();
            }
            const wrapper = document.createElement('div');
            wrapper.id = this.NameContendorSelectionReplace;
            wrapper.innerHTML = [
                '<div class="modal fade modal_opacity" style="z-index:1000601" id="modal_seleccion_remplazo_001" role="dialog" data-backdrop="false">',
                    '<div class="modal-dialog modal-dialog-scrollable">',
                        '<div class="modal-content">',
                            '<div class="modal-header">',
                                '<h6 style="color: black" class="modal-title">Opciones de remplazo</h4>',
                            '</div>',
                            '<div class="modal-body">',
                                '<div class="form-check">',
                                    '<input class="form-check-input" type="radio" name="flexRadioDefault" id="radio_option_adjunta" checked="checked" />',
                                    '<label class="form-check-label font-weight-light" for="radio_option_adjunta">',
                                        ' Reemplázalo con el archivo adjunto desde tu dispositivo.',
                                    '</label>',
                                '</div>',
                                '<div class="form-check">',
                                    '<input class="form-check-input" type="radio" name="flexRadioDefault" id="radio_option_digitaliza" />',
                                    '<label class="form-check-label font-weight-light" for="radio_option_digitaliza">',
                                        ' Reemplázalo con el archivo escaneado o digitalizado.',
                                    '</label>',
                                '</div>',
                            '</div>',
                            '<div class=" modal-footer">',
                                '<button type="button" id="Button_seeleccion_cancelar" class="btn  btn-light" data-dismiss="modal" title="">Cancelar</button>',
                                '<button type="button" id="Button_seeleccion_remplazo" class="btn  btn-primary" title="">Aceptar</button>',
                            '</div>',
                        '</div>',
                    '</div>',
                '</div>',
            ].join('');
            let content = document.getElementById(this.settings.NameControlParent);
            if (content) {
                content.append(wrapper);
            }
            this._BtnoActiveSelectionReplace = document.getElementById("Button_seeleccion_remplazo");
            this._BtnoActiveSelectionReplace.addEventListener("click", this._SelectionRemplaceDocumento, false);
            this._OptionChekSelectionAdjunta = document.getElementById("radio_option_adjunta");
            this._OptionChekSelectionScan = document.getElementById("radio_option_digitaliza");
            this._ModalSelectionReplace = $("#modal_seleccion_remplazo_001");
            this._ModalSelectionReplace.modal("show");
            return "YES";

        } catch (ex) {
            return ex.mensaje
        }
    }
    async _ModalShowLoadReplaceVersion() {
        try {
            if (document.getElementById(this.NameContendorLoadReplaceVersion)) {
                let element = document.getElementById(this.NameContendorLoadReplaceVersion);
                element.remove();
            }
            const wrapper = document.createElement('div');
            wrapper.id = this.NameContendorLoadReplaceVersion;
            wrapper.innerHTML = [
                '<div class="modal fade modal_opacity" style="z-index:100060" id="modal_adjunta_remplazo_version_document_001" role="dialog" data-backdrop="false">',
                    '<div class="modal-dialog  modal-mediunscreen-sm-down ">',
                        '<div class="modal-content-fullscreen">',
                            '<div class="modal-header">',
                                '<h4 style="color: black" class="modal-title">Adjunta documento remplazo</h4>',
                                '<button type="button" class="close" data-dismiss="modal">&times;</button>',
                            '</div>',
                            '<div class="modal-body-fullscreen modal-body">',
                                '<div class="row row-body-fullscreen">',
                                    '<div class="p-4 w-100">',
                                        '<div class="row p-2" id="content_boton_adjunta_remplazo_version_document_001">',
                                            '<div class="col-12 p-0 pl-1">',
                                                '<div class="file-select " id="src-file_remplazo_version_001">',
                                                    '<input id="file_element_adjunta_remplazo_version_document_001" type="file" multiple="multiple" accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />',
                                                '</div>',
                                                '<a id="save_file_element_adjunta_remplazo_version_document_001" title="Guardar todos los archivos" class="btn  btn-success ml-1" style="opacity: 0; color: white"><i style="color: white" class="fas fa-save "></i> Guardar </a>',
                                                '<a id="delete_file_element_adjunta_remplazo_version_document_001" title="Elminar todos los archivos cargados" class="btn  btn-danger ml-1" style="opacity: 0; color: white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>',
                                                '<a id="cancel_file_element_adjunta_remplazo_version_document_001" title="Cancelar guardar archivos" class="btn  btn-warning" style="opacity: 0; color: white"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>',
                                            '</div>',
                                        '</div>',
                                        '<div class="paren_element background_upload" id="conten_file_element_adjunta_remplazo_version_document_001" style="overflow: auto; height: 80%">',
                                            '<div id="content_drop_element_adjunta_remplazo_version_document_001" claas="">',
                                            '</div>',
                                            '<table id="table_file_element_adjunta_remplazo_version_document_001" class="table table-striped">',
                                            '</table>',
                                        '</div>',
                                    '</div>',
                                '</div>',
                            '</div>',
                            '<div id="error_content_adjunta_remplazo_version_document_001" style="position: relative; width: 100%" class="pl-4 pr-4"></div>',
                            '<div class=" modal-footer_">',
                                '<div class="row border_ pt-2 w-100" id="content_pie_title_adjunta_remplazo_version_document_001">',
                                    '<div class="col-8 justify-content-start">',
                                        '<div class="row p-2">',
                                            '<div class="col-4 p-0">',
                                                '<div>',
                                                    '<asp:Label ID="Label_progres_bar_file_element_adjunta_remplazo_version_document_001" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>',
                                                '</div>',
                                                '<div id="pogres_file_element_contador_adjunta_remplazo_version_document_001" style="text-align: center; font-family: Arial; font-size: 14px">',
                                                '</div>',
                                                '<div id="pogres_file_element_porcent_adjunta_remplazo_version_document_001" style="text-align: center; font-family: Arial; font-size: 14px">',
                                                '</div>',
                                            '</div>',
                                            '<div class="col-5 p-0">',
                                                '<div>',
                                                    '<div id="myProgress_file_element_adjunta_remplazo_version_document_001">',
                                                        '<div id="myBar_file_element_adjunta_remplazo_version_document_001" class="file-select-bar"></div>',
                                                    '</div>',
                                                '</div>',
                                            '</div>',
                                            '<div class="col-3 p-0 pl-3">',
                                                '<p id="count_byte_file_element_adjunta_remplazo_version_document_001"></p>',
                                            '</div>',
                                        '</div>',
                                    '</div>',
                                    '<div class="col-4 justify-content-end pt-2">',
                                        '<p id="count_file_element_adjunta_remplazo_version_document_001" class="font-weight-light" style="float: right"> Estado </p>',
                                    '</div>',
                                '</div>',
                            '</div>',
                        '</div>',
                    '</div>',
                '</div>',
            ].join('');
            let content = document.getElementById(this.settings.NameControlParent);
            if (content) {
                content.append(wrapper);
            }
            this._ModalShowLoadVReplaceersion_ = $("#modal_adjunta_remplazo_version_document_001");
            return "YES";
        } catch (ex) {
            return ex.message
        }
    }
    /**
     *Activa el componente para adjuntar la nueva versión del documento 
     * @param {any} e
     */
    async _ShowLoadVersionDocumento(e) {
        try {
            delete_alert_boot();
            let result = "";
            NAME_MODULO_VERSION_DOCUMENT = _JSVesrionDocumento.settings.NameModulo;
            if (_JSVesrionDocumento.settings.NameModulo == "MIGRACION") {
                NAME_GABINETE_VERSION = GABINETE_MIG;
                ID_IMAGEN_VERSION = ID_IMAGEN_MIG;
            } else {
                NAME_GABINETE_VERSION = _JSVesrionDocumento.settings.Gabinete;
                ID_IMAGEN_VERSION = _JSVesrionDocumento.settings.IdImagen;
            }
            posicion_update_pogres('progres_bar');
            let _OPtionFileLoad = ({
                NameLoadProceso: "ADJUNTAVERSION",
                NameContenedorError: "error_content_adjunta_documeto_load_documento_006",
                funcion_name: "adjunta_nueva_version_document", evento_adjunta: "ADJUNTAVERSION",
                IdRespuestaIdExpediente: 0,
                NameContendorLoadDocumento: _JSVesrionDocumento.settings.NameControlParent, ModalWidth: 75, CargaTipologia: 0,
                CargaFecha: 0, CargaPreview: 1, multi_select: "",
                element_parent: "modal_adjunta_documeto_load_documento_006", TipoFormulario: 1,
                element_html_table: _JSVesrionDocumento.settings.NameTable,
                tipo_adjunta: 1, gabinete: _JSVesrionDocumento.settings.Gabinete,
                id_imagen: _JSVesrionDocumento.settings.IdImagen, name_modulo: _JSVesrionDocumento.settings.NameModulo,
                name_class_element_icono_aspnet: _JSVesrionDocumento.settings.name_class_element_icono_aspnet,
                TipoTable: _JSVesrionDocumento.settings.TipoTable, NameTable: "tabl_lista_version_documento",
                NameCampo: _JSVesrionDocumento.settings.NameCampo, NameCampoId: _JSVesrionDocumento.settings.NameCampoId,
                element_html_table: "tabl_lista_version_documento", element_html_lab_conteo: ""
            });
            result = await IniLoadPerson(_OPtionFileLoad); //Dependence FileUploadHandler
            if (result != "YES") {
                alert_bot(result, 'warning', _JSVesrionDocumento.settings.ContentError);
            }
            
        } catch (ex) {
            alert_bot(ex.message, 'warning', _JSVesrionDocumento.settings.ContentError);
        } finally {
            progres_hiden('progres_bar');
        }
    }
    //-------Cambia el icnono de las tablas ASP.NET
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
    //--------Activa el remplazo de versión de documentos en los modulos del gestor doumental
  
    async _SelectionRemplaceDocumento() {
        try {
            let result = "";
            if (_JSVesrionDocumento._OptionChekSelectionAdjunta.checked == true) {
                let _OPtionFileLoad = ({
                    NameLoadProceso: "REMPLAZAVERSION",
                    NameContenedorError: "error_content_adjunta_documeto_load_documento_006",
                    funcion_name: "adjunta_documeto_version_document", evento_adjunta: "REMPLAZAVERSION",
                    IdRespuestaIdExpediente: 0,
                    NameContendorLoadDocumento: _JSVesrionDocumento.settings.NameControlParent, ModalWidth: 75, CargaTipologia: 0,
                    CargaFecha: 0, CargaPreview: 1, multi_select: "",
                    element_parent: "modal_adjunta_documeto_load_documento_006", TipoFormulario: 1,
                    element_html_table: _JSVesrionDocumento.settings.NameTable,
                    tipo_adjunta: 1, gabinete: _JSVesrionDocumento.settings.Gabinete,
                    id_imagen: _JSVesrionDocumento.settings.IdImagen, name_modulo: _JSVesrionDocumento.settings.NameModulo,
                    name_class_element_icono_aspnet: _JSVesrionDocumento.settings.name_class_element_icono_aspnet,
                    TipoTable: _JSVesrionDocumento.settings.TipoTable, NameTable: _JSVesrionDocumento.settings.NameTable,
                    NameCampo: _JSVesrionDocumento.settings.NameCampo, NameCampoId: _JSVesrionDocumento.settings.NameCampoId
                });
                result = await IniLoadPerson(_OPtionFileLoad); //Dependence FileUploadHandler
                if (result != "YES") {
                    alert_bot(result, 'warning', _JSVesrionDocumento.settings.ContentError);
                }
                _JSVesrionDocumento._ModalSelectionReplace.modal("hide");
                return true;
            }
            if (_JSVesrionDocumento._OptionChekSelectionScan.checked == true) {
                let option_ = ({
                    IdImagen: _JSVesrionDocumento.settings.IdImagen, Gabinete: _JSVesrionDocumento.settings.Gabinete,
                    name_class_element_icono_aspnet: _JSVesrionDocumento.settings.name_class_element_icono_aspnet,
                    DocumentoTilte: _JSVesrionDocumento.settings.DocumentoTilte, OptionRemlazo: _JSVesrionDocumento.settings.OptionRemPlazo,
                    ContentError: _JSVesrionDocumento.settings.ContentError, NameModulo: "REMPLAZAVERSION", NameControlPadreScan: _JSVesrionDocumento.settings.NameControlParent,
                    UrlSacan: "../workflow/WebFormEscan.aspx", TipoTable: _JSVesrionDocumento.settings.TipoTable, NameTable: _JSVesrionDocumento.settings.NameTable,
                    NameCampo: _JSVesrionDocumento.settings.NameCampo, NameCampoId: _JSVesrionDocumento.settings.NameCampoId
                });
                let result_ = await JSReplaceScanFileBoot(option_);
                if (result_ != "YES") {
                    alert_bot(result_, 'warning', _JSVesrionDocumento.settings.ContentError);
                }
            }

            return true;
        } catch (ex) {
            alert_bot(ex.mensaje, 'warning', _JSVesrionDocumento.settings.ContentError);
        } finally {
            _JSVesrionDocumento._ModalSelectionReplace.modal("hide");
        }
    }
    AutoZiseVersionDocument() {
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
        let height_header = document.getElementById('header_modal_version_document').clientHeight;
        let height_footer = document.getElementById('footer_modal_version_document').clientHeight;
        let height_toolbar = document.getElementById('tool_bar_version_document').clientHeight;
        if (height_footer < 1) {
            height_footer = 73;
        }
        if (height_header < 1) {
            height_header = 73;
        }
        if (height_toolbar < 1) {
            height_toolbar = 73;
        }

        $('#content_tabl_lista_version_documento').css("height", ((espacio_iframe - (height_header + height_header + height_toolbar))) + "px");
        $('#content_view_version_documento').css("height", ((espacio_iframe - (height_header + height_header + height_toolbar))) + "px");
        let heig_table = (espacio_iframe - (height_header + height_header + height_toolbar));
        table_reize_heigth("tabl_lista_version_documento", heig_table, "", "table-borderless");

    } catch (ex) { alert_bot(ex.mensaje, 'warning', _JSVesrionDocumento.settings.ContentError); }

}
}
let _JSVesrionDocumento;
const JSVesrionDocumentoBoot = async (Option) => {
    _JSVesrionDocumento = new JSVesrionDocumento(Option);
    let result = await _JSVesrionDocumento.LoadJSVesrionDocumento();
    return result;
}
const JSVesrionOptionSelectionBoot = async (Option) => {
    _JSVesrionDocumento = new JSVesrionDocumento(Option);
    let result = await _JSVesrionDocumento._ModalShowSelctionReplace();
    if (result != "YES") {
        return result;
    }
    result = await _JSVesrionDocumento._ModalShowLoadReplaceVersion();
    return result;
}
const ShowActivaOpcionRemplazo = async (option) => {
    try {
        let result = await JSVesrionOptionSelectionBoot(option);
        if (result != "YES") {
            alert_bot(result, 'warning', _JSVesrionDocumento.settings.ContentError);
        }

    } catch (ex) {
        alert_bot(ex.message, 'warning', _JSVesrionDocumento.settings.ContentError);
    }

}
const ShowListVersionDocumento = async (option) => {
    try {
        let result = await JSVesrionDocumentoBoot(option);
        if (result != "YES") {
            alert_bot(result, 'warning', _JSVesrionDocumento.settings.ContentError);
        }

    } catch (ex) {
        alert_bot(ex.message, 'warning', _JSVesrionDocumento.settings.ContentError);
    }
}
//-------------ZONA CONTROL VERSIONES DOCUMENTO----------------------------
let MODULO_LISTA_VERSION = 0;    //- 1 Mod migracion  (MIGRACION)   2- Modulo workflow (WORFKLOW)    -3 modulo producion (PRODUCCION)  4- modulo docuarchi (DOCUARCHI)  5- Radicacion  (RADICACION)  6- Modulo gestion correspondencia  (CORRESPO)
let NAME_MODULO_VERSION_DOCUMENT = "";
let NAME_CONTENT_ERROR_LIST_VERSION = "error_content_version_documento";
let NAME_GABINETE_VERSION = "";
let ID_IMAGEN_VERSION = 0;
let HEIGTH_TABLE_VERSION = 0;
let ID_GABINETE_VERSION_DOCUMENTO = 0;

const ini_event_version_document = () => {
    delete_alert_boot();
    let array_element = new Array;
    array_element.push({ id: "Button_activa_adjunta_document_version" }
    );
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", event_element_click_version_document, false);
        }
    }

}
const handler_element_version_document_event = (e) => {
    try {
        let name_ID = e.currentTarget.id;
        switch (name_ID) {
            //Activa carga de archivos para version
            case "Button_activa_adjunta_document_version":
                event_element_click_version_document(e);
                break;

        }
    } catch (ex) {
        alert(ex.mensaje);
    }
}
//------Captura los evento de click sobre botones 
const event_element_click_version_document = async (e) => {
    try {
        let result = "";
        let name_control = e.currentTarget.id;
        let name_modulo = ""
        delete_alert_boot();
        //Activa evento de carga de archivo
        if (name_control == "Button_activa_adjunta_document_version") {
            switch (MODULO_LISTA_VERSION) {
                case 1:
                    name_modulo = "MIGRACION"
                    break;
                case 2:
                    name_modulo = "WORKFLOW"
                    break;
                case 3:
                    name_modulo = "PRODUCCION"
                    break;
                case 3:
                    name_modulo = "DOCUARCHI"
                    break;
                case 5:
                    name_modulo = "RADICACION"
                    break;
                case 6:
                    name_modulo = "CORRESPO"
                    break;

            }
            NAME_MODULO_VERSION_DOCUMENT = name_modulo;
            //Asigna los parametros de gabinete y de imagen para modulo de migración
            if (name_modulo == "MIGRACION") {
                NAME_GABINETE_VERSION = GABINETE_MIG;
                ID_IMAGEN_VERSION = ID_IMAGEN_MIG;
            }
            //Asigna los parametros de gabinete y de imagen para modulo workflow
            if (name_modulo == "WORKFLOW") {
                //NAME_GABINETE_VERSION = GABINETE_MIG;
                //ID_IMAGEN_VERSION = ID_IMAGEN_MIG;
            }
            //Asigna los parametros de gabinete y de imagen para modulo pruducion
            if (name_modulo == "PRODUCCION") {
                //NAME_GABINETE_VERSION = GABINETE_MIG;
                //ID_IMAGEN_VERSION = ID_IMAGEN_MIG;
            }
            if (name_modulo == "DOCUARCHI") {
                //NAME_GABINETE_VERSION = GABINETE_MIG;
                //ID_IMAGEN_VERSION = ID_IMAGEN_MIG;
            }
            if (name_modulo == "RADICACION") {
                //NAME_GABINETE_VERSION = GABINETE_MIG;
                //ID_IMAGEN_VERSION = ID_IMAGEN_MIG;
            }
            if (name_modulo == "CORRESPO") {
                //NAME_GABINETE_VERSION = GABINETE_MIG;
                //ID_IMAGEN_VERSION = ID_IMAGEN_MIG;
            }
            e.currentTarget.disabled = true;
            posicion_update_pogres('progres_bar');
            let name_apost_control = "adjunta_documeto_version_document";
            let name_function_star_load = "start_file_save_UploadFile_document_verion";
            let name_control_modal = "modal_adjunta_documeto_version_document";
            result = await InitUploadFileClientAsync(name_apost_control, name_function_star_load);
            if (result !== "YES") {       
                document.getElementById(name_control).disabled = false;
                alert_bot(result, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
                return true;
            }
            result = await service_REST_parameter_upload_boot(name_modulo, name_control_modal, "", name_apost_control);
            if (result !== "YES") {          
                document.getElementById(name_control).disabled = false;
                alert_bot(result, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
            } else {
                document.getElementById(name_control).disabled = false;
            }
        }
    }
    catch (ex) {       
        alert_bot(ex.message, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
    } finally {
        progres_hiden('progres_bar');
    }
}
//------Función inicializadora de la copia de archivos
const start_file_save_UploadFile_document_verion = () => {
    let funcion_name = NAME_MODULO_VERSION_DOCUMENT;  // Determina el modulo que adjunta documento   //- 1 Mod migracion  (MIGRACION)   2- Modulo workflow (WORFKLOW)    -3 modulo producion (PRODUCCION)  4- modulo docuarchi (DOCUARCHI)  5- Radicacion  (RADICACION)  6- Modulo gestion correspondencia  (CORRESPO)
    let evento_adjunta = "ADJUNTAVERSION";
    let element_html_actuliza = "";
    let tipo_adjunta = 0; // Determina si remplaza version de documento en gabinete
    let id_respuesta = 0;
    let element_update_panel = "";
    let id_tipo_docuental = -1;
    let element_parent = "modal_adjunta_documeto_version_document";
    let numero_documento_relacionado = - 1;
    let estado_adjunto = -1;
    let estado_relacion = -1;
    let element_isert_table = "modal_adjunta_documeto_version_document";
    //Determina si remplaza la versión en el gabinete
    switch (NAME_MODULO_VERSION_DOCUMENT) {
        case "MIGRACION":
            tipo_adjunta = 0;
            break;
        case "WORKFLOW":
            tipo_adjunta = 0;
            break;
        case "PRODUCCION":
            tipo_adjunta = 0;
            break;
        case "CORRESPO":
            tipo_adjunta = 0;
            break;
    }
    star_copy_interval_file_Upload(estado_adjunto, estado_relacion, id_tipo_docuental, funcion_name, element_parent, evento_adjunta,
        numero_documento_relacionado, element_html_actuliza, element_update_panel, id_respuesta, tipo_adjunta, element_isert_table, "", NAME_GABINETE_VERSION, ID_IMAGEN_VERSION);
}
//----------Activa listar las versiones del documento
const list_show_version_document = async (id_imagen, id_gabinete, name_content_error, tipo_modulo) => {
    try {
        let result = "";
        MODULO_LISTA_VERSION = tipo_modulo;
        //NAME_CONTENT_ERROR_LIST_VERSION = name_content_error
        posicion_update_pogres('progres_bar');
        result = await Service_REST_listar_versiones_documentos(id_imagen, id_gabinete);
        if (result !== "YES") {
            alert_bot(result, 'warning', name_content_error);
            return true;
        }
       
    } catch (ex) {
        
        alert_bot(ex.message, 'warning', name_content_error);
    } finally {
        progres_hiden('progres_bar');
    }
}
//----------Activa listar las versiones del documento desde modulo que no tienen la idenitifcación del gabinete
const list_show_version_document_name_gabinete = async (id_imagen, gabinete, name_content_error, tipo_modulo) => {
    try {
        let result = "";
        MODULO_LISTA_VERSION = tipo_modulo;
        let id_gabinete = 0;
        posicion_update_pogres('progres_bar');
        result = await Service_REST_solicita_estructura_configuracion_gabinete(gabinete);
        if (result !== "YES") {
            alert_bot(result, 'warning', name_content_error);
            return true;
        }
        result = await Service_REST_listar_versiones_documentos(id_imagen, ID_GABINETE_VERSION_DOCUMENTO);
        if (result !== "YES") {
            alert_bot(result, 'warning', name_content_error);
            return true;
        }
    } catch (ex) {      
        alert_bot(ex.message, 'warning', name_content_error);
    } finally {
        progres_hiden('progres_bar');
    }
}
//---------Activa restaurar la versión del documento en el gabinete
const restore_version_document_gabinete = async (id_registro_version, e) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_Restaura_version_documento_gabinete(id_registro_version, MODULO_LISTA_VERSION);
        if (result !== "YES") {
            alert_bot(result, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
            return true;
        }
        
    } catch (ex) {  
        alert_bot(ex.message, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
    } finally {
        progres_hiden('progres_bar');
    }
}
//--------Activa la eliminación de una version de documentos
const delete_version_document = async (id_registro_version, elimina_permante, valida_firma_digital) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_elimina_version_documento(id_registro_version, MODULO_LISTA_VERSION, elimina_permante, valida_firma_digital);
        if (result !== "YES") {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
            return true;
        }
        progres_hiden('progres_bar');
    } catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
    }
}
//--------Activa la visusalizacion de version de documentos
const show_version_document_visor = async (id_registro_version) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_solicita_documentos_version(id_registro_version);
        if (result !== "YES") {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
            return true;
        }
        progres_hiden('progres_bar');
    } catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
    }
}
//-------Activa la descarga de versiones de documentos
const download_version_document = async (id_registro_version) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_descarga_version_documento(id_registro_version);
        if (result !== "YES") {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
            return true;
        }
        progres_hiden('progres_bar');
    } catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
    }
}
const detail_version_document = async (id_registro_version) => {
    try {
        let result = "";
        posicion_update_pogres('progres_bar');
        result = await Service_REST_detalle_version_documento(id_registro_version);
        if (result !== "YES") {
            progres_hiden('progres_bar');
            alert_bot(result, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
            return true;
        }
        progres_hiden('progres_bar');
    } catch (ex) {
        progres_hiden('progres_bar');
        alert_bot(ex.message, 'warning', NAME_CONTENT_ERROR_LIST_VERSION);
    }
}

//-------Operador de interface que lista las opciones en el tabla versiones del documento
function operate_list_version_document(value, row, index) {
    let ident = table_boot_return_objet_jonson(row);
    let IconoAwsome = "fal " + ident.IconoAsome;
    return [
        '<div class="row pl-2">',
        '<div class="col-8 p-0">',
        '<a class="active_version_document_view_document nav-link pl-5 font-weight-light" style="color: black" href="javascript:void(0)" title="Visualiza versión documento">  <i style="color: #0062cc" class="' + IconoAwsome + '"></i>  </a>',
        '</div > ',
        '<div class="col-4 p-0">',
        '<a class="nav-link  dropdown-toggle " style="color: black" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display:none" class="fad fa-th-list"></i>  ',
        '</a>',
        '<div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">',
        '<a class="active_version_document_view_document dropdown-item font-weight-light" href="javascript:void(0)" title="Visualiza versión documento">  <i style="color: #black" class="' + IconoAwsome + '"></i> Visualiza versión </a>',
        '<a class="active_version_detail_document dropdown-item font-weight-light" href="javascript:void(0)" title="Información de la versión del documento">  <i style="color: #black" class="far fa-info-square"></i> Detalle versión </a>',
        '<a class="active_donwload_document btn dropdown-item font-weight-light" href="javascript:void(0)" title="Descarga documento">  <i style="color: #black" class="fad fa-arrow-to-bottom"></i> Descarga versión </a>',
        '<a class="active_version_delete_version dropdown-item font-weight-light" href="javascript:void(0)" title="Elimina versión del documento">  <i style="color: #black" class="far fa-trash"></i> Elimina versión </a>',
        '<a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir del menu</a>',
        '</div>',
        '</a>',
        '</div>',
        '</div>',  
    ].join('')
}
//-------Controlador de eventos de la tabla versione de documento
 window.operateEventsVesrion  = {
   'click .active_version_delete_version': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        if (confirm("Desea eliminar el registro de versión (" + obje_jon.id_version_doc + ")")) {
            delete_version_document(obje_jon.id_registro_version, 0, 1);
        }
    }, 'click .active_version_document_view_document': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        show_version_document_visor(obje_jon.id_registro_version);

    }, 'click .active_donwload_document': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        download_version_document(obje_jon.id_registro_version);
    }, 'click .active_version_detail_document': (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        detail_version_document(obje_jon.id_registro_version);
    }, 'click .cheke_event_asing' : (e, value, row, index) => {
        let obje_jon = table_boot_return_objet_jonson(row);
        if (e.currentTarget.checked === true) {
            restore_version_document_gabinete(obje_jon.id_registro_version, e);
        }
     }
}
function version_operateFormatter_asing(value, row, index) {
    let ident = table_boot_return_objet_jonson(row);
    let chekd = false;
    if (ident.ESTADO_ACTIVO_GABINETE == "SI") {
        chekd = "checked";
    } else {
        chekd = "";
    }
    return [
        '<label> <input id=' + ident.id_registro_version +  '_radio_version_any title="Asigna la versión como documento principal en el gabinete"  data-index="0" type="radio" name="my-input" class="cheke_event_asing ml-4"  ', chekd, '> </label>'
    ].join('')
}
const restore_value_radio_version_active = (table) => {
    let ident = table_boot_return_get_data(table);
    if (ident) {
        for (i = 0; i <= (ident.length-1); i++) {
            if (ident[i].ESTADO_ACTIVO_GABINETE == "SI") {
                let name_element = ident[i].id_registro_version + '_radio_version_any';
                let ElementRadio = document.getElementById(name_element);
                if (ElementRadio) {
                    if (ElementRadio.checked == false) {
                        ElementRadio.checked = true;
                    }     
                }
            }
        }
    }
}
//-----Descarga la version del archivo en la interface del cliente
const dowload_file_version = (filename, name_file) => {
    try {
        var element = document.createElement('a');
        element.setAttribute('href', filename);
        element.setAttribute('download', name_file);
        element.style.display = 'none';
        document.body.appendChild(element);
        element.click();
        document.body.removeChild(element);
    } catch (err) {
        alert(err.mensaje + " funcion dowload_file_version");
    }
}
//-------Inserta registro version documento en la interface de la lista de versiones
const insert_new_versio_document = (stru_parserdocument) => {
    if (stru_parserdocument[0].Class_list_detalle_version_document[0].option_remplaza == 1) { return true; }
    if (stru_parserdocument[0].Class_list_detalle_version_document[0].error_sistema != "YES") {
        alert_bot(stru_parserdocument[0].Class_list_detalle_version_document[0].error_sistema, 'warning', "error_content_adjunta_documeto_version_document_005");
        return true;
    }
    let $table = $('#tabl_lista_version_documento');
    $table.bootstrapTable('insertRow', {
        index: 150000000,
        row: {
            id_registro_version: stru_parserdocument[0].Class_list_detalle_version_document[0].id_registro_version,
            id_version_doc: stru_parserdocument[0].Class_list_detalle_version_document[0].id_version_doc,
            fecha_registro_version: stru_parserdocument[0].Class_list_detalle_version_document[0].fecha_registro_version,
            ESTADO_ACTIVO_GABINETE: stru_parserdocument[0].Class_list_detalle_version_document[0].ESTADO_ACTIVO_GABINETE,
            IconoAsome: stru_parserdocument[0].Class_list_detalle_version_document[0].IconoAsome
        }
    })
    if (stru_parserdocument[0].Class_list_detalle_version_document[0].ESTADO_ACTIVO_GABINETE != "") {
        $table.bootstrapTable('updateCellByUniqueId', {
            id: stru_parserdocument[0].Class_list_detalle_version_document[0].id_registro_version_old,
            field: 'ESTADO_ACTIVO_GABINETE',
            value: "",
            reinit: false
        });
        $table.bootstrapTable('updateCellByUniqueId', {
            id: stru_parserdocument[0].Class_list_detalle_version_document[0].id_registro_version,
            field: 'ESTADO_ACTIVO_GABINETE',
            value: "SI",
            reinit: true
        });
        if (NAME_MODULO_VERSION_DOCUMENT == "MIGRACION") {
            $table = $('#table_consulta_migracion');
            $table.bootstrapTable('updateCellByUniqueId', {
                id: stru_parserdocument[0].Class_list_detalle_version_document[0].ID,
                field: 'ESTENSION',
                value: stru_parserdocument[0].Class_list_detalle_version_document[0].TIPO_ARCHIVO,
                reinit: true
            });
        }
        if (NAME_MODULO_VERSION_DOCUMENT == "WORKFLOW") {
            //Ejecuta actualizacion worfkflow
        }
    }
    _JSVesrionDocumento._ModalLoadDocumento.modal("hide");
}
//-------Detalle version de documento
const Service_REST_detalle_version_documento = async (id_registro_version) => {
    let myPromise = new Promise(function (resolve) {

        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_solicita_detalle_version_documento', {
                data: "{" + "'id_registro_version':'" + id_registro_version + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        document.getElementById("spn_id_registro_version").innerText = data.d[0].id_registro_version;
                        document.getElementById("spn_id_version_doc").innerText = data.d[0].id_version_doc;
                        document.getElementById("spn_fecha_registro_version").innerText = data.d[0].fecha_registro_version;
                        document.getElementById("spn_tipo_archivo").innerText = data.d[0].TIPO_ARCHIVO;
                        document.getElementById("spn_peso_documento").innerText = data.d[0].PESO_DOCUMENTO;
                        document.getElementById("spn_paginas_document").innerText = data.d[0].PAGINAS_DOCUMENT;
                        if (document.getElementById("spn_productor_document")) {
                            document.getElementById("spn_productor_document").innerText = data.d[0].name_usuario;
                        }
                        $("#modal_detalle_version_documento").modal("show");
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
//-------Descarga documento version
const Service_REST_descarga_version_documento = async (id_registro_version) => {
    let myPromise = new Promise(function (resolve) {

        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_descarga_version_documento', {
                data: "{" + "'id_registro_version':'" + id_registro_version + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        dowload_file_version(data.d[0].url_iframe, data.d[0].name_file);
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
const Service_REST_elimina_version_documento = async (id_registro_version, tipo_modulo, elimina_permante, valida_firma_digital) => {
    let myPromise = new Promise(function (resolve) {

        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_elimina_version_documento', {
                data: "{" + "'id_registro_version':'" + id_registro_version + "','" + "tipo_modulo':'" + tipo_modulo + "','elimina_permante':'" + elimina_permante + "','valida_firma_digital':'" + valida_firma_digital + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);

                    } else {
                        let $table = $('#tabl_lista_version_documento');
                        $table.bootstrapTable('removeByUniqueId', id_registro_version);
                        document.getElementById("Iframe_document_visor_version").src = "";
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
//-------Solicita los datos de visualización del documento version
const Service_REST_solicita_documentos_version = async (id_registro_version) => {
    let myPromise = new Promise(function (resolve) {
        document.getElementById("Iframe_document_visor_version").src = "";
        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_solicita_documentos_version', {
                data: "{" + "'id_registro_version':'" + id_registro_version + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);
                    } else {
                        document.getElementById("Iframe_document_visor_version").src = data.d[0].url_iframe;
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
//-------Restaura la versión del documento en el gabinete
const Service_REST_Restaura_version_documento_gabinete = async (id_registro_version, tipo_modulo) => {
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_Restaura_version_documento_gabinete', {
                data: "{" + "'id_registro_version':'" + id_registro_version + "','" + "tipo_modulo':'" + tipo_modulo + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        restore_value_radio_version_active('tabl_lista_version_documento');
                        resolve(data.d[0].Error_result);
                    } else {
                        let $table = $('#tabl_lista_version_documento');
                        if (MODULO_LISTA_VERSION == 1) {
                            $table = $('#table_consulta_migracion');
                            $table.bootstrapTable('updateCellByUniqueId', {
                                id: data.d[0].imagen,
                                field: 'ESTENSION',
                                value: data.d[0].extension_archivo,
                                reinit: false
                            });
                        }
                        let IconoAsome = data.d[0].ILIST_lista_detalle_version_document[0].IconoAsome;
                        if (_JSVesrionDocumento.settings.TipoTable == "asp.net") {
                                _JSVesrionDocumento._ChangeIconoImageTableAspnet(_JSVesrionDocumento.settings.name_class_element_icono_aspnet, IconoAsome);
                        }
                        let ValueIcono = 0;
                        if (IconoAsome == "fa-file-certificate") {
                            ValueIcono = 1;
                        }
                        if (_JSVesrionDocumento.settings.TipoTable == "bootstrap") {
                            updateCelByUniqueIdReinit(_JSVesrionDocumento.settings.NameTable, _JSVesrionDocumento.settings.NameCampo, _JSVesrionDocumento.settings.IdImagen, ValueIcono);
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
//-------Lista la versiones del documento
const Service_REST_listar_versiones_documentos = async (id_imagen, id_gabinete) => {
    let myPromise = new Promise(function (resolve) {
        document.getElementById("Iframe_document_visor_version").src = "";
        try {
            $.ajax('../webservice/WebServiceVersionDocumento.asmx/Service_lista_versiones_de_documentos', {
                data: "{" + "'id_imagen':'" + id_imagen + "','" + "id_gabinete':'" + id_gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);

                    } else {
                        init_row_constant_table_boostrap_table("tabl_lista_version_documento", data.d[0].ILIST_lista_detalle_version_document,
                            "content_tabl_lista_version_documento", null, null, "", "single", "bt-selected", false, false, false,
                            false, ["active_version_document_view_document"]);
                        document.getElementById("h_title_gabinete_image").innerText = "DOCUMENTO  : " + id_imagen + "  GABINETE : " + data.d[0].Gabinete;
                        $("#modal_version_document").modal("show");
                        auto_zise_version_document();
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
const Service_REST_solicita_estructura_configuracion_gabinete = async (gabinete) => {
    let myPromise = new Promise(function (resolve) {

        try {
            $.ajax('../webservice/WebServiceDocuarchi.asmx/Service_solicita_estructura_configuracion_gabinete', {
                data: "{" + "'gabinete':'" + gabinete + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].Error_result !== "YES") {
                        resolve(data.d[0].Error_result);

                    } else {
                        ID_GABINETE_VERSION_DOCUMENTO = data.d[0].id_gabinete;
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
//----------Debijua y redimenciona la tabla que muestra la lista de versiones
function auto_zise_version_document() {
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
        let height_header = document.getElementById('header_modal_version_document').clientHeight;
        let height_footer = document.getElementById('footer_modal_version_document').clientHeight;
        let height_toolbar = document.getElementById('tool_bar_version_document').clientHeight;
        if (height_footer < 1) {
            height_footer = 73;
        }
        if (height_header < 1) {
            height_header = 73;
        }
        if (height_toolbar < 1) {
            height_toolbar = 73;
        }

        $('#content_tabl_lista_version_documento').css("height", ((espacio_iframe - (height_header + height_header + height_toolbar))) + "px");
        $('#content_view_version_documento').css("height", ((espacio_iframe - (height_header + height_header + height_toolbar))) + "px");
        let heig_table = (espacio_iframe - (height_header + height_header + height_toolbar));
        table_reize_heigth("tabl_lista_version_documento", heig_table, "", "table-borderless");
       
    } catch (ex) { alert("Funcion auto_zise_version_document " + ex.message); }

}