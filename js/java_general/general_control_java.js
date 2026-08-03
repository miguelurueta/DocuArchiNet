/**/
const _ValidateFechaFocusFormControl = (param)=> {
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
        alert("Error en _ValidateFechaFocusFormControl: " + ex.message);
    }
}

/**
 * Formatea el valor de la fecha cundo se presiona 
 * la tecla del input fecha yyyy-mm-dd
 * @param {any} e
 */
const _ValidateKeyPrresFechaFormControl = (e) => {
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
        alert(err.message + " funcion _ValidateKeyPrresFechaFormControl " + err.message);
    }
}
const _ValidatekeyPressNumeroFormControl=(e) => {
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
        alert(err.message + " funcion _ValidatekeyPressNumeroFormControl " + err.message);
    }
}
const _ValidateNumeroFormControl=(param) => {
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
        alert(err.message + " funcion _ValidateNumeroFormControl " + err.message);
    }
}
const _validateEmailFormFocusControl=(param) => {
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
class JSPopupConfirm {
    constructor(options = {}) {
        let defaults = {
            TitlePopup: "",          
            MensajepPoput: "",
            NameContenPopup: ""
        }
        this.settings = $.extend(true, defaults, options);
        this._BtnoCancel;
        this._BtnoAceptar;
        this.estado_control = -1;
        this.NameParentPopup = "NamePopupAuxConta";
        this.ResultPopup = ({ EstadoErrorPopup: "", ReSulTPopup: "" });
        this.NamePopup = "";

    }
    async LoadJSPopupConfirm() {
        try {
            let result = "";
            result = await this._ModalShowpPopup();
            if (result != "YES") {
                this.ResultPopup.EstadoErrorPopup = result;
                return this.ResultPopup;
            }
            this.ResultPopup.ReSulTPopup = await this._WaitResponse();
            this._ModalHidePopup();
            return this.ResultPopup;

        } catch (ex) {
            this._ModalHidePopup();
            this.ResultPopup.EstadoErrorPopup = ex.mensaje;
            return this.ResultPopup;
        }
    }
    async _WaitResponse() {
        return new Promise(resolve => {
            setInterval(() => {
                if (this.estado_control == 1) {
                    resolve('YES');
                }
                if (this.estado_control == 2) {
                    resolve('NOT');
                }
            }, 10);
        });
    }
   async _ModalShowpPopup() {
        try {
            if (document.getElementById(this.NameParentPopup)) {
                let element = document.getElementById(this.NameParentPopup);
                element.remove();
            }
            const wrapper = document.createElement('div');
            wrapper.id = this.NameParentPopup;
            wrapper.innerHTML = [
                '<div class="modal fade modal_opacity " style="z-index:100063" id="modal_show_popup_001" role="dialog" aria-hidden="false" data-backdrop="false">',
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
                '<button type="button" id="butoon_cancel_popup_001" class="btn btn-secondary">Cancelar</button>',
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
            this._BtnoCancel = document.getElementById("butoon_cancel_popup_001");
            this._BtnoCancel.addEventListener("click", this._CancelPopup, false);
            this._BtnoAceptar = document.getElementById("butoon_aceptar_popup_001");
            this._BtnoAceptar.addEventListener("click", this._ContinuarPopup, true);
            this.estado_control = 0; //Pone en estado de espera de la respuesta
            this.NamePopup = $("#modal_show_popup_001");
            this.NamePopup.modal("show");
            return "YES";
        } catch (ex) {
            return ex.mensaje;
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
}
let _JSPopupConfirm;
const JSPopupConfirmInit = async (Option) => {
    _JSPopupConfirm = new JSPopupConfirm(Option);
    let result = await _JSPopupConfirm.LoadJSPopupConfirm();
    return result;
}

var ITEM_GENERAL_CONTROL_ARRAY = new Array(); //Recibe los datos recuperados del a interface
var ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array(); //Recibe los datos del web service para gestion de formulario
var ITEM_GENERAL_CONTROL_ARRAY_DIFERENT = new Array(); //Recibe los datos que se cambiaron en el formulario
let RESULT_EVENT_GENERAL = "YES";
let CONTROL_EVENT_GENERAL = "";
//add interface consulta formulario gabinete
const Create_interfaz_formulario_search_gabinet = (name_control_padre, class_name_form_control, asigna_valor, apost_name_content, add_check, NameEvent) => {
    try {
        let name_padre_control = document.getElementById(name_control_padre);
        if (name_padre_control == null) {
            return "Imposible encontrar el control padre de la interface " + name_control_padre;
        }
        //limpia los campos anteriores
        while (name_padre_control.hasChildNodes()) {
            name_padre_control.removeChild(name_padre_control.firstChild);
        }
        //Crea el div de alerta
        var divtml = document.createElement("div");
        divtml.classList.add("w-100");
        divtml.classList.add("row");
        divtml.classList.add("p-1");
        divtml.classList.add("alert-danger-alert");
        divtml.classList.add("text-center-alert");
        divtml.classList.add("hide-alert");
        divtml.classList.add("alert-alert");
        divtml.classList.add(class_name_form_control + "error_alert");
        name_padre_control.appendChild(divtml);
        for (var i = 0; i < ITEM_GENERAL_CONTROL_ARRAY_ASING.length; i++) {
            //Add campo DIV ROW
            divtml = document.createElement("div");
            divtml.classList.add("row");
            divtml.classList.add("p-1");
            name_padre_control.appendChild(divtml);
            //Add campo DIV COLUMNA
            var divtml_ = document.createElement("div");
            divtml_.classList.add("col-4");
            divtml_.classList.add("pl-1");
            //Agrega el control SPAN
            var spntml = document.createElement("span");
            spntml.classList.add("h6");
            spntml.classList.add("font-weight-light");
            let string_ = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].aleas_campo;
            spntml.innerHTML = string_;

            divtml_.appendChild(spntml);
            divtml.appendChild(divtml_);
            //Add campo DIV COLUMNA CAMPOS
            divtml_ = document.createElement("div");
            divtml_.classList.add("col-8");
            //Agrega campo IMPUT/OPTION/TEXTAREA
            var imputhml;
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 1) {
                imputhml = document.createElement("INPUT");
                if (asigna_valor == 1) {
                    imputhml.value = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo;
                }
            }
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 2) {
                imputhml = document.createElement("TEXTAREA");
                if (asigna_valor == 1) {
                    imputhml.value = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo;
                }
            }
            //Valida campo disabled o enabled
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].disable_campo == 0) {
                imputhml.disabled = true;
            }
            //valida numero maximo de caracteres   
            imputhml.maxLength = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].max_leng_campo;
            //Agrega los atributos al control
            imputhml.setAttribute("atrib_aleas_c", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].aleas_campo);
            imputhml.setAttribute("atrib_campo_O", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].obligatorio_campo);
            imputhml.setAttribute("atrib_campo_n", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo);
            imputhml.setAttribute("atrib_campo_v", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].alow_tipo_value);
            imputhml.setAttribute("atrib_campo_tip", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip);
            imputhml.setAttribute("atrib_campo_nl", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].alow_null);
            imputhml.setAttribute("atrib_campo_id", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dms_id_registro);
            imputhml.setAttribute("atrib_name_campo_id", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo_id);
            imputhml.setAttribute("atrib_campo_t", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo);
            imputhml.setAttribute("atrib_campo_tbl", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control);
            imputhml.setAttribute("atrib_campo_drow_destino", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].drow_name_controls_destino);
            imputhml.setAttribute("atrib_name_espace_control", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_space_campo);
            imputhml.setAttribute("atrib_control_tip_correo", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].control_tip_correo);
            imputhml.setAttribute("atrib_value_campo_old", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo_old);
            imputhml.setAttribute("atrib_drow_name_control_id", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].drow_name_control_id);
            imputhml.setAttribute("atrib_campo_beetwen", 0);
            imputhml.setAttribute("atrib_Tom_alow", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Tom_alow);
            imputhml.id = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control;
            imputhml.classList.add(class_name_form_control);
            divtml_.appendChild(imputhml);
            //Agrega atribute date           
            switch (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo) {
                case "DATE":
                    load_date_form_control(imputhml.id);
                    imputhml.addEventListener("keypress", validate_fecha_form_control);
                    imputhml.placeholder = "yyyy mm dd";
                    imputhml.classList.add("w-50");
                    imputhml.classList.add("form-control-person");
                    break;
                case "INT":
                    imputhml.addEventListener("keypress", validate_numero_form_control);
                    imputhml.classList.add("w-50");
                    imputhml.classList.add("form-control-person");
                    break;
                default:
                    imputhml.classList.add("w-100");
                    imputhml.classList.add("form-control");

            }
            let imputhml_;
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo == "DATE" || ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo == "INT") {
                imputhml_ = imputhml.cloneNode();
                imputhml_.id = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control + "_par";
                imputhml_.setAttribute("atrib_campo_beetwen", 1);
                //Agrega el hasta
                var spntml = document.createElement("span");
                spntml.classList.add("h7");
                spntml.innerHTML = ":"
                //divtml_.appendChild(spntml);
                divtml_.appendChild(imputhml_);
                //Agrega atribute date           
                switch (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo) {
                    case "DATE":
                        load_date_form_control(imputhml.id);
                        imputhml_.addEventListener("keypress", validate_fecha_form_control);
                        imputhml_.placeholder = "yyyy mm dd";
                        imputhml_.classList.add("w-50");
                        imputhml_.classList.add("form-control-person");
                        break;
                    case "INT":
                        imputhml_.addEventListener("keypress", validate_numero_form_control);
                        imputhml_.classList.add("w-50");
                        imputhml_.classList.add("form-control-person");
                        break;
                    default:
                        imputhml.classList.add("w-100");
                        imputhml.classList.add("form-control");
                }
            }
            divtml.appendChild(divtml_);
            //Agrega los eventos del control
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control) {
                for (let z = 0; z < ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control.length; z++) {
                    let name_event = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control[z].name_event_control;
                    let name_funtion_event = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control[z].name_function_event_control;
                    switch (name_funtion_event) {
                        case "ValidateCapitalLeter":
                            imputhml.addEventListener("focusout", ValidateCapitalLeter);
                            break;
                        case "validateLowercase":
                            imputhml.addEventListener("focusout", validateLowercase);
                            break;
                        case "validateUpperCase":
                            imputhml.addEventListener("focusout", validateUpperCase);
                            break;
                    }
                }
            }
            //Agrega los eventos al segundo control
            if (imputhml_) {
                if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control) {
                    for (let z = 0; z < ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control.length; z++) {
                        let name_event = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control[z].name_event_control;
                        let name_funtion_event = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control[z].name_function_event_control;
                        switch (name_funtion_event) {
                            case "ValidateCapitalLeter":
                                imputhml_.addEventListener("focusout", ValidateCapitalLeter);
                                break;
                            case "validateLowercase":
                                imputhml_.addEventListener("focusout", validateLowercase);
                                break;
                            case "validateUpperCase":
                                imputhml_.addEventListener("focusout", validateUpperCase);
                                break;
                        }
                    }
                }
            }
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 1 || ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 2) {
                if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control == "") {
                    service_auto_complete_form_control(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dbms_control,
                        ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo);

                } else {
                    service_auto_complete_form_control_person(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dbms_control,
                        ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control,
                        ITEM_GENERAL_CONTROL_ARRAY_ASING[i].service_control, NameEvent);
                }
                //Agrega el auto complete al segundo control

                if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo == "DATE" || ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo == "INT") {
                    if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control == "") {
                        service_auto_complete_form_control(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_par", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dbms_control,
                            ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo);

                    } else {
                        service_auto_complete_form_control_person(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control + "_par", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dbms_control,
                            ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control,
                            ITEM_GENERAL_CONTROL_ARRAY_ASING[i].service_control, NameEvent);
                    }
                }

            }
        }
        return "YES";
    } catch (ex) {
        return "Inconsistencia general fucion Create_interfaz_formulario_search_gabinet : " + ex.message;
    }
}
function valida_solicita_datos_control_general(name_espace_class) {
    try {
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
        let control_element_ = document.getElementsByClassName(name_espace_class);
        if (control_element_.length == 0) {
            return "No hay controles disponibles para el espacio de nombres (" + name_espace_class + ")";
        }
        element_alert_clear_class(name_espace_class);
        for (var i = 0; i < control_element_.length; i++) {
            //Valida no escanear los campo espoeciales (TOM)
            if (control_element_[i].tagName == "TEXTAREA" || control_element_[i].tagName == "INPUT" || control_element_[i].tagName == "SELECT") {
            atrib_campo_aleas = control_element_[i].attributes["atrib_aleas_c"].value;
            atrib_campo_obliga = control_element_[i].attributes["atrib_campo_O"].value;
            atrib_name_campo = control_element_[i].attributes["atrib_campo_n"].value;
            atrib_campo_v = control_element_[i].attributes["atrib_campo_v"].value;
            atrib_campo_nl = control_element_[i].attributes["atrib_campo_nl"].value;
            atrib_campo_tip = control_element_[i].attributes["atrib_campo_tip"].value;
            atrib_campo_id = control_element_[i].attributes["atrib_campo_id"].value;
            atrib_campo_t = control_element_[i].attributes["atrib_campo_t"].value;
            atrib_campo_tbl = control_element_[i].attributes["atrib_campo_tbl"].value;
            atrib_control_tip_correo = control_element_[i].attributes["atrib_control_tip_correo"].value;
            atrib_name_campo_id = control_element_[i].attributes["atrib_name_campo_id"].value;
            atrib_drow_name_control_id = control_element_[i].attributes["atrib_drow_name_control_id"].value;
            atrib_Tom_alow = control_element_[i].attributes["atrib_tom_alow"].value;  
            let itemTom;
            let control_TOM;
            let select_TOM;
            if (atrib_Tom_alow == "null") {
                valor_campo = control_element_[i].value;
            } else {     
                select_TOM = document.getElementById(control_element_[i].id);
                control_TOM = select_TOM.tomselect;
                itemTom = Tom_Set_item_Aray_G(control_TOM); 
                if (itemTom.length == 0) {
                    valor_campo = "";
                } else {
                    if (itemTom[0].id_value == -1) {
                        return "Error sevivcion TOM select " + itemTom[0].tex_value;
                    }
                    valor_campo = itemTom[0].tex_value;   
                }
            }
            //--Valida campos tipo text
            if (atrib_campo_obliga == "1" && (valor_campo == "" || valor_campo == "0" || valor_campo == "-1")) {       
                if (atrib_Tom_alow == "null") {
                    control_element_[i].focus();
                    element_alert(control_element_[i]);
                    return "El campo (" + atrib_campo_aleas + ") es obligatorio";
                } else {
                    control_TOM.focus();        
                    return "El campo (" + atrib_campo_aleas + ") es obligatorios";
                }
                
            }
            
            //Valida campos tipo correo
            if (atrib_campo_tip == "cor" &&  valor_campo != "") {
                let res = validate_email_form_control(valor_campo);
                if (res !== "YES") {
                    element_alert(control_element_[i]);
                    control_element_[i].focus();
                    return res;
                }
            }
            //Valida campos tipo correo
            if (atrib_control_tip_correo == 1 && valor_campo != "") {
                let res = validate_email_form_control(valor_campo);
                if (res !== "YES") {
                    element_alert(control_element_[i]);
                    control_element_[i].focus();
                    return res;
                }
            }
            //Valida campos tipo fecha
            if (atrib_campo_t == "DATE" &&  valor_campo != "") {
                let result = validate_fecha_focus(valor_campo);
                if (result !== "YES") {
                    element_alert(control_element_[i]);
                    control_element_[i].focus();
                    return result;
                }
            }
            
            //Valida campos numericos
            }
        }
        //Asigna datos del campo de un formulario
        let validate_chek = 0;
        let validate_chek_one = 0;
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        for (var i = 0; i < control_element_.length; i++) {
            //Valida no escanear los campo espoeciales (TOM)
            if (control_element_[i].tagName == "TEXTAREA" || control_element_[i].tagName == "INPUT" || control_element_[i].tagName == "SELECT") {
            atrib_campo_aleas = control_element_[i].attributes["atrib_aleas_c"].value;
            atrib_campo_obliga = control_element_[i].attributes["atrib_campo_O"].value;
            atrib_name_campo = control_element_[i].attributes["atrib_campo_n"].value;
            atrib_campo_v = control_element_[i].attributes["atrib_campo_v"].value;
            atrib_campo_nl = control_element_[i].attributes["atrib_campo_nl"].value;
            atrib_campo_tip = control_element_[i].attributes["atrib_campo_tip"].value;
            atrib_campo_id = control_element_[i].id;
            atrib_campo_t = control_element_[i].attributes["atrib_campo_t"].value;
            atrib_campo_tbl = control_element_[i].attributes["atrib_campo_tbl"].value;
            atrib_name_campo_id = control_element_[i].id;
            valor_campo = control_element_[i].value;
            atrib_value_campo_old = control_element_[i].attributes["atrib_value_campo_old"].value;
            atrib_drow_name_control_id = control_element_[i].attributes["atrib_drow_name_control_id"].value;
            atrib_control_tip_correo = control_element_[i].attributes["atrib_control_tip_correo"].value;
            atrib_campo_beetwen = control_element_[i].attributes["atrib_campo_beetwen"].value;
            atrib_Tom_alow = control_element_[i].attributes["atrib_tom_alow"].value;     
            if (control_element_[i].tagName == "INPUT" && atrib_Tom_alow == "null") {
                valor_campo = control_element_[i].value;
                texto_campo = control_element_[i].value;
            }
            if (control_element_[i].tagName == "TEXTAREA") {
                valor_campo = control_element_[i].value;
                texto_campo = control_element_[i].value;
            }
            if (control_element_[i].tagName == "SELECT") {
                texto_campo = control_element_[i].options[control_element_[i].selectedIndex].text;
                valor_campo = control_element_[i].options[control_element_[i].selectedIndex].value;
            }
            //campos tipo TOM SELECT
                let array_item_tom;
                if (atrib_Tom_alow == "1") {
                let select = document.getElementById(control_element_[i].id);
                let control = select.tomselect;
                if (control) {
                    array_item_tom = Tom_Set_item_Aray_G(control);
                    if (array_item_tom[0].id_value == -1) {
                        return "Error sevivcion TOM select " + array_item_tom[0].tex_value;
                    }
                    
                } else {
                    return "El control (" + select.id + ") no tiene relacinado campo TOM SELCT"
                }
                    valor_campo = array_item_tom[0].id_value;
                    texto_campo = array_item_tom[0].tex_value.trim();
                    texto_campo = texto_campo.replaceAll("\n", "");
                    texto_campo = texto_campo.replace("\t", "");
            }
            
            let chek_itent = 0;
            let name_chek = "chek_item_" + control_element_[i].id;
            if (document.getElementById(name_chek)) {
                validate_chek = 1;
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
            ITEM_GENERAL_CONTROL_ARRAY.push({
                value_campo: valor_campo.replace("'", ""), tipo_control: control_element_[i].tagName, name_campo: atrib_name_campo, 
                texto_campo: texto_campo.replace("'", ""), obligatorio_campo: atrib_campo_obliga, name_space_campo: name_espace_class,
                max_leng_campo: control_element_[i].maxLength, aleas_campo: atrib_campo_aleas, alow_tipo_value: atrib_campo_v, alow_null: atrib_campo_nl,
                campo_tip: atrib_campo_tip, dms_id_registro: atrib_campo_id, tipo_campo: atrib_campo_t, tbl_control: atrib_campo_tbl, atrib_chek: chek_itent,
                value_campo_old: atrib_value_campo_old, name_campo_id: atrib_name_campo_id, drow_name_control_id: atrib_drow_name_control_id,
                control_tip_correo: atrib_control_tip_correo, Item_Tom_row: array_item_tom, value_campo_beetwen: value_campo_beetwen
            });
            }
        }
        if (validate_chek == 1 && validate_chek_one == 0) {
            return "Debe chequear al menos un campo del formulario ";
        }
        return "YES";
    } catch (ex) {
        return "Error valida_solicita_datos_control_general : " + ex.message;
    }   
}
const Tom_Set_item_Aray_G = (control_tom) => {
    let Item = new Array();
    try {
       
    for (i = 0; i < control_tom.items.length; i++) {
        let get = control_tom.getItem(control_tom.items[i]);
        if (get) {
            Item.push({ id_value: get.attributes["data-value"].nodeValue, tex_value: Tom_Format_value_text(get.textContent)});
        }
    }
        return Item;
    } catch (ex) {
        
        Item.push({ id_value: -1, tex_value: ex.message });
        return Item;
    }
}
const Tom_Format_value_text = (valor_tom) => {
    if (valor_tom == "") {
        return "";
    }
    let value_format = valor_tom;
    let Caracter_Invald = value_format.slice(-1);
    //Elimina el carater x de eliminar el item
    //console.log(Caracter_Invald);
    if (Caracter_Invald == '×') {
        value_format = value_format.substr(0, value_format.length - 1);   
    } 
    value_format = value_format.trim();
    value_format = value_format.replaceAll("\n", "");
    value_format = value_format.replace("\t", "");  
    return value_format;
}
const valida_solicita_datos_control_general_async = async (name_espace_class) => {
    let myPromise = new Promise(function (resolve) {
        try {
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
            let value_campo_beetwen = "";
            let atrib_campo_beetwen = "0";
            let control_element_ = document.getElementsByClassName(name_espace_class);
            if (control_element_.length == 0) {
               return resolve( "No hay controles disponibles para el espacio de nombres (" + name_espace_class + ")");
            }
            element_alert_clear_class(name_espace_class);
            for (var i = 0; i < control_element_.length; i++) {
                atrib_campo_aleas = control_element_[i].attributes["atrib_aleas_c"].value;
                atrib_campo_obliga = control_element_[i].attributes["atrib_campo_O"].value;
                atrib_name_campo = control_element_[i].attributes["atrib_campo_n"].value;
                atrib_campo_v = control_element_[i].attributes["atrib_campo_v"].value;
                atrib_campo_nl = control_element_[i].attributes["atrib_campo_nl"].value;
                atrib_campo_tip = control_element_[i].attributes["atrib_campo_tip"].value;
                atrib_campo_id = control_element_[i].attributes["atrib_campo_id"].value;
                atrib_campo_t = control_element_[i].attributes["atrib_campo_t"].value;
                atrib_campo_tbl = control_element_[i].attributes["atrib_campo_tbl"].value;
                atrib_control_tip_correo = control_element_[i].attributes["atrib_control_tip_correo"].value;
                atrib_name_campo_id = control_element_[i].attributes["atrib_name_campo_id"].value;
                atrib_drow_name_control_id = control_element_[i].attributes["atrib_drow_name_control_id"].value;
                valor_campo = control_element_[i].value;
                if (atrib_campo_obliga == "1" && (valor_campo == "")) {
                    control_element_[i].focus();
                    element_alert(control_element_[i]);
                    return resolve( "El campo (" + atrib_campo_aleas + ") es obligatorio");
                }
                //Valida campos tipo correo
                if (atrib_campo_tip == "cor" && valor_campo != "") {
                    let res = validate_email_form_control(valor_campo);
                    if (res !== "YES") {
                        element_alert(control_element_[i]);
                        control_element_[i].focus();
                        return resolve (res);
                    }
                }
                //Valida campos tipo correo
                if (atrib_control_tip_correo == 1 && valor_campo != "") {
                    let res = validate_email_form_control(valor_campo);
                    if (res !== "YES") {
                        element_alert(control_element_[i]);
                        control_element_[i].focus();
                        return   resolve(res);
                    }
                }
                //Valida campos tipo fecha
                if (atrib_campo_t == "DATE" && valor_campo != "") {
                    let result = validate_fecha_focus(valor_campo);
                    if (result !== "YES") {
                        element_alert(control_element_[i]);
                        control_element_[i].focus();
                        return  resolve(result);
                        
                    }
                }

                //Valida campos numericos
            }
            //Asigna datos del campo de un formulario
            let validate_chek = 0;
            let validate_chek_one = 0;
            ITEM_GENERAL_CONTROL_ARRAY = new Array();
            for (var i = 0; i < control_element_.length; i++) {
                atrib_campo_aleas = control_element_[i].attributes["atrib_aleas_c"].value;
                atrib_campo_obliga = control_element_[i].attributes["atrib_campo_O"].value;
                atrib_name_campo = control_element_[i].attributes["atrib_campo_n"].value;
                atrib_campo_v = control_element_[i].attributes["atrib_campo_v"].value;
                atrib_campo_nl = control_element_[i].attributes["atrib_campo_nl"].value;
                atrib_campo_tip = control_element_[i].attributes["atrib_campo_tip"].value;
                atrib_campo_id = control_element_[i].id;
                atrib_campo_t = control_element_[i].attributes["atrib_campo_t"].value;
                atrib_campo_tbl = control_element_[i].attributes["atrib_campo_tbl"].value;
                atrib_name_campo_id = control_element_[i].id;
                valor_campo = control_element_[i].value;
                atrib_value_campo_old = control_element_[i].attributes["atrib_value_campo_old"].value;
                atrib_drow_name_control_id = control_element_[i].attributes["atrib_drow_name_control_id"].value;
                atrib_control_tip_correo = control_element_[i].attributes["atrib_control_tip_correo"].value;
                atrib_campo_beetwen = control_element_[i].attributes["atrib_campo_beetwen"].value;
                valor_campo = "";
                texto_campo = "";
                if (control_element_[i].tagName == "INPUT") {
                    valor_campo = control_element_[i].value;
                    texto_campo = control_element_[i].value;
                }
                if (control_element_[i].tagName == "TEXTAREA") {
                    valor_campo = control_element_[i].value;
                    texto_campo = control_element_[i].value;
                }
                if (control_element_[i].tagName == "SELECT") {
                    if (control_element_[i].options.length > 0) {
                        texto_campo = control_element_[i].options[control_element_[i].selectedIndex].text;
                        valor_campo = control_element_[i].options[control_element_[i].selectedIndex].value;
                    } else {
                        texto_campo = "";
                        valor_campo = "0";
                    }
                }
                let chek_itent = 0;
                let name_chek = "chek_item_" + control_element_[i].id;
                if (document.getElementById(name_chek)) {
                    validate_chek = 1;
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
                ITEM_GENERAL_CONTROL_ARRAY.push({
                    value_campo: valor_campo.replace("'", ""), tipo_control: control_element_[i].tagName, name_campo: atrib_name_campo,
                    texto_campo: texto_campo.replace("'", ""), obligatorio_campo: atrib_campo_obliga, name_space_campo: name_espace_class,
                    max_leng_campo: control_element_[i].maxLength, aleas_campo: atrib_campo_aleas, alow_tipo_value: atrib_campo_v, alow_null: atrib_campo_nl,
                    campo_tip: atrib_campo_tip, dms_id_registro: atrib_campo_id, tipo_campo: atrib_campo_t, tbl_control: atrib_campo_tbl, atrib_chek: chek_itent,
                    value_campo_old: atrib_value_campo_old, name_campo_id: atrib_name_campo_id, drow_name_control_id: atrib_drow_name_control_id,
                    control_tip_correo: atrib_control_tip_correo, value_campo_beetwen: value_campo_beetwen
                });

            }
            //valida campo con chekc
            if (validate_chek == 1 && validate_chek_one == 0) {
                return  resolve( "Debe chequear al menos un campo del formulario ");
            }
            return resolve("YES");
        } catch (ex) {
            resolve("Error valida_solicita_datos_control_general : " + ex.message);
        }
    })
    let result = myPromise;
    return result;
}
function Asigna_datos_control_interface(name_espace_class) {
    try {
        var atrib_campo_aleas = "";
        var atrib_campo_obliga = "";
        var atrib_name_campo = "";
        var atrib_campo_v = "";
        var atrib_campo_nl = "";
        var atrib_campo_tip = "";
        var valor_campo = "";
        var texto_campo = "";
        var control_element_ = document.getElementsByClassName(name_espace_class);
        if (control_element_.length == 0) {
            return "No hay controles disponibles para el espacio de nombres (" + name_espace_class + ")";
        }
        //Asigna datos del campo de un formulario   
        for (var k = 0; k < ITEM_GENERAL_CONTROL_ARRAY_ASING.length; k++) {
            for (var i = 0; i < control_element_.length; i++) {
                control_element_[i].value = "";
                atrib_name_campo = control_element_[i].attributes["atrib_campo_n"].value;
                if (atrib_name_campo == ITEM_GENERAL_CONTROL_ARRAY_ASING[k].name_campo) {
                    valor_campo = ITEM_GENERAL_CONTROL_ARRAY_ASING[k].value_campo;
                    if (control_element_[i].tagName == "INPUT") {
                        control_element_[i].value = valor_campo; 
                    }
                    if (control_element_[i].tagName == "TEXTAREA") {
                        control_element_[i].value = valor_campo;
                    }           
                }    
            }
        }    
        return "YES";
    }
    catch (ex) {
        return "Error valida_solicita_datos_control_general : " + ex.message;
    }
}
//limpia el valor de los campos de una clase expecifica  
const restore_value_form_control = (name_espace_class) => {
    try {
    var control_element_ = document.getElementsByClassName(name_espace_class);
    if (control_element_.length == 0) {
        return "YES";
    }
    for (var i = 0; i < control_element_.length; i++) {
        if (control_element_[i].tagName == "INPUT") {
            control_element_[i].value = "";
        }
        if (control_element_[i].tagName == "TEXTAREA") {
            control_element_[i].value = "";
        }
        if (control_element_[i].tagName == "SELECT") {
            $("#" + control_element_[i].id).empty();
        }
    }
    return "YES";
}
    catch (ex) {
    return "Error valida_solicita_datos_control_general : " + ex.message;
}
}
function validate_email_form_control(value_correo) {
    try {
       
        //valida que se incluya el (@) en la direccion de correo electrónico
        if (value_correo.indexOf("@") == -1) {
            return "el email no es valido falta separador (@)";
        }
        //Valida nombre del servidor de dominio
        let spl = value_correo.split("@");
        let value_ = spl[spl.length - 1];
        if (value_ == "") {
            return "El email no es valido falta el servidor dominio (@gmail, @hotmail, @....)";
        }
        if (value_ == "@") {
            return "El email no es valido falta el servidor dominio (@gmail, @hotmail, @....)";
        }
        //valida que se incluya el (.) en la direccion de correo electrónico
        let value_punto = value_correo.indexOf(".");
        if (value_punto == -1 || value_punto == 0) {
            return "El email no es valido falta separador de servidor y dominio (.) ";
        }
        
        //valida que el nombre del dominio este presente (co, com etc.....)
        spl = value_correo.split(".");
        value_ = spl[spl.length - 1];
        if (value_ == "") {
            return "El email no es valido falta el dominio (.com .co etc...)";
        }
        if (value_ == ".") {
            return "El email no es valido falta el dominio (.com .co etc...)";
        }
       
        return "YES";
       
    } catch (ex) {
        return "Error " + ex.message;
    }
}
//CREA MENSAJE EN FORMULARIO
const alert_bot = (message, type, name_control) => {
    let element = document.getElementById("alert_boot_element_global");
    if (element) {
        element.remove();
    }
    const wrapper = document.createElement('div');
    wrapper.innerHTML = [
        `<div id="alert_boot_element_global" class="alert alert-${type} alert-dismissible row ml-0 mr-0" role="alert">`,
        `   <div class="col-11">${message}</div>`,
        ' <div class="col-1">  <button type="button" onclick="delete_alert_boot()" class="btn-close-person" data-bs-dismiss="alert" aria-label="Close"></button>  </div>',
        '</div>'
    ].join('')
    let alertPlaceholder = document.getElementById(name_control);
    if (alertPlaceholder) {
        alertPlaceholder.append(wrapper);
    } else {
        document.append(wrapper);
    }
}
const delete_alert_boot = () => {
    let element = document.getElementById("alert_boot_element_global");
    if (element) {
        element.remove();
    }
}
const element_alert = (element) => {
    element.classList.add("control-person-alert");
}
const element_alert_array = (element_array) => {
    if (element_array) {
        for (let i = 0; i < element_array.length; i++) {
            if (document.getElementById(element_array[i].id_name_campo)) {
                let element_ = document.getElementById(element_array[i].id_name_campo);
                element_.style.background = "pink";
                element_.style.borderColor = "red";
                element_.focus();
            }
        }
    }
    element.style.background = "pink";
    element.style.borderColor = "red";
}
const element_alert_clear_class = (name_class) => {
    for (const element of document.getElementsByClassName(name_class)) {
        element.classList.remove("control-person-alert");
    }
}
const element_alert_clear_class_form = () => {
    for (const element of document.getElementsByClassName("control-person-alert")) {
        element.classList.remove("control-person-alert");
    }
}
//----Crear mensaje descarga de archivo
let FILE_DONWLOAD_ALERT = "";
let FILE_NAME_ALERT = "";
let TARGET_DONWLOAD_ALERT = "";
const alert_down_load_file = (file, filename, message, type, name_control, target) => {
    let element = document.getElementById("alert_boot_element_global");
    if (element) {
        element.remove();
    }
    FILE_DONWLOAD_ALERT = file;
    FILE_NAME_ALERT = filename;
    TARGET_DONWLOAD_ALERT = target;
    const wrapper = document.createElement('div');
    wrapper.innerHTML = [
        `<div id="alert_boot_element_global" class="alert alert-${type} alert-dismissible row ml-0 mr-0" role="alert">`,
        `<div class="col-11 d-flex"><a  class=" font-weight-light"  onclick="donw_load_file()" style="color: black" href="javascript:void(0)" title="Descargar archivo"> <i style="color: black" class="far fa-file-download"></i> ${message} </a> <div>  </div></div>`,
        ' <div class="col-1 p-0">  <button type="button" onclick="delete_alert_boot()" class="btn-close-person" data-bs-dismiss="alert" aria-label="Close"></button>  </div>',
        '</div>'
    ].join('')
    let alertPlaceholder = document.getElementById(name_control);
    if (alertPlaceholder) {
        alertPlaceholder.append(wrapper);
    } else {
        document.append(wrapper);
    }
}
const donw_load_file = () => {
    var element = document.createElement('a');
    element.setAttribute('href', FILE_DONWLOAD_ALERT);
    element.setAttribute('download', FILE_NAME_ALERT);
    element.setAttribute('target', TARGET_DONWLOAD_ALERT);
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
    
}
const donw_load_file_general = (url_file, file_name, target) => {
    var element = document.createElement('a');
    element.setAttribute('href', url_file);
    element.setAttribute('download', file_name);
    element.setAttribute('target', target);
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);

}
const Detec_chanque_valor_campo = () => {
    let ITEM_GENERAL_CONTROL_ARRAY_DIFERENT_ASENT = new Array();
    for (let i = 0; i < ITEM_GENERAL_CONTROL_ARRAY_ASING.length; i++) {
        for (let k = 0; k < ITEM_GENERAL_CONTROL_ARRAY.length; k++) {
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo == ITEM_GENERAL_CONTROL_ARRAY[k].name_campo &&
                ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo != ITEM_GENERAL_CONTROL_ARRAY[k].value_campo) {
                ITEM_GENERAL_CONTROL_ARRAY_DIFERENT_ASENT.push(ITEM_GENERAL_CONTROL_ARRAY[k]);
            }
        }
    }
    return ITEM_GENERAL_CONTROL_ARRAY_DIFERENT_ASENT;
}
//CREA INTERFACE FORMULARIOS

