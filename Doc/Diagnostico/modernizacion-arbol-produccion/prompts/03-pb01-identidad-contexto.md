# Prompt PB01 para validar identidad y contexto de las operaciones

**Estado:** borrador de implementación. Requiere PB00; decisiones D01, D08, D09, D10, D14 y alcance de D18 aprobados para contexto explícito. Si D08 conserva solo sesión, este diseño debe revisarse.

## Objetivo

Actúa como arquitecto y desarrollador de ASP.NET WebForms/VB.NET. Lee [el contexto común](contexto-comun.md) y el inventario PB00. Implementa el contrato mínimo para que las rutas aprobadas resuelvan un destino explícito, autorizado y correlacionado, sin depender exclusivamente de la última selección mutable de sesión.

## Áreas previstas

Revisa `Gestion/WebFormProducionDocumental.aspx.vb`, `webservice/WebServiceProducion.asmx.vb`, `Gestion/Class_ra_pro_niveles_has_expediente_archivo.vb`, las validaciones de autoridad del módulo y sus consumidores JavaScript. Añade DTO/servicio/adaptador/repositorio solo si corresponde a la estructura real del proyecto; revisa inclusiones del `.vbproj`.

## Contrato a concretar

1. Define una representación tipada de nivel o aparición de expediente. Para nivel, referencia válida de nivel; para expediente, relación + nivel + expediente. Rechaza mezclas, referencias vacías, inválidas, desbordadas y relaciones que ya no corresponden al recurso.
2. Acuerda rangos con todos los consumidores legacy y evita conversiones silenciosas. Si el transporte usa cadenas decimales para IDs, normalízalas con una regla única y verifica el rango antes de convertir. No amplíes `Integer` a `UInteger` solo porque importación utilice otro tipo.
3. Obtén usuario/empresa de autoridad autenticada y valida recurso, ámbito, visibilidad aplicable y permisos específicos. La relación enviada debe comprobarse contra datos del servidor. No uses alias ni la primera relación encontrada como identidad.
4. Concreta correlación de página/solicitud y, si se adopta versión de contexto, quién la emite y valida y cómo caduca. Un contador del cliente no es prueba de autoridad. Documenta protección de solicitudes según el patrón del módulo.
5. Define resultado de lectura/validación con identidad normalizada, correlación y errores funcionales. Si contiene capacidades, explica su vigencia y deja la revalidación en cada escritura. No serialices mensajes SQL, cadenas de conexión o excepciones crudas.

## Integración de selección y compatibilidad

Implementa el punto de integración de selección acordado en PB00, manteniendo compatibles los contratos públicos existentes. La selección de interfaz puede actualizar sesión legacy cuando se necesite, pero el adaptador no debe considerar esa actualización una autorización durable para otra petición.

Para acciones posteriores, expón una entrada interna reutilizable que permita validar y ejecutar sobre el mismo destino en la misma petición. PB02 conectará sus operaciones. No publiques un endpoint genérico para invocar cualquier método por nombre ni cambies masivamente los ASMX existentes.

Define limpiar selección de forma coherente con campos, variables y estado del servidor que realmente consume el módulo. Preserva cancelación/errores del navegador; no introduzcas escrituras de dominio o auditoría documental para una lectura de contexto. Registra las rutas legacy que siguen pendientes de PB02/PB04.

## Verificación de aceptación

- Identidad inexistente, mezcla de nivel/expediente, relación ajena, referencia fuera de rango y dos alias iguales.
- Usuario/empresa distintos, permiso retirado, nodo oculto cuando corresponde y sesión vencida.
- A/B rápido, solicitud tardía y dos páginas con contextos distintos. Demuestra qué compara el servidor y qué descarta el cliente.
- Limpieza deja las rutas afectadas sin destino anterior utilizable silenciosamente.
- Los consumidores legacy mantienen formato de respuesta o disponen de una adaptación integrada, probada y documentada.

Utiliza pruebas de resolución y autorización con dependencias sustituibles, sin conexiones reales por defecto. Compila el VB.NET y prueba el transporte/contrato integrado; las pruebas estáticas o solo de cliente no acreditan este paquete completo.

## Entrega

Entrega código integrado, pruebas y `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB01-contexto.md`. Incluye firmas definitivas, campos, validaciones, errores, semántica de sesión/correlación, consumidores y rutas todavía sin adaptar. PB02 debe poder consumir el contrato sin inventar otra identidad o autorización.
