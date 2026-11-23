# DOC-125 — Implementación local 2026-11-23

**Backend local verificado; E2E real todavía no ejecutada.** 26/31 tareas completas.
Abiertas: 4.5 diagnóstico/aplicación manual, 5.3 aceptación nativa real de rollback/carrera,
5.4 matriz E2E real, 5.5 capacidad medida y 5.7 revisión final de aceptación.
La revisión técnica de Codex no representa aprobación humana ni autorización operativa.

## Cambios

Confirmar y ConsultarResultado reutilizan autoridad, datos y recursos del módulo.
El resultado histórico se recupera antes de leer archivo, plantilla o límites actuales.
El bootstrap puede emitir contexto de consulta con nuevas cargas cerradas por límites
inválidos. La nueva ejecución valida el archivo completo/snapshots v1 y bloquea/relee
ejecución y plantilla/campos antes de aplicar sus efectos.

Dos conexiones conservan los locks por ID y destino en MySQL5.1; filas y CONFIRMADA,
conteo y UTC comparten una transacción InnoDB. AGREGAR rechaza conflictos íntegramente;
REEMPLAZAR filtra empresa y revierte DELETE/INSERT ante fallo. CREAR se recupera por
procedencia/estructura propia y reconoce el commit implícito del DDL. Un commit o
limpieza inciertos no degrada una confirmación durable ni provoca replay automático.

## Validación ejecutada

| Comprobación | Resultado |
| --- | --- |
| Suite local final | 27/27, cero fallos; regresión heredada incluida. |
| Checks nativos | DOC122246 + DOC123318 + DOC124492 + DOC1253292 = 4348; ADO/autoridad sintéticos. |
| Build net461 aislado | Cero errores; 308 advertencias previas, cero en ImportacionExcel. |
| Refinamiento | PASS 15/15. |
| OpenSpec global estricto | PASS 96/96. |
| Playwright | Un test @doc125-confirm descubierto; cero autenticaciones/HTTP reales. |
| Libros de fixture | Cinco XLSX sintéticos verificados por lector DOC123: 1000/1000/1000/300/10 filas. |

Comando: `node --test tests/doc122-importacion-excel-behavior.test.cjs tests/doc122-e2e-reuse.test.cjs tests/doc124-e2e-reuse.test.cjs tests/doc125-e2e-reuse.test.cjs`.
El arnés existente compila/ejecuta las fuentes VB/C# reales con dobles ADO independientes;
no compara sólo texto ni acredita un servidor MySQL. Los nombres de locks del control
JS se contrastan con un vector obtenido del código VB compilado. El ejecutor nativo
rechaza falta de autorización antes de abrir recursos. Los libros se comprueban antes
de pedir credenciales.

DLL desplegada preservada: `e1767f58a8d784bc0f958b8c141efad7363669a793eb0eb5c164aefed071e730`.
Build aislado: `0646c354af0d6a7b1aaa5f61e3a880b9c58697a1a0e75a6e055d833e221d34c9`. No se desplegó.
Comandos, hashes UTF8/LF de 33 fuentes y límites: [JSON](doc125-implementacion-local-20261123.json).

## Recorrido real preparado

La suite/runner actuales incorporan cinco casos sobre tablas descartables exclusivas
de DOC125, con cuentas, archivos, hashes, conteos iniciales y máximos de efectos
congelados. Los casos positivos cubren tres modos; el rollback utiliza un segundo
UNIQUE sobre VALOR después de DELETE. El actor externo del ejecutor DOC115 escribe
una sola fila de fixture sin lock cooperativo y la conserva. Sólo CONFLICTO_CLAVE
acredita carrera; un rechazo de prevalidación no la acredita. Se observarán ambos
locks activos y después libres, sin afirmar inspección global del pool.

El acuse se ignora deliberadamente en el cliente y se consulta desde otra sesión:
no se presenta como corte físico de red o de COMMIT. No se añade un endpoint de fallos,
autenticación paralela, nueva conexión Web.config, siembra o limpieza automática.
Las huellas de CONTRATOS y demás destinos protegidos deben conservarse. Los resultados
DOC124 y sus 625 contratos no son autorización para nuevas escrituras.

## SQL y condiciones restantes

[SQL18–21 y guía](../Sql/README.md): ENTREGADOS/PENDIENTE_EJECUCION_MANUAL, no ejecutados.
SQL18 diagnostica; SQL19 crea dos fixtures; SQL20 registra dos fuentes; SQL21 tres
plantillas y tres filas sintéticas iniciales. Los campos de resultado ya existen
en las 22 columnas operativas. No se ejecutó SQL real ni se modificaron datos de
negocio por esta entrega. Cualquier adecuación requiere otro archivo/alcance.

La inspección exige TRIGGER efectivo y SELECT global para acreditar FK entrantes
desde otras bases. Si no se acredita cierra la capacidad; no concede ni recomienda
ampliar permisos automáticamente. Se debe revisar SQL18 antes de aceptar el ambiente.

## Límites de evidencia

No hay pausa durable por fila ni commits parciales: una interrupción conserva ID y
se consulta antes de repetir. ASP.NET puede serializar una misma sesión. Los 20000
configurados no son una medición de confirmación real; las fronteras locales acreditan
agrupación para 20000, rechazo del lector para 20001 y 10MB exactos/exceso. Los 12759
registros preparados en DOC124 no son benchmark de escritura. COLUMNA, CREATE fallido
y commit incierto tienen cobertura local; la matriz preparada no induce esos fallos
físicos en servidor. Los consumidores existentes siguen leyendo datos vigentes;
no se añadió congelación de datos diferidos. La interfaz/publicación de fuentes
permanecen en sus fases. No archivar ni cerrar Jira sin completar la aceptación real.
