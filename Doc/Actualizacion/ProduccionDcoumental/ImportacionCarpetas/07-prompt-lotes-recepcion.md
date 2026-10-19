# Prompt 07 — Lotes y recepción de archivos

## Encargo

Lee guía 00, prompt 03 y diseño 04. Implementa la persistencia operativa y recepción recuperable de archivos de un plan validado por 06. Esta entrega no crea expedientes ni almacena documentos definitivos.

## Trabajo

Dividir la entrega en 07-A (DDL y repositorios de plan/operación a partir de contratos 04 y perfil 02) y 07-B (confirmación/recepción integradas al preflight 06). 07-A no espera a que 06 esté implementado. Un único script operativo y una única implementación de repositorios sirven a ambas fases. La protección HTTP y autorización se aplican también a multipart y reintentos.

- Implementa repositorios de lote, raíz, archivo e intentos/eventos definidos, con transacciones acotadas, consultas parametrizadas y separación de lectura/escritura. Expón contratos que consumirán 08–11; no acoples al HTML.
- Confirmar plan mediante clave de idempotencia y restricción única de ámbito. Guardar configuración efectiva, nivel, inventario original y política completa/parcial. Misma clave con otro payload: conflicto; repetición idéntica: IDs y estado previos. Evita check-then-insert sin exclusión transaccional.
- Recibir cada archivo solo si corresponde al lote autorizado y estado permitido. Rechazar IDs cruzados aunque los nombres coincidan. El contrato de recepción no acepta un gabinete o expediente arbitrario que eluda el plan.
- Escribir en staging controlado mientras se recibe, con identidad física generada, contención de ruta y acceso no público. No concatenar nombre original/ruta relativa para construir la ruta de escritura. Un archivo parcial no se considera RECIBIDO.
- Validar bytes reales, cuotas, formato permitido, huella y completitud. Los límites vienen de configuración aprobada y del servidor; no elevar IIS/ASP.NET globalmente para ocultar un límite. No fingir carga fragmentada si no está implementada.
- Coordinar cierre/renombrado del temporal y registro de recepción. Diseña recuperación de escritura completada sin fila actualizada, fila que referencia temporal ausente y recepción concurrente. No prometer una transacción única entre disco y MySQL.
- Repetir contenido ya confirmado devuelve recepción previa; contenido distinto bajo la misma identidad requiere rechazo/revisión. Proteger cuotas frente a concurrencia y a reintentos contabilizados dos veces.
- No permitir que el cliente fuerce estados. Implementa revisión/exclusión persistida según diseño para que 10 reclame fases de forma segura.

## SQL y entregables

El staging confirmado pertenece a esta capa y conserva el contenido necesario para recuperación. Entregar a 09 una referencia controlada, no autorización implícita para consumir el único original. Su copia de trabajo legacy, huella, nombre/extensión y limpieza son responsabilidades coordinadas con 09/11 según la guía 00.

Entregar `Sql/02-carga-carpetas-mysql51.sql` con DDL completo de entidades operativas, índices y relaciones compatibles. Referenciar las tablas de perfil desplegadas por `Sql/01-perfil-importacion-mysql51.sql`, sin recrearlas. Añadir pre/postcomprobaciones de solo lectura, orden de aplicación, repetición y recuperación parcial. El usuario aplica el SQL. Si ya existe un despliegue, entregar incremento separado.

## Aceptación

Probar doble confirmación simultánea, clave repetida con payload diferente, archivo ajeno, ruta maliciosa, exceder cuota, archivo truncado, dos recepciones concurrentes, desconexión, resultado de recepción perdido y contenido cambiado. Verificar que completo espera todos los archivos de la unidad exigida y que recepción no produce efectos documentales. Las pruebas de transacciones requieren un entorno de pruebas autorizado; reportar lo que solo se probó con dobles.
