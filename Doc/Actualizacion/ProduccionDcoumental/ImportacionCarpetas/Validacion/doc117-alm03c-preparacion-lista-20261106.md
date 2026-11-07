# DOC-117 ALM-03C: preparación nueva lista para autorización

El ejecutor de preparación del caso nuevo está implementado y validado localmente: **333/333 pruebas PASS**, `--check` y OpenSpec estricto PASS. **No se ejecutó autenticación, SELECT real, recepción o creación de recursos en este avance.** DOC-117 sigue 16/18, con 17.1 y 18.1 abiertas.

## Alcance revisable de esta fase

| Concepto | Límite propuesto |
| --- | --- |
| Ambiente/cuenta | `https://localhost/GestionDocumental-Docuarchi.net/`, usuario 141/empresa 2 |
| Controles | Cuenta MySQL exclusivamente SELECT; DSN registrados MembershipUsers |
| Importación | Nivel 7, versión publicada 38, gabinete 10, tipología 79, índice 14 TEXTO `EEE` |
| Raíces nuevas | `DOC117-ALM03C-A-20261106` y `DOC117-ALM03C-B-20261106` |
| PDFs nuevos en staging | `DOC117-ALM03C-IMPORT-A-20261106.pdf` y `DOC117-ALM03C-IMPORT-B-20261106.pdf`, 677 bytes cada uno, hash aprobado |
| Creaciones máximas | Un plan, un lote, dos recepciones, dos expedientes nuevos y dos materializaciones de valores |
| Documentos/ALMACENAR | Cero; los dos comandos futuros solo se sellan privadamente, sin enviarlos |
| Destino individual | Expediente existente 467/nivel 8/relación 1072/tipología 50: comprobar autoridad, clasificación, ausencia del nombre nuevo y descripción canónica; cero cargas individuales |
| Nombre individual reservado | `DOC117-ALM03C-IND-20261106.pdf`; se prepara únicamente la copia local, no se sube |
| Runtime | Cero instalaciones o activaciones; conservar DLL original y 912 entradas originales |
| Preservación | Todos los recursos anteriores, incluidos lotes 18–22, A 16224/23322/intento 58 y B/archivo 37 sin iniciar; conservar también todos los recursos nuevos |
| Reintentos/limpieza | Ninguno; sin replay, reconciliación, reparación, cambios de permisos/configuración/esquema o borrado |

Los IDs del plan, lote, raíces, archivos, expedientes y relaciones futuros serán los devueltos por el servidor. No se predicen ni se reservan mediante SQL. Se rechazan los IDs históricos y cualquier plan/lote que esté dentro de los límites del inventario preservado antes de recibir PDFs o avanzar fases. Se mantienen los controles originales de publicación, contexto, originales, valores efectivos y ausencia documental.

## Ampliación de presupuesto pendiente

El acumulado vigente es **250 SELECT**. Hasta el techo histórico 320 quedan 70; la preparación anterior consumió 80 y este caso añade la lectura canónica y comprobaciones de documentos. No se inicia suponiendo que el saldo sea suficiente.

Se propone **hasta 160 SELECT adicionales, contados desde 250, con techo acumulado 410**. Esto requiere una aprobación expresa para ampliar el techo **320 → 410**. El permiso anterior y un simple contador en cero no lo conceden. El ejecutor exige `budgetIncreaseAuthorized=true`, los tres valores 250/160/410, ambiente, cuentas, efectos de preparación y huellas exactas de propuesta, código y calculadora .NET. Las banderas públicas de ejecución y ampliación siguen en `false`; no se creó ninguna concesión real.

El contador reserva cada solicitud antes de acceder al puente SELECT, carga también solicitudes fallidas y rechaza la 161 antes del acceso. Un estado inconsistente o reiniciado se rechaza. Si existe una corrida ALM-03C anterior, aunque se haya detenido, este ejecutor exige revisión de lo consumido y no permite otra invocación automática.

## Corrección canónica integrada y cierre de preparación

Antes de cualquier efecto se obtiene una descripción inequívoca del catálogo, contextualizada por empresa, expediente, instrumento activo y tipología 50. La descripción exacta y su SHA-256 se fijan en el alcance V2 del caso nuevo; no se imprime ese valor ni se registran secretos. Al final se relee el catálogo y se rechaza cualquier cambio, aunque la nueva representación sea equivalente. La descripción no se sustituye por el literal mixto que causó el rechazo anterior.

El inventario global inicial debe seguir en 8814. Sus conteos y las huellas de los documentos del expediente 467 deben coincidir al final. La preparación compara también la preservación de planes/lotes/raíces/archivos/intentos/eventos anteriores y verifica los originales nuevos recibidos. Si falla una respuesta, lectura, huella, creación o materialización, registra etapa, códigos cerrados e identidades disponibles y se detiene sin retry.

La salida correcta entrega privadamente un alcance `ALM03_CANONICAL_CLOSED_RESOURCES_BEFORE_WRITER_V2`, las dos identidades nuevas reales y sus solicitudes futuras selladas. **No autoriza ejecutarlas.** Después harán falta adaptar y revisar la candidata activa y el ejecutor de coexistencia con ese alcance, compilar y ensayar restauración con sus bytes, y obtener otro permiso específico para documentos/runtime y su presupuesto. No se repite ni continúa el lote 22.

## Evidencia local

- Propuesta: `doc117-alm03c-recursos-propuestos-20261106.json`, SHA `ae0f3ae1c7d1cf62e3c4d51dc5d48e1482e726d2f0d48c5955fc0233ae9f7dfc`.
- Código revisado: `8f5bb24cc2e9a4a1bcaa17aefcd6185a12e3373f06821d80fa42c5aaf391066c`.
- Ejecutable de cálculo .NET copiado y verificado fuera de bin: `.tmp/doc117-alm03c-hash-tool-20261106/Doc117SolicitudHash.exe`; no modifica ni instala el sitio.
- Regresión conjunta final: `.tmp/doc117-alm03c-setup-reviewed-regression-20261106.tap`, **333/333 PASS**. Incluye fallos de catálogo, inventario/documentos, IDs históricos, respuesta perdida, creación incierta, preservación, calculadora, presupuesto y saneamiento de errores; ningún ALMACENAR enviado.
- Comprobación sin efectos: `node tools/e2e/scripts/run-doc117-alm03c-setup.cjs --check`.
- Certificado independiente: `doc117-alm03c-setup-local-20261106.json`, con huellas de fuentes, presupuesto, calculadora, prueba real anterior e invariancia local.

La DLL original, cuatro respaldos, 912 entradas originales, candidatos y artefactos históricos permanecen conservados. Esta preparación todavía está **NO_EJECUTADA**: **250 SELECT reales acumulados, 16/18**, sin crédito nuevo de concurrencia, serialización, XML/SII, cierre o archivo.
