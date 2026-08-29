const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");

const modelsSource = fs.readFileSync(path.resolve(__dirname, "../Modelo/Workflow/Notas/NotasWorkflowModels.vb"), "utf8");
const interfacesSource = fs.readFileSync(path.resolve(__dirname, "../Modelo/Workflow/Notas/NotasWorkflowInterfaces.vb"), "utf8");
const dtoSource = fs.readFileSync(path.resolve(__dirname, "../DTOs/Workflow/Notas/NotasWorkflowDtos.vb"), "utf8");
const serviceSource = fs.readFileSync(path.resolve(__dirname, "../Services/Workflow/Notas/ServicioNotasWorkflow.vb"), "utf8");
const repositorySource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Repositories/Workflow/MySqlNotasWorkflowRepository.vb"), "utf8");
const contextGateSource = fs.readFileSync(path.resolve(__dirname, "../webservice/WorkflowPreviewSessionContextGate.vb"), "utf8");
const workflowModelsSource = fs.readFileSync(path.resolve(__dirname, "../Modelo/Workflow/Terminar/WorkflowModernModels.vb"), "utf8");
const projectSource = fs.readFileSync(path.resolve(__dirname, "../GestionDocumental-Docuarchi.net.vbproj"), "utf8");

function classBlock(source, name) {
    const match = source.match(new RegExp(`Public Class ${name}[\\s\\S]*?End Class`));
    assert.ok(match, `falta ${name}`);
    return match[0];
}

function withoutComments(source) {
    return source.replace(/^\s*'.*$/gm, "");
}

test("los contratos de Notas exigen tarea explícita y nota cuando corresponde", () => {
    for (const name of [
        "SolicitudListarNotasWorkflow",
        "SolicitudContarNotasWorkflow",
        "SolicitudCrearNotaWorkflow",
        "SolicitudConsultarNotaWorkflow",
        "SolicitudActualizarNotaWorkflow",
        "SolicitudEliminarNotaWorkflow",
    ]) {
        assert.match(classBlock(modelsSource, name), /IdTarea As Long/);
    }
    for (const name of ["SolicitudConsultarNotaWorkflow", "SolicitudActualizarNotaWorkflow", "SolicitudEliminarNotaWorkflow"]) {
        assert.match(classBlock(modelsSource, name), /IdNota As Long/);
    }
    assert.doesNotMatch(withoutComments(modelsSource), /Session|HttpContext|Page|GridView|UpdatePanel/);
    assert.doesNotMatch(withoutComments(dtoSource), /Session|HttpContext|Page|GridView|UpdatePanel|IdRuta|NombreRuta|Tabla/);
});

test("los puertos de Notas reciben contexto y snapshot de tarea, no información del navegador", () => {
    for (const operation of ["Listar", "Contar", "Crear", "Consultar", "Actualizar", "Eliminar"]) {
        assert.match(interfacesSource, new RegExp(`Function ${operation}\\(ByVal contexto As ContextoModuloWorkflow`));
    }
    assert.match(interfacesSource, /ByVal tarea As TareaWorkflow/);
    assert.doesNotMatch(withoutComments(interfacesSource), /Session|HttpContext|Page|GridView|UpdatePanel|IdRuta|NombreRuta|Tabla/);
});

test("el gate de Notas calcula Interactuar_Anotaciones en servidor y falla cerrado", () => {
    const block = contextGateSource.match(/Public Function AsegurarContextoNotas[\s\S]*?End Function/)[0];

    assert.match(workflowModelsSource, /PuedeInteractuarAnotaciones As Boolean/);
    assert.match(block, /SolicitaPermisosUsuarioWorkflow/);
    assert.match(block, /permisos\.Length > 9/);
    assert.match(block, /permisos\(9\)/);
    assert.match(block, /PuedeInteractuarAnotaciones = False/);
    assert.doesNotMatch(block, /ID_TAREA_SELECCIONDA|WorkflowCentroTrabajoModernActive/);
});

test("el servicio valida permiso, tarea y ruta antes de delegar", () => {
    for (const operation of ["Listar", "Contar", "Crear", "Consultar", "Actualizar", "Eliminar"]) {
        assert.match(serviceSource, new RegExp(`Public Function ${operation}\\(`));
    }
    assert.match(serviceSource, /Not contexto\.PuedeInteractuarAnotaciones/);
    assert.match(serviceSource, /_tareaRepository\.ObtenerTarea\(contexto, idTarea\)/);
    assert.match(serviceSource, /tarea\.IdRuta <> contexto\.IdRutaWorkflow/);
    assert.match(serviceSource, /CodigosResultadoNotasWorkflow\.Forbidden/);
    assert.match(serviceSource, /CodigosResultadoNotasWorkflow\.TaskNotActive/);
    assert.match(serviceSource, /CodigosResultadoNotasWorkflow\.InvalidContent/);
    assert.match(serviceSource, /CodigosResultadoNotasWorkflow\.VersionConflict/);
    assert.match(serviceSource, /LongitudMaximaContenido As Integer = 16000/);
    assert.match(serviceSource, /contenido\.Length > LongitudMaximaContenido/);
    assert.match(serviceSource, /ContieneCaracterFueraDelPlanoBasico\(contenido\)/);
    assert.match(serviceSource, /Char\.IsSurrogate\(caracter\)/);
    assert.doesNotMatch(withoutComments(serviceSource), /Session|HttpContext|Page|GridView|UpdatePanel|WebMethod|Class_anotacion_tarea/);
});

test("el repositorio de fundación no consulta ni escribe y queda fail-closed", () => {
    assert.match(repositorySource, /Implements INotasWorkflowRepository/);
    assert.match(repositorySource, /New MySqlParameter/);
    assert.match(repositorySource, /CodigosResultadoNotasWorkflow\.Unavailable/);
    assert.doesNotMatch(repositorySource, /\b(?:SELECT|INSERT|UPDATE|DELETE|CALL)\b/i);
    assert.doesNotMatch(repositorySource, /CreateOpenConnection|ExecuteReader|ExecuteScalar|ExecuteNonQuery|Class_anotacion_tarea/);
});

test("el proyecto incluye los contratos y la base interna de Notas", () => {
    for (const file of [
        "DTOs\\Workflow\\Notas\\NotasWorkflowDtos.vb",
        "Modelo\\Workflow\\Notas\\NotasWorkflowModels.vb",
        "Modelo\\Workflow\\Notas\\NotasWorkflowInterfaces.vb",
        "Services\\Workflow\\Notas\\ServicioNotasWorkflow.vb",
        "Infrastructure\\Repositories\\Workflow\\MySqlNotasWorkflowRepository.vb",
    ]) {
        assert.ok(projectSource.includes(`<Compile Include="${file}" />`), `falta incluir ${file} en el proyecto.`);
    }
});
