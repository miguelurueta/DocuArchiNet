/*2025-09-01
 * Miguel Angel Urueta Miranda
 Como Utilizar
 JS OBLIGATORIO 
    <script src="../js/java_general/general_code_java.js" type="text/javascript"></script>
    <script src="../js/java_general/ASMXClient.js" type="text/javascript"></script>
    <script src="../js/java_general/JSProgresBar.js" type="text/javascript"></script>
    <script src="../js/Docuarchi/JSIndiceGabinete.js"></script>
    <script src="../js/table_boo/table_boot_config.js" type="text/javascript"></script>
    <script src="../js/java_general/BootstrapTable.js" type="text/javascript"></script>
 */
//Eemplos
/*
 * Ejemplos
 * Parametros 
 * IdGabineteConsulta     : Representa la identificación del gabinete
 * NombreGabineteConsulta : Representa el nombre del gabinete
 * IdmagenGabinete        : Representa la identifcación de la imagen del indice
 * NameParent             : Representa el nopmbre del control padre
 * NameEspaceControl      : Represental el nombre del espacion de nombres o clase que agrupa los campos del indice
 * NameModulo             : Representa el nombre del modulo que instancia el indice
 * NameTable              : Representa el nombre de la tabla 
 * NameAsmxUpdaTeIndice   : Rpresenta el servicio que actualiza el indice
 * OptionIndice           : Representa si actualzia un indice   o multiples indice 1 ->Varios indices   2-> Un solo indice
 * Opcion para actualziar un solo indice
 *  const EventActivaIdiceBatch = async (Idmagen) => {
    try {
        let Option = [{
            IdGabineteConsulta: IdGabineteConsulta , NombreGabineteConsulta: NombreGabineteConsulta,
            IdmagenGabinete: Idmagen, NameParent: "parent_workflow", NameEspaceControl: "class_indice_batch_gabinete_001",
            NameModulo: "DOCUARCHI", NameTable: "table_consulta_gabinete", NameAsmxUpdaTeIndice: "ServiceActualizaIndiceBatchGabinete",
            OptionIndice:2
        }];
        _JSIndiceGabinete = new JSindiceGabinete(Option[0]);
        let Result = await _JSIndiceGabinete._LoadIndexGabinete();
        return Result;
    } catch (ex) {
        return "Inconsistencia EventActivaIdiceBatch " + ex.message;
    }
}
Opcion para actualziar varios indices
const EventActivaMultplexIndexBatch = async () => {
    try {
        let SelectionTable = new Array();
        SelectionTable = $('#' + 'table_consulta_gabinete').bootstrapTable('getSelections');
        if (SelectionTable.length == 0) { return  "Debe seleccionar los ítems de la tabla que desea actualizar." }
        let Option = [{
            IdGabineteConsulta: IdGabineteConsulta, NombreGabineteConsulta: NombreGabineteConsulta,
            IdmagenGabinete: SelectionTable[0].ID, NameParent: "parent_workflow", NameEspaceControl: "class_indice_batch_gabinete_001",
            NameModulo: "DOCUARCHI", NameTable: "table_consulta_gabinete", NameAsmxUpdaTeIndice: "ServiceActualizaIndiceBatchGabinete"
        }];
        _JSIndiceGabinete = new JSindiceGabinete(Option[0]);
        let Result = await _JSIndiceGabinete._LoadIndexGabinete();
        return Result;
    } catch (ex) {
        return "Inconsistencia general funcion EventActivaMultplexIndexBatch " + ex.message
    }
}
 */
