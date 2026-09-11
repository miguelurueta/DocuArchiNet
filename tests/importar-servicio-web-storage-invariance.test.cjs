const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const crypto = require("node:crypto");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const legacyPath = path.join(root, "workflow/ClassAlmacenamiento.vb");
const adapterPath = path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb");
const expectedLegacyBlob = "b875d24f0a9ff63f24a4fff96f637cb04afb1405";

const gitBlobHash = (buffer) => {
  const header = Buffer.from(`blob ${buffer.length}\0`);
  return crypto.createHash("sha1").update(Buffer.concat([header, buffer])).digest("hex");
};

test("ClassAlmacenamiento conserva exactamente la linea base de DOC-56", () => {
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
