# Prompt PB03 para aportar lecturas de contexto cuando sean necesarias

**Estado:** borrador opcional de implementación. Requiere PB00/PB01 y sus decisiones; D05, D06, D09 y D16 aprobadas según los datos solicitados.

## Objetivo

Actúa como arquitecto de ASP.NET WebForms y consultas MySQL. Lee [el contexto común](contexto-comun.md) y determina qué información necesita realmente PF02 que no esté ya disponible de forma autorizada. Implementa solo esas lecturas. Si no hace falta backend adicional, documenta la omisión y el origen de cada dato del cliente.

## Inventario de necesidades

Para ruta, contenido directo, capacidades e indicadores registra consumidor, fuente existente, vigencia, identidad y motivo para consultar. No uses los números o permisos simulados de la maqueta como datos de producción.

El contenido de un nivel es su conjunto directo de subniveles y expedientes según D05. No conviertas esta lectura en agregación recursiva de documentos. La búsqueda aprobada sobre nodos cargados no necesita automáticamente un buscador global.

## Si se requieren nuevas lecturas

1. Reutiliza identidad y autoridad PB01. El usuario y la empresa no se aceptan como autoridad solo porque lleguen en la petición.
2. Define operaciones acotadas para las necesidades aprobadas. No expongas SQL libre, nombres de tablas ni un ejecutor genérico de métodos.
3. Valida tipo/referencia y aplica visibilidad y reglas de acceso antes de devolver nombres, rutas, hijos o capacidades. No reveles un elemento ajeno para explicar por qué está denegado.
4. Parametriza consultas y ajusta tipos, collation y sintaxis al motor confirmado. No añadas esquema, índices o versiones de base de datos por suposición.
5. Evita consultas por cada nodo y carga repetida de toda la jerarquía. Si la respuesta es parcial, limita su alcance de forma explícita y proporciona una navegación compatible; no inventes totales completos a partir de una página.
6. Define ausencia, vacío, denegación y fallo con resultados distintos y controlados. Incluye la identidad y correlación necesarias para rechazar respuestas tardías en PF02.
7. Las capacidades se calculan por operación con sus reglas vigentes. Distingue no aplicable, no autorizado y no disponible cuando el contrato lo requiera. Cada escritura mantiene revalidación en servidor; un permiso devuelto antes no autoriza ejecución futura.
8. Contadores deben indicar si miden hijos directos, nodos disponibles o total completo. No consultes cantidad de documentos por expediente solo para decorar el árbol.

## Límites de efectos

Estas lecturas no cambian documentos, estados, relaciones, permisos ni auditoría documental. No marcan tareas, importaciones o expedientes como vistos/procesados salvo una decisión de negocio propia. Si se mantiene sesión de interfaz por compatibilidad, documenta ese efecto y no lo confundas con una escritura de dominio.

No sustituyas el cargador documental ni el adaptador de importación para proporcionar ruta o capacidades del explorador. Si el contrato aprobado no necesita datos nuevos, termina con omisión justificada en lugar de crear servicios vacíos.

## Verificación de aceptación

- Nivel vacío y mixto; expediente usado como nivel; relación inválida.
- Propietario y receptor con capacidades diferentes; revocación de permiso entre lectura y escritura.
- Rama oculta, usuario/empresa ajenos, ruta inválida y ausencia de recurso.
- Identidad de respuesta y solicitud desfasadas; fallo de consulta distinto de contenido vacío.
- Cantidad de consultas y tamaño de respuesta acordes con el contrato; sin consulta por insignia ni carga global innecesaria.

Ejecuta pruebas locales de autorización, consultas parametrizadas y forma de respuesta, con sustituciones o datos aislados. Una inspección de base real necesita autorización aplicable y solo SELECT de control.

## Entrega

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB03-lecturas.md`. Si se implementa, incluye contratos, origen/vigencia de datos, consultas, errores, pruebas y compilación. Si se omite, incluye la necesidad evaluada y la fuente existente de cada dato para que PF02 pueda integrarse sin asumir un servicio inexistente.
