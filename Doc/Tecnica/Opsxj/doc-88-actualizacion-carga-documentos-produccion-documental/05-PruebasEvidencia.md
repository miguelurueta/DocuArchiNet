# DOC-88 — Pruebas y evidencia

Fecha: 2026-10-06.

## Evidencia automatizada

| Verificación | Comando | Resultado |
|---|---|---|
| Caracterización y contratos DOC-88 | `node --test tests/doc88-produccion-*.test.cjs` | PASS: 14/14; incluye parámetro opcional ausente, saneamiento por etapa y terminación sin consulta de existencia |
| Sintaxis JavaScript | `node --check generic_control/FileUploadHandler.js` | PASS |
| Política y registro E2E local DOC-88 | `npm.cmd --prefix tools/e2e run test:doc88:policy` | PASS: 11/11 casos; registra preview y ejecución real separados, valida perfil, recursos, respuesta completa y control por expediente |
| Regresión de plataforma y ciclo de recursos | `node --test tools/e2e/tests/workflow-e2e-platform.test.cjs tools/e2e/tests/doc87-registro-tarea-ruta-sii-platform.test.cjs tools/e2e/tests/e2e-test-resource-lifecycle.test.cjs` | PASS: 21/21 casos |
| Regresión de registro, perfiles y seguridad de plataforma | `node --test tools/e2e/tests/workflow-e2e-platform-security.test.cjs tools/e2e/tests/workflow-e2e-platform-registry.test.cjs tools/e2e/tests/workflow-e2e-platform-profile.test.cjs tools/e2e/tests/workflow-e2e-platform-notes-write.test.cjs` | PASS: 13/13 casos |
| DOC-85, handler y Bootstrap compartido | `node --test tests/radicacion-simple-attachment-*.test.cjs tests/bootstrap-table-global-contract.test.cjs tests/doc88-produccion-upload-contract.test.cjs` | PASS: 22/22 casos, sin relajar aserciones |
| Workflow, SII, ENLASE y protección DOC-87 | Suite de 6 archivos registrada en el repositorio | PASS: 44/44 usando un índice Git temporal que toma DOC-88 como línea base; el índice real no se modificó y la aserción de superficies protegidas permaneció intacta |
| Compilación .NET Framework | `MSBuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m` | PASS: 0 errores, 310 advertencias legacy |
| Formato del diff | `git diff --check` | PASS |
| OpenSpec | `openspec validate doc-88-actualizacion-carga-documentos-produccion-documental --strict` | PASS |

La prueba de huella confirma que `generic_control/FileUploadHandler_.ashx.vb` conserva la versión DOC-88 aprobada `8459dd56d2abed043203c21a0eea2f2ae31fc8035f9c9e386fb774bd867a3b73`. El cambio del handler está limitado a aceptar `radicado_radicacion` ausente y sanear por etapa únicamente el evento `PRODUCCION`; los demás eventos conservan el `Catch` y contrato heredados.

## Preview E2E ejecutable sin almacenamiento

El escenario `production-document-upload-preview` quedó registrado en la plataforma común. Reutiliza la sesión autenticada, secretos efímeros, control ODBC registrado y evidencia saneada, sin reservar un recurso mutante. La operación comprueba:

1. rechazo funcional saneado sin selección, sin excepción interna y sin llamada al handler;
2. correspondencia entre el expediente seleccionado y `CDexpedienteSeleccionado[0].IdExpediente`;
3. carga del PDF y selección de tipología mediante los controles reales;
4. presencia del control `Guardar`, sin activarlo;
5. cero solicitudes al handler de carga;
6. ausencia de navegación durante la preparación;
7. control `SELECT` registrado para el expediente sin cambios.

El perfil de ejemplo es `tools/e2e/profiles/doc88-production-document-upload.profile.example.json`. El perfil local debe conservar únicamente datos no sensibles y permanecer dentro de `tools/e2e/profiles`.

Comando autorizado:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario production-document-upload-preview --profile doc88-production-document-upload.local.runtime.json --authorize environment,account,discardable-file
```

La consola solicita tres confirmaciones explícitas y captura las credenciales de aplicación y lectura únicamente de forma efímera.

## Ejecución E2E real autorizada

Se ejecutó el escenario autenticado contra el ambiente local autorizado con la cuenta de prueba `LUZ.AGUILERA`, expediente `467`, tipología `50` y un PDF descartable. Los secretos se capturaron de forma efímera en la consola interactiva y no se persistieron ni se incluyeron en la evidencia.

El 2026-10-06 se ejecutó el escenario mutante `production-document-upload-execution` contra el sitio local compilado. Las corridas de caracterización delimitaron primero el fallo a `FileUploadHandler_.ashx.vb` y después al contexto temporal anterior a `UploadSaveFile`. La causa reproducida fue la lectura de `radicado_radicacion`, parámetro opcional que `PRODUCCION` no envía: la expresión aplicaba `.Trim()` sobre el valor nulo. La corrección usa `String.Empty` únicamente cuando ese parámetro está ausente.

Después de recompilar, la corrida real final terminó con `success=true` y comprobó:

- exactamente una solicitud al handler y un documento procesado;
- rechazo seguro de la selección inválida;
- preparación del expediente y tipología confirmadas;
- respuesta de almacenamiento confirmada con identificadores positivos;
- una fila nueva proyectada sin navegación;
- `production-document-records changed=true` mediante el control `SELECT` registrado;
- recurso mutante consumido, sin reintento automático.

La evidencia saneada se conserva en `tools/e2e/artifacts/workflow-e2e-platform-production-document-upload-execution.json`; no contiene credenciales, cookies, cadenas de conexión, identificadores persistidos, rutas del archivo ni mensajes internos del servidor.

## Preview E2E autenticado completado

La corrida vigente `production-document-upload-preview` terminó correctamente en el ambiente local autorizado con el expediente `467` y la tipología `50`. El artefacto saneado `tools/e2e/artifacts/workflow-e2e-platform-production-document-upload-preview.json` registra:

- selección inválida bloqueada;
- preparación y tipología confirmadas;
- almacenamiento no invocado;
- navegación no observada;
- un control `SELECT` ejecutado y sin cambios;
- ausencia de reserva mutante.

Antes de completarla se aislaron y corrigieron dos defectos exclusivos del runner E2E: el control intentaba leer `resource.profileField` aunque el preview declara `resource: null`, y el valor saneado `NOT_REQUESTED` era rechazado por la política de evidencia. El escenario ahora declara `controlProfileField: expedientId`, valida ese parámetro antes de consultar y registra `NOT_INVOKED`. La regresión ejecutable cubre ambos contratos.

## QA manual pendiente

Con autorización:

1. Abrir Producción Documental y seleccionar un expediente autorizado.
2. Verificar rechazo controlado con selección caducada y tipología inválida.
3. Cargar un archivo descartable y comprobar una sola fila con el identificador retornado.
4. Activar Guardar dos veces durante la misma solicitud y confirmar un solo envío activo.
5. Simular una falla de proyección y confirmar que no se realiza otra llamada al handler.
6. Comprobar un evento compartido distinto de `PRODUCCION` sin cambio observable.
