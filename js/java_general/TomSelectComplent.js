class TomSelectManager {
    constructor(selectId, seting) {
        this.selectId = selectId;
        this.seting = seting;
        this.tomSelectInstance = null;
        this.init();
    }
    init() {
        this.loadTomSelect();
    }
    buildRequestBody(query) {
        const param = {
            ...this.seting,
            value_auto: query,
            TomParameter: this.seting.TomParameter || []
        };
        return JSON.stringify({ parameter: JSON.stringify([param]) });
    }
    loadTomSelect() {
        const url = this.seting.ur_web_service + "/" + this.seting.name_web_service;
        if (this.tomSelectInstance) {
            this.tomSelectInstance.destroy();
        }
        this.tomSelectInstance = new TomSelect(this.selectId, {
            valueField: "id_value",
            labelField: "tex_value",
            searchField: "tex_value",
            create: this.seting.create ?? false,
            maxItems: this.seting.maxItems ?? 1,
            plugins: ['remove_button'],
            openOnFocus: true,
            mode: this.seting.mode ?? "multi",
            shouldLoad: function (query) {
                return query.length > 0;
            },
            load: async (query, callback) => {
                try {
                    const response = await fetch(url, {
                        method: "POST",
                        body: this.buildRequestBody(query),
                        headers: { "Content-Type": "application/json" }
                    });
                    if (!response.ok) {
                        console.error("Error en fetch:", response.statusText);
                        callback();
                        return;
                    }
                    const json = await response.json();
                    if (json.d[0].error_gestion == "YES") {
                        callback(json.d[0].row_tom);
                    } else {
                        callback();
                    }
                    //callback(json);
                } catch (err) {
                    console.error("Error en loadTomSelect:", err);
                    callback();
                }
            },
            render: {
                option: (data, escape) => {
                    switch (this.seting.case_Option) {
                        case "solicitante":
                            return `
            <div class="py-2 d-flex align-items-center">
                <div>
                    <span class="h5">${escape(data.tex_value)}</span>
                </div>
                <div class="ms-auto">
                    ${["edit_item", "delete_item"].map(action => `
                        <a style="color: black"
                           title="${action === "edit_item" ? "Editar" : "Eliminar"} registro"
                           href="#"
                           onclick="prevent_event(event,this)"
                           idd="${escape(data.id_value)}"
                           id_escript="${escape(this.seting.id_escript)}"
                           tip_event="${action}"
                           class="dropdown-item_ remove font-weight-light active_show_lista_document_actos d-none">
                           <i class="${action === "edit_item" ? "fad fa-edit" : "fas fa-trash-alt"}"></i>
                        </a>
                    `).join("")}
                </div>
            </div>`;
                        case "destinatario":
                            return `
            <div class="py-2 d-flex align-items-center">
                <span class="h5">${escape(data.tex_value)} ${escape(data.text_value_descritipo)}</span>
            </div>`;
                        default:
                            return `<div class="h6">${escape(data.tex_value)}</div>`;
                    }
                },

                item: (data, escape) => {
                    switch (this.seting.case_Item) {
                        case "solicitante":
                            return `
            <div class="active-tom-item pd-3 d-flex align-items-center">
                <span>${escape(data.tex_value)}</span>
                <div class="ms-auto">
                    ${["edit_item", "delete_item"].map(action => `
                        <a style="color: black"
                           title="${action === "edit_item" ? "Editar" : "Eliminar"} registro"
                           href="#"
                           onclick="prevent_event(event,this)"
                           idd="${escape(data.id_value)}"
                           id_escript="${escape(this.seting.id_escript)}"
                           tip_event="${action}"
                           class="dropdown-item_ ps-2 font-weight-light active_show_lista_document_actos">
                           <i class="${action === "edit_item" ? "fad fa-edit" : "fas fa-trash-alt"}"></i>
                        </a>
                    `).join("")}
                </div>
            </div>`;
                        case "destinatario":
                            return `
            <div class="active-tom-item pd-3 d-flex align-items-center">
                <span>${escape(data.tex_value)} &lt;${escape(data.text_value_descritipo)}&gt;</span>
            </div>`;
                        default:
                            return `<div class="active-tom-item">${escape(data.tex_value)}</div>`;
                    }
                },

                option_create: (data, escape) =>
                    `<div class="create">Agregue <strong>${escape(data.input)}</strong>&hellip;</div>`,

                no_results: () => `<div class="no-results">No se encontraron resultados</div>`,
                loading: () => `<div class="spinner"></div>`,
                optgroup_header: (data, escape) =>
                    `<div class="optgroup-header">${escape(data.label)}</div>`,

                not_loading: () => "",
                optgroup: (data) => `<div class="optgroup">${data.options}</div>`,
                dropdown: () => `<div></div>`
            }
        });
    }

    setTomParameters(paramList) {
        this.seting.TomParameter = paramList;
        this.reload();
    }
    updateTomParameter(name_campo, valor) {
        if (!this.seting.TomParameter) this.seting.TomParameter = [];
        const idx = this.seting.TomParameter.findIndex(p => p.name_campo === name_campo);
        if (idx >= 0) {
            this.seting.TomParameter[idx].valor = valor;
        } else {
            this.seting.TomParameter.push({ name_campo, valor });
        }
        this.reload();
    }
    // 🔹 Eliminar todos los tokens
    clearAllTokens() {
        if (this.tomSelectInstance) {
            this.tomSelectInstance.clear();
        }
    }
    // 🔹 Eliminar todos los tokens de ESTE TomSelect
    clearTokens() {
        if (this.tomSelectInstance) {
            this.tomSelectInstance.clear();
        }
    }
    // 🔹 Eliminar un token específico por valor
    removeToken(value) {
        if (this.tomSelectInstance) {
            this.tomSelectInstance.removeItem(value);
        }
    }
    updateUrlBase(newUrl,Service) {
        this.seting.ur_web_service = newUrl;
        this.seting.name_web_service = Service;
        this.reload();
    }
    reload() {
        this.loadTomSelect();
    }
}

