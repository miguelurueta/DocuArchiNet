const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const crypto = require("node:crypto");

const root = path.resolve(__dirname, "..");
const expectedBlobs = {
  "workflow/ClassAlmacenamiento.vb": "b875d24f0a9ff63f24a4fff96f637cb04afb1405",
  "Gestion/ClassGaExpediente.vb": "2998523902ec2d455ac4b96a644297674d6d14b9",
  "webservice/WebServiceGaExpediente.asmx.vb": "11a60af88f9b3591e70ca3e91567d34fe39fce7f",
  "webservice/WebService_integracion_sii.asmx.vb": "580c3832343205f2246aa0acbfcc8f703f2a0ebe",
};
const expectedIntegrationTree = "c64fde736b91acef8b3baec5d9bb63d32b39a318a6ac08afae2aa01c6bc45823";

function normalizedBuffer(relative) {
  return Buffer.from(fs.readFileSync(path.join(root, relative), "utf8").replace(/\r\n/g, "\n"), "utf8");
}

function gitBlobHash(buffer) {
  const header = Buffer.from(`blob ${buffer.length}\0`);
  return crypto.createHash("sha1").update(Buffer.concat([header, buffer])).digest("hex");
}

function filesBelow(directory) {
  return fs.readdirSync(directory, { withFileTypes: true }).flatMap((entry) => {
    const target = path.join(directory, entry.name);
    return entry.isDirectory() ? filesBelow(target) : [target];
  });
}

test("archivos legacy protegidos conservan sus blobs canonicos", () => {
  for (const [relative, expected] of Object.entries(expectedBlobs)) {
    assert.equal(gitBlobHash(normalizedBuffer(relative)), expected, relative);
  }
});

test("el arbol Integracionccv conserva su linea base completa", () => {
  const hash = crypto.createHash("sha256");
  const files = filesBelow(path.join(root, "Integracionccv"))
    .sort((left, right) => left.localeCompare(right, "en"));
  for (const file of files) {
    const relative = path.relative(root, file).replaceAll("\\", "/");
    hash.update(relative);
    hash.update("\0");
    hash.update(fs.readFileSync(file, "utf8").replace(/\r\n/g, "\n"));
    hash.update("\0");
  }
  assert.equal(hash.digest("hex"), expectedIntegrationTree);
});

test("la implementacion moderna no llama ASMX legacy por HTTP interno", () => {
  for (const area of ["DTOs", "Modelo", "Services", "Infrastructure"]) {
    for (const file of filesBelow(path.join(root, area)).filter((item) => /\.vb$/i.test(item))) {
      const source = fs.readFileSync(file, "utf8");
      assert.doesNotMatch(source, /WebService(?:GaExpediente|_integracion_sii)\.asmx/i, file);
    }
  }
});
