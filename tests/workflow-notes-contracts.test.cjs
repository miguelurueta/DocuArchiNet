const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");

const read = (...parts) => fs.readFileSync(path.resolve(__dirname, "..", ...parts), "utf8");
const modelsSource = read("Modelo", "Workflow", "Notas", "NotasWorkflowModels.vb");
const interfacesSource = read("Modelo", "Workflow", "Notas", "NotasWorkflowInterfaces.vb");
const dtoSource = read("DTOs", "Workflow", "Notas", "NotasWorkflowDtos.vb");
const serviceSource = read("Services", "Workflow", "Notas", "ServicioNotasWorkflow.vb");
const repositorySource = read("Infrastructure", "Repositories", "Workflow", "MySqlNotasWorkflowRepository.vb");
const cursorSource = read("Infrastructure", "Workflow", "Notas", "NotasWorkflowCursorCodec.vb");
const notesAsmxSource = read("webservice", "WebServiceWorkflowNotesModern.asmx.vb");
const notesAsmxMarkup = read("webservice", "WebServiceWorkflowNotesModern.asmx");
const contextGateSource = read("webservice", "WorkflowPreviewSessionContextGate.vb");
const workflowModelsSource = read("Modelo", "Workflow", "Terminar", "WorkflowModernModels.vb");
const projectSource = read("GestionDocumental-Docuarchi.net.vbproj");

function classBlock(source, name) {
    const match = source.match(new RegExp(`Public Class ${name}[\\s\\S]*?End Class`));
    assert.ok(match, `falta ${name}`);
    return match[0];
}

