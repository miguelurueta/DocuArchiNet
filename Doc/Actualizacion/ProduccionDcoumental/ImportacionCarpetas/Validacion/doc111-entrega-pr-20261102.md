# DOC-111: validación de la rama de entrega

La entrega se prepara en feature/DOC-111, sobre main 6313f91f, que ya incorpora DOC-110 mediante PR #120. Se reutilizó el worktree previo de DOC-111 y se preservó una copia local de sus catorce archivos modificados antes de actualizarlo. El workspace original de pruebas continúa en su rama previa; no se despliega la nueva compilación.

## Verificación de la copia de entrega

- **88 pruebas PASS**: driver REC, publicación PARCIAL, degradación/restauración, sesión compartida, controles SELECT y política DOC-88.
- **15 pruebas PASS**: arnés de comportamiento .NET Framework y cliente DOC-111, incluidos Chromium local, recarga/reselección y fases/rama con dobles explícitos.
- **Rebuild Debug PASS**, 0 errores y 308 advertencias existentes. Comando MSBuild de aplicación WebForms con `Disable_CopyWebApplication=true`: compila el producto; no empaqueta los contenidos temporales/ignorados preexistentes referenciados por el proyecto legacy. No equivale a una publicación del sitio.
- Especificación principal validada en modo estricto; 130/130 tareas archivadas.
- Los 31 archivos protegidos coinciden con los usados en la validación real, normalizando finales de línea: almacenamiento, SII, carga individual y Web.config.
- Revisión de los archivos seleccionados contra los tokens configurados y patrones de claves: cero coincidencias de credenciales. No se incluyen .env, staging, documentos operativos, bin/obj, perfiles runtime o artefactos privados de Playwright.

La prueba unitaria de verificación final usaba un archivo sintético bajo artifacts y dependía de que ese directorio local existiera. Se cambió únicamente esa fixture a os.tmpdir(), con su limpieza existente; el conjunto de 88 pruebas pasó desde el worktree limpio. Esta corrección no modifica código productivo ni el recorrido real previamente aceptado.

## Procedencia de aceptación

Los REC reales EXPEDIENTE/LOTE/PARCIAL y la degradación/carga individual corresponden a las sesiones 18/22/25/26 y sus huellas históricas en la [matriz final](doc111-matriz-final-20261102.md). No se ejecutaron otra carga o DDL para preparar el PR. La rama de entrega conserva las fuentes productivas validadas y registra por separado las comprobaciones locales anteriores.

SII real y limpieza de originales siguen NO_EJECUTADO; los recursos de pruebas se conservan. El cierre en Jira requiere un PR integrado conforme al flujo del repositorio; el archivo local de OpenSpec no sustituye esa condición.
