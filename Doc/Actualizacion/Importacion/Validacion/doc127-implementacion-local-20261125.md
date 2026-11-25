# DOC-127 — Implementación local y aceptación pendiente

**19/33 tareas verificadas.** Corte de fuentes: `10f307112c6239faa8180a021a8064eccc25b538` más las huellas de la entrega en el JSON. Pendientes: 2.4, 4.1, 5.1, 5.2, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7, 7.1, 7.4, 7.5. Se reutilizan el runner DOC110, suite Excel, scopes DOC125/126, autenticación, recursos de conexión y controles SELECT DOC32. No se declara cierre de Jira ni aceptación real nueva.

## Resultado local

MSBuild net461 aislado: **0 errores / 308 advertencias**; DLL instalada y configuración conservadas. Regresión inicial **193/193**, con 4607 checks nativos del corte anterior a la prueba adicional de bytes. Regresión focal final de adaptadores **59/59**; nativo final **4616 checks** (DOC122307, DOC123377, DOC124492, DOC1253440) y regresión heredada PASS. **19/19 SII**; **1/1 prueba reproducible de18 libros**. Los conteos de suites se solapan y no se suman como ejecuciones distintas.

Se añadieron libros ZIP/XML válidos de10.000.000/10.000.001 bytes: el primero se lee completo, el segundo es válido con11MB y se rechaza por tamaño con10MB. Permanecen las fronteras locales10000/10001 y20000/20001 y cambios de configuración. El generador DOC123 conserva los nueve hashes originales y produce18 combinaciones locales; el lector del producto validó todas. No son18 importaciones MySQL.

La extensión de capacidad ejecuta una combinación por ventana, valida runtime/configuración/archivos y consume la autorización antes del login. El control previo verifica cuenta, ausencia de uso del archivo y huella/conteo del destino. Conserva ID antes de confirmar y detiene nuevos efectos ante incertidumbre. Registra tiempos HTTP por etapa y muestreo IIS compartido cada2s; una confirmación sin medición suficiente conserva su resultado y queda incompleta. Pruebas de transporte y memoria son sintéticas; el smoke del monitor sólo produjo un baseline sin carga.

## Comprobación de instalación

**19 SELECT, cero SQL modificador y cero sesiones autenticadas de aplicación.** Incluye un intento interrumpido tras un SELECT de catálogo (error del nombre del helper, corregido), inventario de16 SELECT y2 SELECT adicionales de metadatos RA. Conexión reutilizada del catálogo/módulo: empresa2, DA11/RA12. Secretos sólo en memoria, sin guardarlos o mostrarlos.

Se contrastaron tipos, ordinales, nulabilidad, defaults, charset/collation de las22 columnas, cabecera11/12, PK/tres índices secundarios completos y cinco permisos. Sin diferencias de ese contrato; relaciones/triggers ausentes según visibilidad de la cuenta. Cuentas141/194/196 mantienen permisos esperados. Los cinco resultados y conteos DOC126 permanecen: agregar1002, reemplazar1000, asistente1000. Estos conteos no sustituyen las huellas completas de una futura ventana. No se aplicó ni reaplicó SQL00–27.

Entregados [28 preflight](../Sql/28-preflight-doc127.sql) y [29 EXPLAIN manual](../Sql/29-explain-proveedor-doc127.sql). El proveedor activo1/versión1 consulta EGRESOS por CONSECUTIVO/TEXTO y devuelve NIT/BENFICIARIO; el alias histórico se conserva. El agente no ejecutó EXPLAIN ni modificó EGRESOS. Archivo29: **PENDIENTE_EJECUCION_MANUAL**, plan aún no aportado. Ver [orden/estados](../Sql/README.md) y el inventario JSON por archivo.

## Matriz de requisitos del corte