function withoutComments(source) {
    return source.replace(/^\s*'.*$/gm, "");
}

test("los contratos de lectura exigen tarea explícita, nota cuando corresponde y no reciben contexto del navegador", () => {
    for (const name of ["SolicitudListarNotasWorkflow", "SolicitudContarNotasWorkflow", "SolicitudConsultarNotaWorkflow"]) {
        assert.match(classBlock(modelsSource, name), /IdTarea As Long/);
    }
    assert.match(classBlock(modelsSource, "SolicitudConsultarNotaWorkflow"), /IdNota As Long/);
    assert.match(classBlock(modelsSource, "SolicitudListarNotasWorkflow"), /FechaCursorUtc As Nullable\(Of DateTime\)/);
    assert.match(dtoSource, /Public Property Exito As Boolean/);
    assert.match(dtoSource, /Public Property CodigoBloqueo As String/);
    assert.doesNotMatch(withoutComments(modelsSource), /Session|HttpContext|Page|GridView|UpdatePanel/);
    assert.doesNotMatch(withoutComments(dtoSource), /Session|HttpContext|Page|GridView|UpdatePanel|IdRuta|NombreRuta|Tabla/);
});

test("los puertos usan contexto y tarea confiables, incluido cursor protegido", () => {
    for (const operation of ["Listar", "Contar", "Crear", "Consultar", "Actualizar", "Eliminar"]) {
        assert.match(interfacesSource, new RegExp(`Function ${operation}\\(ByVal contexto As ContextoModuloWorkflow`));
    }
    assert.match(interfacesSource, /ByVal tarea As TareaWorkflow/);
    assert.match(interfacesSource, /Public Interface INotasWorkflowCursorCodec/);
    assert.doesNotMatch(withoutComments(interfacesSource), /Session|HttpContext|Page|GridView|UpdatePanel|IdRuta|NombreRuta|Tabla/);
});

test("el gate de Notas calcula el permiso en servidor y falla cerrado", () => {
    const block = contextGateSource.match(/Public Function AsegurarContextoNotas[\s\S]*?End Function/)[0];
    assert.match(workflowModelsSource, /PuedeInteractuarAnotaciones As Boolean/);
    assert.match(block, /SolicitaPermisosUsuarioWorkflow/);
    assert.match(block, /permisos\.Length > 9/);
    assert.match(block, /permisos\(9\)/);
    assert.match(block, /PuedeInteractuarAnotaciones = False/);
    assert.doesNotMatch(block, /ID_TAREA_SELECCIONDA|WorkflowCentroTrabajoModernActive/);
});

test("el servicio valida autorización, tamaño y cursor antes de delegar", () => {
    assert.match(serviceSource, /Not contexto\.PuedeInteractuarAnotaciones/);
    assert.match(serviceSource, /_tareaRepository\.ObtenerTarea\(contexto, idTarea\)/);
    assert.match(serviceSource, /tarea\.IdRuta <> contexto\.IdRutaWorkflow/);
    assert.match(serviceSource, /TamanoPaginaPredeterminado As Integer = 25/);
    assert.match(serviceSource, /TamanoPaginaMaximo As Integer = 50/);
    assert.match(serviceSource, /_cursorCodec\.Validar/);
    assert.match(serviceSource, /_cursorCodec\.Proteger/);
    assert.doesNotMatch(withoutComments(serviceSource), /Session|HttpContext|Page|GridView|UpdatePanel|WebMethod|Class_anotacion_tarea/);
});

test("el cursor se protege contra reutilización entre tarea, actor, grupo y ruta", () => {
    for (const marker of ["MachineKey.Protect", "MachineKey.Unprotect", "tarea.IdTarea", "contexto.IdUsuarioWorkflow", "contexto.IdGrupoWorkflow", "contexto.IdRutaWorkflow", "tarea.TokenVersion", "FECHA_DESC_ID_DESC"]) {
        assert.match(cursorSource, new RegExp(marker.replace(/[.*+?^${}()|[\]\\]/g, "\\$&")));
    }
});

test("el repositorio de lectura usa SQL parametrizado, orden fijo y contador consistente", () => {
    assert.match(repositorySource, /Implements INotasWorkflowRepository/);
    assert.match(repositorySource, /EjecutarLectura/);
    assert.match(repositorySource, /at\.INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea/);
    assert.match(repositorySource, /at\.ID_ANOTACION = @idNota/);
    assert.match(repositorySource, /at\.ESTADO_TAREA = 1/);
    assert.match(repositorySource, /ORDER BY at\.FECHA_ANOTACION DESC, at\.ID_ANOTACION DESC/);
    assert.match(repositorySource, /COUNT\(\*\) AS TOTAL/);
    assert.doesNotMatch(repositorySource, /Class_anotacion_tarea|ExecuteNonQuery|INSERT|UPDATE|DELETE|CALL/i);
    assert.doesNotMatch(repositorySource, /SELECT \*/i);
});

test("el ASMX especializado expone solo las tres lecturas y no usa la tarea de sesión", () => {
    assert.match(notesAsmxMarkup, /WebServiceWorkflowNotesModern/);
    for (const operation of ["ListarNotas", "ConsultarNota", "ContarNotas"]) {
        assert.match(notesAsmxSource, new RegExp(`Public Function ${operation}\\(`));
    }
    assert.match(notesAsmxSource, /AsegurarContextoNotas/);
    assert.match(notesAsmxSource, /New MySqlNotasWorkflowRepository/);
    assert.doesNotMatch(withoutComments(notesAsmxSource), /ID_TAREA_SELECCIONDA|Class_anotacion_tarea|CrearNota|ActualizarNota|EliminarNota/);
});

test("el proyecto incluye el endpoint y las piezas modernas de Notas", () => {
    for (const file of [
        "DTOs\\Workflow\\Notas\\NotasWorkflowDtos.vb",
        "Modelo\\Workflow\\Notas\\NotasWorkflowModels.vb",
        "Modelo\\Workflow\\Notas\\NotasWorkflowInterfaces.vb",
        "Services\\Workflow\\Notas\\ServicioNotasWorkflow.vb",
        "Infrastructure\\Repositories\\Workflow\\MySqlNotasWorkflowRepository.vb",
        "Infrastructure\\Workflow\\Notas\\NotasWorkflowCursorCodec.vb",
        "webservice\\WebServiceWorkflowNotesModern.asmx.vb",
    ]) {
        assert.ok(projectSource.includes(`<Compile Include="${file}"`), `falta incluir ${file} en el proyecto.`);
    }
    assert.ok(projectSource.includes('<Content Include="webservice\\WebServiceWorkflowNotesModern.asmx"'), "falta el ASMX de Notas.");
});
