class JSProgresBar {
	constructor(options = {}) {
		let defaults = {
			name_service: "",              //Parametro que representa el nombre del servicio
			OptionItemSelect: Object,      //Paramentro que representa la lista de elementos seleccionados
			mensaje_title: "",             //Paramentro que representa el titulo de progrebar
			NameControlPadreProgres: "",   //Paramentro que representa el contendor del progrebar
			NameProceso: "",               //Paramentro que representa el nombre del proceso en el progresbar
			IdtipoDocumentalTrd: 0,        //Paramentro que representa la identiifcacion de la tipologia trd
			IdTramite: 0,                  //Paramentro que representa la identiifcacion del tramite tabla tipo tramite
			ObjectComponente: Object       //Paramentro que representa el componente o clase donde se ecuentra la funcion 
		}
		this.settings = $.extend(true, defaults, options);
		this._ModalPorgresBarr;
		this._BtnoCancel;
		this._ModalCancelProgress;
		this._BtnoCancelProces;
		this._BtnoContinuarProces;
		this._BtnoConfirmCancelProces;
		this._BtnoConfirmContinuarProces;
		this.estado_control = 1;
		this.NameContendorProgress = "name_contenedor_progress_0001";
		this.NameContenedorCancel = "name_contenedor_cancel_0001";
		this.NameContendorConfirmProgress = "name_contenedor_progress_confirm_0001";
		this.ErrorConfirmProgres = "";
		this.OptionLength = 0;
		this.valuePercent = 1;
		this.NumeroElmentNoProcess = 0;
		this.ErrorElmentNoProcess = 0;
		this.Conteo = 1;
		this.ResultadoPeocesing = {
			Value: "YES",
			ErrorProcecing: ""
		};
	}
	/**
	 * 
	 * @param {any} OptionItemSelect
	 */
	async *_GeneraProcesingProgres() {
		try {
			let lengOption = this.settings.OptionItemSelect.length - 1;
			let result = "";
			let i = 0;
			for (i = 0; i <= lengOption; i++) {
				if (this.estado_control == 0) {
					this.ResultadoPeocesing.Value = 'YES';
					this.ResultadoPeocesing.ErrorProcecing = "Prueba de proceso";
					yield this.ResultadoPeocesing;
				}
				this.valuePercent = (i + 1);
				let r = await this._SetPorcentProgres(this.valuePercent, this.OptionLength, this.settings.NameProceso, "");
				/*Registra y vincula sello de inscripcion */
				if (this.settings.name_service == "ServiceRegistraExpeidenteSIIVincula") {
					result = await ServiceRESTviculaDocumentoExpediente(this.settings.OptionItemSelect[i]);
					this.ResultadoPeocesing.Value = result;
					this.ResultadoPeocesing.ErrorProcecing = "";
					yield this.ResultadoPeocesing;
				}
				/*Guarda sellos o constancias de inscripción de integración  SII ServiceRESTGuardaConstanciaInscripcionSII*/
				if (this.settings.name_service == "ServiceGuardaSelloSII") {
					result = await ServiceRESTGuardaConstanciaInscripcionSII(this.settings.OptionItemSelect[i], this.settings.IdtipoDocumentalTrd, this.settings.IdTramite);
					this.ResultadoPeocesing.Value = result;
					this.ResultadoPeocesing.ErrorProcecing = "";
					yield this.ResultadoPeocesing;
				}
				/*firma_digital_andes_001*/
				if (this.settings.name_service == "firma_digital_andes_001") {
					result = await LoadStampFile(this.settings.OptionItemSelect[i]);
					if (result == "YES") {
						this.ResultadoPeocesing.Value = result;
						this.ResultadoPeocesing.ErrorProcecing = "";
						yield this.ResultadoPeocesing;
					} else {
						if (result == "El archivo ya ha sido firmado digitalmente. No es posible realizar más modificaciones o firmar nuevamente.") {
							this.ResultadoPeocesing.Value = "YES";
							this.ResultadoPeocesing.ErrorProcecing = "YES";
							yield this.ResultadoPeocesing;
						} else {
							this.ResultadoPeocesing.Value = result;
							this.ResultadoPeocesing.ErrorProcecing = result;
							yield this.ResultadoPeocesing;
						}
					}
				}
				/*Carga los archivos al servidor desde el dispositivo*/
				if (this.settings.name_service == "EnviaArchivoServidor") {
					result = await this.settings.ObjectComponente._EnviaArchivoServidor(this.settings.OptionItemSelect[i]);
					this.ResultadoPeocesing.Value = result;
					this.ResultadoPeocesing.ErrorProcecing = "";
					yield this.ResultadoPeocesing;
				}
				/*Actualiza indice batch*/
				if (this.settings.name_service == "ActualizaIndiceBatch") {
					result = await this.settings.ObjectComponente._ActualizaIndiceBatch(this.settings.OptionItemSelect[i]);
					this.ResultadoPeocesing.Value = result;
					this.ResultadoPeocesing.ErrorProcecing = "";
					yield this.ResultadoPeocesing;
				}
				/*Actualiza indice batch   depedencia WebFormDaPrincipal*/
				if (this.settings.name_service == "EliminarDocumentoGabinete") {
					if (this.settings.ObjectComponente == null) {
						result = await ElimnaRegistroDcoumento(this.settings.OptionItemSelect[i]);
					} else {
						result = await this.settings.ObjectComponente.ElimnaRegistroDcoumento(this.settings.OptionItemSelect[i]);
					}
					this.ResultadoPeocesing.Value = result;
					this.ResultadoPeocesing.ErrorProcecing = "";
					yield this.ResultadoPeocesing;
				}
			}
		} catch (ex) {
			this.ResultadoPeocesing.Value = ex.message;
			this.ResultadoPeocesing.ErrorProcecing = ex.message;
			yield this.ResultadoPeocesing;
		}
	}
	/** Funtion que carga la interfaz del progress y ejecuta las funciones */
	async LoadJSProgresBar() {
		try {
			this.OptionLength = this.settings.OptionItemSelect.length;
			this.valuePercent = 1;
			let DataOptionItemSelect;
			let result = "";
			if (this.settings.OptionItemSelect.length == 0) {
				return "¡Atención! No has seleccionado todos los ítems. Por favor, asegúrate de completar tu selección antes de continuar.";
			}
			result = await this._ModalShowJSProgresBar();
			if (result != "YES") {
				return result;
			}
			for await (let ResultadoPeocesing of this._GeneraProcesingProgres()) {
				/*Sale del for cuando se igual al contador*/
				if (this.OptionLength == this.valuePercent) {
					this._ModalHideProgress();
					this._ModalHideCancelProgress();
					this._ModalHideCancelConfirmProgress();
					if (ResultadoPeocesing.Value == "CTRLRETURN") {
						ResultadoPeocesing.Value = ResultadoPeocesing.ErrorProcecing;
					}
					return ResultadoPeocesing.Value;
					break;
				}
				/*Cancela el for*/
				if (this.estado_control == 0) {
					ResultadoPeocesing.Value = "YES";
					break;
				}
				/*Pausa el for por confirnación*/
				if (this.estado_control == 2) {
					result = await this._PendinSYNC();
				}
				/*Captura un error con decision de continuar*/
				if (ResultadoPeocesing.Value == "CTRLRETURN") {
					this.estado_control = 2;
					this.ErrorConfirmProgres = ResultadoPeocesing.ErrorProcecing;
					this._ModalShowConfirm();
					this.NumeroElmentNoProcess++;
					this.ErrorElmentNoProcess = this.valuePercent;
					result = await this._PendinSYNC();
				}
				/*Captura un error controlado*/
				if (ResultadoPeocesing.Value == "CTRL") {
					this.NumeroElmentNoProcess++;
					this.ErrorElmentNoProcess = this.valuePercent;
				}
				/*Sale del for error no controlado*/
				if (ResultadoPeocesing.Value != "YES" && ResultadoPeocesing.Value != "CTRL" & ResultadoPeocesing.Value != "CTRLRETURN") {
					this._ModalHideProgress();
					this._ModalHideCancelProgress();
					this._ModalHideCancelConfirmProgress();
					return ResultadoPeocesing.Value;
					break;
				}
			}
			this._ModalHideProgress();
			this._ModalHideCancelProgress();
			this._ModalHideCancelConfirmProgress();
			return this.ResultadoPeocesing.Value;
		} catch (ex) {
			return ex.message;
		}
	}
	async _PruebaaSYNC() {
		return new Promise(resolve => {
			setTimeout(() => {
				this.ResultadoPeocesing.Value = 'YES';
				this.ResultadoPeocesing.ErrorProcecing = "Pruba de proceso";
				resolve(this.ResultadoPeocesing);
			}, 1000); // Simulate a random delay up to 5000 ms (5 seconds).
		});
	}
	async _PendinSYNC() {
		return new Promise(resolve => {
			setInterval(() => {
				if (this.estado_control != 2) {
					this.ResultadoPeocesing.Value = 'YES';
					this.ResultadoPeocesing.ErrorProcecing = "";
					resolve(this.ResultadoPeocesing);
				}
			}, 1);
		});
	}

