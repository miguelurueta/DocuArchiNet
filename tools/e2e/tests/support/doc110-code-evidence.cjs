'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {execFileSync}=require('node:child_process');
function captureCodeEvidence(){
  const root=path.resolve(__dirname,'../../../..');
  const files=[
    'tools/e2e/tests/doc110-perfil-importacion-session.spec.cjs',
    'tools/e2e/tests/support/doc110-draft-lifecycle.cjs',
    'tools/e2e/tests/support/doc110-publication-lifecycle.cjs',
    'tools/e2e/tests/support/doc110-permission-revocation.cjs',
    'tools/e2e/tests/support/doc110-code-evidence.cjs',
    'tools/e2e/tests/support/doc110-acceptance.cjs',
    'tools/e2e/scripts/verify-doc110-identities-readonly.ps1',
    'tools/e2e/scripts/support/doc110-permission-db.ps1',
    'Services/ProduccionDocumental/ImportacionCarpetas/ServicioPerfilImportacion.vb',
    'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlPerfilImportacionRepository.vb',
    'webservice/WebServiceImportacionCarpetas.asmx.vb',
    'js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js',
    'bin/GestionDocumental-Docuarchi.net.dll'
  ].sort().map(file=>({file,sha256:crypto.createHash('sha256').update(fs.readFileSync(path.join(root,file))).digest('hex')}));
  const head=execFileSync('git',['rev-parse','HEAD'],{cwd:root,encoding:'utf8',stdio:['ignore','pipe','ignore']}).trim();
  return {head,scopeSha256:crypto.createHash('sha256').update(JSON.stringify(files)).digest('hex'),files,
    note:'HEAD alone does not represent uncommitted files; hashes include active DLL and listed source/test files.'};
}
module.exports={captureCodeEvidence};
