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