	_ModalHideProgress() {
		if (document.getElementById(this.NameContendorProgress)) {
			let element = document.getElementById(this.NameContendorProgress);
			element.remove();
		}
	}
	_ModalHideCancelProgress() {
		if (document.getElementById(this.NameContenedorCancel)) {
			let element = document.getElementById(this.NameContenedorCancel);
			element.remove();
		}
	}
	_ModalHideCancelConfirmProgress() {
		if (document.getElementById(this.NameContendorConfirmProgress)) {
			let element = document.getElementById(this.NameContendorConfirmProgress);
			element.remove();
		}
	}
	/*Move control progress barr*/
	async _SetPorcentProgres(valuePercent, ValueLeng, NameProceso, ElementProceso) {
		let porcent = Math.floor((valuePercent / ValueLeng) * 100);
		const elementProgres = document.getElementById("progres_bar_0000001");
		if (elementProgres) {
			elementProgres.style.width = porcent + "%";
			elementProgres.innerText = porcent + "%";
		}
		const ElmentNameProceso = document.getElementById("label_proces_progres_bar_0000001");
		if (ElmentNameProceso) {
			ElmentNameProceso.innerText = NameProceso;
		}
		const ElementProceso_ = document.getElementById("label_element_progres_bar_0000001");
		if (ElementProceso_) {
			ElementProceso_.innerText = "Procesando : " + ElementProceso;
		}
		const ElmentConteoBar = document.getElementById("label_element_conteo_progres_bar_0000001");
		if (ElmentConteoBar) {
			ElmentConteoBar.innerText = valuePercent + " de " + (ValueLeng);
		}
	}
	/**Show modal progress */
	async _ModalShowJSProgresBar() {
		try {
			if (document.getElementById(this.NameContendorProgress)) {
				let element = document.getElementById(this.NameContendorProgress);
				element.remove();
			}
			const wrapper = document.createElement('div');
			wrapper.id = this.NameContendorProgress;
			wrapper.innerHTML = [
				'<div class="modal fade modal_opacity  " style="z-index:100062" id="modal_show_progre_barr_001" role="dialog" aria-hidden="false" data-backdrop="false">',
				'<div class="modal-dialog modal-dialog-centered">',
				'<div class="modal-content">',
				'<div class="modal-header d-none">',
				'<h5 class="modal-title" id="staticBackdropLabel"></h5>',
				'<button type="button" class="close" data-dismiss="modal" aria-label="Close">',
				'<span aria-hidden="true">&times;</span>',
				'</button>',
				'</div>',
				'<div class="modal-body">',
				'<div class="d-flex justify-content-center">',
				'<div class="h6" id="label_proces_progres_bar_0000001"> </div>',
				'</div>',
				'<div class="d-flex justify-content-center">',
				'<div class="h6" id="label_element_progres_bar_0000001">  </div>',
				'</div>',
				'<div class="d-flex justify-content-center">',
				'<div class="h6" id="label_element_conteo_progres_bar_0000001">  </div>',
				'</div>',
				'<div class="progress" role="progressbar" aria-label="Success example" aria-valuenow="25" aria-valuemin="0" aria-valuemax="100">',
				'<div class="progress-bar bg-success" id="progres_bar_0000001" style="width: 0%"></div>',
				'</div>',
				'</div>',
				'<div class="modal-footer d-flex justify-content-center">',
				'<button type="button" id="butoon_cancel_progre_barr_001" class="btn btn-secondary" data-dismiss="modal_">Cancelar</button>',
				'</div>',
				'</div>',
				'</div>',
				'</div>'
			].join('')
			let content = document.getElementById(this.settings.NameControlPadreProgres);
			if (content) {
				content.append(wrapper);
			}
			this._BtnoCancel = document.getElementById("butoon_cancel_progre_barr_001");
			this._BtnoCancel.addEventListener("click", this._SowCancelProgress, false);
			this._ModalPorgresBarr = $("#modal_show_progre_barr_001");
			this._ModalPorgresBarr.modal("show");
			return "YES";
		}
		catch (ex) {
			return ex.message;
		}
	}
	/**Show modal cancelar */
	async _ModalShowCancel() {
		if (document.getElementById(this.NameContenedorCancel)) {
			let element = document.getElementById(this.NameContenedorCancel);
			element.remove();
		}
		const wrapper = document.createElement('div');
		wrapper.id = this.NameContenedorCancel;
		wrapper.innerHTML = [
			'<div class="modal fade modal_opacity " style="z-index:100068" id="modal_show_cancel_001" role="dialog" aria-hidden="false" data-backdrop="false">',
			'<div class="modal-dialog modal-dialog-centered">',
			'<div class="modal-content">',
			'<div class="modal-header">',
			'<h5 class="modal-title" id="staticBackdropLabelCancel"></h5>',
			'<button type="button" class="close d-none" data-dismiss="modal" aria-label="Close">',
			'<span aria-hidden="true">&times;</span>',
			'</button>',
			'</div>',
			'<div class="modal-body">',
			'<div class="h6"> Hemos procesado ' + this.valuePercent + '  de ' + this.OptionLength + ' elemento(s) ¿Está seguro que desea cancelar el proceso? </div>',
			'<div class="h6">  </div>',
			'</div>',
			'<div class="modal-footer d-flex justify-content-end">',
			'<button type="button" id="butoon_continuar_pross_001" class="btn btn-primary" >Continuar</button>',
			'<button type="button" id="butoon_cancel_aceptar_001" class="btn btn-secondary">Cancelar</button>',
			'</div>',
			'</div>',
			'</div>',
			'</div>'
		].join('')
		let content = document.getElementById(this.settings.NameControlPadreProgres);
		if (content) {
			content.append(wrapper);
		}
		this._BtnoCancelProces = document.getElementById("butoon_cancel_aceptar_001");
		this._BtnoCancelProces.addEventListener("click", this._CancelProgressBarr, false);
		this._BtnoContinuarProces = document.getElementById("butoon_continuar_pross_001");
		this._BtnoContinuarProces.addEventListener("click", this._ContinuarProgrres, true);
		this._ModalCancelProgress = $("#modal_show_cancel_001");
		this._ModalCancelProgress.modal("show");
		this.estado_control = 2; //Pone en estado de espera el progress
		return "YES";
	}
	/**Show modal confirmar error controlado*/
	async _ModalShowConfirm() {
		if (document.getElementById(this.NameContendorConfirmProgress)) {
			let element = document.getElementById(this.NameContendorConfirmProgress);
			element.remove();
		}
		const wrapper = document.createElement('div');
		wrapper.id = this.NameContendorConfirmProgress;
		wrapper.innerHTML = [
			'<div class="modal fade modal_opacity " style="z-index:100068" id="modal_show_confirm_001" role="dialog" aria-hidden="false" data-backdrop="false">',
			'<div class="modal-dialog modal-dialog-centered">',
			'<div class="modal-content">',
			'<div class="modal-header">',
			'<h5 class="modal-title" id="staticBackdropLabelConfirm"></h5>',
			'<button type="button" class="close d-none" data-dismiss="modal" aria-label="Close">',
			'<span aria-hidden="true">&times;</span>',
			'</button>',
			'</div>',
			'<div class="modal-body">',
			'<div class="h6"> Hemos detectado el siguiente mensaje del sistema : "' + this.ErrorConfirmProgres + '" ¿Desea cancelar el proceso? </div>',
			'<div class="h6">  </div>',
			'</div>',
			'<div class="modal-footer d-flex justify-content-end">',
			'<button type="button" id="butoon_confirm_continuar_pross_002" class="btn btn-primary" >Continuar</button>',
			'<button type="button" id="butoon_confirm_cancel_pross_002" class="btn btn-secondary">Cancelar</button>',
			'</div>',
			'</div>',
			'</div>',
			'</div>'
		].join('')
		let content = document.getElementById(this.settings.NameControlPadreProgres);
		if (content) {
			content.append(wrapper);
		}

		this._BtnoConfirmCancelProces = document.getElementById("butoon_confirm_cancel_pross_002");
		this._BtnoConfirmCancelProces.addEventListener("click", this._CancelConfirmCancelProgressBarr, false);
		this._BtnoConfirmContinuarProces = document.getElementById("butoon_confirm_continuar_pross_002");
		this._BtnoConfirmContinuarProces.addEventListener("click", this._ContinuarConfirmProgrres, true);
		this._ModalCancelProgress = $("#modal_show_confirm_001");
		this._ModalCancelProgress.modal("show");
		this.estado_control = 2; //Pone en estado de espera el progress
		return "YES";
	}
	_SowCancelProgress(event) {
		_JSProgresBar._ModalShowCancel();
	}
	/**
	 * Cancela el progresbar desde la ventana de solicitud de cancelación
	 * @param {any} event
	 */
	_CancelProgressBarr(event) {
		_JSProgresBar.estado_control = 0;
	}
	/**
	 * Cancela el progresbar desde la ventana de error controlado
	 * @param {any} event
	 */
	_CancelConfirmCancelProgressBarr(event) {
		_JSProgresBar.estado_control = 0;
	}
	/** continua el proceso y cierra la ventana de confirnación de cancelación */
	_ContinuarProgrres() {
		_JSProgresBar._ModalHideCancelProgress();
		_JSProgresBar.estado_control = 1;
	}
	/** continua el proceso y cierra la ventana de confirnación de error controlado */
	_ContinuarConfirmProgrres() {
		_JSProgresBar._ModalHideCancelConfirmProgress();
		_JSProgresBar.estado_control = 1;
	}
}



let _JSProgresBar;
const JSProgresBarBoot = async (Option) => { 
	_JSProgresBar = new JSProgresBar(Option);
	let result = await _JSProgresBar.LoadJSProgresBar();
	return result;
}