//ZONA INTERFACE CONSULTA GABINETE MIGRACION
//
const search_form_control_gabinete = (name_espace_class) => {
    try {
        let atrib_campo_aleas = "";
        let atrib_campo_obliga = "";
        let atrib_name_campo = "";
        let atrib_campo_v = "";
        let atrib_campo_nl = "";
        let atrib_campo_tip = "";
        let valor_campo = "";
        let texto_campo = "";
        let valor_campo_beetwen = "";
        let texto_campo_beetwen = "";
        let atrib_campo_id = "0";
        let atrib_campo_t = "";
        let atrib_campo_tbl = "";
        let atrib_control_tip_correo = "";
        let atrib_value_campo_old = "";
        let atrib_name_campo_id = "";
        let atrib_drow_name_control_id = "";
        let atrib_campo_beetwen = 0;
        let control_element_ = document.getElementsByClassName(name_espace_class);
        if (control_element_.length == 0) {
            return "No hay controles disponibles para el espacio de nombres (" + name_espace_class + ")";
        }
        element_alert_clear_class(name_espace_class);
        //Valida campos obligatorios del formulario   
        for (var i = 0; i < control_element_.length; i++) {
            atrib_campo_aleas = control_element_[i].attributes["atrib_aleas_c"].value;
            atrib_campo_obliga = control_element_[i].attributes["atrib_campo_O"].value;
            atrib_name_campo = control_element_[i].attributes["atrib_campo_n"].value;
            atrib_campo_v = control_element_[i].attributes["atrib_campo_v"].value;
            atrib_campo_nl = control_element_[i].attributes["atrib_campo_nl"].value;
            atrib_campo_tip = control_element_[i].attributes["atrib_campo_tip"].value;
            atrib_campo_id = control_element_[i].attributes["atrib_campo_id"].value;
            atrib_campo_t = control_element_[i].attributes["atrib_campo_t"].value;
            atrib_campo_tbl = control_element_[i].attributes["atrib_campo_tbl"].value;
            atrib_control_tip_correo = control_element_[i].attributes["atrib_control_tip_correo"].value;
            atrib_name_campo_id = control_element_[i].attributes["atrib_name_campo_id"].value;
            atrib_drow_name_control_id = control_element_[i].attributes["atrib_drow_name_control_id"].value;
            valor_campo = control_element_[i].value;
            //Valida campos tipo correo
            if (atrib_campo_tip == "cor" && valor_campo != "") {
                let res = validate_email_form_control(valor_campo);
                if (res !== "YES") {
                    element_alert(control_element_[i]);
                    control_element_[i].focus();
                    return res;
                }
            }
            //Valida campos tipo correo
            if (atrib_control_tip_correo == 1 && valor_campo != "") {
                let res = validate_email_form_control(valor_campo);
                if (res !== "YES") {
                    element_alert(control_element_[i]);
                    control_element_[i].focus();
                    return res;
                }
            }
            //Valida campos tipo fecha
            if (atrib_campo_t == "DATE" && valor_campo != "") {
                let result = validate_fecha_focus(valor_campo);
                if (result !== "YES") {
                    element_alert(control_element_[i]);
                    control_element_[i].focus();
                    return result;
                }
            }

            //Valida campos numericos
        }
        //Asigna datos del formulario a la estructura
        let validate_chek = 0;
        let validate_chek_one = 0;
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        for (var i = 0; i < control_element_.length; i++) {
            atrib_campo_beetwen = control_element_[i].attributes["atrib_campo_beetwen"].value;
            if (atrib_campo_beetwen == 0) {  
            atrib_campo_aleas = control_element_[i].attributes["atrib_aleas_c"].value;
            atrib_campo_obliga = control_element_[i].attributes["atrib_campo_O"].value;
            atrib_name_campo = control_element_[i].attributes["atrib_campo_n"].value;
            atrib_campo_v = control_element_[i].attributes["atrib_campo_v"].value;
            atrib_campo_nl = control_element_[i].attributes["atrib_campo_nl"].value;
            atrib_campo_tip = control_element_[i].attributes["atrib_campo_tip"].value;
            atrib_campo_id = control_element_[i].attributes["atrib_campo_id"].value;
            atrib_campo_t = control_element_[i].attributes["atrib_campo_t"].value;
            atrib_campo_tbl = control_element_[i].attributes["atrib_campo_tbl"].value;
            atrib_name_campo_id = control_element_[i].attributes["atrib_name_campo_id"].value;
            valor_campo = control_element_[i].value;
            atrib_value_campo_old = control_element_[i].attributes["atrib_value_campo_old"].value;
            atrib_drow_name_control_id = control_element_[i].attributes["atrib_drow_name_control_id"].value;
            atrib_control_tip_correo = control_element_[i].attributes["atrib_control_tip_correo"].value;
            if (control_element_[i].tagName == "INPUT") {
                valor_campo = control_element_[i].value;
                texto_campo = control_element_[i].value;
            }
            if (control_element_[i].tagName == "TEXTAREA") {
                valor_campo = control_element_[i].value;
                texto_campo = control_element_[i].value;
            }
            if (control_element_[i].tagName == "SELECT") {
                texto_campo = control_element_[i].options[control_element_[i].selectedIndex].text;
                valor_campo = control_element_[i].options[control_element_[i].selectedIndex].value;
            }
            let chek_itent = 0;
            let name_chek = "chek_item_" + control_element_[i].id;
            if (document.getElementById(name_chek)) {
                validate_chek = 1;
                let htmlImPutChec = document.getElementById(name_chek)
                if (htmlImPutChec.checked == true) {
                    chek_itent = 1;
                    validate_chek_one = 1;
                }
            }
            if (atrib_campo_t == "DATE" || atrib_campo_t == "INT") {
                texto_campo_beetwen = document.getElementById(control_element_[i].id + "_par").value;
                valor_campo_beetwen = document.getElementById(control_element_[i].id + "_par").value;
            } else {
                texto_campo_beetwen = "";
                valor_campo_beetwen = "";
            }
            if (valor_campo_beetwen != "" || valor_campo !="") {
            ITEM_GENERAL_CONTROL_ARRAY.push({
                value_campo: valor_campo.replace("'", ""), tipo_control: control_element_[i].tagName, name_campo: atrib_name_campo,
                texto_campo: texto_campo.replace("'", ""), obligatorio_campo: atrib_campo_obliga, name_space_campo: name_espace_class,
                max_leng_campo: control_element_[i].maxLength, aleas_campo: atrib_campo_aleas, alow_tipo_value: atrib_campo_v, alow_null: atrib_campo_nl,
                campo_tip: atrib_campo_tip, dms_id_registro: atrib_campo_id, tipo_campo: atrib_campo_t, tbl_control: atrib_campo_tbl, atrib_chek: chek_itent,
                value_campo_old: atrib_value_campo_old, name_campo_id: atrib_name_campo_id, drow_name_control_id: atrib_drow_name_control_id,
                control_tip_correo: atrib_control_tip_correo, campo_beetwen: atrib_campo_beetwen, texto_campo_beetwen: texto_campo_beetwen,
                value_campo_beetwen: valor_campo_beetwen
            });
            }
        }
        }
        //valida campo con chekc
        if (validate_chek == 1 && validate_chek_one == 0) {
            return "Debe chequear al menos un campo del formulario ";
        }
        if (ITEM_GENERAL_CONTROL_ARRAY.length == 0) {
            return "Debe informar un parametro de busqueda";
        }
        return "YES";
    } catch (ex) {
        return "Error search_form_control_gabinete : " + ex.message;
    }
}

