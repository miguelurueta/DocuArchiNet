# DOC-111: transiciones e aislamiento, 2026-10-28

Trabajo local sobre HEAD `b2e93b726f221868cf2b82d43200a8e568871117` con cambios sin commit. Avance: **75/130 tareas completas**. Se completan 9.5, 9.6, 9.8 y 10.1; DOC-111 continúa en implementación.

`CoordinadorImportacionCarpetas.vb` implementa la tabla cerrada de PLAN, RECEPCION, EFECTO, RAIZ, INTENTO y LOTE documentada en 03. Rechaza transiciones no enumeradas y devuelve copias de sus listas. El repositorio operativo aplica la validación al reservar, confirmar y registrar fallos de recepción, conservando sus controles previos de revisión, pertenencia y evidencia. La legalidad de una transición no autoriza un efecto: reclamaciones documentales, barreras, ejecución y reconciliación siguen pendientes.

El arnés Framework existente comprueba todas las parejas de estados por dimensión, estados terminales, rechazo de variantes de mayúsculas y mutación de copias. Incluye prohibiciones explícitas de repetir fases inciertas o reutilizar un intento terminal. Conserva las familias previas de guard, preflight, confirmación, recepción y staging con persistencia simulada explícita. Se comprueba rechazo de ambas extensiones `.tif` y `.tiff` antes de leer el stream.

La caracterización estática de almacenamiento en el documento 15 identifica sesión, FileInfo.Name, índices, identidades distintas de imagen/inventario, eliminación de temporales, incorporación/índice ya realizados y fronteras de commits/disco. El XML del expediente se guarda después del commit y cierre de conexión; no se ha demostrado exclusión con escritores legacy externos. La operación inferior conserva esas fronteras. **El adaptador de almacenamiento permanece deshabilitado**: esta decisión satisface 9.6 sin presentar un bloqueo DOC-111 como exclusión global. No se modifica el writer compartido ni SII.

El extractor Multi-TIFF elimina previamente los archivos del directorio de salida recibido. No se invocó; TIFF/Multi-TIFF se declara no soportado en esta entrega del receptor. 9.7 sigue abierta: la copia de staging conserva el nombre, pero falta verificar el nombre/extensión y relaciones realmente persistidos mediante un almacenamiento aislado. También permanecen pendientes 9.9–9.12 y las fases documentales de 10.2 en adelante.

Verificación local PASS:

- Build Debug completo mediante MSBuild, sin despliegue, salida/intermedios en `.tmp/doc111-transiciones-build/`; log `.tmp/doc111-transiciones-build.log`, código 0.
- `node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc111-importacion-carpetas-client.test.cjs tools/e2e/tests/doc88-production-document-upload-policy.test.cjs`: 16 pruebas aprobadas. La primera ejecuta las familias VB/C# en .NET Framework.
- `openspec.cmd validate doc-111-produccion-importacion-carga-carpeta --strict` y `git diff --check`.

No se ejecutaron HTTP/E2E autenticados, consultas SQL, almacenamiento documental, creación de expedientes, extractor de imágenes, despliegue ni cambios de configuración. Las transacciones/rollback/carreras MySQL y el pipeline HTTP reales de recepción siguen sin acreditarse con estas pruebas locales.
