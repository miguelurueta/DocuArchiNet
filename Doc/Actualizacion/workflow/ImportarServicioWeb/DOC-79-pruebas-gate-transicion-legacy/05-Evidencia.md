# Evidencia

- Ticket: DOC-79
- Cambio OpenSpec: doc-79-pruebas-retiro-gate
- Clasificacion: cross_cutting

## Evidencia local

| Comando | Resultado |
| --- | --- |
| `powershell -File tools/validation/Verify-ImportarServicioWebFrontend.ps1` | 24/24 pruebas; 0 fallos |
| `node --test tools/e2e/tests/importar-servicio-web-modern.spec.cjs` | 22/22 pruebas estructurales; 0 fallos |
| `node --test tests/importar-servicio-web-*.test.cjs` | 445/445 pruebas; 0 fallos |
| `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m` | 0 errores; 310 advertencias preexistentes |

Estas ejecuciones no autenticaron usuarios, no usaron red, no modificaron tareas y no activaron el gate.

## Evidencia E2E real

La corrida de lectura fue autorizada expresamente para el ambiente y la cuenta de prueba, después de leer el runbook obligatorio. Se reutilizó exclusivamente la plataforma E2E existente.

| Campo | Resultado saneado |
| --- | --- |
| Escenario | `import-sii-read` |
| Tarea autorizada | `219877` |
| Recibo | `S002188422` |
| Código de barras | `18221398` |
| Tamaño de muestra efectivo | `1` |
| Controles | `7` |
| Cambios persistentes | `NO` (`sinCambios=SI`) |
| Resultado | Correcto |
| Gate al finalizar | `false`; usuarios y grupos vacíos |

El perfil inicialmente solicitó dos elementos, pero la UI expuso menos de dos elementos seleccionables. La plataforma se detuvo antes de cualquier mutación con `IMPORT_E2E_PREPARATION_UI_MULTIPLE_ITEMS_UNAVAILABLE`. Se ajustó únicamente el perfil runtime a una muestra compatible de un elemento y la repetición autorizada terminó correctamente. La cobertura multidocumento permanece en las suites automatizadas específicas.

## Saneamiento

No se registraron credenciales, cookies, tokens, cadenas de conexión ni valores sensibles. Las consultas de control fueron exclusivamente `SELECT`.
