# Resultado parcial de P01 de radicación interna

Fecha: 2026-11-26. Estado: **en refinamiento, no aceptado todavía**. La instrucción de continuar inició P01 y permitió concretar trabajo independiente; no implementó P02 ni la aplicación. Falta cerrar G01 y sus consecuencias documentales. La pregunta sobre expediente previo o clasificación posterior está enviada al usuario; no se tomó el silencio como elección.

## Entrega documental

- [Contratos v1](contratos-v1.md): límites, entidades, transporte, autoridad, revisiones, operaciones y persistencia propia propuesta.
- [Decisiones](decisiones.md): D-RI vigentes y puertas G01–G11 con responsables/paquetes, sin aprobaciones inventadas.
- [Inventario](inventario-archivos.md): rutas concretas, clases/responsabilidades, funciones existentes y unidades de trabajo por subfase.
- [Matriz de caracterización](matriz-caracterizacion.md): 18 casos del núcleo, metadatos, recepción, identidad, concurrencia y consumidores.

Se localizó el menú real en `Defaul/ClassGestorSesion.vb`, `Solicita_items_modulo_gestion`; el acceso actual de Producción usa el nodo `GD-GD-14`. El nuevo acceso necesita su permiso/código de nodo definidos en P02/G03; no se agregó ni se reutilizó ese código.

Se revisaron patrones de recepción, request guard y contexto de importación de carpetas como referencias de diseño, y el script de perfil MySQL local. No se reutiliza un lote/importación para radicar, no se modifica DOC-127 y no se afirma que el ambiente tenga el esquema propuesto.

## Avance por tarea

| Tarea | Resultado y límite |
|---|---|
| RI00.1 | Aprobaciones preservadas. G01 pendiente; variante sin expediente no se habilita ni se supone viable tocando el motor. |
| RI00.2 | Contrato técnico v1 y responsabilidades mínimas documentados; capacidades nuevas dependen de sus puertas funcionales. |
| RI00.3 | Estado/persistencia propios propuestos y patrón local inventariado. DDL, motor/versiones y esquema de ambiente no verificados ni aplicados. |
| RI00.4 | Núcleo/efectos trazados en diagnósticos y matriz C01–C18 preparada. Caracterización funcional previa/posterior no ejecutada por ser un paquete documental sin ambiente autorizado. |
| RI00.5 | Matriz de autoridad y catálogos/menú situada en contratos/inventario; decisiones G02–G11 asignadas a sus paquetes. Capacidad de markup no se declara operativa por existir el control. |
| RI00.6 | Rutas, clases y unidades de entrega/verificación concretadas. Registrar versión de contrato si las decisiones pendientes cambian consumidores. |

## Verificación ejecutada

```powershell
node tools/diagnostico/verify-radicacion-storage-freeze.cjs
node --test tests/bootstrap-table-global-contract.test.cjs tests/doc90-radicacion-classic-attachment.test.cjs tests/radicacion-simple-attachment-upload-signatures.test.cjs tests/workflow-user-send.test.cjs
```

El guard pasó: archivo central y 24 implementaciones protegidas en 10 archivos sin diferencias. Las cuatro suites locales pasaron **26 comprobaciones**, sin fallos. Son comprobaciones de contrato/focales existentes; no prueban la nueva transacción, los endpoints propuestos, almacenamiento real ni la UI completa en IIS.

La validación documental comprobó los cinco entregables, siete decisiones D-RI, once puertas G, 18 casos de caracterización y las seis huellas de prompts sin cambios. La revisión de vínculos del módulo completo pasó sobre 20 documentos y 172 enlaces locales. Los endpoints/control propuestos siguen sin existir; esta comprobación evita presentarlos como implementados.

No se modificaron aplicación, almacenamiento, configuración, esquema ni permisos. No se ejecutaron MSBuild, migraciones, consultas autenticadas, radicación, upload real, correo o E2E de ambiente. Los nombres de archivos nuevos en el inventario son propuestas; no se crearon modelos/DTOs/servicios de aplicación en P01.

## Próximo punto de refinamiento

Cerrar G01 y ajustar la secuencia documental compatible con D-RI-07. Después verificar las precondiciones de P02 por subfase, conservando la autoridad pendiente de G03 y la evidencia necesaria antes del cambio funcional. La falta de acuerdo no impide revisar contratos y diseñar pruebas independientes, pero no permite dar por aceptado P01 ni activar carga/radicación.
