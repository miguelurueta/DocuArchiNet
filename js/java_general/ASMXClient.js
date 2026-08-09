//Configuración inicial

const AsmxServicesConfig = {
    Radicacion: "../webservice/WebServiceRadicacion.asmx/",
    RadicacionSimplificada: "../webservice/WebService_radicacion_Simplificada.asmx/",
    ConfigDigitaliacion: "../webservice/WebService_Config_Digitalizacion.asmx/",
    Workflow: "../webservice/WebServiceWorkflow.asmx/",
    Docuarchi: "../webservice/WebServiceDocuarchi.asmx/"

};
/* 
// 1. Servicio de digitalización
const resp1 = await client
    .use("digitalizacion")
    .call("ServiceSolicitaEstructuraConfiguracion", { IdTipoTramite: 123 });

if (resp1.error) {
    console.error("❌ Error digitalización:", resp1.message);
} else {
    console.log("✅ Estructura:", resp1.data);
}

// 2. Servicio de usuario
const resp2 = await client
    .use("usuario")
    .call("GetUsuarioById", { IdUsuario: 45 });

console.log(resp2);

// 3. Servicio workflow
const resp3 = await client
    .use("workflow")
    .call("GetTareasPendientes", { IdUsuario: 45, Estado: "Pendiente" });

console.log(resp3);
*/

class ASMXClient {
    constructor(config = {}) {
        this.services = config;   // { nombre: urlBase }
        this.currentBaseUrl = null;
        this.globalOptions = {
            delay: 300,
            minChars: 3,
            maxResults: 10,
            urlOverride: null
        };
    }

    use(serviceName) {
        if (!this.services[serviceName]) {
            throw new Error(`Servicio '${serviceName}' no está configurado`);
        }
        this.currentBaseUrl = this.services[serviceName];
        return this;
    }

    async call(serviceName, parameters = {}) {
        if (!this.currentBaseUrl) {
            throw new Error("Debes seleccionar un servicio con .use(nombreServicio) antes de llamar");
        }

        try {
            const payload = typeof parameters === "object"
                ? parameters
                : { value: String(parameters) };

            const response = await fetch(this.currentBaseUrl + serviceName, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                return { error: true, status: response.status, message: response.statusText };
            }

            const responsejson = await response.json();
            const result = responsejson?.d;
            if (!result) {
                return { error: true, status: response.status, message: "Respuesta vacía del servicio" };
            }

            const errorMessage = result[0].AppError || result[0].ErrorMensaje ||
                result[0].error_gestion || result[0].error_sistema ||
                result[0].Error_result; 

            if (errorMessage != "YES") {
                return { error: true, message: errorMessage, data: result, raw: responsejson, status: "" };
            }

            return {
                error: false,
                message: result[0].Message || "YES",
                data: result,
                raw: responsejson
            };

        } catch (err) {
            return { error: true, message: err.message };
        }
    }

    // 🔹 Mostrar spinner pequeño al lado del input
    showSpinner(inputEl) {
        this.removeSpinner(inputEl); // por si ya había uno

        const spinner = document.createElement("span");
        spinner.className = "asmx-spinner";
        spinner.style.cssText = `
            margin-left: 5px;
            border: 2px solid #ccc;
            border-top: 2px solid #333;
            border-radius: 50%;
            width: 14px;
            height: 14px;
            display: inline-block;
            animation: spin 0.8s linear infinite;
        `;

        inputEl.insertAdjacentElement("afterend", spinner);
    }

    // 🔹 Quitar spinner
    removeSpinner(inputEl) {
        const nextEl = inputEl.nextElementSibling;
        if (nextEl && nextEl.classList.contains("asmx-spinner")) {
            nextEl.remove();
        }
    }

    autoCompleteNative(serviceName, inputId, methodName, extraParams = {}, onSelect = null, options = {}) {
        const config = { ...this.globalOptions, ...options };
        const inputEl = document.getElementById(inputId);
        if (!inputEl) return console.error(`No se encontró input con id ${inputId}`);

        let dataListId = inputId + "_list";
        let dataList = document.getElementById(dataListId);
        if (!dataList) {
            dataList = document.createElement("datalist");
            dataList.id = dataListId;
            document.body.appendChild(dataList);
            inputEl.setAttribute("list", dataListId);
        }

        // Función debounce para limitar las solicitudes
        function debounce(callback, wait) {
            let timeout;
            return (...args) => {
                clearTimeout(timeout);
                timeout = setTimeout(() => {
                    callback(...args);
                }, wait);
            };
        }

        // Función para mostrar el spinner
        const showSpinner = (inputEl) => {
            const spinner = document.createElement("div");
            spinner.className = "spinner";
            inputEl.parentElement.appendChild(spinner); // Añadimos el spinner al input
        };

        // Función para remover el spinner
        const removeSpinner = (inputEl) => {
            const spinner = inputEl.parentElement.querySelector(".spinner");
            if (spinner) {
                spinner.remove();
            }
        };

        // Función para obtener los datos del autocompletado
        const fetchData = async () => {
            const term = inputEl.value;
            if (term.length < config.minChars) return;

            const payload = {
                NameDbsAuto: extraParams.NameDbsAuto || "",
                NameTableAuto: extraParams.NameTableAuto || "",
                NameCampoAuto: extraParams.NameCampoAuto || "",
                IdTable: extraParams.IdTable || "0",
                Value: term,
                FechaConsulta: new Date().toISOString()
            };

            try {
                showSpinner(inputEl); // 🔹 Mostrar el spinner mientras se hace la solicitud
                const response = await fetch(this.currentBaseUrl + methodName, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ parameter: payload })
                });

                const json = await response.json();
                const items = json?.d?.[0]?.country || [];
                console.log(items);
                // Limpiar el datalist antes de agregar nuevos resultados
                dataList.innerHTML = "";

                items.slice(0, config.maxResults).forEach(item => {
                    const option = document.createElement("option");
                    option.value = item;
                    dataList.appendChild(option);
                });

            } catch (err) {
                console.error("Error en autocomplete:", err);
            } finally {
                removeSpinner(inputEl); // 🔹 Remover el spinner después de la solicitud
            }
        };

        // Agregar el evento 'input' para activar la búsqueda
        inputEl.addEventListener("input", debounce(fetchData, config.delay));

        // Evento 'change' para manejar la selección de un valor del datalist
        inputEl.addEventListener("change", () => {
            if (typeof onSelect === "function") {
                onSelect(inputEl.value, inputEl);
            }

            // Después de la selección, cerramos el datalist y limpiamos el valor
            if (inputEl.value) {
                inputEl.blur(); // Perdemos el foco para evitar el despliegue del datalist
            }
        });
    }

}


