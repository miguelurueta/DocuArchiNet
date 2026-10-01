const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const crypto = require("node:crypto");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const legacyPath = path.join(root, "workflow/ClassAlmacenamiento.vb");
const adapterPath = path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb");
const expectedLegacyBlob = "78e9fcd8bdd2325043a5b024db7eef49277fb76f";

const gitBlobHash = (buffer) => {
  const header = Buffer.from(`blob ${buffer.length}\0`);
  return crypto.createHash("sha1").update(Buffer.concat([header, buffer])).digest("hex");
};

test("ClassAlmacenamiento conserva la linea base aditiva caracterizada por DOC-81", () => {
  const normalized = Buffer.from(fs.readFileSync(legacyPath, "utf8").replace(/\r\n/g, "\n"), "utf8");
  assert.equal(gitBlobHash(normalized), expectedLegacyBlob);
});

test("el adaptador conserva la unica invocacion moderna al almacenamiento legacy", () => {
  const adapter = fs.readFileSync(adapterPath, "utf8");
  assert.equal((adapter.match(/\.AlmacenaDocumentoTareaWorkflow\(/g) || []).length, 1);
  for (const area of ["DTOs", "Modelo", "Services", "Infrastructure"]) {
    const stack = [path.join(root, area)];
    while (stack.length) {
      const current = stack.pop();
      for (const entry of fs.readdirSync(current, { withFileTypes: true })) {
        const target = path.join(current, entry.name);
        if (entry.isDirectory()) stack.push(target);
        else if (/\.vb$/i.test(entry.name) && target !== adapterPath) {
          assert.doesNotMatch(fs.readFileSync(target, "utf8"), /\.AlmacenaDocumentoTareaWorkflow\(/, target);
        }
      }
    }
  }
});

test("orquestador no repite almacenamiento durante expedientes o vinculación", () => {
  const orchestrator = fs.readFileSync(path.join(root, "Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb"), "utf8");
  const steps = fs.readFileSync(path.join(root, "Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb"), "utf8");
  const relations = fs.readFileSync(path.join(root, "Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb"), "utf8");
  assert.doesNotMatch(orchestrator + relations, /AlmacenaDocumentoTareaWorkflow|_storage\.Almacenar/);
  assert.equal((steps.match(/_storage\.Almacenar\(command\)/g) || []).length, 1);
});

test("la versión oficial no conserva gate ni listas de alcance", () => {
  const configuration = fs.readFileSync(path.join(root, "web.config"), "utf8");
  assert.doesNotMatch(configuration, /WorkflowCentroTrabajoModernActive|WorkflowCentroTrabajoModernUsers|WorkflowCentroTrabajoModernGroups/i);
});
