# DOC126 — Interfaz de carga y menú

Corte inicial: 40/44. La ampliación del asistente alcanza 47/52; aceptación real pendiente. Contratos y decisiones en [OpenSpec](../../../../openspec/changes/doc-126-importacion-excel-06-interfaz-menu/design.md). [Evidencia local](../Validacion/doc126-implementacion-local-20261124.md).

## Acceso e integración

RenderImportacionExcel consulta el permiso vigente propio y entrega únicamente rutas de aplicación y namespace opaco. El menú reutiliza asegurarMenu/rotularAccesoMenu existentes, sin exigir hosts o permisos de carpetas. El ASPX tolera el binario anterior mediante invocación opcional del renderer; con la DLL nueva la autoridad funcional sigue siendo el permiso. Los tres assets figuran como Content. Reinicializar/endRequest reemplaza sólo nodos/listeners Excel.

## Contrato de carga

Apertura POST obtiene token/límites/conexiones. El cliente acepta el envelope Excel con o sin .d y conserva datos de validación fallida. Envía cabeceras CSRF/contexto propias; la cabecera contextual opcional del servidor protege el tab antiguo sin cambiar DTOs o métodos DOC122–125. La recepción conserva el ID antes de enviar los cuatro campos multipart y el archivo; FormData genera el boundary. Hoja procede del libro recibido y modo de la plantilla, sin guardar configuración por seleccionar. Preparar usa CAS vigente; cambiar intención revoca confirmabilidad y consentimiento.

## Plantillas y resultado

El editor conserva IDs, transformaciones/null y revisión/huella. Un guardado incierto no repite el alta. Confirmar congela ID/hashes y exige aceptación del contenido autorizado para reemplazar; su transacción/unicidad pertenecen al backend DOC125. La consulta por ID recupera el resultado sin reimportar. sessionStorage guarda sólo hasta 20 pares ID/conexión en namespace contextual; sin almacenamiento se avisa y conserva memoria/consulta manual. Una recarga no reconstruye una intención confirmable. PREPARADA en una consulta no demuestra rollback.

## Presentación y verificación

Muestra e incidencias acotadas a 25 con totales, números canónicos como texto, fechas sin desplazamiento y render seguro. Estados visibles y aria-live; un solo envío, generación contra respuestas tardías, foco/Tab/Escape y scroll de acciones. Geometría ajustada al viewport visual y zoom; validaciones locales en 320x568,768x1024,1280x720 y 200%.

## Instalación y límites

[SQL 22–25](../Sql/README.md) se entregan para ejecución manual separada. Reutilizan la instalación y preparan fixtures nuevos, sin tocar históricos. Pruebas no instalan, siembran, limpian ni conceden permisos. Mantener net461/MySQL5.1.37/MySql.Data6.2.3,20000filas/10MB y recursos existentes. E2E real necesita despliegue compatible y scope propio; no se ejecutó en esta entrega. No se publica una fuente RA desde el diálogo ni se altera SII, CSV o almacenamiento documental.

## Asistente autorizado para altas

D-16/RQ-16 añade análisis multipart sin plantilla ni efectos durables, con propuestas revisables y File sólo en memoria. Conserva los contratos del motor y el editor manual; no necesita migración SQL. [Implementación, límites y evidencia actual](../Validacion/doc126-asistente-20261124.md). Las nuevas rutas Compile/Content y la decisión de evitar staging durable figuran en design.md de OpenSpec.

## Guía por semáforo — 2026-11-24, video 191328

56/61 tareas completas; pendientes 6.9, 6.10, 6.11, 7.5 y 8.8. Regresión 178/178 y comprobación final de UI 39/39; 4603 checks nativos sintéticos. Assets doc126-5 contrastados por HTTP anónimo, sin cambiar DLL, SQL ni ejecutar E2E autenticada. La misma plantilla se reutiliza: Nueva importación consulta CREAR/AGREGAR vigente; Cambiar operación edita el mismo ID con CAS y guardado explícito. Reemplazo conserva consentimiento. Colores con texto y siguiente acción, sin confirmación automática. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-semaforo-reutilizacion-20261124.md`.

## Rollback y concurrencia reales — 2026-11-24

6.11 completada:58/62 tareas. E2E real PASS, dos casos: rollback conserva íntegramente1000 filas; concurrencia conserva las1001 anteriores y sólo una fila externa adicional (total1002), con cero filas propias confirmadas. Actor observó INSERT bloqueado y locks activos antes de confirmar.27 tablas protegidas, configuración y dos resultados positivos previos intactos; locks liberados.41/41 pruebas Node y4607 checks sintéticos. Sin instalación/limpieza/permisos nuevos. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-e2e-rollback-concurrencia-20261124.md`.

## Asistente y tres modos reales — 2026-11-25

6.10 y8.8 completadas:60/62 tareas. E2E real del asistente PASS: una plantilla nueva/dos campos, CREAR1000 filas y29 tablas protegidas con huellas intactas. Combinada con AGREGAR/REEMPLAZAR anteriores acredita1000 filas por cada modo; rollback/concurrencia ya aprobados. Se conservan permisos, históricos y cuatro resultados previos; sin SQL de instalación, limpieza o cambio productivo.43/43 regresiones de adaptadores PASS. Pendientes6.9 (cuentas exclusiva/restringida) y7.5 (cierre de gobierno). Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-e2e-asistente-real-20261125.md`.
