const { spawnSync } = require("node:child_process");
const path = require("node:path");

const root = path.resolve(__dirname, "..");

function run(label, command, args) {
  process.stdout.write(`\n[DOC-67] ${label}\n`);
  const result = spawnSync(command, args, { cwd: root, stdio: "inherit", shell: false });
  if (result.error) {
    process.stderr.write(`[DOC-67] ${label}: ${result.error.message}\n`);
    process.exit(1);
  }
  if (result.status !== 0) process.exit(result.status ?? 1);
}

run("contratos, integración local y regresión", process.execPath, [
  "--test",
  "tests/importar-servicio-web-*.test.cjs",
]);

const msbuild = "C:\\Program Files\\Microsoft Visual Studio\\18\\Enterprise\\MSBuild\\Current\\Bin\\MSBuild.exe";
run("compilación VB.NET", msbuild, [
  "GestionDocumental-Docuarchi.net.vbproj",
  "/t:Build",
  "/p:Configuration=Debug",
  "/m",
  "/v:minimal",
  "/clp:ErrorsOnly",
]);

process.stdout.write("\n[DOC-67] READINESS_OK: validación local completa; la E2E puede reservarse para integración real.\n");
