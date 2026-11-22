# DOC-122 — Revisión final OpenSpec

Resultado **APROBADO**, sin hallazgos críticos ni advertencias pendientes para la fase02. Revisión Codex2026-11-22 sobre código d218ef04 y la evidencia vinculada; las correcciones finales de propósito, redacción y alcance de los artefactos son documentales.

| Dimensión | Resultado |
|---|---|
| Completitud |14/14 tareas y4 artefactos completos. |
| Corrección |8/8 requisitos mapeados:7 añadidos y1 modificado;22 escenarios contemplados. |
| Coherencia |D-01 a D-07 seguidas, incluida la resolución DA del módulo solicitada por el usuario. |
| Evidencia |246 controles nativos, build aislado, regresión,6 pruebas de reutilización y E2E real2/2. |
| Revisión de cambios |Sin cambios de código .NET desde c88d8a37; test E2E final identificado por hash. |

## Cobertura de requisitos
| Requisito | Implementación revisada | Evidencia y alcance |
|---|---|---|
| RQ-01: autoridad propia | ImportacionExcelContextAdapter.Resolver; indicador de permiso en Gestion/Class_remit_dest_interno_perfil_produccion.vb. | Usuario activo/empresa/permiso exacto reconsultados en RA. Native: revocación, inactivo, empresa ajena, permiso exclusivo. Real: cuenta autorizada y cuenta restringida. |
| RQ-02: DA del módulo | ImportacionExcelConexionAdapter.CadenaDA/ConexionVigente/LeerReferencias y resolver compartido. | Empresa/módulo, origen capturado, ID histórico y servidor/base/proveedor. Native cubre manipulación/cambio de módulo; real correlaciona fixture API con control ODBC. No se seleccionan credenciales del catálogo ni se exige conexión adicional. |
| RQ-03: DTO/metadatos | DTOs/ProduccionDocumental/ImportacionExcel; ServicioPlantillasExcel.ValidarDestino; MySqlPlantillaExcelRepository.Listar/LeerDestino. | Paginación/filtro parametrizados, prefijo y nombres acotados, columnas/tipos físicos comprobados y ampliaciones conservadas. Native cubre reglas y destino existente; real cubre listado/detalle y modo CREAR con tabla ausente. |
| RQ-04: atomicidad | MySqlPlantillaExcelRepository.Guardar/GuardarCampos. | InnoDB, SELECT FOR UPDATE, revisión y huella, parámetros, lectura antes de commit y rollback. Native acredita rollback tras fallo del segundo campo; real acredita un ganador/un conflicto. |
| RQ-05: compatibilidad | ServicioPlantillasExcel.Huella/ModoHistorico/FormatoHistorico; persistencia por ID de campo. | Vector v1 independiente, NULL explícito y ajustes JSON globales aislados. Native conserva ruta legacy/IDs/transformaciones y consulta sin clave; real conserva ruta y mapeos del fixture. |
| RQ-06: solicitud | ImportacionExcelRequestGuard y WebServiceImportacionExcel. | POST JSON, límite de cuerpo, origen, propósito CSRF y contexto; DTO tipados sin secretos. Reflexión SOAP nativa; rechazo CSRF real. |
| RQ-07: operación/evidencia | Harness DOC110 reutilizado, runner de perfiles existente, SQL10 SELECT y evidencia saneada. | E2E real2/2,23 SELECT y14 llamadas de servicio. Datos protegidos sin cambios; SQL de instalación no ejecutado. |
| DOC121 D-04/RQ-04 modificado | Delta de conexión y contrato de configuración vigente. | Sustituye la cuenta adicional por la DA del módulo conforme a la revisión autorizada. Mantiene permiso propio, límites positivos y controles de ámbito/destino. |

Los22 escenarios de ambos deltas se contrastaron con estas rutas y evidencias. La E2E real es un subconjunto explícito; no se presenta como ejecución física de todos los escenarios negativos nativos.

## Coherencia y correcciones documentales
La propuesta describe ahora los componentes entregados y las dos capacidades afectadas. Se añadió propósito explícito para la capacidad nueva y se corrigieron cuatro frases normativas incompletas. La fuente Jira se mantiene como antecedente; la revisión de conexión aprobada está identificada en diseño/refinement y delta DOC121.

La sincronización selecciona únicamente los dos spec.md normativos enumerados por openspec status. jira-context.md es la captura del ticket y no se convierte en una capacidad. El merge de DOC121 conserva sus otros siete requisitos y todos los escenarios no afectados.

## Límites conservados
Esta entrega administra plantillas. No recibe ni importa el Excel aportado, no cambia el límite10000/10MB y no valida fases03–07. El archivo aportado excede10000 filas y su tratamiento corresponde a una decisión posterior, sin ampliar límites en este cierre.

La cuenta MySQL se comparte con el módulo por la decisión revisada. No se certifican privilegios mínimos ni se cambian cuentas o concesiones. El escritor antiguo puede modificar configuración después del commit, de modo que preparación/confirmación futuras deben revalidar huella.

La concurrencia real usa una sesión ASP.NET: demuestra revisión optimista, no dos escritores MySQL simultáneos. El rollback por fallo parcial conserva evidencia nativa sintética y no se confunde con una inyección de fallo real. El paquete completo de contenido web no se certifica; sí el build de código aislado y la DLL desplegada con WSDL200.

## Referencias
- [E2E real](doc122-e2e-real-20261122.md) y su JSON:2/2,23 SELECT,14 llamadas y huellas protegidas.
- [Conexión reutilizada](doc122-conexion-reutilizada-20261121.md):246 controles nativos y build.
- Cambio OpenSpec DOC122: propuesta/diseño/specs/refinement/tasks,14/14 tareas.
- La evidencia privada y las referencias del fixture permanecen fuera de Git; no se vuelve a ejecutar la E2E durante la revisión ni el archivo.