function Create_interface_formulario_control(name_control_padre, class_name_form_control, asigna_valor, apost_name_content, add_check) {
    try {
        let name_padre_control = document.getElementById(name_control_padre);
        if (name_padre_control == null) {
            return "Imposible encontrar el control padre de la interface " + name_control_padre;
        }
        //limpia los campos anteriores
        while (name_padre_control.hasChildNodes()) {
            name_padre_control.removeChild(name_padre_control.firstChild);
        }
        //Crea el div de alerta
        var divtml = document.createElement("div");
        divtml.classList.add("w-100");
        divtml.classList.add("row");
        divtml.classList.add("p-1");
        divtml.classList.add("alert-danger-alert");
        divtml.classList.add("text-center-alert");
        divtml.classList.add("hide-alert");
        divtml.classList.add("alert-alert");
        divtml.classList.add(class_name_form_control + "error_alert");
        name_padre_control.appendChild(divtml);
        for (var i = 0; i < ITEM_GENERAL_CONTROL_ARRAY_ASING.length; i++) {
            //Add campo DIV ROW
            divtml = document.createElement("div");
            divtml.classList.add("row");    
            name_padre_control.appendChild(divtml);
            //Add campo DIV COLUMNA
            var divtml_ = document.createElement("div");
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].type_cells_alow_control == "group") {
                divtml_.classList.add("btn-group");
                divtml_.classList.add("col-12");
                divtml.classList.add("p-1");
                divtml.classList.add("pt-3");
            } else {
                divtml.classList.add("p-1");
                divtml_.classList.add("col-6");
            }   
            //Add control chkek si eta actviva la function
            if (add_check == 1) {
                let imputhml = document.createElement("INPUT");
                imputhml.setAttribute("type", "checkbox");
                imputhml.setAttribute("atrib_campo_n", "chek_" + ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo);
                imputhml.classList.add("chek_" + apost_name_content);
                imputhml.id = "chek_item_" + ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control;
                imputhml.classList.add("mr-1" );
                divtml_.appendChild(imputhml);
            }
            //Agrega el control SPAN  
            var spntml = document.createElement("span");
            spntml.classList.add("h6");
            //Agrega la clase agrega el formato de tonalidad del control label
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].label_input_class_font !== "" && ITEM_GENERAL_CONTROL_ARRAY_ASING[i].label_input_class_font !== null) {
                spntml.classList.add(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].label_input_class_font);
            } else {
                spntml.classList.add("font-weight-light");
            }
            
            let string_ = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].aleas_campo;
            var estado_obligatorio;
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].obligatorio_campo== "1") {
                        estado_obligatorio = " *";
                    } else {
                        estado_obligatorio = " ";
            }
            //Agrega la clase que acerca el control label al control padre
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].type_cells_alow_control == "group") {
                spntml.classList.add("control-label");
            }
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Tupcae_label == "MALL") {
                spntml.innerHTML = string_.toUpperCase() + estado_obligatorio;
            } else {
                spntml.innerHTML = string_ + estado_obligatorio;
            }     
            divtml_.appendChild(spntml);
             //Agrega el control popou ayuda popup
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tooltipAyuda !== null && ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tooltipAyuda != "") {
                let itml = document.createElement("i");
                itml.classList.add("fal");
                itml.classList.add("fa-info-circle");
                itml.classList.add("ml-1");
                let atml = document.createElement("a");
                atml.setAttribute("data-bs-toggle", "tooltip");
                atml.setAttribute("data-bs-placement", "top");
                atml.setAttribute("title", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tooltipAyuda);
                atml.appendChild(itml);
                divtml_.appendChild(atml);
            }
            divtml.appendChild(divtml_);
            //Add campo DIV COLUMNA PARA CAMPO IMPUT/OPTION/TEXTAREA   
            divtml_ = document.createElement("div");       
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].type_cells_alow_control == "group") {
                //---Agrega el nuevo row para los campos group
                divtml_.classList.add("btn-group");
                divtml_.classList.add("col-12");
                divtml = document.createElement("div");
                divtml.classList.add("row");
                divtml.classList.add("mt-0");
                name_padre_control.appendChild(divtml);
            } else {
               
                divtml_.classList.add("col-6");
            }
            //Agrega campo IMPUT/OPTION/TEXTAREA
            var imputhml;  
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 1) {
                imputhml = document.createElement("INPUT");
                if (asigna_valor == 1) {
                    imputhml.value = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo;
                }
            }
            //----Agrega control drowslist
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 0) {
                imputhml = document.createElement("SELECT");
                imputhml.classList.add("form-select");
                let m_compare = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist;
                if (m_compare != null) {
                    for (let z = 0; z < ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist.length; z++) {
                        let opt = document.createElement("OPTION");
                        opt.text = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist[z].value_campo;
                        opt.value = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist[z].id_value;
                        //Asigna el valor default del campo seleccion
                        if (asigna_valor == 1) {
                            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].texto_campo == ITEM_GENERAL_CONTROL_ARRAY_ASING[i].ilist_row_drowlist[z].value_campo) {
                                opt.selected = true;
                            }
                        }
                        imputhml.add(opt);   
                    }
                }
                imputhml.addEventListener("change", event_change_drowslis_form);
               
            }
            //---Agrega control text
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 2) {
                imputhml = document.createElement("TEXTAREA");
                if (asigna_valor == 1) {
                    imputhml.value = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo;
                }
            }
            //Valida campo disabled o enabled
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].disable_campo == 0) {
                imputhml.disabled = true;
            }
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].type_cells_alow_control == "group") {
                imputhml.classList.add("w-100");
            }
            //valida numero maximo de caracteres
            imputhml.maxLength = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].max_leng_campo;
            //Agrega los atributos al control
            imputhml.setAttribute("atrib_aleas_c", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].aleas_campo);
            imputhml.setAttribute("atrib_campo_O", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].obligatorio_campo);
            imputhml.setAttribute("atrib_campo_n", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo);
            imputhml.setAttribute("atrib_campo_v", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].alow_tipo_value);
            imputhml.setAttribute("atrib_campo_tip", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip);
            imputhml.setAttribute("atrib_campo_nl", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].alow_null);
            imputhml.setAttribute("atrib_campo_id", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dms_id_registro);
            imputhml.setAttribute("atrib_name_campo_id", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo_id);
            imputhml.setAttribute("atrib_campo_t", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo);
            imputhml.setAttribute("atrib_campo_tbl", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control);
            imputhml.setAttribute("atrib_campo_drow_destino", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].drow_name_controls_destino);
            imputhml.setAttribute("atrib_name_espace_control", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_space_campo);
            imputhml.setAttribute("atrib_control_tip_correo", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].control_tip_correo);
            imputhml.setAttribute("atrib_value_campo_old", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].value_campo_old);
            imputhml.setAttribute("atrib_drow_name_control_id", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].drow_name_control_id);
            imputhml.setAttribute("atrib_Tom_alow", ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Tom_alow);
            imputhml.id = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control;
            imputhml.classList.add(class_name_form_control);
            //-------Agrega la clase del control control_input_class          
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].control_input_class !== "" && ITEM_GENERAL_CONTROL_ARRAY_ASING[i].control_input_class !== null) {
                imputhml.classList.add(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].control_input_class);
            }
            //-------Agrega el campo IMPUT/OPTION/TEXTAREA a la celda
            divtml_.appendChild(imputhml);
            divtml.appendChild(divtml_);
            //Agrega atribute date           
            switch (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tipo_campo) {
                case "DATE":
                    load_date_form_control(imputhml.id);
                    imputhml.addEventListener("keypress", validate_fecha_form_control);
                    imputhml.placeholder = "yyyy mm dd";
                    imputhml.classList.add("W-25");
                    imputhml.classList.add("form-control-person");
                    break;
                case "INT":
                    imputhml.addEventListener("keypress", validate_numero_form_control);
                    imputhml.classList.add("W-25");
                    imputhml.classList.add("form-control-person");
                    break;
                default:
                    imputhml.classList.add("form-control");

            }
            //Agrega place hold   Place_Holder
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 1 || ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 2) {
                if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Place_Holder != "" && ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Place_Holder !== null) {
                    imputhml.placeholder = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].Place_Holder;
                    imputhml.classList.add("form-controls");
                }
            }
            //Agrega los eventos del control
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control) {
                for (let z = 0; z < ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control.length; z++) {
                    let name_event = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control[z].name_event_control;
                    let name_funtion_event = ITEM_GENERAL_CONTROL_ARRAY_ASING[i].event_control[z].name_function_event_control;
                    switch (name_funtion_event)  {
                        case "ValidateCapitalLeter" :
                            imputhml.addEventListener("focusout", ValidateCapitalLeter);
                            break;
                        case "validateLowercase":    
                            imputhml.addEventListener("focusout", validateLowercase);
                            break;
                        case "validateUpperCase":
                            imputhml.addEventListener("focusout", validateUpperCase);
                            break;
                    }   
                }
            }
            if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 1 || ITEM_GENERAL_CONTROL_ARRAY_ASING[i].campo_tip == 2) {
               
                if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control == "") {
                    service_auto_complete_form_control(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dbms_control,
                        ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo);

                } else {
                    if (ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control != "NA") {
                        service_auto_complete_form_control_person(ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo + "_" + class_name_form_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].dbms_control,
                            ITEM_GENERAL_CONTROL_ARRAY_ASING[i].tbl_control, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].name_campo, ITEM_GENERAL_CONTROL_ARRAY_ASING[i].clas_service_control,
                            ITEM_GENERAL_CONTROL_ARRAY_ASING[i].service_control);
                    } 
                   
                }           
            }     
        }
        return "YES";
    } catch (ex) {
        return "Inconsistencia general fucion Create_interface_formulario_control : " + ex.message;
    }

}
function search_valor_campo_form_control(name_espace_class) {
    try {
        ITEM_GENERAL_CONTROL_ARRAY = new Array();
        var atrib_campo_aleas = "";
        var atrib_campo_obliga = "";
        var atrib_name_campo = "";
        var atrib_campo_v = "";
        var atrib_campo_nl = "";
        var atrib_campo_tip = "";
        var valor_campo = "";
        var texto_campo = "";      
        var control_element_ = document.getElementsByClassName(name_espace_class);
        if (control_element_.length == 0) {
            return "No hay controles disponibles para el espacio de nombres (" + name_espace_class + ")";
        }
        for (var i = 0; i < control_element_.length; i++) {
            atrib_campo_aleas = control_element_[i].attributes["atrib_aleas_c"].value;
            atrib_campo_obliga = control_element_[i].attributes["atrib_campo_O"].value;
            atrib_name_campo = control_element_[i].attributes["atrib_campo_n"].value;
            atrib_campo_v = control_element_[i].attributes["atrib_campo_v"].value;
            atrib_campo_nl = control_element_[i].attributes["atrib_campo_nl"].value;
            atrib_campo_tip = control_element_[i].attributes["atrib_campo_tip"].value;
            valor_campo = control_element_[i].value;
            if (control_element_[i].tagName == "INPUT") {
                valor_campo = control_element_[i].value;
                texto_campo = control_element_[i].value;
            }
            if (control_element_[i].tagName == "TEXTAREA") {
                valor_campo = control_element_[i].value;
                texto_campo = control_element_[i].value;
            }
            if (control_element_[i].tagName == "SELECT") {
                texto_campo = control_element_[i].options[control_element_[i].selectedIndex].text;
                valor_campo = control_element_[i].options[control_element_[i].selectedIndex].value;
            }
            ITEM_GENERAL_CONTROL_ARRAY.push({
                value_campo: valor_campo.replace("'", ""), tipo_control: control_element_[i].tagName, name_campo: atrib_name_campo,
                texto_campo: texto_campo.replace("'", ""), obligatorio_campo: atrib_campo_obliga, name_space_campo: name_espace_class,
                max_leng_campo: control_element_[i].maxLength, aleas_campo: atrib_campo_aleas, alow_tipo_value: atrib_campo_v, alow_null: atrib_campo_nl,
                campo_tip: atrib_campo_tip
            });

        }
        return "YES";
    } catch (ex) {
        return ex.message;
    }
}
//--------------------------//---------------------
//ZONA DROWLIST 
//--------------------------//---------------------
//--Event control change
function event_change_drowslis_form(e) {
    let ecourrent = e.currentTarget;
    let value_e = ecourrent.value; 
    let atrrib_name_control_destino = ecourrent.attributes["atrib_campo_drow_destino"].nodeValue;
    let atrrib_name_espace_control = ecourrent.attributes["atrib_name_espace_control"].nodeValue;
    let atrrib_name_control = ecourrent.attributes["atrib_campo_n"].nodeValue;
    let name_control = atrrib_name_control_destino + "_" + atrrib_name_espace_control; 
    if (atrrib_name_control_destino == "") {
        return true;
    } 
    if (document.getElementById(name_control)) {   
        let array_drow_config_list = Drow_confing_service(atrrib_name_control_destino, ITEM_GENERAL_CONTROL_ARRAY_ASING);
        if (array_drow_config_list) { 
            //-Clear drow relacionado al control
            Drow_delete_rows(atrrib_name_espace_control, atrrib_name_control, ITEM_GENERAL_CONTROL_ARRAY_ASING);
            service_source_ilist_drow_control_general(value_e, array_drow_config_list, name_control);
        }
    } else {
        alert("Imposible encontrar el control (" + name_control + ")");
    }
}
//---Search field in array to return array config service drow list
const Drow_confing_service = (name_campo, array) => {
    for (let i = 0; i < array.length; i++) {
        if (array[i].name_campo == name_campo) {
            return array[i].config_service_drowlis_destino;
        }
    }
}
//---Clear row drow element childrens
const Drow_delete_rows = (name_espace, name_campo, array) => {
    for (let i = 0; i < array.length; i++) {
        if (array[i].drow_name_padre_control == name_campo) {
            let name_control = array[i].name_campo + "_" + name_espace;
            $("#" + name_control).empty();
        }
    }
}
//-------------------------//----------------------
//  ZONA FORMAT DATE
//------------------------//-----------------------
function load_date_form_control(name) {
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
        
     
    } catch (ex) { alert(ex.mensaje + " funcion  load_date_form_control") }

}
//------------------------//-----------------------
// ZONA VALIDATE KEY
//-----------------------//------------------------
function validate_fecha_form_control(e) {
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
        alert(err.message + " funcion validate_fecha_form_control " + err.message);
    }
}
function validate_fecha_focus(value) {
    try {
        if (value == "") {
            return "YES";
        }
        let leng = value.length;
        if (leng < 10) {
            return "formato incorrecto de fecha";
        }
       
        let year = value.substring(0, 4);
        let month = value.substring(5, 7);
        let day = value.substring(8, 10);
        if (day > 31 || day < 1) {
            return "Dia no valido para la fecha";
        }
        if (month > 12 || month < 1) {
            return "Mes no valido para la fecha";
        }
        return "YES";
    } catch (ex) {
        alert("function validate_fecha_focus " + ex.mensaje)
    }
}
function validate_numero_form_control(e) {
    try {
       let  tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 32) {
            return false;
        }
       let patron = /^[0-9]$/;
        let te = String.fromCharCode(tecla);
       let res=patron.test(te);
        if (res) {
            return patron.test(te);
        } else {
            e.preventDefault();
            return false;
        }
    } catch (err) {
        alert(err.message + " funcion validate_numero_form_control " + err.message);
    }
}
//Valida letra capital texto
const ValidateCapitalLeter = () => {   
    //let ev = event.target.value;   
    let str = event.target.value;
    str = str.toLowerCase();
    if (str != "") {
        let stringToArray = str.split(' ');
        let conversionOfAllFirstCharacterofEachWord = stringToArray.map(word => word[0].toUpperCase() + word.substring(1));
        let newString = conversionOfAllFirstCharacterofEachWord.join(' ');
        event.target.value = newString;
    }
    
}
//Valida letra minuscula texto
const validateLowercase = () => {
    let str = event.target.value;
    if (str != "") {
        str = str.toLowerCase();
        event.target.value = str;
    }  
}
//Valida letra mayuscula texto
const validateUpperCase = () => {
    let str = event.target.value;
    if (str != "") {
        str = str.toUpperCase();
        event.target.value = str;
    }
}
//------------------------//-----------------------
// ZONA  OPERATION TABLE
//-----------------------//------------------------
const update_row_table_aspnet = (control_array_config_service, table_html, identy_row) => {
    try {
        $("#" + table_html + " tr[id=" + identy_row + "]").each(function () {
            for (i = 0; i <= control_array_config_service.length - 1; i++) {
                var idex = -1;
                var text = control_array_config_service[i].texto_campo;
                idex = colum_index_table_aspnet(control_array_config_service[i].name_campo, table_html);
                if (idex != -1) {
                    $(this)[0].cells[idex].innerText = text;
                }
            }

        })
    } catch (ex) {
        return "Error funcion update_row_table_aspnet (" + ex.mensaje + ")";
    }
}
const colum_index_table_aspnet = (colum_name, table_aspnet) => {
    try {
        let x = $('#' + table_aspnet + ' th');
        let i;
        for (i = 0; i < x.length; i++) {
            if (x[i].innerText.toUpperCase() == colum_name.toUpperCase()) {
                return i;
            }
        }
        return -1;
    }
    catch (err) {
        return -1;
    }
}
const insert_row_table_aspnet = (control_array_config_service, table_html) => {
    try {
        let element_table = document.getElementById(table_html);
        let element_row;
        let element_td;
        let index_tr_title = -1;
        //Determina el index del row
        for (i = 0; i < element_table.rows.length; i++) {
            if (element_table.rows[i].className == "GridviewScrollHeader_line_boot") {
                index_tr_title = i + 1;
            }
        }
        
        //Crea el row de la tabla
        element_row = element_table.insertRow(index_tr_title);
        let id_Row = control_array_config_service[0].dms_id_registro;
        //Agrega los atributos del row
        var conta_td = 0;
        element_td = element_row.insertCell(conta_td);
        element_row.setAttribute("id", id_Row);
        element_row.style.cursor = "pointer";
        //element_row.style.background = "white";
        //element_row.style.color = "black";
        //Agrega los botones de interación de la tabla
        let divhtml = document.createElement("div");
        let ihtml = document.createElement("i");
        let ahtml = document.createElement("a");
        for (i = 0; i < control_array_config_service[0].config_service_boton_atributes_awsome.length; i++) {
            let atribute_html = control_array_config_service[0].config_service_boton_atributes_awsome[i];
            //Add elment i
            ihtml = document.createElement("i");
            ihtml.style.color = atribute_html.Element_i_name_atribute_color_awsome;
            ihtml.classList.add(atribute_html.Element_i_name_atribute_fas_awsome);
            ihtml.classList.add(atribute_html.Element_i_name_atribute_boton_awsome);
            ihtml.classList.add(atribute_html.Element_i_name_atribute_dimension_awsome);

            //Add element a
            ahtml = document.createElement("a");
            ahtml.classList.add(atribute_html.Element_a_name_atribute_fas_awsome);
            ahtml.classList.add(atribute_html.Element_a_name_atribute_color_awsome);
            ahtml.classList.add(atribute_html.Element_a_name_atribute_dimension_awsome);
            ahtml.setAttribute("onclick", atribute_html.Element_a_name_atribute_onclclick_awsome);
            ahtml.setAttribute("title", atribute_html.Element_a_name_atribute_title_awsome);
            //ahtml.setAttribute("idd", id_tarea);
            ahtml.setAttribute("tip_event", atribute_html.Element_a_name_atribute_tip_event_awsome);
            for (k = 0; k < atribute_html.Element_a_atribute_atributes_boton_awsome.length; k++) {
                let atribute_dinamicos = atribute_html.Element_a_atribute_atributes_boton_awsome[k];
                ahtml.setAttribute(atribute_dinamicos.name_atribute, atribute_dinamicos.value_atribute);
            }
            ahtml.style.marginLeft = "3px";
            ahtml.appendChild(ihtml);
            divhtml.appendChild(ahtml);
           
        }
        divhtml.style.display = "inline-flex";
        element_td.appendChild(divhtml);
        for (i = 0; i < control_array_config_service.length; i++) {
             conta_td++;
             element_td = element_row.insertCell(conta_td);
             element_td.innerHTML = control_array_config_service[i].texto_campo;
             element_td.classList.add("GridviewScrollItem_line_cort_tr_flex");
             element_td.setAttribute("onclick", "prevent_scrol(event,this)");
        }
        $('#' + table_html + ' tr[id]').click(function () {
            $('#' + table_html + ' tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#e8e8f7", "color": "Black" });
            //var fer = $(this).attr("id");
            //$('#hdnEmailID_VAL').val(fer);
        });
        $('#' + table_html + ' tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
    } catch (ex) {
        return "Error funcion insert_row_table_aspnet (" + ex.mensaje + ")";
    }
}
const delete_row_table_aspnet = (array_config_delete) => {
    try {
        $('#' + array_config_delete[0].table_html + ' tr[id=' + array_config_delete[0].identy_row + ']').remove();
        $('#' + array_config_delete[0].control_seting).val("-1");
        var chid = $('#' + array_config_delete[0].table_html + ' >tbody >tr').length;
        if (chid >= 1) {
            chid = chid - 1;
        }   
        if (document.getElementById(array_config_delete[0].control_title)) {
            document.getElementById(array_config_delete[0].control_title).innerHTML = "Se encontraron " + chid + " registro(s)  ";
        }
    }
    catch (err) {
        alert(err.message + " Funcion delete_row_table_aspnet");
    }
}
//-----------------------//------------------------
//                 zona service
//----------------------//-------------------------

const service_delete_row = (array_config_delete, class_service, name_service, name_container, name_control_error) => {
    try {
        CONTROL_EVENT_GENERAL = name_control_error;
        $.ajax('../webservice/' + class_service + '/' + name_service, {
            data: "{" + "'parameter':'" + array_config_delete[0].identy_row + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    ESTADO_EVENT_GENERAL = "out";
                    RESULT_EVENT_GENERAL = data.d[0].error_gestion;
                    if (document.getElementById(name_control_error)) {
                        alert_bot(data.d[0].error_gestion, 'warning', name_control_error);
                    } else {
                        alert(data.d[0].error_gestion);
                    }
                    return data.d[0].error_gestion;
                } else {
                    if (name_container !== "") {
                        $find(name_container).hide();
                    }
                    delete_row_table_aspnet(array_config_delete)
                    RESULT_EVENT_GENERAL = "YES";
                    ESTADO_EVENT_GENERAL = "out";

                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    RESULT_EVENT_GENERAL = 'Not connect: Verify Network.';

                } else if (xception.status == 404) {

                    RESULT_EVENT_GENERAL = 'Requested page not found [404]';

                } else if (xception.status == 500) {

                    RESULT_EVENT_GENERAL = 'Internal Server Error [500].' + xception.responseText;

                } else if (textStatus === 'parsererror') {

                    RESULT_EVENT_GENERAL = 'Requested JSON parse failed.';

                } else if (textStatus === 'timeout') {

                    RESULT_EVENT_GENERAL = 'Time out error.';

                } else if (textStatus === 'abort') {

                    RESULT_EVENT_GENERAL = 'Ajax request aborted.';

                } else {

                    RESULT_EVENT_GENERAL = 'Uncaught Error: ' + xception.responseText;

                }
            }, compelete: function () {
                RESULT_EVENT_GENERAL = "YES";
            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        RESULT_EVENT_GENERAL = ex.message;


    }
}
const service_insert_row = (control_array_config_service, class_service, name_service, name_container, name_control_error, name_table_html) => {
    try {
        CONTROL_EVENT_GENERAL = name_control_error;
        var serialice = JSON.stringify(control_array_config_service);
        $.ajax('../webservice/' + class_service + '/' + name_service, {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    ESTADO_EVENT_GENERAL = "out";
                    RESULT_EVENT_GENERAL = data.d[0].error_gestion;
                    if (document.getElementById(name_control_error)) {
                        alert_bot(data.d[0].error_gestion, 'warning', name_control_error);
                        element_alert_array(data.d[0].config_service_controls_error);
                        
                    } else {
                        alert(data.d[0].error_gestion);
                    }
                    return data.d[0].error_gestion;
                } else {
                    if (name_container !== "") {
                        $find(name_container).hide();
                    }
                    insert_row_table_aspnet(data.d, name_table_html)
                    RESULT_EVENT_GENERAL = "YES";
                    ESTADO_EVENT_GENERAL = "out";

                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    RESULT_EVENT_GENERAL = 'Not connect: Verify Network.';

                } else if (xception.status == 404) {

                    RESULT_EVENT_GENERAL = 'Requested page not found [404]';

                } else if (xception.status == 500) {

                    RESULT_EVENT_GENERAL = 'Internal Server Error [500].' + xception.responseText;

                } else if (textStatus === 'parsererror') {

                    RESULT_EVENT_GENERAL = 'Requested JSON parse failed.';

                } else if (textStatus === 'timeout') {

                    RESULT_EVENT_GENERAL = 'Time out error.';

                } else if (textStatus === 'abort') {

                    RESULT_EVENT_GENERAL = 'Ajax request aborted.';

                } else {

                    RESULT_EVENT_GENERAL = 'Uncaught Error: ' + xception.responseText;

                }
            }, compelete: function () {
                RESULT_EVENT_GENERAL = "YES";
            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        RESULT_EVENT_GENERAL = ex.message;


    }
}
const service_update_row = (control_array_config_service, class_service, name_service, name_container, name_control_error, name_table_html) => {
    try {
        CONTROL_EVENT_GENERAL = name_control_error;
        var serialice = JSON.stringify(control_array_config_service);
        $.ajax('../webservice/' + class_service + '/' + name_service, {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    ESTADO_EVENT_GENERAL = "out";
                    RESULT_EVENT_GENERAL = data.d[0].error_gestion;
                    if (document.getElementById(name_control_error)) {
                        alert_bot(data.d[0].error_gestion, 'warning', name_control_error);
                        element_alert_array(data.d[0].config_service_controls_error);
                        
                    } else {
                        element_alert_array(data.d[0].config_service_controls_error);
                        alert(data.d[0].error_gestion);
                    } 
                    return data.d[0].error_gestion;
                } else {
                    if (name_table_html !== "") {
                        update_row_table_aspnet(data.d, name_table_html, data.d[0].dms_id_registro);
                    }
                    if (name_container !== "") {
                        $find(name_container).hide();
                    }
                                   
                    RESULT_EVENT_GENERAL = "YES";
                    ESTADO_EVENT_GENERAL = "out";
                   
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    RESULT_EVENT_GENERAL = 'Not connect: Verify Network.';

                } else if (xception.status == 404) {

                    RESULT_EVENT_GENERAL = 'Requested page not found [404]';

                } else if (xception.status == 500) {

                    RESULT_EVENT_GENERAL = 'Internal Server Error [500].' + xception.responseText;

                } else if (textStatus === 'parsererror') {

                    RESULT_EVENT_GENERAL = 'Requested JSON parse failed.';

                } else if (textStatus === 'timeout') {

                    RESULT_EVENT_GENERAL = 'Time out error.';

                } else if (textStatus === 'abort') {

                    RESULT_EVENT_GENERAL = 'Ajax request aborted.';

                } else {

                    RESULT_EVENT_GENERAL = 'Uncaught Error: ' + xception.responseText;

                }
            }, compelete: function () {
                RESULT_EVENT_GENERAL = "YES";
            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        RESULT_EVENT_GENERAL = ex.message;


    }
}
//Servicio web que solicita los datos y dibuja los contorles dentro del formulario
//-----Parameter 
//---- id_registro : Identificador del registro
//---- class_service : nombre del web service
//---- name_service  : nombre de la fución web service
//---- name_container : Nombre del modal popup
//---- name_control_padre : Nombre del control div contenedor
//---- asigna_valor   :  Representa si asigna valores al formulario
//---- apost_name_content : Nombre del panel y de la clase de los chek
//---- add_check  : Determina si agrega chekbox
//---- name_form_parent_mensaje : nombre del control donde se dibuja el mensaje
//---- name_control_tittle : Name del control del titulo
//----- title : title que muestra el control popup 
//----- porcent_heigth : Porcentaje de altura
const html_form_ontrol = (id_registro, class_service, name_service, name_container, name_control_padre,
    asigna_valor, apost_name_content, add_check, name_form_parent_mensaje, name_control_tittle, title,porcent_heigth) => {
    try {
        ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array();
        $.ajax('../webservice/' + class_service + '/' + name_service, {
            data: "{" + "'parameter':'" + id_registro + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_gestion !== "YES") {
                    if (document.getElementById(name_form_parent_mensaje)) {
                        alert_bot(data.d[0].error_gestion, 'warning', name_form_parent_mensaje);
                    } else {
                        alert(data.d[0].error_gestion);
                    }    
                    ESTADO_EVENT_GENERAL = "out";
                    RESULT_EVENT_GENERAL = data.d[0].error_gestion;
                    CONTROL_EVENT_GENERAL = name_form_parent_mensaje;
                    
                } else {       
                    $.each(data.d, function (k, v) {
                        ITEM_GENERAL_CONTROL_ARRAY_ASING.push(v);
                    });
                    delete_alert_boot();
                    name_form_control = ITEM_GENERAL_CONTROL_ARRAY_ASING[0].name_space_campo;
                    var resultado = Create_interface_formulario_control(name_control_padre, name_form_control, asigna_valor, apost_name_content, add_check);
                    if (resultado == "YES") {
                        if (name_container !== "") {
                            $find(name_container).show();
                            if (apost_name_content != "") {
                                auto_zise_popup_lista_form_control_person_procent(apost_name_content, porcent_heigth);
                            }
                        }
                        if (name_control_tittle != "") {
                            let control_title = document.getElementById(name_control_tittle);
                            if (control_title) {
                                if (title == "") {
                                    control_title.textContent = data.d[0].title_control;
                                } else {
                                    control_title.textContent = title;
                                }
                               
                            }
                        }
                        RESULT_EVENT_GENERAL = resultado;
                        ESTADO_EVENT_GENERAL = "out";      
                        return resultado;
                    } else {
                        if (document.getElementById(name_form_parent_mensaje)) {
                            alert_bot(resultado, 'warning', name_form_parent_mensaje);
                        } else {
                            alert(resultado);
                        }
                        RESULT_EVENT_GENERAL = resultado;
                        ESTADO_EVENT_GENERAL = "out";             
                        return resultado;
                    }
                   
                }
            }, error: function (xception, textStatus, errorThrown) {
                ESTADO_EVENT_GENERAL = "out";
                if (xception.status === 0) {

                    RESULT_EVENT_GENERAL = 'Not connect: Verify Network.';

                } else if (xception.status == 404) {

                    RESULT_EVENT_GENERAL = 'Requested page not found [404]';

                } else if (xception.status == 500) {

                    RESULT_EVENT_GENERAL = 'Internal Server Error [500].' + xception.responseText;

                } else if (textStatus === 'parsererror') {

                    RESULT_EVENT_GENERAL = 'Requested JSON parse failed.';

                } else if (textStatus === 'timeout') {

                    RESULT_EVENT_GENERAL = 'Time out error.';

                } else if (textStatus === 'abort') {

                    RESULT_EVENT_GENERAL = 'Ajax request aborted.';

                } else {

                    RESULT_EVENT_GENERAL = 'Uncaught Error: ' + xception.responseText;

                }
            }, compelete: function () {
                RESULT_EVENT_GENERAL = "YES";
            }
        });
    } catch (ex) {
        ESTADO_EVENT_GENERAL = "out";
        RESULT_EVENT_GENERAL = ex.message;    
    }
}
//Dibuja los contoroles de busqueda de la consulta por campos de gabinete migracion  .
const add_form_control_search_gabinete = async (id_registro, class_service, name_service, name_container, name_control_padre,
                                                asigna_valor, apost_name_content, add_check, name_form_parent_mensaje,
    name_control_tittle, title, porcent_heigth, aplica_campo_date, aplica_campo_id, NameEvent) => {
    let name_padre_control_ = document.getElementById(name_control_padre); 
    ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array();
    let myPromise = new Promise(function (resolve) {
        if (name_padre_control_ == null) {
            resolve( "Imposible encontrar el control padre de la interface " + name_control_padre);
        }
       //limpia los campos anteriores
        while (name_padre_control_.firstChild) {
            name_padre_control_.removeChild(name_padre_control_.firstChild);

        }
        //sale de la funcion si no hay registro
        if (id_registro == -1 || id_registro == 0) {
          return resolve("YES");
        }
        try {
            $.ajax('../webservice/' + class_service + '/' + name_service, {
                data: "{" + "'id_gabinete':'" + id_registro + "','" + "aplica_campo_date':'" + aplica_campo_date + "','" + "aplica_campo_id':'" + aplica_campo_id + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        return resolve(data.d[0].error_gestion); 

                    } else {
                        $.each(data.d, function (k, v) {
                            ITEM_GENERAL_CONTROL_ARRAY_ASING.push(v);
                        });
                        delete_alert_boot();
                        let class_name_form_control = ITEM_GENERAL_CONTROL_ARRAY_ASING[0].name_space_campo;
                        var resultado = Create_interfaz_formulario_search_gabinet(name_control_padre, class_name_form_control, asigna_valor, apost_name_content, add_check, NameEvent);
                        if (resultado == "YES") {
                            if (name_container !== "") {
                                $find(name_container).show();
                                if (apost_name_content != "") {
                                    auto_zise_popup_lista_form_control_person_procent(apost_name_content, porcent_heigth);
                                }
                            }
                            if (name_control_tittle != "") {
                                let control_title = document.getElementById(name_control_tittle);
                                if (control_title) {
                                    if (title == "") {
                                        control_title.textContent = data.d[0].title_control;
                                    } else {
                                        control_title.textContent = title;
                                    }

                                }
                            }
                           
                            return resolve(resultado);
                        } else {
                         
                            return resolve(resultado);
                        }

                    }
                }, error: function (xception, textStatus, errorThrown) {
                    ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                       
                        return resolve('Not connect: Verify Network.');
                    } else if (xception.status == 404) {

                        
                        return resolve('Requested page not found [404]');
                    } else if (xception.status == 500) {

                        
                        return resolve('Internal Server Error [500].' + xception.responseText);
                    } else if (textStatus === 'parsererror') {

                        
                        return resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        
                        return resolve('Time out error.');
                    } else if (textStatus === 'abort') {

                        
                        return resolve('Ajax request aborted.');

                    } else {

                        
                        return resolve('Uncaught Error: ' + xception.responseText);

                    }
                }, compelete: function () {
                    
                    return resolve("YES");
                }
            });
           
        } catch (ex) {
            return resolve(EX.mensaje);
        }
    });
    let resul =  myPromise;
    return resul;
}
const Service_REST_lista_interface_busqueda_documentos_migrados = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array();
    let myPromise = new Promise(function (resolve) {
        try {
            $.ajax('../webservice/' + parameter[0].class_service + '/' + parameter[0].name_service, {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);

                    } else {
                        $.each(data.d, function (k, v) {
                            ITEM_GENERAL_CONTROL_ARRAY_ASING.push(v);
                        });
                        var resultado = Create_interfaz_formulario_search_gabinet(parameter[0].name_container, parameter[0].class_name_control,
                            parameter[0].asigna_valor, parameter[0].apost_name_content, parameter[0].add_check);
                        if (resultado == "YES") {
                            
                            resolve(resultado);
                        } else {
                            resolve(resultado);
                        }
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_lista_interface_busqueda_documentos_migrados");
        }
    })
    let result = await myPromise;
    return result;
}
const Service_REST_interface_form_clontrol_bootStrap = async (parameter) => {
    var serialice = JSON.stringify(parameter);
    ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array();
    let myPromise = new Promise(function (resolve) {
        try {    
            $.ajax('../webservice/' + parameter[0].class_service + '/' + parameter[0].name_service, {
                data: "{" + "'parameter':'" + serialice + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        resolve(data.d[0].error_gestion);
                       
                    } else {  
                        $.each(data.d, function (k, v) {
                            ITEM_GENERAL_CONTROL_ARRAY_ASING.push(v);
                        });
                        //console.log(ITEM_GENERAL_CONTROL_ARRAY_ASING);
                        var resultado = Create_interface_formulario_control(parameter[0].name_container, parameter[0].class_name_control,
                            parameter[0].asigna_valor, parameter[0].apost_name_content, parameter[0].add_check);
                        if (resultado == "YES") {
                            if (parameter[0].name_control_padre !== "") {
                                $("#" + parameter[0].name_control_padre).modal("show");     
                            }
                            resolve(resultado);
                        } else {
                            resolve(resultado);
                        }
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    //ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        resolve('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        resolve('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        resolve('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        resolve('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        resolve('Time out error.');

                    } else if (textStatus === 'abort') {

                        resolve('Ajax request aborted.');

                    } else {

                        resolve('Uncaught Error: ' + xception.responseText);

                    }
                }
            });
        } catch (ex) {
            resolve(ex.message + " funcion Service_REST_interface_form_clontrol_bootStrap");
        }
    })     
    let result = await myPromise;
    return result;
}
function Service_interface_form_control(id_registro, class_service, name_service, name_container, name_control_padre, asigna_valor, apost_name_content,add_check) {
        try {
            ITEM_GENERAL_CONTROL_ARRAY_ASING = new Array();
            $.ajax('../webservice/' + class_service + '/' + name_service, {
                data: "{" + "'parameter':'" + id_registro + "'}",
                dataType: 'json',
                type: "POST",
                traditional: true,
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data.d[0].error_gestion !== "YES") {
                        alert(data.d[0].error_gestion);
                        ESTADO_EVENT_GENERAL = "out";
                    } else {
                        ESTADO_EVENT_GENERAL = "out";
                        $.each(data.d, function (k, v) {
                            ITEM_GENERAL_CONTROL_ARRAY_ASING.push(v);
                        });   
                        name_form_control = ITEM_GENERAL_CONTROL_ARRAY_ASING[0].name_space_campo;
                        var resultado = Create_interface_formulario_control(name_control_padre, name_form_control, asigna_valor, apost_name_content, add_check);
                        if (resultado == "YES") {
                            if (name_container !== "") {
                                $find(name_container).show();
                                if (apost_name_content != "") {
                                    auto_zise_popup_lista_form_control_person(apost_name_content);
                                }
                            }

                        } else {
                            alert(resultado);
                        }
                    }
                }, error: function (xception, textStatus, errorThrown) {
                    ESTADO_EVENT_GENERAL = "out";
                    if (xception.status === 0) {

                        alert('Not connect: Verify Network.');

                    } else if (xception.status == 404) {

                        alert('Requested page not found [404]');

                    } else if (xception.status == 500) {

                        alert('Internal Server Error [500].' + xception.responseText);

                    } else if (textStatus === 'parsererror') {

                        alert('Requested JSON parse failed.');

                    } else if (textStatus === 'timeout') {

                        alert('Time out error.');

                    } else if (textStatus === 'abort') {

                        alert('Ajax request aborted.');

                    } else {

                        alert('Uncaught Error: ' + xception.responseText);

                    }
                }, compelete: function () {

                }
            });
        } catch (ex) {
            ESTADO_EVENT_GENERAL = "out";
            alert(ex.message + " funcion Service_interface_form_control");
        }
    }
