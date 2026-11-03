# DOC-113 — Validación final de ventana integrada

Fecha: 2026-11-03. Implementación técnica y verificación completadas: **12/12 tareas**, **6 requisitos / 13 escenarios** cubiertos, **UI-01/UI-02/UI-03 EJECUTADO_OK**. Rama candidata feature/DOC-113; HEAD inicial f7a88e38176e789e0b0369a6a23a56ab46769635 no incluye los cambios aún sin commit. Publicación, merge y cierre Jira no realizados.

## Cambios de producto

- F-01: se valida la forma de apertura, perfiles y preparación antes de asignar datos al modelo. Incidencias/valores anidados incompatibles muestran error accesible sin romper revisión ni habilitar confirmación.
- F-02: el controlador captura el disparador antes de deshabilitarlo; al cerrar restaura foco después de habilitarlo, solo si sigue conectado. Se conservan los hooks e inicialización del host existentes.
- Único asset de producto modificado: js/gestion/ImportacionCarpetas/ImportacionCarpetas.js. API, inventario, vista y CSS existentes se reutilizan. No hubo cambios SQL, Web.config, permisos, SII o almacenamiento.

## Casos reales autorizados

Ambiente y cuenta de pruebas de DOC-111, con autorización específica DOC-113 registrada. A=7, B=8; publicaciones 38/39 verificadas por consulta de lectura. Credenciales efímeras solo en consola; controles RA/DA exclusivamente SELECT por DSN registrado.

| Caso | Estado | Comprobación | Efectos |
|---|---|---|---|
| UI-01 | EJECUTADO_OK | Cambio de perfil 38/39 y navegación 7 a 8 mantienen destino y solicitudes en A; una ventana/apertura. | 0 preparaciones, 0 confirmaciones. |
| UI-02 | EJECUTADO_OK | Raíces homónimas con subcarpetas conservan IDs/rutas; cambiar inventario invalida revisión anterior y requiere nueva preparación. | Planes 8/9 conservados; exactamente 2 preparaciones, 0 confirmaciones. |
| UI-03 | EJECUTADO_OK | Postback parcial, Shift+Tab, Escape/foco, rechazo real nivel 0 y reapertura normal; tres aperturas, una petición por acción. | 0 preparaciones, 0 confirmaciones. |

No se recibió contenido, creó expediente ni almacenó/incorporó documento. Los lotes y documentos históricos de DOC-111 se conservaron. La prueba no confirma un lote: ese efecto quedó fuera del alcance aprobado.

## Evidencia y huellas

- [UI-02 real y manifiesto original E2E](doc113-ui02-real-20261103.json).
- [UI-01 real y fallo inicial de UI-03](doc113-ui01-ui03-real-20261103.json).
- [UI-03 final, manifiesto y resumen consolidado](doc113-ui03-real-20261103.json).
- [Captura del diálogo tras reapertura](doc113-ui03-real-20261103.png), revisada localmente: sin login ni credenciales.
- [Manifiesto local inicial](doc113-evidencia-local-20261103.json) y [regresión tras corregir la aserción](doc113-evidencia-local-focus-20261103.json).

SHA256 del JavaScript servido en los tres casos: 1bf5e26678f80f2bffeefc91610f368bc38288b7c641bd7d8059485f8e7f944a. La suite comprobó igualdad de bytes con el candidato antes de operar. Huella E2E UI-01/UI-02: b34ec1dcbe16006eeca5c8f16f74ba6a760065335cb14be4cbe09a31a9e36c35; UI-03 final: 32090b4a14ecd418f7d41fe31f79f82c7ee5f8427101afb0f3cc260f3f642b9c. Entre ambos manifiestos E2E solo cambió el helper de prueba para identificar el diálogo visible; ningún asset de producto cambió. El ámbito del manifiesto local es diferente y se conserva por separado.

## Fallo inicial de UI-03 y resolución

