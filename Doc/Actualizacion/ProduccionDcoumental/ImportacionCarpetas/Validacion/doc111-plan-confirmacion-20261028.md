# DOC-111 — Preparación y confirmación revalidadas

Fecha: 2026-10-28. Base HEAD `b2e93b726f221868cf2b82d43200a8e568871117` con cambios locales previos; evidencia del workspace, sin commit o despliegue en este corte. Se implementan 7.13–7.15 y 8.1–8.2: progreso **62/130**. Esta evidencia complementa el corte de apertura/preflight del mismo día y sustituye su limitación de bloqueo global permanente cuando el lector de coincidencias está disponible.

## Cambios comprobados

`MySqlImportacionCarpetasRepository.ComprobarCoincidencias` consulta metadatos de las columnas documentales, exige charset/collation compatibles y compara códigos mediante el motor. Comprueba representabilidad sin pérdida y duplicados del inventario; los IDs de raíz usan comparación binaria ASCII. Todos los códigos, raíces, empresa y área se parametrizan. Solo se interpolan nombres de charset/collation leídos de metadatos y validados, nunca SQL o identificadores de cliente.

La búsqueda acotada usa empresa/área/código y abarca todos los volúmenes; no inventa un volumen implícito ni se anuncia como identidad documental completa. El catálogo usa Codigo_Area como AreaRef, coincidente con CODIGO_AREA_TRD del escritor inspeccionado. Uno o varios candidatos bloquean la raíz según política/cardinalidad. REUTILIZAR_VALIDADO con un candidato devuelve REUTILIZACION_NO_DISPONIBLE hasta verificar identidad, destino, clasificación, estado y permisos; no se reutiliza ni se cambia silenciosamente la política. Este límite conservador puede bloquear códigos que otra operación permitiría en distintos volúmenes. La comparación física y la caracterización completa siguen pendientes.

`ServicioPreparacionImportacion` integra el lector, conserva incidencias originales y evalúa elegibilidad por EXPEDIENTE/LOTE/parcial. Resuelve los metadatos de todos los archivos en una llamada al coordinador existente, para compartir respuestas de proveedor por versión/clave dentro de la preparación. Metadatos indisponibles dejan incidencia global y plan no confirmable. `Preparar` fija creación y vencimiento UTC a segundos y delega GuardarPlan al repositorio único. Devuelve el resultado persistido con revisión/huella; una solicitud nueva prepara una identidad nueva, no modifica planes confirmados.

El ASMX expone PrepararPlan por el dispatcher operativo con guard JSON y cuerpo cerrado. No acepta snapshot, resultados ni identidad autenticada del payload.

`ServicioRecepcionImportacion` implementa la fase de confirmación. La recepción de bytes no se declara implementada por este archivo. ObtenerContextoPlan resuelve la referencia persistida filtrada por usuario/empresa; después el servicio reconstruye y autoriza nivel, sello y gabinete antes de cargar y confirmar el plan. Una confirmación nueva revalida revisión/huella, expiración, publicación exacta, cuotas y coincidencias de unidades elegibles. La comprobación transaccional final sigue en ConfirmarPlan del repositorio existente.

Un plan ya CONFIRMADO conserva la revalidación de autoridad/recursos, pero no vuelve a aplicar expiración o disponibilidad del perfil como si fuera nuevo. El repositorio resuelve la repetición/contenido conflictivo y devuelve el mismo lote. La fachada ConfirmarPlan comparte guard/sobre operativo y devuelve únicamente referencias, estado, revisión y totales; no llama a adaptadores documentales. La construcción de selección de perfil/proveedores se comparte entre ambas fases, sin duplicar repositorio ni política.

Los contratos y límites vigentes están en Diseno/02-contratos-operacion.md y Diseno/06-coincidencias-disponibilidad.md. ServicioRecepcionImportacion.vb se incluye en Compile del proyecto y en el arnés Framework existente. No hay nuevas tablas, migraciones, configuración de ambiente o runner.

## Verificaciones

