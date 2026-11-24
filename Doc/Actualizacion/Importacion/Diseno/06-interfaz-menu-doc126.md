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