| Requisito | Evidencia/caso | Estado | Límite |
| --- | --- | --- | --- |
| RQ-01 | DOC127_SOURCE_AND_EVIDENCE_AUDIT | LOCAL_PASS | La huella de una fuente no transfiere toda la aceptacion de un runtime diferente. |
| RQ-02 | tests/doc127-e2e-reuse.test.cjs | LOCAL_PASS_REAL_PENDIENTE | Una celda por ventana; sin instalar/sembrar/limpiar; aun sin ventana DOC127 autorizada. |
| RQ-03 | DOC122_NATIVE; DOC124_NATIVE; DOC126 lectura real v6 | LOCAL_PASS_HISTORICA_LIMITADA | Revocacion fisica y lectura real v7 no ejecutadas en DOC127. |
| RQ-04 | DOC127_SELECT_CONTRACT; Sql/README.md | SELECT_PASS_ENTREGA_SQL | Controles actuales equivalentes, no ejecucion manual integra de28; metadatos sujetos a visibilidad. |
| RQ-05 | DOC122–125_NATIVE; DOC127_LOCAL_READER; DOC126 reales | LOCAL_PASS_REAL_PARCIAL | 18 libros locales no equivalen a18 importaciones. XLS real por modo sin completar. |
| RQ-06 | DOC124_NATIVE; DOC125_NATIVE; DOC126 rollback/carrera | LOCAL_PASS_HISTORICA_REUTILIZADA | Escenarios adversos de motor/FK/trigger y politica COLUMNA tienen cobertura sintetica. |
| RQ-07 | DOC125_NATIVE; DOC126 rollback/carrera; transporte DOC127 | LOCAL_PASS_HISTORICA_REUTILIZADA | Acuse ignorado no acredita corte fisico COMMIT/red. No se repiten las carreras anteriores. |
| RQ-08 | ImportacionExcelLecturaTests.Boundaries/Reports; ConfirmacionTests.Rejections | LOCAL_PASS | Fronteras grandes y cambios de configuracion son locales; IIS permanece intacto. |
| RQ-09 | DOC127_LOCAL_READER; DOC125 tiempos historicos | CAPACIDAD_REAL_PENDIENTE | Sin XLS/XLSX5000/10000 confirmadas ni benchmark20000; monitor local sin carga no mide capacidad. |
| RQ-10 | harness DOC110;19 tests SII;29-explain-proveedor-doc127.sql | LOCAL_PASS_EXPLAIN_PENDIENTE | Plan manual no aportado; importar tabla no publica proveedor/fuente. |
| RQ-11 | tests/doc126-importacion-excel-ui.test.cjs; client | LOCAL_PASS_REAL_V7_PENDIENTE | La lectura real anterior corresponde a assets v6. |
| RQ-12 | MSBuild net461; logs locales por SHA | LOCAL_PASS | Cero despliegues; el binario compilado aislado no sustituye el instalado. |
| RQ-13 | Sql/README.md; Operacion-Excel.md | DOCUMENTADO | La carga no ofrece pausa por fila; cualquier cambio SQL sigue el procedimiento manual. |
| RQ-14 | Este corte y tareas OpenSpec | CIERRE_ABIERTO | Capacidad/autorizacion/EXPLAIN y aceptacion vigente impiden declarar cierre. |

## Capacidad pendiente

| Formato | Filas | Lector local | Confirmación física |
| --- | --- | --- | --- |
| XLS | 1000 | PASS, tres modos | NO_EJECUTADA |
| XLS | 5000 | PASS, tres modos | NO_EJECUTADA |
| XLS | 10000 | PASS, tres modos | NO_EJECUTADA |
| XLSX | 1000 | PASS, tres modos | HISTORICA_TRES_MODOS; metodologia y runtime originales |
| XLSX | 5000 | PASS, tres modos | NO_EJECUTADA |
| XLSX | 10000 | PASS, tres modos | NO_EJECUTADA |

La evidencia DOC125/126 de1000 XLSX por modo conserva su SHA, runtime y límites originales; los tiempos DOC125 son HTTP e incluyen metadatos/transporte y su memoria es agregada/parcial. DOC1248000/12759 sólo acredita preparación. No se atribuye5000/10000 confirmadas por ambos formatos ni20000 físicas al runtime actual. La lectura real v6 no se convierte en lectura real v7.

## Dependencias para continuar

1. Ejecutar manualmente29 y aportar el plan. Revisar28 por destino si se desea reproducir el preflight; no reinstalar objetos compatibles.
2. Resolver las referencias XLS y siguientes CREAR sin editar las plantillas aceptadas. La plantilla57 sigue sin destino físico;58/59 son XLSX y sus destinos ya contienen datos. La preparación administrativa/configuración y cualquier archivo adicional se realizan fuera de E2E.
3. Cerrar scopes nuevos con referencias/huellas actuales, efectos y cuentas expresamente autorizados por ventana. Los18 archivos privados ya son legibles; su existencia no autoriza las18 escrituras. La aceptación real v7 y capacidad siguen abiertas.

Guía de operación entregada en [Operacion-Excel.md](../Operacion-Excel.md). La declaración manual previa del usuario sigue reconocida y no se pide repetirla para este informe. Datos de ejecución, libros, muestras y manifiestos permanecen excluidos de Git. Revisión técnica de Codex; sin aprobación humana global atribuida.