class JSindiceGabinete {
    constructor(options = {}) {
        let defaults = {
            NameParent: "",
            NameContendorLoadDocumento: "div_paren_load_indice_batch_gabinete_001",///-----Representa el contendor del formulario
            NameContendorError: "error_actualiza_indice_batch_gabinete_001",       ///-----Representa el nombre del control error  del indice
            NameModalIndice: "modal_actualiza_indice_batch_gabinete_001",          ///-----Representa el nombre del control modal el indice
            NameBotonIndiceBatch: "btn_indice_Batch_gabinete_001",                 ///-----Representa el nombre del control que actualiza el indice
            NameControlParentCampos: "div_actualiza_indice_batch_gabiente_001",    ///-----Representa el nombre del contendor padre de los campos 
            NameEspaceControl: "class_indice_gabinete_001",                        ///-----Representa el nombre de clase que identiifca los campos del formulario
            AsignaValorControl: "1",                                               ///-----Representa si asigna valores a los campos
            ApostNameControl: "",                                                  ///-----Representa el nombre del control apost del check
            AddCheckControl: "1",                                                  ///-----Representa si agrega check a los controles
            IdGabineteConsulta: "0",                                               ///-----Representa la identificación del gabinete
            NombreGabineteConsulta: "",                                            ///-----Representa el nombre del gabinete
            NameTable: "",                                                         //------Representa el nombre de la tabla donde se registran los documento y donde se actualizan los campos 
            TipoTable: "Boot",                                                     //------Representa el tipo de tabla
            NombreServcioAsmx: "ServiceCreaInterfazindiceGabinete",                ///-----Representa el nombre del servicio asmx
            NombreServicio: "Docuarchi",                                           ///-----Representa el nombre del servicio 
            IdmagenGabinete: "0",                                                  ///-----Representa la identificación de la imagen
            NameServiceAutoComplete: "Docuarchi",                                  ///-----Representa el nombre del servicio que contiene el asmx
            NameAsmx: "ServiceSolicitaAutoCompleteCampoGabinete",                  ///-----Representa el nombre del servicio de auto complete asmx
            NameAsmxUpdaTeIndice: "",                                              ///-----Representa el nombre del servicio de auto complete asmx
            NameModulo: "",                                                        ///-----Representa el nombre del módulo DOCUARCHI-WORKFLOW-PRODUCCION-MIGRACION
            OptionIndice: 1                                                        ///-----Representa si valida el formulario es de tipo bacth o indece simple 1-Batch  2-Indice
        }
        this.settings = $.extend(true, defaults, options);
        this.SpinnerManager = new SpinnerManager();
        this.asmxClient = new ASMXClient(AsmxServicesConfig);
        this.ArregloEstructuraControles = [];
        this.ArregloEstructuraControlesAsignados = [];
        this.ContadorProgress = 0;
        this.LengProgress = 0;
    }
    /*Crea el formulario modal
     * del indice
     * 
     */ 
    _CreateModalIndex = async () => {
        try {
            if (!this.settings.NameParent) {
                return "Informe el nombre del contenedor del formulario del Índice";
            }

            const parentElement = document.getElementById(this.settings.NameParent);
            if (!parentElement) {
                return `El contenedor con el ID ${this.settings.NameParent} no existe.`;
            }

            if (document.getElementById(this.settings.NameContendorLoadDocumento)) {
                document.getElementById(this.settings.NameContendorLoadDocumento).remove();
            }

            const wrapper = document.createElement('div');
            wrapper.id = this.settings.NameContendorLoadDocumento;
            wrapper.innerHTML = [
                '<div class="modal fade modal_opacity" style="z-index:100061" id="' + this.settings.NameModalIndice + '" role="dialog" data-backdrop="false">',
                '<div class="modal-dialog  modal-mediunscreen-sm-down modal-dialog-scrollable">',
                '<div class="modal-content-fullscreen">',
                '<div class="modal-header">',
                '<h5 style="color: black" class="modal-title">Actualiza índice</h5>',
                '<button type="button" class="close" data-dismiss="modal">&times;</button>',
                '</div>',
                '<div class="modal-body-fullscreen modal-body" style="overflow:auto">',
                '<div id="' + this.settings.NameControlParentCampos + '" style="height: 100%">',
                '</div>',
                '</div>',
                '<div id="' + this.settings.NameContendorError + '" style="position: relative; width: 100%"></div>',
                '<div class=" modal-footer">',
                '<button type="button" id="' + this.settings.NameBotonIndiceBatch + '" class="btn btn-primary" title="Actualiza índice gabinete">Aceptar</button>',
                '</div>',
                '</div>',
                '</div>',
                '</div>',
            ].join('');
            parentElement.append(wrapper);

            // Agregar evento con el contexto adecuado
            const botonIndice = document.getElementById(this.settings.NameBotonIndiceBatch);
            if (botonIndice) {
                botonIndice.addEventListener("click", this._EventElementClickPromiseIndex.bind(this), false);
            }

            return "YES";
        } catch (ex) {
            return "Inconsistencia general en función CreateModalIndex: " + ex.message;
        }
    }
    /*Agrega los campos indice del formulario modal */
    _CreateCamposModalIndex = async () => {
        try {
            const NombreControlPadre = document.getElementById(this.settings.NameControlParentCampos);
            if (!NombreControlPadre) {
                return `Imposible encontrar el control padre de la interfaz ${this.settings.NameControlParentCampos}`;
            }
            // Limpiar los campos anteriores
            while (NombreControlPadre.hasChildNodes()) {
                NombreControlPadre.removeChild(NombreControlPadre.firstChild);
            }
            // Crear div de alerta
            let divtml = document.createElement("div");
            divtml.classList.add("w-100", "row", "p-1", "alert-danger-alert", "text-center-alert", "hide-alert", "alert-alert", `${this.NameEspaceControl}error_alert`);
            NombreControlPadre.appendChild(divtml);

            // Iterar sobre los controles
            for (let i = 0; i < this.ArregloEstructuraControles.length; i++) {
                const control = this.ArregloEstructuraControles[i];
                // Agregar campo DIV ROW
                divtml = document.createElement("div");
                divtml.classList.add("row", "p-1");
                NombreControlPadre.appendChild(divtml);
                // Agregar campo DIV COLUMNA
                let divtml_ = document.createElement("div");
                divtml_.classList.add("col-12","col-sm-6");
                /*if (control.type_cells_alow_control === "group") {
                    divtml_.classList.add("btn-group", "col-12", "p-1", "pt-3");
                } else {
                    divtml.classList.add("p-1");
                    divtml_.classList.add("col-6");
                }*/
                // Agregar control checkbox si está activada la función    `${control.name_campo}_${this.settings.NameEspaceControl}`;
                if (this.settings.AddCheckControl === "1") {
                    let imputhml = document.createElement("INPUT");
                    imputhml.setAttribute("type", "checkbox");
                    imputhml.setAttribute("atrib_campo_n", `chek_${control.name_campo}`);
                    imputhml.classList.add(`chek_${this.settings.ApostNameControl}`);
                    imputhml.id = `chek_item_${control.name_campo}_${this.settings.NameEspaceControl}`;
                    imputhml.classList.add("mr-1");
                    imputhml.classList.add("ml-2");
                    imputhml.classList.add("form-check-input");
                    imputhml.setAttribute("tabindex", "-1");
                    divtml_.appendChild(imputhml);
                }
                // Agregar el control SPAN
                let spntml = document.createElement("span");
                spntml.classList.add("h6");
                const labelClass = "font-weight-light";
                spntml.classList.add(labelClass);
                let string_ = control.aleas_campo;
                let estado_obligatorio = control.obligatorio_campo === "1" ? " *" : " ";
                if (control.type_cells_alow_control === "group") {
                    spntml.classList.add("control-label");
                }
                let textoCapitalizado = string_.toLowerCase().split(" ").map(function (palabra) {
                    return palabra.charAt(0).toUpperCase() + palabra.slice(1);
                }).join(" ");
                spntml.innerHTML = textoCapitalizado;
                divtml_.appendChild(spntml);
               
                // Agregar el control popup ayuda
                //if (control.tooltipAyuda) {
                    /*let itml = document.createElement("i");
                    itml.classList.add("fal", "fa-info-circle", "ml-1");
                    let atml = document.createElement("a");
                    atml.setAttribute("data-bs-toggle", "tooltip");
                    atml.setAttribute("data-bs-placement", "top");
                    atml.setAttribute("title", control.tooltipAyuda);
                    atml.appendChild(itml);
                    divtml_.appendChild(atml);*/
                //}

                divtml.appendChild(divtml_);
                // Agregar campo para control de entrada (Input, Select, Textarea)
                divtml_ = document.createElement("div");
                divtml_.classList.add("col-12", "col-sm-6");
                /*if (control.type_cells_alow_control === "group") {
                    divtml_.classList.add("btn-group", "col-12");
                    divtml = document.createElement("div");
                    divtml.classList.add("row", "mt-0");
                    NombreControlPadre.appendChild(divtml);
                } else {
                    divtml_.classList.add("col-6");
                }*/
                let imputhml;
                switch (control.campo_tip) {
                    case "1":  // Tipo INPUT
                        imputhml = document.createElement("INPUT");
                        if (this.settings.AsignaValorControl === "1") {
                            imputhml.value = control.value_campo;
                        }
                        break;

                    case "0":  // Tipo SELECT
                        imputhml = document.createElement("SELECT");
                        imputhml.classList.add("form-select");
                        if (control.ilist_row_drowlist) {
                            control.ilist_row_drowlist.forEach(option => {
                                let opt = document.createElement("OPTION");
                                opt.text = option.value_campo;
                                opt.value = option.id_value;
                                if (this.settings.AsignaValorControl === "1" && option.value_campo === control.texto_campo) {
                                    opt.selected = true;
                                }
                                imputhml.add(opt);
                            });
                        }
                        imputhml.addEventListener("change", event_change_drowslis_form);
                        break;

                    case "2":  // Tipo TEXTAREA
                        imputhml = document.createElement("TEXTAREA");
                        if (this.settings.AsignaValorControl === "1") {
                            imputhml.value = control.value_campo;
                        }
                        break;

                    default:
                        return `Tipo de campo desconocido: ${control.campo_tip}`;
                        break;
                }

                // Validación de campo (habilitado/deshabilitado)
                if (control.disable_campo === 0) {
                    imputhml.disabled = true;
                }
                //Agrega atribute date           
                switch (control.tipo_campo) {
                    case "DATE":
                        imputhml.placeholder = "yyyy mm dd";
                        imputhml.classList.add("w-50");
                        imputhml.addEventListener("keypress", this._ValidateKeyPrresFechaFormControlIndiceBatch);
                        imputhml.addEventListener("blur", this._ValidateFechaFocusIndiceBatch);
                        imputhml.setAttribute("max-length", "10");
                        imputhml.classList.add("form-control-person");
                        break;
                    case "INT":
                        imputhml.addEventListener("keypress", this._ValidatekeyPressNumeroFormControlIndiceBatch);
                        imputhml.classList.add("w-50");
                        imputhml.classList.add("form-control-person");
                        break;
                    default:
                        imputhml.classList.add("w-100");
                        imputhml.classList.add("form-control");
                }
                //Valida campos tipo correo electrónico//
                if (control.control_tip_correo == 1) {
                    imputhml.addEventListener("blur", this._validateEmailFormFocusControlIndiceBatch);
                }
                // Asignación de atributos adicionales
                imputhml.maxLength = control.max_leng_campo;
                imputhml.setAttribute("atrib_aleas_c", control.aleas_campo);
                imputhml.setAttribute("atrib_campo_O", control.obligatorio_campo);
                imputhml.setAttribute("atrib_campo_n", control.name_campo);
                imputhml.setAttribute("atrib_campo_v", control.alow_tipo_value);
                imputhml.setAttribute("atrib_campo_tip", control.campo_tip);
                imputhml.setAttribute("atrib_campo_nl", control.alow_null);
                imputhml.setAttribute("atrib_campo_id", control.dms_id_registro);
                imputhml.setAttribute("atrib_name_campo_id", control.name_campo_id);
                imputhml.setAttribute("atrib_campo_t", control.tipo_campo);
                imputhml.setAttribute("atrib_campo_tbl", control.tbl_control);
                imputhml.setAttribute("atrib_campo_drow_destino", control.drow_name_controls_destino);
                imputhml.setAttribute("atrib_name_espace_control", control.name_space_campo);
                imputhml.setAttribute("atrib_control_tip_correo", control.control_tip_correo);
                imputhml.setAttribute("atrib_value_campo_old", control.value_campo_old);
                imputhml.setAttribute("atrib_drow_name_control_id", control.drow_name_control_id);
                imputhml.setAttribute("atrib_Tom_alow", control.Tom_alow);
                imputhml.id = `${control.name_campo}_${this.settings.NameEspaceControl}`;
                imputhml.classList.add(this.settings.NameEspaceControl);
                imputhml.classList.add("form-control-person");
                
                let SpanHtml = document.createElement("SPAN");
                SpanHtml.classList.add("w-auto");
                SpanHtml.classList.add("text-danger");
                SpanHtml.setAttribute("data-asp-danger", imputhml.id);


                // Agregar el control al contenedor
                divtml_.appendChild(imputhml);
                divtml_.appendChild(SpanHtml);

                if (control.disable_campo === 0) {
                    imputhml.disabled = true;
                }

                if (control.type_cells_alow_control === "group") {
                    imputhml.classList.add("w-100");
                }
                if (control.max_leng_campo) {
                    imputhml.maxLength = control.max_leng_campo;
                }
                imputhml.classList.add(this.settings.NameEspaceControl);
                divtml_.appendChild(imputhml);
                //Agrega el toltip de ayuda
              
                divtml.appendChild(divtml_);
                this.asmxClient.autoCompleteNative(
                    this.settings.NameServiceAutoComplete,
                    imputhml.id,
                    this.settings.NameAsmx,
                    {
                        NameDbsAuto: "",
                        NameTableAuto: this.settings.NombreGabineteConsulta,
                        NameCampoAuto: control.name_campo,
                        IdTable: this.settings.IdGabineteConsulta
                    },
                    async (Value, inputEl) => {
                        try {
                            this.asmxClient.showSpinner(inputEl); // 🔹 spinner sigue activo durante tu lógica
                            //let Rest = await EventSolicitaConsultaLikeGabinete(Value, 2);
                        } finally {
                            this.asmxClient.removeSpinner(inputEl); // 🔹 se quita solo al terminar
                        }
                    },
                    { minChars: 4, maxResults: 15 }
                );
               
            }
            return "YES";
        } catch (ex) {
            return `Inconsistencia general en función _CreateCamposModalIndex: ${ex.message}`;
        }
    }
    /**Valida los parametros obligatorios para incializar el componete */
    _ValidaParameter = async () => {
        
        if (this.settings.NameModulo == "") { return "Debe informar el nombre del modulo" };
        if (this.settings.NameParent == "") { return "Debe informar el nombre del control padre" };
        if (this.settings.NameTable == "") { return "Debe informar el nombre de la tabla" };
        if (this.settings.NameAsmxUpdaTeIndice == "") { return "Debe informar el nombre de la función asmx para actualizar indice" };
        return "YES";
    }
    /**Inicializa el componente indice solicitando los permisos del usuario sobre el gabinete */
    _LoadIndexGabinete = async () => {
        try {
            const CDParamenterGabinete = [{
                IdGabinete: this.settings.IdGabineteConsulta,
                NombreGabinete: this.settings.NombreGabineteConsulta,
                IdImagen: this.settings.IdmagenGabinete,
                NameEspaceControl: this.settings.NameEspaceControl
            }];
            let result = await this._ValidaParameter();
            if (result != "YES") { return result };
            /**Solicita la estructurua de los capos 
             *WebServiceDocuarchi -> ServiceCreaInterfazindiceGabinete --> SolicitaEstructuraValoresCamposIndice*/
            let resp1 = await this.asmxClient
                .use(this.settings.NombreServicio)
                .call(this.settings.NombreServcioAsmx, { Parameter: CDParamenterGabinete });
            if (resp1.error) {
                return `${resp1.message} ${resp1.status}`;
            }

            if (resp1.message !== "YES") {
                return resp1.message;
            }
            let Data = resp1.data;
            this.ArregloEstructuraControles = Data;
            /*
             * Crea el formulario modal 
             */
            result = await this._CreateModalIndex();
            if (result !== "YES") { return result; }
            /*
             * Agrega los campos al formulario
             */
            result = await this._CreateCamposModalIndex();
            if (result !== "YES") { return result; }
            /*
             * Abre el formulario
             */
            result = await this._ToggleModal(this.settings.NameModalIndice, "show");
            if (result !== "YES") { return result; }
            return "YES";
        } catch (ex) {
            return `Inconsistencia en función _LoadIndexGabinete: ${ex.message}`;
        }
    }
    /**
     * Evento promise que que ejecuta los eventos del indice
     * @param {any} e
     */
    _EventElementClickPromiseIndex = async (e) => {
        let IdControl = e.currentTarget.id;
        try {
            let result = "";
            delete_alert_boot();
            e.currentTarget.disabled = true;
            if (document.getElementById(IdControl)) {
                this.SpinnerManager.mostrarProgresBar();
                this.SpinnerManager.showOnButton(IdControl, "circle");
            }
            switch (IdControl) {
                case this.settings.NameBotonIndiceBatch:
                    if (this.settings.OptionIndice == 1) {
                        this.SpinnerManager.ocultarProgresBar();
                        this.SpinnerManager.hideOnButton(IdControl);
                        result = await this._EventActivaActualizaIndiceBatch();
                        if (result != "YES") { alert_bot(result, 'warning', this.settings.NameContendorError) };
                    } else {
                        result = await this._EventActivaActualizaIndice();
                        if (result != "YES") { alert_bot(result, 'warning', this.settings.NameContendorError) };
                    }
                    break;

                default:
                    alert_bot(`El control (${IdControl}) no tiene evento registrado.`, 'warning', this.settings.NameContendorContenedorError);
            }
        } catch (ex) {
            alert_bot(ex.message, 'warning', this.settings.NameContendorContenedorError);
        } finally {
            if (document.getElementById(IdControl)) {
                this.SpinnerManager.hideOnButton(IdControl);
                this.SpinnerManager.ocultarProgresBar();
            }
            document.getElementById(IdControl).disabled = false;
        }
    }
    /**
     * Evento que activa la actualización de multplex indieces
     * @param {any} e
     */
    _EventActivaActualizaIndiceBatch = async (e) => {
        try {
            let SelectionTable = new Array();
            switch (this.settings.TipoTable) {
                case "Boot":
                    SelectionTable = $('#' + this.settings.NameTable).bootstrapTable('getSelections');
                    break;
                default:
                    return "Tipo table (" + this.settings.TipoTable + ") no identificado ";
            }
            if (SelectionTable.length == 0) { return "Debe seleccionar los ítems de la tabla que desea actualizar." };
            this.ContadorProgress = 0;                              //-----Incializa contador progrees
            this.LengProgress = SelectionTable.length               //-----Incializa el numero de item del progress
            /**Solicita los datos del formulario indice */
            let Result = await this._SolicitaDatosControlIndiceBatch();
            if (Result == "NO") { return "YES" };
            if (Result != "NO" && Result != "YES") { return Result};
            if (SelectionTable.length == 1) {
                const CDParamenterGabinete = [{
                    IdGabinete: this.settings.IdGabineteConsulta,
                    NombreGabinete: this.settings.NombreGabineteConsulta,
                    IdImagen: this.settings.IdmagenGabinete,
                    NameEspaceControl: this.settings.NameEspaceControl,
                    ClassConfigGeneralService: this.ArregloEstructuraControlesAsignados,
                    NombreModulo: this.settings.NameModulo
                }];
                Result = await this._ActualizaIndiceBatch(CDParamenterGabinete);
                if (Result !== "YES") { return Result; }
                Result = await this._ToggleModal(this.settings.NameModalIndice,"hide");
                if (Result !== "YES") { return Result; }
            } else {
                const CDParamenterGabinete = [];
                SelectionTable.forEach(r => {
                    let ParamenterGabinete = [];
                    ParamenterGabinete.push({
                        IdGabinete: this.settings.IdGabineteConsulta,
                        NombreGabinete: this.settings.NombreGabineteConsulta,
                        IdImagen: r.ID,
                        NameEspaceControl: this.settings.NameEspaceControl,
                        ClassConfigGeneralService: this.ArregloEstructuraControlesAsignados,
                        NombreModulo: this.settings.NameModulo
                    })
                    CDParamenterGabinete.push(ParamenterGabinete);
                });
                let _OPtionProgresBar = ({
                    name_service: "ActualizaIndiceBatch",
                    OptionItemSelect: CDParamenterGabinete,
                    NameControlPadreProgres: this.settings.NameParent, NameProceso: "Actualizando Indices", ObjectComponente: this
                });
                Result = await JSProgresBarBoot(_OPtionProgresBar);
                return Result;
            }
            return "YES";
        } catch (ex) {
            return "Inconsistencia funcion _EventActivaActualizaIndiceBatch " + ex.message;
        }
    }
    /**
     * Evento que activa la actualización de un solo indice
     * @param {any} e
     */
    _EventActivaActualizaIndice = async (e) => {
        try {
            let Result = await this._SolicitaDatosControlIndiceBatch();
            if (Result == "NO") { return "YES" };
            if (Result != "NO" && Result != "YES") { return Result };
            const CDParamenterGabinete = [{
                IdGabinete: this.settings.IdGabineteConsulta,
                NombreGabinete: this.settings.NombreGabineteConsulta,
                IdImagen: this.settings.IdmagenGabinete,
                NameEspaceControl: this.settings.NameEspaceControl,
                ClassConfigGeneralService: this.ArregloEstructuraControlesAsignados,
                NombreModulo: this.settings.NameModulo
            }];
            Result = await this._ActualizaIndiceBatch(CDParamenterGabinete,false);
            if (Result !== "YES") { return Result; }
            Result = await this._ToggleModal(this.settings.NameModalIndice, "hide");
            if (Result !== "YES") { return Result; }
            return "YES";
        } catch (ex) {
            return "Inconsistencia general funcion  _EventActivaActualizaIndice " + ex.message;
        }
    }
    /**
     * Función que actualiza los indices en el gabinete
     * @param {any} CDParamenterGabinete   
     * @param {any} uncheckRow
     */
    _ActualizaIndiceBatch = async (CDParamenterGabinete, uncheckRow = true) => {
        try {
            // Llamada asíncrona a la API para actualizar el índice   
            // 🚀ServiceActualizaIndiceBatchGabinete--> 
            let resp1 = await this.asmxClient
                .use(this.settings.NombreServicio)
                .call(this.settings.NameAsmxUpdaTeIndice, { Parameter: CDParamenterGabinete });

            // Verificar si hay un error en la respuesta
            if (resp1.error) {
                return `${resp1.message} ${resp1.status}`;
            }

            // Verificar si la respuesta es "YES"
            if (resp1.message !== "YES") {
                return resp1.message;
            }

            // Incrementar el contador de progreso
            this.ContadorProgress++;

            // Obtener los datos actualizados
            let DataUpdateIndex = resp1.data;

            // Llamar a la función _ActualizaTableDocumentos
            let Result = await this._ActualizaTableDocumentos(DataUpdateIndex, CDParamenterGabinete, uncheckRow);
            if (Result !== "YES") {
                return Result;  // Si hay algún problema, retorna el mensaje de error
            }

            // Verificar si se ha alcanzado el contador de progreso
            if (this.ContadorProgress === this.LengProgress) {  // Usar === para comparación
                // Ocultar el modal cuando se haya completado el progreso
                Result = await this._ToggleModal(this.settings.NameModalIndice, "hide");
                if (Result !== "YES") {
                    return Result;
                }
            }

            return "YES";  // Todo ha ido bien, devolver "YES"
        } catch (ex) {
            return "Inconsistencia funcion _ActualizaIndiceBatch " + ex.message;
        }
    };