class TomSelectGroup {
    constructor(configs) {
        this.managers = {};

        configs.forEach(cfg => {
            this.managers[cfg.name] = new TomSelectManager(cfg.selectId, cfg.config);
        });
    }
    getManager(name) {
        return this.managers[name] || null;
    }
    // obtener por selectId
    getManagerById(selectId) {
        return this.managers.find(m => m.selectId === selectId) || null;
    }
    updateParameter(name, campo, valor) {
        if (this.managers[name]) {
            this.managers[name].updateTomParameter(campo, valor);
        }
    }
    updateUrl(name, newUrl, Service) {
        if (this.managers[name]) {
            this.managers[name].updateUrlBase(newUrl, Service);
        }
    }
    // 🔹 Limpiar todos los tokens de TODOS los managers
    clearAllTokens() {
        Object.values(this.managers).forEach(manager => {
            if (manager && manager.tomSelectInstance) {
                manager.clearTokens();
            }
        });
    }

    // 🔹 Limpiar tokens de un manager específico
    clearTokensByName(name) {
        const manager = this.getManager(name);
        if (manager && manager.tomSelectInstance) {
            manager.clearTokens();
        }
    }

    // 🔹 Eliminar un token específico en un manager
    removeTokenByName(name, id) {
        const manager = this.getManager(name);
        if (manager && manager.tomSelectInstance) {
            manager.removeTokenById(id);
        }
    }
    
}
// --- Uso ---
/*document.addEventListener("DOMContentLoaded", () => {
    const configs = [
        {
            name: "usuarios",
            selectId: "#selectUsuarios",
            config: {
                dbms: "SQLServer",
                name_plantilla_validacion: "plantillaUsuarios",
                campo_nombre: "nombre",
                campo_primary: "id",
                ur_web_service: "http://localhost:3000",
                name_web_service: "usuarios",
                TomParameter: [{ name_campo: "estado", valor: "activo" }],
                create: false,
                maxItems: 1
            }
        },
        {
            name: "roles",
            selectId: "#selectRoles",
            config: {
                dbms: "MySQL",
                name_plantilla_validacion: "plantillaRoles",
                campo_nombre: "rol",
                campo_primary: "id",
                ur_web_service: "http://localhost:3000",
                name_web_service: "roles",
                TomParameter: [],
                create: true,
                maxItems: 3
            }
        },
        {
            name: "ciudades",
            selectId: "#selectCiudades",
            config: {
                dbms: "Postgres",
                name_plantilla_validacion: "plantillaCiudades",
                campo_nombre: "ciudad",
                campo_primary: "id",
                ur_web_service: "http://localhost:3000",
                name_web_service: "ciudades",
                TomParameter: [],
                create: false,
                maxItems: 1
            }
        },
        {
            name: "departamentos",
            selectId: "#selectDepartamentos",
            config: {
                dbms: "Oracle",
                name_plantilla_validacion: "plantillaDepartamentos",
                campo_nombre: "departamento",
                campo_primary: "id",
                ur_web_service: "http://localhost:3000",
                name_web_service: "departamentos",
                TomParameter: [],
                create: false,
                maxItems: 1
            }
        }
    ];

    const group = new TomSelectGroup(configs);

    // 👉 Ejemplo: actualizar parámetro en "roles" después de 5s
    ///setTimeout(() => {
    //    group.updateParameter("roles", "nivel", "admin");
    //}, 5000);

    // 👉 Ejemplo: cambiar URL base en "ciudades" después de 10s
    //setTimeout(() => {
    //    group.updateUrl("ciudades", "https://api.midominio.com");
    //}, 10000);
});*/
const Tom_Set_item_Aray = (control_tom) => {
    let get_ = control_tom.getValue(1);
    let Item = new Array();
    for (i = 0; i <= get_.length - 1; i++) {
        let get = control.getItem(get_[i]);
        Item.push({ id_value: get.attributes["data-value"].value, tex_value: get.textContent });
    }
    return Item;
}
function Seting_Tom_Select_Option(name_control, ur_web_service, name_web_service, create, maxItems, maxOptions,
    campo_nombre, campo_primary, name_plantilla_validacion, dbms, id_script, case_Option, case_Item, TomParameter) {
    this.name_control = name_control;
    this.ur_web_service = ur_web_service;
    this.name_web_service = name_web_service;
    this.create = create;
    this.maxItems = maxItems;
    this.maxOptions = maxOptions;
    this.campo_nombre = campo_nombre;
    this.campo_primary = campo_primary;
    this.name_plantilla_validacion = name_plantilla_validacion;
    this.dbms = dbms;
    this.id_script = id_script;
    this.case_Option = case_Option;
    this.case_Item = case_Item;
    this.TomParameter = TomParameter;
}
const load_Tom_Select_API_Rest = (Seting) => {
    new TomSelect('#' + Seting.name_control, {
        valueField: 'id_value',
        labelField: 'tex_value',
        searchField: 'tex_value',
        create: Seting.create,
        maxItems: Seting.maxItems,
        plugins: ['remove_button'],
        openOnFocus: true,
        shouldLoad: function (query) {
            return query.length > 0;
        },

        // fetch remote data
        load: function (query, callback) {
            let parameter = new Array();
            let RTomParameter = Seting.TomParameter;
            if (!RTomParameter) {
                RTomParameter = [];
            }
            parameter.push({
                name_dbs_auto: Seting.dbms,
                name_plantilla_validacion: Seting.name_plantilla_validacion,
                campo_nombre_plantilla_val: Seting.campo_nombre,
                campo_primary_plantilla_val: Seting.campo_primary,
                value_auto: query, TomParameter: RTomParameter
            });
            var url = Seting.ur_web_service + "/" + Seting.name_web_service;
           
            fetch(url, {
                method: 'POST',
                body: "{" + "'parameter':'" + JSON.stringify(parameter) + "'}", // tu data JSON se convierte en TEXTO
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(json => {
                    if (json.d[0].error_gestion == "YES") {
                        callback(json.d[0].row_tom);
                    } else {
                        callback();
                    }
                }).catch(() => {
                    callback();
                });

        },
        render: {
            option: function (data, escape) {
                switch (Seting.case_Option) {
                    case "solicitante":
                        return `<div class="py-2 row d-flex">
							<div class="mb-1 col-8">
								<span class="h5">
									${escape(data.tex_value)}
								</span>
							</div>
					 		<div class="ms-auto col-2">
                                <a style="color: black" title="Editar registro " href="#" onclick="prevent_event(event,this)" idd=${escape(data.id_value)} id_escript=${escape(Seting.id_script)} tip_event="edit_item" class="dropdown-item_ remove font-weight-light active_show_lista_document_actos d-none" >
                                <i class="fad fa-edit"></i>
                                </a>
                                <a style="color: black" title="Eliminar registro " href="#" onclick="prevent_event(event,this)" idd=${escape(data.id_value)} tip_event="delete_item" id_escript=${escape(Seting.id_script)} class="dropdown-item_ remove font-weight-light active_show_lista_document_actos d-none" >
                                <i class="fas fa-trash-alt"></i>
                                </a>
                           </div>
						</div>`;
                        break;
                    case "destinatario":
                        return `<div class="py-2 d-flex">
							<div class="mb-1">
								<span class="h5">
									${escape(data.tex_value)}
								</span>
                                <span class="h5 pl-1">
									${escape(data.text_value_descritipo)}
								</span>
							</div>
					 		
						</div>`;
                        break;
                    default:
                        return '<div class="h6">' + escape(data.tex_value) + '</div>';
                }

                //return '<div>' + escape(data.tex_value) + '</div>';

            },
            item: function (data, escape) {
                switch (Seting.case_Item) {
                    case "solicitante":
                        return `<div class="active-tom-item pd-3"> ${escape(data.tex_value)}  <a style="color: black" title="Editar registro " href="#" onclick="prevent_event(event,this)" idd=${escape(data.id_value)} id_escript=${escape(Seting.id_script)} tip_event="edit_item" remove class="dropdown-item_ pl-1 font-weight-light active_show_lista_document_actos" >
                                <i class="fad fa-edit"></i>
                                </a>
                                <a style="color: black" title="Eliminar registro " href="#" onclick="prevent_event(event,this)" idd=${escape(data.id_value)} tip_event="delete_item" id_escript=${escape(Seting.id_script)} class="dropdown-item_ pl-1 remove font-weight-light active_show_lista_document_actos" >
                                <i class="fas fa-trash-alt"></i>
                                </a>  </div >`;
                        break;
                    case "destinatario":
                        //return `<div > ${escape(data.tex_value)}  </div >  <div class="ms-auto "> ${escape(data.text_value_descritipo)}</div>`;
                        return `<div class="py-2_ d-flex_ active-tom-item active-tom-item pd-3">
							<div class="">
								<span>
									${escape(data.tex_value)} ${escape(" <" + data.text_value_descritipo + ">")}
								</span>
                                
							</div>
					 		
						</div>`;
                        break;
                    default:
                        return '<div class="active-tom-item">' + escape(data.tex_value) + '</div>';
                }

            },
            option_create: function (data, escape) {
                return '<div class="create">Agregue <strong>' + escape(data.input) + '</strong>&hellip;</div>';
            },
            no_results: function (data, escape) {

            },
            not_loading: function (data, escape) {
                // no default content
            },
            optgroup: function (data) {
                let optgroup = document.createElement('div');
                optgroup.className = 'optgroup';
                optgroup.appendChild(data.options);
                return optgroup;
            },
            optgroup_header: function (data, escape) {
                return '<div class="optgroup-header">' + escape(data.label) + '</div>';
            },
            loading: function (data, escape) {
                return '<div class="spinner"></div>';
            },
            dropdown: function () {
                return '<div></div>';
            }
        }

    });
}