function service_auto_complete_form_control(name_control, name_dbs_auto, name_table_auto, name_campo_auto) {
    var ITEM_SERIAL = new Array();
    ITEM_SERIAL.push({
        "name_dbs_auto": name_dbs_auto, "name_table_auto": name_table_auto, "name_campo_auto": name_campo_auto,
        value_auto: document.getElementById(name_control).value
    })
    var serialice = JSON.stringify(ITEM_SERIAL);
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#" + name_control)
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "../webservice/WebService_control_general.asmx/Service_Solicita_datos_auto_complete_campos_form_control",
                    data: "{'parameter':'" + serialice + "','value':'" + document.getElementById(name_control).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    //dataFilter: function (data) { return data; },
                    success: function (data) {
                        term: extractLast(request.term)
                        response($.ui.autocomplete.filter(
                            data.d, extractLast(request.term)));
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },

            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                document.getElementById(name_control).value = ui.item.label;
                
            }

            , minLength: 3, max: 10, scroll: true
        });
}
function service_auto_complete_form_control_person(name_control, name_dbs_auto, name_table_auto, name_campo_auto, clas_service_control, service_control,NameEvent) {
    var ITEM_SERIAL = new Array();
    ITEM_SERIAL.push({
        "name_dbs_auto": name_dbs_auto, "name_table_auto": name_table_auto, "name_campo_auto": name_campo_auto,
        value_auto: document.getElementById(name_control).value
    })
    var serialice = JSON.stringify(ITEM_SERIAL);
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#" + name_control)
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "../webservice/" + clas_service_control + "/" + service_control,
                    data: "{'parameter':'" + serialice + "','value':'" + document.getElementById(name_control).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.d !== null) {
                            term: extractLast(request.term)
                            response($.ui.autocomplete.filter(
                                data.d, extractLast(request.term)));
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },

            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                document.getElementById(name_control).value = ui.item.label;
                if (NameEvent) {
                    switch (NameEvent) {
                        case "SearchGabineteConsulta":
                            EventSolicitaConsultaGabinete();
                            break;
                    }
              
                }
            }

            , minLength: 3, max: 10, scroll: true
        });
}
function service_source_list_item_control_general(id_, name_url_service ,name_service, name_control) {
    try {
        $.ajax('../webservice/WebServiceRadicacion.asmx/' + name_service, {
            data: "{'id':" + "'" + id_ + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_sistema !== "YES") {
                    alert(data.d[0].error_sistema);

                } else {
                    ITEMS_DATOS_DROW = new Array();
                    $.each(data.d[0].item_sistema, function (k, v) {
                        ITEMS_DATOS_DROW.push(v);
                    });
                    if (document.getElementById(name_control)) {
                        var element_drow = document.getElementById(name_control);
                        for (var i = 0; i < ITEMS_DATOS_DROW.length; i++) {
                            element_drow[i] = new Option(ITEMS_DATOS_DROW[i].text, ITEMS_DATOS_DROW[i].value);
                        }
                    }
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {

        alert('service_source_list_item_control_general ' + ex.message);
    }
}
function service_source_ilist_drow_control_general(value, array_config_general_service_drowlist, name_control) {
    var serialice = JSON.stringify(array_config_general_service_drowlist);
    try {
        $.ajax('../webservice/WebService_control_general.asmx/service_source_ilist_drow_control_general' , {
            data: "{'value':" + "'" + value + "','seralice_config_general_service_drowlist':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_sistema !== "YES") {
                    alert(data.d[0].error_sistema);

                } else {         
                    if (document.getElementById(name_control)) {
                        $("#" + name_control).empty();
                        var element_drow = document.getElementById(name_control);
                        if (data.d[0].id_value != "-11") {
                            for (var i = 0; i < data.d.length; i++) {
                                element_drow[i] = new Option(data.d[i].value_campo, data.d[i].id_value);
                            }
                        }
                    }
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {

                    alert('Not connect: Verify Network.');

                } else if (xception.status == 404) {

                    alert('Requested page not found [404]');

                } else if (xception.status == 500) {

                    alert('Internal Server Error [500].' + xception.responseText);

                } else if (textStatus === 'parsererror') {

                    alert('Requested JSON parse failed.');

                } else if (textStatus === 'timeout') {

                    alert('Time out error.');

                } else if (textStatus === 'abort') {

                    alert('Ajax request aborted.');

                } else {

                    alert('Uncaught Error: ' + xception.responseText);

                }
            }
        });
    }
    catch (ex) {

        alert('service_source_ilist_drow_control_general ' + ex.message);
    }
}
//------------------------///--------------------------------------
//                  ZONA AUTO SIZE
//-----------------------///---------------------------------------
function auto_zise_popup_lista_form_control_person(apost_name_content) {
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

        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 2) / 100);  // Indica el porcentaje de espacio vertical del elemento
        let control_panel = document.getElementById("Panel_" + apost_name_content);
        let heigth_panel = 1;
        if (control_panel) {
            $('#' + control_panel.id).css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            heigth_panel = control_panel.clientHeight;
            //heigth_panel = document.getElementById(control_panel.id).clientHeight;
        }
        let control_modal_foter = document.getElementById("modal_foter_" + apost_name_content);
        let heigth_modal_foter = 1;
        if (control_modal_foter) {
            heigth_modal_foter = control_modal_foter.clientHeight;
        }
        let control_modal = document.getElementById("modal_content_" + apost_name_content);
        let heigth_modal = 1;
        if (control_modal) {
            $('#' + control_modal.id).css("height", (heig_porcent - heigth_modal_foter) + "px"); //Asigna altura al panel contenedor del modal
            heigth_modal = control_modal.clientHeight;
        }

        let control_title = document.getElementById("title_" + apost_name_content);
        let heigth_title = 1;
        if (control_title) {
            heigth_title = control_title.clientHeight;
        }
        
        let control_contenido = document.getElementById("contenido_procesa_" + apost_name_content);
        let heigth_contenido = 1;
        if (control_contenido) {     
            $('#' + control_contenido.id).css("height", (heigth_modal - (heigth_title)) + "px");   //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            heigth_contenido = control_contenido.clientHeight;
        }
        let control_div = document.getElementById("div_" + apost_name_content);
        let heigth_div = 1;
        if (control_div) {
            $('#' + control_div.id).css("height", (heigth_contenido - 5) + "px"); //Asigna altura del control padre
            heigth_div = control_div.clientHeight;
        }
        //$('#modal_content_lista_actividades_ruta').css("height", (heig_porcent - 3) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        //$('#contenido_procesa_lista_actividades_ruta').css("height", (document.getElementById("modal_content_lista_actividades_ruta").clientHeight - (document.getElementById("divcabecer2_lista_actividades_ruta").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#div_gred_actividades').css("height", (document.getElementById("contenido_procesa_lista_actividades_ruta").clientHeight - (document.getElementById("contenido_titulo_data_grid_lista_actividades_ruta").clientHeight + document.getElementById("div_contenido_procesa_lista_actividades_ruta_botones_desicion").clientHeight)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_form_control_person " + err.message);
    }
}

