# DOC-90 — Pruebas y evidencia

- Ticket: DOC-90
- Cambio OpenSpec: doc-90-actualizar-adjuntar-documento-radicacion
- Clasificacion: cross_cutting

Fecha de ejecución final: 2026-10-08.

## Automatización local

| Validación | Resultado |
|---|---|
| Contrato focal de Radicación Entrante clásica | PASS, 10/10 |
| Matriz DOC-90, DOC-85, DOC-88, DOC-89 y controles compartidos | PASS, 75/75 |
| Regresión posterior a la corrección del constructor de índices | PASS, 25/25 focales y 57/57 en matriz DOC-90/DOC-85/consumidores compartidos |
| Política local del E2E DOC-90 | PASS, incluida en la matriz 75/75 |
| Compilación `GestionDocumental-Docuarchi.net.vbproj` Debug | PASS, ensamblado generado, sin errores; advertencias legacy existentes |
| `openspec validate ... --strict` | PASS |
| `opsxj:refine DOC-90 --sync` | PASS, trazabilidad sincronizada |
| `git diff --check` | PASS; únicamente avisos de normalización LF/CRLF |

Comando focal:

```powershell
node --test tests/doc90-radicacion-classic-attachment.test.cjs tests/doc90-file-upload-shared-consumers.test.cjs tests/radicacion-simple-attachment-upload-signatures.test.cjs tests/radicacion-simple-attachment-protected-files.test.cjs tests/radicacion-simple-attachment-radicado-fallback.test.cjs
```

La matriz ampliada incluyó todos los `radicacion-simple-attachment-*.test.cjs`, `radicacion-simple-control-contract`, `bootstrap-table-global-contract`, las suites `doc88-produccion-*`, las políticas E2E DOC-88 y DOC-89, los dos contratos DOC-90 y `tools/e2e/tests/doc90-radicacion-classic-attachment-policy.test.cjs`.

Compilación:

```powershell
msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal
```

## Cobertura

- Evento exclusivo y versión del recurso clásico.
- Estado seguro de pestañas sin asignación, independiente del contador de pendientes, y transiciones idempotentes al asignar, iniciar o terminar.
- Permanencia del selector y contador de pendientes en el encabezado común, fuera de la pestaña documental deshabilitada.
- Correspondencia de `Hidden_radicado_seleccion`, campana y contador con los `UpdatePanel` que participan en la asignación parcial.
- Firmas de 10 y 12 argumentos y sus ramas propietarias.
- Rechazos antes del prealmacenamiento por módulo, plantilla, sesión, radicado, tarea y trámite.
- Una única llamada clásica de prealmacenamiento y propagación de tarea.
- Construcción de índices con radicado y plantilla autoritativos antes de almacenar, sin redescubrimiento clásico desde `DAT_ADIC_TAR`.
- Respuesta completa y proyección incremental sin postback.
- Protección del servicio y cliente de Radicación Simplificada.
- Inventario de los quince eventos, nueve llamadas backend y consumidores JavaScript identificados.
- Regresión de Producción, Workflow/escáner, versiones, SII, migración y correspondencia.

## E2E real

Estado: **PASS** con autorización explícita para ambiente, cuenta, carga real y recurso descartable. La corrida final ejecutó una prueba en un worker y terminó `1 passed` en 27,7 segundos, con código `0`.

El artefacto saneado `tools/e2e/artifacts/doc90-radicacion-classic-attachment-e2e.json` registró:

- contrato multipart clásico verificado;
- persistencia `0 → 1`;
- una fila insertada sin recarga ni postback;
- archivos protegidos intactos;
- gate intacto.

El artefacto conserva una huella irreversible del recurso y no guarda radicado, cuenta, credenciales, cookies, consultas, cadena de conexión ni contenido documental.

Durante la validación, una corrida previa alcanzó correctamente el handler y fue rechazada antes del almacenamiento con `El radicado autoritativo es obligatorio para construir los índices del gabinete.`. La reconciliación `SELECT` confirmó `0` documentos. Esa evidencia permitió corregir el orden: la ruta clásica ahora construye los índices con el radicado y la plantilla ya resueltos. Tras devolver manualmente el recurso al estado pendiente mediante el flujo autorizado, la corrida final produjo exactamente una persistencia.

Comando ejecutado:

```powershell
npm.cmd --prefix tools/e2e run test:doc90:radicacion-classic-attachment -- --profile tools/e2e/profiles/<perfil-doc90-runtime.json>
```

No regresión autenticada de Simplificada, pendiente de una autorización y un recurso independientes:

```powershell
npm.cmd --prefix tools/e2e run test:radicacion-simple:attachment -- --profile tools/e2e/profiles/<perfil-doc85-runtime.json>
```

La E2E clásica demuestra el recorrido autenticado y una única persistencia para Radicación Entrante. No acredita por sí sola una corrida autenticada de Radicación Simplificada ni la ausencia absoluta de regresiones fuera de las suites ejecutadas.