    /**
     * Función que actualiza las tablas de consulta
     * @param {any} DataUpdateIndex
     * @param {any} CDParamenterGabinete
     * @param {any} uncheckRow
     */
    _ActualizaTableDocumentos = async (DataUpdateIndex, CDParamenterGabinete, uncheckRow = true) => {
        try {
            let Result = "";
            switch (this.settings.NameModulo) {
                case "DOCUARCHI":
                    if (this.settings.TipoTable == "Boot") {
                        let IdImgage = CDParamenterGabinete[0].IdImagen;
                        Result = await updateRecordById(this.settings.NameTable, IdImgage, DataUpdateIndex[0].CamposUpdateIndiceBach, uncheckRow);
                        if (Result !== "YES") { return Result; }
                    }
                    break;
            }
            return "YES";
        } catch (ex) {
            _ActualizaTableDocumentos = "Inconsistencia funcion _ActualizaTableDocumentos " + ex.message;
        }
    }
    /**
     * Funcion que elimina las alertas del formlario agurpadas por la clase
     * @param {any} NameClass
     */
    _ElementDangerAlertClearClass = async (NameClass) => {
        for (const element of document.getElementsByClassName(NameClass)) {
            element.style.background = "";
            let SpanDanger = document.querySelector('span[data-asp-danger="' + element.id + '"]');
            if (SpanDanger) { SpanDanger.textContent = "" };
            element.classList.remove("control-person-alert");
        }
    }
    _ElementTextClear = async (NameClass) => {
        for (const element of document.getElementsByClassName(NameClass)) {
            element.value = "";
        }
    }
    /**
     * Funcion que aplica el control danger de los campos
     * @param {any} ElementId
     * @param {any} TextDanger
     */
    _ElementDangerControl = (ElementId, TextDanger) => {
        let SpanDanger = document.querySelector('span[data-asp-danger="' + ElementId + '"]');
        if (SpanDanger) {
            SpanDanger.textContent = TextDanger;
        }
        let HtmlInput = document.getElementById(ElementId);
        if (HtmlInput) { HtmlInput.style.background = "#f7d2d2" };
    }
    /**
     * Función que cierra o abre un control modal
     * @param {any} modalId
     * @param {any} action
     */
    _ToggleModal = async (modalId, action = "show") => {
        try {
            let modalElement = document.getElementById(modalId);

            if (!modalElement) return "Modal " + modalId + " no encontrado";

            // Normalizar acción (solo acepta show/hide)
            action = action.toLowerCase();
            if (action !== "show" && action !== "hide") {
                return "Acción inválida: use 'show' o 'hide'";
            }

            // Bootstrap 4 (con jQuery)
            if (typeof $ !== "undefined" && typeof $.fn.modal === "function") {
                $('#' + modalId).modal(action);
                return "YES";
            }

            // Bootstrap 5 (sin jQuery)
            if (typeof bootstrap !== "undefined" && bootstrap.Modal) {
                let modalInstance = bootstrap.Modal.getInstance(modalElement);
                if (!modalInstance) modalInstance = new bootstrap.Modal(modalElement);

                action === "show" ? modalInstance.show() : modalInstance.hide();
                return "YES";
            }

            console.error("No se detectó ni Bootstrap 4 ni Bootstrap 5 correctamente cargado.");
            return "No se detectó ni Bootstrap 4 ni Bootstrap 5 correctamente cargado.";
        } catch (ex) {
            return "Error función _ToggleModal " + ex.message;
        }
    }
    /**
     * Valida los controles input tipo fecha cuando pierden el foco
     * @param {any} e
     */
    _ValidateFechaFocusIndiceBatch(param) {
        try {
            let control;

            // Si el parámetro es un evento, obtener el control desde e.currentTarget
            if (param instanceof Event) {
                control = param.currentTarget;
            } else if (typeof param === 'string') {
                // Si el parámetro es un ID, buscar el control por su ID
                control = document.getElementById(param);
            }

            if (!control) {
                console.error('No se encontró el control');
                return 'No se encontró el control' + control.id;
            }

            let value = control.value;
            let SpanDanger = document.querySelector(`span[data-asp-danger="${control.id}"]`);

            if (!SpanDanger) {
                console.error(`No se encontró el span para el control con el ID: ${control.id}`);
                return `No se encontró el span para el control con el ID: ${control.id}`;
            }

            if (value == "") {
                control.style.background = "";
                SpanDanger.textContent = "";
                return "YES";
            }

            const Actual = new Date().getFullYear();
            let year = value.substring(0, 4);
            let month = value.substring(5, 7);
            let day = value.substring(8, 10);

            SpanDanger.textContent = "";

            if (Actual < year) {
                control.focus();
                control.setSelectionRange(0, 4);
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "El año ingresado no es válido.";
                return "NO";
            }
            if (month != "" && (month > 12 || month < 1)) {
                control.focus();
                control.setSelectionRange(5, 7);
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "El mes ingresado no es válido.";
                return "NO";
            }
            if (day != "" && (day > 31 || day < 1)) {
                control.focus();
                control.setSelectionRange(8, 10);
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "El día ingresado no es válido.";
                return "NO";
            }
            let Fecha = new Date(year, month - 1, day);
            if (Fecha.getDate() !== Number(day)) {
                control.focus();
                control.setSelectionRange(8, 10);
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "El día ingresado no es válido.";
                return "NO";
            }
            let leng = value.length;
            if (leng != 10) {
                control.focus();
                control.setSelectionRange(0, leng);
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "Tamaño de fecha no válido.";
                return "NO";
            }
            control.style.background = "";
            return "YES";
        } catch (ex) {
            alert("Error en _ValidateFechaFocusIndiceBatch: " + ex.message);
        }
    }

