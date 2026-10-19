# Diseño de implementación

Entrega documental del prompt 04, basada en el diagnóstico de coherencia. No constituye DDL aplicado ni implementación.

1. [Modelo de datos](01-modelo-datos.md): tablas, tipos, relaciones, índices, snapshots y responsables SQL.
2. [Contratos de operación](02-contratos-operacion.md): apertura, plan, confirmación, recepción, avance, consulta y errores.
3. [Estados y recuperación](03-estados-recuperacion.md): barreras, exclusión, ventanas de fallo y archivos temporales.
4. [Decisiones y pendientes](04-decisiones-y-pendientes.md): evidencia, decisiones confirmadas, propuestas y condiciones de salida.

**Decisión aprobada:** soportar ambos ámbitos configurables en el perfil. En modo completo se debe elegir EXPEDIENTE o LOTE al publicar; en parcial se representa NO_APLICA. El plan y el lote conservan la política efectiva. Esta decisión queda cerrada; los demás pendientes técnicos conservan su estado.
