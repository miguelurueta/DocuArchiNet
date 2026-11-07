# Diseño de implementación

Entrega documental del prompt 04, basada en el diagnóstico de coherencia. No constituye DDL aplicado ni implementación.

1. [Modelo de datos](01-modelo-datos.md): tablas, tipos, relaciones, índices, snapshots y responsables SQL.
2. [Contratos de operación](02-contratos-operacion.md): apertura, plan, confirmación, recepción, avance, consulta y errores.
3. [Estados y recuperación](03-estados-recuperacion.md): barreras, exclusión, ventanas de fallo y archivos temporales.
4. [Decisiones y pendientes](04-decisiones-y-pendientes.md): evidencia, decisiones confirmadas, propuestas y condiciones de salida.

**Decisión aprobada:** soportar ambos ámbitos configurables en el perfil. En modo completo se debe elegir EXPEDIENTE o LOTE al publicar; en parcial se representa NO_APLICA. El plan y el lote conservan la política efectiva. Esta decisión queda cerrada; los demás pendientes técnicos conservan su estado.
# Contrato complementario

[Orígenes de datos](08-origenes-datos.md): topología multibase confirmada, evidencia RA/DA y límites de SQL/referencias de catálogo.

[HTTP y seguridad del perfil](07-contrato-http-seguridad-perfil.md): entregable técnico de 1.5/1.6, métodos/DTO, sesión/CSRF, errores y recuperación; sin implementación.

[Coincidencias y disponibilidad](06-coincidencias-disponibilidad.md): políticas aprobadas 2026-10-20, límites de cada entrega y pruebas exigidas.

[Fuentes de código, fechas y responsable del expediente](05-contrato-fuentes-expediente.md): matriz de DOC-110 / tarea 1.3, definición técnica para revisión. No acredita implementación.


[Proveedores de datos para índices](09-proveedores-datos-indices.md): AMP-01 / D-08 / RQ-11, contrato extensible, tablas configurables por proveedor y entrega de valores al plan. Estado documental; no acredita implementación.

[Catálogo de proveedores de tabla](10-catalogo-proveedores-tabla.md): selección de conexiones, metadatos, diccionario RA, límites del adaptador MySQL y orden de instalación SQL 04.
# Refinamiento de integración DOC-111

El borrador 2026-10-27 incorpora el [inventario exacto de implementación](design.md) y la [precisión de integración con DOC-110](15-integracion-carga-doc111.md). Conserva modelo/operaciones/estados existentes y propietarios 04–12; no acredita código o esquema operativo ni aprobación. La selección actual de DOC-110 es administrativa y requiere una vía operativa autorizada sin elevar esa facultad.

## Estado validado DOC-111 — 2026-11-02

El corte anterior es histórico. El alcance 04–12 quedó implementado y validado con 130/130 tareas: REC EXPEDIENTE, LOTE y PARCIAL, degradación del esquema operativo y carga individual reales. La [matriz final](../Validacion/doc111-matriz-final-20261102.md) separa evidencia real/local y límites, incluida preservación SII estática/local. El cambio está archivado en OpenSpec y la especificación principal quedó sincronizada; los recursos de pruebas se conservan.
