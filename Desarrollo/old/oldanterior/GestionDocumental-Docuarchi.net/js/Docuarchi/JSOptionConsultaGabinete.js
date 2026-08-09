class JSPopupBusquedaGabinete {
    constructor(options = {}) {
        let defaults = {
            TitlePopup: "",
            MensajepPoput: "",
            NameContenPopup: "",
            NameControlPadre: "",
            NameClassControlGroup: "",
            OptionAsginaValorCampos: 0,
            NameEvent: "",
            NameModalPopup: "modal_show_search_popup_gabinete",
            NombreServcioAsmx: "ServiceListaInterfaceBusquedaGabinete",            ///-----Representa el nombre del servicio asmx
            NombreServicio: "Docuarchi",                                           ///-----Representa el nombre del servicio 
            IdmagenGabinete: "0",                                                  ///-----Representa la identificación de la imagen
            NameServiceAutoComplete: "Docuarchi",                                  ///-----Representa el nombre del servicio que contiene el asmx
            NameAsmx: "ServiceSolicitaAutoCompleteCampoGabinete",                  ///-----Representa el nombre del servicio de auto complete asmx
            NombreGabineteConsulta: "",
            IdGabineteConsulta: 0
        }
        this.settings = $.extend(true, defaults, options);
        this.SpinnerManager = new SpinnerManager();
        this.asmxClient = new ASMXClient(AsmxServicesConfig);
        this._BtnoCancel;
        this._BtnoAceptar;
        this.estado_control = -1;
        this.NameParentPopup = "NamePopupAuxConta";
        this.ResultPopup = ({ EstadoErrorPopup: "", ReSulTPopup: "" });
        this.NamePopup = "";
        this.ArregloEstructuraControles = [];
        this.ArregloEstructuraControlesAsignados = [];
        //this._DatepickerHandler = new DatepickerHandler();

    }
    async LoadJSPopupBusquedaGabinete() {
        try {
            let result = "";
            const CDParamenterGabinete = [{
                IdGabinete: this.settings.IdGabineteConsulta,
                NombreGabinete: this.settings.NombreGabineteConsulta,
                IdImagen: this.settings.IdmagenGabinete,
                NameEspaceControl: this.settings.NameEspaceControl
            }];

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
            //result = await this._ModalShowpBusqueda();
            //if (result != "YES") {
            //return result;
            //}
            result = await this._AddCotntrolSearch();
            if (result != "YES") { return result };
            return "YES";

        } catch (ex) {
            //this._ModalHidePopup();
            return ex.mensaje;
            //this.ResultPopup.EstadoErrorPopup = ex.mensaje;
            //return this.ResultPopup;
        }
    }

    async _ModalShowpBusqueda() {
        try {
            if (document.getElementById(this.NameParentPopup)) {
                let element = document.getElementById(this.NameParentPopup);
                element.remove();
            }
            const wrapper = document.createElement('div');
            wrapper.id = this.NameParentPopup;
            wrapper.innerHTML = [
                '<div class="modal fade modal_opacity " style="z-index:100063" id="' + this.settings.NameModalPopup + '" role="dialog" aria-hidden="false" data-backdrop="false">',
                '<div class="modal-dialog modal-dialog-centered">',
                '<div class="modal-content">',
                '<div class="modal-header">',
                '<h5 class="modal-title d-none" id="staticBackdropLabelCancelPopup">' + this.settings.TitlePopup + '</h5>',
                '<button type="button" class="close d-none" data-dismiss="modal" aria-label="Close">',
                '<span aria-hidden="true">&times;</span>',
                '</button>',
                '</div>',
                '<div class="modal-body">',
                '<div class="h6"> ' + this.settings.MensajepPoput + ' </div>',
                '<div class="h6">  </div>',
                '</div>',
                '<div class="modal-footer d-flex justify-content-end">',
                '<button type="button" id="butoon_cancel_popup_sear_gabinete" class="btn btn-secondary">Cancelar</button>',
                '<button type="button" id="butoon_aceptar_popup_001" class="btn btn-primary" data-dismiss="modal">Aceptar</button>',
                '</div>',
                '</div>',
                '</div>',
                '</div>'
            ].join('')
            let content = document.getElementById(this.settings.NameContenPopup);
            if (content) {
                content.append(wrapper);
            }
            this._BtnoCancel = document.getElementById("butoon_cancel_popup_sear_gabinete");
            this._BtnoCancel.addEventListener("click", this._CancelPopup, false);
            this._BtnoAceptar = document.getElementById("butoon_aceptar_popup_sear_gabinete");
            this._BtnoAceptar.addEventListener("click", this._ContinuarPopup, true);
            this.estado_control = 0; //Pone en estado de espera de la respuesta
            this.NamePopup = $("#" + this.settings.NameModalPopup);
            ///this.NamePopup.modal("show");
            return "YES";
        } catch (ex) {
            return ex.mensaje;
        }
    }
    async _AddCotntrolSearch() {
        try {
            let NameControlPadre = document.getElementById(this.settings.NameControlPadre);
            if (NameControlPadre == null) {
                return "Imposible encontrar el control padre de la interface " + NameControlPadre;
            }
            //limpia los campos anteriores
            while (NameControlPadre.hasChildNodes()) {
                NameControlPadre.removeChild(NameControlPadre.firstChild);
            }
            //Crea el div de alerta
            let divtml = document.createElement("div");
            divtml.classList.add("w-100");
            divtml.classList.add("row");
            divtml.classList.add("p-1");
            divtml.classList.add("alert-danger-alert");
            divtml.classList.add("text-center-alert");
            divtml.classList.add("hide-alert");
            divtml.classList.add("alert-alert");
            divtml.classList.add(this.settings.NameClassControlGroup + "error_alert");
            NameControlPadre.appendChild(divtml);
            for (let i = 0; i < this.ArregloEstructuraControles.length; i++) {
                const control = this.ArregloEstructuraControles[i];
                /*Agrega el div row */
                divtml = document.createElement("div");
                divtml.classList.add("row");

                /*Agrega el div cell de los controles col-md-4 mb-3 */
                let HtmlDivClascell = document.createElement("div");
                HtmlDivClascell.classList.add("col-md-12");
                HtmlDivClascell.classList.add("mb-3");
                divtml.appendChild(HtmlDivClascell);

                /*Agrega el campo label  _${control.name_campo}_${this.settings.NameEspaceControl}`;*/
                let HtmlLabel = document.createElement("label");
                HtmlLabel.setAttribute("for", `_${control.name_campo}_${this.settings.NameClassControlGroup}`)
                let string_ = control.aleas_campo;
                let textoCapitalizado = string_.toLowerCase().split(" ").map(function (palabra) {
                    return palabra.charAt(0).toUpperCase() + palabra.slice(1);
                }).join(" ");
                HtmlLabel.innerHTML = textoCapitalizado;
                HtmlDivClascell.appendChild(HtmlLabel);
                let imputhml = null;
                let imputhml_ = null;
                let SpanHtml;
                let SpanHtml_;
                let iHtml;
                let HtmlDivFlex;
                let HtmlDivAgrupa;
                let HtmlDivinputGroup;
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
                //Agrega atribute campos          
                switch (control.tipo_campo) {
                    case "DATE":
                        /*Agrega el div cell de los controles d-flex */
                        HtmlDivFlex = document.createElement("div");
                        HtmlDivFlex.classList.add("d-flex");
                        HtmlDivFlex.classList.add("search-container");
                        HtmlDivinputGroup = document.createElement("div");
                        HtmlDivinputGroup.classList.add("input-group");
                        /*Ageregamos el primer input date*/
                        imputhml.placeholder = "yyyy-mm-dd";
                        imputhml.classList.add("mr-2");
                        imputhml.addEventListener("keypress", _ValidateKeyPrresFechaFormControl);
                        imputhml.addEventListener("blur", _ValidateFechaFocusFormControl);
                        imputhml.setAttribute("max-length", "10");
                        imputhml.setAttribute("type", "text");
                        imputhml.classList.add("form-control");
                        imputhml.classList.add("date-input");
                        imputhml.id = control.name_campo + "_" + this.settings.NameClassControlGroup;
                        HtmlDivinputGroup.appendChild(imputhml);
                        SpanHtml_ = document.createElement("SPAN");
                        SpanHtml_.classList.add("w-auto");
                        SpanHtml_.classList.add("text-danger");
                        SpanHtml_.setAttribute("data-asp-danger", imputhml.id);
                        HtmlDivinputGroup.appendChild(SpanHtml_);
                        /*Agega div agrupa*/
                        // HtmlDivAgrupa = document.createElement("div");
                        //HtmlDivAgrupa.classList.add("input-group-append");
                        /*Agrega span calendar agrupa*/
                        /*SpanHtml_ = document.createElement("SPAN");
                        SpanHtml_.classList.add("input-group-text");
                        SpanHtml_.classList.add("calendar-icon");
                        HtmlDivAgrupa.appendChild(SpanHtml_);
                        /*Agrega i calendar*/
                        /*iHtml = document.createElement("i");
                        iHtml.classList.add("fas");
                        iHtml.classList.add("fa-calendar");
                        SpanHtml_.appendChild(iHtml);
                        //HtmlDivAgrupa.appendChild(SpanHtml_);
                        HtmlDivinputGroup.appendChild(HtmlDivAgrupa);*/
                        HtmlDivFlex.appendChild(HtmlDivinputGroup);
                        /*Ageregamos el span*/
                        SpanHtml = document.createElement("span");
                        SpanHtml.innerHTML = "a";
                        SpanHtml.classList.add("mx-2");
                        HtmlDivFlex.appendChild(SpanHtml);

                        /*Ageregamos el segundo input date*/
                        HtmlDivinputGroup = document.createElement("div");
                        HtmlDivinputGroup.classList.add("input-group");
                        imputhml_ = imputhml.cloneNode();
                        imputhml_.id = control.name_campo + "_" + this.settings.NameClassControlGroup + "_par";
                        imputhml_.setAttribute("atrib_campo_beetwen", 1);
                        imputhml_.addEventListener("keypress", _ValidateKeyPrresFechaFormControl);
                        imputhml_.addEventListener("blur", _ValidateFechaFocusFormControl);
                        HtmlDivinputGroup.appendChild(imputhml_);
                        /*Agerega span de error*/
                        SpanHtml_ = document.createElement("SPAN");
                        SpanHtml_.classList.add("w-auto");
                        SpanHtml_.classList.add("text-danger");
                        SpanHtml_.setAttribute("data-asp-danger", imputhml_.id);
                        HtmlDivinputGroup.appendChild(SpanHtml_);
                        /*Agega div agrupa*/
                        //HtmlDivAgrupa = document.createElement("div");
                        //HtmlDivAgrupa.classList.add("input-group-append");
                        /*Agrega span calendar agrupa*/
                        /*SpanHtml_ = document.createElement("SPAN");
                        SpanHtml_.classList.add("input-group-text");
                        HtmlDivAgrupa.appendChild(SpanHtml_);
                        /*Agrega i calendar*/
                        /*iHtml = document.createElement("i");
                        iHtml.classList.add("fas");
                        iHtml.classList.add("fa-calendar");
                        SpanHtml_.appendChild(iHtml);
                        //HtmlDivAgrupa.appendChild(SpanHtml_);
                        HtmlDivinputGroup.appendChild(HtmlDivAgrupa);*/
                        HtmlDivFlex.appendChild(HtmlDivinputGroup);
                        HtmlDivClascell.appendChild(HtmlDivFlex);  //Agrega la celda d-flex  a la celda col-md-4 mb-3
                        break;
                    case "INT":
                        /*Agrega el div cell de los controles d-flex */
                        HtmlDivFlex = document.createElement("div");
                        HtmlDivFlex.classList.add("d-flex");
                        /*Ageregamos el primer input int*/
                        imputhml.placeholder = "Desde";
                        imputhml.classList.add("mr-2");
                        imputhml.addEventListener("keypress", this._ValidatekeyPressNumeroFormControl);
                        imputhml.setAttribute("max-length", control.max_leng_campo);
                        imputhml.setAttribute("type", "number");
                        imputhml.classList.add("form-control");
                        imputhml.id = control.name_campo + "_" + this.settings.NameClassControlGroup;
                        HtmlDivFlex.appendChild(imputhml);
                        SpanHtml_ = document.createElement("SPAN");
                        SpanHtml_.classList.add("w-auto");
                        SpanHtml_.classList.add("text-danger");
                        SpanHtml_.setAttribute("data-asp-danger", imputhml.id);
                        HtmlDivFlex.appendChild(SpanHtml_);
                        /*Ageregamos el span*/
                        SpanHtml = document.createElement("span");
                        SpanHtml.innerHTML = "a";
                        SpanHtml.classList.add("mx-2");
                        HtmlDivFlex.appendChild(SpanHtml);
                        /*Ageregamos el segundo input numerico*/
                        imputhml_ = imputhml.cloneNode();
                        imputhml_.addEventListener("keypress", this._ValidatekeyPressNumeroFormControl);
                        imputhml_.id = control.name_campo + "_" + this.settings.NameClassControlGroup + "_par";
                        imputhml_.setAttribute("atrib_campo_beetwen", 1);
                        imputhml_.placeholder = "Hasta";
                        HtmlDivFlex.appendChild(imputhml_);
                        SpanHtml_ = document.createElement("SPAN");
                        SpanHtml_.classList.add("w-auto");
                        SpanHtml_.classList.add("text-danger");
                        SpanHtml_.setAttribute("data-asp-danger", imputhml_.id);
                        HtmlDivFlex.appendChild(SpanHtml_);
                        HtmlDivClascell.appendChild(HtmlDivFlex);  //Agrega la celda d-flex  a la celda col-md-4 mb-3
                        break;
                    default:
                        if (control.campo_tip == 1 || control.campo_tip == 2) {
                            /*Ageregamos el text*/
                            imputhml.placeholder = "Buscar";
                            imputhml.classList.add("mr-2");
                            imputhml.setAttribute("max-length", control.max_leng_campo);
                            imputhml.setAttribute("type", "text");
                            imputhml.classList.add("form-control");
                            imputhml.id = control.name_campo + "_" + this.settings.NameClassControlGroup;
                            HtmlDivClascell.appendChild(imputhml);
                            SpanHtml_ = document.createElement("SPAN");
                            SpanHtml_.classList.add("w-auto");
                            SpanHtml_.classList.add("text-danger");
                            SpanHtml_.setAttribute("data-asp-danger", imputhml.id);
                            HtmlDivClascell.appendChild(SpanHtml_);
                        }
                        if (control.campo_tip == 0) {
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
                            HtmlDivClascell.appendChild(imputhml);
                            SpanHtml_ = document.createElement("SPAN");
                            SpanHtml_.classList.add("w-auto");
                            SpanHtml_.classList.add("text-danger");
                            SpanHtml_.setAttribute("data-asp-danger", imputhml.id);
                            HtmlDivClascell.appendChild(SpanHtml_);
                        }
                }
                //Valida campos tipo correo electrónico//
                if (control.control_tip_correo == 1) {
                    imputhml.addEventListener("blur", _validateEmailFormFocusControl);
                }
                // Asignación de atributos adicionales   
                if (imputhml !== null) {
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
                    imputhml.setAttribute("atrib_campo_id", imputhml.id);
                    imputhml.classList.add(this.settings.NameClassControlGroup);
                }
                /**Agrega los atirbutos adicionales si exite el segundo control*/
                if (imputhml_ !== null) {
                    imputhml_.setAttribute("atrib_aleas_c", control.aleas_campo);
                    imputhml_.setAttribute("atrib_campo_O", control.obligatorio_campo);
                    imputhml_.setAttribute("atrib_campo_n", control.name_campo);
                    imputhml_.setAttribute("atrib_campo_v", control.alow_tipo_value);
                    imputhml_.setAttribute("atrib_campo_tip", control.campo_tip);
                    imputhml_.setAttribute("atrib_campo_nl", control.alow_null);
                    imputhml_.setAttribute("atrib_campo_id", control.dms_id_registro);
                    imputhml_.setAttribute("atrib_name_campo_id", control.name_campo_id);
                    imputhml_.setAttribute("atrib_campo_t", control.tipo_campo);
                    imputhml_.setAttribute("atrib_campo_tbl", control.tbl_control);
                    imputhml_.setAttribute("atrib_campo_drow_destino", control.drow_name_controls_destino);
                    imputhml_.setAttribute("atrib_name_espace_control", control.name_space_campo);
                    imputhml_.setAttribute("atrib_control_tip_correo", control.control_tip_correo);
                    imputhml_.setAttribute("atrib_value_campo_old", control.value_campo_old);
                    imputhml_.setAttribute("atrib_drow_name_control_id", control.drow_name_control_id);
                    imputhml_.setAttribute("atrib_Tom_alow", control.Tom_alow);
                    imputhml_.setAttribute("atrib_campo_id", imputhml_.id);
                    imputhml_.classList.add(this.settings.NameClassControlGroup + "_parlet");
                }
                
                NameControlPadre.appendChild(divtml);
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
            console.log(ex);
            return "Inconsistecia general funcion " + ex.message;
        }
    }
    /**Solicita la estructrura de datos de los capos y aplica las restricciones */
   async _SolicitaDatosConsulta  () {
        try {
            let NameEspaceControl = this.settings.NameClassControlGroup;
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
            await this._ElementDangerAlertClearClass();

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
                        let res = _validateEmailFormFocusControl(ControlsElement[i].id);
                        if (res !== "YES") {
                            return res;
                        }
                    }
                    //Valida campos tipo correo
                    if (atrib_control_tip_correo == 1 && valor_campo != "") {
                        let res = _validateEmailFormFocusControl(ControlsElement[i].id);
                        if (res !== "YES") {
                            return res;
                        }
                    }
                    //Valida campos tipo fecha
                    if (atrib_campo_t == "DATE" && valor_campo != "") {
                        let res = _ValidateFechaFocusFormControl(ControlsElement[i].id);
                        if (res !== "YES") {
                            return res;
                        }
                    }
                    //Valida campos numericos 
                    if (atrib_campo_t == "INT" && valor_campo != "") {
                        let res = _ValidateNumeroFormControl(ControlsElement[i].id);
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
                    //----------Asigna el valor del beetwen al campo padre-----------///
                    let HtmpCampoPar = document.getElementById(atrib_campo_id + "_par");
                    if (HtmpCampoPar) {
                        value_campo_beetwen = HtmpCampoPar.value;
                    }
                    //--------Setea el valor del campo par si el campo padre esta lleno--------///     
                    let namePadre = atrib_name_campo_id.replace("_par", "");
                    let htmlPadre = document.getElementById(namePadre);
                    if (atrib_campo_beetwen == 1 && htmlPadre?.value) {
                        valor_campo = "";
                        texto_campo = "";
                        value_campo_beetwen = "";
                    }
                    this.ArregloEstructuraControlesAsignados.push({
                        value_campo: valor_campo.replace("'", ""), tipo_control: ControlsElement[i].tagName, name_campo: atrib_name_campo,
                        texto_campo: texto_campo.replace("'", ""), obligatorio_campo: atrib_campo_obliga, name_space_campo: NameEspaceControl,
                        max_leng_campo: ControlsElement[i].maxLength, aleas_campo: atrib_campo_aleas, alow_tipo_value: atrib_campo_v, alow_null: atrib_campo_nl,
                        campo_tip: atrib_campo_tip, dms_id_registro: atrib_campo_id, tipo_campo: atrib_campo_t, tbl_control: atrib_campo_tbl, atrib_chek: chek_itent,
                        value_campo_old: atrib_value_campo_old, name_campo_id: atrib_name_campo_id, drow_name_control_id: atrib_drow_name_control_id,
                        control_tip_correo: atrib_control_tip_correo, value_campo_beetwen: value_campo_beetwen
                    });
                }
            }
            console.log(this.ArregloEstructuraControlesAsignados);
            return "YES";
        } catch (ex) {
            return "Error _SolicitaDatosControlIndiceBatch : " + ex.message;
        }
    }
   async _ConsultaOpcionesGabinete() {
        try {
            
            let Result = await this._SolicitaDatosConsulta();
            if (Result != "YES") {
                return Result;
            }
            const CDParamenterGabinete = [{ ValorConsulta: "", TipoConsulta: 1, IdGabinete: this.settings.IdGabineteConsulta, NombreGabinete: this.settings.NombreGabineteConsulta, ClassConfigGeneralService: this.ArregloEstructuraControlesAsignados }];
            let resp1 = await asmxClient
                .use("Docuarchi")
                .call("ServiceConsultaGabinete", { Parameter: CDParamenterGabinete });
            if (resp1.error) {
                return resp1.message;
            }
            let Data = resp1.data[0];
            let class_stru_row_Gabinete_Generic = JSON.parse(Data.Obj_ilist_row_generic);
            init_row_feld_table_boostrap_table("table_consulta_gabinete",
                Data.Obj_ilist_fileds_generic,
                class_stru_row_Gabinete_Generic,
                "contenido_table_boot_migracion",
                "table-bordered",
                "table-borderless",
                "multiple",
                "bt-selected",    // hex (#e7ebf6) o clase CSS
                 false,
                 false,              // quitar bordes de tr/td
                 false,           // habilitar/deshabilitar table-hover
                 true,        // bordes redondeados en seleccionados
                ["vis_doc_selecion_rad"], // SOLO estas clases disparan selección
                true);
            return "YES";
       
        } catch (ex) {
            return "Incosistencia general funcion " + ex.message;
        }
    }

    _CancelPopup(event) {
        _JSPopupConfirm.estado_control = 2;
    }
    _ContinuarPopup() {
        _JSPopupConfirm.estado_control = 1;
    }
    _ModalHidePopup() {
        if (document.getElementById(this.NameParentPopup)) {
            let element = document.getElementById(this.NameParentPopup);
            element.remove();
        }
    }
    _ElementDangerAlertClearClass = async () => {
        try {
        for (const element of document.getElementsByClassName(this.settings.NameClassControlGroup)) {
            element.style.background = "";
            let SpanDanger = document.querySelector('span[data-asp-danger="' + element.id + '"]');
            if (SpanDanger) { SpanDanger.textContent = "" };
            element.classList.remove("control-person-alert");
        }
            return "YES";
        } catch (ex) {
            return "Inconsistencia general funcion _ElementDangerAlertClearClass " + ex.mensaje;
        }
    }
    _ElementTextClear = async () => {
        try {
            for (const element of document.getElementsByClassName(this.settings.NameClassControlGroup)) {
                element.value = "";
            }
            for (const element of document.getElementsByClassName(this.settings.NameClassControlGroup + "_parlet")) {
                element.value = "";
            }
            return "YES";
        } catch (ex) {
            return "Inconsistencia general funcion _ElementTextClear " + ex.mensaje;
       }
    } 
}