| Verificación | Resultado y alcance |
|---|---|
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs` | PASS. Todas las familias DOC-110/DOC-111, con nuevas pruebas de consultas, preparación y confirmación. Compila código VB real y ejecuta .NET Framework. |
| Build Debug del proyecto real con MSBuild, DeployOnBuild=false, OutputPath=.tmp/doc111-plan-confirm-build/bin/ e IntermediateOutputPath=.tmp/doc111-plan-confirm-build/obj/ | PASS, exit 0; log `.tmp/doc111-plan-confirm-build.log`. Sin despliegue o sustitución de DLL del sitio; se mantienen advertencias del proyecto legacy. |
| `node --test tests/doc111-importacion-carpetas-client.test.cjs tools/e2e/tests/doc88-production-document-upload-policy.test.cjs` | PASS: 15 pruebas locales de inventario/cliente y política de carga individual. Sin carga autenticada real. |
| `openspec.cmd validate doc-111-produccion-importacion-carga-carpeta --strict` | PASS estructural. No equivale a validación de implementación o aceptación real. |
| `git diff --check` | PASS, con aviso LF/CRLF sobre el reporte OPSXJ previamente modificado. |

Casos de coincidencia: cero/uno/varios candidatos; ámbito y área enviados por parámetros; códigos con comillas sin interpolación; metadatos incompletos, tipo y charset incompatibles, collation manipulada; codificación no representable; duplicados de raíces según respuesta del motor; autoridad denegada antes de leer. Los resultados de comparación se suministran mediante un doble explícito; estas pruebas validan composición y tratamiento, **no certifican ejecución SQL, collation ni compatibilidad física sobre MySQL real**.

Casos del servicio de preparación: mismo inventario/snapshot con resultado determinista; fecha de control desde reloj UTC; vencimiento según política; huella del documento persistible; round-trip mediante documentos y lector IDataRecord reales del repositorio sobre filas en memoria; conservación de incidencias/totales. En EXPEDIENTE una raíz válida puede confirmarse; LOTE bloquea ante un archivo inválido; parcial permite un archivo válido dentro de una raíz afectada conservando el rechazado. Esquema/comparación no comprobables bloquean globalmente.

Casos de confirmación: hash alterado, expiración, destino movido, permiso de gabinete revocado, publicación incompatible/sustituida y coincidencia nueva se rechazan antes del callback transaccional. La confirmación válida devuelve lote/totales; repetir conserva lote y omite nueva consulta de perfil/coincidencias aunque el plan haya vencido. Cambiar revisión produce conflicto; revocar autoridad impide también la repetición. El callback del lote es un doble local; no se ejecuta una nueva carrera ni transacción real en este corte. La evidencia real previa del repositorio continúa separada.

Casos del ASMX: invocación en HttpContext sintético; el cuerpo cerrado prevalece sobre el objeto interpretado por ASMX, y campos de resultado o destino añadidos por cliente no llegan a persistencia/confirmación. Se comprueban sobre y status HTTP del objeto de respuesta. No se prueba el pipeline IIS/ASMX mediante red ni autenticación real.

La compilación real usa `Option Compare Text`. El repositorio operativo, canonizador y servicios de preparación/recepción declaran ahora `Option Compare Binary` para que identidades/rutas no se fusionen por diferencias de mayúsculas. El arnés se ejecuta con `/optioncompare:Text`, igual que el proyecto, y las pruebas nuevas comprueban que `rA` y `ra` reciben valores separados y una ruta con raíz de distinto caso no se acepta como idéntica. Las pruebas DOC-110 se conservan con esa opción real de compilación.

## Validación pendiente

- 5.9 y 6.6: sesión autenticada y regresión HTTP integrada, con autorización específica.
- MySQL/HTTP reales de las nuevas composiciones, incluyendo collation, código no representable, plan preparado por servicio y repetición de su confirmación. No se infiere autorización de las corridas previas de repositorio.
- Reutilización completa de candidatos, carrera con escritores externos y adaptadores documentales; la consulta preventiva no reserva identidad ni garantiza ausencia de carreras. La ventana sin correlación inequívoca conserva INCIERTO y prohíbe replay.
- 8.3 en adelante: staging privado, recepción de bytes, validación de formatos, recuperación y coordinación por fases; integración visual y REC-01/02.

No se ejecutaron consultas contra bases reales, DDL/DML, login, E2E real, concesiones/revocaciones de permisos, cambios de perfil, transferencia de archivos ni efectos documentales. No se habilitó reutilización, no se aplicaron cuotas de ambiente y no se modificó Web.config. Los casos reales de este corte permanecen **NO_EJECUTADO**; 62/130 expresa tareas implementadas con el alcance local documentado, no aceptación o cierre de DOC-111.
