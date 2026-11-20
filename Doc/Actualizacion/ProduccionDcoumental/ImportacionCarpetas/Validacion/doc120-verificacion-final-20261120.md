# DOC-120 — revisión final OpenSpec (6.5)

Skill aplicada: openspec-verify-change. Revisión de completitud/corrección/coherencia sobre `aee3fe8eb5023546b134266c6e14ff66b94f8a02` y la consolidación documental de esta revisión. El cambio es spec-driven;15 requisitos/114 escenarios normativos. El SHA final se vincula en los registros locales OPSXJ después del commit, evitando una referencia circular dentro del propio documento.

## Completitud y compuertas

267/270 tareas estaban completas al iniciar.3.187 tiene PASS MySQL real; quedan aceptación6.4, consolidación/gobierno6.5 y entrega6.6. El trabajo técnico de6.5 se consolida aquí, con evidencia y revisión registradas sobre el SHA de entrega.6.4 no se cierra por redactar la propuesta de límites y6.6 no se presenta como merge/archivo ya realizados. No hay autorización para repetir E2E o borrar datos por esta revisión.

## Correspondencia de requisitos

| Requisito y escenarios | Implementación/evidencia revisada | Dictamen |
| --- | --- | --- |
| RQ-01 (2) | Primarios conservan SHA/runtime/fuentes.154 huellas de655467f4 coinciden con las entradas actuales; el backend b0b5d0d3 coincide. Vista/ASPX posteriores a08bc33cc se identifican como diferencia de presentación cubierta localmente. | Procedencia explícita; una huella distinta no se convierte en fallo o nueva E2E. |
| RQ-02 (2) | matriz-verificacion.md/resultados.md enlazan casos, componentes, primarios y límites. | Índices actuales y cronología conservada. |
| RQ-03 (2) | EXP/ALM/COORD/SEG y variantes de laboratorio mantienen alcance y estados;3.187 se añade como prueba nativa independiente. | Sin sumar pruebas superpuestas ni rehabilitar STOPPED. |
| RQ-04 (45) | Barra/asistente/consulta/árbol y cliente de importación; ejecutor existente test-doc120-transitions.cjs. Manual3.87/3.127 y E2E08bc33cc conservados. |226/226 locales aprobados en esta revisión; cobertura real/manual por sus primarios. |
| RQ-05 (35) | Coordinador, barreras/valores, protocoloV2, ControlFocal/ControlArchivoIndependiente; regresiones Framework y carreras/rollback MySQL655467f4. | Sin fallo crítico abierto comprobado de autoridad/destino/integridad/recuperación; no replay por incertidumbre. |
| RQ-06 (2) | ValidarMetadatos y degradación histórica/local; auxiliar de menú corregido y probado. | Acotado a incompatibilidades comprobadas; otras variantes no se inventan. |
| RQ-07 (6) | Medición optativa por etapas, cuatro CSV y1.000 funcionales consolidados. | No falta repetir la importación; máximos/SLA/memoria/disco excluidos de certificación propuesta. |
| RQ-08 (3) | Consola privada, manifest/run únicos, presupuestos y selectores de E2E existentes; ventanas cerradas/claves eliminadas. | Autorización por ventana; ningún nuevo efecto real de esta revisión. |
| RQ-09 (4) | Snapshots/planes y evidencias previas protegidos;17 grupos previos/5 del fixture en655467f4; limpieza sin efectos reales nuevos. | Conservación y exclusiones verificables. |
| RQ-10 (2) | Correcciones con reproducción/regresión y matriz de aceptación/decisiones explícitas. | Pendiente aceptación operativa expresa6.4; no la concede CI. |
| Aislamiento de estados (5) | Contextos consulta/revisión, filtros y borradores; doc120-auditoria-transiciones-browser y suites relacionadas. | Incluido en226/226 locales. |
| Guardado real y pausa (2) |08bc33cc, cuatro documentos RA/DA y pausa/cancelación/cierre/recarga con acuse controlado. | PASS real en su corte; sin presentar retención de entrega como transacción SQL mantenida abierta. |
| Encabezado (1) | Cambioeace9bcf y regresión de estados de UI. | Incluido en226/226; texto no cambia negocio. |
| Lote completado (1) |7ace0d25,10 SELECT de control,1.000 identidades únicas y expediente696/relación1120. | Metadatos reales, sin prometer integridad física o capacidad total. |
| Acceso al expediente (2) | Buscador/inserción existentes por relación/nivel/ID, ramas contraídas y contenedores ocultos; pruebas del árbol. | Incluido en226/226; no selección por código repetido ni nuevo efecto. |

Los114 escenarios se inventarían por requisito; no se afirma114 sesiones reales ni una cifra automática de cobertura de líneas. Se contrasta el mapa con el diseño y las pruebas/evidencias existentes.

## Coherencia y correcciones documentales

Los cinco documentos obligatorios de gobierno aún describían en sus secciones principales el corte inicial «sin cambios de producto/servicios/DTO». Se corrigen a la implementación entregada, manteniendo esos textos como antecedentes identificados. Se actualiza el resumen vigente de los artefactos y los índices sin reescribir primarios. No se cambian producto, cuotas, políticas, DLL, configuración o datos.

## Verificación reproducible

Regresión local: `node tools/e2e/scripts/test-doc120-transitions.cjs`,226/226,202,8604199 s; log excluido `.tmp/doc120-final-local-transitions.log`. Sus entradas se sellan en el JSON compañero. Las35/35 pruebas del ejecutor y2/2 Framework/DDL del corte6fc5d343 se reutilizan por entradas sin cambios, junto al PASS real655467f4; no son nuevas ejecuciones de BD.

OpenSpec estricto y auditoría refinement15/15 se comprueban sobre los artefactos consolidados. La evidencia manual se registra como confirmación previa del usuario y revisión de procedencia, sin afirmar una nueva sesión IIS. Estado global de gobierno debe distinguir el PASS de documentación/revisión/evidencias del bloqueo restante por tareas de aceptación/entrega; no se debilitan compuertas.

## Resultado

Corrección técnica/coherencia: dictamen favorable, sin defecto crítico abierto hallado dentro del alcance comprobado. Completitud de salida: **no listo para archivo mientras6.4/6.6 sigan pendientes**. La propuesta concreta y responsables están en [matriz de límites](doc120-aceptacion-limites-final-20261120.md). PR y SHA se registran en la entrega; archivo y Jira no anteceden a aceptación y merge verificado.
