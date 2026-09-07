# Flujo de integración

- Ticket: DOC-53
- Cambio OpenSpec: doc-53-orquestacion-estados
- Clasificacion: cross_cutting

1. Revalidar usuario, permiso, tarea, ruta, trámite y proveedor.
2. Obtener intención y comparar `VersionToken`.
3. Persistir `Validada` antes de efectos.
4. Ejecutar por elemento: recurso, expediente, índices, almacenamiento y caché.
5. Persistir cada transición mediante comparación optimista.
6. Clasificar fallo conocido como previo/parcial, o no demostrable como `ResultadoIncierto`.
7. Detener únicamente entre unidades; conservar confirmados y no iniciar pendientes.
8. Consultar siempre la instantánea persistida mediante Get.

Cada paso mutador confirma solo su propia frontera local. No existe ni se promete una transacción distribuida.