    /**
     * Formatea el valor de la fecha cundo se presiona 
     * la tecla del input fecha yyyy-mm-dd
     * @param {any} e
     */
    _ValidateKeyPrresFechaFormControlIndiceBatch(e) {
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
            alert(err.message + " funcion _ValidateFechaFormControlIndiceBatch " + err.message);
        }
    }
    /**
     * 
     * @param {any} e
     */
    _ValidatekeyPressNumeroFormControlIndiceBatch(e) {
        try {
        let tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 32) {
            return false;
        }
        let patron = /^[0-9]$/;
        let te = String.fromCharCode(tecla);
        let res = patron.test(te);
        if (res) {
            return patron.test(te);
        } else {
            e.preventDefault();
            return false;
        }
    } catch (err) {
        alert(err.message + " funcion _ValidatekeyPressNumeroFormControlIndiceBatch " + err.message);
    }
    }
    _ValidateNumeroFormControlIndiceBatch(param) {
        try {
            let control;
            // Si el parámetro es un evento, se obtiene el control de e.currentTarget
            if (param instanceof Event) {
                control = param.currentTarget;
            } else if (typeof param === 'string') {
                // Si el parámetro es un identificador de control, lo buscamos por ID
                control = document.getElementById(param);
            }

            if (!control) {
                //console.error('No se encontró el control');
                return 'No se encontró el control ' + control.id;
            }

            let value_numero = control.value;
            let SpanDanger = document.querySelector(`span[data-asp-danger="${control.id}"]`);
            if (!SpanDanger) {
                //console.error(`No se encontró el span para el control con el ID: ${control.id}`);
                return `No se encontró el span para el control con el ID: ${control.id}`;
            }

            // Si el campo está vacío, reiniciar el estilo y mensaje de error
            if (value_numero === "") {
                control.style.background = "";
                SpanDanger.textContent = "";
                return "YES";
            }
            let i = control.value.search(/\D/); // busca primer NO dígito
            if (i !== -1) {
                control.focus();
                control.setSelectionRange(i, i + 1);
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "Solo se permiten números.";
                return "NO";
            }
            return "YES";
        } catch (err) {
            alert(err.message + " funcion _ValidatekeyPressNumeroFormControlIndiceBatch " + err.message);
        }
    }
    _validateEmailFormFocusControlIndiceBatch(param) {
        try {
            let control;
            // Si el parámetro es un evento, se obtiene el control de e.currentTarget
            if (param instanceof Event) {
                control = param.currentTarget;
            } else if (typeof param === 'string') {
                // Si el parámetro es un identificador de control, lo buscamos por ID
                control = document.getElementById(param);
            }

            if (!control) {
                //console.error('No se encontró el control');
                return 'No se encontró el control ' + control.id;
            }

            let value_correo = control.value;
            let SpanDanger = document.querySelector(`span[data-asp-danger="${control.id}"]`);
            if (!SpanDanger) {
                //console.error(`No se encontró el span para el control con el ID: ${control.id}`);
                return `No se encontró el span para el control con el ID: ${control.id}`;
            }

            // Si el campo está vacío, reiniciar el estilo y mensaje de error
            if (value_correo === "") {
                control.style.background = "";
                SpanDanger.textContent = "";
                return "YES";
            }

            // Valida que se incluya el (@) en la dirección de correo electrónico
            if (value_correo.indexOf("@") === -1) {
                control.focus();
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "El email no es válido, falta el separador (@)";
                return "YES";
            }

            // Valida que el nombre del servidor de dominio no esté vacío
            let spl = value_correo.split("@");
            let value_ = spl[spl.length - 1];
            if (!value_ || value_ === "@") {
                control.focus();
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "El email no es válido, falta el servidor dominio (@gmail, @hotmail, @...)";
                return "YES";
            }

            // Valida que se incluya el (.) en la dirección de correo electrónico
            let value_punto = value_correo.indexOf(".");
            if (value_punto === -1 || value_punto === 0) {
                control.focus();
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "El email no es válido, falta el separador del dominio (.)";
                return "YES";
            }

            // Valida que el nombre del dominio esté presente (.com, .co, etc.)
            spl = value_correo.split(".");
            value_ = spl[spl.length - 1];
            if (!value_ || value_ === ".") {
                control.focus();
                control.style.background = "#f7d2d2";
                SpanDanger.textContent = "El email no es válido, falta el dominio (.com, .co, etc.)";
                return "YES";
            }
            // Si todo es válido
            return "YES";
        } catch (ex) {
            //console.error(`Error: ${ex.message}`);
            return "Error " + ex.message;
        }
    }
    /**Solicita la estructrura de datos de los capos y aplica las restricciones */
    _SolicitaDatosControlIndiceBatch = async () => {
        try {
            let NameEspaceControl = this.settings.NameEspaceControl;
            let atrib_campo_aleas = "";
            let atrib_campo_obliga = "";
            let atrib_name_campo = "";
            let atrib_campo_v = "";
            let atrib_campo_nl = "";
            let atrib_campo_tip = "";
            let valor_campo = "";
            let texto_campo = "";
            let atrib_campo_id = "0";
            let atrib_campo_t = "";
            let atrib_campo_tbl = "";
            let atrib_control_tip_correo = "";
            let atrib_value_campo_old = "";
            let atrib_name_campo_id = "";
            let atrib_drow_name_control_id = "";
            let atrib_Tom_alow = "";
            let value_campo_beetwen = "";
            let atrib_campo_beetwen = "0";
            //Asigna datos del campo de un formulario
            let validate_chek_one = 0;
            let ControlsElement = document.getElementsByClassName(NameEspaceControl);
            if (ControlsElement.length == 0) {
                return "No hay controles disponibles para el espacio de nombres (" + NameEspaceControl + ")";
            }
            this.ArregloEstructuraControlesAsignados = new Array();
            await this._ElementDangerAlertClearClass(NameEspaceControl);
            for (var i = 0; i < ControlsElement.length; i++) {
                if (ControlsElement[i].tagName == "TEXTAREA" || ControlsElement[i].tagName == "INPUT" || ControlsElement[i].tagName == "SELECT") {
                    atrib_campo_aleas = ControlsElement[i].attributes["atrib_aleas_c"].value;
                    atrib_campo_obliga = ControlsElement[i].attributes["atrib_campo_o"].value;
                    atrib_name_campo = ControlsElement[i].attributes["atrib_campo_n"].value;
                    atrib_campo_v = ControlsElement[i].attributes["atrib_campo_v"].value;
                    atrib_campo_nl = ControlsElement[i].attributes["atrib_campo_nl"].value;
                    atrib_campo_tip = ControlsElement[i].attributes["atrib_campo_tip"].value;
                    atrib_campo_id = ControlsElement[i].attributes["atrib_campo_id"].value;
                    atrib_campo_t = ControlsElement[i].attributes["atrib_campo_t"].value;
                    atrib_campo_tbl = ControlsElement[i].attributes["atrib_campo_tbl"].value;
                    atrib_control_tip_correo = ControlsElement[i].attributes["atrib_control_tip_correo"].value;
                    atrib_name_campo_id = ControlsElement[i].attributes["atrib_name_campo_id"].value;
                    atrib_drow_name_control_id = ControlsElement[i].attributes["atrib_drow_name_control_id"].value;
                    atrib_Tom_alow = ControlsElement[i].attributes["atrib_tom_alow"].value;
                    if (ControlsElement[i].tagName == "INPUT" && atrib_Tom_alow == "null") {
                        valor_campo = ControlsElement[i].value;
                        texto_campo = ControlsElement[i].value;
                    }
                    if (ControlsElement[i].tagName == "TEXTAREA") {
                        valor_campo = ControlsElement[i].value;
                        texto_campo = ControlsElement[i].value;
                    }
                    if (ControlsElement[i].tagName == "SELECT") {
                        texto_campo = ControlsElement[i].options[ControlsElement[i].selectedIndex].text;
                        valor_campo = ControlsElement[i].options[ControlsElement[i].selectedIndex].value;
                    }
                    //--Valida campos tipo text
                    if (atrib_campo_obliga == "1" && (valor_campo == "" || valor_campo == "0" || valor_campo == "-1")) {
                        this._ElementDangerControl(ControlsElement[i].id, "El campo (" + atrib_campo_aleas + ") es obligatorio");
                        return "NO";
                    }
                    //Valida campos tipo correo
                    if (atrib_campo_tip == "cor" && valor_campo != "") {
                        let res = this._validateEmailFormFocusControlIndiceBatch(ControlsElement[i].id);
                        if (res !== "YES") {
                            return res;
                        }
                    }
                    //Valida campos tipo correo
                    if (atrib_control_tip_correo == 1 && valor_campo != "") {
                        let res = this._validateEmailFormFocusControlIndiceBatch(ControlsElement[i].id);
                        if (res !== "YES") {
                            return res;
                        }
                    }
                    //Valida campos tipo fecha
                    if (atrib_campo_t == "DATE" && valor_campo != "") {
                        let res = this._ValidateFechaFocusIndiceBatch(ControlsElement[i].id);
                        if (res !== "YES") {
                            return res;
                        }
                    }
                    //Valida campos numericos 
                    if (atrib_campo_t == "INT" && valor_campo != "") {
                        let res = this._ValidateNumeroFormControlIndiceBatch(ControlsElement[i].id);
                        if (res !== "YES") {
                            return res;
                        }
                    }
                    let chek_itent = 0;
                    let name_chek = "chek_item_" + ControlsElement[i].id;
                    if (document.getElementById(name_chek)) {
                        let htmlImPutChec = document.getElementById(name_chek)
                        if (htmlImPutChec.checked == true) {
                            chek_itent = 1;
                            validate_chek_one = 1;
                        }
                    }
                    this.ArregloEstructuraControlesAsignados.push({
                        value_campo: valor_campo.replace("'", ""), tipo_control: ControlsElement[i].tagName, name_campo: atrib_name_campo,
                        texto_campo: texto_campo.replace("'", ""), obligatorio_campo: atrib_campo_obliga, name_space_campo: NameEspaceControl,
                        max_leng_campo: ControlsElement[i].maxLength, aleas_campo: atrib_campo_aleas, alow_tipo_value: atrib_campo_v, alow_null: atrib_campo_nl,
                        campo_tip: atrib_campo_tip, dms_id_registro: atrib_campo_id, tipo_campo: atrib_campo_t, tbl_control: atrib_campo_tbl, atrib_chek: chek_itent,
                        value_campo_old: atrib_value_campo_old, name_campo_id: atrib_name_campo_id, drow_name_control_id: atrib_drow_name_control_id,
                        control_tip_correo: atrib_control_tip_correo
                    });
                }
            }
            if (this.settings.AddCheckControl == "1" && validate_chek_one == 0) {
                return "Debe seleccionar al menos una casilla para actualizar.";
            }
            return "YES";
        } catch (ex) {
            return "Error _SolicitaDatosControlIndiceBatch : " + ex.message;
        }
    }
}