function auto_zise_popup_lista_form_control_person_procent(apost_name_content,porcent_height) {
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

        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * porcent_height) / 100);  // Indica el porcentaje de espacio vertical del elemento
        let control_panel = document.getElementById("Panel_" + apost_name_content);
        let heigth_panel = 1;
        if (control_panel) {
            $('#' + control_panel.id).css("height", (heig_porcent) + "px"); //Asigna altura al panel contenedor del modal
            heigth_panel = control_panel.clientHeight;
            //heigth_panel = document.getElementById(control_panel.id).clientHeight;
        }
        let control_modal_foter = document.getElementById("modal_foter_" + apost_name_content);
        let heigth_modal_foter = 1;
        if (control_modal_foter) {
            heigth_modal_foter = control_modal_foter.clientHeight;
        }
        let control_modal = document.getElementById("modal_content_" + apost_name_content);
        let heigth_modal = 1;
        if (control_modal) {
            $('#' + control_modal.id).css("height", (heig_porcent - heigth_modal_foter) + "px"); //Asigna altura al panel contenedor del modal
            heigth_modal = control_modal.clientHeight;
        }

        let control_title = document.getElementById("title_" + apost_name_content);
        let heigth_title = 1;
        if (control_title) {
            heigth_title = control_title.clientHeight;
        }

        let control_contenido = document.getElementById("contenido_procesa_" + apost_name_content);
        let heigth_contenido = 1;
        if (control_contenido) {
            $('#' + control_contenido.id).css("height", (heigth_modal - (heigth_title)) + "px");   //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
            heigth_contenido = control_contenido.clientHeight;
        }
        let control_div = document.getElementById("div_" + apost_name_content);
        let heigth_div = 1;
        if (control_div) {
            $('#' + control_div.id).css("height", (heigth_contenido - 5) + "px"); //Asigna altura del control padre
            heigth_div = control_div.clientHeight;
        }
        //$('#modal_content_lista_actividades_ruta').css("height", (heig_porcent - 3) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
        //$('#contenido_procesa_lista_actividades_ruta').css("height", (document.getElementById("modal_content_lista_actividades_ruta").clientHeight - (document.getElementById("divcabecer2_lista_actividades_ruta").clientHeight)) + "px");
        //Para los modal que contiene gred
        //$('#div_gred_actividades').css("height", (document.getElementById("contenido_procesa_lista_actividades_ruta").clientHeight - (document.getElementById("contenido_titulo_data_grid_lista_actividades_ruta").clientHeight + document.getElementById("div_contenido_procesa_lista_actividades_ruta_botones_desicion").clientHeight)) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_popup_lista_form_control_person " + err.message);
    }
}

