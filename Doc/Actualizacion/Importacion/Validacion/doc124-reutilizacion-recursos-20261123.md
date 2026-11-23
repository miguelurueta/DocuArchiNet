# DOC-124 — Reutilización de E2E y tablas existentes

Instrucción del usuario2026-11-23: reutilizar las E2E existentes y las tablas
consumidas en este repositorio. Se corrige la exigencia anterior de otros destinos
descartables; no se cambia el contrato funcional de Preparar ni sus validaciones.

## Cambios verificados

- Conserva `run-doc110-profile-session.cjs --excel-prepare`, perfil DOC88,
  autenticación oficial, consola y `importacion-excel.spec.cjs` DOC122/DOC124.
- Retira la exclusión fija de EGRESOS/CONTRATOS. Un alcance con
  `destinationReadOnly=true` permite reutilizar tablas existentes sin declararlas
  descartables. Las operaciones siguen siendo recibir/preparar, sin confirmar,
  importar, activar o modificar el destino.
- Conserva `sourceFixture` DOC110: EGRESOS y CRÉDITOS, clave CONSECUTIVO. Usa
  los proveedores/fuentes reales del catálogo para identificar los recursos;
  no selecciona una tabla alternativa por tener un índice más conveniente.
- Metadatos por `queryRows` y huella completa de filas por `queryFingerprint`
  DOC32. La segunda evita el límite10000 de la vía Rows y no toma muestra/LIMIT.
  El máximo10000 del archivo Excel preparado se mantiene.
- Amplía `inspect-doc110-final-prerequisites.ps1 -Mode Doc124Excel`, reutilizando
  su conexión/configuración de módulo y funciones SELECT, sin otro inspector o
  cuenta. No consulta contraseñas de imp01_xmlconexion ni las publica.

Pruebas focales:14/14,0 fallos, sin autenticación. Incluyen recursos existentes,
huella completa estable/cambiada y ausencia/nombre de CREAR. Descubrimiento
Playwright:3 tests, mismos dos DOC122 y uno DOC124. OpenSpec estricto y auditoría
de refinamiento15/15. Sin cambios a producto VB, proyecto, binario o Web.config;
la evidencia nativa490/DOC122246/DOC123312 anterior corresponde al mismo producto.

## Inventario SELECT actual

El inspector existente identificó cinco catálogos visibles; sólo uno conserva las
22 columnas de ejecución. Su identidad coincide con el recurso de módulo utilizado
en la evidencia previa, pero la DA de una nueva sesión web aún debe corroborarse
durante la E2E. La última lectura completa ejecutó34 SELECT entre resolución de
módulo e inventario. Sólo guarda metadatos saneados en artefactos locales excluidos
de Git. No ejecutó HTTP, login, DDL, DML, activación o limpieza.

| Recurso existente | Observado | Consecuencia para la prueba positiva |
|---|---|---|
| imp_02_egresos | Referenciada por proveedor y sourceFixture; fuente2 activa/revisión2, CONEXION. No tiene UNIQUE completo de CONSECUTIVO. | El servicio debe rechazar la unicidad insuficiente; no corregir o borrar registros para producir PREPARADA. |
| imp_02_creditos | Referenciada por proveedor y sourceFixture; fuente3 inactiva/revisión6. Sólo PK técnica IMP_02_ID_CLAVE. | No habilitar automáticamente la fuente ni sustituir CONSECUTIVO por la PK. |
| Plantilla Excel EGRESOS | ID2, conexión histórica2, CAMPO_UNICO NULL, nombre legado sin imp_02_. | No equivale a una plantilla web compatible sobre la conexión de módulo inspeccionada. No renombrar/reasignar la histórica. |
| Plantilla Excel CREDITOS | ID6, conexión histórica3, CAMPO_UNICO NULL, flags0/0/0. | Requiere configuración revisada; no inventar un modo o clave desde la E2E. |
| Fixture Excel DOC122 | ID24, conexión6, revisión3, inactivo; tabla ausente. | Se conserva; no reactivar para conseguir un caso CREAR positivo. |
| imp_02_contratos | Tiene UNIQUE completo NCONTRATO; sin fuente/proveedor equivalente en el inventario consultado. | Su índice no lo convierte en reemplazo automático de las fuentes consumidas. |

La conexión histórica6 es la que coincide con el recurso local inspeccionado;
las referencias2/3 no coinciden. Esto es inventario de configuración, no autorización
para cambiar conexiones, clonar plantillas o modificar consumidores antiguos.

El primer inventario encontró catálogos legados sin CAMPO_UNICO/REVISION_CARGA
(error1054). El inspector ahora los marca con columnas faltantes y continúa su
inventario; no les aplica migraciones ni los declara compatibles.

## Estado y siguiente condición

E2E real de recepción/preparación: **NO_EJECUTADA**. Este inventario SELECT no la
sustituye. Permanecen21/23 tareas:2.6 y3.2 pendientes. No hace falta solicitar otras
tablas de prueba: se reutilizan las fuentes existentes, manteniendo sus datos.
Para una aceptación positiva completa debe resolverse de forma revisable la
configuración de plantilla/clave/política/índice que hoy no cumple; el runner no la
arregla ni trata un rechazo correcto como prueba de PREPARADA.

Se entrega [SQL12](../Sql/12-preflight-recursos-e2e-doc124.sql), exclusivamente
SELECT para comprobar esas condiciones manualmente. No es un ALTER ni una solicitud
de limpieza. Cualquier ajuste permanente necesita su archivo específico y la
ejecución manual acordada. El sitio conserva la DLL anterior; no se desplegó esta
fase ni se reutilizaron secretos de ventanas cerradas.

El [JSON](doc124-reutilizacion-recursos-20261123.json) registra hashes de archivos,
recursos reutilizados y alcance. Los informes de validación local anteriores
permanecen como evidencia de su corte, sin atribuirles este nuevo inventario.
