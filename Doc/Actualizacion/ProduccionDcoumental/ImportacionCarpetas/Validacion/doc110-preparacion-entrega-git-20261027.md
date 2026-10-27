# DOC-110: preparación de entrega en Git

La implementación validada del directorio principal se incorporó al worktree existente `.tmp/doc110-work`, rama feature/DOC-110, cuya base era la propuesta inicial 8deedeb3991894253b91c0c87b1fbf80a73580bd. Se conserva el directorio principal en feature/DOC-105 y no se incorporan sus cambios ajenos al perfil.

Antes de sustituir la propuesta activa y documentos técnicos incompletos del worktree, se conservaron íntegros en `.tmp/doc110-delivery-backup-20261027-155846/`. Incluye el refinamiento local modificado. El archivo OpenSpec terminado reemplaza la propuesta activa dentro del conjunto de entrega; no se borró el respaldo de ese trabajo anterior.

La selección contiene los siete archivos productivos/runner ya existentes modificados por DOC-110, modelos/DTOs/servicios/repositorios/clientes propios, la fachada ASMX, pruebas y fixtures sintéticos, contratos/SQL/evidencias documentales, especificación principal y cambio archivado. Las actualizaciones de documentos ya presentes en la rama incluyen identidad incremental, fuentes, permisos y contratos de integridad aprobados. La copia y patch se comprobaron sin conflictos con la base de la rama.

Quedan fuera del índice de entrega los archivos locales de configuración/credenciales, App_Data, documentos operativos y PDF de impresión/repositorio, outputs de compilación, dependencias y reportes locales de otros cambios. Los dos PDF incluidos bajo tools/e2e/fixtures son sintéticos declarados de DOC-110. Una inspección de patrones de secretos de alta confianza no encontró coincidencias en los 270 archivos copiados; no equivale a una auditoría exhaustiva de secretos.

## Verificación en el worktree de entrega

- `node --test tests/doc110-*.test.cjs`: 27 PASS, 4.2 s, incluye compilación/ejecución del arnés .NET.
- `node --test tools/e2e/tests/doc110-*.test.cjs tools/e2e/tests/doc88-production-document-upload-policy.test.cjs`: 73 PASS, 2.9 s.
- Playwright de siete suites locales de navegación, fuentes, proveedores, autocomplete y detalle guiado: 15 PASS, 18.9 s, sin casos autenticados seleccionados.
- MSBuild completo del proyecto, Debug, DeployOnBuild=false y salida/intermedios en `.tmp/doc110-delivery-build`: salida 0; no despliega el sitio.

La primera compilación del paquete señaló la falta de copia de los dos archivos ASMX; se incorporaron y la compilación posterior pasó. La primera prueba Chromium descubrió una carrera del driver local: tras pulsar Guardar comprobaba el aviso de opciones del formulario anterior y podía publicar mientras el guardado seguía ocupado. Se corrigió doc110-fuentes-guided.spec.cjs para esperar la revisión 2 confirmada por la autoridad simulada antes de esperar los metadatos del formulario regenerado. La prueba de fuentes pasó por separado (3 PASS) y luego las siete suites completas (15 PASS). No se alteró el comportamiento productivo para resolverlo.

Las dependencias locales se reutilizaron mediante junctions a bin/packages y node_modules ya disponibles; se excluyen del commit. No se copiaron secretos ni se repitieron autenticaciones, cargas, cambios de permisos, renombrados de tablas o consultas de negocio.

Esta verificación local del paquete se distingue de las [aceptaciones autenticadas anteriores](doc110-verificacion-final-20261027.md) y del [cierre documental OPSXJ](doc110-cierre-opsxj-20261027.md). Conserva empresa única NO_APLICA, aceptación expresa de 7.6, aplicación de permisos externa fuera del proyecto y fixture legado sintético inactivo. Los identificadores finales del commit/PR se informan al completar cada operación; este documento describe la preparación y resultados previos a publicarlos.