La primera corrida devolvió DOC113_FOCUS_ESCAPED. Diagnóstico de solo lectura detectó cuatro roles dialog en DOM; document.querySelector tomaba un diálogo legacy oculto, con controls=[] y clasificación OUTSIDE relativa al elemento equivocado. Se reprodujo localmente con otro diálogo oculto anterior y se corrigió únicamente la comprobación E2E mediante el locator accesible del diálogo visible. La regresión demuestra que Tab/Shift+Tab se mantienen dentro y que un foco realmente externo sigue rechazado. La repetición autorizada de UI-03 pasó; se conserva el intento anterior como evidencia histórica.

## Validación local y compilación

| Comprobación | Resultado / comando |
|---|---|
| Cliente/Chromium sintético | 16/16 PASS: node --test tests/doc111-importacion-carpetas-client.test.cjs. Sin login ni HTTP de aplicación. |
| Política/driver/controles | 43/43 PASS: node --test tools/e2e/tests/doc111-importacion-carpetas-driver.test.cjs. |
| Sintaxis | node --check de scripts afectados PASS. |
| Registro y autorización | 3 casos descubiertos; sin autorización se omiten antes del login. --check no ejecuta operaciones reales. |
| Framework MSBuild | Visual Studio 18 MSBuild.exe, /t:Build /p:Configuration=Debug /p:OutputPath=.tmp/doc113-build/bin/ /p:IntermediateOutputPath=.tmp/doc113-build/obj/ /p:BaseIntermediateOutputPath=.tmp/doc113-build/obj/ /p:BuildProjectReferences=false /p:UseSharedCompilation=false: código 0; DLL aislada de 9627648 bytes, advertencias legacy. DLL no desplegada. |

Línea base previa: 13 pruebas DOC-111; primera corrección: 15; regresión del selector de foco: 16. Total final local: 59 PASS. El build no se repitió tras cambiar únicamente helper/prueba JavaScript. Logs originales en .tmp del worktree. Los REC históricos DOC-111 mantienen su procedencia y no sustituyen estos recorridos UI.

## Verificación contra especificación

| Requisito / decisiones | Cobertura |
|---|---|
| RQ-01 / D-01 | UI-01 destino estable y apertura única; reutilización de componentes. |
| RQ-02 / D-02 | Regresión de incidencias/perfiles/valores inválidos y respuesta tardía, revisión recuperable sin confirmación. |
| RQ-03 / D-03 | Regresión de mismo host/retirada durante apertura y UI-03 real de foco/postback/cierre/reapertura. |
| RQ-04 / D-04 | UI-01/02/03 reales, con DOM y solicitudes comprobados; ningún REC usado como sustituto. |
| RQ-05 / D-05 | Políticas de autorización y suite omitida sin permiso; sesión/consola/DSN compartidos y guard de operaciones. |
| RQ-06 / D-06 | Manifiestos por caso, build aislado, regresiones y fallo histórico conservados. |

## Despliegue de pruebas y reversa

Se aplicó exclusivamente el JavaScript autorizado al runtime raíz; respaldo previo .tmp/doc113-controller-before-ui.js y autorización .tmp/doc113-ui-authorization.json. El resto de la implementación permanece en el worktree aislado, con artefactos OpenSpec y dossier reflejados en el repositorio local. La reversa de UI consiste en restaurar ese asset o revertir el diff focal; no elimina planes 8/9 ni recursos históricos. La autorización de prueba no se convierte en aprobación editorial o publicación.

## Aprobación editorial posterior

El 2026-11-03 el usuario aprobó el cierre técnico y la continuación del flujo de publicación/cierre. Reporte revisado SHA256 f0d84ee388169f0e40ac7ffbcba2bf0876d373f667e5e8c23469798e06850f42. Los estados de aprobación pendiente de secciones anteriores corresponden al corte previo; este registro no afirma publicación o merge aún realizados.
