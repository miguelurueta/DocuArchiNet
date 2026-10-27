# DOC-110 — Contratos que requieren respuesta y trabajo independiente

Registro 2026-10-20. No cambia el estado draft ni acredita pruebas de implementación.

## Coincidencias: evidencia, no política inventada

`Gestion/ClassGaExpediente.vb:8467`, `Verfica_Existencia_Codigo_Unico_Expediente`, consulta por código, empresa, volumen y área. No busca por nombre visible ni decide reutilización. La llamada observada dentro de Conservación está comentada (`:7652`); no atribuirle una protección activa contra duplicados en ese recorrido.

Consecuencias verificables para la tarea 1.9:

- Distinguir coincidencia de negocio de reintento de una carpeta del mismo lote. Un reintento debe recuperar su relación confirmada, no aplicar nuevamente creación.
- Código de expediente no es identidad global. No reducir la búsqueda a nombre de carpeta ni asumir que un ID físico basta sin su clave completa.
- No deducir de esa consulta una autorización para reutilizar: exige estado, clasificación, nivel, permisos y resultado único comprobados por los adaptadores.
- Una consulta previa no elimina carreras con creación manual ni otros escritores. Mantener la caracterización del adaptador 08 y resultados inciertos del coordinador.

El usuario aprobó BLOQUEAR por defecto y REUTILIZAR_VALIDADO configurable. Contrato cerrado documentalmente en `Diseno/06-coincidencias-disponibilidad.md`; caracterización de identidad real sigue siendo tarea del adaptador 08.

## Desactivación: alcance de la discrepancia

RQ-02 impide nuevas selecciones y conserva historial. La propuesta anterior T11 de detener efectos por desactivar perfil fue sustituida por la decisión aprobada: lotes confirmados continúan, sujetos a permisos vigentes.

El usuario aprobó continuidad de lotes confirmados y bloqueo de nuevas confirmaciones. Detener lote es explícito y pérdida de permiso impide nuevos efectos. Se sincronizan diseño y prompts; cierre documental 1.10, sin implementar control de lotes en el servicio del perfil.

## Permisos: resultado físico

Ver `evidencia-esquema-permiso.json`. Tabla y columnas actuales verificadas; 78 usuarios con una fila cada uno hoy, pero índice no único por usuario. El nuevo adaptador debe rechazar ambigüedad. La ausencia actual de duplicados no justifica seleccionar arbitrariamente la primera fila ni alterar índices legacy.

La columna nueva se entrega en SQL 01 con default 0; no existe en el esquema consultado. La clave de sesión propuesta sigue siendo GA_ADMINISTRAR_PERFILES_IMPORTACION. El usuario confirmó la ubicación en otra aplicación fuera del repo. Se cierra búsqueda local de 1.8; contrato de integración documentado, aceptación externa abierta.

## Entregables independientes preparados

- SQL 01 con tres tablas de configuración y extensión administrativa mínima.
- Procedimiento manual, pre/postcontroles y recuperación de DDL parcial.
- Diccionario físico materializado a partir del modelo común. No se incluyen políticas operativas inventadas dentro de columnas nuevas.

Rutas: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/`. La creación del archivo es trabajo documental completado; su revisión/aprobación, ejecución manual y pruebas reales son hechos diferentes.
