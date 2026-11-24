# DOC125: ejecución real y capacidad medida

Corte 2026-11-24T14:43:15.282Z. Aceptación real5/5 y revisión técnica verificadas;31/31 tareas. SQL18–21 aplicados/verificados por delegación expresa «ejecuta» fuera de tests; fuentes6/7, plantillas29/30/31, conexión6. Sin grants ni limpieza. DLLa14034d586facb7f700ff1cdf92c22bf74e607e69b43d3b3ab7a8556efc2e0b0, WSDL200, Web.config intacto.

Se confirmaron1000filas por CREAR44cf8b1c-13b7-4408-a705-99bf94f13ef6, AGREGAR e32e318a-16cc-4955-8f52-a418e6338a71 y REEMPLAZAR ee834d0a-cb94-4e2a-a6de-c7c48c8da6bc. La ventana5 agotó300s durante controles SELECT después de confirmar; se conservaron los IDs y se recuperaron sin reimportar. La ventana6 verificó el rollback real04f83353-65a9-46c6-8ff5-9a7a6af44a2f: archivo300 con VALOR repetido, FALLIDA/CONFLICTO_CLAVE,0confirmadas y hash íntegro de las1000filas previas. CREAR1000, AGREGAR1002 y REEMPLAZAR1000 al último checkpoint; dos cargas antiguas PREPARADA/0 se conservan. CONTRATOS625, EGRESOS, CRÉDITOS y configuración anterior conservan sus huellas completas.

La carrera2c97bd2f-5ad4-4c1c-aa73-eb739499a349 terminó FALLIDA/CONFLICTO_CLAVE con0filas propias confirmadas. El actor externo conservó exactamente1fila, acreditando lock, INSERT del dueño del lock observado y commit externo; exit0. El fallo previo ODBC UInt32 ocurrió antes de insertar y se corrigió con BigInt/Int64 y preflight sólo lectura. La ventana7 pasó5/5 en427861ms con136controles SELECT, consultando las tres confirmaciones y el rollback ya acreditados sin repetir esas escrituras; continuó el mismo ID de carrera. Los cinco casos corresponden a evidencia conservada de las ventanas5/6/7. Preflight propietario OK y cuenta restringida LOGIN_OPEN_ACCESO_DENEGADO antes de efectos. Sesiones terminadas y secretos eliminados.

La marca anterior excedía60caracteres en MySQL5.1. Ahora docuarchi/x1/ más SHA256/base64url vincula UUID/EsquemaHash completos en56ASCII; se reconoce una marca literal previa únicamente si coincide íntegramente con la misma identidad/esquema, sin ALTER. [Límite documentado por MySQL](https://dev.mysql.com/doc/relnotes/workbench/en/news-5-2-41.html). La inspección global FK medida10–15s se acredita una vez por operación/conexión/base/destino: presupuesto20s específico, restoSQL15s y HTTP30s, sin caché entre sesiones. Siguen comprobándose permisos/TRIGGER/motores/esquema/autoridad/locks; se excluye DDL administrativo concurrente. El diagnóstico real anterior acreditó snapshots v1 idénticos y lectura completa1000, con0DML/CREATE.

El presupuesto del protocolo E2E es900s por cinco casos y múltiples procesos SELECT medidos; no amplía los30s de cada HTTP, no activa reintentos y no representa tiempo de escritura. La carrera sólo confirma la fila externa tras observar por SELECT el INSERT del dueño del lock sobre el destino exacto, con máximo20s; el padre exige observación, commit externo y CONFLICTO_CLAVE/rollback del módulo. Una pausa fija no acredita esa carrera.

## Mediciones de las operaciones reales

Tiempos del cliente por petición, en ms: incluyen transporte, lectura/revalidación, metadatos y DML; no son tiempo SQL puro. La pareja concurrente AGREGAR usa el mayor tiempo de las dos peticiones efectivas.

| Modo | Filas confirmadas | Preparar ms | Confirmar ms |
| --- | --- | --- | --- |
| crear | 1000 | 240 | 13177 |
| agregar | 1000 | 187 | 11061 |
| reemplazar | 1000 | 188 | 11464 |

Rollback300: Preparar130ms, Confirmar fallido11288ms. Carrera real: Confirmar11912ms. Las peticiones CSRF/reemplazo sin consentimiento/replay se distinguen en el JSON; no se suman como escritura. Los controles SELECT PowerShell dominan el presupuesto del protocolo y no acreditan lentitud equivalente de importación.

Muestreo IIS: 104muestras entre 2026-11-24T12:52:09.9810382Z y2026-11-24T12:55:38.2675476Z, máximo observado RSS agregado279072768bytes y privados581500928bytes. Incluye procesos IIS compartidos, empieza después del inicio de la ventana y puede omitir picos; no mide asignación exclusiva por carga ni permite extrapolar memoria para20000filas. El JSON conserva cobertura temporal y métricas separadas.

Se conservan20000filas/10000000bytes configurados, predeterminado lector10000,128parámetros y min(65536,max_allowed_packet/2)bytes por agrupación, una sola transacción DML por carga. La capacidad real medida es1000filas por modo; las12759 anteriores sólo se prepararon y los20000/20001 se verificaron localmente. No se certifica confirmación física de20000, pausa por fila, rendimiento bajo cualquier carga concurrente, COLUMNA físico ni cortes reales de COMMIT/red. El acuse ignorado es una decisión del cliente; los fallos antes/después de COMMIT y DDL superviviente tienen cobertura sintética explícita.

Verificación vigente:94/94tests,4492comprobaciones nativas sintéticas y regresión heredada PASS; MSBuild net4610errores/308advertencias previas/0en ImportacionExcel. No se modifica framework, conector6.2.3, SII, importación de carpetas, menús ni interfaz fase06. [Revisión final de implementación](doc125-verificacion-final-20261124.md):15requisitos y44escenarios revisados, sin hallazgos de implementación. No se ejecutó archivo ni cierre de Jira. Cortes previos en doc125-instalacion-ejecucion-20261123 y doc125-e2e-parada-correccion-20261124; los intentos detenidos no son PASS.
